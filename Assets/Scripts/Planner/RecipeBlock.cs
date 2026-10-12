using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class RecipeBlock : MonoBehaviour
{
    UIPlanner parent;
    Recipe recipe;
    Fraction factor;

    [SerializeField] static RecipeBlock recipeBlockPrefab;
    [SerializeField] GameObject inputs;
    [SerializeField] GameObject products;
    public void Start()
    {
        Debug.Log("RecipeBlock: Start");
    }
    public static RecipeBlock CreateRecipeBlock(UIPlanner caller, Transform parent, Recipe recipe, Fraction factor)
    {
        Debug.Log("RecipeBlock: CreateRecipeBlock");
        RecipeBlock recipeBlock = Instantiate(recipeBlockPrefab, parent);
        recipeBlock.parent = caller;
        recipeBlock.SetRecipe(recipe);
        recipeBlock.SetFactor(factor);
        return recipeBlock;
    }
    public void SetRecipe(Recipe recipe)
    {
        Debug.Log("RecipeBlock: SetRecipe");
        // Erase previous nodes
        List<Node> nodes = GetNodes();
        for (int i = 0; i < nodes.Count; i++)
        {
            Destroy(nodes[i].gameObject);
        }
        // Add nodes from recipe
        List<ItemValue> recipeInputs = recipe.GetRawInputs();
        for (int i = 0; i < recipeInputs.Count; i++)
        {
            Node.CreateNode(this, inputs.transform, NODETYPE.INPUT, recipeInputs[i].GetItem(), recipeInputs[i].GetValue());
        }
        List<ItemValue> recipeProducts = recipe.GetRawProducts();
        for (int i = 0; i < recipeProducts.Count; i++)
        {
            Node.CreateNode(this, products.transform, NODETYPE.PRODUCT, recipeProducts[i].GetItem(), recipeProducts[i].GetValue());
        }
        // Update height of recipe block
        int height = GetHeight();
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 35 + 65 * height);
    }
    public int GetHeight()
    {
        Debug.Log("RecipeBlock: GetHeight");
        if (inputs.transform.childCount > products.transform.childCount)
            return inputs.transform.childCount;
        return products.transform.childCount;
    }
    public void SetFactor(Fraction factor)
    {
        Debug.Log("RecipeBlock: SetFactor");
        if (factor == 0)
        {
            Destroy(gameObject);
            return;
        }
        List<Node> inputs = GetInputs();
        for (int i = 0; i < inputs.Count; i++)
        {
            Fraction value = recipe.GetRawInput(inputs[i].GetItem()) * factor;
            inputs[i].SetValue(value);
        }
        List<Node> products = GetProducts();
        for (int i = 0; i < products.Count; i++)
        {
            Fraction value = recipe.GetRawProduct(products[i].GetItem()) * factor;
            products[i].SetValue(value);
        }
    }
    public List<Node> GetInputs()
    {
        Debug.Log("RecipeBlock: GetInputs");
        List<Node> list = new List<Node>();
        for (int i = 0; i < inputs.transform.childCount; i++)
        {
            list.Add(inputs.transform.GetChild(i).GetComponent<Node>());
        }
        return list;
    }
    public List<Node> GetProducts()
    {
        Debug.Log("RecipeBlock: GetProducts");
        List<Node> list = new List<Node>();
        for (int i = 0; i < products.transform.childCount; i++)
        {
            list.Add(products.transform.GetChild(i).GetComponent<Node>());
        }
        return list;
    }
    public List<Node> GetNodes()
    {
        Debug.Log("RecipeBlock: GetNodes");
        List<Node> list = new List<Node>();
        for (int i = 0; i < inputs.transform.childCount; i++)
        {
            list.Add(inputs.transform.GetChild(i).GetComponent<Node>());
        }
        for (int i = 0; i < products.transform.childCount; i++)
        {
            list.Add(products.transform.GetChild(i).GetComponent<Node>());
        }
        return list;
    }
    public void OnDestroy()
    {
        Debug.Log("RecipeBlock: OnDestroy");
    }
}