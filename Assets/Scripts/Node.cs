using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class Node : MonoBehaviour
{
    public enum TYPE {INPUT, PRODUCT, COMBINATION};
    public ItemDisplay parent;
    public Item item;
    public Fraction value;
    public List<Node> leftNodes;
    public List<Node> rightNodes;
    public List<Arrow> relatedArrows;
    public TYPE type;

    // Called when the ItemDisplay is clicked
    public void OnPointerClick(BaseEventData data)
    {
        Debug.Log(new ItemValue(item, value));
    }
    public bool IsSatisfied()
    {
        Fraction leftRates = GetRates(leftNodes);
        Fraction rightRates = GetRates(rightNodes);
        if (type == TYPE.INPUT)
        {
            if (value > leftRates)
            {
                return false;
            }
            if (value == leftRates)
            {
                return true;
            }
            Debug.LogError("ERROR: Node input is somehow receiving more than necessary!");
            return true;
        }
        
        if (type == TYPE.PRODUCT)
        {
            if (value > rightRates)
            {
                return false;
            }
            if (value == rightRates)
            {
                return true;
            }
            Debug.LogError("ERROR: Node product is somehow donating more than possible!");
            return true;
        }
        
        if (leftRates == rightRates)
        {
            return true;
        }
        Debug.LogError("ERROR: Combination node is somehow mismatched!");
        return false;
    }
    public Fraction GetRates(List<Node> list)
    {
        Fraction rates = 0;
        for (int i = 0; i < list.Count; i++)
        {
            rates += list[i].value;
        }
        return rates;
    }
}
