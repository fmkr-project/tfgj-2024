using System;
using UnityEngine;

public struct History : IEquatable<History>
{
    public int Ok;
    public int Seen;

    public bool Equals(History other)
    {
        return Ok == other.Ok && Seen == other.Seen;
    }

    public override bool Equals(object obj)
    {
        return obj is History other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Ok, Seen);
    }
}

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

    public void Pass(float t)
    {
        var temp = CardManager.Progress[this];
        temp.Ok++;
        temp.Seen++;
        CardManager.Progress[this] = temp;
        CardManager.AddTime(this, t);
    }

    public void Fail()
    {
        var temp = CardManager.Progress[this];
        temp.Seen++;
        CardManager.Progress[this] = temp;
    }
}