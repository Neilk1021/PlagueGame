using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Shops;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUi : MonoBehaviour
    {
        [SerializeField] Transform listRoot;
        [SerializeField] RowUi rowPrefab;
        [Header("TextFields")]
        [SerializeField] TextMeshProUGUI shopName;
        [SerializeField] TextMeshProUGUI totalField;
        [Header("Buttons")]
        [SerializeField] Button confirmButton;
        [SerializeField] Button SwitchButton;
        [Header("SwitchButton Images")]
        [SerializeField] Sprite BuyImage;
        [SerializeField] Sprite SellImage;

        Shopper shopper = null;
        Shop currentShop = null;

        [SerializeField] Color OverBalanceColor;
        Color OriginalTotalColor;

        private void Awake() { shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>(); }

        // Start is called before the first frame update
        void Start()
        {
            OriginalTotalColor = totalField.color;
            if (shopper == null) return;
            shopper.activeShopChanged += ShopChanged;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            SwitchButton.onClick.AddListener(SwitchMode);

            ShopChanged();
        }

        void ShopChanged()
        {
            if (currentShop != null) currentShop.onChange -= RefreshUI;

            currentShop = shopper?.getActiveShop();
            gameObject.SetActive(currentShop != null);

            foreach (FilterButtonUI buttons in GetComponentsInChildren<FilterButtonUI>())
            {
                buttons.SetShop(currentShop);
            }

            if (currentShop == null) return;
            shopName.text = currentShop.GetName();
            currentShop.onChange += RefreshUI;

            RefreshUI();
        }

        public void RefreshUI()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (ShopItem item in currentShop.GetFilteredItems())
            {
                print("eee");
                RowUi row = Instantiate<RowUi>(rowPrefab, listRoot);
                row.Setup(item, currentShop, this);
            }
            Color color = currentShop.IsOverBalance() ? OverBalanceColor : OriginalTotalColor;
            totalField.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>Total: ${currentShop.TotalPrice():N2}</color>";
            confirmButton.interactable = currentShop.CanTransact();

            Image[] buyImages = SwitchButton.GetComponentsInChildren<Image>();
            Image buyImage = null;
            for (int i = 0; i < buyImages.Length; i++)
            {
                if (buyImages[i] != SwitchButton.targetGraphic)
                {
                    buyImage = buyImages[i];
                    break;
                }
            }

            TextMeshProUGUI comfirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if (currentShop.IsBuyingMode())
            {
                buyImage.sprite = SellImage;
                comfirmText.text = "Buy";
            }
            else
            {
                buyImage.sprite = BuyImage;
                comfirmText.text = "Sell";
            }
        }

        public void Close()
        {
            shopper.SetActiveShop(null);
        }

        public void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }

        public void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode());
        }

    }
}