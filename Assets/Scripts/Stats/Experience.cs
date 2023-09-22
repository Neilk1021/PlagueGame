using System;
using UnityEngine;
using RPG.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints;
        public event Action onExperiencedGained;

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void GainExperience(float Exp)
        {
            experiencePoints += Exp;
            onExperiencedGained();
        }

        public float GetPoints()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }

}