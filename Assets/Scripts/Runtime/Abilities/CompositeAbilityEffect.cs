using System;
using System.Collections.Generic;
using UnityEngine;
using MobaRoguelike.Core.Abilities;

namespace MobaRoguelike.Runtime.Abilities
{
    /// <summary>
    /// Chains multiple IAbilityEffect implementations in sequence.
    /// Use this as the root effect on an AbilityDefinitionSO to stack
    /// base effects with roguelike upgrade modifiers at run-time.
    /// </summary>
    [Serializable]
    public class CompositeAbilityEffect : IAbilityEffect
    {
        [SerializeReference]
        public List<IAbilityEffect> Effects = new List<IAbilityEffect>();

        public void Apply(AbilityContext context)
        {
            foreach (var effect in Effects)
                effect?.Apply(context);
        }
    }
}
