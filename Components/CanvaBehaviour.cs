using AmongUs.Data;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Crewmeleon.Buttons;
using Crewmeleon.Essential;
using Crewmeleon.GameMode;
using Crewmeleon.Roles;
using Crewmeleon.RPC;
using FungleAPI.Components;
using FungleAPI.Hud;
using FungleAPI.Networking;
using FungleAPI.Role;
using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.Components
{
    public class CanvaBehaviour : PlayerComponent
    {
        public Dictionary<Vector2Int, PaintStroke> Strokes = new Dictionary<Vector2Int, PaintStroke>();

        public bool[] Paintable;
        public Color32[] TextureBuffer;
        public SpriteRenderer Canva;

        public bool OutlineActive;
        public bool ForceShow;

        public RoleTypes ChameleonRole = CustomRoleManager.GetRoleType<ChameleonRole>();

        public float SendTimer;
        public void Start()
        {
            StartCoroutine(CoInitialize().WrapToIl2Cpp());
        }
        public void Update()
        {
            if (Canva == null) return;

            Canva.gameObject.SetActive(IsCanvaActive());
            Canva.flipX = player.cosmetics.FlipX;

            SendTimer += Time.deltaTime;
            if (SendTimer >= 0.2f)
            {
                if (Strokes.Count > 0)
                {
                    Rpc<RpcSyncBody>.Instance.Send(player);
                }
                SendTimer = 0;
            }

            if (Canva.gameObject.activeSelf)
            {
                Canva.material.SetFloat("_Outline", OutlineActive ? 1 : 0);
            }

            if (CanvaPaintBehaviour.Instance == null && player.AmOwner)
            {
                CanvaPaintBehaviour.Instance = Canva.gameObject.AddComponent<CanvaPaintBehaviour>();
                CanvaPaintBehaviour.Instance.Canva = this;
                CanvaPaintBehaviour.Instance.Paint = () => ZoomButton.Zoom == null || !ZoomButton.Zoom.gameObject.activeSelf;
            }
        }
        public bool IsCanvaActive()
        {
            if (ForceShow)
            {
                return true;
            }

            if (player.Data.RoleType != ChameleonRole)
            {
                return false;
            }

            if (PlayerControl.LocalPlayer.Data.Role.TeamType == RoleTeamTypes.Impostor)
            {
                return true;
            }

            if (!player.AmOwner && PlayerControl.LocalPlayer.Data.RoleType == ChameleonRole && ChameleonModeSettings.InfectionSettings.Infection.BooleanValue && ChameleonModeSettings.InfectionSettings.HidePlayers)
            {
                return false;
            }
            return true;
        }
        public System.Collections.IEnumerator CoInitialize()
        {
            while (GameData.Instance == null && player.Data == null && player.Data.IsIncomplete)
            {
                yield return null;
            }
            int colorId = -1;
            while (colorId < 0)
            {
                colorId = player.Data.DefaultOutfit.ColorId;
                yield return null;
            }

            Canva = new GameObject("Canva")
            {
                layer = player.gameObject.layer,
                transform =
                {
                    parent = player.transform,
                    localPosition = Vector3.zero,
                    localScale = Vector3.one
                }
            }.AddComponent<SpriteRenderer>();

            Canva.material = new Material(Shader.Find("Sprites/Outline"));
            Canva.material.SetColor("_OutlineColor", Color.white);

            ResetTexture(colorId);

            Canva.gameObject.SetActive(false);
        }
        public void ResetTexture(int colorId)
        {
            Material playerM = new Material(Shader.Find("Unlit/PlayerShader"));
            PlayerMaterial.SetColors(colorId, playerM);

            Sprite original = ChameleonAssets.DefaultIdle;

            Sprite sp = Sprite.Create(
                ChameleonHelper.ApplyMaterial(original.texture, playerM),
                original.rect,
                original.pivot / original.rect.size,
                original.pixelsPerUnit,
                0,
                SpriteMeshType.FullRect
            );

            Canva.sprite = sp;

            Texture2D texture = sp.texture;
            Color[] pixels = texture.GetPixels();
            Paintable = new bool[pixels.Length];

            for (int i = 0; i < pixels.Length; i++)
            {
                Paintable[i] = pixels[i].a > 0;
            }

            TextureBuffer = texture.GetPixels32();
        }
        public bool PaintBrush(Vector2Int center, Color32 color, int brushSize)
        {
            int radiusSqr = brushSize * brushSize;

            int minX = Mathf.Max(0, center.x - brushSize);
            int maxX = Mathf.Min(Canva.sprite.texture.width - 1, center.x + brushSize);
            int minY = Mathf.Max(0, center.y - brushSize);
            int maxY = Mathf.Min(Canva.sprite.texture.height - 1, center.y + brushSize);

            bool paintedAny = false;

            for (int y = minY; y <= maxY; y++)
            {
                int dySqr = (y - center.y) * (y - center.y);
                int rowOffset = y * Canva.sprite.texture.width;

                for (int x = minX; x <= maxX; x++)
                {
                    int dx = x - center.x;
                    if (dx * dx + dySqr > radiusSqr)
                    {
                        continue;
                    }

                    int index = rowOffset + x;
                    if (!Paintable[index])
                    {
                        continue;
                    }

                    TextureBuffer[index] = color;
                    paintedAny = true;
                }
            }

            if (player.AmOwner)
            {
                Strokes[center] = new PaintStroke(color, (byte)brushSize);
            }

            return paintedAny;
        }
    }
}
