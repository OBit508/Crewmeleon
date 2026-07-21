using Crewmeleon.Components;
using Crewmeleon.Essential;
using Crewmeleon.Roles;
using FungleAPI.Base.Buttons;
using FungleAPI.Components;
using FungleAPI.Hud;
using FungleAPI.Ship;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.Buttons
{
    public class ZoomButton : RoleButton<ChameleonRole>
    {
        public static SpriteRenderer Zoom;
        public static float X = 1;
        public override bool Active => base.Active && ChameleonHelper.PaintState != PaintState.None;
        public override ButtonLocation Location => ButtonLocation.BottomLeft;
        public override Sprite ButtonSprite => ChameleonAssets.Zoom1;
        public override string OverrideText => ChameleonTranslation.Zoom.GetString();
        public override Color32 TextOutlineColor => Color.black;
        public override float Cooldown => 1;
        public override void OnClick()
        {
            GameObject zoom = GetZoom().gameObject;
            zoom.SetActive(!zoom.activeSelf);
            Button.graphic.sprite = zoom.activeSelf ? ChameleonAssets.Zoom2 : ChameleonAssets.Zoom1;
        }
        public SpriteRenderer GetZoom()
        {
            if (Zoom == null)
            {
                Zoom = new GameObject("Zoom")
                {
                    layer = 5,
                    transform =
                {
                    parent = Camera.main.transform,
                    localPosition = Vector3.zero
                }
                }.AddComponent<SpriteRenderer>();
                Zoom.sprite = ChameleonAssets.ZoomBackground;

                SpriteRenderer spriteRenderer = new GameObject("Player")
                {
                    layer = 5,
                    transform =
                {
                    parent = Zoom.transform,
                    localPosition = new Vector3(0, 0, -1),
                    localScale = Vector3.one * 3
                }
                }.AddComponent<SpriteRenderer>();
                CanvaPaintBehaviour canvaPaintBehaviour = spriteRenderer.gameObject.AddComponent<CanvaPaintBehaviour>();
                canvaPaintBehaviour.Canva = CanvaPaintBehaviour.Instance.Canva;
                canvaPaintBehaviour.Paint = () => true;

                GameObject quad = new GameObject("Quad")
                {
                    layer = 5,
                    transform =
                {
                    parent = Zoom.transform,
                    localPosition = new Vector3(0, 0, -1),
                    localScale = new Vector3(5, 5, 5)
                }
                };

                MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = quad.AddComponent<MeshRenderer>();

                Mesh quadMesh = new Mesh
                {
                    name = "Quad"
                };

                quadMesh.vertices = new Vector3[]
                {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3( 0.5f, -0.5f, 0),
                new Vector3(-0.5f,  0.5f, 0),
                new Vector3( 0.5f,  0.5f, 0)
                };

                quadMesh.uv = new Vector2[]
                {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(0,1),
                new Vector2(1,1)
                };

                quadMesh.triangles = new int[]
                {
                0,2,1,
                2,3,1
                };

                quadMesh.RecalculateNormals();
                quadMesh.RecalculateBounds();

                meshFilter.sharedMesh = quadMesh;

                Camera camera = GameObject.Instantiate(ShipPrefabLoader.SkeldPrefab.transform.GetChild(21).GetChild(0).GetChild(1).GetChild(0).GetComponent<SystemConsole>().MinigamePrefab.SafeCast<SurveillanceMinigame>().CameraPrefab, Zoom.transform);
                camera.orthographicSize = 0.58f;

                RenderTexture rt = new RenderTexture(512, 512, 32);
                rt.Create();

                camera.targetTexture = rt;

                Material mat = new Material(Shader.Find("Unlit/Texture"));
                mat.mainTexture = rt;

                meshRenderer.sharedMaterial = mat;

                Zoom.gameObject.AddComponent<Updater>().update = delegate
                {
                    spriteRenderer.flipX = CanvaPaintBehaviour.Instance.Canva.Canva.flipX;
                    spriteRenderer.sprite = CanvaPaintBehaviour.Instance.Canva.Canva.sprite;
                    camera.transform.position = PlayerControl.LocalPlayer.transform.position;
                    if (canvaPaintBehaviour.cursorPreviewObject != null)
                    {
                        canvaPaintBehaviour.cursorPreviewObject.layer = 5;
                    }
                };
                Zoom.gameObject.SetActive(false);
            }
            return Zoom;
        }
    }
}