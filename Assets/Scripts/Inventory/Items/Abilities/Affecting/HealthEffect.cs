using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Health Effect", menuName = "Items/Abilities/Effects/Heath")]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] float healthChange;
        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach (GameObject target in data.getTargets())
            {
                if(healthChange < 0)
                    target.GetComponent<Health>()?.TakeDamage(Math.Abs(healthChange), data.GetUser());
                else
                    target.GetComponent<Health>()?.Heal(healthChange);

            }

            finished();
        }
    }
}