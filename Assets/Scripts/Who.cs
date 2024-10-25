using System;

public record Who
{
    public string Name;

    public Who(string n)
    {
        Name = n;
    }
    
    public string GetImageUrl()
    {
        // Images should be saved in Resources as .png images and should adopt snake_case.
        return String.Join("", Name.ToSnakeCase().Split(' '));
    }
}