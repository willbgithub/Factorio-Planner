using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlanner : MonoBehaviour
{
    public GameObject canvas;
    public RecipeBlock recipeBlockPrefab;
    public Recipe recipe;

    bool dragging = false;
    RecipeBlock dragged = null;
    void Start()
    {
        RecipeBlock block = Instantiate(recipeBlockPrefab, canvas.transform);
        block.Initialize(this, recipe, 1);
    }
    public void OnRecipeBlockPointerDown(RecipeBlock block, BaseEventData data)
    {

    }
    public void OnRecipeBlockPointerUp(RecipeBlock block, BaseEventData data)
    {

    }
}
