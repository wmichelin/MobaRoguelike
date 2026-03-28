using UnityEngine;
using MobaRoguelike.Core.GameLoop;
using MobaRoguelike.Runtime.Bootstrap;
using MobaRoguelike.Runtime.Enemy;
using MobaRoguelike.Runtime.Hero;

namespace MobaRoguelike.Runtime.GameLoop
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            if (GameBootstrap.StateMachine == null)
            {
                Debug.LogError("[GameManager] GameBootstrap not initialized.");
                return;
            }

            // Ensure we're in MainMenu state first, then transition to InRun
            if (GameBootstrap.StateMachine.Current == GamePhase.None)
                GameBootstrap.StateMachine.TryTransition(GamePhase.MainMenu);

            bool success = GameBootstrap.StateMachine.TryTransition(GamePhase.InRun);
            Debug.Log($"[GameManager] Transitioned to InRun: {success}. Current: {GameBootstrap.StateMachine.Current}");

            var hero = FindObjectOfType<HeroController>();
            Vector3 spawnPos = hero != null
                ? hero.transform.position + new Vector3(5f, 0f, 0f)
                : new Vector3(5f, 0f, 0f);

            var enemy = EnemyController.Create(spawnPos);
            Debug.Log($"[GameManager] Spawned enemy at {enemy.transform.position}");
        }
    }
}
