namespace RPG.Inventory
{
    [System.Serializable]
    public class InventoryItemSlot
    {
        public int itemCount;
        public int SlotCount;
        public int Slot;

        public InventoryItemSlot(int itemCount = 0, int SlotCount = 0, int Slot = 0)
        {
            this.itemCount = itemCount;
            this.SlotCount = SlotCount;
            this.Slot = Slot;
        }
    }
}