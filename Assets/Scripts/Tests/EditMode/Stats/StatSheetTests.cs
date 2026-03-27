using NUnit.Framework;
using MobaRoguelike.Core.Stats;

namespace MobaRoguelike.Tests.EditMode.Stats
{
    public class StatSheetTests
    {
        private StatSheet _sheet;

        [SetUp]
        public void SetUp()
        {
            _sheet = new StatSheet();
            _sheet.SetBase(StatType.AttackDamage, 100f);
        }

        [Test]
        public void NoModifiers_ReturnBase()
        {
            Assert.AreEqual(100f, _sheet.GetFinalValue(StatType.AttackDamage));
        }

        [Test]
        public void FlatModifier_AddsToBase()
        {
            _sheet.AddModifier(new StatModifier(StatType.AttackDamage, 20f, ModifierType.Flat, 1));
            Assert.AreEqual(120f, _sheet.GetFinalValue(StatType.AttackDamage));
        }

        [Test]
        public void PercentModifier_MultipliesTotal()
        {
            _sheet.AddModifier(new StatModifier(StatType.AttackDamage, 0.5f, ModifierType.Percent, 1));
            Assert.AreEqual(150f, _sheet.GetFinalValue(StatType.AttackDamage), 0.001f);
        }

        [Test]
        public void FlatAndPercent_CombinedFormula()
        {
            _sheet.AddModifier(new StatModifier(StatType.AttackDamage, 20f, ModifierType.Flat, 1));
            _sheet.AddModifier(new StatModifier(StatType.AttackDamage, 0.5f, ModifierType.Percent, 2));
            // (100 + 20) * (1 + 0.5) = 180
            Assert.AreEqual(180f, _sheet.GetFinalValue(StatType.AttackDamage), 0.001f);
        }

        [Test]
        public void MultiplePercents_AreAdditive()
        {
            _sheet.AddModifier(new StatModifier(StatType.AttackDamage, 0.3f, ModifierType.Percent, 1));
            _sheet.AddModifier(new StatModifier(StatType.AttackDamage, 0.2f, ModifierType.Percent, 2));
            // (100) * (1 + 0.3 + 0.2) = 150
            Assert.AreEqual(150f, _sheet.GetFinalValue(StatType.AttackDamage), 0.001f);
        }
    }
}
