using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Target Prefab", menuName = "Items/Abilities/Effects/SpawnTargetPrefab")]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] GameObject prefabEffect;
        [SerializeField] float destroyDelay = -1;
        public override void StartEffect(AbilityData data, Action finished)
        {
            GameObject prefab = Instantiate(prefabEffect, data.GetTargetedPoint(), Quaternion.identity);
            if (destroyDelay >= 0) 
            Destroy(prefab, destroyDelay);
        }
    }
}
