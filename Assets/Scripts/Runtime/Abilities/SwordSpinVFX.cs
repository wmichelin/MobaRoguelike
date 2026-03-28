using System.Collections;
using UnityEngine;
using MobaRoguelike.Core.Abilities;

namespace MobaRoguelike.Runtime.Abilities
{
    /// <summary>
    /// Shows a fading circle ring at cast time for the Sword Spin ability.
    /// Auto-added alongside DefaultAbilitiesSetup — no manual wiring required.
    /// </summary>
    [RequireComponent(typeof(AbilityCasterBridge))]
    public class SwordSpinVFX : MonoBehaviour
    {
        [SerializeField] private int   _segments = 48;
        [SerializeField] private float _duration = 0.45f;
        [SerializeField] private float _lineWidth = 0.12f;
        [SerializeField] private Color _color = new Color(1f, 0.75f, 0.1f, 1f);

        private AbilityCasterBridge _bridge;

        private void Awake()
        {
            _bridge = GetComponent<AbilityCasterBridge>();
        }

        private void OnEnable()
        {
            if (_bridge != null)
                _bridge.Caster.OnAbilityCast += HandleAbilityCast;
        }

        private void OnDisable()
        {
            if (_bridge != null)
                _bridge.Caster.OnAbilityCast -= HandleAbilityCast;
        }

        private void HandleAbilityCast(AbilitySlot slot)
        {
            if (slot != AbilitySlot.Q) return;

            float radius = 2f;
            var data = _bridge.Caster.GetAbility(AbilitySlot.Q);
            if (data?.Effect is SwordSpinEffect spin)
                radius = spin.Radius;

            StartCoroutine(DrawCircle(transform.position, radius));
        }

        private IEnumerator DrawCircle(Vector3 center, float radius)
        {
            var go = new GameObject("SwordSpinVFX_Circle");
            var lr = go.AddComponent<LineRenderer>();

            lr.loop            = true;
            lr.positionCount   = _segments + 1;
            lr.startWidth      = _lineWidth;
            lr.endWidth        = _lineWidth;
            lr.useWorldSpace   = true;
            lr.startColor      = _color;
            lr.endColor        = _color;

            for (int i = 0; i <= _segments; i++)
            {
                float angle = i / (float)_segments * Mathf.PI * 2f;
                go.GetComponent<LineRenderer>().SetPosition(i,
                    center + new Vector3(Mathf.Cos(angle) * radius, 0.05f, Mathf.Sin(angle) * radius));
            }

            float elapsed = 0f;
            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float alpha   = 1f - elapsed / _duration;
                var fadeColor = _color;
                fadeColor.a   = alpha;
                lr.startColor = fadeColor;
                lr.endColor   = fadeColor;
                yield return null;
            }

            Destroy(go);
        }
    }
}
