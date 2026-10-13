using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlanner : MonoBehaviour
{
    // Test values
    [SerializeField] Recipe testRecipe;
    [SerializeField] Fraction testFactor;

    [SerializeField] GameObject canvas;
    [SerializeField] RecipeBlock recipeBlockPrefab;
    [SerializeField] GameObject recipeBlocks;

    const string UNITY_RECIPE_PATH = @"Assets/Prototypes/Recipes/";
    public void Start()
    {
        Debug.Log("UIPlanner: Start");
        CreateRecipeBlock(testRecipe, testFactor);
    }
    public float GetScale()
    {
        return canvas.GetComponent<RectTransform>().localScale.x;
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
    public RecipeBlock CreateRecipeBlock(Recipe recipe, Fraction factor)
    {
        Debug.Log("RecipeBlock: CreateRecipeBlock");
        RecipeBlock recipeBlock = Instantiate(recipeBlockPrefab, recipeBlocks.transform);
        recipeBlock.SetParent(this);
        recipeBlock.SetRecipe(recipe);
        recipeBlock.SetFactor(factor);
        return recipeBlock;
    }
}
