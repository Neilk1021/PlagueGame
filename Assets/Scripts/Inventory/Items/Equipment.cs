using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventory
{
    public abstract class Equipment : Item
    {
        public enum EquipmentSlot
        {
            Weapon,
            Legs,
            Torso,
            Head,
            Cape,
            Ring
        }

        public enum Hand
        {
            RightHand,
            LeftHand,
        }

        [SerializeField] EquipmentSlot equipmentSlot;
        [SerializeField] Hand hand;

        public EquipmentSlot GetEquipmentSlot()
        {
            return equipmentSlot;
        }

        public Hand getHand()
        {
            return hand;
        }
    }
}

