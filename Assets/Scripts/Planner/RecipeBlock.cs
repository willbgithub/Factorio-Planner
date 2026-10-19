using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class RecipeBlock : MonoBehaviour
{
    UIPlanner parent;
    Recipe recipe;
    bool inputLocked = false;
    bool productLocked = false;
    bool focused = false;
    bool pointed = false;
    bool dragging = false;
    bool hasFocusedChild = false;
    bool initialized = false;
    // Serialized objects
    [SerializeField] Color defaultColor;
    [SerializeField] Color focusedColor;
    [SerializeField] Color buttonDefaultColor;
    [SerializeField] Color buttonFocusedColor;
    [SerializeField] Color buttonEnabledDefaultColor;
    [SerializeField] Color buttonEnabledFocusedColor;
    [SerializeField] GameObject inputs;
    [SerializeField] GameObject products;
    [SerializeField] TMP_Text recipeLabel;
    [SerializeField] TMP_Text factorLabel;
    [SerializeField] TMP_InputField factorInput;
    [SerializeField] Image recipeIcon;
    [SerializeField] Image recipeIconBackground;
    [SerializeField] Image inputButton;
    [SerializeField] Image productButton;
    [SerializeField] Image background;
    [SerializeField] GameObject nodePrefab;
    const int CELL_SIZE = 65;
    const int BOTTOM_SPACE = 50;
    public void Start()
    {
        initialized = true;
        SetFactorPrompt(false);
        if (inputLocked || productLocked)
        {
            UpdateFactor();
        }
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
        int height = inputs.transform.childCount;
        if (products.transform.childCount > height)
        {
            height = products.transform.childCount;
        }
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, BOTTOM_SPACE + CELL_SIZE * height);
        // Update UI
        recipeLabel.text = recipe.GetEnglishName();
        recipeIcon.sprite = recipe.GetIcon();
    }
    public void SetFactor(Fraction factor)
    {
        Debug.Log("RecipeBlock: SetFactor");
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
        // Update connected blocks
        List<RecipeBlock> inputBlocks = GetConnectedInputBlocks();
        Debug.Log(this + " has " + inputBlocks.Count + " input blocks.");
        for (int i = 0; i < inputBlocks.Count; i++)
        {
            if (inputBlocks[i].GetProductLocked())
            {
                inputBlocks[i].UpdateFactor();
            }
        }
        List<RecipeBlock> productBlocks = GetConnectedProductBlocks();
        Debug.Log(this + " has " + productBlocks.Count + " product blocks.");
        for (int i = 0; i < productBlocks.Count; i++)
        {
            if (productBlocks[i].GetInputLocked())
            {
                productBlocks[i].UpdateFactor();
            }
        }
        // Update connected nodes
        List<Node> connectedNotes = GetConnectedNodes();
        for (int i = 0; i < connectedNotes.Count; i++)
        {
            connectedNotes[i].UpdateExcess();
        }
    }
    public bool GetInputLocked()
    {
        return inputLocked;
    }
    public bool GetProductLocked()
    {
        return productLocked;
    }
    public List<RecipeBlock> GetConnectedBlocks()
    {
        List<RecipeBlock> blocks = new List<RecipeBlock>();
        List<Node> nodes = GetConnectedNodes();
        for (int i = 0; i < nodes.Count; i++)
        {
            RecipeBlock block = nodes[i].GetParent();
            if (blocks.Contains(block))
            {
                continue;
            }
            blocks.Add(block);
        }
        return blocks;
    }
    public List<RecipeBlock> GetConnectedInputBlocks()
    {
        List<RecipeBlock> blocks = new List<RecipeBlock>();
        List<Node> nodes = GetConnectedProducts();
        for (int i = 0; i < nodes.Count; i++)
        {
            RecipeBlock block = nodes[i].GetParent();
            if (blocks.Contains(block))
            {
                continue;
            }
            blocks.Add(block);
        }
        return blocks;
    }
    public List<RecipeBlock> GetConnectedProductBlocks()
    {
        List<RecipeBlock> blocks = new List<RecipeBlock>();
        List<Node> nodes = GetConnectedInputs();
        for (int i = 0; i < nodes.Count; i++)
        {
            RecipeBlock block = nodes[i].GetParent();
            if (blocks.Contains(block))
            {
                continue;
            }
            blocks.Add(block);
        }
        return blocks;
    }
    public void UpdateFactor()
    {
        if (!initialized || (!inputLocked && !productLocked))
        {
            return;
        }
        Debug.Log("RecipeBlock: UpdateFactor");
        Fraction greatestFactor = 0;
        if (inputLocked)
        {
            List<Node> connectedProducts = GetConnectedProducts();
            Debug.Log("Looking at " + connectedProducts.Count + " connected products");
            for (int i = 0; i < connectedProducts.Count; i++)
            {
                Debug.Log(connectedProducts[i]);
                Fraction factor = ResourceGenerator.GetFactor(connectedProducts[i].GetRecipe(), connectedProducts[i].GetItem(), connectedProducts[i].GetDemand());
                Debug.Log("factor: " + factor);
                if (factor > greatestFactor)
                {
                    greatestFactor = factor;
                }
            }
        }
        if (productLocked)
        {
            List<Node> connectedInputs = GetConnectedInputs();
            Debug.Log("Looking at " + connectedInputs.Count + " connected inputs");
            for (int i = 0; i < connectedInputs.Count; i++)
            {
                Debug.Log(connectedInputs[i]);
                Fraction factor = ResourceGenerator.GetFactor(recipe, connectedInputs[i].GetItem(), connectedInputs[i].GetExcess(connectedInputs[i]));
                Debug.Log("factor: " + factor);
                if (factor > greatestFactor)
                {
                    greatestFactor = factor;
                }
            }
        }
        SetFactor(greatestFactor);
    }
    public Recipe GetRecipe()
    {
        return recipe;
    }
    public void SetInputLocked(bool inputLocked)
    {
        this.inputLocked = inputLocked;
        if (inputLocked)
        {
            inputButton.color = buttonEnabledDefaultColor;
            UpdateFactor();
        }
        else
        {
            inputButton.color = buttonDefaultColor;
        }
    }
    public void SetProductLocked(bool productLocked)
    {
        this.productLocked = productLocked;
        if (productLocked)
        {
            productButton.color = buttonEnabledDefaultColor;
            UpdateFactor();
        }
        else
        {
            productButton.color = buttonDefaultColor;
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
    public List<Node> GetConnectedInputs()
    {
        Debug.Log(this + ": GetConnectedInputs");
        List<Node> nodes = GetProducts();
        Debug.Log("nodes.Count: " + nodes.Count);
        for (int i = 0; i < nodes.Count; i++)
        {
            Debug.Log(nodes[i]);
        }
        List<Node> connectedNodes = new List<Node>();
        for (int i = 0; i < nodes.Count; i++)
        {
            List<Node> nodeConnections = nodes[i].GetNodes();
            Debug.Log("connected to this node: " + nodeConnections.Count);
            for (int j = 0; j < nodeConnections.Count; j++)
            {
                connectedNodes.Add(nodeConnections[j]);
            }
        }
        return connectedNodes;
    }
    public List<Node> GetConnectedProducts()
    {
        List<Node> nodes = GetInputs();
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
    public Node GetInputNode(Item item)
    {
        List<Node> inputs = GetInputs();
        for (int i = 0; i < inputs.Count; i++)
        {
            Node input = inputs[i];
            if (input.GetItem() == item)
            {
                return input;
            }
        }
        Debug.LogError("ERROR: Could not find input \"" + item + "\" in recipe block!");
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
    public override string ToString()
    {
        return "{RecipeBlock: " + recipe + "}";
    }

    // EVENT SYSTEMS
    public void SetFocusedChild(bool focusedChild)
    {
        hasFocusedChild = focusedChild;
        if (!hasFocusedChild && pointed)
        {
            SetFocused(true);
        }
        else
        {
            SetFocused(false);
        }
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
    public void OnPointerEnter(BaseEventData d)
    {
        //Debug.Log("RecipeBlock: OnPointerEnter");
        pointed = true;
        SetFocused(true);
    }
    public void OnPointerExit(BaseEventData d)
    {
        //Debug.Log("RecipeBlock: OnPointerExit");
        pointed = false;
        SetFocused(false);
    }
    public void OnBeginDrag(BaseEventData d)
    {
        SetDragging(true);
    }
    public void OnEndDrag(BaseEventData d)
    {
        SetDragging(false);
    }
    public void OnDrag(BaseEventData d)
    {
        PointerEventData data = (PointerEventData)d;
        float width = GetComponent<RectTransform>().sizeDelta.x;
        float height = GetComponent<RectTransform>().sizeDelta.y;
        float scale = parent.GetScale();
        float left = width/2 * scale;
        float right = (parent.GetWidth() - width / 2) * scale;
        float down = height / 2 * scale;
        float up = (parent.GetHeight() - height / 2) * scale;
        
        transform.position = new Vector2(transform.position.x + data.delta.x, transform.position.y + data.delta.y);
        Vector2 pos = new Vector2(ResourceGenerator.Clamp(transform.position.x, left, right), ResourceGenerator.Clamp(transform.position.y, down, up));
        transform.position = pos;

        parent.UpdateArrows(this);
    }
    public void OnFactorLabelPointerClick(BaseEventData d)
    {
        //Debug.Log("Factor label clicked");
        SetFactorPrompt(true);
    }
    public void SetFactorPrompt(bool inputPrompt)
    {
        factorInput.gameObject.SetActive(inputPrompt);
        factorLabel.gameObject.SetActive(!inputPrompt);
        if (inputPrompt)
        {
            factorInput.text = factorLabel.text;
            factorInput.Select();
        }
    }
    public void OnFactorEndEdit(string factor)
    {
        SetFactor(new Fraction(factor));
        SetFactorPrompt(false);
    }
    public void SetRecipeIconFocused(bool recipeIconBackgroundFocused)
    {
        SetFocusedChild(recipeIconBackgroundFocused);
        if (recipeIconBackgroundFocused)
        {
            recipeIconBackground.color = buttonFocusedColor;
        }
        else
        {
            recipeIconBackground.color = buttonDefaultColor;
        }
    }
    public void OnRecipeIconPointerEnter(BaseEventData d)
    {
        SetRecipeIconFocused(true);
    }
    public void OnRecipeIconPointerExit(BaseEventData d)
    {
        SetRecipeIconFocused(false);
    }
    public void OnRecipeIconPointerClick(BaseEventData d)
    {
        Destroy(gameObject);
    }
    public void SetInputButtonFocused(bool focus)
    {
        SetFocusedChild(focus);
        if (focus)
        {
            if (inputLocked)
            {
                inputButton.color = buttonEnabledFocusedColor;
            }
            else
            {
                inputButton.color = buttonFocusedColor;
            }
        }
        else
        {
            if (inputLocked)
            {
                inputButton.color = buttonEnabledDefaultColor;
            }
            else
            {
                inputButton.color = buttonDefaultColor;
            }
        }
    }
    public void OnInputButtonPointerEnter(BaseEventData d)
    {
        SetInputButtonFocused(true);
    }
    public void OnInputButtonPointerExit(BaseEventData d)
    {
        SetInputButtonFocused(false);
    }
    public void OnInputButtonPointerClick(BaseEventData d)
    {
        SetInputLocked(!inputLocked);
        if (inputLocked)
        {
            inputButton.color = buttonEnabledFocusedColor;
        }
        else
        {
            inputButton.color = buttonFocusedColor;
        }
    }
    public void SetProductButtonFocused(bool focus)
    {
        SetFocusedChild(focus);
        if (focus)
        {
            if (productLocked)
            {
                productButton.color = buttonEnabledFocusedColor;
            }
            else
            {
                productButton.color = buttonFocusedColor;
            }
        }
        else
        {
            if (productLocked)
            {
                productButton.color = buttonEnabledDefaultColor;
            }
            else
            {
                productButton.color = buttonDefaultColor;
            }
        }
    }
    public void OnProductButtonPointerEnter(BaseEventData d)
    {
        SetProductButtonFocused(true);
    }
    public void OnProductButtonPointerExit(BaseEventData d)
    {
        SetProductButtonFocused(false);
    }
    public void OnProductButtonPointerClick(BaseEventData d)
    {
        SetProductLocked(!productLocked);
        if (productLocked)
        {
            productButton.color = buttonEnabledFocusedColor;
        }
        else
        {
            productButton.color = buttonFocusedColor;
        }
    }
    public void OnArrowGlow(Arrow arrow, bool glow)
    {
        parent.OnArrowGlow(arrow, glow);
    }
    public void OnNodePointerClick(Node node, BaseEventData d)
    {
        parent.OnNodePointerClick(node, d);
    }
}