using NUnit.Framework;
using MobaRoguelike.Runtime.Abilities;

namespace MobaRoguelike.Tests.EditMode.Enemy
{
    public class EnemySwordSpinTests
    {
        [Test]
        public void SwordSpinEffect_DefaultRadius_Is4()
        {
            var effect = new SwordSpinEffect();
            Assert.AreEqual(4f, effect.Radius, 0.001f);
        }

        [Test]
        public void SwordSpinEffect_DefaultDamage_Is50()
        {
            var effect = new SwordSpinEffect();
            Assert.AreEqual(50f, effect.Damage, 0.001f);
        }

        [Test]
        public void SwordSpinEffect_RadiusIsAssignable()
        {
            var effect = new SwordSpinEffect { Radius = 10f };
            Assert.AreEqual(10f, effect.Radius, 0.001f);
        }
    }
}
