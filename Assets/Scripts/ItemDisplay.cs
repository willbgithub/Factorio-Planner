using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public GameObject NODE_PREFAB;
    public GameObject inputLabel;
    public GameObject productLabel;
    public void Initialize(Item item, Fraction rate, bool isProduct, bool satisfied=false)
    {
        itemDisplay.sprite = item.GetIcon();
        this.satisfied = satisfied;
        this.item = item;
        Node node = Instantiate(NODE_PREFAB, transform).GetComponent<Node>();
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
