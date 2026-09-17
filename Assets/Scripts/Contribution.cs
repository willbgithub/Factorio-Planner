// Contribution.cs
// A list of ItemRates that can be added to.
// 2 September 2026
// will b. gaming
using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
[Serializable]
public class Contribution
{
    // Constructor
    public Contribution()
    {
        Count = 0;
        itemRates = new List<ItemRate>();
    }
    public Contribution(List<ItemRate> itemRates)
    {
        itemRates = new List<ItemRate>();
        for (int i = 0; i < itemRates.Count; i++)
        {
            this.itemRates.Add(itemRates[i]);
        }
        Count = itemRates.Count;
    }
    public Contribution(Contribution contribution)
    {
        itemRates = new List<ItemRate>();
        for (int i = 0; i < contribution.itemRates.Count; i++)
        {
            itemRates.Add(contribution.itemRates[i]);
        }
        Count = itemRates.Count;
    }
    public Contribution(Contribution contribution, Fraction factor)
    {
        itemRates = new List<ItemRate>();
        for (int i = 0; i < contribution.itemRates.Count; i++)
        {
            itemRates.Add(itemRates[i].Multiply(factor));
        }
        Count = itemRates.Count;
    }
    public Contribution(Recipe recipe, Fraction factor)
    {
        Contribution contribution = recipe.GetContribution();
        itemRates = new List<ItemRate>();
        for (int i = 0; i < contribution.itemRates.Count; i++)
        {
            itemRates.Add(contribution.Index(i).Multiply(factor));
        }
        Count = itemRates.Count;
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
                itemRates[i].Add(new ItemRate(itemRate));
                found = true;
            }
        }
        else
        {
            itemRates.Add(new ItemRate(itemRate));
            Count++;
        }
    }
    public void Add(Contribution contribution)
    {
        for (int i = 0; i < contribution.Count; i++)
        {
            Add(contribution.Index(i));
        }
    }
    // Accessors
    public List<ItemRate> GetItemRates()
    {
        List<ItemRate> returnList = new List<ItemRate>();
        for (int i = 0; i < itemRates.Count; i++)
        {
            returnList.Add(itemRates[i]);
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
    public ItemRate Index(int index)
    {
        return itemRates[index];
    }

    public List<ItemValue> GetInputs()
    {
        List<ItemValue> inputs = new List<ItemValue>();
        for (int i = 0; i < Count; i++)
        {
            inputs.Add(itemRates[i].GetInput());
        }
        return inputs;
    }
    public List<ItemValue> GetIntermediates()
    {
        List<ItemValue> intermediates = new List<ItemValue>();
        for (int i = 0; i < Count; i++)
        {
            intermediates.Add(itemRates[i].GetIntermediate());
        }
        return intermediates;
    }
    public List<ItemValue> GetProducts()
    {
        List<ItemValue> products = new List<ItemValue>();
        for (int i = 0; i < Count; i++)
        {
            products.Add(itemRates[i].GetProduct());
        }
        return products;
    }
    // Member data
    [SerializeField] List<ItemRate> itemRates;
    [ReadOnly] public int Count;
}
