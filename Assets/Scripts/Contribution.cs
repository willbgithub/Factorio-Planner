// Contribution.cs
// A list of ItemRates that can be added to.
// 2 September 2026
// will b. gaming
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class Contribution
{
    // Constructor
    public Contribution()
    {
        itemRates = new List<ItemRate>();
    }

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
    // Mutators
    public void Add(ItemRate itemRate)
    {
        if (Contains(itemRate.GetItem()))
        {
            bool found = false;
            int i = 0;
            while (!found)
            {
                if (itemRates[i].GetItem().GetPrefabName() != itemRate.GetItem().GetPrefabName())
                {
                    i++;
                    continue;
                }
                itemRates[i].Add(itemRate);
                found = true;
            }
        }
        else
        {
            itemRates.Add(itemRate);
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
    public bool Contains(Item item)
    {
        for (int i = 0; i < itemRates.Count; i++)
        {
            if (itemRates[i].GetItem().GetPrefabName() == item.GetPrefabName())
            {
                return true;
            }
        }
        return false;
    }
    // Member data
    List<ItemRate> itemRates;
}
