using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Timer : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Vector2 _startPosition;
        private TextMeshProUGUI _timerText;
        private TextMeshProUGUI _scoreText;

        public float time;
        public int ok;
        public int done;
    
        private float _animationDuration = 0.33f;
        private float _animationOffset = 200f;
        
        public bool isRunning;

        private void Awake()
        {
            _rectTransform = transform.Find("Canvas/Bg").GetComponent<RectTransform>();
            _timerText = transform.Find("Canvas/Bg/Text").GetComponent<TextMeshProUGUI>();
            _scoreText = transform.Find("Canvas/Bg/Score").GetComponent<TextMeshProUGUI>();
        }
        
        private void Start()
        {
            _startPosition = _rectTransform.localPosition;
            _rectTransform.localPosition += _animationOffset * Vector3.up;
        }

        public void Go()
        {
            isRunning = true;
        }

        public void Halt()
        {
            isRunning = false;
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
                    _startPosition + _animationOffset * Vector2.up,
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
                    _startPosition + _animationOffset * Vector2.up,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
        }

        public void Refresh()
        {
            _timerText.SetText(Math.Max(time, 0).ToString("N1", CultureInfo.InvariantCulture));
            _scoreText.SetText($"<{ok}/{done}>");
        }

        public void ResetScore()
        {
            ok = 0;
            done = 0;
        }

        private void Update()
        {
            if (isRunning) time -= Time.deltaTime;
            Refresh();
        }

        public IEnumerator ArtificialTimerDecrease(float duration)
        {
            print("fir");
            var elapsed = 0f;
            while (elapsed < duration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                time -= deltaTime;
                Refresh();
                yield return new WaitForSeconds(deltaTime);
            }
            print("ok");
        }
    }
}