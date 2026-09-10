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
    const string CFG_PATH = FACTORIO_PATH + @"data/base/locale/en/base.cfg";

    const string UNITY_ITEM_PATH = @"Assets/Prototypes/Items/";
    const string UNITY_RECIPE_PATH = @"Assets/Prototypes/Recipes/";

    [MenuItem("Factorio/Debug")]
    static void DebugFunc()
    {
        CreateItems();
    }
    static void CreateItems()
    {
        CreateFiles(GetPrototypes(ITEM_PATH));
        CreateFiles(GetPrototypes(FLUID_PATH));
    }
    static void CreateRecipes()
    {
        CreateFiles(GetPrototypes(RECIPE_PATH));
    }
    static List<string> GetPrototypes(string file_path)
    {
        List<string> blocks = GetBlocks(File.ReadAllText(file_path), 2, false);
        List<string> prototypes = new List<string>();
        for (int i = 0; i < blocks.Count; i++)
        {
            string prototype = blocks[i];
            if (IsBlacklisted(prototype))
            {
                continue;
            }
            prototypes.Add(prototype);
        }
        return prototypes;
    }
    static void CreateFiles(List<string> prototypes)
    {
        for (int i = 0; i < prototypes.Count; i++)
        {
            string prototype = prototypes[i];
            string prefabName = GetStringProperty(prototype, "name");
            string englishName = GetEnglishName(prefabName);
            string typeName = GetStringProperty(prototype, "type");
            Sprite icon = GetIconProperty(prototype);
            // Item
            if (typeName != "recipe")
            {
                List<Recipe> craftedIn = new List<Recipe>();
                Recipe bestRecipe = null;
                // if there are no recipe files, ignore
                DirectoryInfo recipeDirectory = Directory.CreateDirectory(UNITY_RECIPE_PATH);
                FileInfo[] recipeFiles = recipeDirectory.GetFiles();
                for (int j = 0; j < recipeFiles.Length; j++)
                {
                    string recipePath = Regex.Match(recipeFiles[j].FullName, "Factorio-Planner/(.+)").Groups[1].Value;
                    Recipe recipe = AssetDatabase.LoadAssetAtPath<Recipe>(recipePath);
                    if (recipe.GetRate(prefabName) > 0)
                    {
                        craftedIn.Add(recipe);
                        bestRecipe = recipe;
                    }
                }
                Item obj = Item.CreateItem(prefabName, englishName, typeName, icon, craftedIn, bestRecipe);
                AssetDatabase.CreateAsset(obj, UNITY_ITEM_PATH + obj.GetPrefabName() + ".asset");
            }
            // Recipe
            else if (typeName == "recipe")
            {
                // if there are no item files, ignore
                DirectoryInfo itemDirectory = Directory.CreateDirectory(UNITY_ITEM_PATH);
                FileInfo[] itemFiles = itemDirectory.GetFiles();
                if (itemFiles.Length == 0)
                {
                    continue;
                }
                Contribution contribution = GetRecipeContribution(prototype);
                Recipe obj = Recipe.CreateRecipe(prefabName, englishName, typeName, icon, contribution);
                AssetDatabase.CreateAsset(obj, UNITY_RECIPE_PATH + obj.GetPrefabName() + ".asset");
            }
        }
    }
    static string GetEnglishName(string prefabName)
    {
        string cfg = File.ReadAllText(CFG_PATH);
        MatchCollection matches = Regex.Matches(cfg, prefabName + "=(.+)");
        string bestMatch = matches[0].Value;
        for (int i = 0; i < matches.Count; i++)
        {
            string match = matches[i].Value;

        }
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
    static bool IsBlacklisted(string prototype)
    {
        string prefabName = GetStringProperty(prototype, "name");
        string typeName = GetStringProperty(prototype, "type");
        return (prefabName == "parameter-");
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
