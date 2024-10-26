using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class CardManager
{
    public static List<InnerCard> Inner;
    public static List<OuterCard> Outer;

    public static void LoadCards()
    {
        Inner = CardLoader.LoadInnerCards();
        Outer = CardLoader.LoadOuterCards();
    }

    public static InnerCard PullInner()
    {
        var rand = new Random();
        return Inner[rand.Next(Inner.Count)];
    }

    public static InnerCard ForcePullInner(string shortTitle)
    {
        // Use only during the tutorial!
        return Inner.FirstOrDefault(card => card.ShortTitle == shortTitle);
    }

    public static OuterCard ForcePullOuter(string shortTitle)
    {
        // Use only during the tutorial!
        return Outer.FirstOrDefault(card => card.ShortTitle == shortTitle);
    }

    public static OuterCard PullOuter(InnerCard reference, int lowerLim, int upperLim, GameDifficulty diff,
        List<OuterCard> ignored)
    {
        // Throws an ArgumentOutOfRange on card exhaustion. In that case, reroll.
        var rand = new Random();
        
        return Outer
            .Where(item => (diff == GameDifficulty.Easy && item.GameDifficulty == GameDifficulty.Easy)
                           || (diff == GameDifficulty.LessEasy &&
                               item.GameDifficulty is GameDifficulty.Easy or GameDifficulty.LessEasy)
                           || (diff == GameDifficulty.Akyuu &&
                               item.GameDifficulty is GameDifficulty.LessEasy or GameDifficulty.Akyuu)
            )
            .Where(item => Math.Abs(item.GetTrueDate() - reference.Year) > lowerLim
                           && Math.Abs(item.GetTrueDate() - reference.Year) < upperLim)
            .OrderBy(item => rand.Next())
            .Except(ignored)
            .ElementAt(0);
    }

    public static int GetInnerCount()
    {
        return Inner.Count;
    }

    public static int GetOuterCount()
    {
        return Outer.Count;
    }
}