using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UICard : MonoBehaviour
    {
        private Card _reference;

        private TextMeshProUGUI _text;
        private RectTransform _imageRectTransform;

        private void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _imageRectTransform = transform.Find("Canvas/Bg").GetComponent<RectTransform>();
        }

        private void Update()
        {
            _imageRectTransform.rotation *= Quaternion.Euler(1, 0, 0);
        }
    }
}