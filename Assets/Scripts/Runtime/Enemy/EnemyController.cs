using UnityEngine;
using UnityEngine.AI;
using MobaRoguelike.Core.Enemy;

namespace MobaRoguelike.Runtime.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private EnemyHealth  _health;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>() ?? gameObject.AddComponent<EnemyHealth>();

            if (GetComponent<EnemyFlashEffect>() == null)
                gameObject.AddComponent<EnemyFlashEffect>();

            _agent = GetComponent<NavMeshAgent>(); // null until NavMesh is baked
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
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Enemy";
            go.transform.position = position;

            var controller = go.AddComponent<EnemyController>();

            if (data != null)
                controller._health.Configure(data.MaxHealth);

            return controller;
        }
    }
}
