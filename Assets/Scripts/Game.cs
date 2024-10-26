using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameDifficulty currentDifficulty;
    private List<OuterCard> _ignoredCards = new();
    
    private MainMenuScreen _mainMenuScreen;

    private List<OuterCard> _pool;
    
    private void Awake()
    {
        _mainMenuScreen = transform.Find("/Canvas").GetComponent<MainMenuScreen>();
        
        CardManager.LoadCards();
    }

    private void Start()
    {
        _mainMenuScreen.callTutorialTurn1.AddListener(ForceTutorial1);
        _mainMenuScreen.callTutorialTurn2.AddListener(ForceTutorial2);
        _mainMenuScreen.callNextTurn.AddListener(GetCards);
        _mainMenuScreen.callGame.AddListener(InitializeGame);
    }

    private void InitializeGame()
    {
        _ignoredCards = new List<OuterCard>();
        currentDifficulty = _mainMenuScreen.selectedDifficulty;
    }

    public void Turn()
    {
    }

    private void UpdateCards(string innerSt, string outerSt)
    {
        _mainMenuScreen.UpdateCardDisplay(
            CardManager.ForcePullInner(innerSt),
            CardManager.ForcePullOuter(outerSt)
            );
    }

    private void ForceTutorial1()
    {
        UpdateCards("Akyuu's birth", "American Civil War");
    }

    private void ForceTutorial2()
    {
        UpdateCards("Akyuu's birth", "Okinawa restitution");
    }

    private void GetCards()
    {
        var lowerLim = currentDifficulty switch
        {
            GameDifficulty.Easy => 10,
            GameDifficulty.LessEasy => 5,
            GameDifficulty.Akyuu => 1,
            _ => throw new ArgumentException("Unknown game difficulty!")
        };
        var upperLim = currentDifficulty switch
        {
            GameDifficulty.Easy => 999,
            GameDifficulty.LessEasy => 25,
            GameDifficulty.Akyuu => 10,
            _ => throw new ArgumentException("Unknown game difficulty!")
        };
        
        InnerCard inner = null;
        OuterCard outer = null;
        var rerolls = 0;
        // There should always be at least 8 cards for each inner and for each difficulty.
        // Safeguard just in case.
        while (outer is null && rerolls < 4 * CardManager.GetInnerCount())
        {
            try
            {
                inner = CardManager.PullInner();
                outer = CardManager.PullOuter(inner, lowerLim, upperLim, currentDifficulty, _ignoredCards);
            }
            catch (ArgumentOutOfRangeException e)
            {
                print("Rerolling due to card exhaustion.");
                rerolls++;
            }
        }

        // TODO this is absolutely not elegant
        UpdateCards(inner.ShortTitle, outer.ShortTitle);
        _ignoredCards.Add(outer);
    }
}