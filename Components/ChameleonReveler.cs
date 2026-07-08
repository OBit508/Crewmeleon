using Crewmeleon.Essential;
using Crewmeleon.Roles;
using FungleAPI.Components;
using FungleAPI.Extensions;
using FungleAPI.GameModes;
using FungleAPI.Role;
using FungleAPI.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crewmeleon.Components
{
    public class ChameleonReveler : PlayerComponent
    {
        public const float MaxProximityLevel = 0.8F;

        public float proximityLevel;
        public float stopedTimer;

        public float updateNearbyImpostorsTime;

        public bool wasReveled;

        public HorizontalGauge Gauge;
        public void Start()
        {
            HorizontalGauge gaugePrefab;
            if (GetMinigamePrefab(TaskTypes.UploadData) != null)
            {
                gaugePrefab = GetMinigamePrefab(TaskTypes.UploadData).TryCast<UploadDataGame>().Gauge;
            }
            else
            {
                gaugePrefab = GetMinigamePrefab(TaskTypes.ProcessData).TryCast<ProcessDataMinigame>().Gauge;
            }
            Gauge = GameObject.Instantiate<HorizontalGauge>(gaugePrefab, player.transform);
            Gauge.transform.localPosition = new Vector3(0, -0.7f, 0);
            Gauge.transform.localScale = new Vector3(0.65f, 1, 1);

            Gauge.gameObject.SetActive(false);
        }
        public void Update()
        {
            if (player.Data.RoleType != CustomRoleManager.GetRoleType<ChameleonRole>()) return;

            updateNearbyImpostorsTime += Time.deltaTime;
            if (updateNearbyImpostorsTime >= 0.05f)
            {
                if (NearbySeekers())
                {
                    proximityLevel += Time.deltaTime;
                    if (proximityLevel > MaxProximityLevel)
                    {
                        proximityLevel = MaxProximityLevel;
                    }

                    if (player.AmOwner)
                    {
                        Gauge.gameObject.SetActive(true);
                        Gauge.Value = proximityLevel / MaxProximityLevel;
                    }
                }
                else
                {
                    proximityLevel = 0;

                    if (player.AmOwner)
                    {
                        Gauge.gameObject.SetActive(false);
                    }
                }

                updateNearbyImpostorsTime = 0;
            }

            UpdateStopTimer();

            bool reveled = stopedTimer < 2.5f || proximityLevel >= MaxProximityLevel;

            player.cosmetics.SetOutline(reveled, new Il2CppSystem.Nullable<Color>(Color.white));

            if (reveled && !wasReveled && !SeekerRole.SafeToKill.Contains(player.Data))
            {
                SeekerRole.SafeToKill.Add(player.Data);
            }
            else if (!reveled && wasReveled && SeekerRole.SafeToKill.Contains(player.Data))
            {
                SeekerRole.SafeToKill.Remove(player.Data);
            }

            wasReveled = reveled;
        }
        public void UpdateStopTimer()
        {
            if (player.rigidbody2D.velocity == Vector2.zero)
            {
                stopedTimer += Time.deltaTime;
            }
            else
            {
                stopedTimer = 0;
            }
        }
        public Minigame GetMinigamePrefab(TaskTypes type)
        {
            return ShipPrefabLoader.SkeldPrefab.GetAllTasks().FirstOrDefault(t => t.TaskType == type).MinigamePrefab;
        }
        public bool NearbySeekers()
        {
            return PlayerControl.AllPlayerControls.Any(p => p.Data.Role.TeamType == RoleTeamTypes.Impostor && Vector2.Distance(p.transform.position, transform.position) <= 1.5f);
        }
        public void OnDestroy()
        {
            if (SeekerRole.SafeToKill.Contains(player.Data))
            {
                SeekerRole.SafeToKill.Remove(player.Data);
            }
        }
    }
}
