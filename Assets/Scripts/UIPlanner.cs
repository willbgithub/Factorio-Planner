using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlanner : MonoBehaviour
{
    public GameObject canvas;
    public RecipeBlock recipeBlockPrefab;
    public Recipe recipe;
    void Start()
    {
        RecipeBlock block = Instantiate(recipeBlockPrefab, canvas.transform);
        block.Initialize(this, recipe, 1);
    }
    public void RecipeBlockOnDrag(RecipeBlock block, BaseEventData data)
    {
        PointerEventData data2 = (PointerEventData)data;
        RectTransform blockRect = block.GetComponent<RectTransform>();
        blockRect.position += (Vector3)data2.delta;
        Vector2 clamp = block.GetComponent<RectTransform>().localPosition;
        float blockX = blockRect.sizeDelta.x;
        float blockY = blockRect.sizeDelta.y;
        if (clamp.x < blockX/2 - 960)
            clamp.x = blockX/2 - 960;
        else if (clamp.x > 960 - blockX/2)
            clamp.x = 960 - blockX/2;
        if (clamp.y < blockY - 540)
            clamp.y = blockY - 540;
        else if (clamp.y > 540-blockY)
            clamp.y = 540 - blockY;
        blockRect.localPosition = clamp;
    }
}
