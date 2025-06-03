using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OnInventoryTabClicked : MonoBehaviour
{
    private PlayerInventory pInventory;
    public int tabIndex;
    private float transparentAlpha = 0.3f;

    [SerializeField] private ItemTypeUI itemTypeUI;

    void Awake()
    {
        pInventory = transform.parent.transform.parent.gameObject.GetComponent<PlayerInventory>();

        if (tabIndex != 1)
        {
            TabIsSelected(false);
        }
    }

    public void InvTabClicked()
    {
        pInventory.InventoryTab(itemTypeUI);
        pInventory.tabIndex = tabIndex;
    }

    public void TabIsSelected(bool s)
    {
        if (s)
        {
            // No transparency
            SetAlpha(1);
        }
        else
        {
            // Transparency
            SetAlpha(transparentAlpha);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color tmp = GetComponent<Image>().color;
        tmp.a = alpha;
        GetComponent<Image>().color = tmp;
    }
}
