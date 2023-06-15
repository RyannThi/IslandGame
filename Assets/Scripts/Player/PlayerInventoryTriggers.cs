using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInventoryTriggers : EventTrigger, IPointerClickHandler
{
    private PlayerInventory playerInventory;
    private void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
    }
    public void ChangeSelectionIndex()
    {
        if (playerInventory.isOpen == true)
        {
            playerInventory.audioSource.PlayOneShot(playerInventory.buttonMove);

            for (int i = 0; i <= 11; i++)
            {
                if (gameObject.name == "InvSlot_" + i)
                {
                    playerInventory.ChangeSlotHighlight(i);
                    playerInventory.selectedItem = i;
                }
            }
        }
    }
    public void ConfirmSelection()
    {
        if (playerInventory.isOpen == true)
        {
            playerInventory.hasClicked = true;
        }
    }
}
