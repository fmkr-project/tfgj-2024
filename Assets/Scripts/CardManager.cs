using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class CardManager
{
    private List<InnerCard> _inner;
    private List<OuterCard> _outer;

    public void LoadCards()
    {
        _inner = CardLoader.LoadInnerCards();
        _outer = CardLoader.LoadOuterCards();
    }

    public InnerCard PullInner()
    {
        var rand = new Random();
        return _inner[rand.Next(_inner.Count)];
    }

    public OuterCard PullOuter(InnerCard reference, int threshold, GameDifficulty diff)
    {
        var rand = new Random();
        try
        {
            return _outer
                .Where(item => (diff == GameDifficulty.Easy && item.GameDifficulty == GameDifficulty.Easy)
                               || (diff == GameDifficulty.LessEasy &&
                                   item.GameDifficulty is GameDifficulty.Easy or GameDifficulty.LessEasy)
                               || (diff == GameDifficulty.Akyuu &&
                                   item.GameDifficulty is GameDifficulty.LessEasy or GameDifficulty.Akyuu)
                )
                .Where(item => Math.Abs(item.Year - reference.Year) < threshold)
                .OrderBy(item => rand.Next())
                .ElementAt(0);
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning("No card corresponding to these criteria found!");
            return null;
        }
    }
}