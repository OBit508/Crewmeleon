using Crewmeleon.Components;
using Crewmeleon.Essential;
using Crewmeleon.RPC;
using FungleAPI.Base.Roles;
using FungleAPI.Extensions;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Translation;
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
        public RoleConfiguration Configuration => new RoleConfiguration(this);

        public SpriteRenderer Chameleon;
        private bool[] paintable;
        public float sendTimer;

        public void Start()
        {
            if (Player == null) return;

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

            Player.cosmetics.ToggleHat(false);
            Player.cosmetics.TogglePetVisible(false);
            Player.cosmetics.ToggleVisor(false);
            Player.cosmetics.ToggleNameVisible(false);
            Player.cosmetics.skin.Visible = false;
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
        }
        public void OnDestroy()
        {
            if (Player == null) return;

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
        }
    }
}
