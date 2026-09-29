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
    public Fraction factor;
    public Recipe recipe;
    public GameObject ITEM_DISPLAY_PREFAB;
    public void Initialize(UIPlanner parent, Recipe recipe, Fraction factor)
    {
        this.parent = parent;
        SetRecipe(recipe);
        SetFactor(factor);
        UpdateItems();
    }
    public void OnDrag(BaseEventData data)
    {
        parent.RecipeBlockOnDrag(this, data);
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
        itemDisplay.Initialize(input.GetItem(), input.GetValue(), false);
    }
    public void AddProduct(ItemValue product)
    {
        ItemDisplay itemDisplay = Instantiate(ITEM_DISPLAY_PREFAB, products.transform).GetComponent<ItemDisplay>();
        itemDisplay.Initialize(product.GetItem(), product.GetValue(), true);
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
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, EMPTY_SIZE + height * CELL_SIZE);
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
