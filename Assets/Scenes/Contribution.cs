// Contribution.cs
// A list of ItemRates that can be added to.
// 2 September 2026
// will b. gaming
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Contribution
{
    // Constructor
    public Contribution(List<ItemRate> itemRates)
    {
        itemRates = new List<ItemRate>();
        for (int i = 0; i < itemRates.Count; i++)
        {
            this.itemRates[i] = new ItemRate(itemRates[i]);
        }
    }
    public Contribution(Contribution contribution)
    {
        itemRates = new List<ItemRate>();
        for (int i = 0; i < contribution.itemRates.Count; i++)
        {
            itemRates[i] = new ItemRate(contribution.itemRates[i]);
        }
    }
    // Accessors
    public List<ItemRate> GetItemRates()
    {
        List<ItemRate> returnList = new List<ItemRate>();
        for (int i = 0; i < itemRates.Count; i++)
        {
            returnList[i] = new ItemRate(itemRates[i]);
        }
        return returnList;
    }
    // Member data
    List<ItemRate> itemRates;
}
