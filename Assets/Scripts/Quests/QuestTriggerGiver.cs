using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestTriggerGiver : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                GetComponent<QuestGiver>().GiveQuest();
                Destroy(gameObject);
            }
        }
    }

}
