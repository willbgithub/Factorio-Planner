// Planner.cs
// Given a list of items and rates to produce, generates a list of inputs, intermediates, products, and byproducts.
// 17 September 2026
// will b. gaming
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Planner
{
    const string UNITY_ITEM_PATH = @"Assets/Prototypes/Items/";
    const string UNITY_RECIPE_PATH = @"Assets/Prototypes/Recipes/";

    static int savior = 0;

    [MenuItem("Factorio/Planner Debug")]
    public static void DebugFunc()
    {
        Item item = GetItem("automation-science-pack");
        ItemValue input = new ItemValue(item, 5);
        Debug.Log(ResolveInput(input));
    }
    //static Contribution Plan(Contribution userRequest)
    //{

    //}
    //static Contribution ResolveInputs(Contribution contribution)
    //{
    //    List<ItemValue> inputs = contribution.GetInputs();
    //    for (int i = 0; i < inputs.Count; i++)
    //    {
    //        contribution.Add(ResolveInput(inputs[i]));
    //    }
    //}
    static Contribution ResolveInput(ItemValue input)
    {
        savior++;
        if (savior > 30)
            return null;
        Recipe recipe = input.GetItem().GetBestRecipe();
        if (recipe.IsUnityNull())
        {
            Debug.LogError("ERROR: Could not find best recipe for \"" + input.GetItem() + "\"!");
            return new Contribution();
        }
        Contribution resolvedContribution = new Contribution(recipe, input.GetValue());
        Contribution duplicateContribution = new Contribution(resolvedContribution);
        for (int i = 0; i < resolvedContribution.Count; i++)
        {
            ItemValue subInput = resolvedContribution.Index(i).GetInput();
            if (subInput.GetValue() == 0 || IsRaw(subInput.GetItem()))
            {
                continue;
            }
            duplicateContribution.Add(ResolveInput(subInput));
        }
        return duplicateContribution;
    }
    static bool IsRaw(Item item)
    {
        Debug.Log("IsRaw(" + item + ") -> " + item.GetBestRecipe().IsUnityNull());
        return (item.GetBestRecipe().IsUnityNull());
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
