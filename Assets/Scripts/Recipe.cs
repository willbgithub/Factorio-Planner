// Recipe.cs
// A Factorio Recipe with products and ingredients.
// 2 September 2026
// will b. gaming
using System;
using UnityEngine;
using static UnityEditor.Progress;
[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
[Serializable]
public class Recipe : Prototype
{
    // Constructor
    protected Recipe(string prefabName, string englishName, string typeName, Sprite icon, Contribution contribution) : base(prefabName, englishName, typeName, icon)
    {
        this.contribution = contribution;
    }
    protected void Initialize(string prefabName, string englishName, string typeName, Sprite icon, Contribution contribution)
    {
        this.prefabName = prefabName;
        this.englishName = englishName;
        this.typeName = typeName;
        this.icon = icon;
        this.contribution = new Contribution(contribution);
    }
    public static Recipe CreateRecipe(string prefabName, string englishName, string typeName, Sprite icon, Contribution contribution)
    {
        Recipe recipe = CreateInstance<Recipe>();
        recipe.Initialize(prefabName, englishName, typeName, icon, contribution);
        return recipe;
    }
    // Accessors
    public Contribution GetContribution()
    {
        return contribution;
    }
    public Fraction GetRate(Item item)
    {
        return contribution.GetRate(item);
    }
    public Fraction GetRate(string prefabName)
    {
        return contribution.GetRate(prefabName);
    }
    // Member Data
    [SerializeField] Contribution contribution;
}
