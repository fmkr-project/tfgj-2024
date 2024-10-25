using System;

public record Card
{
    public int Year;
    public string ShortTitle;
    public string FlavorText;
    public string Description;
    public string Comments;
    public string Source;

    public string GetImageUrl()
    {
        // Images should be saved in Resources as .png images and should adopt snake_case.
        return String.Join("", ShortTitle.ToSnakeCase().Split(' '));
    }
}