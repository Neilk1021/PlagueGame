using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

namespace RPG.Inventory
{
    [CreateAssetMenu(fileName ="Weapon", menuName = "Items/Weapons/CreateWeapon")]
    public class WeaponConfig : Equipment
    {
        [SerializeField] AnimatorOverrideController weaponOverride;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float PctDamage = 0;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] Projectile projectile = null;

        const string WeaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if(weaponPrefab != null)
            {
                weapon = Instantiate(weaponPrefab, getHand(rightHand, leftHand));
                weapon.gameObject.name = WeaponName;
            }

            var overrideCon = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (weaponOverride != null)
            {
                animator.runtimeAnimatorController = weaponOverride;
            }
            else if(overrideCon != null)
            {
                animator.runtimeAnimatorController = overrideCon.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform OldWeapon = rightHand.Find(WeaponName);
            if(OldWeapon == null) OldWeapon = leftHand.Find(WeaponName);
            if (OldWeapon == null) return;

            OldWeapon.name = "DESTROYING";
            Destroy(OldWeapon.gameObject);
        }

        public bool hasProjectile()
        {
            if (projectile != null) return true;
            return false;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float CalcDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, getHand(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, CalcDamage, instigator);
        }

        private Transform getHand(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (getHand() == Hand.RightHand) handTransform = rightHand;
            else handTransform = leftHand;

            return handTransform; 
        }

        public float GetDamage()
        {
            return weaponDamage; 
        }

        public float GetPct()
        {
            return PctDamage;
        }

        public float GetRange()
        {
            return weaponRange;
        }

    }

}
