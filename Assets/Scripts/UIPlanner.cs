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
    public void OnPointerClick(BaseEventData data)
    {
        PointerEventData data2 = (PointerEventData)data;
        Debug.Log(data2.button);
    }
    public void RecipeBlockOnDrag(RecipeBlock block, BaseEventData data)
    {
        PointerEventData data2 = (PointerEventData)data;
        RectTransform blockRect = block.GetComponent<RectTransform>();
        blockRect.position += (Vector3)data2.delta;
        Vector2 clamp = blockRect.localPosition;

        float blockX = blockRect.sizeDelta.x * blockRect.localScale.x;
        float blockY = blockRect.sizeDelta.y * blockRect.localScale.y;
        float canvasX = canvas.GetComponent<RectTransform>().sizeDelta.x;
        float canvasY = canvas.GetComponent<RectTransform>().sizeDelta.y;
        if (clamp.x < blockX/2 - canvasX/2)
            clamp.x = blockX/2 - canvasX/2;
        else if (clamp.x > canvasX/2 - blockX/2)
            clamp.x = canvasX/2 - blockX/2;
        if (clamp.y < blockY/2 - canvasY/2)
            clamp.y = blockY/2 - canvasY/2;
        else if (clamp.y > canvasY/2-blockY/2)
            clamp.y = canvasY/2 - blockY/2;
        blockRect.localPosition = clamp;
    }
}
