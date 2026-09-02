// Recipe.cs
// A Factorio Recipe with products and ingredients.
// 2 September 2026
// will b. gaming
using UnityEngine;

public class Recipe : Prototype
{
    // Constructor
    public Recipe(string prefabName, string englishName, string typeName, Sprite icon, Contribution contribution) : base(prefabName, englishName, typeName, icon)
    {
        this.contribution = new Contribution(contribution);
    }
    public Recipe(Recipe recipe) : base(recipe.prefabName, recipe.englishName, recipe.typeName, recipe.icon)
    {
        contribution = new Contribution(recipe.contribution);
    }

    // Accessors
    public Contribution GetContribution()
    {
        return new Contribution(contribution);
    }
    // Member Data
    Contribution contribution;
}
