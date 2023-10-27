using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;
using TMPro;
using System;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title, progress;

    QuestStatus qs; 
    public void Init(QuestStatus quest)
    {
        this.qs = quest;
        title.text = qs.GetQuest().GetTitle();
        progress.text = qs.GetCompletedCount()+ "/" + qs.GetQuest().GetObjectiveCount();
    }

    public QuestStatus GetQuestStatus()
    {
        return qs;
    }
}
