using System.Collections.Generic;
using UnityEngine;

public enum NODETYPE { INPUT, PRODUCT }

public class Node : MonoBehaviour
{
    
    
    NODETYPE nodeType;
    Item item;
    Fraction value;
    List<Arrow> relatedArrows;
    List<Node> inputNodes;
    List<Node> productNodes;
    [SerializeField] GameObject nodePrefab;
    public void Start()
    {
        Debug.Log("Node: Start");
    }
    public Node CreateNode(Transform parent, NODETYPE nodeType, Item item, Fraction value)
    {
        Debug.Log("Node: CreateNode");
        Node node = Instantiate(nodePrefab, parent).GetComponent<Node>();
        node.nodeType = nodeType;
        node.item = item;
        node.SetValue(value);
        return node;
    }
    public void SetItem(Item item)
    {

    }
    public void SetValue(Fraction value)
    {
        Debug.Log("Node: SetValue");
        this.value = value;
        UpdateExcess();
    }
    public void UpdateExcess()
    {
        Debug.Log("Node: UpdateExcess");
    }
    public void OnDestroy()
    {
        Debug.Log("Node: OnDestroy");
        for (int i = 0; i < relatedArrows.Count; i++)
        {
            Destroy(relatedArrows[i].gameObject);
        }
    }
}
