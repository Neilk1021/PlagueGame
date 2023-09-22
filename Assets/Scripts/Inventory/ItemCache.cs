using UnityEngine;

namespace RPG.Inventory
{
    public class ItemCache : MonoBehaviour
    {
        [SerializeField] ItemIndex Reference; 
        public void GetWeapon(string UUID, out WeaponConfig item)
        {
            if ((WeaponConfig)Reference.GetItem(UUID) != null)
            {
                item = (WeaponConfig)Reference.GetItem(UUID);
                return;
            }
            item = null;
        }

        public Armor GetArmor(string UUID)
        {
            if ((Armor)Reference.GetItem(UUID) != null)
            {
                return (Armor)Reference.GetItem(UUID);
            }
            return null;
        }

        public Equipment GetEquipment(string UUID)
        {
            if (Reference.GetItem(UUID) != null)
            {
                return (Equipment)Reference.GetItem(UUID);
            }

            return null;
        }

        public Item GetItem(string UUID)
        {
            if(Reference.GetItem(UUID) != null)
            {
                return Reference.GetItem(UUID);
            }

            return null;
        }
    }
}