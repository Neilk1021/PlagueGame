using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Inventory;
using RPG.Stats;
using RPG.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] TakeDamageEvent UpdateUi;
        [SerializeField] UnityEvent death;
        Armor HeadArmor, TorsoArmor, LegArmor;
        float resistancePnts = 0;
        float resistancePct = 0;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        LazyValue<float> CurrentHealth; 
        bool Dead = false;
        BaseStats baseStats;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            CurrentHealth = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
           return baseStats.GetStat(Stat.Health);
        }

        private void Start()
        {
            CurrentHealth.ForceInit();
            //if(CurrentHealth < 0){
            //    CurrentHealth = baseStats.GetStat(Stat.Health);
            //}
        }

        public void EquipArmor(string UUID)
        {
            Armor temp = GetComponent<ItemCache>().GetArmor(UUID);
            switch (temp.GetEquipmentSlot())
            {
                case Equipment.EquipmentSlot.Head:
                    HeadArmor = temp;
                    break;
                case Equipment.EquipmentSlot.Torso:
                    TorsoArmor = temp;
                    break;
                case Equipment.EquipmentSlot.Legs:
                    LegArmor = temp;
                    break; 
                default:
                    break;
            }

            resistancePnts = 0;
            if (HeadArmor != null) resistancePnts += HeadArmor.GetArmorResistancePoints();
            if (TorsoArmor != null) resistancePnts += TorsoArmor.GetArmorResistancePoints();
            if (LegArmor != null) resistancePnts += LegArmor.GetArmorResistancePoints();

            resistancePct = 0;
            if (HeadArmor != null) resistancePct += HeadArmor.GetArmorResistancePct();
            if (TorsoArmor != null) resistancePct += TorsoArmor.GetArmorResistancePct();
            if (LegArmor != null) resistancePct += LegArmor.GetArmorResistancePct();
        }

        private void OnEnable()
        {
            baseStats.onLevelUp.AddListener(HealthReset);
        }

        private void OnDisable()
        {
            baseStats.onLevelUp.RemoveListener(HealthReset);
        }

        public float getMaxHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float getHealth()
        {
            return CurrentHealth.value;
        }


        public void HealthReset()
        {
            CurrentHealth.value = getMaxHealth();
        }


        public object CaptureState()
        {
            return CurrentHealth.value;
        }

        public void RestoreState(object state)
        {
            CurrentHealth.value = (float)state;

            if (CurrentHealth.value <= 0) {
                Die();
            }
        }

        public bool isDead()
        {
            return Dead;
        }


        public void TakeDamage(float Damage, GameObject Instigator)
        {
            Damage = Mathf.Clamp((Damage - resistancePnts) * (1 - resistancePct), 2, Damage);

            CurrentHealth.value = Mathf.Max(CurrentHealth.value - Damage, 0);

            if (CurrentHealth.value <= 0)
            {
                Die();
                AwardExperience(Instigator);
            }

            takeDamage?.Invoke(Damage);
            UpdateUi?.Invoke(getHealth()/getMaxHealth());
        }

        public void Heal(float HealAmount)
        {
            CurrentHealth.value += HealAmount;
            Mathf.Clamp(CurrentHealth.value, 0, getMaxHealth());   
        }

        public void AwardExperience(GameObject Instigator)
        {
            Experience experience = Instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(baseStats.GetStat(Stat.Experience));
            
        }

        /*public float GetPercentage()
        {
            return CurrentHealth / getMaxHealth();
        }*/

        private void Die()
        {
            if (!Dead)
            {
                Dead = true;
                death?.Invoke();
                GetComponent<Animator>().SetTrigger("Die");
                GetComponent<ActionSchedular>().CancelCurrentAction();
            }
        }
    }

}