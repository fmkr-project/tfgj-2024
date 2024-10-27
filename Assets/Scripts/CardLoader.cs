using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CardLoader
{
    private const string InnerCardFileName = "inner";
    private const string OuterCardFileName = "outer";

    private const string Separator = "; ";
    
    // Do not put semicolons in any field!
    public static List<InnerCard> LoadInnerCards()
    {
        List<InnerCard> final = new();
        TextAsset csvData = Resources.Load(InnerCardFileName) as TextAsset;
        StringReader reader = new StringReader(csvData.text);

        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            if (line is "" or null) continue;
            var values = line.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            
            var year = int.Parse(values[0]);
            var shortTitle = values[1];
            var flavor = values[2];
            var source = values[3];
            var description = values[4];
            var comments = values[5];
            
            final.Add(new InnerCard(year, shortTitle, flavor, description, comments, source));
        }

        return final;
    }

    public static List<OuterCard> LoadOuterCards()
    {
        List<OuterCard> final = new();
        TextAsset csvData = Resources.Load(OuterCardFileName) as TextAsset;
        StringReader reader = new StringReader(csvData.text);

        while (reader.Peek() != -1)
        {
            var line = reader.ReadLine();
            if (line is "" or null) continue;
            var values = line.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            
            var year = int.Parse(values[0]);
            var shortTitle = values[1];
            Debug.Log(shortTitle);
            var tag = values[2] switch
            {
                "event" => CardTag.Event,
                "people" => CardTag.People,
                "place" => CardTag.Place,
                "tech" => CardTag.Tech,
                "d_tech" => CardTag.DeadTech,
                _ => throw new ArgumentException($"Unknown tag for card: {shortTitle}!")
            };
            var kokunai = values[3] switch
            {
                "0" => false,
                "1" => true,
                _ => throw new ArgumentException($"Unknown boolean for card: {shortTitle}!")
            };
            GameDifficulty diff = values[4] switch
            {
                "easy" => GameDifficulty.Easy,
                "noteasy" => GameDifficulty.LessEasy,
                "akyuu" => GameDifficulty.Akyuu,
                _ => throw new ArgumentException($"Can't parse the difficulty for card: {shortTitle}!")
            };
            var flavor = values[5];
            var source = values[6];
            var description = values[7];
            var comments = values[8];
            
            final.Add(new OuterCard(
                year, tag, kokunai, diff, shortTitle, flavor, description, comments, source));
        }

        return final;
    }
}