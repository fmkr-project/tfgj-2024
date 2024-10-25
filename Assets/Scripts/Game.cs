using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    private CardManager _cardManager = new();
    
    private void Awake()
    {
        _cardManager.LoadCards();
        for (int i = 0; i < 10; i++)
        {
            var test = _cardManager.PullOuter(_cardManager.PullInner(), 100, GameDifficulty.Akyuu);
            print(test);
        }
    }
}