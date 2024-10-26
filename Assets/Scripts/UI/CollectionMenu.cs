using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CollectionMenu : MonoBehaviour
    {
        [SerializeField] private GameObject arrow;
        private Vector3 _arrowStartPosition;
        private RectTransform _arrowRectTransform;
        
        [SerializeField] private GameObject textField1;
        [SerializeField] private GameObject textField2;
        [SerializeField] private GameObject textField3;
        [SerializeField] private GameObject textField4;
        [SerializeField] private GameObject textField5;

        private float _animationDuration = 0.22f;
        private float _animationOffset = 2200f;
        private float _arrowStep = 0.14f;
        
        private List<GameObject> _textFieldList;

        private RectTransform _bgRectTransform;
        private Vector3 _bgStartPosition;
        private RectTransform _cardRectTransform;
        private Vector3 _cardStartPosition;

        private bool _isDisplayingInner = true;
        private int _firstDisplayed;
        private List<Card> _displayedCards = new();
        private int _arrowPosition;

        private void Awake()
        {
            _textFieldList = new List<GameObject> { textField1, textField2, textField3, textField4, textField5 };
            _bgRectTransform = transform.Find("Canvas/Bg").GetComponent<RectTransform>();
            _bgStartPosition = _bgRectTransform.localPosition;
            _bgRectTransform.localPosition -= _animationOffset * Vector3.down;
            _cardRectTransform = transform.Find("Canvas/Card").GetComponent<RectTransform>();
            _cardStartPosition = _cardRectTransform.localPosition;
            _cardRectTransform.localPosition -= _animationOffset * Vector3.down;
            _arrowRectTransform = transform.Find("Canvas/Arrow").GetComponent<RectTransform>();
            _arrowStartPosition = _arrowRectTransform.localPosition;
            _arrowRectTransform.localPosition -= _animationOffset * Vector3.down;
            
            transform.Find("Canvas/Card/Image").GetComponent<Image>().preserveAspect = true;
        }

        public void ShowFrom(int first)
        {
            _firstDisplayed = first;
            if (_isDisplayingInner)
            {
                for (int i = first; i < first + 5; i++)
                {
                    try
                    {
                        _displayedCards.Add(CardManager.Inner[i]);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        break;
                    }
                }
            }

            for (var i = 0; i < _displayedCards.Count; i++)
            {
                // Set year
                _textFieldList[i].GetComponent<TextMeshProUGUI>().SetText(_displayedCards[i].Year.ToString());
                // Set flavor
                _textFieldList[i].transform.Find("Flavor").GetComponent<TextMeshProUGUI>()
                    .SetText(_displayedCards[i].FlavorText);
            }

            for (var i = _displayedCards.Count; i < 5; i++)
            {
                // Set to empty
                _textFieldList[i].GetComponent<TextMeshProUGUI>().SetText("");
                _textFieldList[i].transform.Find("Flavor").GetComponent<TextMeshProUGUI>()
                    .SetText("");
            }
        }

        public IEnumerator Show()
        {
            var elapsed = 0f;

            // Some extreme boilerplate here
            while (elapsed < _animationDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _animationDuration;
                _arrowRectTransform.localPosition = Vector3.Lerp(
                    _arrowStartPosition + _animationOffset * Vector3.down,
                    _arrowStartPosition,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                _bgRectTransform.localPosition = Vector3.Lerp(
                    _bgStartPosition + _animationOffset * Vector3.down,
                    _bgStartPosition,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                _cardRectTransform.localPosition = Vector3.Lerp(
                    _cardStartPosition + _animationOffset * Vector3.down,
                    _cardStartPosition,
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
                _arrowRectTransform.localPosition = Vector3.Lerp(
                    _arrowStartPosition,
                    _arrowStartPosition + _animationOffset * Vector3.down,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                _bgRectTransform.localPosition = Vector3.Lerp(
                    _bgStartPosition,
                    _bgStartPosition + _animationOffset * Vector3.down,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                _cardRectTransform.localPosition = Vector3.Lerp(
                    _cardStartPosition,
                    _cardStartPosition + _animationOffset * Vector3.down,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            _displayedCards.Clear();
        }

        public void MoveArrowUp()
        {
            _arrowPosition = Math.Clamp(_arrowPosition - 1, 0, Math.Min(_displayedCards.Count, 5) - 1);
            UpdateCard(_displayedCards[_arrowPosition]);
        }

        public void MoveArrowDown()
        {
            _arrowPosition = Math.Clamp(_arrowPosition + 1, 0, Math.Min(_displayedCards.Count, 5) - 1);
            UpdateCard(_displayedCards[_arrowPosition]);
        }

        public void SwitchSide()
        {
            _isDisplayingInner = !_isDisplayingInner;
        }

        private void UpdateCard(Card reference)
        {
            // Update image
            transform.Find("Canvas/Card/Image").GetComponent<Image>().sprite =
                Resources.Load<Sprite>(Resources.Load<Sprite>(reference.GetImageUrl()) is not null
                    ? reference.GetImageUrl()
                    : "unknown_card");
            
            // Update title
            transform.Find("Canvas/Card/ShortTitle").GetComponent<TextMeshProUGUI>().SetText(reference.ShortTitle);
            
            // Update source
            transform.Find("Canvas/Card/Source").GetComponent<TextMeshProUGUI>().SetText(reference.Source);
            
            // Update description
            transform.Find("Canvas/Card/Desc").GetComponent<TextMeshProUGUI>().SetText(reference.Description);
            
            // Update comments
            transform.Find("Canvas/Card/Comm").GetComponent<TextMeshProUGUI>().SetText(reference.Comments);
        }
    }
}