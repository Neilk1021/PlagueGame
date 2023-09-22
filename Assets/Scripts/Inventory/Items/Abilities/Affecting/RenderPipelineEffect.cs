using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Renderer Effect", menuName = "Items/Abilities/Effects/RenderPipeline")]
    public class RenderPipelineEffect : EffectStrategy
    {
        [SerializeField] float Durration;
        [SerializeField] UnityEngine.Rendering.Universal.UniversalRendererData feature;
        public override void StartEffect(AbilityData data, Action finished)
        { 
            feature.rendererFeatures[1].SetActive(false);
            feature.rendererFeatures[2].SetActive(true);
            data.StartCoroutine(Return(Durration));
        }

        IEnumerator Return(float Durration_)
        {
            yield return new WaitForSeconds(Durration_);
            feature.rendererFeatures[1].SetActive(true);
            feature.rendererFeatures[2].SetActive(false);
        }
    }
}
