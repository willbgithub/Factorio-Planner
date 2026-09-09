// ResourceGenerator.cs
// Looks through Factorio game files to create appropriate objects and save them as Unity asset files.
// 31 August 2026
// will b. gaming

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.IO;
class ResourceGenerator
{
    const string FACTORIO_PATH = @"C:/Program Files (x86)/Steam/steamapps/common/Factorio/";
    const string ITEM_PATH = FACTORIO_PATH + @"data/base/prototypes/item.lua";
    const string FLUID_PATH = FACTORIO_PATH + @"data/base/prototypes/fluid.lua";
    const string RECIPE_PATH = FACTORIO_PATH + @"data/base/prototypes/recipe.lua";

    const string UNITY_ITEM_PATH = @"Assets/Prototypes/Items/";
    const string UNITY_RECIPES_PATH = @"Assets/Prototypes/Recipes/";        

    [MenuItem("Factorio/Debug")]
    static void DebugFunc()
    {
        //string iconPath = "graphics/icons/stone-brick";
        //Sprite icon = Resources.Load<Sprite>(iconPath);
        //Debug.Log("iconPath: \"" + iconPath + "\"");
        //Debug.Log("icon: " + icon);
        //Debug.Log("icon.IsUnityNull(): " + icon.IsUnityNull());
        //Item testItem = Item.CreateItem("test-brick", "Test brick", "item", icon);
        //AssetDatabase.CreateAsset(testItem, UNITY_ITEM_PATH + testItem.GetPrefabName() + ".asset");
        //Item testItem2 = AssetDatabase.LoadAssetAtPath<Item>(UNITY_ITEM_PATH + testItem.GetPrefabName() + ".asset");
        //Debug.Log("testItem2: " + testItem2.GetPrefabName());
        Debug.Log(GetItem("stone-brick").GetEnglishName());
    }
    static void CreateFiles(List<string> prototypes)
    {
        for (int i = 0; i < prototypes.Count; i++)
        {
            string prototype = prototypes[i];
            string prefabName = GetStringProperty(prototype, "name");

        }
    }
    static string GetEnglishName(string prefabName)
    {

    }
    static Sprite GetIconProperty(string prototype)
    {
        string iconPath = GetStringProperty(prototype, "icon");
        if (iconPath.Length == 0)
        {
            Item item = GetItem(GetStringProperty(prototype, "name"));
            return item.GetIcon();
        }
        iconPath = Regex.Match(iconPath, "/(.+)").Groups[1].Value;
        return Resources.Load<Sprite>(iconPath);
        
    }
    static Contribution GetRecipeContribution(string prototype)
    {
        Contribution contribution = new Contribution();
        string ingredientsText = Regex.Match(prototype, "ingredients ?=(.+)", RegexOptions.Singleline).Groups[1].Value;
        List<string> ingredients = GetBlocks(ingredientsText, 2, true);
        for (int i = 0; i < ingredients.Count; i++)
        {
            string ingredient = ingredients[i];
            string itemName = GetStringProperty(ingredient, "name");
            Item item = GetItem(itemName);
            Fraction rate = GetFractionProperty(ingredient, "amount");
            ItemRate itemRate = new ItemRate(item, rate, 0);
            contribution.Add(itemRate);
        }
        string productsText = Regex.Match(prototype, "results ?=(.+)", RegexOptions.Singleline).Groups[1].Value;
        List<string> products = GetBlocks(productsText, 2, true);
        for (int i = 0; i < products.Count; i++)
        {
            string product = products[i];
            string itemName = GetStringProperty(product, "name");
            Item item = GetItem(itemName);
            Fraction rate = GetFractionProperty(product, "amount");
            ItemRate itemRate = new ItemRate(item, 0, rate);
            contribution.Add(itemRate);
        }
        return contribution;
    }
    //static ItemValue GetItemValueProperty(string prototype, string propertyName)
    //{

    //}
    static Item GetItem(string prefabName)
    {
        Item item = AssetDatabase.LoadAssetAtPath<Item>(UNITY_ITEM_PATH + prefabName + ".asset");
        if (item.IsUnityNull())
        {
            Debug.LogError("ERROR: Could not find item \"" + prefabName + "\"");
        }
        return item;
    }
    static Fraction GetFractionProperty(string prototype, string propertyName)
    {
        Fraction fraction = new Fraction(Regex.Match(prototype, "\\W" + propertyName + "\\W*?=[^\"]*?\"([^\"]*?)\"").Groups[1].Value);
        if (fraction.IsUnityNull())
        {
            Debug.LogError("ERROR: Could not extract \"" + propertyName + "\" from \"" + prototype + "\"!");
        }
        return fraction;
    }
    static string GetStringProperty(string prototype, string propertyName)
    {
        string str = Regex.Match(prototype, "\\W" + propertyName + "\\W*?=[^\"]*?\"([^\"]*?)\"").Groups[1].Value;
        if (str.Length == 0)
        {
            Debug.LogError("ERROR: Could not extract \"" + propertyName + "\" from \"" + prototype + "\"!");
        }
        return str;
    }
    static bool IsBlacklisted(Prototype prototype)
    {
        return false;
    }
    static List<string> GetBlocks(string inputText, int targetDepth, bool consecutive)
    {
        int depth = 0;
        int index = 0;
        List<string> blocks = new List<string>();
        string block = "";
        bool consecutiveCheck = true;
        while (index < inputText.Length && consecutiveCheck)
        {
            char c = inputText[index];
            if (c == '{')
            {
                depth++;
            }
            if (depth >= targetDepth)
            {
                block += c;
            }
            if (c == '}')
            {
                depth--;
                if (depth < targetDepth)
                {
                    if (block.Length > 0)
                    {
                        blocks.Add(block);
                        block = "";
                    }
                    if (consecutive && depth < targetDepth-1)
                    {
                        consecutiveCheck = false;
                    }
                }
            }
            index++;
        }
        if (block.Length > 0)
        {
            blocks.Add(block);
        }
        return blocks;
    }
}
