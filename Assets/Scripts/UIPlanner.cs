using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UIPlanner : MonoBehaviour
{
    public GameObject canvas;
    public GameObject recipeBlocks;
    public GameObject arrows;
    public GameObject recipePrompt;
    public GameObject recipeInput;
    //public GameObject factorInput;
    public RecipeBlock recipeBlockPrefab;
    public Arrow arrowPrefab;
    public Node rawInput;
    public Node rawProduct;
    [DoNotSerialize] public Node selectedNode = null;
    public Vector2 targetPosition;
    bool prompting = false;

    const string UNITY_ITEM_PATH = @"Assets/Prototypes/Items/";
    const string UNITY_RECIPE_PATH = @"Assets/Prototypes/Recipes/";

    public void Start()
    {
        //Debug.Log("UIPLANNER: Start");
        recipePrompt.SetActive(false);
        CreateRecipeBlock(GetRecipe("iron-gear-wheel"), 1, new Vector2(1500, 540));
        CreateRecipeBlock(GetRecipe("iron-plate"), 1, new Vector2(500, 540));
    }
    public Item GetItem(string prefabName)
    {
        Item item = AssetDatabase.LoadAssetAtPath<Item>(UNITY_ITEM_PATH + prefabName + ".asset");
        if (item.IsUnityNull())
        {
            Debug.LogError("ERROR: Could not find item \"" + prefabName + "\"");
        }
        return item;
    }
    public void OnPointerClick(BaseEventData data)
    {
        //Debug.Log("OnPointerClick");
        PointerEventData data2 = (PointerEventData)data;
        if (data2.button == PointerEventData.InputButton.Right && !prompting)
        {
            //Debug.Log("RMB pressed");
            targetPosition = data2.position / canvas.GetComponent<RectTransform>().localScale;
            CreateRecipeBlock();
        }
    }
    public void CreateRecipeBlock()
    {
        //Debug.Log("CreateRecipeBlock1");
        recipePrompt.SetActive(true);
        prompting = true;
        // Next call at OnRecipePromptConfirm
    }
    public void OnRecipePromptConfirm()
    {
        Recipe recipe = GetRecipe(recipeInput.GetComponent<TMP_InputField>().text);
        if (recipe.IsUnityNull())
        {
            return;
        }
        CreateRecipeBlock(recipe, 1, targetPosition);
        recipePrompt.SetActive(false);
        prompting = false;
    }
    public Recipe GetRecipe(string prefabName)
    {
        Recipe recipe = AssetDatabase.LoadAssetAtPath<Recipe>(UNITY_RECIPE_PATH + prefabName + ".asset");
        if (recipe.IsUnityNull())
        {
            Debug.LogError("ERROR: Could not find recipe \"" + prefabName + "\"");
        }
        return recipe;
    }
    public void OnDestroy()
    {
        //Debug.Log("UIPLANNER: OnDestroy");
    }
    public void RecipeBlockUpdateValues(RecipeBlock recipeBlock)
    {
        //Debug.Log("UIPLANNER: RecipeBlockUpdateValues");
        //Debug.Log("Recipe block has " + (recipeBlock.inputs.transform.childCount).ToString() + " inputs and " + (recipeBlock.products.transform.childCount).ToString() + " products.");
        for (int i = 0; i < recipeBlock.inputs.transform.childCount; i++)
        {
            //Debug.Log("recipeBlock: " + recipeBlock);
            //Debug.Log(".inputs: " + recipeBlock.inputs);
            //Debug.Log(".transform: " + recipeBlock.inputs.transform);
            //Debug.Log(".GetChild(i): " + recipeBlock.inputs.transform.GetChild(i));
            //Debug.Log(".gameObject: " + recipeBlock.inputs.transform.GetChild(i).gameObject);
            //Debug.Log(".ItemDisplay: " + recipeBlock.inputs.transform.GetChild(i).gameObject.GetComponent<ItemDisplay>());
            //Debug.Log(".node: " + recipeBlock.inputs.transform.GetChild(i).gameObject.GetComponent<ItemDisplay>().node);
            UpdateRawValues(recipeBlock.inputs.transform.GetChild(i).GetComponent<ItemDisplay>().node);
        }
        for (int i = 0; i < recipeBlock.products.transform.childCount; i++)
        {
            //Debug.Log("Product node: " + recipeBlock.products.transform.GetChild(i).GetComponent<ItemDisplay>().node);
            UpdateRawValues(recipeBlock.products.transform.GetChild(i).GetComponent<ItemDisplay>().node);
        }
    }
    public void OnItemDisplayPointerClick(ItemDisplay itemDisplay, BaseEventData data)
    {
        PointerEventData data2 = (PointerEventData)data;
        if (data2.button == PointerEventData.InputButton.Left)
        {
            SelectOrConnect(itemDisplay);
        }
        else if (data2.button == PointerEventData.InputButton.Right)
        {
            Resolve(itemDisplay);
        }
    }
    public void Resolve(ItemDisplay itemDisplay)
    {
        //Debug.Log("UIPLANNER: Resolve");
        if (itemDisplay.node.type == Node.TYPE.INPUT)
        {
            Fraction unresolved = itemDisplay.GetUnaccountedInput();
            Item item = itemDisplay.item;
            Recipe recipe = item.GetBestRecipe();
            Fraction factor = unresolved / recipe.GetRate(item);
            RecipeBlock block = CreateRecipeBlock(recipe, factor, Vector2.zero);
            block.Connect(itemDisplay.node);
        }
    }
    public RecipeBlock CreateRecipeBlock(Recipe recipe, Fraction factor, Vector2 position)
    {
        //Debug.Log("UIPLANNER: CreateRecipeBlock(" + recipe + ", " + factor + ")");
        RecipeBlock recipeBlock = Instantiate(recipeBlockPrefab, recipeBlocks.transform);
        recipeBlock.transform.position = position* canvas.GetComponent<RectTransform>().localScale;
        recipeBlock.Initialize(this, recipe, factor);
        return recipeBlock;
    }
    public void SelectOrConnect(ItemDisplay itemDisplay)
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
        Connect(selectedNode, itemDisplay.node);
        selectedNode.parent.SetSelected(false);
        itemDisplay.SetSelected(false);
        selectedNode = null;
    }
    public void UpdateRawValues(Node node)
    {
        Debug.Log("UpdateRawValues on " + node);
        node.Disconnect(rawInput);
        node.Disconnect(rawProduct);
        Debug.Log("Disconnected node from raw inputs and raw products.");
        Fraction rawInputValue = 0;
        Fraction rawProductValue = 0;
        if (node.type == Node.TYPE.INPUT)
        {
            rawInputValue = node.GetUnaccountedInput();
            Debug.Log("Node has " + rawInputValue + " unsatisfied demand");
        }
        else if (node.type == Node.TYPE.PRODUCT)
        {
            rawProductValue = node.GetUnaccountedProduct();
            Debug.Log("Node has " + rawProductValue + " extra production");
        }
        if (rawInputValue > 0)
        {
            Connect(node, rawInput);
            Debug.Log("Connected node to raw input");
        }
        if (rawProductValue > 0)
        {
            Connect(node, rawProduct);
            Debug.Log("Connected node to raw product");
        }
    }
    public void Connect(Node node1, Node node2)
    {
        if (node1.Incompatible(node2))
        {
            Debug.LogError("Cannot connect these two nodes!");
            return;
        }
        // Create arrow
        Arrow arrow = Instantiate(arrowPrefab, arrows.transform);
        arrow.Initialize(node1, node2, canvas.GetComponent<RectTransform>().localScale);
        node1.relatedArrows.Add(arrow);
        node2.relatedArrows.Add(arrow);
        // Update info
        if (node1.type == Node.TYPE.INPUT || node2.type == Node.TYPE.PRODUCT)
        {
            node1.leftNodes.Add(node2);
            node2.rightNodes.Add(node1);
            if (node1.IsConnected(rawInput) && node1.GetUnaccountedInput() <= 0)
            {
                node1.Disconnect(rawInput);
            }
            if (node2.IsConnected(rawProduct) && node2.GetUnaccountedProduct() <= 0)
            {
                node2.Disconnect(rawProduct);
            }
        }
        else if (node1.type == Node.TYPE.PRODUCT || node2.type == Node.TYPE.INPUT)
        {
            node1.rightNodes.Add(node2);
            node2.leftNodes.Add(node1);
            if (node1.IsConnected(rawProduct) && node1.GetUnaccountedProduct() <= 0)
            {
                node1.Disconnect(rawProduct);
            }
            if (node2.IsConnected(rawInput) && node2.GetUnaccountedInput() <= 0)
            {
                node2.Disconnect(rawInput);
            }
        }
        else
        {
            Debug.LogError("ERROR: You are connecting two combination nodes which is NOT SUPPORTED YET!!!");
        }
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

        block.UpdateArrows(canvas.GetComponent<RectTransform>().localScale);
    }
}
