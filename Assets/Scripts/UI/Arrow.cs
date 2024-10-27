using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Arrow : MonoBehaviour
    {
        public int position;
        public int maxPosition;
        public float endYAnchor;

        private RectTransform _rectTransform;
        private float _animationDuration = 0.3f;

        private float _movementDuration = 0.05f;
        
        private Image _texture;
        private const float StepDuration = 0.8f;

        private void Awake()
        {
            _texture = transform.Find("Canvas/Texture").GetComponent<Image>();
            _rectTransform = transform.Find("Canvas/Texture").GetComponent<RectTransform>();
        }

        private void Start()
        {
            StartCoroutine(UpdateAnchors());
        }

        public void MoveUp()
        {
            var old = position;
            position = Math.Clamp(position - 1, 0, maxPosition - 1);
            if (old != position) SoundManager.Fire("tick");
            StartCoroutine(UpdateAnchors());
        }

        public void MoveDown()
        {
            var old = position;
            position = Math.Clamp(position + 1, 0, maxPosition - 1);
            if (old != position) SoundManager.Fire("tick");
            StartCoroutine(UpdateAnchors());
        }

        public IEnumerator Show()
        {
            var initialRotation = Quaternion.Euler(90, 0, 0);
            
            // Smootherstep again.
            var elapsed = 0f;
            while (elapsed < _animationDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _animationDuration;
                _rectTransform.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(0, 0, 0),
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            _rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public IEnumerator Hide()
        {
            var finalRotation = Quaternion.Euler(-90, 0, 0);
            
            // Smootherstep again.
            var elapsed = 0f;
            while (elapsed < _animationDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _animationDuration;
                _rectTransform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), finalRotation,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            _rectTransform.rotation = finalRotation;
        }

        public IEnumerator UpdateAnchors()
        {
            var correctedPosition = maxPosition - position - 1;
            var anchorMin = _rectTransform.anchorMin;
            var newAnchorMin = new Vector2(0.7f, endYAnchor + (0.12f * correctedPosition) + 0.025f);
            var anchorMax = _rectTransform.anchorMax;
            var newAnchorMax = new Vector2(0.73f, endYAnchor + (0.12f * correctedPosition) + 0.075f);
            
            // Smootherstep!
            var elapsed = 0f;
            while (elapsed < _movementDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _movementDuration;
                _rectTransform.anchorMin = Vector2.Lerp(
                    anchorMin,
                    newAnchorMin,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                _rectTransform.anchorMax = Vector2.Lerp(
                    anchorMax,
                    newAnchorMax,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            _rectTransform.anchorMin = newAnchorMin;
            _rectTransform.anchorMax = newAnchorMax;
        }
    }
}
