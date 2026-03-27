using UnityEngine;
using MobaRoguelike.Core.Abilities;

namespace MobaRoguelike.Runtime.Abilities
{
    [CreateAssetMenu(fileName = "AbilityDefinition", menuName = "MobaRoguelike/AbilityDefinition")]
    public class AbilityDefinitionSO : ScriptableObject
    {
        public string Id;
        public float Cooldown;
        public float ManaCost;
        public string DisplayName;
        public Sprite Icon;

        [SerializeReference]
        public IAbilityEffect Effect;

        public AbilityData ToAbilityData()
        {
            return new AbilityData
            {
                Id = Id,
                Cooldown = Cooldown,
                ManaCost = ManaCost,
                DisplayName = DisplayName,
                Effect = Effect ?? new NoOpAbilityEffect()
            };
        }
    }
}
