using UnityEngine;
using UnityEngine.AI;
using Unity.Profiling;
using MobaRoguelike.Runtime.Input;

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

        private static readonly ProfilerMarker s_UpdateMarker =
            new ProfilerMarker("HeroController.Update");

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            _speedHash = Animator.StringToHash("Speed");

            _agent.updatePosition = false;
            _agent.updateRotation = false;
        }

        private void Start()
        {
            _agent.Warp(transform.position);
        }

        private void OnEnable()
        {
            if (_inputReader != null)
                _inputReader.OnMoveInput += HandleMoveInput;
        }

        private void OnDisable()
        {
            if (_inputReader != null)
                _inputReader.OnMoveInput -= HandleMoveInput;
        }

        private void HandleMoveInput(Vector2 dir)
        {
            _moveInput = dir;
        }

        private void Update()
        {
            using (s_UpdateMarker.Auto())
            {
                if (_moveInput.sqrMagnitude > 0.01f)
                {
                    Vector3 worldDir = s_IsometricRotation * new Vector3(_moveInput.x, 0f, _moveInput.y);
                    Vector3 move = worldDir.normalized * (_moveSpeed * Time.deltaTime);
                    _agent.Move(move);
                    transform.rotation = Quaternion.LookRotation(worldDir.normalized);
                }

                transform.position = _agent.nextPosition;

                float speed = _moveInput.sqrMagnitude > 0.01f ? _moveSpeed : 0f;
                if (_animator != null)
                    _animator.SetFloat(_speedHash, speed);
            }
        }

        public void SetMoveSpeed(float speed)
        {
            _moveSpeed = speed;
        }
    }
}
