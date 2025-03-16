using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemTypeUI
{
    WEAPON,
    ARMOUR,
    SPELL,
    CONSUMABLE,
    KEYITEM,
    ATTACKITEM,
}

public class OnInventoryIconClicked : MonoBehaviour
{
    private PlayerInventory pInventory;
    [SerializeField] private GameObject ITEM;
    public ItemTypeUI itemTypeUI;

    private Image iconImg;

    [SerializeField] private bool DEBUG = false;

    void Awake()
    {
        pInventory = transform.parent.transform.parent.gameObject.GetComponent<PlayerInventory>();

        iconImg = GetComponent<Image>();

        iconImg.color = SetIconColour();

        if (ITEM == null && !DEBUG)
        {
            Debug.LogError(gameObject.name + " does not have a gameObject selected");
        }
    }

    public void InvIconClicked()
    {
        pInventory.ItemIsSelected(ITEM, this.gameObject);
        
    }

    public void Selected(bool selected)
    {
        if (selected)
        {
            iconImg.color = Color.white;
        }
        else
        {
            iconImg.color = SetIconColour();
        }
    }

    public Color SetIconColour()
    {
        // ICON COLOURS SUBJECT TO CHANGE
        switch (itemTypeUI)
        {
            case ItemTypeUI.WEAPON:
                return Color.red;

            case ItemTypeUI.ARMOUR:
                return Color.blue;

            case ItemTypeUI.SPELL:
                //Orange
                return new Color(1.0f, 0.64f, 0.0f);

            case ItemTypeUI.CONSUMABLE:
                return Color.green;

            case ItemTypeUI.KEYITEM:
                return Color.magenta;

            case ItemTypeUI.ATTACKITEM:
                return Color.yellow;

            default:
                return Color.white;
        }
        
    }

}
