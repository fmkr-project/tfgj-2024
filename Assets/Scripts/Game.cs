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
    
    public void Turn()
    {
    }
}