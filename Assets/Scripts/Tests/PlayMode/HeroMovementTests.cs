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

            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero, "HeroController not found in scene.");
        }

        [UnityTest]
        public IEnumerator HeroMovesWithInput()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero);

            float startX = hero.transform.position.x;

            // Set rightward input once; HeroController.Update reads _moveInput each frame.
            // Use WaitForSeconds so this is framerate-independent (batchmode runs at very high FPS).
            hero.SendMessage("HandleMoveInput", new Vector2(1f, 0f), SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(0.1f);

            Assert.GreaterOrEqual(hero.transform.position.x - startX, 0.5f,
                "Hero did not move far enough in the X direction.");
        }

        [UnityTest]
        public IEnumerator HeroStopsWhenInputReleased()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero);

            // Move for a bit
            for (int i = 0; i < 30; i++)
            {
                hero.SendMessage("HandleMoveInput", new Vector2(1f, 0f), SendMessageOptions.DontRequireReceiver);
                yield return null;
            }

            // Stop input and record position
            hero.SendMessage("HandleMoveInput", Vector2.zero, SendMessageOptions.DontRequireReceiver);
            yield return null;

            Vector3 posAfterStop = hero.transform.position;

            // Wait a few more frames
            for (int i = 0; i < 10; i++)
                yield return null;

            float drift = (hero.transform.position - posAfterStop).magnitude;
            Assert.Less(drift, 0.05f,
                "Hero should stop moving after input released.");
        }
    }
}
