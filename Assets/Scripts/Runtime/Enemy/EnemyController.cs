using UnityEngine;
using UnityEngine.AI;
using MobaRoguelike.Core.Enemy;
using MobaRoguelike.Runtime.Hero;

namespace MobaRoguelike.Runtime.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private EnemyHealth  _health;
        private Transform    _heroTransform;

        private const float MoveSpeed = 2f;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>() ?? gameObject.AddComponent<EnemyHealth>();

            if (GetComponent<EnemyFlashEffect>() == null)
                gameObject.AddComponent<EnemyFlashEffect>();

            _agent = GetComponent<NavMeshAgent>();
            if (_agent != null)
                _agent.speed = MoveSpeed;
        }

        private void Start()
        {
            var hero = Object.FindAnyObjectByType<HeroController>();
            if (hero != null)
                _heroTransform = hero.transform;
        }

        private void Update()
        {
            if (_agent != null && _heroTransform != null && _agent.isOnNavMesh)
                _agent.SetDestination(_heroTransform.position);
        }


        /// <summary>
        /// Move toward a world position. No-op until a NavMeshAgent is present.
        /// </summary>
        public void SetDestination(Vector3 position)
        {
            _agent?.SetDestination(position);
        }

        /// <summary>
        /// Spawns a cube enemy at the given world position with optional data overrides.
        /// </summary>
        public static EnemyController Create(Vector3 position, EnemyData data = null)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Enemy";
            go.transform.position = position;

            go.AddComponent<NavMeshAgent>();

            var controller = go.AddComponent<EnemyController>();

            if (data != null)
                controller._health.Configure(data.MaxHealth);

            return controller;
        }
    }
}
