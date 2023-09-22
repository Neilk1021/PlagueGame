using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using RPG.Saving;
using TMPro;

namespace RPG.Inventory
{
    [System.Serializable]
    public struct SaveItem
    {
        public string UUID { get; }
        public int ItemCount { get; } 

        public SaveItem(string uuid, int itemCount)
        {
            UUID = uuid;
            ItemCount = itemCount;
        }
    }

    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] InventoryManager inventoryManager;
        [SerializeField] Color defaultColor;
        [SerializeField] Item item;
        public int Pos;
        //[SerializeField] int ItemCount;
        [SerializeField] Image icon, background;
        [SerializeField] Sprite defautBackground;

        [Header("Desc")]
        [SerializeField] TextMeshProUGUI Name;
        [SerializeField] TextMeshProUGUI Description, Value;
        [SerializeField] GameObject DescBox;
        [SerializeField] TextMeshProUGUI CountText;
        [SerializeField] GameObject CountBox; 

        private void Awake()
        {
            inventoryManager = GetComponentInParent<InventoryManager>();
        }

        private void Start()
        {
            if (Empty())
            {
                CountBox.SetActive(false);
                icon.sprite = null;
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
                background.color = defaultColor;
            }
        }

        public void LoadItem(Color color, Item ItemToLoad)
        {
            item = ItemToLoad;
            icon.sprite = ItemToLoad.GetSprite();
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
            background.color = color;
        }

        public int GetPos()
        {
            return Pos; 
        }

        public string GetUUID()
        {
            return item.GetUUID();
        }

        public void UpdateUI(int Count)
        {
            CountBox.SetActive(false);
                item = null;
                Pos = Count;
                icon.sprite = null;
                //ItemCount = 0;
                icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
                background.color = defaultColor;
                return;
        }

        public void UpdateUI(KeyValuePair<Item, InventoryItemSlot> item, int Count)
        {
            CountBox.SetActive(true);

            int ItemCount = 0;

            if(item.Value.Slot < item.Value.SlotCount)
            {
                ItemCount = item.Key.GetStackSize();
            }
            else
            {
                ItemCount = item.Value.itemCount % item.Key.GetStackSize();
                if(ItemCount == 0) ItemCount = item.Key.GetStackSize();
            }

            CountText.text = ItemCount.ToString();
            InventoryManager parent = GetComponentInParent<InventoryManager>();
            LoadItem(parent.GetColor(item.Key.GetRarity()), item.Key);
        }

        //Implement Method to send amount
        public void Click()
        {
            string UUID;
            if (item != null) UUID = item.GetUUID(); else UUID = "";

            inventoryManager.ClickItem(UUID);
        }

        public void loadItemDesc()
        {
            //" + item.GetRarity().ToString() + "
            if (item == null) return;
            DescBox.SetActive(true);
            Name.text = "<i>" + item.GetName() + " " + "<color=#"+ ColorUtility.ToHtmlStringRGBA(background.color) + ">"+ "- "+ item.GetRarity().ToString() + "</color></i>";
            Description.text = item.GetDesc();
            Value.text = "Value: " + item.GetValue().ToString();
        }

        public void unloadItemDesc()
        {
            DescBox.SetActive(false);
        }

        public bool Empty()
        {
            return (item == null) ? true : false;
        }
    }
}
