using UnityEngine;
using UnityEngine.AI;
using Unity.Profiling;
using MobaRoguelike.Runtime.Input;

namespace MobaRoguelike.Runtime.Hero
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private float _lookAheadDistance = 2f;

        private NavMeshAgent _agent;
        private Animator _animator;
        private int _speedHash;
        private Vector2 _moveInput;

        private static readonly ProfilerMarker s_UpdateMarker =
            new ProfilerMarker("HeroController.Update");

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            _speedHash = Animator.StringToHash("Speed");

            _agent.updateRotation = false;
            _agent.acceleration = 20f;
            _agent.stoppingDistance = 0.1f;
            _agent.radius = 0.4f;
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
                    Vector3 worldDir = Quaternion.Euler(0f, 45f, 0f) * new Vector3(_moveInput.x, 0f, _moveInput.y);
                    _agent.SetDestination(transform.position + worldDir.normalized * _lookAheadDistance);
                }
                else
                {
                    _agent.SetDestination(transform.position);
                }

                if (_agent.velocity.sqrMagnitude > 0.01f)
                {
                    Vector3 dir = _agent.velocity.normalized;
                    transform.rotation = Quaternion.LookRotation(
                        Vector3.RotateTowards(transform.forward, dir, 20f * Time.deltaTime, 0f));
                }

                if (_animator != null)
                    _animator.SetFloat(_speedHash, _agent.velocity.magnitude);
            }
        }

        public void SetMoveSpeed(float speed)
        {
            if (_agent != null)
                _agent.speed = speed;
        }
    }
}
