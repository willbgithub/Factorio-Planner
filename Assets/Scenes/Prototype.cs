using UnityEngine;

public class Prototype
{
    public Prototype(string name)
    {
        Debug.Log("Prototype constructor: \"" + name + "\"");
        this.name = name;
    }
    public void SetName(string name)
    {
        this.name = name;
    }
    public string GetName()
    {
        return name;
    }
    string name;
}
