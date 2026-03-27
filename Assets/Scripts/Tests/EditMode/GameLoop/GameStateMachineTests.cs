using NUnit.Framework;
using MobaRoguelike.Core.GameLoop;

namespace MobaRoguelike.Tests.EditMode.GameLoop
{
    public class GameStateMachineTests
    {
        private GameStateMachine _fsm;

        [SetUp]
        public void SetUp()
        {
            _fsm = new GameStateMachine();
        }

        [Test]
        public void Initial_IsNone()
        {
            Assert.AreEqual(GamePhase.None, _fsm.Current);
        }

        [Test]
        public void ValidTransition_ReturnsTrue()
        {
            Assert.IsTrue(_fsm.TryTransition(GamePhase.MainMenu));
        }

        [Test]
        public void InvalidTransition_ReturnsFalse()
        {
            Assert.IsFalse(_fsm.TryTransition(GamePhase.InRun));
        }

        [Test]
        public void OnPhaseChanged_FiresWithCorrectFromTo()
        {
            GamePhase capturedFrom = GamePhase.None;
            GamePhase capturedTo = GamePhase.None;
            bool fired = false;

            _fsm.OnPhaseChanged += (from, to) =>
            {
                capturedFrom = from;
                capturedTo = to;
                fired = true;
            };

            _fsm.TryTransition(GamePhase.MainMenu);

            Assert.IsTrue(fired);
            Assert.AreEqual(GamePhase.None, capturedFrom);
            Assert.AreEqual(GamePhase.MainMenu, capturedTo);
        }

        [Test]
        public void InvalidTransition_EventNotFired()
        {
            bool fired = false;
            _fsm.OnPhaseChanged += (_, __) => fired = true;

            _fsm.TryTransition(GamePhase.InRun);

            Assert.IsFalse(fired);
        }

        [Test]
        public void FullPath_NoneToMainMenuToInRun_Succeeds()
        {
            Assert.IsTrue(_fsm.TryTransition(GamePhase.MainMenu));
            Assert.IsTrue(_fsm.TryTransition(GamePhase.InRun));
            Assert.AreEqual(GamePhase.InRun, _fsm.Current);
        }
    }
}
