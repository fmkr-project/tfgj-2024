using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public enum CardRotationDirection
    {
        Up,
        Down
    }
    
    public class UICard : MonoBehaviour
    {
        private Card _reference;

        private TextMeshProUGUI _text;
        private RectTransform _imageRectTransform;

        private float _popTime = 0.3f;

        private void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            
            _imageRectTransform = transform.Find("Canvas/Bg").GetComponent<RectTransform>();
            _imageRectTransform.rotation = Quaternion.Euler(90, 0, 0);
        }

        public void SetupCard(Card refer)
        {
            _reference = refer;
            _text.SetText(refer.FlavorText);
        }

        public void Hide()
        {
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
                _imageRectTransform.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(0, 0, 0),
                        (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            _imageRectTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        public float GetAnimationTime()
        {
            return _popTime;
        }
    }
}