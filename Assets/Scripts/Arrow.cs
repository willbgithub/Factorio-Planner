using Radishmouse;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Node leftNode;
    public Node rightNode;
    public UILineRenderer line;

    // start at point on left
    public void Instantiate(Node leftNode, Node rightNode, Vector2 scale)
    {
        this.leftNode = leftNode;
        this.rightNode = rightNode;
        line.points = new Vector2[2];
        UpdateGraphic(scale);
    }
    public void UpdateGraphic(Vector2 scale)
    {
        Debug.Log("point 1: " + leftNode.transform.position/scale);
        Debug.Log("point 2: " + rightNode.transform.position / scale);
        line.points[0] = leftNode.transform.position / scale;
        line.points[1] = rightNode.transform.position / scale;
    }
}
