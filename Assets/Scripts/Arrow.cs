using Radishmouse;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Node leftNode;
    public Node rightNode;
    public UILineRenderer line;

    // start at point on left
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
        //Debug.Log("point 1: " + leftPos);
        //Debug.Log("point 2: " + rightPos);
        line.points[0] = leftPos;
        line.points[1] = rightPos;
        line.OnRebuildRequested();
    }
}
