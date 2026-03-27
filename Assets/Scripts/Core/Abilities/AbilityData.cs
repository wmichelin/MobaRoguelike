namespace MobaRoguelike.Core.Abilities
{
    public class AbilityData
    {
        public string Id;
        public float Cooldown;
        public float ManaCost;
        public string DisplayName;
        public IAbilityEffect Effect;
    }
}
