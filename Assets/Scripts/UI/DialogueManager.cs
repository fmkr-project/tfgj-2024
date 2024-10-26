using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private GameObject keineBoxPrefab;
        [SerializeField] private GameObject otherBoxPrefab;

        private DialogueBox _keineBox;
        private DialogueBox _otherBox;

        private void Awake()
        {
            _keineBox = keineBoxPrefab.GetComponent<DialogueBox>();
            _otherBox = otherBoxPrefab.GetComponent<DialogueBox>();
        }

        private void Start()
        {
            _keineBox.gameObject.SetActive(false);
            _otherBox.gameObject.SetActive(false);
        }

        public void ChangeOtherTalking(Who newWho)
        {
            _otherBox.ChangeWhoSpeaks(newWho);
        }

        public IEnumerator KeineAppear(string text)
        {
            _keineBox.ChangeWhoSpeaks(new Who("Keine")); // Not very efficient
            _keineBox.gameObject.SetActive(true);
            _keineBox.ChangeDialogue(text);
            StartCoroutine(_keineBox.Appear(AnimationDirection.Left));
            yield return null;
        }

        public IEnumerator KeineRetire()
        {
            StartCoroutine(_keineBox.Retire(AnimationDirection.Left));
            yield return null;
        }

        public void KeineChangeDialogue(string text)
        {
            _keineBox.ChangeDialogue(text);
        }

        public void OtherChangeDialogue(string text)
        {
            _otherBox.ChangeDialogue(text);
        }

        public IEnumerator OtherAppear(string text)
        {
            _otherBox.gameObject.SetActive(true);
            _otherBox.ChangeDialogue(text);
            StartCoroutine(_otherBox.Appear(AnimationDirection.Right));
            yield return null;
        }

        public IEnumerator OtherRetire()
        {
            StartCoroutine(_otherBox.Retire(AnimationDirection.Right));
            yield return null;
        }

        public float GetAnimationWaitTime()
            // Whoops, _remoteTransition should actually be here
        {
            return 2 * _keineBox.GetAnimationWaitTime();
        }
    }
}