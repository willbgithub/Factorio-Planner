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
        this.contribution = new Contribution(contribution);
    }
    protected Recipe(Recipe recipe) : base(recipe.prefabName, recipe.englishName, recipe.typeName, recipe.icon)
    {
        contribution = new Contribution(recipe.contribution);
    }
    protected void Initialize(string prefabName, string englishName, string typeName, Sprite icon, Contribution contribution)
    {
        this.prefabName = prefabName;
        this.englishName = englishName;
        this.typeName = typeName;
        this.icon = icon;
        this.contribution = new Contribution(contribution);
    }
    protected void Initialize(Recipe recipe)
    {
        prefabName = recipe.prefabName;
        englishName = recipe.englishName;
        typeName = recipe.typeName;
        icon = recipe.icon;
        contribution = new Contribution(recipe.contribution);
    }
    public static Recipe CreateRecipe(string prefabName, string englishName, string typeName, Sprite icon, Contribution contribution)
    {
        Recipe recipe = ScriptableObject.CreateInstance<Recipe>();
        recipe.Initialize(prefabName, englishName, typeName, icon, contribution);
        return recipe;
    }
    public static Recipe CreateRecipe(Recipe recipe)
    {
        Recipe returnRecipe = ScriptableObject.CreateInstance<Recipe>();
        returnRecipe.Initialize(recipe);
        return returnRecipe;
    }
    // Accessors
    public Contribution GetContribution()
    {
        return new Contribution(contribution);
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
    Contribution contribution;
}
