using System;
using UnityEngine;

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
        //return String.Join("", ShortTitle.ToSnakeCase().Split(' '));
        return ShortTitle.ToUrl();
    }

    public static bool CompareYear(InnerCard inner, OuterCard outer)
    {
        if (inner.Year == outer.GetTrueDate())
            throw new ArgumentException("Two cards with the same true year should not be used simultaneously!");
        return inner.Year < outer.GetTrueDate();
    }
}