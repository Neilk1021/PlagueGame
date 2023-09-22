using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventory;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        Shop activeShop = null;

        public event Action activeShopChanged;

        public void SetActiveShop(Shop shop)
        {
            activeShop?.SetShopper(null);
            activeShop = shop;
            activeShop?.SetShopper(this);
            activeShopChanged?.Invoke();
        }

        public Shop getActiveShop()
        {
            return activeShop;
        }
    }

}
