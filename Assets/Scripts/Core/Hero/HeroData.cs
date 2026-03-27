using MobaRoguelike.Core.Stats;

namespace MobaRoguelike.Core.Hero
{
    public class HeroData
    {
        public int Level { get; private set; } = 1;
        public float Experience { get; private set; } = 0f;
        public StatSheet Stats { get; }

        public HeroData(StatSheet stats)
        {
            Stats = stats;
        }

        public void AddExperience(float amount)
        {
            Experience += amount;
        }
    }
}
