using System;
using System.Collections.Generic;

namespace MobaRoguelike.Core.Stats
{
    public class StatSheet
    {
        private static readonly int StatCount = Enum.GetValues(typeof(StatType)).Length;

        private readonly float[] _baseValues = new float[StatCount];
        private readonly float[] _flatSums = new float[StatCount];
        private readonly float[] _percentSums = new float[StatCount];
        private readonly List<StatModifier> _modifiers = new List<StatModifier>();

        public event Action<StatType> OnStatChanged;

        public void SetBase(StatType stat, float value)
        {
            _baseValues[(int)stat] = value;
            OnStatChanged?.Invoke(stat);
        }

        public float GetBase(StatType stat) => _baseValues[(int)stat];

        public float GetFinalValue(StatType stat)
        {
            int i = (int)stat;
            return (_baseValues[i] + _flatSums[i]) * (1f + _percentSums[i]);
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            ApplyModifier(modifier, 1f);
            OnStatChanged?.Invoke(modifier.StatType);
        }

        public void RemoveModifier(int sourceId)
        {
            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                if (_modifiers[i].SourceId == sourceId)
                {
                    ApplyModifier(_modifiers[i], -1f);
                    StatType changed = _modifiers[i].StatType;
                    _modifiers.RemoveAt(i);
                    OnStatChanged?.Invoke(changed);
                }
            }
        }

        private void ApplyModifier(StatModifier modifier, float sign)
        {
            int i = (int)modifier.StatType;
            if (modifier.ModifierType == ModifierType.Flat)
                _flatSums[i] += modifier.Value * sign;
            else
                _percentSums[i] += modifier.Value * sign;
        }
    }
}
