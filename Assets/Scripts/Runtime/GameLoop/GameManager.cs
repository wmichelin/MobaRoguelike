using UnityEngine;
using MobaRoguelike.Core.GameLoop;
using MobaRoguelike.Runtime.Bootstrap;

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
        }
    }
}
