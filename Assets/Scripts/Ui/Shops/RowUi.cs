using RPG.Shops;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace RPG.UI.Shops
{
    public class RowUi : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameField;
        [SerializeField] TextMeshProUGUI availabilityField;
        [SerializeField] TextMeshProUGUI priceField;
        [SerializeField] TextMeshProUGUI quantityField;
        [SerializeField] Image iconImage;

        Shop currentShop;
        ShopItem heldItem;
        public void Setup(ShopItem item, Shop shop, ShopUi shopUi)
        {
            nameField.text = item.getName();
            iconImage.sprite = item.getIcon();
            availabilityField.text = $"{item.getAvailability()}";
            priceField.text = $"${item.getPrice():N2}";
            quantityField.text = $"{item.getQuantity()}";
            currentShop = shop;
            heldItem = item;
        }

        public void Add()
        {
            currentShop.AddToTransaction(heldItem.getItem(), 1);
        }

        public void Remove()
        {
            currentShop.AddToTransaction(heldItem.getItem(), -1);
        }
    }

}