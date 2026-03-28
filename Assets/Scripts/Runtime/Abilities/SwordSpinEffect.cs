using System;
using UnityEngine;
using MobaRoguelike.Core.Abilities;

namespace MobaRoguelike.Runtime.Abilities
{
    /// <summary>
    /// AoE spin attack: damages all IDamageable targets within Radius of the caster.
    /// Assign to an AbilityDefinitionSO's Effect field via [SerializeReference].
    /// Stack with CompositeAbilityEffect for roguelike upgrade modifiers.
    /// </summary>
    [Serializable]
    public class SwordSpinEffect : IAbilityEffect
    {
        public float Radius = 4f;
        public float Damage = 50f;

        public void Apply(AbilityContext context)
        {
            var origin = new Vector3(context.CasterPositionX, 0f, context.CasterPositionZ);
            Collider[] hits = Physics.OverlapSphere(origin, Radius);

            foreach (var col in hits)
            {
                if (col.gameObject.GetInstanceID() == context.CasterId) continue;

                if (col.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(Damage, context.CasterId);
            }
        }
    }
}
