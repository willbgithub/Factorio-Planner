// Prototype.cs
// A Factorio prototype. Parent class to items and recipes.
// 31 August 2026
// will b. gaming

using UnityEngine;

public abstract class Prototype
{
    // Constructor
    public Prototype(string prefabName, string englishName, string typeName, Sprite icon)
    {
        this.prefabName = prefabName;
        this.englishName = englishName;
        this.typeName = typeName;
        this.icon = icon;
    }
    // Mutators
    public void SetPrefabName(string prefabName)
    {
        this.prefabName = prefabName;
    }
    public void SetEnglishName(string englishName)
    {
        this.englishName = englishName;
    }
    public void SetType(string typeName)
    {
        this.typeName = typeName;
    }
    public void SetIcon(Sprite icon)
    {
        this.icon = icon;
    }
    // Accessors
    public string GetPrefabName()
    {
        return prefabName;
    }
    public string GetEnglishName()
    {
        return englishName;
    }
    public string GetTypeName()
    {
        return typeName;
    }
    public Sprite GetIcon()
    {
        return icon;
    }
    // Member data
    protected string prefabName;
    protected string englishName;
    protected string typeName;
    protected Sprite icon;
}
