using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventory
{
    [System.Serializable]
    public class Item : ScriptableObject
    {
        public enum Rarity
        {
            Common,
            Uncommon,
            Rare,
            Legendary
        }
        [SerializeField] protected string Name;
        [TextArea]
        [SerializeField] protected string Desc;
        [Range(0, 10000)]
        [SerializeField] protected float Value;
        [SerializeField] protected ItemCatagory itemCatagory;
        [SerializeField] Rarity rarity;
        [SerializeField] int StackSize = 1;
        [SerializeField] Sprite Icon;
        [SerializeField] Sprite PlayerCursor;
        //[SerializeField] int UUID;
        [SerializeField] string UUID = System.Guid.NewGuid().ToString();
        public virtual string GetName() { return Name;}
        public string GetDesc() { return Desc; }
        public float GetValue() { return Value; }
        public Sprite GetTexture() { return PlayerCursor;}
        public Sprite GetSprite() { return Icon; }
        public string GetUUID() { return UUID; }
        public int GetStackSize() { return StackSize; }
        public Rarity GetRarity() { return rarity; }

        public ItemCatagory GetItemCatagory()
        {
            return itemCatagory;
        }

    }
}