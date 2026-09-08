// ResourceGenerator.cs
// Looks through Factorio game files to create appropriate objects and save them as Unity asset files.
// 31 August 2026
// will b. gaming

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
class ResourceGenerator
{
    const string FACTORIO_PATH = @"C:/Program Files (x86)/Steam/steamapps/common/Factorio/";
    const string ITEM_PATH = FACTORIO_PATH + @"data/base/prototypes/item.lua";
    const string FLUID_PATH = FACTORIO_PATH + @"data/base/prototypes/fluid.lua";
    const string RECIPE_PATH = FACTORIO_PATH + @"data/base/prototypes/recipe.lua";

    const string ICON_PATH = @"Assets/Icons/";
    const string UNITY_ITEM_PATH = @"Assets/Prototypes/Items/";
    const string UNITY_RECIPES_PATH = @"Assets/Prototypes/Recipes/";        

    [MenuItem("Factorio/Debug")]
    static void DebugFunc()
    {
        string iconPath = ICON_PATH + "graphics/icons/stone-brick.png";
        Sprite icon = Resources.Load<Sprite>(iconPath);
        Item testItem = ScriptableObject.CreateInstance<Item>();
        testItem.SetPrefabName("test-brick");
        AssetDatabase.CreateAsset(testItem, UNITY_ITEM_PATH + testItem.GetPrefabName() + ".asset");
    }
    static Item GetItem(string prefabName)
    {
        return null;
    }
    static Fraction GetFractionProperty(string prototype, string propertyName)
    {
        return new Fraction(Regex.Match(prototype, "\\W" + propertyName + "\\W*?=[^\"]*?\"([^\"]*?)\"").Groups[1].Value);
    }
    static string GetStringProperty(string prototype, string propertyName)
    {
        return Regex.Match(prototype, "\\W" + propertyName + "\\W*?=[^\"]*?\"([^\"]*?)\"").Groups[1].Value;
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
