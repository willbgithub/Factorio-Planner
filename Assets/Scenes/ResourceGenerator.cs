using UnityEditor;
using UnityEngine;

public class ResourceGenerator
{
    [MenuItem("Factorio/Debug")]
    public static void DebugFunc()
    {
        Debug.Log("Debug start");
        Prototype x = new Prototype("gumburger");
        Debug.Log("Created prototype x with string \"" + x.GetName() + "\"");
    }
}
