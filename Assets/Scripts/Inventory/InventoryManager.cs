using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] bool DisableChange = true;

        [Range(0,1)]
        [SerializeField] float IconSize = 1;
        [SerializeField] GameObject Slot;
        [SerializeField] int SlotWidth, SlotHeight;
        [SerializeField] RectTransform background;
        [SerializeField] float Size = 1;

        [SerializeField] Color CommonColor;
        [SerializeField] Color UncommonColor;
        [SerializeField] Color RareColor;
        [SerializeField] Color LegendaryColor;

        [SerializeField] ItemInventory inventory;
        [SerializeField] List<RectTransform> slots;

        [SerializeField] EquipmentSlot Head;
        [SerializeField] EquipmentSlot Body;
        [SerializeField] EquipmentSlot Legs;
        [SerializeField] EquipmentSlot Weapon;

        ItemCache itemCache;
        GameObject Icon;

        int CachedAmount = 1;
        Item cachedItem = null;

        private void Awake()
        {
            itemCache = GetComponent<ItemCache>(); 
        }

        public void WeaponUse(string UUID)
        {
            inventory.EquipWeapon(UUID);
        }

        public void AnmorEquip(string UUID)
        {
            inventory.EquipArmor(UUID);
        }

        public Color GetColor(Item.Rarity rarity)
        {
            switch (rarity)
            {
                case Item.Rarity.Common:
                    return CommonColor;
                case Item.Rarity.Uncommon:
                    return UncommonColor;
                case Item.Rarity.Rare:
                    return RareColor;
                case Item.Rarity.Legendary:
                    return LegendaryColor;
            }

            return CommonColor;
        }


        public Item GetItem(string UUID)
        {
            return itemCache.GetItem(UUID);
        }

        public Item GetCachedItem() { return cachedItem; }
        public int GetCachedAmount() { return CachedAmount; }

        public void UpdateUi(Dictionary<Item, InventoryItemSlot> playerInventory)
        {
            int Count = 0;

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].GetComponent<ItemSlot>()?.UpdateUI(i);
            }

            foreach (KeyValuePair<Item, InventoryItemSlot> item in playerInventory)
            {
                item.Value.Slot = 0;
                for (int i = 0; i < item.Value.SlotCount; i++)
                {
                    item.Value.Slot++;
                    ItemSlot current = slots[Count].GetComponent<ItemSlot>();
                    current.UpdateUI(item, Count);
                    Count++;
                }
            }

        }

        public void ClickItem(string UUID, bool isInventorySlot = true, int Count = 1)
        {
            if (cachedItem != null)
            {
                if (isInventorySlot) inventory.AddItem(cachedItem, CachedAmount);
                Destroy(Icon.gameObject);
                cachedItem = null;
            }

            if (string.IsNullOrWhiteSpace(UUID)) return;

            cachedItem = GetItem(UUID);
            CachedAmount = Count;
            Icon = new GameObject();
            Icon.transform.parent = transform.parent;
            Icon.name = "MouseButton";
            Image sprite = Icon.AddComponent<Image>();
            sprite.raycastTarget = false;
            sprite.sprite = cachedItem.GetTexture();
            Icon.transform.position = Input.mousePosition;
            Icon.transform.localScale = Vector3.one * IconSize;
            if(isInventorySlot) inventory.RemoveItem(cachedItem);
        }

        public void EquipItem(Equipment.EquipmentSlot slot)
        {
            if (cachedItem == null) return;
            Equipment equipment = (Equipment)cachedItem;
            if (equipment == null) return;
            if (equipment.GetEquipmentSlot() != slot) return;

            inventory.EquipItem(equipment);
            switch (slot)
            {
                case Equipment.EquipmentSlot.Head:
                    if (!string.IsNullOrWhiteSpace(Head.GetItemUUID()))
                    {
                        inventory.AddItem(itemCache.GetItem(Head.GetItemUUID()));
                    }
                    Head.UpdateUI(equipment);
                    break;
                case Equipment.EquipmentSlot.Torso:
                    if (!string.IsNullOrWhiteSpace(Body.GetItemUUID()))
                    {
                        inventory.AddItem(itemCache.GetItem(Body.GetItemUUID()));
                    }
                    Body.UpdateUI(equipment);
                    break;
                case Equipment.EquipmentSlot.Legs:
                    if (!string.IsNullOrWhiteSpace(Legs.GetItemUUID()))
                    {
                        inventory.AddItem(itemCache.GetItem(Legs.GetItemUUID()));
                    }
                    Legs.UpdateUI(equipment);
                    break;
                case Equipment.EquipmentSlot.Weapon:
                    if (!string.IsNullOrWhiteSpace(Weapon.GetItemUUID()))
                    {
                        inventory.AddItem(itemCache.GetItem(Weapon.GetItemUUID()));
                    }
                    Weapon.UpdateUI(equipment);
                    break;
                default:
                    break;
            }

            cachedItem = null;
            Destroy(Icon);
        }

        private void Update()
        {
            if (cachedItem == null) return;
            if(Icon != null) Icon.transform.position = Input.mousePosition;
        }

        public void UnequipItem(EquipmentSlot equipment, string DefaultSet)
        {
            Item cachedItemStore = cachedItem;
            if (cachedItem == null)
            {
                ClickItem(equipment.GetItemUUID(), false);
                inventory.EquipItem(DefaultSet);
                equipment.UpdateUI(null);
                return;
            }
            ClickItem(equipment.GetItemUUID(), false);
            inventory.EquipItem(cachedItemStore.GetUUID());
            equipment.UpdateUI(cachedItemStore);
        }

        public void EquipItem(string UUID)
        {
            Equipment equipment = itemCache.GetEquipment(UUID);
            if (equipment == null) return;

            Equipment.EquipmentSlot slot = equipment.GetEquipmentSlot();
            inventory.EquipItem(equipment);
            inventory.RemoveItem(equipment);

            switch (slot)
            {
                case Equipment.EquipmentSlot.Head:
                    if(!string.IsNullOrWhiteSpace(Head.GetItemUUID()))
                    {
                        inventory.AddItem(itemCache.GetItem(Head.GetItemUUID()));
                    }
                    Head.UpdateUI(equipment);
                    break;
                case Equipment.EquipmentSlot.Torso:
                    if (!string.IsNullOrWhiteSpace(Body.GetItemUUID()))
                    {
                        inventory.AddItem(itemCache.GetItem(Body.GetItemUUID()));
                    }
                    Body.UpdateUI(equipment);
                    break;
                case Equipment.EquipmentSlot.Legs:
                    if (!string.IsNullOrWhiteSpace(Legs.GetItemUUID()))
                    {
                        inventory.AddItem(itemCache.GetItem(Legs.GetItemUUID()));
                    }
                    Legs.UpdateUI(equipment);
                    break;
                case Equipment.EquipmentSlot.Weapon:
                    if (!string.IsNullOrWhiteSpace(Weapon.GetItemUUID()))
                    {
                        inventory.AddItem(itemCache.GetItem(Weapon.GetItemUUID()));
                    }
                    Weapon.UpdateUI(equipment);
                    break;
                default:
                    break;
            }
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (DisableChange) return;

            float IncAmtWidth = background.rect.width/SlotWidth;
            float IncAmtHeight = background.rect.height/SlotHeight;
            float OffsetX = (background.rect.width - IncAmtWidth * (SlotWidth - 1) / 2) - (background.rect.width / 2);
            float OffsetY = (background.rect.height - IncAmtHeight * (SlotHeight - 1) / 2) - (background.rect.height / 2);

            if (slots!=null && slots.Count != SlotWidth * SlotHeight)
            {
                foreach (RectTransform slot in slots)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(slot.gameObject);
                    };
                }
                slots.Clear();
            }

            if(slots==null || slots.Count == 0)
            {
                for (int i = 0; i < SlotHeight; i++)
                {
                    for (int j = 0; j < SlotWidth; j++)
                    {
                        Vector2 SlotPos = new Vector2(IncAmtWidth * j - background.rect.width / 2 + OffsetX, -IncAmtHeight * i + background.rect.height / 2 -OffsetY);
                        RectTransform rectT = Instantiate(Slot, transform).GetComponent<RectTransform>();
                        rectT.localScale = new Vector3(Size, Size, Size);
                        rectT.anchoredPosition = SlotPos;
                        rectT.GetComponent<ItemSlot>().Pos = j + (SlotWidth * i);
                        slots.Add(rectT);
                    }
                }
            }
            else
            {
                for (int i = 0; i < SlotHeight; i++)
                {
                    for (int j = 0; j < SlotWidth; j++)
                    {
                        Vector2 SlotPos = new Vector2(IncAmtWidth * j - background.rect.width / 2 + OffsetX, -IncAmtHeight * i + background.rect.height / 2 - OffsetY);
                        slots[j + (SlotWidth * i)].anchoredPosition = SlotPos;
                        slots[j + (SlotWidth * i)].localScale = new Vector3(Size, Size, Size);
                    }
                }
            }

        }
#endif
    }
}
