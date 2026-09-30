using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Node leftNode;
    public Node rightNode;
    public LineRenderer line;
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
        Vector2 left = leftNode.GetComponent<RectTransform>().position;
        Vector2 right = rightNode.GetComponent<RectTransform>().position;
        line.SetPosition(0, left);
        line.SetPosition(1, right);
    }
}
