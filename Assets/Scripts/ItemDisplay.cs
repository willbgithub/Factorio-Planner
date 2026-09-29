using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public GameObject NODE_PREFAB;
    public GameObject inputLabel;
    public GameObject productLabel;
    public RecipeBlock parent;
    public Item item;
    public bool satisfied;
    public Image itemDisplay;
    public Image colorBackground;
    public Image background;
    public Color UNSATISFIED_COLOR = new Color(255, 71, 93);
    public Color SATISFIED_COLOR = new Color(148, 255, 157);
    public Color DEFAULT_COLOR;
    public Color FOCUSED_COLOR;
    public Node node;
    public void Initialize(RecipeBlock parent, Item item, Fraction rate, bool isProduct, bool satisfied=false)
    {
        this.parent = parent;
        itemDisplay.sprite = item.GetIcon();
        this.satisfied = satisfied;
        this.item = item;
        node = Instantiate(NODE_PREFAB, transform).GetComponent<Node>();
        node.parent = this;
        node.item = item;
        node.value = rate;
        if (isProduct)
        {
            node.type = Node.TYPE.PRODUCT;
            inputLabel.SetActive(false);
            productLabel.GetComponent<TMP_Text>().text = rate.ToString();
        }
        else
        {
            node.type = Node.TYPE.INPUT;
            productLabel.SetActive(false);
            inputLabel.GetComponent<TMP_Text>().text = rate.ToString();
        }
    }
    public void OnPointerEnter(BaseEventData data)
    {
        background.color = FOCUSED_COLOR;
        parent.ItemDisplayOnPointerEnter();
    }
    public void OnPointerExit(BaseEventData data)
    {
        background.color = DEFAULT_COLOR;
        parent.ItemDisplayOnPointerExit();
    }
    public void OnPointerClick(BaseEventData data)
    {
        node.OnPointerClick(data);
    }
    public void SetSatisfied(bool satisfied)
    {
        this.satisfied = satisfied;
        if (satisfied)
        {
            colorBackground.color = SATISFIED_COLOR;
        }
        else
        {
            colorBackground.color = UNSATISFIED_COLOR;
        }
    }
}
