public record OuterCard : Card
{
    public string Tag;
    public bool Kokunai;
    public GameDifficulty GameDifficulty;

    public OuterCard(int y, string tag, bool k, GameDifficulty gd, string st, string ft, string desc, string com, string src)
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
}