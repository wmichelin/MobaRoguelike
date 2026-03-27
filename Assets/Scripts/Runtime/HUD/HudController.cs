using UnityEngine;
using MobaRoguelike.Core.Stats;
using MobaRoguelike.Core.Abilities;
using MobaRoguelike.Runtime.Bootstrap;
using MobaRoguelike.Runtime.Abilities;

namespace MobaRoguelike.Runtime.HUD
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private HealthBarView _healthBar;
        [SerializeField] private AbilitySlotView[] _abilitySlotViews = new AbilitySlotView[4];
        [SerializeField] private AbilityCasterBridge _casterBridge;

        private void Start()
        {
            if (GameBootstrap.HeroStats != null)
            {
                GameBootstrap.HeroStats.OnStatChanged += OnStatChanged;
                RefreshHealth();
            }

            if (_casterBridge != null)
            {
                _casterBridge.Caster.OnCooldownChanged += OnCooldownChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameBootstrap.HeroStats != null)
                GameBootstrap.HeroStats.OnStatChanged -= OnStatChanged;

            if (_casterBridge != null)
                _casterBridge.Caster.OnCooldownChanged -= OnCooldownChanged;
        }

        private void OnStatChanged(StatType stat)
        {
            if (stat == StatType.CurrentHealth || stat == StatType.MaxHealth)
                RefreshHealth();
        }

        private void RefreshHealth()
        {
            if (_healthBar == null) return;
            float current = GameBootstrap.HeroStats.GetFinalValue(StatType.CurrentHealth);
            float max = GameBootstrap.HeroStats.GetFinalValue(StatType.MaxHealth);
            _healthBar.UpdateHealth(current, max);
        }

        private void OnCooldownChanged(AbilitySlot slot, AbilityState state)
        {
            int i = (int)slot;
            if (i < _abilitySlotViews.Length && _abilitySlotViews[i] != null)
            {
                AbilityData data = _casterBridge.Caster.GetAbility(slot);
                float maxCooldown = data != null ? data.Cooldown : 1f;
                float fill = state.IsOnCooldown ? state.RemainingCooldown / maxCooldown : 0f;
                _abilitySlotViews[i].UpdateCooldown(fill);
            }
        }
    }
}
