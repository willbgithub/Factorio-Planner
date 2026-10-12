using System.Collections.Generic;
using UnityEngine;

public enum NODETYPE { INPUT, PRODUCT }

public class Node : MonoBehaviour
{
    
    
    NODETYPE nodeType;
    Item item;
    Fraction value;
    List<Arrow> relatedArrows;
    public void Start()
    {
        Debug.Log("Node: Start");
    }
    public void Initialize(Item item, Fraction value, NODETYPE nodeType)
    {
        Debug.Log("Node: Initialize");
        this.nodeType = nodeType;
        this.item = item;
        SetValue(value);
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
