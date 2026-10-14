using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows;

public enum NODETYPE { INPUT, PRODUCT }

public class Node : MonoBehaviour
{
    RecipeBlock parent;
    NODETYPE nodeType;
    [SerializeField] Item item;
    [SerializeField] Fraction value;
    [SerializeField] List<Arrow> relatedArrows;
    [SerializeField] List<Node> nodes;

    bool focused = false;
    bool pointed = false;
    bool selected = false;
    [SerializeField] Color defaultColor;
    [SerializeField] Color focusedColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Image background;
    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text inputLabel;
    [SerializeField] TMP_Text productLabel;
    public void Awake()
    {
        //Debug.Log("Node: Awake");
        relatedArrows = new List<Arrow>();
        nodes = new List<Node>();
    }
    public void AddNode(Node node)
    {
        nodes.Add(node);
    }
    // Only called by a node that has already properly disconnected this one
    public void RemoveNode(Node node)
    {
        nodes.Remove(node);
    }
    public string ListToString(List<Node> list)
    {
        if (list.Count == 0)
        {
            return "{}";
        }
        string msg = "{";
        for (int i = 0; i < list.Count-1; i++)
        {
            msg += list[i] + ", ";
        }
        msg += list[list.Count - 1] + "}";
        return msg;
    }
    public void AddArrow(Arrow arrow)
    {
        relatedArrows.Add(arrow);
    }
    public bool Contains(Node node)
    {
        return nodes.Contains(node);
    }
    public void SetSelected(bool selected)
    {
        this.selected = selected;
        if (selected)
        {
            background.color = selectedColor;
        }
        else
        {
            SetFocused(true);
        }
    }
    public void SetParent(RecipeBlock parent)
    {
        this.parent = parent;
    }
    public void SetNodeType(NODETYPE nodeType)
    {
        //Debug.Log("Node: SetNodeType");
        this.nodeType = nodeType;
        if (nodeType == NODETYPE.INPUT)
        {
            inputLabel.gameObject.SetActive(true);
            productLabel.gameObject.SetActive(false);
            return;
        }
        else if (nodeType == NODETYPE.PRODUCT)
        {
            inputLabel.gameObject.SetActive(false);
            productLabel.gameObject.SetActive(true);
        }
        else
        {
            //Debug.LogError("ERROR: Unhandled node type!");
        }
    }
    public NODETYPE GetNodeType()
    {
        return nodeType;
    }
    public void SetItem(Item item)
    {
        //Debug.Log("Node: SetItem");
        this.item = item;
        itemIcon.sprite = item.GetIcon();
    }
    public Item GetItem()
    {
        //Debug.Log("Node: GetItem");
        return item;
    }
    public void SetValue(Fraction value)
    {
        //Debug.Log("Node: SetValue");
        this.value = value;
        if (nodeType == NODETYPE.INPUT)
        {
            inputLabel.text = value.ToString();
        }
        else if (nodeType == NODETYPE.PRODUCT)
        {
            productLabel.text = value.ToString();
        }
        else
        {
            //Debug.LogError("ERROR: Unhandled node type!");
        }
            
        UpdateExcess();
    }
    public void UpdateExcess()
    {
        //Debug.Log("Node: UpdateExcess");
    }
    public void OnDestroy()
    {
        //Debug.Log(this + ": OnDestroy");
        if (relatedArrows.IsUnityNull())
        {
            return;
        }
        DisconnectAll();
    }
    public void DisconnectAll()
    {
        //Debug.Log(this + ": DisconnectAll");
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].RemoveNode(this);
        }
        for (int i = 0; i < relatedArrows.Count; i++)
        {
            Destroy(relatedArrows[i].gameObject);
        }
    }
    public void DisconnectArrow(Arrow arrow)
    {
        relatedArrows.Remove(arrow);
    }
    public void Disconnect(Node node)
    {
        //Debug.Log(this + ": Disconnect(" + node + ")");
        if (!Contains(node))
        {
            Debug.LogError("ERROR: Attempted to disconnect node that wasn't connected!");
        }
        nodes.Remove(node);
        for (int i = 0; i < relatedArrows.Count; i++)
        {
            if (relatedArrows[i].Contains(node))
            {
                Destroy(relatedArrows[i].gameObject);
                i = relatedArrows.Count;
            }
        }
        if (node.Contains(this))
        {
            node.RemoveNode(this);
        }
    }
    public override string ToString()
    {
        if (nodeType == NODETYPE.INPUT)
        {
            return "{InputNode, " + item + ", " + value + "}";
        }
        else if (nodeType == NODETYPE.PRODUCT)
        {
            return "{ProductNode, " + item + ", " + value + "}";
        }
        else
        {
            //Debug.LogError("ERROR: Unexpected node type!");
            return null;
        }
    }
    // POINTER EVENTS
    public void SetFocused(bool focused)
    {
        this.focused = focused;
        parent.SetFocusedChild(focused);
        if (focused && !selected && pointed)
        {
            background.color = focusedColor;
        }
        else if (!selected)
        {
            background.color = defaultColor;
        }
    }
    public void OnPointerEnter(BaseEventData data)
    {
        //Debug.Log("Node: OnPointerEnter");
        pointed = true;
        SetFocused(true);
        parent.SetFocused(false);
    }
    public void OnPointerExit(BaseEventData data)
    {
        //Debug.Log("Node: OnPointerExit");
        pointed = false;
        SetFocused(false);
        parent.SetFocused(true);
    }
    public void OnPointerClick(BaseEventData data)
    {
        //Debug.Log("Node: OnPointerClick");
        parent.OnNodePointerClick(this);
    }
    public List<Arrow> GetRelatedArrows()
    {
        return relatedArrows;
    }
}
