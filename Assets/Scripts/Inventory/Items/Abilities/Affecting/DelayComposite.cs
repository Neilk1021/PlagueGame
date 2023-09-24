using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "DelayComposite Effect", menuName = "Items/Abilities/Effects/DelayComposite")]
    public class DelayComposite : EffectStrategy
    {
        [SerializeField] float delay = 0;
        [SerializeField] EffectStrategy[] delayedEffects;
        [SerializeField] bool abortOnCancel = false;
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayedEffect(data, finished));
        }

        private IEnumerator DelayedEffect(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(delay);
            if(abortOnCancel && data.isCancelled()) { yield break; }
            foreach(EffectStrategy effect in delayedEffects)
            {
                effect.StartEffect(data, finished);
            }

        }
    }
}
