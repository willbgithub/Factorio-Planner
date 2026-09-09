// Prototype.cs
// A Factorio prototype. Parent class to items and recipes.
// 31 August 2026
// will b. gaming

using System;
using UnityEngine;
[Serializable]
public abstract class Prototype : ScriptableObject
{
    // Constructor
    protected Prototype(string prefabName, string englishName, string typeName, Sprite icon)
    {
        this.prefabName = prefabName;
        this.englishName = englishName;
        this.typeName = typeName;
        this.icon = icon;
    }
    protected void Initialize(string prefabName, string englishName, string typeName, Sprite icon)
    {
        this.prefabName = prefabName;
        this.englishName = englishName;
        this.typeName = typeName;
        this.icon = icon;
    }
    protected static Prototype CreatePrototype(string prefabName, string englishName, string typeName, Sprite icon)
    {
        Prototype prototype = ScriptableObject.CreateInstance<Prototype>();
        prototype.Initialize(prefabName, englishName, typeName, icon);
        return prototype;
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
    [SerializeField] protected string prefabName;
    [SerializeField] protected string englishName;
    [SerializeField] protected string typeName;
    [SerializeField] protected Sprite icon;
}
