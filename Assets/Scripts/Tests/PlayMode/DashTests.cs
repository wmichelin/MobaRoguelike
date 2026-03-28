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

            // Move right briefly to establish facing direction.
            // Use WaitForSeconds so the test is framerate-independent (batchmode runs at very high FPS).
            hero.SendMessage("HandleMoveInput", new Vector2(1f, 0f), SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(0.05f);

            // Stop movement and record position
            hero.SendMessage("HandleMoveInput", Vector2.zero, SendMessageOptions.DontRequireReceiver);
            yield return null;

            Vector3 posBeforeDash = hero.transform.position;

            // Trigger dash
            hero.SendMessage("HandleDashInput", SendMessageOptions.DontRequireReceiver);

            // Wait longer than DashDuration (0.15s) to ensure the dash fully completes
            yield return new WaitForSeconds(0.5f);

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

            // Wait longer than DashDuration (0.15s) for the dash to finish and cooldown to start.
            // Use WaitForSeconds so the test is framerate-independent (batchmode runs at very high FPS).
            yield return new WaitForSeconds(0.3f);

            Assert.IsTrue(canvas.gameObject.activeSelf, "Cooldown bar should be visible during cooldown.");

            // Wait for cooldown to expire (DashCooldown = 1.0s, add buffer)
            yield return new WaitForSeconds(1.5f);

            Assert.IsFalse(canvas.gameObject.activeSelf, "Cooldown bar should hide after cooldown expires.");
        }
    }
}
