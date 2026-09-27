// Planner.cs
// Given a list of items and rates to produce, generates a list of inputs, intermediates, products, and byproducts.
// 17 September 2026
// will b. gaming
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Planner
{
    const string UNITY_ITEM_PATH = @"Assets/Prototypes/Items/";
    const string UNITY_RECIPE_PATH = @"Assets/Prototypes/Recipes/";
    static List<string> RAW_ITEMS = new List<string>()
    {
        "iron-ore", "copper-ore", "stone", "coal", "crude-oil", "uranium-ore", "water", "wood", 
    };

    static List<Demand> demands = new List<Demand>();

    [MenuItem("Factorio/Utility Debug")]
    public static void Debug2()
    {
        DirectoryInfo itemDirectory = Directory.CreateDirectory(UNITY_ITEM_PATH);
        FileInfo[] itemFiles = itemDirectory.GetFiles();

        for (int i = 0; i < itemFiles.Length; i++)
        {
            if (itemFiles[i].FullName.EndsWith(".asset.meta"))
            {
                continue;
            }
            string itemPath = Regex.Replace(itemFiles[i].FullName, @"\\", "/");
            itemPath = Regex.Match(itemPath, "Factorio-Planner/(.+)").Groups[1].Value;
            Item item = AssetDatabase.LoadAssetAtPath<Item>(itemPath);
            if (item.GetCraftedIn().Count > 1)
            {
                Debug.Log(item);
            }
        }
    }
    [MenuItem("Factorio/Planner Debug")]
    public static void DebugFunc()
    {
        
        Item item = GetItem("chemical-science-pack");
        Debug.Log("Item: " + item);
        Fraction demandRate = 1;
        Debug.Log("Demand rate: " + demandRate);

        ItemValue itemValue = new ItemValue(item, demandRate);
        Debug.Log("itemValue: " + itemValue);
        Recipe recipe = item.GetBestRecipe();
        Debug.Log("recipe: " + recipe);
        Demand demand = new Demand(itemValue, recipe);
        Debug.Log("demand: " + demand);
        demands.Add(demand);
        Contribution totalContributions = new Contribution(demand);
        Debug.Log("initial contribution from demand: " + totalContributions);
        ResolveInputs(totalContributions);
        ReportLog(totalContributions);
    }
    static void ReportLog(Contribution contribution)
    {
        List<ItemValue> products = contribution.GetProducts();
        List<ItemValue> intermediates = contribution.GetIntermediates();
        List<ItemValue> inputs = contribution.GetInputs();
        //List<ItemValue> byproducts = new List<ItemValue>();

        Debug.Log("Products: " + ListToString(products, "+"));
        Debug.Log("Intermediates: " + ListToString(intermediates, "±"));
        Debug.Log("Inputs: " + ListToString(inputs, "-"));
    }
    static string ListToString(List<ItemValue> list, string prefix = "")
    {
        if (list.Count == 0)
        {
            return "{}";
        }
        string msg = "{";
        for (int i = 0; i < list.Count - 1; i++)
        {
            msg += list[i].GetItem() + ": " + prefix + list[i].GetValue() + ", ";
        }
        msg += list[list.Count-1].GetItem() + ": " + prefix + list[list.Count-1].GetValue() + "}";
        return msg;
    }
    static void ResolveInputs(Contribution contribution)
    {
        Debug.Log("ResolveInputs called on " + contribution);
        List<ItemValue> unresolvedInputs = GetUnresolvedInputs(contribution);
        while (unresolvedInputs.Count > 0)
        {
            for (int i = 0; i < unresolvedInputs.Count; i++)
            {
                contribution.Add(ResolveInput(unresolvedInputs[i]));
            }
            unresolvedInputs = GetUnresolvedInputs(contribution);
        }
    }
    static Contribution ResolveInput(ItemValue input)
    {
        Recipe recipe = input.GetItem().GetBestRecipe();
        return new Contribution(recipe, input.GetValue()/recipe.GetRate(input.GetItem()));
    }
    static List<ItemValue> GetUnresolvedInputs(Contribution contribution)
    {
        List<ItemValue> inputs = contribution.GetInputs();
        List<ItemValue> unresolvedInputs = new List<ItemValue>();
        for (int i = 0; i < inputs.Count; i++)
        {
            ItemValue input = inputs[i];
            if (IsRaw(input.GetItem()) || input.GetValue() == 0)
            {
                continue;
            }
            unresolvedInputs.Add(input);
        }
        Debug.Log("unresolved inputs of " + contribution + " : " + ListToString(unresolvedInputs));
        return unresolvedInputs;
    }
    static bool IsRaw(Item item)
    {
        string prefabName = item.GetPrefabName();
        //Debug.Log("IsRaw(" + item + ") -> " + RAW_ITEMS.Contains(prefabName));
        return RAW_ITEMS.Contains(prefabName);
    }
    static Item GetItem(string prefabName)
    {
        Item item = AssetDatabase.LoadAssetAtPath<Item>(UNITY_ITEM_PATH + prefabName + ".asset");
        if (item.IsUnityNull())
        {
            Debug.LogError("ERROR: Could not find item \"" + prefabName + "\"");
        }
        return item;
    }
}
