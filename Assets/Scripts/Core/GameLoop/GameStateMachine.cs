using System;

namespace MobaRoguelike.Core.GameLoop
{
    public class GameStateMachine
    {
        public GamePhase Current { get; private set; } = GamePhase.None;

        public event Action<GamePhase, GamePhase> OnPhaseChanged;

        public bool TryTransition(GamePhase to)
        {
            if (!IsValidTransition(Current, to))
                return false;

            GamePhase from = Current;
            Current = to;
            OnPhaseChanged?.Invoke(from, to);
            return true;
        }

        private static bool IsValidTransition(GamePhase from, GamePhase to)
        {
            return (from, to) switch
            {
                (GamePhase.None,      GamePhase.MainMenu)   => true,
                (GamePhase.MainMenu,  GamePhase.InRun)      => true,
                (GamePhase.InRun,     GamePhase.InWave)     => true,
                (GamePhase.InRun,     GamePhase.GameOver)   => true,
                (GamePhase.InRun,     GamePhase.Victory)    => true,
                (GamePhase.InWave,    GamePhase.Upgrading)  => true,
                (GamePhase.Upgrading, GamePhase.InRun)      => true,
                (GamePhase.GameOver,  GamePhase.MainMenu)   => true,
                (GamePhase.Victory,   GamePhase.MainMenu)   => true,
                _ => false
            };
        }
    }
}
