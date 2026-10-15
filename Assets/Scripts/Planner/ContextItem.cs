using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextItem : MonoBehaviour
{
    UIPlanner parent;
    [SerializeField] Image itemIcon;
    [SerializeField] Image background;
    [SerializeField] TMP_Text itemLabel;
    [SerializeField] Color defaultColor;
    [SerializeField] Color focusedColor;

    public void SetItem(Sprite icon, string name)
    {
        itemIcon.sprite = icon;
        itemLabel.text = name;
    }
    public void SetParent(UIPlanner parent)
    {
        this.parent = parent;
    }
    // EVENT SYSTEMS
    public void SetFocused(bool focused)
    {
        if (focused)
        {
            background.color = focusedColor;
        }
        else
        {
            background.color = defaultColor;
        }
    }

    public void OnPointerEnter(BaseEventData d)
    {
        SetFocused(true);
    }
    public void OnPointerExit(BaseEventData d)
    {
        SetFocused(false);
    }
}
