using System;
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
        private Image _texture;
        private const float StepDuration = 0.8f;

        private void Awake()
        {
            _texture = transform.Find("Canvas/Texture").GetComponent<Image>();
            _rectTransform = _texture.GetComponent<RectTransform>();
        }

        public void MoveUp()
        {
            position = Math.Clamp(position - 1, 0, maxPosition - 1);
            UpdateAnchors();
        }

        public void MoveDown()
        {
            position = Math.Clamp(position + 1, 0, maxPosition - 1);
            UpdateAnchors();
        }

        public void Show()
        {
            _texture.enabled = true;
        }

        public void Hide()
        {
            _texture.enabled = false;
        }

        public void UpdateAnchors()
        {
            var correctedPosition = maxPosition - position - 1;
            _rectTransform.anchorMin = new Vector2(0.7f, endYAnchor + (0.12f * correctedPosition) + 0.025f);
            _rectTransform.anchorMax = new Vector2(0.73f, endYAnchor + (0.12f * correctedPosition) + 0.075f);
        }
    }
}
