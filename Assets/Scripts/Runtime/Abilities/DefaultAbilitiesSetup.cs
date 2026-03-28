using UnityEngine;
using MobaRoguelike.Core.Abilities;

namespace MobaRoguelike.Runtime.Abilities
{
    /// <summary>
    /// Auto-added by AbilityCasterBridge. Populates default ability loadout in code —
    /// no ScriptableObject assets or Inspector wiring required.
    /// Only fills slots that don't already have an ability assigned (respects SO overrides).
    /// To customize abilities per-run, call Caster.SetAbility() after this runs.
    /// </summary>
    [RequireComponent(typeof(AbilityCasterBridge))]
    public class DefaultAbilitiesSetup : MonoBehaviour
    {
        private void Awake()
        {
            var bridge = GetComponent<AbilityCasterBridge>();

            bridge.Caster.SetAbility(AbilitySlot.Q, new AbilityData
            {
                Id          = "sword_spin",
                DisplayName = "Sword Spin",
                Cooldown    = 1f,
                Effect      = new SwordSpinEffect { Radius = 2f, Damage = 50f }
            });

            bridge.Caster.SetAbility(AbilitySlot.W, new AbilityData
            {
                Id          = "ability_w",
                DisplayName = "Ability W",
                Cooldown    = 10f,
                Effect      = new NoOpAbilityEffect()
            });

            bridge.Caster.SetAbility(AbilitySlot.E, new AbilityData
            {
                Id          = "ability_e",
                DisplayName = "Ability E",
                Cooldown    = 12f,
                Effect      = new NoOpAbilityEffect()
            });

            bridge.Caster.SetAbility(AbilitySlot.R, new AbilityData
            {
                Id          = "ability_r",
                DisplayName = "Ability R",
                Cooldown    = 15f,
                Effect      = new NoOpAbilityEffect()
            });

            if (GetComponent<SwordSpinVFX>() == null)
                gameObject.AddComponent<SwordSpinVFX>();
        }
    }
}
