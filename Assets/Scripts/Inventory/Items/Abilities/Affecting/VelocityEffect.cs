using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Velocity Effect", menuName = "Items/Abilities/Effects/Velocity")]
    public class VelocityEffect : EffectStrategy
    {
        [SerializeField] float Speed, Durration;
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.GetUser().GetComponent<Move>()?.AddForce(data.GetTargetedPoint(), Speed, Durration);
        }
    }
}