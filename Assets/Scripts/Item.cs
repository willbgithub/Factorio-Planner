// Item.cs
// A Factorio item.
// 1 September 2026
// will b. gaming

using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
[Serializable]
public class Item : Prototype
{
    // Constructor
    protected Item(string prefabName, string englishName, string typeName, Sprite icon, List<Recipe> craftedIn=null, Recipe bestRecipe=null) : base(prefabName, englishName, typeName, icon)
    {
        this.craftedIn = new List<Recipe>();
        if (!craftedIn.IsUnityNull())
        {
            for (int i = 0; i < craftedIn.Count; i++)
            {
                this.craftedIn.Add(craftedIn[i]);
            }
        }
        this.bestRecipe = bestRecipe;
    }
    protected void Initialize(string prefabName, string englishName, string typeName, Sprite icon, List<Recipe> craftedIn=null, Recipe bestRecipe=null)
    {
        this.prefabName = prefabName;
        this.englishName = englishName;
        this.typeName = typeName;
        this.icon = icon;
        this.craftedIn = new List<Recipe>();
        if (!craftedIn.IsUnityNull())
        {
            for (int i = 0; i < craftedIn.Count; i++)
            {
                this.craftedIn.Add(craftedIn[i]);
            }
        }
        this.bestRecipe = bestRecipe;
    }
    public static Item CreateItem(string prefabName, string englishName, string typeName, Sprite icon, List<Recipe> craftedIn = null, Recipe bestRecipe = null)
    {
        Item item = CreateInstance<Item>();
        item.Initialize(prefabName, englishName, typeName, icon, craftedIn, bestRecipe);
        return item;
    }
    // Mutators
    public void SetCraftedIn(List<Recipe> craftedIn)
    {
        this.craftedIn = new List<Recipe>();
        for (int i = 0; i < craftedIn.Count; i++)
        {
            this.craftedIn.Add(craftedIn[i]);
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
            returnList.Add(craftedIn[i]);
        }
        return returnList;
    }
    public Recipe GetBestRecipe()
    {
        return bestRecipe;
    }
    // Member data
    [SerializeField] List<Recipe> craftedIn;
    [SerializeField] Recipe bestRecipe;
}
