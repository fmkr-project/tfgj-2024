using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    public class UICardManager : MonoBehaviour
    {
        [SerializeField] private GameObject innerObject;
        [SerializeField] private GameObject outerObject;
        [SerializeField] private GameObject questionObject;
        
        private UICard _innerUI;
        private UICard _outerUI;
        private InnerCard _inner;
        private OuterCard _outer;

        private QuestionText _question;

        private void Awake()
        {
            // Initialize card UI interfaces
            _innerUI = innerObject.GetComponent<UICard>();
            _outerUI = outerObject.GetComponent<UICard>();
            
            _question = questionObject.GetComponent<QuestionText>();
        }

        private void Start()
        {
            _innerUI.Hide();
            _outerUI.Hide();
        }

        public IEnumerator ShowCards()
        {
            StartCoroutine(_innerUI.Show(CardRotationDirection.Down));
            StartCoroutine(_outerUI.Show(CardRotationDirection.Up));
            StartCoroutine(_question.Show(CardRotationDirection.Up));
            yield return null;
        }

        public float GetAnimationTime()
            // I know, this should be here and not in UICard, but who cares.
        {
            return _innerUI.GetAnimationTime();
        }
    }
}