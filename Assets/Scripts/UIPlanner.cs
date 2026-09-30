using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlanner : MonoBehaviour
{
    public GameObject canvas;
    public RecipeBlock recipeBlockPrefab;
    public RecipeBlock recipeBlock1;
    public RecipeBlock recipeBlock2;
    public Recipe recipe;
    public Node selectedNode = null;
    void Start()
    {

    }
    public void OnItemDisplayPointerClick(ItemDisplay itemDisplay, BaseEventData data)
    {
        if (selectedNode.IsUnityNull())
        {
            selectedNode = itemDisplay.node;
            itemDisplay.SetSelected(true);
            return;
        }
        if (selectedNode == itemDisplay.node)
        {
            selectedNode = null;
            itemDisplay.SetSelected(false);
            return;
        }
        if (itemDisplay.Incompatible(selectedNode))
        {
            Debug.Log("node 1: " + selectedNode);
            Debug.Log("node 2: " + itemDisplay.node);
            Debug.LogError("Cannot connect these two nodes!");
            selectedNode.parent.SetSelected(false);
            itemDisplay.SetSelected(false);
            selectedNode = null;
            return;
        }
        else
        {
            Debug.Log("Node is ready to connect");
            selectedNode.parent.SetSelected(false);
            itemDisplay.SetSelected(false);
            selectedNode = null;
        }
        // connect them
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
