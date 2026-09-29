using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows;

public class RecipeBlock : MonoBehaviour
{
    const int CELL_SIZE = 60;
    const int EMPTY_SIZE = 40;
    public UIPlanner parent;
    public TMP_Text recipeLabel;
    public TMP_Text factorLabel;
    public Image recipeIconBackground;
    public Image recipeIcon;
    public GameObject inputs;
    public GameObject products;
    public Image background;
    public Fraction factor;
    public Recipe recipe;
    public GameObject ITEM_DISPLAY_PREFAB;
    public Color DEFAULT_COLOR = new Color(43 / 255, 43 / 255, 43/255);
    public Color FOCUS_COLOR = new Color(70/255, 70/255, 70/255);
    bool dragging = false;
    bool pointed = false;
    public void Initialize(UIPlanner parent, Recipe recipe, Fraction factor)
    {
        this.parent = parent;
        SetRecipe(recipe);
        SetFactor(factor);
        UpdateItems();
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
        background.color = FOCUS_COLOR;
    }
    public void OnPointerExit(BaseEventData data)
    {
        pointed = false;
        if (dragging)
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
        for (int i = 0; i < inputs.transform.childCount; i++)
        {
            Destroy(inputs.transform.GetChild(0).gameObject);
        }
        for (int i = 0; i < products.transform.childCount; i++)
        {
            Destroy(products.transform.GetChild(0).gameObject);
        }
        Contribution contribution = recipe.GetContribution();
        List<ItemValue> inputValues = contribution.GetInputs();
        for (int i = 0; i < inputValues.Count; i++)
        {
            AddInput(inputValues[i]);
        }
        List<ItemValue> productValues = contribution.GetProducts();
        for (int i = 0; i < productValues.Count; i++)
        {
            AddProduct(productValues[i]);
        }
        UpdateHeight();
    }
    public void AddInput(ItemValue input)
    {
        ItemDisplay itemDisplay = Instantiate(ITEM_DISPLAY_PREFAB, inputs.transform).GetComponent<ItemDisplay>();
        itemDisplay.Initialize(this, input.GetItem(), input.GetValue(), false);
    }
    public void AddProduct(ItemValue product)
    {
        ItemDisplay itemDisplay = Instantiate(ITEM_DISPLAY_PREFAB, products.transform).GetComponent<ItemDisplay>();
        itemDisplay.Initialize(this, product.GetItem(), product.GetValue(), true);
    }
    public int GetHeight()
    {
        if (inputs.transform.childCount > products.transform.childCount)
        {
            return inputs.transform.childCount-1;
        }
        return products.transform.childCount-1;
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
