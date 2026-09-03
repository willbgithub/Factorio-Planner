// ResourceGenerator.cs
// Looks through Factorio game files to create appropriate objects and save them as Unity asset files.
// 31 August 2026
// will b. gaming

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

class ResourceGenerator
{
    const string FACTORIO_PATH = @"C:/Program Files (x86)/Steam/steamapps/common/Factorio/";
    const string ITEM_PATH = @"data/base/prototypes/item.lua";
    const string FLUID_PATH = @"data/base/prototypes/fluid.lua";
    const string RECIPE_PATH = @"data/base/prototypes/recipe.lua";

    [MenuItem("Factorio/Debug")]
    static void DebugFunc()
    {

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
