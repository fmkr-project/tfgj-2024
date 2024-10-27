using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum Selected
    {
        Up,
        Down
    }
    
    public class UICardManager : MonoBehaviour
    {
        public bool lastWasCorrect = true;
        
        [SerializeField] private GameObject innerObject;
        [SerializeField] private GameObject outerObject;
        [SerializeField] private GameObject questionObject;
        [SerializeField] private GameObject answerResultObject;
        
        private UICard _innerUI;
        private UICard _outerUI;
        private InnerCard _inner;
        private OuterCard _outer;

        private Image _answerResultImage;
        private float _answerAnimationOffset = 1200;
        private float _answerAnimationDuration = 0.35f;

        private QuestionText _question;

        private void Awake()
        {
            // Initialize card UI interfaces
            _innerUI = innerObject.GetComponent<UICard>();
            _outerUI = outerObject.GetComponent<UICard>();
            
            _answerResultImage = answerResultObject.GetComponentInChildren<Image>();
            _answerResultImage.rectTransform.localPosition = _answerAnimationOffset * Vector3.up;
            
            _question = questionObject.GetComponent<QuestionText>();
        }

        public IEnumerator ShowCards()
        {
            StartCoroutine(_innerUI.Show(CardRotationDirection.Down));
            StartCoroutine(_outerUI.Show(CardRotationDirection.Up));
            StartCoroutine(_question.Show(CardRotationDirection.Up));
            SoundManager.Fire("card");
            yield return null;
        }

        public IEnumerator HideCards()
        {
            StartCoroutine(_innerUI.Hide(CardRotationDirection.Down));
            StartCoroutine(_outerUI.Hide(CardRotationDirection.Up));
            StartCoroutine(_question.Hide(CardRotationDirection.Up));
            yield return null;
        }

        public float GetAnimationTime()
            // I know, this should be here and not in UICard, but who cares.
        {
            return _innerUI.GetAnimationTime();
        }

        public void UpdateCards(InnerCard inner, OuterCard outer)
        {
            _inner = inner;
            _outer = outer;
            _innerUI.SetupCard(inner);
            _outerUI.SetupCard(outer);
        }

        public IEnumerator Choose(Selected what, bool forced)
        {
            switch (what)
            {
                case Selected.Up:
                    StartCoroutine(_outerUI.Push());
                    break;
                case Selected.Down:
                    StartCoroutine(_innerUI.Push());
                    break;
            }

            // Animate the answer
            StartCoroutine(Card.CompareYear(_inner, _outer)
                ? AnswerAnimation(what == Selected.Down)
                : AnswerAnimation(what == Selected.Up));

            if (forced) yield break;
            if ((Card.CompareYear(_inner, _outer) && what == Selected.Down) ||
                (!Card.CompareYear(_inner, _outer) && what == Selected.Up))
            {
                // Correct answer.
                CardManager.Unlock(_inner);
                _inner.Pass();
                CardManager.Unlock(_outer);
                _outer.Pass();
            }
            else
            {
                _inner.Fail();
                _outer.Fail();
            }

            yield return null;
        }

        private IEnumerator AnswerAnimation(bool isCorrect)
        {
            // Animate a circle (ok) or a cross (no) depending on the answer.

            lastWasCorrect = isCorrect;
            // Play a sound.
            SoundManager.Fire(isCorrect ? "ok" : "no");
            
            // Two smoothersteps.
            _answerResultImage.sprite =
                isCorrect ? Resources.Load<Sprite>("ok") : Resources.Load<Sprite>("no");
            var elapsed = 0f;

            while (elapsed < _answerAnimationDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _answerAnimationDuration;
                _answerResultImage.rectTransform.localPosition = Vector3.Lerp(
                    _answerAnimationOffset * Vector3.up,
                    Vector3.zero,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            elapsed = 0f;
            yield return new WaitForSeconds(_answerAnimationDuration);
            
            while (elapsed < _answerAnimationDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _answerAnimationDuration;
                _answerResultImage.rectTransform.localPosition = Vector3.Lerp(
                    Vector3.zero,
                    _answerAnimationOffset * Vector3.down,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
        }

        public float AnswerAnimationDuration()
        {
            return 4 * _answerAnimationDuration;
        }
    }
}