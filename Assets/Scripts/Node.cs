using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class Node : MonoBehaviour
{
    public enum TYPE { INPUT, PRODUCT, COMBINATION, RAWINPUT, RAWPRODUCT };
    public ItemDisplay parent;
    public Item item;
    public Fraction value;
    public List<Node> leftNodes;
    public List<Node> rightNodes;
    public List<Arrow> relatedArrows;
    public TYPE type;


    public void Disconnect(Node node)
    {
        if (!IsConnected(node))
            return;
        for (int i = 0; i < relatedArrows.Count; i++)
        {
            if (relatedArrows[i].Contains(node))
            {
                Arrow relatedArrow = relatedArrows[i];
                relatedArrows.Remove(relatedArrow);
                Destroy(relatedArrow.gameObject);
                i = relatedArrows.Count;
            }
        }
    }
    public bool IsConnected(Node node)
    {
        return leftNodes.Contains(node) || rightNodes.Contains(node);
    }
    public void UpdateArrows(Vector2 scale)
    {
        for (int i = 0; i < relatedArrows.Count; i++)
        {
            relatedArrows[i].UpdateGraphic(scale);
        }
    }
    public override string ToString()
    {
        return "{Node: " + item + ", " + value + ", " + TypeToString() + "}";
    }
    public bool Incompatible(Node node)
    {
        if (IsConnected(node))
        {
            Debug.LogError("These nodes are already connected!");
            return true;
        }
        if (type == TYPE.RAWINPUT || type == TYPE.RAWPRODUCT || node.type == TYPE.RAWINPUT || node.type == TYPE.RAWPRODUCT)
        {
            return false;
        }
        bool returnValue = false;
        if (item != node.item)
        {
            Debug.LogError("Incompatible: This node is of item \"" + item + "\", but the other node is of item \"" + node.item + "\"!");
            returnValue = true;
        }
        if ((type == node.type) && type != TYPE.COMBINATION)
        {
            Debug.LogError("Incompatible: This node is of type \"" + TypeToString() + "\", but the other node is of type \"" + node.TypeToString() + "\"!");
            returnValue = true;
        }
        return returnValue;
    }
    public bool Incompatible(ItemDisplay itemDisplay)
    {
        bool returnValue = false;
        if (item != itemDisplay.node.item)
        {
            Debug.LogError("Incompatible: This node is of item \"" + item + "\", but the other node is of item \"" + itemDisplay.node.item + "\"!");
            returnValue = true;
        }
        if ((type == itemDisplay.node.type) && type != TYPE.COMBINATION)
        {
            Debug.LogError("Incompatible: This node is of type \"" + TypeToString() + "\", but the other node is of type \"" + itemDisplay.node.TypeToString() + "\"!");
            returnValue = true;
        }
        return returnValue;
    }
    public string TypeToString()
    {
        if (type == TYPE.INPUT)
            return "input";
        if (type == TYPE.PRODUCT)
            return "product";
        return "combination";
    }
    public Fraction GetUnaccountedInput()
    {
        if (type == TYPE.PRODUCT)
        {
            Debug.LogError("ERROR: GetUnaccountedInput called on node of type product!");
            return null;
        }
        Fraction leftRates = GetRates(leftNodes);
        Fraction rightRates = GetRates(rightNodes);
        if (type == TYPE.INPUT)
        {
            return value - leftRates;
        }
        // Combination
        return rightRates - leftRates;
    }
    public Fraction GetUnaccountedProduct()
    {
        if (type == TYPE.INPUT)
        {
            Debug.LogError("ERROR: GetUnaccountedProduct called on node of type input!");
            return null;
        }
        Fraction leftRates = GetRates(leftNodes);
        Fraction rightRates = GetRates(rightNodes);
        if (type == TYPE.PRODUCT)
        {
            return value - rightRates;
        }
        
        // Combination
        return leftRates - rightRates;
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
