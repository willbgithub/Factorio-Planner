using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class RecipeBlock : MonoBehaviour
{
    [SerializeField] GameObject inputs;
    [SerializeField] GameObject products;
    Recipe recipe;
    Fraction factor;

    public void Start()
    {
        Debug.Log("RecipeBlock: Start");
    }
    public List<Node> GetInputs()
    {
        Debug.Log("RecipeBlock: GetInputs");
        List<Node> list = new List<Node>();
        for (int i = 0; i < inputs.transform.childCount; i++)
        {
            list.Add(inputs.transform.GetChild(i).GetComponent<Node>());
        }
        return list;
    }
    public List<Node> GetProducts()
    {
        Debug.Log("RecipeBlock: GetProducts");
        List<Node> list = new List<Node>();
        for (int i = 0; i < products.transform.childCount; i++)
        {
            list.Add(products.transform.GetChild(i).GetComponent<Node>());
        }
        return list;
    }
    public List<Node> GetNodes()
    {
        Debug.Log("RecipeBlock: GetNodes");
        List<Node> list = new List<Node>();
        for (int i = 0; i < inputs.transform.childCount; i++)
        {
            list.Add(inputs.transform.GetChild(i).GetComponent<Node>());
        }
        for (int i = 0; i < products.transform.childCount; i++)
        {
            list.Add(products.transform.GetChild(i).GetComponent<Node>());
        }
        return list;
    }
    public void OnDestroy()
    {
        Debug.Log("RecipeBlock: OnDestroy");
        List<Node> nodes = GetNodes();
        for (int i = 0; i < nodes.Count; i++)
        {
            Destroy(nodes[i].gameObject);
        }
    }
}
