// ItemRate.cs
// A Factorio item and its consumption/production rates.
// 2 September 2026
// will b. gaming

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[Serializable]
public class ItemRate
{
    // Constructor
    public ItemRate(Item item, Fraction consumption, Fraction production)
    {
        this.item = Item.CreateItem(item);
        this.consumption = consumption;
        this.production = production;
    }
    public ItemRate(ItemRate itemRate)
    {
        Debug.Log("type of item: " + itemRate.GetItem().GetType());
        item = Item.CreateItem(itemRate.item);
        Debug.Log("type of item: " + item.GetType());
        consumption = itemRate.consumption;
        production = itemRate.production;
    }
    // Mutators
    public void Add(ItemRate itemRate)
    {
        consumption += itemRate.consumption;
        production += itemRate.production;
    }
    public void SetItem(Item item)
    {
        this.item = Item.CreateItem(item);
    }
    public void SetConsumption(Fraction consumption)
    {
        this.consumption = consumption;
    }
    public void AddConsumption(Fraction consumption)
    {
        this.consumption += consumption;
    }
    public void SetProduction(Fraction production)
    {
        this.production = production;
    }
    public void AddProduction(Fraction production)
    {
        this.production += production;
    }
    // Accessors
    public Item GetItem()
    {
        return Item.CreateItem(item);
    }
    public Fraction GetConsumption()
    {
        return consumption;
    }
    public Fraction GetProduction()
    {
        return production;
    }
    // Utility
    public ItemRate Multiply(Fraction factor)
    {
        ItemRate returnValue = new ItemRate(this);
        returnValue.consumption *= factor;
        returnValue.production *= factor;
        return returnValue;
    }
    public override string ToString()
    {
        return item + ": -" + consumption.ToString() + ", +" + production;
    }
    // Member data
    [SerializeField] Item item;
    [SerializeField] Fraction consumption;
    [SerializeField] Fraction production;
}
