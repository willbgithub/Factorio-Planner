using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class RecipeBlock : MonoBehaviour
{
    UIPlanner parent;
    [SerializeField] Recipe recipe;
    [SerializeField] Fraction factor;

    bool focused = false;
    bool dragging = false;
    [SerializeField] Color defaultColor;
    [SerializeField] Color focusedColor;
    [SerializeField] GameObject inputs;
    [SerializeField] GameObject products;
    [SerializeField] TMP_Text recipeLabel;
    [SerializeField] TMP_Text factorLabel;
    [SerializeField] Image recipeIcon;
    [SerializeField] Image background;
    [SerializeField] GameObject nodePrefab;
    public void Awake()
    {
        Debug.Log("RecipeBlock: Awake");
    }
    public Node CreateNode(Transform parent, NODETYPE nodeType, Item item, Fraction value)
    {
        Debug.Log("Node: CreateNode");
        Node node = Instantiate(nodePrefab, parent).GetComponent<Node>();
        node.SetParent(this);
        node.SetNodeType(nodeType);
        node.SetItem(item);
        node.SetValue(value);
        return node;
    }
    public void SetParent(UIPlanner parent)
    {
        this.parent = parent;
    }
    public void SetRecipe(Recipe recipe)
    {
        Debug.Log("RecipeBlock: SetRecipe");
        this.recipe = recipe;
        // Erase previous nodes
        List<Node> nodes = GetNodes();
        for (int i = 0; i < nodes.Count; i++)
        {
            Destroy(nodes[i].gameObject);
        }
        inputs.transform.DetachChildren();
        products.transform.DetachChildren();
        // Add nodes from recipe
        List<ItemValue> recipeInputs = recipe.GetRawInputs();
        for (int i = 0; i < recipeInputs.Count; i++)
        {
            CreateNode(inputs.transform, NODETYPE.INPUT, recipeInputs[i].GetItem(), recipeInputs[i].GetValue());
        }
        List<ItemValue> recipeProducts = recipe.GetRawProducts();
        for (int i = 0; i < recipeProducts.Count; i++)
        {
            CreateNode(products.transform, NODETYPE.PRODUCT, recipeProducts[i].GetItem(), recipeProducts[i].GetValue());
        }
        // Update height of recipe block
        int height = GetHeight();
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 35 + 65 * height);
        // Update UI
        recipeLabel.text = recipe.GetEnglishName();
        recipeIcon.sprite = recipe.GetIcon();
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
        // If the factor is 0, the block is producing and consuming nothing, so it should really just be deleted
        if (factor == 0)
        {
            Destroy(gameObject);
            return;
        }
        // Update all nodes
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
        // Update text
        factorLabel.text = factor.ToString();
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
    public override string ToString()
    {
        return "{RecipeBlock: " + recipe + "}";
    }

    // POINTER EVENTS
    public void SetFocused(bool focused)
    {
        this.focused = focused;
        if (focused)
        {
            background.color = focusedColor;
        }
        else if (!dragging)
        {
            background.color = defaultColor;
        }
    }
    public void SetDragging(bool dragging)
    {
        this.dragging = dragging;
        if (!dragging && !focused)
        {
            background.color = defaultColor;
        }
    }
    public void OnPointerEnter(BaseEventData data)
    {
        SetFocused(true);
    }
    public void OnPointerExit(BaseEventData data)
    {
        SetFocused(false);
    }
    public void OnPointerClick(BaseEventData data)
    {
        float scale = parent.GetScale();
        Debug.Log("Scale: " + scale);
        Debug.Log("pos: " + transform.position);
        Debug.Log("pos * scale: " + transform.position * scale);
        Debug.Log("pos / scale: " + transform.position / scale);
    }
    public void OnBeginDrag(BaseEventData data)
    {
        SetDragging(true);
    }
    public void OnEndDrag(BaseEventData data)
    {
        SetDragging(false);
    }
    public void OnDrag(BaseEventData data)
    {
        PointerEventData data2 = (PointerEventData)data;
        float width = GetComponent<RectTransform>().sizeDelta.x;
        float height = GetComponent<RectTransform>().sizeDelta.y;
        float scale = parent.GetScale();
        float left = width/2 * scale;
        float right = (parent.GetWidth() - width / 2) * scale;
        float down = height / 2 * scale;
        float up = (parent.GetHeight() - height / 2) * scale;
        
        transform.position = new Vector2(transform.position.x + data2.delta.x, transform.position.y + data2.delta.y);
        Vector2 pos = new Vector2(Clamp(transform.position.x, left, right), Clamp(transform.position.y, down, up));
        transform.position = pos;
    }
    public float Clamp(float val, float min, float max)
    {
        if (val < min)
            val = min;
        else if (val > max)
            val = max;
        return val;
    }
}