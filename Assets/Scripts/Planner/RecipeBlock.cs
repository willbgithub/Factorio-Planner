using System.Collections.Generic;
using TMPro;
using Unity.Collections;
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
    bool focused = false; // is the main focus of pointer
    bool pointed = false; // pointer is over this
    bool dragging = false;
    bool hasFocusedChild = false; // whether a child is focused or not
    [SerializeField] Color defaultColor;
    [SerializeField] Color focusedColor;
    [SerializeField] Color updateFactorButtonDefaultColor;
    [SerializeField] Color updateFactorButtonFocusedColor;
    [SerializeField] GameObject inputs;
    [SerializeField] GameObject products;
    [SerializeField] Image updateFactorButton;
    [SerializeField] TMP_Text recipeLabel;
    [SerializeField] TMP_Text factorLabel;
    [SerializeField] Image recipeIcon;
    [SerializeField] Image background;
    [SerializeField] GameObject nodePrefab;
    public void Awake()
    {
        //Debug.Log("RecipeBlock: Awake");
    }
    public void Start()
    {
        parent.OnRecipeBlockStart(this);
    }
    public float Clamp(float val, float min, float max)
    {
        if (val < min)
            val = min;
        else if (val > max)
            val = max;
        return val;
    }
    public Node CreateNode(Transform parent, NODETYPE nodeType, Item item, Fraction value)
    {
        //Debug.Log("Node: CreateNode");
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
        //Debug.Log("RecipeBlock: SetRecipe");
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
        //Debug.Log("RecipeBlock: GetHeight");
        if (inputs.transform.childCount > products.transform.childCount)
            return inputs.transform.childCount;
        return products.transform.childCount;
    }
    public void SetFactor(Fraction factor)
    {
        //Debug.Log("RecipeBlock: SetFactor");
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
        // Update connected nodes
        List<Node> connectedNotes = GetConnectedNodes();
        for (int i = 0; i < connectedNotes.Count; i++)
        {
            connectedNotes[i].UpdateExcess();
        }
    }
    public List<Node> GetConnectedNodes()
    {
        List<Node> nodes = GetNodes();
        List<Node> connectedNodes = new List<Node>();
        for (int i = 0; i < nodes.Count; i++)
        {
            List<Node> nodeConnections = nodes[i].GetNodes();
            for (int j = 0; j < nodeConnections.Count; j++)
            {
                connectedNodes.Add(nodeConnections[j]);
            }
        }
        return connectedNodes;
    }
    public List<Node> GetInputs()
    {
        //Debug.Log("RecipeBlock: GetInputs");
        List<Node> list = new List<Node>();
        for (int i = 0; i < inputs.transform.childCount; i++)
        {
            list.Add(inputs.transform.GetChild(i).GetComponent<Node>());
        }
        return list;
    }
    public List<Node> GetProducts()
    {
        //Debug.Log("RecipeBlock: GetProducts");
        List<Node> list = new List<Node>();
        for (int i = 0; i < products.transform.childCount; i++)
        {
            list.Add(products.transform.GetChild(i).GetComponent<Node>());
        }
        return list;
    }
    public Node GetProductNode(Item item)
    {
        List<Node> products = GetProducts();
        for (int i = 0; i < products.Count; i++)
        {
            Node product = products[i];
            if (product.GetItem() == item)
            {
                return product;
            }
        }
        Debug.LogError("ERROR: Could not find product \"" + item + "\" in recipe block!");
        return null;
    }
    public List<Node> GetNodes()
    {
        //Debug.Log("RecipeBlock: GetNodes");
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
        //Debug.Log("RecipeBlock: OnDestroy");
    }
    public override string ToString()
    {
        return "{RecipeBlock: " + recipe + "}";
    }

    // EVENT SYSTEMS
    public void SetFocusedChild(bool focusedChild)
    {
        hasFocusedChild = focusedChild;
    }
    public void SetFocused(bool focused)
    {
        if (!pointed || hasFocusedChild)
        {
            focused = false;
        }
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
    public bool IsDragging()
    {
        return dragging;
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
        //Debug.Log("RecipeBlock: OnPointerEnter");
        pointed = true;
        SetFocused(true);
    }
    public void OnPointerExit(BaseEventData data)
    {
        //Debug.Log("RecipeBlock: OnPointerExit");
        pointed = false;
        SetFocused(false);
    }
    public void OnPointerClick(BaseEventData data)
    {

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

        parent.UpdateArrows(this);
    }
    public void SetUpdateFactorButtonFocused(bool updateFactorButtonFocused)
    {
        //this.updateFactorButtonFocused = updateFactorButtonFocused;
        if (updateFactorButtonFocused)
        {
            updateFactorButton.color = updateFactorButtonFocusedColor;
        }
        else
        {
            updateFactorButton.color = updateFactorButtonDefaultColor;
        }
    }
    public void OnUpdateFactorButtonPointerEnter(BaseEventData data)
    {
        SetUpdateFactorButtonFocused(true);
    }
    public void OnUpdateFactorButtonPointerExit(BaseEventData data)
    {
        SetUpdateFactorButtonFocused(false);
    }
    public void OnUpdateFactorButtonPointerClick(BaseEventData data)
    {
        UpdateFactor();
    }
    public void OnArrowGlow(Arrow arrow, bool glow)
    {
        parent.OnArrowGlow(arrow, glow);
    }
    public void UpdateFactor()
    {
        Debug.Log("RecipeBlock: UpdateFactor");
        List<Node> nodes = GetProducts();
        Node lowestExcess = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].GetExcess() < lowestExcess.GetExcess())
            {
                lowestExcess = nodes[i];
            }
        }
        Debug.Log("lowestExcess: " + lowestExcess);
        Debug.Log("lowestExcess.GetExcess: " + lowestExcess.GetExcess());
        Debug.Log("lowestExcess.GetValue: " + lowestExcess.GetValue());
        Fraction factor = (lowestExcess.GetValue() - lowestExcess.GetExcess()) / Fraction.abs(recipe.GetRate(lowestExcess.GetItem()));
        Debug.Log("Factor: " + factor);
        SetFactor(factor);
    }
    public List<Arrow> GetRelatedArrows()
    {
        List<Arrow> relatedArrows = new List<Arrow>();
        List<Node> nodes = GetNodes();
        for (int i = 0; i < nodes.Count; i++)
        {
            List<Arrow> nodeRelatedArrows = nodes[i].GetRelatedArrows();
            for (int j = 0; j < nodeRelatedArrows.Count; j++)
            {
                relatedArrows.Add(nodeRelatedArrows[j]);
            }
        }
        return relatedArrows;
    }
    public void OnNodePointerClick(Node node, BaseEventData data)
    {
        parent.OnNodePointerClick(node, data);
    }
}