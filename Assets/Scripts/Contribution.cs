// Contribution.cs
// A list of ItemRates that can be added to.
// 2 September 2026
// will b. gaming
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
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
        Debug.Log("type of item: " + contribution.GetItemRates()[0].GetItem().GetType());
        itemRates = new List<ItemRate>();
        for (int i = 0; i < contribution.itemRates.Count; i++)
        {
            itemRates.Add(new ItemRate(contribution.itemRates[i]));
        }
        Debug.Log("type of item: " + itemRates[0].GetItem().GetType());
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
            returnList.Add(new ItemRate(itemRates[i]));
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
    public Fraction GetRate(Item item)
    {
        for (int i = 0; i < itemRates.Count; i++)
        {
            if (itemRates[i].GetItem().GetPrefabName() == item.GetPrefabName())
            {
                return itemRates[i].GetProduction() - itemRates[i].GetConsumption();
            }
        }
        return 0;
    }
    public Fraction GetRate(string prefabName)
    {
        for (int i = 0; i < itemRates.Count; i++)
        {
            if (itemRates[i].GetItem().GetPrefabName() == prefabName)
            {
                return itemRates[i].GetProduction() - itemRates[i].GetConsumption();
            }
        }
        return 0;
    }
    public override string ToString()
    {
        if (itemRates.Count == 0)
        {
            return "{empty contribution}";
        }
        string str = "";
        for (int i = 0; i < itemRates.Count; i++)
        {
            str += itemRates[i] + "\n";
        }
        return str;
    }
    // Member data
    [SerializeField] List<ItemRate> itemRates;
}
