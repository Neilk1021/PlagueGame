using RPG.Inventory;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Shops
{
    public class ShopItem
    {
        Item item;
        int availability;
        float price;
        int quantityInTransaction;

        public ShopItem(Item item, int availability, float price, int quantityInTransaction)
        {
            this.item = item;
            this.availability = availability;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public Item getItem()
        {
            return item;
        }

        public string getName() { return item.GetName(); }

        public Sprite getIcon()
        {
            return item.GetSprite();
        }

        public int getQuantity()
        {
            return quantityInTransaction;
        }

        public float getPrice()
        {
            return price;
        }

        public int getAvailability()
        {
            return availability;
        }
    }
}
