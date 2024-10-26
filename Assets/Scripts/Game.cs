using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class Game : MonoBehaviour
{
    private CardManager _cardManager = new();
    private MainMenuScreen _mainMenuScreen;

    private List<OuterCard> _pool;
    
    private void Awake()
    {
        _mainMenuScreen = transform.Find("/Canvas").GetComponent<MainMenuScreen>();
        
        _cardManager.LoadCards();
        for (int i = 0; i < 10; i++)
        {
            var test = _cardManager.PullOuter(_cardManager.PullInner(), 100, GameDifficulty.Akyuu);
            print(test);
        }
    }

    private void Start()
    {
        _mainMenuScreen.callTutorialTurn1.AddListener(ForceTutorial1);
        _mainMenuScreen.callTutorialTurn2.AddListener(ForceTutorial2);
        _mainMenuScreen.callNextTurn.AddListener(GetCards);
    }

    public void Turn()
    {
    }

    private void ForceGetCards(string innerSt, string outerSt)
    {
        _mainMenuScreen.UpdateCardDisplay(
            _cardManager.ForcePullInner(innerSt),
            _cardManager.ForcePullOuter(outerSt)
            );
    }

    private void ForceTutorial1()
    {
        ForceGetCards("Akyuu's birth", "American Civil War");
    }

    private void ForceTutorial2()
    {
        ForceGetCards("Akyuu's birth", "Okinawa restitution");
    }

    private void GetCards()
    {
        
    }
}