using UnityEngine;
using UnityEngine.UI;
using MobaRoguelike.Core.Abilities;
using MobaRoguelike.Runtime.Abilities;

namespace MobaRoguelike.Runtime.HUD
{
    /// <summary>
    /// Procedurally builds a 4-slot action bar at the bottom of the screen.
    /// Auto-added by HudController — no manual scene wiring required.
    /// Tunable via Inspector if desired.
    /// </summary>
    public class ActionBarView : MonoBehaviour
    {
        [SerializeField] private Vector2 _slotSize     = new Vector2(80f, 80f);
        [SerializeField] private float   _slotSpacing  = 8f;
        [SerializeField] private float   _bottomOffset = 20f;
        [SerializeField] private Color   _slotColor    = new Color(0.165f, 0.165f, 0.165f, 1f);
        [SerializeField] private Color   _overlayColor = new Color(0f, 0f, 0f, 0.65f);

        private static readonly string[] KeyLabels = { "Q", "W", "E", "R" };

        private AbilitySlotView[] _slots = new AbilitySlotView[4];
        private Font _font;

        private void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            BuildSlots();

            // Wire immediately so HudController.Start() sees the views when it subscribes
            GetComponent<HudController>()?.SetSlotViews(_slots);
        }

        private void Start()
        {
            var casterBridge = FindObjectOfType<AbilityCasterBridge>();
            if (casterBridge == null) return;

            for (int i = 0; i < 4; i++)
            {
                AbilityData data = casterBridge.Caster.GetAbility((AbilitySlot)i);
                if (data != null)
                    _slots[i].SetAbilityName(data.DisplayName ?? string.Empty);
            }
        }

        private void BuildSlots()
        {
            // Parent the bar to the root Canvas so screen-space anchors work correctly.
            // If no Canvas is found, create a standalone one.
            var rootCanvas = GetComponentInParent<Canvas>();
            Transform barParent;

            if (rootCanvas != null)
            {
                barParent = rootCanvas.transform;
            }
            else
            {
                var canvasGo = new GameObject("ActionBarCanvas");
                var canvas   = canvasGo.AddComponent<Canvas>();
                canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10;
                canvasGo.AddComponent<CanvasScaler>();
                canvasGo.AddComponent<GraphicRaycaster>();
                barParent = canvasGo.transform;
            }

            float totalWidth = 4 * _slotSize.x + 3 * _slotSpacing;

            var barGo = new GameObject("ActionBar");
            barGo.transform.SetParent(barParent, false);

            var barRect = barGo.AddComponent<RectTransform>();
            barRect.anchorMin        = new Vector2(0.5f, 0f);
            barRect.anchorMax        = new Vector2(0.5f, 0f);
            barRect.pivot            = new Vector2(0.5f, 0f);
            barRect.sizeDelta        = new Vector2(totalWidth, _slotSize.y);
            barRect.anchoredPosition = new Vector2(0f, _bottomOffset);

            for (int i = 0; i < 4; i++)
            {
                float xPos = i * (_slotSize.x + _slotSpacing) - totalWidth * 0.5f + _slotSize.x * 0.5f;
                _slots[i] = BuildSlot(barGo.transform, i, xPos);
            }
        }

        private AbilitySlotView BuildSlot(Transform parent, int index, float xPos)
        {
            var slotGo = new GameObject($"Slot_{KeyLabels[index]}");
            slotGo.transform.SetParent(parent, false);

            var slotRect = slotGo.AddComponent<RectTransform>();
            slotRect.sizeDelta        = _slotSize;
            slotRect.anchoredPosition = new Vector2(xPos, 0f);

            var slotView = slotGo.AddComponent<AbilitySlotView>();

            // Background
            var bgGo   = new GameObject("Background");
            bgGo.transform.SetParent(slotGo.transform, false);
            StretchToParent(bgGo.AddComponent<RectTransform>());
            var bgImage   = bgGo.AddComponent<Image>();
            bgImage.color = _slotColor;

            // Cooldown overlay (drawn on top)
            var overlayGo = new GameObject("CooldownOverlay");
            overlayGo.transform.SetParent(slotGo.transform, false);
            StretchToParent(overlayGo.AddComponent<RectTransform>());
            var overlayImage   = overlayGo.AddComponent<Image>();
            overlayImage.color = _overlayColor;

            // Key label — top-left corner
            var keyGo  = new GameObject("KeyLabel");
            keyGo.transform.SetParent(slotGo.transform, false);
            var keyRect           = keyGo.AddComponent<RectTransform>();
            keyRect.anchorMin     = new Vector2(0f, 1f);
            keyRect.anchorMax     = new Vector2(0f, 1f);
            keyRect.pivot         = new Vector2(0f, 1f);
            keyRect.anchoredPosition = new Vector2(4f, -4f);
            keyRect.sizeDelta     = new Vector2(24f, 20f);
            var keyText           = keyGo.AddComponent<Text>();
            keyText.text          = KeyLabels[index];
            keyText.fontSize      = 14;
            keyText.fontStyle     = FontStyle.Bold;
            keyText.color         = Color.white;
            keyText.alignment     = TextAnchor.UpperLeft;
            keyText.font          = _font;

            // Ability name — bottom-center
            var nameGo  = new GameObject("AbilityName");
            nameGo.transform.SetParent(slotGo.transform, false);
            var nameRect  = nameGo.AddComponent<RectTransform>();
            nameRect.anchorMin = Vector2.zero;
            nameRect.anchorMax = Vector2.one;
            nameRect.offsetMin = new Vector2(2f,  2f);
            nameRect.offsetMax = new Vector2(-2f, -2f);
            var nameText        = nameGo.AddComponent<Text>();
            nameText.fontSize   = 10;
            nameText.color      = Color.white;
            nameText.alignment  = TextAnchor.LowerCenter;
            nameText.font       = _font;

            // Cooldown timer — large, centered, only visible during cooldown
            var cdGo  = new GameObject("CooldownTimer");
            cdGo.transform.SetParent(slotGo.transform, false);
            var cdRect  = cdGo.AddComponent<RectTransform>();
            cdRect.anchorMin = Vector2.zero;
            cdRect.anchorMax = Vector2.one;
            cdRect.sizeDelta = Vector2.zero;
            var cdText        = cdGo.AddComponent<Text>();
            cdText.fontSize   = 22;
            cdText.fontStyle  = FontStyle.Bold;
            cdText.color      = Color.white;
            cdText.alignment  = TextAnchor.MiddleCenter;
            cdText.font       = _font;
            cdText.text       = string.Empty;

            slotView.Initialize(bgImage, overlayImage, keyText, nameText, cdText);
            return slotView;
        }

        private static void StretchToParent(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
        }
    }
}
