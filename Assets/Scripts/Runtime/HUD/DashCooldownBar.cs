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

        private Canvas _canvas;
        private Image _fillImage;
        private RectTransform _fillRT;
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

            _fillRT.anchorMax = new Vector2(remaining / total, 1f);
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

            var fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(canvasGO.transform, false);
            _fillImage = fillGO.AddComponent<Image>();
            _fillImage.color = _barColor;
            _fillRT = fillGO.GetComponent<RectTransform>();
            _fillRT.anchorMin = Vector2.zero;
            _fillRT.anchorMax = Vector2.one;
            _fillRT.offsetMin = Vector2.zero;
            _fillRT.offsetMax = Vector2.zero;
        }
    }
}
