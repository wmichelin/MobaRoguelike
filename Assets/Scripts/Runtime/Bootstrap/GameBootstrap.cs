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

        private static GameBootstrap _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                // Scene reloaded (e.g. in tests). Reset the state machine for the new session
                // but keep the persistent bootstrap alive. Destroy only this component so
                // other components on this GameObject (e.g. GameManager) can still Start().
                StateMachine = new GameStateMachine();
                Destroy(this);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            HeroStats = new StatSheet();
            if (_heroBaseStats != null)
                _heroBaseStats.ApplyTo(HeroStats);

            HeroData = new HeroData(HeroStats);
            StateMachine = new GameStateMachine();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                StateMachine = null;
            }
        }
    }
}
