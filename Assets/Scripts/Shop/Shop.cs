using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventory;
using System;
using RPG.Control;
using RPG.Stats;
using RPG.Saving;

namespace RPG.Shops
{
    public partial class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] string Name;
        [Range(0,1)]
        [SerializeField] float sellingDiscount = 0.8f;

        public event Action onChange;
        ItemCache itemCache;
        [SerializeField] StockItemConfig[] stockConfig;
        [System.Serializable]
        class StockItemConfig
        {
            public Item itemHeld;
            public int initialStock;
            [Range(0,1)]
            public float buyingDiscountPct;
            [Range(1, 50)]
            public int minimumLevel;
        }

        Dictionary<Item, int> transaction = new Dictionary<Item, int>();
        Dictionary<Item, int> stockSold = new Dictionary<Item, int>();

        ItemCatagory itemCatagoryHeld = ItemCatagory.None;
        private Shopper currentShopper;
        bool isBuyingMode = true;

        private void Awake()
        {
            itemCache = GetComponent<ItemCache>();
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (ShopItem item in GetAllItems())
            {
                ItemCatagory itemC = item.getItem().GetItemCatagory();
                if (itemCatagoryHeld == ItemCatagory.None || itemC == itemCatagoryHeld)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            Dictionary<Item, float> prices = GetPrices();
            Dictionary<Item, int> availabilities = GetAvailabilities();

            foreach (Item item in availabilities.Keys)
            {
                if (availabilities[item] <= 0) continue;

                int quantityInTransaction;
                transaction.TryGetValue(item, out quantityInTransaction);
                int availability = availabilities[item];
                yield return new ShopItem(item, availability, prices[item], quantityInTransaction);
            }
        }

        public void SetShopper(Shopper shopper) {
            currentShopper = shopper;
        }

        float getPrice(StockItemConfig config)
        {
            if(isBuyingMode) return config.itemHeld.GetValue() * (1 - config.buyingDiscountPct);
            return config.itemHeld.GetValue() * (sellingDiscount);
        }

        public void SelectFilter(ItemCatagory itemCatagory) { 
            itemCatagoryHeld = (itemCatagory == itemCatagoryHeld) ? ItemCatagory.None : itemCatagory;
            onChange?.Invoke();
        }
        public ItemCatagory GetFilter() { return itemCatagoryHeld; }
        public void SelectMode(bool isBuying)
        {
            isBuyingMode = isBuying;
            transaction = new Dictionary<Item, int>(); 
            onChange?.Invoke();
        }

        public bool IsBuyingMode() { return isBuyingMode; }

        public bool CanTransact()
        {
            if (IsTransactionEmpty()) return false;
            if (IsOverBalance()) return false;
            if (!HasSpaceLeft()) return false;
            return true;
        }

        public bool IsTransactionEmpty()
        {
            return transaction.Count > 0 ? false : true;
        }

        public bool IsOverBalance()
        {
            if (!isBuyingMode) return false;
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperPurse == null) return false;
            return (TotalPrice() > shopperPurse.getBalance());
        }
        public bool HasSpaceLeft()
        {
            if (!isBuyingMode) return true;

            List<Item> flatItems = new List<Item>();
            foreach(ShopItem shopItem in GetAllItems())
            {
                Item item = shopItem.getItem();
                int quantity = shopItem.getQuantity();

                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return currentShopper.GetComponent<ItemInventory>().HasSpaceFor(flatItems);
        }


        public float TotalPrice()
        {
            float total = 0;
            foreach (ShopItem item in GetAllItems())
            {
                total += item.getPrice() * item.getQuantity();
            }

            return total;
        }
        public void AddToTransaction(Item item, int quantity) {

            if(!transaction.ContainsKey(item)) transaction[item] = 0;

            var availabilities = GetAvailabilities();
            int availability = availabilities[item];

            if (transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
                return;
            }
            transaction[item] += quantity;

            if(transaction[item] <= 0) transaction.Remove(item);
            onChange?.Invoke();
        }

        public void ConfirmTransaction() {
            ItemInventory shopperInventory = currentShopper.GetComponent<ItemInventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;

            foreach (ShopItem shopItem in GetAllItems())
            {
                Item item = shopItem.getItem();
                int quantity = shopItem.getQuantity();

                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                        BuyItem(shopperInventory, shopperPurse, shopItem, item);
                    else
                        SellItem(shopperInventory, shopperPurse, shopItem, item);
                }
            }

            onChange?.Invoke();
        }

        public string GetName() { return Name; }

        public bool HandleRaycast(PlayerController callingCon)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingCon.GetComponent<Shopper>().SetActiveShop(this);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        private void BuyItem(ItemInventory shopperInventory, Purse shopperPurse, ShopItem shopItem, Item item)
        {
            if (shopperPurse.getBalance() < shopItem.getPrice()) return;
            bool transacted = shopperInventory.AddItem(item);
            if (transacted)
            {
                AddToTransaction(item, -1);
                if (!stockSold.ContainsKey(item))
                {
                    stockSold[item] = 0;
                }
                stockSold[item]++;
                shopperPurse.UpdateBalance(-shopItem.getPrice());
            }
            return;
        }

        private void SellItem(ItemInventory shopperInventory, Purse shopperPurse, ShopItem shopItem, Item item)
        {
            bool transacted = shopperInventory.RemoveItem(item);
            if (transacted)
            {
                AddToTransaction(item, -1);
                if (!stockSold.ContainsKey(item))
                {
                    stockSold[item] = 0;
                }
                stockSold[item]--;
                shopperPurse.UpdateBalance(shopItem.getPrice());
            }
        }

        private int CountIemInventory(Item itemHeld)
        {
            return currentShopper.GetComponent<ItemInventory>().GetItemCount(itemHeld);
        }

        private int GetShopperLevel()
        {
            BaseStats stats = currentShopper.GetComponent<BaseStats>();
            if (stats == null) return 0;

            return stats.GetLevel();
        }

        private Dictionary<Item, float> GetPrices()
        {
            Dictionary<Item, float> prices = new Dictionary<Item, float>();

            foreach (StockItemConfig config in GetAvailableConfigs())
            {
                if (isBuyingMode)
                {
                    if (!prices.ContainsKey(config.itemHeld))
                    {
                        prices[config.itemHeld] = config.itemHeld.GetValue();
                    }
                    prices[config.itemHeld] *= (1 - config.buyingDiscountPct);
                }
                else
                {
                    prices[config.itemHeld] = config.itemHeld.GetValue() * (sellingDiscount);
                }
               
            }

            return prices;
        }

        private Dictionary<Item, int> GetAvailabilities()
        {
            Dictionary<Item, int> availabilities = new Dictionary<Item, int>();

            foreach (StockItemConfig config in GetAvailableConfigs())
            {
                if (isBuyingMode)
                {
                    if (!availabilities.ContainsKey(config.itemHeld))
                    {
                        int sold = 0;
                        stockSold.TryGetValue(config.itemHeld, out sold);

                        availabilities[config.itemHeld] = -sold;
                    }
                    availabilities[config.itemHeld] += config.initialStock;
                }
                else
                {
                    availabilities[config.itemHeld] = CountIemInventory(config.itemHeld);
                }
            }

            return availabilities;
        }

        private IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach (StockItemConfig config in stockConfig)
            {
                if (config.minimumLevel > shopperLevel) continue;
                yield return config;
            }
        }

        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();
            foreach (KeyValuePair<Item, int> pair in stockSold)
            {
                saveObject[pair.Key.GetUUID()] = pair.Value;
            }

            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;
            stockSold.Clear();
            foreach (KeyValuePair<string, int> pair in saveObject)
            {
                stockSold[itemCache.GetItem(pair.Key)] = pair.Value;
            }
        }
    }
}
