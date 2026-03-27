using UnityEngine;
using MobaRoguelike.Core.Stats;

namespace MobaRoguelike.Runtime.Bootstrap
{
    [CreateAssetMenu(fileName = "HeroBaseStats", menuName = "MobaRoguelike/HeroBaseStats")]
    public class HeroBaseStatsSO : ScriptableObject
    {
        public StatType[] StatTypes;
        public float[] BaseValues;

        public void ApplyTo(StatSheet sheet)
        {
            if (StatTypes == null) return;
            int count = Mathf.Min(StatTypes.Length, BaseValues.Length);
            for (int i = 0; i < count; i++)
                sheet.SetBase(StatTypes[i], BaseValues[i]);
        }
    }
}
