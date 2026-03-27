using UnityEngine;
using Unity.Profiling;

namespace MobaRoguelike.Runtime.Camera
{
    [UnityEngine.DefaultExecutionOrder(10)]
    public class IsometricCameraController : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothTime = 0.1f;

        private Vector3 _offset;
        private Vector3 _velocity;

        private static readonly ProfilerMarker s_LateUpdateMarker =
            new ProfilerMarker("IsometricCameraController.LateUpdate");

        private void Awake()
        {
            transform.rotation = Quaternion.Euler(53f, 45f, 0f);
            if (_target != null)
                _offset = transform.position - _target.position;
        }

        private void LateUpdate()
        {
            using (s_LateUpdateMarker.Auto())
            {
                if (_target == null) return;

                Vector3 targetPos = _target.position + _offset;
                transform.position = Vector3.SmoothDamp(
                    transform.position, targetPos, ref _velocity, _smoothTime);
            }
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            if (_target != null)
                _offset = transform.position - _target.position;
        }
    }
}
