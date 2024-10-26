using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum CollectionSide
    {
        Inner,
        Outer
    }
    
    public class CollectionMenu : MonoBehaviour
    {
        [SerializeField] private GameObject arrow;
        private Vector3 _arrowStartPosition;
        private Vector2 _arrowStartAnchorMin;
        private Vector2 _arrowStartAnchorMax;
        private RectTransform _arrowRectTransform;
        
        [SerializeField] private GameObject textField1;
        [SerializeField] private GameObject textField2;
        [SerializeField] private GameObject textField3;
        [SerializeField] private GameObject textField4;
        [SerializeField] private GameObject textField5;

        private float _animationDuration = 0.22f;
        private float _animationOffset = 2200f;
        private float _arrowStep = 0.111f;
        
        private List<GameObject> _textFieldList;

        private RectTransform _bgRectTransform;
        private Vector3 _bgStartPosition;
        private RectTransform _cardRectTransform;
        private Vector3 _cardStartPosition;

        private CollectionSide _displayedSide = CollectionSide.Inner;
        private int _firstDisplayed;
        private List<Card> _displayedCards = new();
        private int _arrowPosition;

        public bool isReady;

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
            
            _arrowStartAnchorMin = _arrowRectTransform.anchorMin;
            _arrowStartAnchorMax = _arrowRectTransform.anchorMax;
            
            transform.Find("Canvas/Card/Image").GetComponent<Image>().preserveAspect = true;
        }

        public void ShowFrom(int first)
        {
            _firstDisplayed = first;
            if (_displayedSide == CollectionSide.Inner)
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
            
            if (_displayedSide == CollectionSide.Outer)
            {
                for (int i = first; i < first + 5; i++)
                {
                    try
                    {
                        _displayedCards.Add(CardManager.Outer[i]);
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
                _textFieldList[i].GetComponent<TextMeshProUGUI>().SetText(CardManager.CardIsUnlocked(_displayedCards[i])
                ? _displayedCards[i].Year.ToString()
                : "????");
                // Set flavor
                _textFieldList[i].transform.Find("Flavor").GetComponent<TextMeshProUGUI>()
                    .SetText(CardManager.CardIsUnlocked(_displayedCards[i])
                ? _displayedCards[i].FlavorText
                : "??? ? ???? ? ?????");
            }

            for (var i = _displayedCards.Count; i < 5; i++)
            {
                // Set to empty
                _textFieldList[i].GetComponent<TextMeshProUGUI>().SetText("");
                _textFieldList[i].transform.Find("Flavor").GetComponent<TextMeshProUGUI>()
                    .SetText("");
            }
            
            UpdateCard(_displayedCards[_arrowPosition]);
        }

        public IEnumerator Show()
        {
            _arrowPosition = 0;
            UpdateArrowTexture();
            
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

            isReady = true;
        }

        public IEnumerator Hide()
        {
            isReady = false;
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
            _arrowPosition = 0;
        }

        public void MoveArrowUp()
        {
            if (_arrowPosition == 0 && _firstDisplayed >= 5)
                // Try to move up if there are more cards to be displayed.
            {
                _firstDisplayed -= 5;
                _arrowPosition = 4;
                _displayedCards.Clear();
                ShowFrom(_firstDisplayed);
            }
            else _arrowPosition = Math.Clamp(_arrowPosition - 1, 0, Math.Min(_displayedCards.Count, 5) - 1);
            UpdateCard(_displayedCards[_arrowPosition]);
            UpdateArrowTexture();
        }

        public void MoveArrowDown()
        {
            if (_arrowPosition == 4 &&
                ((_firstDisplayed < CardManager.Inner.Count - 5 && _displayedSide == CollectionSide.Inner) ||
                (_firstDisplayed < CardManager.Outer.Count - 5 && _displayedSide == CollectionSide.Outer)))
                // Try to move down if there are more cards to be displayed.
            {
                _firstDisplayed += 5;
                _arrowPosition = 0;
                _displayedCards.Clear();
                ShowFrom(_firstDisplayed);
            }
            else _arrowPosition = Math.Clamp(_arrowPosition + 1, 0, Math.Min(_displayedCards.Count, 5) - 1);
            UpdateCard(_displayedCards[_arrowPosition]);
            UpdateArrowTexture();
        }

        public void PreviousPage()
        {
            if (_firstDisplayed >= 5)
            {
                _firstDisplayed -= 5;
                _arrowPosition = 0;
                _displayedCards.Clear();
                ShowFrom(_firstDisplayed);
            }
        }
        
        public void NextPage()
        {
            if ((_firstDisplayed < CardManager.Inner.Count - 5 && _displayedSide == CollectionSide.Inner) ||
                (_firstDisplayed < CardManager.Outer.Count - 5 && _displayedSide == CollectionSide.Outer))
            {
                _firstDisplayed += 5;
                _arrowPosition = 0;
                _displayedCards.Clear();
                ShowFrom(_firstDisplayed);
            }
        }

        public void SwitchSide(CollectionSide cs)
        {
            _displayedSide = cs;
            _displayedCards.Clear();
            _arrowPosition = 0;
            UpdateArrowTexture();
            ShowFrom(0);
        }

        private void UpdateArrowTexture()
        {
            _arrowRectTransform.anchorMin = _arrowStartAnchorMin - new Vector2(0, _arrowStep * _arrowPosition);
            _arrowRectTransform.anchorMax = _arrowStartAnchorMax - new Vector2(0, _arrowStep * _arrowPosition);
        }

        private void UpdateCard(Card reference)
        {
            var isUnlocked = CardManager.CardIsUnlocked(reference);
            
            // Update card color
            var bg = transform.Find("Canvas/Card").GetComponent<Image>();
            if (!isUnlocked)
                bg.color = new Color(0.7f, 0.7f, 0.7f);
            else
            {
                if (reference is InnerCard)
                    bg.color = new Color(0.8f, 0.5f, 0.5f);
                else
                    bg.color = ((OuterCard) reference).Tag switch
                    {
                        CardTag.Event => new Color(0.7f, 0.7f, 0.9f),
                        CardTag.People => new Color(1f, 0.8f, 0.6f),
                        CardTag.Place => new Color(0.7f, 1f, 0.75f),
                        CardTag.Tech => new Color(0.55f, 0.55f, 0.8f),
                        CardTag.DeadTech => new Color(0.55f, 0.55f, 0.55f)
                    };
            }
            
            // Update image
            transform.Find("Canvas/Card/Image").GetComponent<Image>().sprite =
                Resources.Load<Sprite>(isUnlocked
                    ? (Resources.Load<Sprite>(reference.GetImageUrl()) is not null 
                        ? reference.GetImageUrl()
                        : "image_not_found")
                    : "unknown_card");
            
            // Update title
            transform.Find("Canvas/Card/ShortTitle").GetComponent<TextMeshProUGUI>().SetText(
                isUnlocked ? reference.ShortTitle : "??????????");
            
            // Update source
            transform.Find("Canvas/Card/Source").GetComponent<TextMeshProUGUI>().SetText(
                isUnlocked ? reference.Source : "?");
            
            // Update description
            transform.Find("Canvas/Card/Desc").GetComponent<TextMeshProUGUI>().SetText(
                isUnlocked ? reference.Description : "??? ? ???? ? ????? ????????? ?? ?????? ????? ??? ????? ???????? ????????? ??????? ????????? ???");
            
            // Update comments
            transform.Find("Canvas/Card/Comm").GetComponent<TextMeshProUGUI>().SetText(
                isUnlocked ? reference.Comments : "??? ? ???? ? ????? ????????? ?? ?????? ????? ??? ?????");
        }
    }
}