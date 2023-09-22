using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Abilities;
using RPG.Saving;

namespace RPG.Inventory
{
    public class ActionSlotUI : MonoBehaviour
    {
        [SerializeField] int index = 0;
        [SerializeField] Image cooldownOverlay = null;

        [SerializeField] TextMeshProUGUI Count;
        [SerializeField] Image Icon;
        [SerializeField] GameObject Panel;

        ActionStore store;
        InventoryManager inventoryManager;
        CooldownStore cooldownStore;
        [SerializeField] Sprite DefaultIcon;

        private void Awake()
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            store = player.GetComponent<ActionStore>();
            store.storeUpdated += UpdateIcon;
            cooldownStore = player.GetComponent<CooldownStore>();
        }

        private void Start()
        {
            UpdateIcon();
        }

        private void Update()
        {
            if (GetItem() == null) return;

            cooldownOverlay.fillAmount = cooldownStore.getFractionRemaining(GetItem());
        }
        // PUBLIC

        public void Click()
        {
            ActionItem cachedItem = (ActionItem)inventoryManager.GetCachedItem();

            if(GetItem() != null)
            {
                inventoryManager.ClickItem(GetItem().GetUUID(), false, GetNumber());
                RemoveItems(GetNumber());
            }
            else { 
                inventoryManager.ClickItem(null, false);
            }

            if (cachedItem != null) AddItems(cachedItem, inventoryManager.GetCachedAmount());
        }

        public void AddItems(Item item, int number)
        {
            store.AddAction(item, index, number);
        }

        public Item GetItem()
        {
            return store.GetAction(index);
        }

        public int GetNumber()
        {
            return store.GetNumber(index);
        }

        public int MaxAcceptable(Item item)
        {
            return store.MaxAcceptable(item, index);
        }

        public void RemoveItems(int number)
        {
            store.RemoveItems(index, number);
        }

        // PRIVATE
        void UpdateIcon()
        {
            if(GetItem() == null)
            {
                Icon.sprite = DefaultIcon;
                Panel.SetActive(false);
                return;
            }

            Icon.sprite = GetItem().GetSprite();
            Panel.SetActive(true);
            Count.text = GetNumber().ToString();
        }
    }

}
