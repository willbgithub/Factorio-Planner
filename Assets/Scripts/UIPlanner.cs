using UnityEngine;

public class UIPlanner : MonoBehaviour
{
    public GameObject canvas;
    public GameObject recipeBlock;
    void Start()
    {
        GameObject initialBlock = Instantiate(recipeBlock, canvas.transform);
    }
}
