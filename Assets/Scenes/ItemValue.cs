// ItemValue.cs
// A Factorio item with an associated value.
// 2 September 2026
// will b. gaming

using UnityEngine;

public class ItemValue
{
    // Constructor
    public ItemValue(Item item, Fraction value)
    {
        this.item = item;
        this.value = value;
    }
    public ItemValue(ItemValue itemValue)
    {
        item = itemValue.item;
        value = itemValue.value;
    }
    // Mutators
    public void SetItem(Item item)
    {
        this.item = item;
    }
    public void SetValue(Fraction value)
    {
        this.value = value;
    }
    // Accessors
    public Item GetItem()
    {
        return item;
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
