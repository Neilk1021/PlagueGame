using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RPG.Utils;
using RPG.Stats;
using RPG.Saving;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, ISaveable
    {
        LazyValue<float> mana;

        LazyValue<float> manaRegenRate;

        [SerializeField] UseManaEvent UpdateUi;

        BaseStats stats; 
        [System.Serializable]
        public class UseManaEvent : UnityEvent<float>
        {
        }

        private void Awake()
        {
            stats = GetComponent<BaseStats>();
            mana = new LazyValue<float>(getInitialMana);
            manaRegenRate = new LazyValue<float>(getInitialManaRegen);
        }

        private float getInitialManaRegen()
        {
            return GetComponent<BaseStats>().GetStat(Stat.ManaRegen);
        }

        private float getInitialMana()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        }

        private void Start()
        {
            UpdateUi?.Invoke(mana.value / GetComponent<BaseStats>().GetStat(Stat.Mana));
        }

        public float getMana() { return mana.value; }
        public float getMaxMana() { return stats.GetStat(Stat.Mana); }

        public bool UseMana(float manaToUse)
        {
            if(manaToUse > mana.value) { return false; }
            mana.value -= manaToUse;

            UpdateUi?.Invoke(mana.value / getMaxMana());
            return true;
        }

        private void Update()
        {
            if(mana.value < getMaxMana())
            {
                mana.value += manaRegenRate.value * Time.deltaTime;

                if(mana .value > getMaxMana()) { mana.value = getMaxMana(); }
                UpdateUi?.Invoke(mana.value / getMaxMana());
            }
        }

        public object CaptureState()
        {
            return mana.value;
        }

        public void RestoreState(object state)
        {
            mana.value = (float)state;
        }
    }
}