using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class CardManager
{
    public static List<InnerCard> Inner;
    public static List<OuterCard> Outer;

    private static List<Card> _all = new();

    private static Dictionary<Card, bool> UnlockStatus = new();

    public static Dictionary<Card, History> Progress = new();
    
    public static Dictionary<Card, float> Time = new();

    public static Dictionary<Card, float> BestTime = new();

    // JSON shenanigans.
    public static Dictionary<string, float> SaveT()
    {
        return Time.ToDictionary(pair => pair.Key.ShortTitle, pair => pair.Value);
    }

    public static void LoadT(Dictionary<string, float> t)
    {
        foreach (var pair in t)
        {
            try
            {
                Time.Add(GetCardByName(pair.Key), pair.Value);
            }
            catch (ArgumentException)
            {
                Time[GetCardByName(pair.Key)] = pair.Value;
            }
        }
        
        // Add cards that didn't exist in the save file.
        // This allows the game to be played if cards change.
        // TODO what if cards are removed?
        foreach (var notFound in _all.Except(t.Keys.Select(GetCardByName)).ToList())
        {
            Time[notFound] = 0;
        }
    }

    public static Dictionary<string, float> SaveB()
    {
        return BestTime.ToDictionary(pair => pair.Key.ShortTitle, pair => pair.Value);
    }

    public static void LoadB(Dictionary<string, float> b)
    {
        foreach (var pair in b)
        {
            try
            {
                BestTime.Add(GetCardByName(pair.Key), pair.Value);
            }
            catch (ArgumentException)
            {
                BestTime[GetCardByName(pair.Key)] = pair.Value;
            }
        }
    }

    public static Dictionary<string, bool> SaveU()
    {
        return UnlockStatus.ToDictionary(pair => pair.Key.ShortTitle, pair => pair.Value);
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
        
        foreach (var notFound in _all.Except(u.Keys.Select(GetCardByName)).ToList())
        {
            UnlockStatus[notFound] = false;
        }
    }

    public static void LoadP(Dictionary<string, List<int>> p)
    {
        foreach (var pair in p)
        {
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
        
        foreach (var notFound in _all.Except(p.Keys.Select(GetCardByName)).ToList())
        {
            Progress[notFound] = new History
            {
                Ok = 0,
                Seen = 0
            };
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
    
    public static void LoadCards()
    {
        Inner = CardLoader.LoadInnerCards();
        Outer = CardLoader.LoadOuterCards();

        foreach (var inner in Inner)
        {
            UnlockStatus.Add(inner, false);
            Progress.Add(inner, new History());
            Time.Add(inner, 0f);

            _all.Add(inner);
        }

        foreach (var outer in Outer)
        {
            UnlockStatus.Add(outer, false);
            Progress.Add(outer, new History());
            Time.Add(outer, 0f);
            
            _all.Add(outer);
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

    public static void AddTime(Card c, float t)
    {
        Time[c] += t;
        if (!BestTime.TryAdd(c, t))
        {
            if (BestTime[c] >= t) BestTime[c] = t;
        }
    }

    public static float ReturnAvg(Card c)
    {
        return Time[c] / Progress[c].Ok;
    }

    public static float ReturnBest(Card c)
    {
        try
        {
            return BestTime[c];
        }
        catch (Exception)
        {
            return 999;
        }
    }
}