using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;
using RPG.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int StartingLevel;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression;
        [SerializeField] GameObject Particles;
        [SerializeField] bool shouldUseMod = false;
        Experience exp = null;

        public UnityEvent onLevelUp;

        LazyValue<int> currentLevel;

        public void Awake()
        {
            exp = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(GetInitialLevel);
        }

        private int GetInitialLevel()
        {
            return CalcLevel();
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) *(1+GetPercentageModifier(stat)/100);
        }


        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(characterClass, GetLevel(), stat);
        }

        void UpdateLevel()
        {
            int newLevel = CalcLevel();

            if (currentLevel.value < newLevel)
            {
                currentLevel.value = newLevel;
                GameObject particles = Instantiate(Particles, transform.position, Quaternion.identity);
                particles.transform.parent = transform;
                onLevelUp?.Invoke();
            }
        }

        public int GetLevel()
        {
            if (currentLevel.value < 1) UpdateLevel();

            return currentLevel.value;
        }


        public int CalcLevel()
        {
            if (exp == null) return StartingLevel;

            float currentExp = exp.GetPoints();
            int MaxLevel = progression.GetLevels(Stat.ExpToLevelUp, characterClass);
            for (int i = 1; i <= MaxLevel; i++)
            {
                float expToLevel = progression.GetStat(characterClass, i, Stat.ExpToLevelUp);
                if (currentExp < expToLevel) return i; 
            }

            return MaxLevel++;
        }

        public float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseMod) return 0;

            float Total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifier(stat))
                {
                    Total += modifier;
                }
            }

            return Total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseMod) return 0;

            float Total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifier(stat))
                {
                    Total += modifier;
                }
            }

            return Total;
        }

        private void OnEnable()
        {
            if(exp != null)
            {
                exp.onExperiencedGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if(exp != null)
            {
                exp.onExperiencedGained -= UpdateLevel;
            }   
        }
    }

}