using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeBlock : MonoBehaviour
{
    public UIPlanner parent;
    public GameObject recipeLabel;
    public GameObject recipeIcon;
    public void Instantiate(Recipe recipe, Fraction factor)
    {
        recipeLabel.GetComponent<TMP_Text>().text = recipe.GetEnglishName();
        recipeIcon.GetComponent<Image>().sprite = recipe.GetIcon();
    }
}
