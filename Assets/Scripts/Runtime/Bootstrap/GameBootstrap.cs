using UnityEngine;
using MobaRoguelike.Core.GameLoop;
using MobaRoguelike.Core.Hero;
using MobaRoguelike.Core.Stats;

namespace MobaRoguelike.Runtime.Bootstrap
{
    [UnityEngine.DefaultExecutionOrder(-100)]
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private HeroBaseStatsSO _heroBaseStats;

        public static GameStateMachine StateMachine { get; private set; }
        public static HeroData HeroData { get; private set; }
        public static StatSheet HeroStats { get; private set; }

        private void Awake()
        {
            if (StateMachine != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            HeroStats = new StatSheet();
            if (_heroBaseStats != null)
                _heroBaseStats.ApplyTo(HeroStats);

            HeroData = new HeroData(HeroStats);
            StateMachine = new GameStateMachine();
        }
    }
}
