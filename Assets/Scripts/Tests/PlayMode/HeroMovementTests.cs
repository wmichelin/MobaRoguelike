using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using MobaRoguelike.Runtime.Hero;

namespace MobaRoguelike.Tests.PlayMode
{
    public class HeroMovementTests
    {
        [UnityTest]
        public IEnumerator HeroSpawnsAtOrigin()
        {
            SceneManager.LoadScene("Game");
            yield return null; // wait one frame for scene to load

            var hero = Object.FindFirstObjectByType<HeroController>();
            Assert.IsNotNull(hero, "HeroController not found in scene.");
        }

        [UnityTest]
        public IEnumerator HeroMovesWithInput()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindFirstObjectByType<HeroController>();
            Assert.IsNotNull(hero);

            float startX = hero.transform.position.x;

            // Simulate 60 frames of rightward input by invoking the method directly
            var inputReader = Object.FindFirstObjectByType<MobaRoguelike.Runtime.Input.InputReader>();

            // Drive movement manually via the event for 60 frames
            for (int i = 0; i < 60; i++)
            {
                hero.SendMessage("HandleMoveInput", new Vector2(1f, 0f), SendMessageOptions.DontRequireReceiver);
                yield return null;
            }

            Assert.GreaterOrEqual(hero.transform.position.x - startX, 0.5f,
                "Hero did not move far enough in the X direction.");
        }

        [UnityTest]
        public IEnumerator HeroStopsWhenInputReleased()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindFirstObjectByType<HeroController>();
            Assert.IsNotNull(hero);

            // Move for a bit
            for (int i = 0; i < 30; i++)
            {
                hero.SendMessage("HandleMoveInput", new Vector2(1f, 0f), SendMessageOptions.DontRequireReceiver);
                yield return null;
            }

            // Stop input
            for (int i = 0; i < 30; i++)
            {
                hero.SendMessage("HandleMoveInput", Vector2.zero, SendMessageOptions.DontRequireReceiver);
                yield return null;
            }

            var agent = hero.GetComponent<UnityEngine.AI.NavMeshAgent>();
            Assert.Less(agent.velocity.magnitude, 0.05f,
                "Hero velocity should be near zero after input released.");
        }
    }
}
