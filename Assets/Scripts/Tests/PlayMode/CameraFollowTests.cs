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

            Vector3 originalCamPos = cam.transform.position;

            // Drive hero movement via input so the NavMeshAgent actually moves
            for (int i = 0; i < 60; i++)
            {
                hero.SendMessage("HandleMoveInput", new Vector2(1f, 0f), SendMessageOptions.DontRequireReceiver);
                yield return null;
            }

            float delta = (cam.transform.position - originalCamPos).magnitude;
            Assert.Greater(delta, 0.5f,
                $"Camera should have followed the hero, but only moved {delta} units.");
        }
    }
}
