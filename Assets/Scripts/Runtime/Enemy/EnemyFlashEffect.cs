using System.Collections;
using UnityEngine;

namespace MobaRoguelike.Runtime.Enemy
{
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyFlashEffect : MonoBehaviour
    {
        [SerializeField] private float _flashDuration = 0.2f;
        [SerializeField] private Color _flashColor    = Color.red;

        private EnemyHealth _health;
        private Renderer[]  _renderers;
        private Color[]     _originalColors;

        private void Awake()
        {
            _health    = GetComponent<EnemyHealth>();
            _renderers = GetComponentsInChildren<Renderer>();

            _originalColors = new Color[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++)
                _originalColors[i] = _renderers[i].material.color;
        }

        private void OnEnable()  => _health.OnDamaged += HandleDamaged;
        private void OnDisable() => _health.OnDamaged -= HandleDamaged;

        private void HandleDamaged(float _)
        {
            StopAllCoroutines();
            StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            SetFlashColor();

            float elapsed = 0f;
            while (elapsed < _flashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _flashDuration;
                for (int i = 0; i < _renderers.Length; i++)
                    _renderers[i].material.color = Color.Lerp(_flashColor, _originalColors[i], t);
                yield return null;
            }

            RestoreColors();
        }

        private void SetFlashColor()
        {
            foreach (var r in _renderers)
                r.material.color = _flashColor;
        }

        private void RestoreColors()
        {
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].material.color = _originalColors[i];
        }
    }
}
