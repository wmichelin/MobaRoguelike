using NUnit.Framework;
using MobaRoguelike.Core.Stats;

namespace MobaRoguelike.Tests.EditMode.Stats
{
    public class StatModifierTests
    {
        private StatSheet _sheet;

        [SetUp]
        public void SetUp()
        {
            _sheet = new StatSheet();
            _sheet.SetBase(StatType.AttackDamage, 100f);
        }

        [Test]
        public void RemoveBySourceId_RemovesCorrectModifier()
        {
            _sheet.AddModifier(new StatModifier(StatType.AttackDamage, 50f, ModifierType.Flat, 10));
            _sheet.AddModifier(new StatModifier(StatType.AttackDamage, 25f, ModifierType.Flat, 20));
            _sheet.RemoveModifier(10);
            // Only +25 remains
            Assert.AreEqual(125f, _sheet.GetFinalValue(StatType.AttackDamage), 0.001f);
        }

        [Test]
        public void RemoveNonexistentId_IsNoOp()
        {
            _sheet.AddModifier(new StatModifier(StatType.AttackDamage, 50f, ModifierType.Flat, 10));
            _sheet.RemoveModifier(999);
            Assert.AreEqual(150f, _sheet.GetFinalValue(StatType.AttackDamage), 0.001f);
        }
    }
}
