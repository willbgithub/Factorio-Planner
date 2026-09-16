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
    // Mutators
    public void SetDemandValue(ItemValue demandValue)
    {
        this.demandValue = demandValue;
    }
    public void SetRecipe(Recipe recipe)
    {
        this.recipe = recipe;
    }
    // Accessors
    public ItemValue GetDemandValue()
    {
        return demandValue;
    }
    public Recipe GetRecipe()
    {
        return recipe;
    }
    // Member data
    ItemValue demandValue;
    Recipe recipe;
}
