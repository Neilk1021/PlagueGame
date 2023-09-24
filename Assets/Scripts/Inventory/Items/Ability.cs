using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventory;
using RPG.Attributes;
using RPG.Core;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Items/Abilities/CreateAbility")]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        [SerializeField] EffectStrategy[] effectFinishedStrategies;
        [SerializeField] float manaCost = 0;

        AbilityData dataStore;
        [SerializeField] float cooldown;
        Ability()
        {
            itemCatagory = ItemCatagory.Abilities;
        }

        public override void Use(GameObject user)
        {
            if(user.GetComponent<Mana>() == null) { return; }
            float mana = user.GetComponent<Mana>().getMana();

            if (user.GetComponent<CooldownStore>()?.GetTimeRemaining(this) > 0) return;
            if(mana < manaCost) { return; }

            AbilityData data = new AbilityData(user);

            user.GetComponent<ActionSchedular>().StartAction(data);

            targetingStrategy.StartTargeting(data, 
                () => 
                TargetAquired(data));
        }

        private void TargetAquired(AbilityData data)
        {
            if (data.isCancelled()) { return; }

            Mana mana = data.GetUser().GetComponent<Mana>();
            if (mana == null) {
                Debug.LogError("No mana component");
                return; }
            if(!mana.UseMana(manaCost)) { return; }

            data.GetUser()?.GetComponent<CooldownStore>()?.StartCooldown(this, cooldown);
            for (int i = 0; i < filterStrategies.Length; i++)
            {
                data.setTargets(filterStrategies[i].Filter(data.getTargets()));
            }

            for (int i = 0; i < effectStrategies.Length; i++)
            {
                effectStrategies[i].StartEffect(data, EffectFinished);
            }
            dataStore = data;
        }

        private void EffectFinished()
        {
        }
    }
}