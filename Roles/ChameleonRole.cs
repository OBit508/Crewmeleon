using Crewmeleon.Components;
using Crewmeleon.Essential;
using FungleAPI.Base.Roles;
using FungleAPI.Extensions;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

            Player.MyPhysics.Animations.group.SpriteAnimator.transform.parent.localScale = Vector3.zero;

            Player.cosmetics.ToggleHat(false);
            Player.cosmetics.TogglePetVisible(false);
            Player.cosmetics.ToggleVisor(false);
            Player.cosmetics.ToggleNameVisible(false);
            Player.cosmetics.skin.Visible = false;
        }
        public void Update()
        {
            if (Chameleon == null) return;

            Chameleon.flipX = Player.cosmetics.FlipX;
        }
        public void OnDestroy()
        {
            if (Player == null) return;

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
