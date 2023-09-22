using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RPG.Inventory
{
    [CreateAssetMenu(fileName = "ItemIn", menuName = "Core/ItemIndex")]
    public class ItemIndex : ScriptableObject
    {
        [SerializeField] Item[] items;

        /*public Item GetItem(int UUID)
        {
            return items[UUID];
        }*/

        public Item GetItem(string UUID)
        {
            return items.First(x => x.GetUUID() == UUID);
        }
    }
}
