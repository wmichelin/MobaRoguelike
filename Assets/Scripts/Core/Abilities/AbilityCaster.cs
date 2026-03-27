using System;

namespace MobaRoguelike.Core.Abilities
{
    public class AbilityCaster
    {
        private readonly AbilityData[] _abilities = new AbilityData[4];
        private readonly AbilityState[] _states = new AbilityState[4];

        public event Action<AbilitySlot> OnAbilityCast;
        public event Action<AbilitySlot, AbilityState> OnCooldownChanged;

        public void SetAbility(AbilitySlot slot, AbilityData data)
        {
            _abilities[(int)slot] = data;
        }

        public AbilityData GetAbility(AbilitySlot slot) => _abilities[(int)slot];
        public AbilityState GetState(AbilitySlot slot) => _states[(int)slot];

        public bool TryCast(AbilitySlot slot, AbilityContext context)
        {
            int i = (int)slot;
            if (_abilities[i] == null) return false;
            if (_states[i].IsOnCooldown) return false;

            _states[i] = new AbilityState
            {
                RemainingCooldown = _abilities[i].Cooldown,
                IsOnCooldown = true
            };

            _abilities[i].Effect?.Apply(context);
            OnAbilityCast?.Invoke(slot);
            OnCooldownChanged?.Invoke(slot, _states[i]);
            return true;
        }

        public void Tick(float dt)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!_states[i].IsOnCooldown) continue;

                _states[i].RemainingCooldown -= dt;
                if (_states[i].RemainingCooldown <= 0f)
                {
                    _states[i] = new AbilityState { RemainingCooldown = 0f, IsOnCooldown = false };
                }
                OnCooldownChanged?.Invoke((AbilitySlot)i, _states[i]);
            }
        }
    }
}
