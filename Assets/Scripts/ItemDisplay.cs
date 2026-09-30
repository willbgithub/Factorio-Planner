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
    public Color UNSATISFIED_COLOR = new Color(255/255, 71 / 255, 93 / 255);
    public Color SATISFIED_COLOR = new Color(148 / 255, 255 / 255, 157 / 255);
    public Color DEFAULT_COLOR;
    public Color FOCUSED_COLOR;
    public Color SELECTED_COLOR = new Color(220/255, 220/255, 80/255);
    public Node node;
    public bool selected = false;
    public bool focused = false;
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
            Debug.Log("isProduct is true: this is for item \"" + item + "\" of rate " + rate);
            node.type = Node.TYPE.PRODUCT;
            inputLabel.SetActive(false);
            productLabel.GetComponent<TMP_Text>().text = rate.ToString();
        }
        else
        {
            Debug.Log("isInput is true: this is for item \"" + item + "\" of rate " + rate);
            node.type = Node.TYPE.INPUT;
            productLabel.SetActive(false);
            inputLabel.GetComponent<TMP_Text>().text = rate.ToString();
        }
    }
    public override string ToString()
    {
        return "{ItemDisplay: " + item + ", " + node.TypeToString() + "}";
    }
    public void SetSelected(bool selected)
    {
        this.selected = selected;
        if (selected)
        {
            background.color = SELECTED_COLOR;
        }
        else if (focused)
        {
            background.color = FOCUSED_COLOR;
        }
        else
        {
            background.color = DEFAULT_COLOR;
        }
    }
    public bool Incompatible(ItemDisplay itemDisplay)
    {
        return node.Incompatible(itemDisplay);
    }
    public bool Incompatible(Node node)
    {
        return this.node.Incompatible(node);
    }
    public Fraction GetUnaccountedInput()
    {
        return node.GetUnaccountedInput();
    }
    public Fraction GetUnaccountedProduct()
    {
        return node.GetUnaccountedProduct();
    }
    public void OnPointerEnter(BaseEventData data)
    {
        focused = true;
        if (selected)
        {
            return;
        }
        background.color = FOCUSED_COLOR;
        parent.ItemDisplayOnPointerEnter();
    }
    public void OnPointerExit(BaseEventData data)
    {
        focused = false;
        if (selected)
        {
            return;
        }
        background.color = DEFAULT_COLOR;
        parent.ItemDisplayOnPointerExit();
    }
    public void OnPointerClick(BaseEventData data)
    {
        parent.OnItemDisplayPointerClick(this, data);
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
