using NUnit.Framework;
using MobaRoguelike.Core.Abilities;

namespace MobaRoguelike.Tests.EditMode.Abilities
{
    public class AbilityCasterTests
    {
        private AbilityCaster _caster;
        private AbilityContext _ctx;

        [SetUp]
        public void SetUp()
        {
            _caster = new AbilityCaster();
            _ctx = default;
        }

        private AbilityData MakeAbility(float cooldown = 5f) => new AbilityData
        {
            Id = "test",
            Cooldown = cooldown,
            Effect = new NoOpAbilityEffect()
        };

        [Test]
        public void EmptySlot_ReturnsFalse()
        {
            Assert.IsFalse(_caster.TryCast(AbilitySlot.Q, _ctx));
        }

        [Test]
        public void FirstCast_ReturnsTrue()
        {
            _caster.SetAbility(AbilitySlot.Q, MakeAbility());
            Assert.IsTrue(_caster.TryCast(AbilitySlot.Q, _ctx));
        }

        [Test]
        public void ImmediateSecondCast_ReturnsFalse()
        {
            _caster.SetAbility(AbilitySlot.Q, MakeAbility());
            _caster.TryCast(AbilitySlot.Q, _ctx);
            Assert.IsFalse(_caster.TryCast(AbilitySlot.Q, _ctx));
        }

        [Test]
        public void Tick_ReducesRemainingCooldown()
        {
            _caster.SetAbility(AbilitySlot.Q, MakeAbility(5f));
            _caster.TryCast(AbilitySlot.Q, _ctx);
            _caster.Tick(2f);
            Assert.AreEqual(3f, _caster.GetState(AbilitySlot.Q).RemainingCooldown, 0.001f);
        }

        [Test]
        public void Tick_ExpiresCooldown_IsOnCooldownFalse()
        {
            _caster.SetAbility(AbilitySlot.Q, MakeAbility(3f));
            _caster.TryCast(AbilitySlot.Q, _ctx);
            _caster.Tick(3f);
            Assert.IsFalse(_caster.GetState(AbilitySlot.Q).IsOnCooldown);
        }

        [Test]
        public void OnCooldownChanged_FiresExactlyOnceOnExpire()
        {
            _caster.SetAbility(AbilitySlot.Q, MakeAbility(1f));
            _caster.TryCast(AbilitySlot.Q, _ctx);

            int fireCount = 0;
            _caster.OnCooldownChanged += (slot, state) =>
            {
                if (!state.IsOnCooldown) fireCount++;
            };

            _caster.Tick(1f);
            Assert.AreEqual(1, fireCount);
        }

        [Test]
        public void CastAfterCooldown_ReturnsTrue()
        {
            _caster.SetAbility(AbilitySlot.Q, MakeAbility(2f));
            _caster.TryCast(AbilitySlot.Q, _ctx);
            _caster.Tick(2f);
            Assert.IsTrue(_caster.TryCast(AbilitySlot.Q, _ctx));
        }
    }
}
