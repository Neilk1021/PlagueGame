using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventory;
using RPG.Saving;

namespace RPG.Control
{
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] Material[] playerMat;
        [SerializeField] GameObject[] Helmets;
        [SerializeField] GameObject[] Torsos;
        [SerializeField] GameObject[] Legs;

        [SerializeField] GameObject Hair, Head;

        ArmorUUID defaultArmor;
        ArmorUUID CurrentArmor;
        ItemCache itemCache;

        private void Awake()
        {
            itemCache = GetComponent<ItemCache>();
        }


        void SetDefaults(GameObject hair, GameObject head, Armor defaultHelmet, Armor defaultTorso, Armor defaultLegs)
        {
            Hair = hair;
            Head = head;

            defaultArmor = new ArmorUUID(defaultHelmet.GetUUID(), defaultTorso.GetUUID(), defaultLegs.GetUUID());
        }

        public void LoadArmor(string UUID)
        {
            Armor armor = itemCache.GetArmor(UUID);

            switch (armor.GetEquipmentSlot())
            {
                case Equipment.EquipmentSlot.Head:
                    LoadHelmet(armor);
                    break;
                case Equipment.EquipmentSlot.Torso:
                    LoadTorso(armor);
                    break;
                case Equipment.EquipmentSlot.Legs:
                    LoadLegs(armor);
                    break;
            }
        }

        void LoadHelmet(Armor armor)
        {
            for (int i = 0; i < Helmets.Length; i++)
            {
                Helmets[i].SetActive(false);
            }

            for (int i = 0; i < armor.ArmorPieces.Length; i++)
            {
                Helmets[armor.ArmorPieces[i]].SetActive(true);
            }


            switch (armor.helmetType)
            {
                case Armor.HelmetType.NoHead:
                    Head.SetActive(false);
                    Hair.SetActive(false);
                    break;

                case Armor.HelmetType.NoHair:
                    Head.SetActive(true);
                    Hair.SetActive(false);
                    break;
                case Armor.HelmetType.Hair:
                    Head.SetActive(true);
                    Hair.SetActive(true);
                    break;
            }
        }

        void LoadTorso(Armor armor)
        {
            for (int i = 0; i < Torsos.Length; i++)
            {
                Torsos[i].SetActive(false);
            }

            for (int i = 0; i < armor.ArmorPieces.Length; i++)
            {
                Torsos[armor.ArmorPieces[i]].SetActive(true);
            }

        }

        void LoadLegs(Armor armor)
        {
            for (int i = 0; i < Legs.Length; i++)
            {
                Legs[i].SetActive(false);
            }


            for (int i = 0; i < armor.ArmorPieces.Length; i++)
            {
                Legs[armor.ArmorPieces[i]].SetActive(true);
            }

        }

    }

}