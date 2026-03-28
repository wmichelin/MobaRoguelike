using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using MobaRoguelike.Core.Abilities;
using MobaRoguelike.Runtime.Abilities;
using MobaRoguelike.Runtime.Enemy;
using MobaRoguelike.Runtime.Hero;

namespace MobaRoguelike.Tests.PlayMode
{
    public class EnemyBehaviorTests
    {
        // Enemy spawns immediately when GameManager starts (first SpawnLoop iteration)
        [UnityTest]
        public IEnumerator EnemySpawnsOnGameManagerStart()
        {
            SceneManager.LoadScene("Game");
            yield return null;
            yield return null;

            var enemy = Object.FindAnyObjectByType<EnemyController>();
            Assert.IsNotNull(enemy, "An enemy should spawn immediately when GameManager starts.");
        }

        // Enemy uses a Capsule primitive (same shape as the hero)
        [UnityTest]
        public IEnumerator EnemyIsCapsuleShaped()
        {
            SceneManager.LoadScene("Game");
            yield return null;
            yield return null;

            var enemy = Object.FindAnyObjectByType<EnemyController>();
            Assert.IsNotNull(enemy);
            Assert.IsNotNull(
                enemy.GetComponent<CapsuleCollider>(),
                "Enemy should be a Capsule primitive (has CapsuleCollider).");
        }

        // Enemy should get closer to the hero over a few seconds via NavMesh pathfinding
        [UnityTest]
        public IEnumerator EnemyMovesTowardHero()
        {
            SceneManager.LoadScene("Game");
            yield return null;
            yield return null;

            var hero  = Object.FindAnyObjectByType<HeroController>();
            var enemy = Object.FindAnyObjectByType<EnemyController>();
            Assert.IsNotNull(hero);
            Assert.IsNotNull(enemy);

            float startDist = Vector3.Distance(enemy.transform.position, hero.transform.position);

            yield return new WaitForSeconds(3f);

            float endDist = Vector3.Distance(enemy.transform.position, hero.transform.position);
            Assert.Less(endDist, startDist, "Enemy should move closer to the hero over time.");
        }

        // Enemy inside spin radius (2 units) should take damage
        [UnityTest]
        public IEnumerator SpinAttack_DamagesEnemyWithinRadius()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero);

            foreach (var e in Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
                Object.Destroy(e.gameObject);
            yield return null;

            var enemy = EnemyController.Create(hero.transform.position + new Vector3(2f, 0f, 0f));
            yield return null;

            var health = enemy.GetComponent<EnemyHealth>();
            float initialHealth = health.CurrentHealth;

            var bridge = hero.GetComponent<AbilityCasterBridge>();
            bridge.Caster.TryCast(AbilitySlot.Q, new AbilityContext
            {
                CasterPositionX = hero.transform.position.x,
                CasterPositionZ = hero.transform.position.z,
                CasterId        = hero.gameObject.GetInstanceID()
            });
            yield return null;

            Assert.Less(health.CurrentHealth, initialHealth,
                "Enemy within spin radius should take damage.");
        }

        // Enemy outside spin radius (5 units) should not take damage
        [UnityTest]
        public IEnumerator SpinAttack_DoesNotDamageEnemyBeyondRadius()
        {
            SceneManager.LoadScene("Game");
            yield return null;

            var hero = Object.FindAnyObjectByType<HeroController>();
            Assert.IsNotNull(hero);

            foreach (var e in Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
                Object.Destroy(e.gameObject);
            yield return null;

            var enemy = EnemyController.Create(hero.transform.position + new Vector3(5f, 0f, 0f));
            yield return null;

            var health = enemy.GetComponent<EnemyHealth>();
            float initialHealth = health.CurrentHealth;

            var bridge = hero.GetComponent<AbilityCasterBridge>();
            bridge.Caster.TryCast(AbilitySlot.Q, new AbilityContext
            {
                CasterPositionX = hero.transform.position.x,
                CasterPositionZ = hero.transform.position.z,
                CasterId        = hero.gameObject.GetInstanceID()
            });
            yield return null;

            Assert.AreEqual(initialHealth, health.CurrentHealth, 0.001f,
                "Enemy beyond spin radius should not take damage.");
        }
    }
}
