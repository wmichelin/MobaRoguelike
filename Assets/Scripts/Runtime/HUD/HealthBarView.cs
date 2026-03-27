using UnityEngine;
using UnityEngine.UI;

namespace MobaRoguelike.Runtime.HUD
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        public void UpdateHealth(float current, float max)
        {
            if (_slider == null) return;
            _slider.maxValue = max;
            _slider.value = current;
        }
    }
}
