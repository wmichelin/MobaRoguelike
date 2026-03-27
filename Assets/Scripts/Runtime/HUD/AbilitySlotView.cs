using UnityEngine;
using UnityEngine.UI;

namespace MobaRoguelike.Runtime.HUD
{
    public class AbilitySlotView : MonoBehaviour
    {
        [SerializeField] private Image _cooldownOverlay;

        private void Awake()
        {
            if (_cooldownOverlay != null)
            {
                _cooldownOverlay.type = Image.Type.Filled;
                _cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
                _cooldownOverlay.fillAmount = 0f;
            }
        }

        public void UpdateCooldown(float fillAmount)
        {
            if (_cooldownOverlay != null)
                _cooldownOverlay.fillAmount = fillAmount;
        }
    }
}
