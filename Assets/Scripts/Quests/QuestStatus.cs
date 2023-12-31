using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Quests
{
    [System.Serializable]
    public class QuestStatus 
    {
        Quest quest;
        [SerializeField] List<string> completedObjectives = new List<string>();

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }
    }

}