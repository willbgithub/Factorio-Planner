using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlanner : MonoBehaviour
{
    // Test values
    [SerializeField] Recipe testRecipe;
    [SerializeField] Recipe testRecipe2;
    [SerializeField] Fraction testFactor;
    [SerializeField] Fraction testFactor2;

    [ReadOnly] Node selectedNode;

    [SerializeField] GameObject canvas;
    [SerializeField] Transform arrows;
    [SerializeField] Transform recipeBlocks;
    [SerializeField] RecipeBlock recipeBlockPrefab;
    [SerializeField] Arrow arrowPrefab;

    const string UNITY_RECIPE_PATH = @"Assets/Prototypes/Recipes/";
    public void Start()
    {
        Debug.Log("UIPlanner: Start");
        CreateRecipeBlock(testRecipe, testFactor, new Vector2(300, 500));
        CreateRecipeBlock(testRecipe2, testFactor2, new Vector2(1500, 500));
    }
    public float GetWidth()
    {
        return canvas.GetComponent<RectTransform>().sizeDelta.x;
    }
    public float GetHeight()
    {
        return canvas.GetComponent<RectTransform>().sizeDelta.y;
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
    public RecipeBlock CreateRecipeBlock(Recipe recipe, Fraction factor, Vector2 position)
    {
        Debug.Log("RecipeBlock: CreateRecipeBlock");
        if (recipe.IsUnityNull())
        {
            Debug.LogError("ERROR: Recipe is null!");
        }
        if (factor.IsUnityNull())
        {
            Debug.LogError("ERROR: Factor is null!");
        }
        if (position.IsUnityNull())
        {
            Debug.LogError("ERROR: Position is null!");
        }
        RecipeBlock recipeBlock = Instantiate(recipeBlockPrefab, recipeBlocks.transform);
        recipeBlock.SetParent(this);
        recipeBlock.transform.position = position * GetScale();
        recipeBlock.SetRecipe(recipe);
        recipeBlock.SetFactor(factor);
        return recipeBlock;
    }

    // EVENT SYSTEMS

    public void OnNodePointerClick(Node node)
    {
        if (selectedNode.IsUnityNull())
        {
            SelectNode(node);
        }
        else if (selectedNode == node)
        {
            DeselectNode();
        }
        else
        {
            Connect(selectedNode, node);
        }
    }
    public void Connect(Node node1, Node node2)
    {
        if (!Compatible(node1, node2))
        {
            DeselectNode();
            return;
        }

        Node inputNode = node1;
        Node productNode = node2;
        if (node1.GetNodeType() == NODETYPE.PRODUCT)
        {
            inputNode = node2;
            productNode = node1;
        }
        inputNode.AddProduct(productNode);
        productNode.AddInput(inputNode);
        Arrow arrow = CreateArrow(inputNode.transform.position * GetScale(), productNode.transform.position * GetScale());
        inputNode.AddArrow(arrow);
        productNode.AddArrow(arrow);
    }
    public Arrow CreateArrow(Vector2 pos1, Vector2 pos2)
    {
        Arrow arrow = Instantiate(arrowPrefab, arrows);
        arrow.UpdatePosition(pos1, pos2);
        return arrow;
    }
    public bool Compatible(Node node1, Node node2)
    {
        bool compatible = true;
        if (node1.GetNodeType() == NODETYPE.INPUT && node2.GetNodeType() == NODETYPE.INPUT)
        {
            Debug.LogError("ERROR: Cannot connect two input nodes!");
            compatible = false;
        }
        else if (node1.GetNodeType() == NODETYPE.PRODUCT && node2.GetNodeType() == NODETYPE.PRODUCT)
        {
            Debug.LogError("ERROR: Cannot connect two product nodes!");
            compatible = false;
        }
        if (node1.GetItem() != node2.GetItem())
        {
            Debug.LogError("ERROR: Items are not the same!");
            compatible = false;
        }
        if (node1.Contains(node2))
        {
            Debug.LogError("ERROR: Nodes are already connected!");
        }
        return compatible;
    }
    public void SelectNode(Node node)
    {
        selectedNode = node;
        node.SetSelected(true);
    }
    public void DeselectNode()
    {
        selectedNode.SetSelected(false);
        selectedNode = null;
    }
}
