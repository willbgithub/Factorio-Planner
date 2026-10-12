using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum NODETYPE { INPUT, PRODUCT }

public class Node : MonoBehaviour
{
    RecipeBlock parent;
    [SerializeField] NODETYPE nodeType;
    [SerializeField] Item item;
    [SerializeField] Fraction value;
    [SerializeField] List<Arrow> relatedArrows;
    [SerializeField] List<Node> inputNodes;
    [SerializeField] List<Node> productNodes;

    
    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text inputLabel;
    [SerializeField] TMP_Text productLabel;
    public void Awake()
    {
        Debug.Log("Node: Awake");
        relatedArrows = new List<Arrow>();
        inputNodes = new List<Node>();
        productNodes = new List<Node>();
    }
    public void SetParent(RecipeBlock parent)
    {
        this.parent = parent;
    }
    public void SetNodeType(NODETYPE nodeType)
    {
        Debug.Log("Node: SetNodeType");
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
            Debug.LogError("ERROR: Unhandled node type!");
        }
    }
    public void SetItem(Item item)
    {
        Debug.Log("Node: SetItem");
        this.item = item;
        itemIcon.sprite = item.GetIcon();
    }
    public Item GetItem()
    {
        Debug.Log("Node: GetItem");
        return item;
    }
    public void SetValue(Fraction value)
    {
        Debug.Log("Node: SetValue");
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
            Debug.LogError("ERROR: Unhandled node type!");
        }
            
        UpdateExcess();
    }
    public void UpdateExcess()
    {
        Debug.Log("Node: UpdateExcess");
    }
    public void OnDestroy()
    {
        Debug.Log("Node: OnDestroy");
        if (relatedArrows.IsUnityNull())
        {
            return;
        }
        for (int i = 0; i < relatedArrows.Count; i++)
        {
            Destroy(relatedArrows[i].gameObject);
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
            Debug.LogError("ERROR: Unexpected node type!");
            return null;
        }
    }
}
