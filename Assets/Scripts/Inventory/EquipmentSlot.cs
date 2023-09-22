using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Saving; 

namespace RPG.Inventory{
    public class EquipmentSlot : MonoBehaviour
    {
        [SerializeField] Image icon, background;
        [SerializeField] Sprite defautBackground;
        [SerializeField] Item item;
        [SerializeField] Equipment.EquipmentSlot slot;
        [SerializeField] string DefaultSet;
        bool reloaded = false;
        InventoryManager inventory;
        [SerializeField] Color defaultColor;

        private void Awake()
        {
            inventory = FindObjectOfType<InventoryManager>();
        }

        public void LoadItem(Color color, Item ItemToLoad)
        {
            item = ItemToLoad;
            icon.sprite = ItemToLoad.GetSprite();
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
            background.color = color;
        }

        public void UnequipItem()
        {
            if(item != null)
            {
                inventory.UnequipItem(this, DefaultSet);
                return;
            }

            inventory.EquipItem(slot);
        }

        public void UpdateUI(Item item)
        {
            if (item == null)
            {
                this.item = null;
                icon.sprite = null;
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
                background.color = defaultColor;
                return;
            }

            LoadItem(inventory.GetColor(item.GetRarity()), item);
        }

        public string GetItemUUID()
        {
            if (item == null) return null;
            return item.GetUUID();
        }

    }
}