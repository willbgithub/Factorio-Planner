// Item.cs
// A Factorio item.
// 1 September 2026
// will b. gaming

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
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
    public void SetUsedBy(List<Recipe> usedBy)
    {
        this.usedBy = new List<Recipe>();
        for (int i = 0; i < usedBy.Count; i++)
        {
            this.usedBy.Add(usedBy[i]);
        }
    }
    // Accessors
    public List<Recipe> GetCraftedIn()
    {
        if (craftedIn.Count == 0)
        {
            UpdateRecipeLists();
        }
        List<Recipe> returnList = new List<Recipe>();
        for (int i = 0; i < craftedIn.Count; i++)
        {
            returnList.Add(craftedIn[i]);
        }
        return returnList;
    }
    public Recipe GetBestRecipe()
    {
        if (bestRecipe.IsUnityNull())
        {
            UpdateRecipeLists();

        }
        return bestRecipe;
    }
    public List<Recipe> GetUsedBy()
    {
        if (usedBy.Count == 0)
        {
            UpdateRecipeLists();
        }
        List<Recipe> returnList = new List<Recipe>();
        for (int i = 0; i < usedBy.Count; i++)
        {
            returnList.Add(usedBy[i]);
        }
        return returnList;
    }
    public void UpdateRecipeLists()
    {
        // if there are no recipe files, ignore
        DirectoryInfo recipeDirectory = Directory.CreateDirectory(ResourceGenerator.UNITY_RECIPE_PATH);
        FileInfo[] recipeFiles = recipeDirectory.GetFiles();
        if (recipeFiles.Length == 0)
        {
            Debug.LogError("ERROR: There are no recipe files!");
            return;
        }
        craftedIn = new List<Recipe>();
        for (int j = 0; j < recipeFiles.Length; j++)
        {
            if (recipeFiles[j].FullName.EndsWith(".asset.meta"))
            {
                continue;
            }
            string recipePath = Regex.Replace(recipeFiles[j].FullName, @"\\", "/");
            recipePath = Regex.Match(recipePath, "Factorio-Planner/(.+)").Groups[1].Value;
            Recipe recipe = AssetDatabase.LoadAssetAtPath<Recipe>(recipePath);
            if (recipe.GetRate(prefabName) > 0)
            {
                craftedIn.Add(recipe);

            }
            else if (recipe.GetRate(prefabName) < 0)
            {
                usedBy.Add(recipe);
            }
            if (craftedIn.Count > 0)
            {
                bestRecipe = craftedIn[0];
            }
        }
        if (craftedIn.Count == 0)
        {
            Debug.LogError("ERROR: No recipe exists for \"" + prefabName + "\"!");
        }
        }
    // Member data
    [SerializeField] List<Recipe> craftedIn;
    [SerializeField] List<Recipe> usedBy;
    [SerializeField] Recipe bestRecipe;
}
