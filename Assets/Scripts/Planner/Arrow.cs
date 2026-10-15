using Radishmouse;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    UILineRenderer line;
    Node inputNode;
    Node productNode;

    [SerializeField] Color defaultColor;
    [SerializeField] Color glowColor;
    public void Awake()
    {
        //Debug.Log("Arrow: Awake");
        line = GetComponent<UILineRenderer>();
        line.points = new Vector2[2];
    }

    public void Glow(bool glow)
    {
        if (glow)
        {
            line.color = glowColor;
        }
        else
        {
            line.color = defaultColor;
        }
    }
    public bool Contains(Node node)
    {
        return node == inputNode || node == productNode;
    }
    public void SetNodes(Node inputNode, Node productNode, float scale)
    {
        this.inputNode = inputNode;
        this.productNode = productNode;
        UpdatePosition(scale);
    }
    public void UpdatePosition(float scale)
    {
        //Debug.Log("Arrow: UpdatePosition");
        line.points[0] = inputNode.transform.position / scale;
        line.points[1] = productNode.transform.position / scale;
        line.OnRebuildRequested();
    }
    public void OnDestroy()
    {
        //Debug.Log("Arrow: OnDestroy");
        inputNode.DisconnectArrow(this);
        productNode.DisconnectArrow(this);
    }
}
