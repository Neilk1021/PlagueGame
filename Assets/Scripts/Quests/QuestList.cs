using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour
    {
        [SerializeField] List<QuestStatus> questStatuses = new List<QuestStatus>();
        public event Action onUpdate;

        public void AddQuest(Quest quest)
        {
            Debug.Log("questing");
            if (HasQuest(quest)) return;

            QuestStatus newStatus = new QuestStatus(quest);
            questStatuses.Add(newStatus);
            onUpdate?.Invoke();
        }

        private bool HasQuest(Quest quest)
        {
            foreach (QuestStatus item in questStatuses)
            {
                if(item.GetQuest() == quest)
                {
                    return true;
                }
            }

            return false; 
        }

        public IEnumerable<QuestStatus> GetStatus()
        {
            return questStatuses;
        }
    }
}