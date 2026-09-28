using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public void Initialize(Item item, Fraction rate, bool isProduct, bool satisfied=false)
    {
        itemDisplay.sprite = item.GetIcon();
        this.satisfied = satisfied;
        this.item = item;
        Node node = new Node();
        Instantiate(node, transform);
        node.parent = this;
        node.item = item;
        node.value = rate;
        if (isProduct)
        {
            node.type = Node.TYPE.PRODUCT;
        }
        else
        {
            node.type = Node.TYPE.INPUT;
        }
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
    public Item item;
    public bool satisfied;
    public Image itemDisplay;
    public Image colorBackground;
    public static Color UNSATISFIED_COLOR = new Color(255, 71, 93);
    public static Color SATISFIED_COLOR = new Color(148, 255, 157);
}
