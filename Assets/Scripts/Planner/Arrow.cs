using Radishmouse;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    UILineRenderer line;
    public void Awake()
    {
        Debug.Log("Arrow: Awake");
        line = GetComponent<UILineRenderer>();
        line.points = new Vector2[2];
    }
    public void UpdatePosition(Vector2 pos1, Vector2 pos2)
    {
        Debug.Log("Arrow: UpdatePosition");
        line.points[0] = pos1;
        line.points[1] = pos2;
        line.OnRebuildRequested();
    }
    public void OnDestroy()
    {
        Debug.Log("Arrow: OnDestroy");
    }
}
