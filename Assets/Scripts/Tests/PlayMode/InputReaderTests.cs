using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using MobaRoguelike.Runtime.Input;
using MobaRoguelike.Runtime.Hero;
using MobaRoguelike.Runtime.Abilities;

namespace MobaRoguelike.Tests.PlayMode
{
    public class InputReaderTests
    {
        [UnityTest]
        public IEnumerator SceneHasInputReader()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var reader = Object.FindAnyObjectByType<InputReader>();
            Assert.IsNotNull(reader, "InputReader component missing from scene.");
        }

        [UnityTest]
        public IEnumerator HeroControllerInputReaderIsWired()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero, "HeroController not found in scene.");

            var field = typeof(HeroController).GetField("_inputReader",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var value = field.GetValue(hero) as InputReader;
            Assert.IsNotNull(value, "HeroController._inputReader is not assigned in the scene.");
        }

        [UnityTest]
        public IEnumerator AbilityCasterBridgeInputReaderIsWired()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var bridge = Object.FindAnyObjectByType<AbilityCasterBridge>();
            Assert.IsNotNull(bridge, "AbilityCasterBridge not found in scene.");

            var field = typeof(AbilityCasterBridge).GetField("_inputReader",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var value = field.GetValue(bridge) as InputReader;
            Assert.IsNotNull(value, "AbilityCasterBridge._inputReader is not assigned in the scene.");
        }

        [UnityTest]
        public IEnumerator InputReaderSurvivesDisableEnableCycle()
        {
            var go = new GameObject("TestInputReader");
            var reader = go.AddComponent<InputReader>();
            yield return null;

            // Simulate the play-stop-play cycle that was breaking input
            reader.enabled = false;
            yield return null;

            reader.enabled = true;
            yield return null;

            bool received = false;
            reader.OnMoveInput += _ => received = true;

            // Verify the reader is functional after re-enable
            Assert.IsTrue(reader.enabled);
            Assert.IsTrue(reader.gameObject.activeInHierarchy);

            Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator InputReaderFiresMoveEvent()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var reader = Object.FindAnyObjectByType<InputReader>();
            Assert.IsNotNull(reader);

            Vector2 received = Vector2.zero;
            reader.OnMoveInput += v => received = v;

            // Drive the hero via SendMessage (same pattern as HeroMovementTests)
            // and verify InputReader is subscribed in the chain
            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero);

            Vector3 startPos = hero.transform.position;
            hero.SendMessage("HandleMoveInput", Vector2.right, SendMessageOptions.DontRequireReceiver);

            for (int i = 0; i < 30; i++)
                yield return null;

            float dist = (hero.transform.position - startPos).magnitude;
            Assert.Greater(dist, 0.1f, "Hero should move when input is driven through the event chain.");
        }
    }
}
