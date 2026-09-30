using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Node leftNode;
    public Node rightNode;
    const int HEIGHT = 100;

    // start at point on left
    public void Instantiate(Node leftNode, Node rightNode)
    {
        this.leftNode = leftNode;
        this.rightNode = rightNode;
        UpdateGraphic();
    }
    public void UpdateGraphic()
    {
        Vector2 left = leftNode.transform.localPosition;
        Vector2 right = rightNode.transform.localPosition;
        Debug.Log("left node: " + leftNode + " at " + left);
        Debug.Log("left node: " + rightNode + " at " + right);
    }
}
