using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using MobaRoguelike.Runtime.Hero;
using MobaRoguelike.Runtime.HUD;

namespace MobaRoguelike.Tests.PlayMode
{
    public class DashTests
    {
        [UnityTest]
        public IEnumerator DashMovesHeroInFacingDirection()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero, "HeroController not found in scene.");

            // Move right for a few frames to establish facing direction
            for (int i = 0; i < 10; i++)
            {
                hero.SendMessage("HandleMoveInput", new Vector2(1f, 0f), SendMessageOptions.DontRequireReceiver);
                yield return null;
            }

            // Stop movement and record position
            hero.SendMessage("HandleMoveInput", Vector2.zero, SendMessageOptions.DontRequireReceiver);
            yield return null;

            Vector3 posBeforeDash = hero.transform.position;

            // Trigger dash
            hero.SendMessage("HandleDashInput", SendMessageOptions.DontRequireReceiver);

            // Wait for dash to complete (~0.15s at 60fps = ~9 frames, give extra)
            for (int i = 0; i < 20; i++)
                yield return null;

            float distanceMoved = (hero.transform.position - posBeforeDash).magnitude;
            Assert.Greater(distanceMoved, 2f, "Hero should move a significant distance during dash.");
        }

        [UnityTest]
        public IEnumerator DashCooldownBlocksSecondDash()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero);

            // Establish facing direction
            for (int i = 0; i < 5; i++)
            {
                hero.SendMessage("HandleMoveInput", new Vector2(1f, 0f), SendMessageOptions.DontRequireReceiver);
                yield return null;
            }
            hero.SendMessage("HandleMoveInput", Vector2.zero, SendMessageOptions.DontRequireReceiver);
            yield return null;

            // First dash
            hero.SendMessage("HandleDashInput", SendMessageOptions.DontRequireReceiver);

            // Wait for dash to complete
            for (int i = 0; i < 20; i++)
                yield return null;

            Vector3 posAfterFirstDash = hero.transform.position;

            // Attempt second dash immediately (should be blocked by cooldown)
            hero.SendMessage("HandleDashInput", SendMessageOptions.DontRequireReceiver);

            for (int i = 0; i < 20; i++)
                yield return null;

            float drift = (hero.transform.position - posAfterFirstDash).magnitude;
            Assert.Less(drift, 0.1f, "Second dash should be blocked by cooldown.");
        }

        [UnityTest]
        public IEnumerator CooldownBarAppearsAndDisappears()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero);

            var cooldownBar = hero.GetComponent<DashCooldownBar>();
            Assert.IsNotNull(cooldownBar, "DashCooldownBar should be auto-added to hero.");

            // The canvas child should start inactive
            var canvas = hero.GetComponentInChildren<Canvas>(true);
            Assert.IsNotNull(canvas, "World-space canvas should exist.");
            Assert.IsFalse(canvas.gameObject.activeSelf, "Cooldown bar should start hidden.");

            // Trigger dash
            hero.SendMessage("HandleDashInput", SendMessageOptions.DontRequireReceiver);

            // Wait for dash to finish and cooldown to start
            for (int i = 0; i < 20; i++)
                yield return null;

            Assert.IsTrue(canvas.gameObject.activeSelf, "Cooldown bar should be visible during cooldown.");

            // Wait for cooldown to expire (~2 seconds = ~120 frames at 60fps, add buffer)
            for (int i = 0; i < 140; i++)
                yield return null;

            Assert.IsFalse(canvas.gameObject.activeSelf, "Cooldown bar should hide after cooldown expires.");
        }
    }
}
