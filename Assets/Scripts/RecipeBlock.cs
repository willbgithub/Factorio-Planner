using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBlock : MonoBehaviour
{
    public UIPlanner parent;
    public TMP_Text recipeLabel;
    public TMP_Text factorLabel;
    public Image recipeIconBackground;
    public Image recipeIcon;
    public GameObject inputs;
    public GameObject products;
    public void Initialize(UIPlanner parent, Recipe recipe, Fraction factor)
    {
        this.parent = parent;
        recipeLabel.text = recipe.GetEnglishName();
        recipeIcon.sprite = recipe.GetIcon();
        factorLabel.text = factor.ToString();
    }
}
