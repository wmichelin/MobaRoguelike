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

            var cam = Object.FindAnyObjectByType<IsometricCameraController>();
            Assert.IsNotNull(cam, "IsometricCameraController not found.");

            var hero = Object.FindAnyObjectByType<MobaRoguelike.Runtime.Hero.HeroController>();
            Assert.IsNotNull(hero, "HeroController not found.");

            // Teleport hero +10 on X
            Vector3 originalCamPos = cam.transform.position;
            hero.transform.position += new Vector3(10f, 0f, 0f);

            // Wait real time for SmoothDamp to converge
            yield return new WaitForSeconds(2f);

            float delta = (cam.transform.position - originalCamPos).magnitude;
            Assert.GreaterOrEqual(delta, 5f,
                $"Camera should have followed the hero, but only moved {delta} units.");
        }
    }
}
