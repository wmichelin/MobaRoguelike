using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using MobaRoguelike.Runtime.Camera;

namespace MobaRoguelike.Tests.PlayMode
{
    public class CameraFollowTests
    {
        [UnityTest]
        public IEnumerator CameraFollowsHero()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var cam = Object.FindFirstObjectByType<IsometricCameraController>();
            Assert.IsNotNull(cam, "IsometricCameraController not found.");

            var hero = Object.FindFirstObjectByType<MobaRoguelike.Runtime.Hero.HeroController>();
            Assert.IsNotNull(hero, "HeroController not found.");

            // Teleport hero +10 on X
            float originalCamX = cam.transform.position.x;
            hero.transform.position += new Vector3(10f, 0f, 0f);

            // Wait 120 frames for camera to follow
            for (int i = 0; i < 120; i++)
                yield return null;

            float delta = cam.transform.position.x - originalCamX;
            Assert.GreaterOrEqual(delta, 9.5f,
                $"Camera should have moved at least 9.5 units on X, but moved {delta}.");
        }
    }
}
