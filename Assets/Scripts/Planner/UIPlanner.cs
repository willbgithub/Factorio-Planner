using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlanner : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("UIPlanner: Start");
        Recipe recipe = GetRecipe("automation-science-pack");
        RecipeBlock.CreateRecipeBlock(this, transform, recipe, 1);
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
    static Recipe GetRecipe(string prefabName)
    {
        Recipe recipe = AssetDatabase.LoadAssetAtPath<Recipe>(UNITY_RECIPE_PATH + prefabName + ".asset");
        if (recipe.IsUnityNull())
        {
            Debug.LogError("ERROR: Could not find item \"" + prefabName + "\"");
        }
        return item;
    }
}
