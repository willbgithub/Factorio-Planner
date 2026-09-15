// ItemValue.cs
// A Factorio item with an associated value.
// 2 September 2026
// will b. gaming

using System;
using UnityEngine;
[Serializable]
public class ItemValue
{
    // Constructor
    public ItemValue(Item item, Fraction value)
    {
        this.item = Item.CreateItem(item);
        this.value = value;
    }
    public ItemValue(ItemValue itemValue)
    {
        item = Item.CreateItem(itemValue.item);
        value = itemValue.value;
    }
    // Mutators
    public void SetItem(Item item)
    {
        this.item = Item.CreateItem(item);
    }
    public void SetValue(Fraction value)
    {
        this.value = value;
    }
    // Accessors
    public Item GetItem()
    {
        return Item.CreateItem(item);
    }
    public Fraction GetValue()
    {
        return value;
    }
    // Utility
    public ItemValue Multiply(Fraction factor)
    {
        ItemValue returnValue = new ItemValue(this);
        returnValue.value *= factor;
        return returnValue; 
    }
    // Member data
    Item item;
    Fraction value;
}
