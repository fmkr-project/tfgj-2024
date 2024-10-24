using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MenuItem : MonoBehaviour
    {
        private TextMeshProUGUI _label;

        public void Awake()
        {
            _label = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetLabel(string text)
        {
            _label.SetText(text);
        }

        public void SetStartYAnchor(float yAnchor)
        {
            var border = transform.Find("Canvas/Border");
            var rectTransform = border.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.75f, yAnchor);
            rectTransform.anchorMax = new Vector2(0.95f, yAnchor + 0.1f);
        }
    }
}
