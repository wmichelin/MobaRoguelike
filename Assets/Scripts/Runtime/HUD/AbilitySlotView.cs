using UnityEngine;
using UnityEngine.UI;

namespace MobaRoguelike.Runtime.HUD
{
    public class AbilitySlotView : MonoBehaviour
    {
        [SerializeField] private Image _cooldownOverlay;

        private Image _background;
        private Text  _keyLabel;
        private Text  _abilityNameLabel;

        private void Awake()
        {
            if (_cooldownOverlay != null)
            {
                _cooldownOverlay.type       = Image.Type.Filled;
                _cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
                _cooldownOverlay.fillAmount = 0f;
            }
        }

        /// <summary>
        /// Called by ActionBarView after it procedurally creates the slot UI elements.
        /// </summary>
        public void Initialize(Image background, Image cooldownOverlay, Text keyLabel, Text abilityNameLabel)
        {
            _background       = background;
            _cooldownOverlay  = cooldownOverlay;
            _keyLabel         = keyLabel;
            _abilityNameLabel = abilityNameLabel;

            if (_cooldownOverlay != null)
            {
                _cooldownOverlay.type       = Image.Type.Filled;
                _cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
                _cooldownOverlay.fillAmount = 0f;
            }
        }

        public void SetAbilityName(string abilityName)
        {
            if (_abilityNameLabel != null)
                _abilityNameLabel.text = abilityName;
        }

        public void UpdateCooldown(float fillAmount)
        {
            if (_cooldownOverlay != null)
                _cooldownOverlay.fillAmount = fillAmount;
        }
    }
}
