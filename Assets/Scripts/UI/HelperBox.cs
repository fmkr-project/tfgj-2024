using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    public class HelperBox : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Vector2 _startPosition;
    
        private float _animationDuration = 0.33f;
        private float _animationOffset = 550f;
    
        private void Awake()
        {
            _rectTransform = transform.Find("Canvas/Bg").GetComponent<RectTransform>();
        }

        private void Start()
        {
            _startPosition = _rectTransform.localPosition;
            _rectTransform.localPosition += _animationOffset * Vector3.right;
        }

        public IEnumerator Show()
        {
            var elapsed = 0f;

            while (elapsed < _animationDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _animationDuration;
                _rectTransform.localPosition = Vector3.Lerp(
                    _startPosition + _animationOffset * Vector2.right,
                    _startPosition,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
        }

        public IEnumerator Hide()
        {
            var elapsed = 0f;

            while (elapsed < _animationDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _animationDuration;
                _rectTransform.localPosition = Vector3.Lerp(
                    _startPosition,
                    _startPosition + _animationOffset * Vector2.right,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return null;
            }
        }

        public float GetAnimationTime()
        {
            return _animationDuration;
        }
    }
}
