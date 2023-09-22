using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent OnHitEvent;
        [SerializeField] UnityEvent OnAttack;

        public void OnHit()
        {
            OnHitEvent?.Invoke();
        }

        public void RunAnim()
        {
            OnAttack?.Invoke();
            GetComponentInChildren<Animator>()?.SetTrigger("Fire");
        }
    }

}