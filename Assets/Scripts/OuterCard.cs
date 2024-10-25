
using System;

public enum CardTag
{
    Event,
    People,
    Place,
    Tech,
    DeadTech
}

public record OuterCard : Card
{
    public CardTag Tag;
    public bool Kokunai;
    public GameDifficulty GameDifficulty;

    public OuterCard(int y, CardTag tag, bool k, GameDifficulty gd, string st, string ft, string desc, string com, string src)
    {
        Year = y;
        Tag = tag;
        Kokunai = k;
        GameDifficulty = gd;
        ShortTitle = st;
        FlavorText = ft;
        Description = desc;
        Comments = com;
        Source = src;
    }

    public int GetTrueDate()
    // Also defines date thresholds.
    {
        var k = Kokunai ? 2 : 1;
        return Tag switch
        {
            CardTag.Event => Year + k * 15,
            CardTag.People => Year + k * 25,
            CardTag.Place => Year + k * 10,
            CardTag.Tech => Year + 30,
            CardTag.DeadTech => Year,
            _ => throw new Exception($"Unknown type when trying to calculate true date for: {ShortTitle}")
        };
    }
}