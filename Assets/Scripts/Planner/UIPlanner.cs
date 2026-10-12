using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlanner : MonoBehaviour
{
    
    [SerializeField] RecipeBlock recipeBlockPrefab;
    [SerializeField] GameObject recipeBlocks;

    const string UNITY_RECIPE_PATH = @"Assets/Prototypes/Recipes/";
    public void Start()
    {
        Debug.Log("UIPlanner: Start");
        Recipe recipe = GetRecipe("automation-science-pack");
        CreateRecipeBlock(recipeBlocks.transform, recipe, 1);
    }
    static Recipe GetRecipe(string prefabName)
    {
        Recipe recipe = AssetDatabase.LoadAssetAtPath<Recipe>(UNITY_RECIPE_PATH + prefabName + ".asset");
        if (recipe.IsUnityNull())
        {
            Debug.LogError("ERROR: Could not find recipe \"" + prefabName + "\"");
        }
        return recipe;
    }
    public RecipeBlock CreateRecipeBlock(Transform parent, Recipe recipe, Fraction factor)
    {
        Debug.Log("RecipeBlock: CreateRecipeBlock");
        RecipeBlock recipeBlock = Instantiate(recipeBlockPrefab, parent);
        recipeBlock.SetParent(this);
        recipeBlock.SetRecipe(recipe);
        Debug.Log("factor: " + factor);
        recipeBlock.SetFactor(factor);
        return recipeBlock;
    }
}
