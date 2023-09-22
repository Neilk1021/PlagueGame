using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreground;
        [SerializeField] Health health;
        [SerializeField] Canvas root;

        private void Start()
        {
            root.enabled = false;
        }

        public void UpdateHealthBar()
        {
            float pct = health.getHealth()/ health.getMaxHealth();
            if(health.getHealth() <= 0 || health.getHealth() == health.getMaxHealth())
            {
                root.enabled = false;
                return;
            }

            root.enabled = true;
            foreground.localScale = new Vector3(pct, 1, 1);
        }
    }

}
