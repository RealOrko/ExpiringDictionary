namespace ExpiringDictionaryWrapper.Tests.Models;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public override string ToString() => $"{Name},{Age}";
    public static Person FromString(string s)
    {
        var parts = s.Split(',');
        return new Person { Name = parts[0], Age = int.Parse(parts[1]) };
    }

    public override bool Equals(object obj) =>
        obj is Person p && Name == p.Name && Age == p.Age;

    public override int GetHashCode() => HashCode.Combine(Name, Age);
}