using Radishmouse;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Node leftNode;
    public Node rightNode;
    public UILineRenderer line;

    public void Initialize(Node leftNode, Node rightNode, Vector2 scale)
    {
        this.leftNode = leftNode;
        this.rightNode = rightNode;
        line.points = new Vector2[2];
        UpdateGraphic(scale);
    }
    public void UpdateGraphic(Vector2 scale)
    {
        Vector2 leftPos = leftNode.transform.position / scale;
        Vector2 rightPos = rightNode.transform.position / scale;
        line.points[0] = leftPos;
        line.points[1] = rightPos;
        line.OnRebuildRequested();
    }
    public bool Contains(Node node)
    {
        return leftNode == node || rightNode == node;
    }
    public void OnDestroy()
    {
        //Debug.Log("ARROW: OnDestroy");
    }
}
