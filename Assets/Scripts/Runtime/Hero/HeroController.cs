using System;
using UnityEngine;
using UnityEngine.AI;
using Unity.Profiling;
using MobaRoguelike.Runtime.Input;
using MobaRoguelike.Runtime.HUD;

namespace MobaRoguelike.Runtime.Hero
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        private NavMeshAgent _agent;
        private Animator _animator;
        private int _speedHash;
        private Vector2 _moveInput;
        private float _moveSpeed = 8f;
        private static readonly Quaternion s_IsometricRotation = Quaternion.Euler(0f, 45f, 0f);

        private const float DashDistance = 4f;
        private const float DashDuration = 0.15f;
        private const float DashCooldown = 2f;

        private bool _isDashing;
        private float _dashElapsed;
        private Vector3 _dashDirection;
        private float _dashSpeed;
        private float _dashCooldownRemaining;
        private bool _dashOnCooldown;

        public event Action<float, float> OnDashCooldownChanged;

        private static readonly ProfilerMarker s_UpdateMarker =
            new ProfilerMarker("HeroController.Update");

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            _speedHash = Animator.StringToHash("Speed");

            _agent.updatePosition = false;
            _agent.updateRotation = false;

            if (GetComponent<DashCooldownBar>() == null)
                gameObject.AddComponent<DashCooldownBar>();
        }

        private void Start()
        {
            _agent.Warp(transform.position);
        }

        private void OnEnable()
        {
            if (_inputReader != null)
            {
                _inputReader.OnMoveInput += HandleMoveInput;
                _inputReader.OnDashInput += HandleDashInput;
            }
        }

        private void OnDisable()
        {
            if (_inputReader != null)
            {
                _inputReader.OnMoveInput -= HandleMoveInput;
                _inputReader.OnDashInput -= HandleDashInput;
            }
        }

        private void HandleMoveInput(Vector2 dir)
        {
            _moveInput = dir;
        }

        private void HandleDashInput()
        {
            if (_isDashing || _dashOnCooldown) return;

            if (_moveInput.sqrMagnitude > 0.01f)
            {
                _dashDirection = (s_IsometricRotation * new Vector3(_moveInput.x, 0f, _moveInput.y)).normalized;
            }
            else
            {
                _dashDirection = transform.forward;
            }

            _isDashing = true;
            _dashElapsed = 0f;
            _dashSpeed = DashDistance / DashDuration;
        }

        private void Update()
        {
            using (s_UpdateMarker.Auto())
            {
                if (_isDashing)
                {
                    _agent.Move(_dashDirection * (_dashSpeed * Time.deltaTime));
                    _dashElapsed += Time.deltaTime;

                    if (_dashElapsed >= DashDuration)
                    {
                        _isDashing = false;
                        _dashOnCooldown = true;
                        _dashCooldownRemaining = DashCooldown;
                        OnDashCooldownChanged?.Invoke(_dashCooldownRemaining, DashCooldown);
                    }
                }
                else if (_moveInput.sqrMagnitude > 0.01f)
                {
                    Vector3 worldDir = s_IsometricRotation * new Vector3(_moveInput.x, 0f, _moveInput.y);
                    Vector3 move = worldDir.normalized * (_moveSpeed * Time.deltaTime);
                    _agent.Move(move);
                    transform.rotation = Quaternion.LookRotation(worldDir.normalized);
                }

                transform.position = _agent.nextPosition;

                float speed = _isDashing ? _dashSpeed : (_moveInput.sqrMagnitude > 0.01f ? _moveSpeed : 0f);
                if (_animator != null)
                    _animator.SetFloat(_speedHash, speed);

                if (_dashOnCooldown)
                {
                    _dashCooldownRemaining -= Time.deltaTime;
                    if (_dashCooldownRemaining <= 0f)
                    {
                        _dashCooldownRemaining = 0f;
                        _dashOnCooldown = false;
                    }
                    OnDashCooldownChanged?.Invoke(_dashCooldownRemaining, DashCooldown);
                }
            }
        }

        public void SetMoveSpeed(float speed)
        {
            _moveSpeed = speed;
        }
    }
}
