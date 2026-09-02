// ItemRate.cs
// A Factorio item and its consumption/production rates.
// 2 September 2026
// will b. gaming

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class ItemRate
{
    // Constructor
    public ItemRate(Item item, Fraction consumption, Fraction production)
    {
        this.item = item;
        this.consumption = consumption;
        this.production = production;
    }
    public ItemRate(ItemRate itemRate)
    {
        item = itemRate.item;
        consumption = itemRate.consumption;
        production = itemRate.production;
    }
    // Mutators
    public void SetItem(Item item)
    {
        this.item = item;
    }
    public void SetConsumption(Fraction consumption)
    {
        this.consumption = consumption;
    }
    public void SetProduction(Fraction production)
    {
        this.production = production;
    }
    // Accessors
    public Item GetItem()
    {
        return item;
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
    // Member data
    Item item;
    Fraction consumption;
    Fraction production;
}
