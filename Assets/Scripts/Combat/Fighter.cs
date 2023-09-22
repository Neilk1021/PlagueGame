using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using RPG.Utils;
using RPG.Inventory;
using System.Collections.Generic;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float TimeBtwnAttacks;
        [SerializeField] Transform rightHandTransform  = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] AnimatorOverrideController weaponOverride;

        Health target;
        Move move;

        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        [SerializeField] WeaponConfig deafaultWeapon; 

        float TimeSinceLastAttack = 200;

        private void Awake()
        {
            move = GetComponent<Move>();
            currentWeaponConfig = deafaultWeapon;
            currentWeapon = new LazyValue<Weapon>(GetInitialWeapon);
        }

        private Weapon GetInitialWeapon()
        {
            return AttachWeapon(deafaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        public void EquipWeapon(string UUID)
        {
            WeaponConfig item;
            GetComponent<ItemCache>().GetWeapon(UUID, out item);

            currentWeaponConfig = item;
            currentWeapon.value = AttachWeapon(item);
        }


        public void EquipWeaponItem(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (!GetComponent<Move>().CanMoveTo(combatTarget.transform.position)) return false;

            Health targetCheck = combatTarget.GetComponent<Health>();

            return (targetCheck != null && !targetCheck.isDead());
        }

        private void Update()
        {
            TimeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.isDead()) return;

            if (!InRange())
            {
                move.MoveTo(target.transform.position, 1);
            }
            else
            {
                transform.LookAt(target.transform.position);
                move.Cancel();
                AttackBehavior();
            }
        }

        private void AttackBehavior()
        {
            if(TimeSinceLastAttack >= TimeBtwnAttacks)
            {
                //Triggers Hit() Event
                GetComponent<Animator>().ResetTrigger("CancelAttack");
                GetComponent<Animator>().SetTrigger("Attack");
                currentWeapon.value?.RunAnim();
                TimeSinceLastAttack = 0;
            }
        }

        void Hit()
        {
            if (target == null) return;

            float FinalDamage = CalcDamage();

            if (currentWeaponConfig.hasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target.GetComponent<Health>(), gameObject, FinalDamage);
            }
            else
            {
                currentWeapon.value?.OnHit();
                target.TakeDamage(FinalDamage, gameObject);
            }
        }

        private float CalcDamage()
        {
           return GetComponent<BaseStats>().GetStat(Stat.Damage);
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPct();
            }
        }

        void Shoot()
        {
            Hit();
        }

        public void Cancel()
        {
            GetComponent<Animator>().SetTrigger("CancelAttack");
            move.Cancel();
            target = null;
        }

        private bool InRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetRange();
        }

        public void Attack(GameObject target)
        {
            GetComponent<ActionSchedular>().StartAction(this);
            this.target = target.transform.GetComponent<Health>();
        }

        public Health GetTarget()
        {
            return target;
        }
        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            WeaponConfig weapon = Resources.Load<WeaponConfig>((string)state);  
            EquipWeaponItem(weapon);
        }

    }
}