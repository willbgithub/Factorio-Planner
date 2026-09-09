// Demand.cs
// A recipe with a specific item demand.
// 2 September 2026
// will b. gaming
using System;
using UnityEngine;
[Serializable]
public class Demand
{
    // Constructor
    public Demand(ItemValue demandValue, Recipe recipe)
    {
        this.demandValue = demandValue;
        this.recipe = recipe;
    }
    public Demand(Demand demand)
    {
        demandValue = new ItemValue(demand.demandValue);
        recipe = Recipe.CreateRecipe(demand.recipe);
    }
    // Mutators
    public void SetDemandValue(ItemValue demandValue)
    {
        this.demandValue = new ItemValue(demandValue);
    }
    public void SetRecipe(Recipe recipe)
    {
        this.recipe = Recipe.CreateRecipe(recipe);
    }
    // Accessors
    public ItemValue GetDemandValue()
    {
        return new ItemValue(demandValue);
    }
    public Recipe GetRecipe()
    {
        return Recipe.CreateRecipe(recipe);
    }
    // Member data
    ItemValue demandValue;
    Recipe recipe;
}
