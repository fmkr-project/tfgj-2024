using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Arrow : MonoBehaviour
    {
        public int position;
        public int maxPosition;

        private Image _texture;

        private void Awake()
        {
            _texture = transform.Find("Canvas/Texture").GetComponent<Image>();
        }

        private void MoveUp()
        {
            position = Math.Clamp(position + 1, 0, maxPosition - 1);
        }

        private void MoveDown()
        {
            position = Math.Clamp(position - 1, 0, maxPosition - 1);
        }
    }
}
