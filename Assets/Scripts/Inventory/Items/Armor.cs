using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventory
{
    [CreateAssetMenu(fileName = "Armor", menuName = "Items/Armor/CreateArmor")]
    public class Armor : Equipment
    {
        public enum HelmetType
        {
            NoHead,
            NoHair,
            Hair
        }

        public int[] ArmorPieces;
        public HelmetType helmetType;
        public bool Empty = false;
        [SerializeField] Color Color_Primary;
        [SerializeField] Color Color_Secondary;
        [SerializeField] Color Color_Leather_Primary;
        [SerializeField] Color Color_Leather_Secondary;
        [SerializeField] Color Color_Metal_Primary;
        [SerializeField] Color Color_Metal_Secondary;
        [SerializeField] Color Color_Metal_Dark;


        [Header("Stats")]
        [SerializeField] float ArmorResitancePoints;
        [SerializeField] float ArmorResitancePct;

        public float GetArmorResistancePoints()
        {
            return ArmorResitancePoints;
        }
        public float GetArmorResistancePct()
        {
            return ArmorResitancePct;
        }
    }
}
