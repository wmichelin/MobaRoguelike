using UnityEngine;
using UnityEngine.UI;
using MobaRoguelike.Runtime.Hero;

namespace MobaRoguelike.Runtime.HUD
{
    [RequireComponent(typeof(HeroController))]
    public class DashCooldownBar : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset = new Vector3(0f, 2.5f, 0f);
        [SerializeField] private Vector2 _barSize = new Vector2(1f, 0.1f);
        [SerializeField] private Color _barColor = new Color(0.2f, 0.6f, 1f, 0.9f);
        [SerializeField] private Color _backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.6f);

        private Canvas _canvas;
        private Image _fillImage;
        private HeroController _hero;
        private Transform _cameraTransform;

        private void Awake()
        {
            _hero = GetComponent<HeroController>();
            CreateBar();
        }

        private void Start()
        {
            _cameraTransform = UnityEngine.Camera.main.transform;
            _canvas.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _hero.OnDashCooldownChanged += HandleCooldownChanged;
        }

        private void OnDisable()
        {
            _hero.OnDashCooldownChanged -= HandleCooldownChanged;
        }

        private void LateUpdate()
        {
            if (_canvas.gameObject.activeSelf && _cameraTransform != null)
            {
                _canvas.transform.rotation = _cameraTransform.rotation;
            }
        }

        private void HandleCooldownChanged(float remaining, float total)
        {
            if (remaining <= 0f)
            {
                _canvas.gameObject.SetActive(false);
                return;
            }

            if (!_canvas.gameObject.activeSelf)
                _canvas.gameObject.SetActive(true);

            _fillImage.fillAmount = remaining / total;
        }

        private void CreateBar()
        {
            var canvasGO = new GameObject("DashCooldownCanvas");
            canvasGO.transform.SetParent(transform, false);
            canvasGO.transform.localPosition = _offset;

            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.sortingOrder = 1;

            var rt = canvasGO.GetComponent<RectTransform>();
            rt.sizeDelta = _barSize;
            rt.localScale = Vector3.one;

            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(canvasGO.transform, false);
            var bgImage = bgGO.AddComponent<Image>();
            bgImage.color = _backgroundColor;
            var bgRT = bgGO.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;

            var fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(canvasGO.transform, false);
            _fillImage = fillGO.AddComponent<Image>();
            _fillImage.color = _barColor;
            _fillImage.type = Image.Type.Filled;
            _fillImage.fillMethod = Image.FillMethod.Horizontal;
            _fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            _fillImage.fillAmount = 1f;
            var fillRT = fillGO.GetComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = Vector2.one;
            fillRT.offsetMin = Vector2.zero;
            fillRT.offsetMax = Vector2.zero;
        }
    }
}
