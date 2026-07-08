using Crewmeleon.Components;
using Crewmeleon.Essential;
using Crewmeleon.RPC;
using FungleAPI.Base.Roles;
using FungleAPI.Extensions;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Ship;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Crewmeleon.Roles
{
    public class ChameleonRole : CrewmateBase, ICustomRole
    {
        public static ChameleonPaint Local;

        public ModdedTeam Team { get; } = ModdedTeamManager.Crewmates;
        public StringNames RoleName { get; } = TranslationManager.GetStringName("Camaleão");
        public StringNames RoleBlur { get; } = TranslationManager.GetStringName("Se camufle");
        public StringNames RoleBlurMed { get; } = TranslationManager.GetStringName("Se camufle");
        public StringNames RoleBlurLong { get; } = TranslationManager.GetStringName("Se camufle");
        public Color RoleColor { get; } = Color.gray;
        public RoleConfiguration Configuration => new RoleConfiguration(this)
        {
            HideInLobby = true
        };

        public SpriteRenderer Chameleon;
        private bool[] paintable;
        public float sendTimer;

        public float proximityLevel;
        public float stopedTimer;

        public float updateNearbyImpostorsTime;

        public bool wasReveled;
        public bool timeUp;

        public HorizontalGauge Gauge;

        public void Start()
        {
            if (Player == null) return;

            Player.MyPhysics.Speed = 1.5f;

            Chameleon = new GameObject("Chameleon")
            {
                layer = Player.gameObject.layer,
                transform =
                {
                    parent = Player.transform,
                    localPosition = Vector3.zero,
                    localScale = Vector3.one
                }
            }.AddComponent<SpriteRenderer>();

            Chameleon.material = new Material(Shader.Find("Sprites/Outline"));
            Chameleon.material.SetColor("_OutlineColor", Color.white);

            SpriteRenderer spriteRenderer = Player.MyPhysics.Animations.group.SpriteAnimator.m_nodes.m_spriteRenderer;

            Sprite original = ChameleonHelper.DefaultIdle;

            Chameleon.sprite = Sprite.Create(
                ChameleonHelper.ApplyMaterial(original.texture, spriteRenderer.material),
                original.rect,
                original.pivot / original.rect.size,
                original.pixelsPerUnit,
                0,
                SpriteMeshType.FullRect
            );

            if (Player.AmOwner)
            {
                Local = Chameleon.gameObject.AddComponent<ChameleonPaint>();
            }
            else
            {
                Color[] pixels = Chameleon.sprite.texture.GetPixels();
                paintable = new bool[pixels.Length];

                for (int i = 0; i < pixels.Length; i++)
                {
                    paintable[i] = pixels[i].a > ChameleonPaint.minAlphaThreshold;
                }
            }

            Player.MyPhysics.Animations.group.SpriteAnimator.transform.parent.localScale = Vector3.zero;

            HorizontalGauge gaugePrefab;
            if (GetTaskPrefab(TaskTypes.UploadData) != null)
            {
                gaugePrefab = GetTaskPrefab(TaskTypes.UploadData).MinigamePrefab.TryCast<UploadDataGame>().Gauge;
            }
            else
            {
                gaugePrefab = GetTaskPrefab(TaskTypes.ProcessData).MinigamePrefab.TryCast<ProcessDataMinigame>().Gauge;
            }
            Gauge = GameObject.Instantiate<HorizontalGauge>(gaugePrefab, Player.transform);
            Gauge.transform.localPosition = new Vector3(0, -0.7f, 0);
            Gauge.transform.localScale = new Vector3(0.65f, 1, 1);

            Gauge.gameObject.SetActive(false);
        }
        public void PaintBrush(Vector2Int center, Color32 brushColor)
        {
            int radius = ChameleonPaint.brushSize;

            Texture2D texture = Chameleon.sprite.texture;

            int minX = Mathf.Max(0, center.x - radius);
            int maxX = Mathf.Min(texture.width - 1, center.x + radius);
            int minY = Mathf.Max(0, center.y - radius);
            int maxY = Mathf.Min(texture.height - 1, center.y + radius);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Vector2Int pixel = new(x, y);
                    float distance = Vector2Int.Distance(pixel, center);

                    if (distance > radius)
                    {
                        continue;
                    }

                    int index = y * texture.width + x;

                    if (!paintable[index])
                    {
                        continue;
                    }

                    Color32 current = texture.GetPixel(x, y);

                    float strength = 1f;

                    if (ChameleonPaint.brushSoftness > 0f)
                    {
                        float edge = radius * (1f - ChameleonPaint.brushSoftness);

                        if (distance > edge)
                        {
                            strength = 1f - Mathf.InverseLerp(edge, radius, distance);
                        }
                    }

                    Color32 result = Color.Lerp(current, brushColor, strength * brushColor.a);
                    result.a = current.a;

                    texture.SetPixel(x, y, result);
                }
            }
        }
        public void Update()
        {
            if (Chameleon == null) return;

            if (!timeUp)
            {
                Player.cosmetics.ToggleHat(false);
                Player.cosmetics.TogglePetVisible(false);
                Player.cosmetics.ToggleVisor(false);
                Player.cosmetics.ToggleNameVisible(false);
                Player.cosmetics.skin.Visible = false;

                Chameleon.flipX = Player.cosmetics.FlipX;

                if (Player.AmOwner)
                {
                    sendTimer += Time.deltaTime;
                    if (sendTimer >= 0.15f)
                    {
                        if (Local != null && Local.PaintStrokes.Count > 0)
                        {
                            Rpc<RpcSyncBody>.Instance.Send(Player.Data);
                        }
                        sendTimer = 0;
                    }
                }

                updateNearbyImpostorsTime += Time.deltaTime;
                if (updateNearbyImpostorsTime >= 0.05f)
                {
                    if (NearbySeekers())
                    {
                        proximityLevel += Time.deltaTime;
                        if (proximityLevel > ChameleonModeSettings.ChameleonSettings.FoundBar.FloatValue)
                        {
                            proximityLevel = ChameleonModeSettings.ChameleonSettings.FoundBar.FloatValue;
                        }

                        if (Player.AmOwner)
                        {
                            Gauge.gameObject.SetActive(true);
                            Gauge.Value = proximityLevel / ChameleonModeSettings.ChameleonSettings.FoundBar.FloatValue;
                        }
                    }
                    else
                    {
                        proximityLevel = 0;

                        if (Player.AmOwner)
                        {
                            Gauge.gameObject.SetActive(false);
                        }
                    }

                    updateNearbyImpostorsTime = 0;
                }

                UpdateStopTimer();

                bool reveled = stopedTimer < ChameleonModeSettings.ChameleonSettings.StopOutline.FloatValue || proximityLevel >= ChameleonModeSettings.ChameleonSettings.FoundBar.FloatValue;

                Chameleon.material.SetFloat("_Outline", (reveled ? 1 : 0));

                if (reveled && !wasReveled && !SeekerRole.SafeToKill.Contains(Player.Data))
                {
                    SeekerRole.SafeToKill.Add(Player.Data);
                }
                else if (!reveled && wasReveled && SeekerRole.SafeToKill.Contains(Player.Data))
                {
                    SeekerRole.SafeToKill.Remove(Player.Data);
                }

                wasReveled = reveled;
            }
        }
        public void OnDestroy()
        {
            if (Player == null) return;

            Player.MyPhysics.Speed = 2.5f;

            if (Player.AmOwner)
            {
                ChameleonHelper.PaintState = PaintState.None;
            }

            Player.MyPhysics.Animations.group.SpriteAnimator.transform.parent.localScale = Vector3.one;

            Chameleon?.gameObject.Destroy();

            Player.cosmetics.ToggleHat(true);
            Player.cosmetics.TogglePetVisible(true);
            Player.cosmetics.ToggleVisor(true);
            Player.cosmetics.ToggleNameVisible(true);
            Player.cosmetics.skin.Visible = true;

            if (SeekerRole.SafeToKill.Contains(Player.Data))
            {
                SeekerRole.SafeToKill.Remove(Player.Data);
            }
        }
        public void UpdateStopTimer()
        {
            if (Player.rigidbody2D.velocity == Vector2.zero)
            {
                stopedTimer += Time.deltaTime;
            }
            else
            {
                stopedTimer = 0;
            }
        }
        public void Reveal()
        {
            Player.moveable = false;

            ArrowBehaviour arrowBehaviour = GameObject.Instantiate(GetTaskPrefab(TaskTypes.FixWiring).SafeCast<NormalPlayerTask>().Arrow);
            arrowBehaviour.gameObject.SetActive(true);
            arrowBehaviour.MaxScale = 0.5f;
            arrowBehaviour.target = Player.transform.position;

            timeUp = true;
            Chameleon.material.SetFloat("_Outline", 1);
        }
        public PlayerTask GetTaskPrefab(TaskTypes type)
        {
            return ShipPrefabLoader.SkeldPrefab.GetAllTasks().FirstOrDefault(t => t.TaskType == type);
        }
        public bool NearbySeekers()
        {
            return PlayerControl.AllPlayerControls.Any(p => p.Data.Role.TeamType == RoleTeamTypes.Impostor && Vector2.Distance(p.transform.position, Player.transform.position) <= ChameleonModeSettings.ChameleonSettings.Proximity.FloatValue);
        }
    }
}
