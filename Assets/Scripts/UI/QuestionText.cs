using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class QuestionText : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private RectTransform _rectTransform;

        private float _popTime = .3f;

        private void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _rectTransform = transform.Find("Canvas/Text").GetComponent<RectTransform>();
            _rectTransform.rotation = Quaternion.Euler(90, 0, 0);
        }

        public void ChangeText(string text)
        {
            _text.SetText(text);
        }
        
        public IEnumerator Show(CardRotationDirection fromDirection)
        {
            var initialRotation = fromDirection switch
            {
                CardRotationDirection.Up => Quaternion.Euler(-90, 0, 0),
                CardRotationDirection.Down => Quaternion.Euler(90, 0, 0)
            };
            
            // Smootherstep again.
            var elapsed = 0f;
            while (elapsed < _popTime)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _popTime;
                _rectTransform.localRotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(0, 0, 0),
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            _rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}