using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIPlanner : MonoBehaviour
{
    // Test values
    [SerializeField] Recipe testRecipe;
    [SerializeField] Recipe testRecipe2;
    [SerializeField] Fraction testFactor;
    [SerializeField] Fraction testFactor2;

    Node selectedNode;
    Vector2 targetPos;
    bool prompting = false;
    bool productPrompting = false;

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject recipePrompt;
    [SerializeField] TMP_InputField recipeNameInput;
    [SerializeField] Transform arrows;
    [SerializeField] Transform glowingArrows;
    [SerializeField] Transform recipeBlocks;
    [SerializeField] RecipeBlock recipeBlockPrefab;
    [SerializeField] Arrow arrowPrefab;

    const string UNITY_RECIPE_PATH = @"Assets/Prototypes/Recipes/";
    public void Start()
    {
        //Debug.Log("UIPlanner: Start");
        recipePrompt.SetActive(false);
        prompting = false;
        productPrompting = false;
        //CreateRecipeBlock(testRecipe, testFactor, new Vector2(300, 500));
        CreateRecipeBlock(testRecipe2, testFactor2, new Vector2(1500, 500));
    }
    
    static Recipe GetRecipe(string prefabName)
    {
        Recipe recipe = AssetDatabase.LoadAssetAtPath<Recipe>(UNITY_RECIPE_PATH + prefabName + ".asset");
        if (recipe.IsUnityNull())
        {
            //Debug.LogError("ERROR: Could not find recipe \"" + prefabName + "\"");
        }
        return recipe;
    }
    public RecipeBlock CreateRecipeBlock(Recipe recipe, Fraction factor, Vector2 position)
    {
        //Debug.Log("RecipeBlock: CreateRecipeBlock");
        if (recipe.IsUnityNull())
        {
            //Debug.LogError("ERROR: Recipe is null!");
        }
        if (factor.IsUnityNull())
        {
            //Debug.LogError("ERROR: Factor is null!");
        }
        if (position.IsUnityNull())
        {
            //Debug.LogError("ERROR: Position is null!");
        }
        RecipeBlock recipeBlock = Instantiate(recipeBlockPrefab, recipeBlocks.transform);
        recipeBlock.SetParent(this);
        recipeBlock.transform.position = position * GetScale();
        recipeBlock.SetRecipe(recipe);
        recipeBlock.SetFactor(factor);
        return recipeBlock;
    }
    public RecipeBlock CreateRecipeBlock(Node node)
    {
        Fraction factor = GetFactor(node.GetItem(), node.GetValue());
        RecipeBlock recipeBlock = CreateRecipeBlock(node.GetItem().GetBestRecipe(), factor, node.transform.position);
        Node node2;
        if (node.GetNodeType() == NODETYPE.INPUT)
        {
            node2 = recipeBlock.GetProductNode(node.GetItem());
            Connect(node, node2);
        }
        return recipeBlock;
    }
    // EVENT SYSTEMS
    public void OnPointerClick(BaseEventData data)
    {
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
            }
            TogglePrompt();
        }
    }
    public void TogglePrompt()
    {
        prompting = !prompting;
        recipePrompt.SetActive(prompting);
    }
    public void OnRecipeButtonPress()
    {
        Recipe recipe = GetRecipe(recipeNameInput.text);
        CreateRecipeBlock(recipe, 1, targetPos);
        prompting = false;
        recipePrompt.SetActive(false);
    }
    public void OnNodePointerClick(Node node, BaseEventData data)
    {
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
                CreateRecipeBlock(node);
            }
            else if (node.GetNodeType() == NODETYPE.PRODUCT)
            {

            }
        }
    }
    public Fraction GetFactor(Item desiredItem, Fraction desiredRate)
    {
        Recipe recipe = desiredItem.GetBestRecipe();
        return desiredRate / recipe.GetRate(desiredItem);
    }
    public void Connect(Node node1, Node node2)
    {
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
    public void OnRecipeBlockStart(RecipeBlock recipeBlock)
    {
        UpdateArrows(recipeBlock);
    }
    public void OnArrowGlow(Arrow arrow, bool glow)
    {
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
        List<Arrow> arrows = recipeBlock.GetRelatedArrows();
        for (int i = 0; i < arrows.Count; i++)
        {
            arrows[i].UpdatePosition(GetScale());
        }
    }
    public Arrow CreateArrow(Node inputNode, Node productNode)
    {
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
        selectedNode = node;
        node.SetSelected(true);
    }
    public void DeselectNode()
    {
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
