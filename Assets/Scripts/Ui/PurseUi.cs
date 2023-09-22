using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventory;
using TMPro;

namespace RPG.UI
{
    public class PurseUi : MonoBehaviour {
        [SerializeField] TextMeshProUGUI balanceField;

        Purse purse = null;

        private void Start()
        {
            purse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();
            RefreshUi();
            purse.OnUpdatePurse += RefreshUi;
        }

        private void RefreshUi()
        {
            balanceField.text = $"${purse.getBalance():N2}";
        }
    }
}