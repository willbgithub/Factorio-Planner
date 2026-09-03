// Item.cs
// A Factorio item.
// 1 September 2026
// will b. gaming

using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class Item : Prototype
{
    // Constructor
    public Item(string prefabName, string englishName, string typeName, Sprite icon, List<Recipe> craftedIn=null, Recipe bestRecipe=null) : base(prefabName, englishName, typeName, icon)
    {
        this.craftedIn = new List<Recipe>();
        if (!craftedIn.IsUnityNull())
        {
            for (int i = 0; i < craftedIn.Count; i++)
            {
                this.craftedIn[i] = craftedIn[i];
            }
        }
        this.bestRecipe = bestRecipe;
    }
    public Item(Item item) : base(item.prefabName, item.englishName, item.typeName, item.icon)
    {
        craftedIn = new List<Recipe>();
        if (!item.craftedIn.IsUnityNull())
        {
            for (int i = 0; i < item.craftedIn.Count; i++)
            {
                craftedIn[i] = item.craftedIn[i];
            }
        }
        bestRecipe = item.bestRecipe;
    }
    // Mutators
    public void SetCraftedIn(List<Recipe> craftedIn)
    {
        this.craftedIn = new List<Recipe>();
        for (int i = 0; i < craftedIn.Count; i++)
        {
            this.craftedIn[i] = craftedIn[i];
        }
    }
    public void SetBestRecipe(Recipe bestRecipe)
    {
        this.bestRecipe = bestRecipe;
    }
    // Accessors
    public List<Recipe> GetCraftedIn()
    {
        List<Recipe> returnList = new List<Recipe>();
        for (int i = 0; i < craftedIn.Count; i++)
        {
            returnList[i] = craftedIn[i];
        }
        return returnList;
    }
    public Recipe GetBestRecipe()
    {
        return bestRecipe;
    }
    // Member data
    List<Recipe> craftedIn;
    Recipe bestRecipe;
}
