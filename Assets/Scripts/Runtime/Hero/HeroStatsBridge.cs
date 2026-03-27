using UnityEngine;
using MobaRoguelike.Core.Stats;
using MobaRoguelike.Runtime.Bootstrap;

namespace MobaRoguelike.Runtime.Hero
{
    public class HeroStatsBridge : MonoBehaviour
    {
        private HeroController _controller;

        private void Awake()
        {
            _controller = GetComponent<HeroController>();
        }

        private void Start()
        {
            if (GameBootstrap.HeroStats == null) return;

            GameBootstrap.HeroStats.OnStatChanged += OnStatChanged;
            SyncMoveSpeed();
        }

        private void OnDestroy()
        {
            if (GameBootstrap.HeroStats != null)
                GameBootstrap.HeroStats.OnStatChanged -= OnStatChanged;
        }

        private void OnStatChanged(StatType stat)
        {
            if (stat == StatType.MoveSpeed)
                SyncMoveSpeed();
        }

        private void SyncMoveSpeed()
        {
            float speed = GameBootstrap.HeroStats.GetFinalValue(StatType.MoveSpeed);
            _controller.SetMoveSpeed(speed);
        }
    }
}
