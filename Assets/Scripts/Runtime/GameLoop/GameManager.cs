using System.Collections;
using UnityEngine;
using MobaRoguelike.Core.GameLoop;
using MobaRoguelike.Runtime.Bootstrap;
using MobaRoguelike.Runtime.Enemy;
using MobaRoguelike.Runtime.Hero;

namespace MobaRoguelike.Runtime.GameLoop
{
    public class GameManager : MonoBehaviour
    {
        public float SpawnInterval = 3f;
        public float SpawnRadius   = 8f;

        private void Start()
        {
            if (GameBootstrap.StateMachine == null)
            {
                Debug.LogError("[GameManager] GameBootstrap not initialized.");
                return;
            }

            if (GameBootstrap.StateMachine.Current == GamePhase.None)
                GameBootstrap.StateMachine.TryTransition(GamePhase.MainMenu);

            bool success = GameBootstrap.StateMachine.TryTransition(GamePhase.InRun);
            Debug.Log($"[GameManager] Transitioned to InRun: {success}. Current: {GameBootstrap.StateMachine.Current}");

            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(SpawnInterval);
            }
        }

        public void SpawnEnemy()
        {
            var hero = Object.FindAnyObjectByType<HeroController>();
            Vector3 center = hero != null ? hero.transform.position : Vector3.zero;

            float angle    = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * SpawnRadius;

            var enemy = EnemyController.Create(center + offset);
            Debug.Log($"[GameManager] Spawned enemy at {enemy.transform.position}");
        }
    }
}
