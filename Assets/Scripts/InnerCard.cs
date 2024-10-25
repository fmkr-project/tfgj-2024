public record InnerCard : Card
{
    public InnerCard(int y, string st, string ft, string desc, string com, string src)
    {
        Year = y;
        ShortTitle = st;
        FlavorText = ft;
        Description = desc;
        Comments = com;
        Source = src;
    }
}