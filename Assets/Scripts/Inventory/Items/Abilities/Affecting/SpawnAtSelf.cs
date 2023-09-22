using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Target Self Prefab", menuName = "Items/Abilities/Effects/SpawnTargetSelf")]
    public class SpawnAtSelf : EffectStrategy
    {
        [SerializeField] GameObject prefabEffect;
        [SerializeField] float destroyDelay = -1;
        [SerializeField] float spawnDelay = -1;
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(EndParticles(data));
        }

        IEnumerator EndParticles(AbilityData data)
        {
            if(spawnDelay >= 0)
            yield return new WaitForSeconds(spawnDelay);
            GameObject prefab = Instantiate(prefabEffect, data.GetUser().transform.position, Quaternion.identity);
            if (destroyDelay >= 0)
                Destroy(prefab, destroyDelay);
        }
    }
}

