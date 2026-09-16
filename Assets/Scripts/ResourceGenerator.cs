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

    }
    [MenuItem("Factorio/Create Items")]
    static void CreateItems()
    {
        CreateFiles(ITEM_PATH);
        CreateFiles(FLUID_PATH);
    }
    [MenuItem("Factorio/Create Recipes")]
    static void CreateRecipes()
    {
        CreateFiles(RECIPE_PATH);
    }
    static void CreateFiles(string file_path)
    {
        // Get prototype strings
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
        bool itemsReady = true;
        DirectoryInfo itemDirectory = Directory.CreateDirectory(UNITY_ITEM_PATH);
        FileInfo[] itemFiles = itemDirectory.GetFiles();
        if (itemFiles.Length == 0)
        {
            itemsReady = false;
        }
        // Create objects for all prototypes
        for (int i = 0; i < 1; i++)
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
                Item item = Item.CreateItem(prefabName, englishName, typeName, icon, craftedIn, bestRecipe);
                AssetDatabase.CreateAsset(item, UNITY_ITEM_PATH + item.GetPrefabName() + ".asset");
            }
            // Recipe
            else if (itemsReady)
            {
                Contribution contribution = GetRecipeContribution(prototype);
                //Debug.Log("prefabName: " + prefabName);
                //Debug.Log("englishName: " + englishName);
                //Debug.Log("typeName: " + typeName);
                //Debug.Log("icon: " + icon);
                //Debug.Log("contribution: " + contribution);
                Debug.Log("type of item: " + contribution.GetItemRates()[0].GetItem().GetType());
                Recipe recipe = Recipe.CreateRecipe(prefabName, englishName, typeName, icon, contribution);
                Debug.Log("type of recipe: " + recipe.GetType());
                Debug.Log("type of item: " + recipe.GetContribution().GetItemRates()[0].GetItem().GetType());
                AssetDatabase.CreateAsset(recipe, UNITY_RECIPE_PATH + recipe.GetPrefabName() + ".asset");
                Debug.Log("Saved recipe at path. Contribution: " + recipe.GetContribution());
            }
        }
    }
    static string GetEnglishName(string prefabName)
    {
        string cfg = File.ReadAllText(CFG_PATH);
        MatchCollection matches = Regex.Matches(cfg, prefabName + "=(.+)");
        if (matches.Count == 0)
        {
            Debug.LogError("ERROR: No english name found for \"" + prefabName + "\"!");
            return "NULL ITEM!!!";
        }
        string bestMatch = matches[0].Groups[1].Value;
        for (int i = 0; i < matches.Count; i++)
        {
            string match = matches[i].Groups[1].Value;
            if (match.Length < bestMatch.Length)
            {
                bestMatch = match;
            }
        }
        return bestMatch;
    }
    static Sprite GetIconProperty(string prototype)
    {
        string iconPath = GetStringProperty(prototype, "icon");
        if (iconPath.Length == 0)
        {
            Item item = GetItem(GetStringProperty(prototype, "name"));
            return item.GetIcon();
        }
        iconPath = Regex.Match(iconPath, "/(.+)\\.png").Groups[1].Value;
        return Resources.Load<Sprite>(iconPath);
        
    }
    static Contribution GetRecipeContribution(string prototype)
    {
        //Debug.Log("GetRecipeContribution called on \"" + prototype + "\"");
        Contribution contribution = new Contribution();
        string ingredientsText = Regex.Match(prototype, "ingredients ?=(.+)", RegexOptions.Singleline).Groups[1].Value;
        //Debug.Log("ingredientsText: \"" + ingredientsText + "\"");
        List<string> ingredients = GetBlocks(ingredientsText, 2, true);
        for (int i = 0; i < ingredients.Count; i++)
        {
            string ingredient = ingredients[i];
            //Debug.Log("ingredient: \"" + ingredient + "\"");
            string itemName = GetStringProperty(ingredient, "name");
            //Debug.Log("itemName: \"" + itemName + "\"");
            Item item = GetItem(itemName);
            //Debug.Log("item: \"" + item + "\"");
            Fraction rate = GetFractionProperty(ingredient, "amount");
            //Debug.Log("rate: " + rate);
            ItemRate itemRate = new ItemRate(item, rate, 0);
            //Debug.Log("itemRate: " + itemRate);
            contribution.Add(itemRate);
        }
        string productsText = Regex.Match(prototype, "results ?=(.+)", RegexOptions.Singleline).Groups[1].Value;
        //Debug.Log("productsText: \"" + productsText + "\"");
        List<string> products = GetBlocks(productsText, 2, true);
        for (int i = 0; i < products.Count; i++)
        {
            string product = products[i];
            //Debug.Log("product: \"" + product + "\"");
            string itemName = GetStringProperty(product, "name");
            //Debug.Log("itemName: \"" + itemName + "\"");
            Item item = GetItem(itemName);
            //Debug.Log("item: \"" + item + "\"");
            Fraction rate = GetFractionProperty(product, "amount");
            //Debug.Log("rate: " + rate);
            ItemRate itemRate = new ItemRate(item, 0, rate);
            //Debug.Log("itemRate: " + itemRate);
            contribution.Add(itemRate);
        }
        //Debug.Log("Returning contribution:\n" + contribution);
        return contribution;
    }
    static Item GetItem(string prefabName)
    {
        Item item = AssetDatabase.LoadAssetAtPath<Item>(UNITY_ITEM_PATH + prefabName + ".asset");
        if (item.IsUnityNull())
        {
            Debug.LogError("ERROR: Could not find item \"" + prefabName + "\"");
        }
        return Item.CreateItem(item);
    }
    static Fraction GetFractionProperty(string prototype, string propertyName)
    {
        //Debug.Log("GetFractionProperty searching for \"" + propertyName + "\" on prototype \"" + prototype + "\"");
        //Debug.Log("Regex match: " + Regex.Match(prototype, "\\W" + propertyName + "\\W?=\\W?(\\d+)").Groups[1].Value);
        Fraction fraction = new Fraction(Regex.Match(prototype, "\\W"+propertyName+"\\W?=\\W?(\\d+)").Groups[1].Value);
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
            if (propertyName == "icon" && GetStringProperty(prototype, "type") == "recipe")
            {
                return "";
            }
            Debug.LogError("ERROR: Could not extract \"" + propertyName + "\" from \"" + prototype + "\"!");
        }
        return str;
    }
    static bool IsBlacklisted(string prototype)
    {
        string prefabName = GetStringProperty(prototype, "name");
        string typeName = GetStringProperty(prototype, "type");
        return (prefabName == "parameter-") || (prefabName.StartsWith("spidertron-rocket-launcher-"));
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
