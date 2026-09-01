// Prototype.cs
// A Factorio prototype. Parent class to items and recipes.
// 31 August 2026
// will b. gaming

using UnityEngine;

public class Prototype
{
    public Prototype(string name)
    {
        Debug.Log("PROTOTYPE: Constructor on \"" + name + "\"");
        this.name = name;
    }
    public void SetName(string name)
    {
        this.name = name;
    }
    public string GetName()
    {
        return name;
    }
    string name;
}
