using UnityEngine;
using UnityEngine.SceneManagement;
using MobaRoguelike.Core.GameLoop;
using MobaRoguelike.Runtime.Bootstrap;

namespace MobaRoguelike.Runtime.GameLoop
{
    public class MainMenuController : MonoBehaviour
    {
        private void Start()
        {
            if (GameBootstrap.StateMachine != null &&
                GameBootstrap.StateMachine.Current == GamePhase.None)
            {
                GameBootstrap.StateMachine.TryTransition(GamePhase.MainMenu);
            }
        }

        public void OnStartGameButtonClicked()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
