using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Combat;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Inventory
{
    [System.Serializable]
    struct SaveableInventroy
    {
        public Dictionary<string, InventoryItemSlot> inventory { get; }
        public string Headpiece { get; }
        public string Torso { get; }
        public string Leggings { get; }
        public string Weapon { get; }

        public SaveableInventroy(Dictionary<string, InventoryItemSlot> inventory, string Headpiece = "", string Torso = "", string Leggings = "", string Weapon = "")
        {
            this.inventory = inventory;
            this.Headpiece = Headpiece;
            this.Torso = Torso;
            this.Leggings = Leggings;
            this.Weapon = Weapon;
        }
    }

    public class ItemInventory : MonoBehaviour, ISaveable
    {
        [SerializeField] int inventorySize;
        Dictionary<Item, InventoryItemSlot> inventory = new Dictionary<Item, InventoryItemSlot>();
        ItemCache itemCache;
        InventoryManager inventoryManager;
        Equipment Headpiece;
        Equipment Torso;
        Equipment Leggings;
        Equipment Weapon;

        public IReadOnlyDictionary<Item, InventoryItemSlot> GetInventory()
        {
            return inventory;
        } 

        private void Awake()
        {
            itemCache = GetComponent<ItemCache>();
            inventoryManager = FindObjectOfType<InventoryManager>();
        }

        public int GetFreeSlots()
        {
            int SlotsAvailable = inventorySize;

            foreach (InventoryItemSlot slot in inventory.Values)
            {
                SlotsAvailable -= slot.SlotCount;
            }

            return SlotsAvailable;
        }

        bool SpaceAvailable(int itemCount, int quantity, int StackSize, ref int SlotCountTaken, int SlotsAvailable )
        {
            if ((itemCount + quantity) > (StackSize * SlotCountTaken))
            {
                //print((itemCount + quantity) + " is larger than " + (StackSize * SlotCountTaken));
                if (SlotsAvailable <= 0) return false;
                SlotCountTaken++;
                SlotsAvailable--;

                if ((itemCount + quantity) > (StackSize * SlotCountTaken))
                {
                    return SpaceAvailable(itemCount, quantity, StackSize, ref SlotCountTaken, SlotsAvailable);
                }
            }

            return true;
        }
        public bool HasSpaceFor(IEnumerable<Item> items)
        {
            Dictionary<Item, int> transaction = new Dictionary<Item, int>();
            foreach (Item item in items)
            {
                if (!transaction.ContainsKey(item))
                {
                    transaction[item] = 0;
                }
                transaction[item]++;
            }
            int ItemCount = 0;
            foreach (var item in transaction)
            {
                int ItemsAmt = 0;
                if (!CanAddItem(item.Key, item.Value, out ItemsAmt))
                {
                    return false;
                }
                if (GetInventory().ContainsKey(item.Key)) ItemsAmt -= GetInventory()[item.Key].SlotCount;
                ItemCount += ItemsAmt;
            }

            if (ItemCount > GetFreeSlots()) return false;

            return true;
        }


        public bool CanAddItem(Item item, int quantity, out int slotCountAmt)
        {
            slotCountAmt = 0;
            int freeSlots = GetFreeSlots();
            Dictionary<Item, InventoryItemSlot> inventroyCopy = new Dictionary<Item, InventoryItemSlot>(GetInventory());

            if (!inventroyCopy.ContainsKey(item))
            {
                if (freeSlots <= 0) return false;
                inventroyCopy[item] = new InventoryItemSlot();
            }

            int SlotCount = inventroyCopy[item].SlotCount;
            if (!SpaceAvailable(inventroyCopy[item].itemCount, quantity, item.GetStackSize(), ref SlotCount, freeSlots))
            {
                return false;
            }

            slotCountAmt = SlotCount;
            return true;
        }

        public bool CanAddItemRef(Item item, int quantity = 1)
        {
            int SlotsAvailable = GetFreeSlots();

            if (!inventory.ContainsKey(item))
            {
                if (SlotsAvailable <= 0) return false;
                inventory[item] = new InventoryItemSlot();
            }

            int SlotCount = inventory[item].SlotCount;

            if (!SpaceAvailable(inventory[item].itemCount, quantity, item.GetStackSize(), ref inventory[item].SlotCount, SlotsAvailable))
            {
                inventory[item].SlotCount = SlotCount;
                return false;
            }

            return true;
        }

        public bool AddItem(Item item, int quantity = 1)
        {
            if (!CanAddItemRef(item, quantity)) return false; 

            inventory[item].itemCount += quantity;
            inventoryManager.UpdateUi(inventory);
            return true;
        }

        public bool RemoveItem(Item item, int quantity = 1)
        {
            if (!inventory.ContainsKey(item)) return false;
            if (quantity > inventory[item].itemCount) return false;

            int SlotCount = inventory[item].SlotCount - 1;
            inventory[item].itemCount -= quantity;

            if (inventory[item].itemCount <= item.GetStackSize() * SlotCount) inventory[item].SlotCount--;
            if (inventory[item].itemCount <= 0) inventory.Remove(item);
            inventoryManager.UpdateUi(inventory);
            return true;
        }

        public void EquipItem(string UUID)
        {
            Equipment equipment = (Equipment)itemCache.GetItem(UUID);
            if (equipment == null) return;

            Equipment.EquipmentSlot slot = equipment.GetEquipmentSlot();
            switch (slot)
            {
                case Equipment.EquipmentSlot.Head:
                    Headpiece = equipment;
                    EquipArmor(UUID);
                    break;
                case Equipment.EquipmentSlot.Torso:
                    Torso = equipment;
                    EquipArmor(UUID);
                    break;
                case Equipment.EquipmentSlot.Legs:
                    Leggings = equipment;
                    EquipArmor(UUID);
                    break;
                case Equipment.EquipmentSlot.Weapon:
                    Weapon = equipment;
                    EquipWeapon(UUID);
                    break;
                default:
                    break;
            }

            inventoryManager.UpdateUi(inventory);
        }

        public void EquipItem(Equipment equipment)
        {
            Equipment.EquipmentSlot slot = equipment.GetEquipmentSlot();
            switch (slot)
            {
                case Equipment.EquipmentSlot.Head:
                    Headpiece = equipment;
                    EquipArmor(equipment.GetUUID());
                    break;
                case Equipment.EquipmentSlot.Torso:
                    Torso = equipment;
                    EquipArmor(equipment.GetUUID());
                    break;
                case Equipment.EquipmentSlot.Legs:
                    Leggings = equipment;
                    EquipArmor(equipment.GetUUID());
                    break;
                case Equipment.EquipmentSlot.Weapon:
                    Weapon = equipment;
                    EquipWeapon(equipment.GetUUID());
                    break;
                default:
                    break;
            }

            inventoryManager.UpdateUi(inventory);
        }

        public void EquipArmor(string UUID)
        {
            GetComponent<PlayerModel>()?.LoadArmor(UUID);
            GetComponent<Health>()?.EquipArmor(UUID);
        }

        public void EquipWeapon(string UUID)
        {
            GetComponent<Fighter>()?.EquipWeapon(UUID);
        }

        public int GetItemCount(Item item)
        {
            if (!inventory.ContainsKey(item)) return 0;

            return inventory[item].itemCount;
        }

        public object CaptureState()
        {
            Dictionary<string, InventoryItemSlot> inventorySave = new Dictionary<string, InventoryItemSlot>();
            foreach (KeyValuePair<Item, InventoryItemSlot> item in inventory)
            {
                inventorySave[item.Key.GetUUID()] = item.Value;
                print("e");
            }
            SaveableInventroy save = new SaveableInventroy(inventorySave, Headpiece?.GetUUID(), Torso?.GetUUID(), Leggings?.GetUUID(), Weapon?.GetUUID());

            return save;
        }

        public void RestoreState(object state)
        {
            SaveableInventroy inventroyTemp = (SaveableInventroy)state;

            Dictionary<Item, InventoryItemSlot> inventoryLoad = new Dictionary<Item, InventoryItemSlot>();
            foreach (KeyValuePair<string, InventoryItemSlot> item in inventroyTemp.inventory)
            {
                Item ReplacementItem = itemCache.GetItem(item.Key);
                inventoryLoad[ReplacementItem] = item.Value;
            }

            inventory = inventoryLoad;
            if (!string.IsNullOrEmpty(inventroyTemp.Headpiece))
            {
                Headpiece = itemCache.GetEquipment(inventroyTemp.Headpiece);
                inventoryManager.EquipItem(Headpiece.GetUUID());
            }
            if (!string.IsNullOrEmpty(inventroyTemp.Torso))
            {
                Torso = itemCache.GetEquipment(inventroyTemp.Torso);
                inventoryManager.EquipItem(Torso.GetUUID());
            }
            if (!string.IsNullOrEmpty(inventroyTemp.Leggings))
            {
                Leggings = itemCache.GetEquipment(inventroyTemp.Leggings);
                inventoryManager.EquipItem(Leggings.GetUUID());
            }
            if (!string.IsNullOrEmpty(inventroyTemp.Weapon))
            {
                Weapon = itemCache.GetEquipment(inventroyTemp.Weapon);
                inventoryManager.EquipItem(Weapon.GetUUID());
            }

            inventoryManager.UpdateUi(inventory);
        }
    }
}