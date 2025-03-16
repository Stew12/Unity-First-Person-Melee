using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OnInventoryTabClicked : MonoBehaviour
{
    private PlayerInventory pInventory;

    [SerializeField] private ItemTypeUI itemTypeUI;

    void Awake()
    {
        pInventory = transform.parent.transform.parent.gameObject.GetComponent<PlayerInventory>();
    }

    public void InvTabClicked()
    {
        pInventory.InventoryTab(itemTypeUI);
    }
}
