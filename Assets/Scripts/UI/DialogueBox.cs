using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum AnimationDirection
    {
        Left,
        Right
    }
    public class DialogueBox : MonoBehaviour
    {
        public Who WhoSpeaks;
        private Image _portrait;
        private TextMeshProUGUI _dialogueText;
        private TextMeshProUGUI _whoText;

        private float _remoteDistance = 2000;
        private float _remoteTransition = 0.25f;

        private void Awake()
        {
            _portrait = transform.Find("Canvas/Pp").GetComponent<Image>();
            _dialogueText = transform.Find("Canvas/Bg/DialogueText").GetComponent<TextMeshProUGUI>();
            _whoText = transform.Find("Canvas/Who/WhoText").GetComponent<TextMeshProUGUI>();
        }

        public void ChangeWhoSpeaks(Who who)
        {
            WhoSpeaks = who;
            _whoText.SetText(who.Name);
            _portrait.sprite = Resources.Load<Sprite>(who.GetImageUrl());
        }

        public void ChangeDialogue(string text)
        {
            _dialogueText.SetText(text);
        }

        public IEnumerator Appear(AnimationDirection direction)
        {
            var initialPosition = direction switch
            {
                AnimationDirection.Left => new Vector2(-_remoteDistance, 0),
                AnimationDirection.Right => new Vector2(_remoteDistance, 0),
                _ => throw new Exception("Unknown animation direction!")
            };
            transform.position = initialPosition;

            // Smootherstep again.
            var elapsed = 0f;
            while (elapsed < _remoteTransition)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _remoteTransition;
                transform.localPosition = Vector2.Lerp(initialPosition, Vector2.zero,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
            
            // Safeguard
            transform.localPosition = Vector2.zero;
        }

        public IEnumerator Retire(AnimationDirection direction)
        {
            var finalPosition = direction switch
            {
                AnimationDirection.Left => new Vector2(-_remoteDistance, 0),
                AnimationDirection.Right => new Vector2(_remoteDistance, 0),
                _ => throw new Exception("Unknown animation direction!")
            };

            var elapsed = 0f;
            while (elapsed < _remoteTransition)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _remoteTransition;
                transform.localPosition = Vector2.Lerp(Vector2.zero, finalPosition,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
            
            transform.localPosition = finalPosition;
            gameObject.SetActive(false);
        }

        public float GetAnimationWaitTime()
        {
            return _remoteTransition;
        }
    }
}