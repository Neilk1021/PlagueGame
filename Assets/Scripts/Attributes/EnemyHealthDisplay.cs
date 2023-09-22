using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Combat;

namespace RPG.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter player;
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            Health health = player.GetTarget();
            if (health != null)
            {
                GetComponent<Text>().text = string.Format("{0:0}/{1:0}", health.getHealth(), health.getMaxHealth());
            }
            else
            {
                GetComponent<Text>().text = "None";
            }
        }
    }
}
