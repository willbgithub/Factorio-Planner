// ResourceGenerator.cs
// Looks through Factorio game files to create appropriate objects and save them as Unity asset files.
// 31 August 2026
// will b. gaming

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class ResourceGenerator
{
    const string FACTORIO_PATH = @"C:/Program Files (x86)/Steam/steamapps/common/Factorio/";
    const string ITEM_PATH = @"data/base/prototypes/item.lua";
    const string FLUID_PATH = @"data/base/prototypes/fluid.lua";
    const string RECIPE_PATH = @"data/base/prototypes/recipe.lua";

    [MenuItem("Factorio/Debug")]
    static void DebugFunc()
    {
        string prototype = "{\r\n    type = \"item\",\r\n    name = \"stone-brick\",\r\n    icon = \"__base__/graphics/icons/stone-brick.png\",\r\n    subgroup = \"terrain\",\r\n    order = \"a[stone-brick]\",\r\n    inventory_move_sound = item_sounds.brick_inventory_move,\r\n    pick_sound = item_sounds.brick_inventory_pickup,\r\n    drop_sound = item_sounds.brick_inventory_move,\r\n    stack_size = 100,\r\n    place_as_tile =\r\n    {\r\n      result = \"stone-path\",\r\n      condition_size = 1,\r\n      condition = {layers={water_tile=true}}\r\n    }\r\n  }";
        Debug.Log(GetStringProperty(prototype, "type"));
    }
    static Item GetItem(string prefabName)
    {

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
