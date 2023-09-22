using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Saving;

namespace RPG.Inventory
{
    public class Purse : MonoBehaviour, ISaveable
    {
        [SerializeField] float startingBalance = 200f;
        public event Action OnUpdatePurse;
        float balance = 0;

        private void Awake()
        {
            balance = startingBalance;
        }

        public float getBalance() { return balance; }

        public void UpdateBalance(float Amount)
        {
            balance += Amount;
            print($"Balance: {balance}");
            OnUpdatePurse?.Invoke();
        }

        public object CaptureState()
        {
            return balance;
        }

        public void RestoreState(object state)
        {
            balance = (float)state;
        }
    }
}