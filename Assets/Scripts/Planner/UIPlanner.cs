using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlanner : MonoBehaviour
{
    // Test values
    [SerializeField] Recipe testRecipe;
    [SerializeField] Recipe testRecipe2;
    [SerializeField] Fraction testFactor;
    [SerializeField] Fraction testFactor2;


    Node selectedNode;

    // Prompts
    bool prompting = false;
    Node targetNode;
    // Recipe prompt
    Vector2 targetPos;
    // Product prompt
    Item demandedItem;
    Fraction demandRate;

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject recipePrompt;
    [SerializeField] GameObject productPrompt;
    [SerializeField] Transform recipeContextMenu;
    [SerializeField] TMP_InputField recipeNameInput;
    [SerializeField] Transform arrows;
    [SerializeField] Transform glowingArrows;
    [SerializeField] Transform recipeBlocks;
    [SerializeField] RecipeBlock recipeBlockPrefab;
    [SerializeField] Arrow arrowPrefab;
    [SerializeField] ContextItem contextitemPrefab;

    const string UNITY_RECIPE_PATH = @"Assets/Prototypes/Recipes/";
    public void Start()
    {
        Debug.Log("UIPlanner: Start");
        Unprompt();
        //CreateRecipeBlock(testRecipe, testFactor, new Vector2(300, 500));
        CreateRecipeBlock(testRecipe2, testFactor2, new Vector2(1500, 500));
    }
    
    static Recipe GetRecipe(string prefabName)
    {
        Debug.Log("UIPlanner: GetRecipe");
        Recipe recipe = AssetDatabase.LoadAssetAtPath<Recipe>(UNITY_RECIPE_PATH + prefabName + ".asset");
        if (recipe.IsUnityNull())
        {
            //Debug.LogError("ERROR: Could not find recipe \"" + prefabName + "\"");
        }
        return recipe;
    }
    public RecipeBlock CreateRecipeBlock(Recipe recipe, Fraction factor, Vector2 position)
    {
        Debug.Log("UIPlanner: CreateRecipeBlock");
        RecipeBlock recipeBlock = Instantiate(recipeBlockPrefab, recipeBlocks.transform);
        recipeBlock.SetParent(this);
        recipeBlock.SetRecipe(recipe);
        recipeBlock.SetFactor(factor);
        recipeBlock.transform.position = position * GetScale();
        return recipeBlock;
    }
    // EVENT SYSTEMS
    public void OnPointerClick(BaseEventData data)
    {
        //Debug.Log("UIPlanner: OnPointerClick");
        PointerEventData data2 = (PointerEventData)data;
        if (data2.button == PointerEventData.InputButton.Left)
        {
            DeselectNode();
        }
        else if (data2.button == PointerEventData.InputButton.Right)
        {
            if (!prompting)
            {
                targetPos = data2.position / GetScale();
                SetRecipePrompt(true);
            }
            else
            {
                Unprompt();
            }
        }
    }
    public void Unprompt()
    {
        Debug.Log("UIPlanner: Unprompt");
        SetRecipePrompt(false);
        SetProductPrompt(false);
    }
    public void SetRecipePrompt(bool prompt)
    {
        Debug.Log("UIPlanner: SetRecipePrompt");
        prompting = prompt;
        recipePrompt.SetActive(prompt);
    }
    public void OnRecipeButtonPress()
    {
        Debug.Log("UIPlanner: OnRecipeButtonPress");
        Recipe recipe = GetRecipe(recipeNameInput.text);
        RecipeBlock recipeBlock = CreateRecipeBlock(recipe, 1, targetPos);
        prompting = false;
        recipePrompt.SetActive(false);
    }
    public void OnNodePointerClick(Node node, BaseEventData data)
    {
        //Debug.Log("UIPlanner: OnNodePointerClick");
        PointerEventData pData = (PointerEventData)data;
        if (pData.button == PointerEventData.InputButton.Left)
        {
            if (selectedNode.IsUnityNull())
            {
                SelectNode(node);
            }
            else if (selectedNode == node)
            {
                DeselectNode();
            }
            else if (node.Contains(selectedNode))
            {
                Debug.Log("UIPlanner: Disconnecting these two nodes");
                node.Disconnect(selectedNode);
                DeselectNode();
            }
            else
            {
                Connect(selectedNode, node);
                DeselectNode();
            }
        }
        else if (pData.button == PointerEventData.InputButton.Right)
        {
            if (node.GetNodeType() == NODETYPE.INPUT)
            {
                Item item = node.GetItem();
                Recipe recipe = item.GetBestRecipe();
                Fraction factor = GetFactor(recipe, item, node.GetValue());
                RecipeBlock recipeBlock = CreateRecipeBlock(recipe, factor, node.gameObject.transform.position);
                Connect(recipeBlock, node);
            }
            else if (node.GetNodeType() == NODETYPE.PRODUCT)
            {
                if (node.GetItem().GetUsedBy().Count == 0 || node.GetExcess() <= 0)
                {
                    return;
                }
                SetProductPrompt(true, node);
            }
        }
    }
    public void SetProductPrompt(bool prompt, Node node = null)
    {
        Debug.Log("UIPlanner: SetProductPrompt");
        prompting = prompt;
        productPrompt.SetActive(prompt);
        if (prompt)
        {
            targetNode = node;
            demandRate = node.GetExcess();
            demandedItem = node.GetItem();
            List<Recipe> usedBy = demandedItem.GetUsedBy();
            for (int i = 0; i < usedBy.Count; i++)
            {
                Recipe recipe = usedBy[i];
                ContextItem contextRecipeItem = Instantiate(contextitemPrefab, recipeContextMenu);
                contextRecipeItem.Initialize(this, recipe);
            }
        }
        else
        {
            //Debug.Log("recipeContextMenu children: " + recipeContextMenu.childCount);
            for (int i = 0; i < recipeContextMenu.childCount; i++)
            {
                Destroy(recipeContextMenu.GetChild(i).gameObject);
            }
        }
    }
    public void OnRecipeContextItemPointerClick(ContextItem item)
    {
        Debug.Log("UIPlanner: OnRecipeContextItemPointerClick");
        Fraction factor = GetFactor(item.GetRecipe(), demandedItem, demandRate);
        RecipeBlock recipeBlock = CreateRecipeBlock(item.GetRecipe(), factor, new Vector2(600, 600));
        Connect(recipeBlock, targetNode);
        SetProductPrompt(false);
    }
    
    public Fraction GetFactor(Recipe recipe, Item desiredItem, Fraction desiredRate)
    {
        Debug.Log("UIPlanner: GetFactor");
        return desiredRate / Fraction.abs(recipe.GetRate(desiredItem));
    }
    public void Connect(Node node1, Node node2)
    {
        Debug.Log("UIPlanner: Connect");
        if (!Compatible(node1, node2))
        {   
            return;
        }
        Node inputNode = node1;
        Node productNode = node2;
        if (node1.GetNodeType() == NODETYPE.PRODUCT)
        {
            inputNode = node2;
            productNode = node1;
        }
        inputNode.AddNode(productNode);
        productNode.AddNode(inputNode);
        Arrow arrow = CreateArrow(inputNode, productNode);
        inputNode.AddArrow(arrow);
        productNode.AddArrow(arrow);
    }
    public void Connect(RecipeBlock block, Node node)
    {
        Debug.Log("UIPlanner: Connect");
        Node node2;
        if (node.GetNodeType() == NODETYPE.INPUT)
        {
            node2 = block.GetProductNode(node.GetItem());
        }
        else
        {
            node2 = block.GetInputNode(node.GetItem());
        }
        Connect(node, node2);
    }
    public void OnRecipeBlockStart(RecipeBlock recipeBlock)
    {
        Debug.Log("UIPlanner: OnRecipeBlockStart");
        UpdateArrows(recipeBlock);
    }
    public void OnArrowGlow(Arrow arrow, bool glow)
    {
        //Debug.Log("UIPlanner: OnArrowGlow");
        if (glow)
        {
            arrow.gameObject.transform.SetParent(glowingArrows);
        }
        else
        {
            arrow.gameObject.transform.SetParent(arrows);
        }
    }
    public void UpdateArrows(RecipeBlock recipeBlock)
    {
        //Debug.Log("UIPlanner: UpdateArrows");
        List<Arrow> arrows = recipeBlock.GetRelatedArrows();
        for (int i = 0; i < arrows.Count; i++)
        {
            arrows[i].UpdatePosition(GetScale());
        }
    }
    public Arrow CreateArrow(Node inputNode, Node productNode)
    {
        Debug.Log("UIPlanner: CreateArrow");
        Arrow arrow = Instantiate(arrowPrefab, arrows);
        arrow.SetNodes(inputNode, productNode, GetScale());
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
            compatible = false;
        }
        return compatible;
    }
    public void SelectNode(Node node)
    {
        Debug.Log("UIPlanner: SelectNode");
        selectedNode = node;
        node.SetSelected(true);
    }
    public void DeselectNode()
    {
        Debug.Log("UIPlanner: DeselectNode");
        if (selectedNode.IsUnityNull())
        {
            return;
        }
        selectedNode.SetSelected(false);
        selectedNode = null;
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
}
