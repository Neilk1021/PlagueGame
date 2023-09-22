using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventory;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        Dictionary<Item, float> cooldownTimers = new Dictionary<Item, float>();
        Dictionary<Item, float> InitialCooldownTimers = new Dictionary<Item, float>();

        private void Update()
        {
            var key = new List<Item>(cooldownTimers.Keys); ;

            foreach (Item ability in key)
            {
                cooldownTimers[ability] -= Time.deltaTime;
                if(cooldownTimers[ability] < 0)
                {
                    cooldownTimers.Remove(ability);
                    InitialCooldownTimers.Remove(ability);
                }
            }
        }

        public void StartCooldown(Item ability, float cooldownTime)
        {
            cooldownTimers[ability] = cooldownTime;
            InitialCooldownTimers[ability] = cooldownTime;
        }

        public float GetTimeRemaining(Item ability)
        {
            if (!cooldownTimers.ContainsKey(ability)) return 0;

            return cooldownTimers[ability];
        }

        public float getFractionRemaining(Item ability)
        {
            if (!cooldownTimers.ContainsKey(ability)) return 0;
            return cooldownTimers[ability] / InitialCooldownTimers[ability];
        }

    }

}