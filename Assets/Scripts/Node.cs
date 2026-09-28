using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Node : MonoBehaviour
{
    public enum TYPE {INPUT, PRODUCT, COMBINATION};
    public ItemDisplay parent;
    public Item item;
    public Fraction value;
    public List<Node> leftNodes;
    public List<Node> rightNodes;
    public List<Arrow> relatedArrows;
    public TYPE type;
}
