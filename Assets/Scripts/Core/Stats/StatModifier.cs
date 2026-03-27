namespace MobaRoguelike.Core.Stats
{
    public enum ModifierType
    {
        Flat,
        Percent
    }

    public readonly struct StatModifier
    {
        public readonly StatType StatType;
        public readonly float Value;
        public readonly ModifierType ModifierType;
        public readonly int SourceId;

        public StatModifier(StatType statType, float value, ModifierType modifierType, int sourceId)
        {
            StatType = statType;
            Value = value;
            ModifierType = modifierType;
            SourceId = sourceId;
        }
    }
}
