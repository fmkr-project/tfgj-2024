using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class CardManager
{
    public static List<InnerCard> Inner;
    public static List<OuterCard> Outer;

    private static Dictionary<Card, bool> UnlockStatus = new();

    public static Dictionary<Card, History> Progress = new();

    // JSON shenanigans.
    public static void LoadProgress(Dictionary<InnerCard, History> i, Dictionary<OuterCard, History> o)
    {
        foreach (var pair in i)
        {
            Progress.Add(pair.Key, pair.Value);
        }

        foreach (var pair in o)
        {
            Progress.Add(pair.Key, pair.Value);
        }
    }

    public static void LoadUnlocks(Dictionary<InnerCard, bool> i, Dictionary<OuterCard, bool> o)
    {
        foreach (var pair in i)
        {
            UnlockStatus.Add(pair.Key, pair.Value);
        }

        foreach (var pair in o)
        {
            UnlockStatus.Add(pair.Key, pair.Value);
        }
    }

    public static Dictionary<string, bool> SaveU()
    {
        var temp = new Dictionary<string, bool>();
        foreach (var pair in UnlockStatus)
        {
            temp.Add(pair.Key.ShortTitle, pair.Value);
        }

        return temp;
    }

    public static void LoadU(Dictionary<string, bool> u)
    {
        foreach (var pair in u)
        {
            try
            {
                UnlockStatus.Add(GetCardByName(pair.Key), pair.Value);
            }
            catch (ArgumentException)
            {
                UnlockStatus[GetCardByName(pair.Key)] = pair.Value;
            }
        }
    }

    public static void LoadP(Dictionary<string, List<int>> p)
    {
        foreach (var pair in p)
        {
            Debug.Log(pair);
            var c = new History
            {
                Ok = pair.Value[0],
                Seen = pair.Value[1]
            };
            try
            {
                Progress.Add(GetCardByName(pair.Key), c);
            }
            catch (ArgumentException)
            {
                Progress[GetCardByName(pair.Key)] = c;
            }
        }
    }

    public static Dictionary<string, List<int>> SaveP()
    {
        var temp = new Dictionary<string, List<int>>();
        foreach (var pair in Progress)
        {
            temp.Add(pair.Key.ShortTitle, new List<int> { Progress[pair.Key].Ok, Progress[pair.Key].Seen } );
        }

        return temp;
    }

    public static Dictionary<InnerCard, bool> SaveInnerUnlocks()
    {
        var temp = new Dictionary<InnerCard, bool>();
        foreach (var pair in UnlockStatus)
        {
            if (pair.Key is InnerCard key) temp.Add(key, pair.Value);
        }

        return temp;
    }
    
    public static Dictionary<OuterCard, bool> SaveOuterUnlocks()
    {
        var temp = new Dictionary<OuterCard, bool>();
        foreach (var pair in UnlockStatus)
        {
            if (pair.Key is OuterCard key) temp.Add(key, pair.Value);
        }

        return temp;
    }

    public static Dictionary<InnerCard, History> SaveInnerProgress()
    {
        var temp = new Dictionary<InnerCard, History>();
        foreach (var pair in Progress)
        {
            if (pair.Key is InnerCard key) temp.Add(key, pair.Value);
        }

        return temp;
    }
    
    public static Dictionary<OuterCard, History> SaveOuterProgress()
    {
        var temp = new Dictionary<OuterCard, History>();
        foreach (var pair in Progress)
        {
            if (pair.Key is OuterCard key) temp.Add(key, pair.Value);
        }

        return temp;
    }
    
    public static void LoadCards()
    {
        Inner = CardLoader.LoadInnerCards();
        Outer = CardLoader.LoadOuterCards();

        foreach (var inner in Inner)
        {
            UnlockStatus.Add(inner, false);
            Progress.Add(inner, new History());
            Debug.Log(inner.ShortTitle);
            Debug.Log(inner.GetImageUrl());
        }

        foreach (var outer in Outer)
        {
            UnlockStatus.Add(outer, false);
            Progress.Add(outer, new History());
            Debug.Log(outer.ShortTitle);
            Debug.Log(outer.GetImageUrl());
        }
    }

    public static Card GetCardByName(string st)
    {
        foreach (var c in Outer)
        {
            if (c.ShortTitle == st) return c;
        }

        foreach (var c in Inner)
        {
            if (c.ShortTitle == st) return c;
        }

        return null;
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

    public static bool CardIsUnlocked(Card c)
    {
        return UnlockStatus[c];
    }

    public static void Unlock(Card c)
    {
        UnlockStatus[c] = true;
    }
}