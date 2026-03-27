using System;
using UnityEngine;
using UnityEngine.InputSystem;
using MobaRoguelike.Core.Abilities;

namespace MobaRoguelike.Runtime.Input
{
    [UnityEngine.DefaultExecutionOrder(-90)]
    public class InputReader : MonoBehaviour
    {
        public event Action<Vector2> OnMoveInput;
        public event Action<AbilitySlot> OnAbilityInput;
        public event Action OnAttackInput;

        private InputSystem_Actions _actions;

        private void OnEnable()
        {
            _actions = new InputSystem_Actions();

            _actions.Player.Move.performed += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
            _actions.Player.Move.canceled += _ => OnMoveInput?.Invoke(Vector2.zero);

            _actions.Player.Attack.performed += _ => OnAttackInput?.Invoke();

            _actions.Player.AbilityQ.performed += _ => OnAbilityInput?.Invoke(AbilitySlot.Q);
            _actions.Player.AbilityW.performed += _ => OnAbilityInput?.Invoke(AbilitySlot.W);
            _actions.Player.AbilityE.performed += _ => OnAbilityInput?.Invoke(AbilitySlot.E);
            _actions.Player.AbilityR.performed += _ => OnAbilityInput?.Invoke(AbilitySlot.R);

            _actions.Player.Enable();
        }

        private void OnDisable()
        {
            _actions.Player.Disable();
            _actions.Dispose();
            _actions = null;
        }
    }
}
