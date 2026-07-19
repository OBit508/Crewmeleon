using Crewmeleon.Components;
using Crewmeleon.Essential;
using Crewmeleon.Roles;
using FungleAPI.Base.Buttons;
using FungleAPI.Components;
using FungleAPI.Hud;
using FungleAPI.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.Buttons
{
    [FungleAPI.Attributes.RegisterPriority(1)]
    public class GradientButton : RoleButton<ChameleonRole>
    {
        public override ButtonLocation Location => ButtonLocation.BottomLeft;
        public override Sprite ButtonSprite => null;
        public override string OverrideText => "Gradiente";
        public override Color32 TextOutlineColor => Color.black;
        public override float Cooldown => 1;
        public override void OnClick()
        {
            if (Minigame.Instance != null) return;

            Minigame.Instance = GameObject.Instantiate(ShipPrefabLoader.SkeldPrefab.transform.Find("TaskAddConsole").GetComponent<SystemConsole>().MinigamePrefab, Camera.main.transform);

            new GameObject("Gradient")
            {
                layer = 5,
                transform =
                {
                    parent = Minigame.Instance.transform,
                    localPosition = new Vector3(0, 0, -1)
                }
            }.AddComponent<SpriteRenderer>().sprite = ChameleonAssets.Gradient;

            Minigame.Instance.timeOpened = Time.realtimeSinceStartup;
            if (PlayerControl.LocalPlayer)
            {
                if (MapBehaviour.Instance)
                {
                    MapBehaviour.Instance.Close();
                }
                PlayerControl.LocalPlayer.MyPhysics.SetNormalizedVelocity(Vector2.zero);
            }
            Minigame.Instance.logger.Info("Opening minigame " + base.GetType().Name, null);
            Minigame.Instance.StartCoroutine(Minigame.Instance.CoAnimateOpen());

            Minigame.Instance.gameObject.AddComponent<Updater>().update = delegate
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ChameleonHelper.BrushColor = ChameleonHelper.GetMousePixelColor();
                }
            };
        }
    }
}
