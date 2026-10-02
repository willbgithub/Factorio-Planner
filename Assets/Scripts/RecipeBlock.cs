using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class RecipeBlock : MonoBehaviour
{
    const int CELL_SIZE = 60;
    const int EMPTY_SIZE = 40;
    public UIPlanner parent;
    public TMP_Text recipeLabel;
    public TMP_Text factorLabel;
    public UnityEngine.UI.Image recipeIconBackground;
    public UnityEngine.UI.Image recipeIcon;
    public GameObject inputs;
    public GameObject products;
    public UnityEngine.UI.Image background;
    public Fraction factor;
    public Recipe recipe;
    public GameObject ITEM_DISPLAY_PREFAB;
    public GameObject updateFactorButton;
    public Color DEFAULT_COLOR = new Color(43 / 255, 43 / 255, 43/255);
    public Color FOCUS_COLOR = new Color(70/255, 70/255, 70/255);
    public Color FACTOR_DEFAULT_COLOR = new Color(236 / 255, 97 / 255, 111 / 255);
    public Color FACTOR_FOCUS_COLOR = new Color(236 / 255, 160 / 255, 160 / 255);

    bool dragging = false;
    bool pointed = false;
    bool taken = false;
    bool initialized = false;

    public void Initialize(UIPlanner parent, Recipe recipe, Fraction factor)
    {
        //Debug.Log("RECIPEBLOCK: Initialize(" + parent + ", " + recipe + ", " + factor + ")");
        this.parent = parent;
        SetRecipe(recipe);
        SetFactor(factor);
        UpdateItems();
        UpdateValues();
        initialized = true;
    }
    public void OnDestroy()
    {
        //Debug.Log("RECIPEBLOCK: OnDestroy");
    }
    public void Connect(Node node)
    {
        //Debug.Log("RECIPEBLOCK: Connect");
        if (node.type == Node.TYPE.INPUT)
        {
            for (int i = 0; i < products.transform.childCount; i++)
            {
                ItemDisplay product = products.transform.GetChild(i).GetComponent<ItemDisplay>();
                if (product.item == node.item)
                {
                    parent.Connect(product.node, node);
                    i = products.transform.childCount;
                }
            }
        }
    }
    public void OnFactorPointerEnter(BaseEventData data)
    {
        taken = true;
        background.color = DEFAULT_COLOR;
        updateFactorButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = FACTOR_FOCUS_COLOR;
    }
    public void OnFactorPointerExit(BaseEventData data)
    {
        taken = false;
        if (pointed)
        {
            background.color = FOCUS_COLOR;
        }
        updateFactorButton.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = FACTOR_DEFAULT_COLOR;
    }
    public void OnFactorPointerClick(BaseEventData data)
    {
        //Debug.Log("RECIPEBLOCK: OnFactorPointerClick");

    }
    public void OnItemDisplayPointerClick(ItemDisplay itemDisplay, BaseEventData data)
    {
        parent.OnItemDisplayPointerClick(itemDisplay, data);
    }
    public void Start()
    {
        //Debug.Log("RECIPEBLOCK: Start()");
        if (!initialized && !parent.IsUnityNull() && !recipe.IsUnityNull() && !factor.IsUnityNull())
        {
            Initialize(parent, recipe, factor);
        }
    }
    public void UpdateArrows(Vector2 scale)
    {
        //Debug.Log("RECIPEBLOCK: UpdateArrows");
        for (int i = 0; i < inputs.transform.childCount; i++)
        {
            inputs.transform.GetChild(i).gameObject.GetComponent<ItemDisplay>().UpdateArrows(scale);
        }
        for (int i = 0; i < products.transform.childCount; i++)
        {
            products.transform.GetChild(i).gameObject.GetComponent<ItemDisplay>().UpdateArrows(scale);
        }
    }
    public void UpdateValues()
    {
        //Debug.Log("RECIPEBLOCK: UpdateValues");
        parent.RecipeBlockUpdateValues(this);
    }
    public void ItemDisplayOnPointerEnter()
    {
        background.color = DEFAULT_COLOR;
    }
    public void ItemDisplayOnPointerExit()
    {
        if (!pointed)
        {
            return;
        }
        background.color = FOCUS_COLOR;
    }
    public void OnPointerEnter(BaseEventData data)
    {
        pointed = true;
        if (taken)
        {
            return;
        }
        background.color = FOCUS_COLOR;
    }
    public void OnPointerExit(BaseEventData data)
    {
        pointed = false;
        if (dragging || taken)
        {
            return;
        }
        background.color = DEFAULT_COLOR;
    }
    public void OnDragStart(BaseEventData data)
    {
        dragging = true;
    }
    public void OnDrag(BaseEventData data)
    {
        parent.RecipeBlockOnDrag(this, data);
    }
    public void OnDragEnd(BaseEventData data)
    {
        dragging = false;
        if (pointed)
        {
            return;
        }
        background.color = DEFAULT_COLOR;
    }
    public void UpdateItems()
    {
        //.Log("RECIPEBLOCK: UpdateItems");
        //Debug.Log("RECIPEBLOCK: Block has " + inputs.transform.childCount + " inputs.");
        for (int i = 0; i < inputs.transform.childCount; i++)
        {
            Destroy(inputs.transform.GetChild(i).gameObject);
        }
        inputs.transform.DetachChildren();
        //Debug.Log("RECIPEBLOCK: Block has " + products.transform.childCount + " products.");
        for (int i = 0; i < products.transform.childCount; i++)
        {
            Destroy(products.transform.GetChild(i).gameObject);
        }
        products.transform.DetachChildren();
        Contribution contribution = recipe.GetContribution();
        List<ItemValue> inputValues = contribution.GetInputs();
        for (int i = 0; i < inputValues.Count; i++)
        {
            AddInput(inputValues[i].Multiply(factor));
        }
        List<ItemValue> productValues = contribution.GetProducts();
        for (int i = 0; i < productValues.Count; i++)
        {
            AddProduct(productValues[i].Multiply(factor));
        }
        UpdateHeight();
    }
    public void AddInput(ItemValue input)
    {
        //Debug.Log("AddInput(" + input + ")");
        ItemDisplay itemDisplay = Instantiate(ITEM_DISPLAY_PREFAB, inputs.transform).GetComponent<ItemDisplay>();
        itemDisplay.Initialize(this, input.GetItem(), input.GetValue(), false);
    }
    public void AddProduct(ItemValue product)
    {
        //Debug.Log("AddProduct(" + product + ")");
        ItemDisplay itemDisplay = Instantiate(ITEM_DISPLAY_PREFAB, products.transform).GetComponent<ItemDisplay>();
        itemDisplay.Initialize(this, product.GetItem(), product.GetValue(), true);
    }
    public int GetHeight()
    {
        if (inputs.transform.childCount > products.transform.childCount)
        {
            return inputs.transform.childCount;
        }
        return products.transform.childCount;
    }
    public void UpdateHeight()
    {
        int height = GetHeight();
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, EMPTY_SIZE + height * (CELL_SIZE+5));
    }
    public void SetRecipe(Recipe recipe)
    {
        this.recipe = recipe;
        recipeLabel.text = recipe.GetEnglishName();
        recipeIcon.sprite = recipe.GetIcon();
    }
    public void SetFactor(Fraction factor)
    {
        if (factor.IsUnityNull())
        {
            factor = 1;
        }
        this.factor = factor;
        factorLabel.text = factor.ToString();
    }
}
