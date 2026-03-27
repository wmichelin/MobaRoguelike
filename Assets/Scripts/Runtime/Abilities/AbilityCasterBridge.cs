using UnityEngine;
using MobaRoguelike.Core.Abilities;
using MobaRoguelike.Runtime.Input;

namespace MobaRoguelike.Runtime.Abilities
{
    public class AbilityCasterBridge : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private AbilityDefinitionSO[] _abilityDefinitions = new AbilityDefinitionSO[4];

        private AbilityCaster _caster;

        public AbilityCaster Caster => _caster;

        private void Awake()
        {
            _caster = new AbilityCaster();

            for (int i = 0; i < 4 && i < _abilityDefinitions.Length; i++)
            {
                if (_abilityDefinitions[i] != null)
                    _caster.SetAbility((AbilitySlot)i, _abilityDefinitions[i].ToAbilityData());
            }
        }

        private void OnEnable()
        {
            if (_inputReader != null)
                _inputReader.OnAbilityInput += HandleAbilityInput;
        }

        private void OnDisable()
        {
            if (_inputReader != null)
                _inputReader.OnAbilityInput -= HandleAbilityInput;
        }

        private void Update()
        {
            _caster.Tick(Time.deltaTime);
        }

        private void HandleAbilityInput(AbilitySlot slot)
        {
            var ctx = new AbilityContext
            {
                CasterPositionX = transform.position.x,
                CasterPositionZ = transform.position.z,
                CasterId = gameObject.GetInstanceID()
            };
            _caster.TryCast(slot, ctx);
        }
    }
}
