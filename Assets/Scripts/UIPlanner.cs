using UnityEngine;

public class UIPlanner : MonoBehaviour
{
    public GameObject canvas;
    public RecipeBlock recipeBlockPrefab;
    public Recipe recipe;
    void Start()
    {
        RecipeBlock block = Instantiate(recipeBlockPrefab, canvas.transform);
        Debug.Log("recipe is " + recipe + " and icon is " + recipe.GetIcon());
        block.Initialize(this, recipe, 1);
    }
}
