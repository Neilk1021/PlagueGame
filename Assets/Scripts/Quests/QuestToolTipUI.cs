using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Quests;

public class QuestToolTipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Transform objectiveContainer;
    [SerializeField] GameObject objectivePrefab;
    [SerializeField] GameObject objectiveIncompletePrefab;
    public void Init(QuestStatus quest)
    {
        title.text = quest.GetQuest().GetTitle();

        foreach (string item in quest.GetQuest().GetObjectives())
        {
            GameObject prefab = objectiveIncompletePrefab;
            if (quest.IsObjectiveComplete(item))
            {
                prefab = objectivePrefab;
            }
  
            TextMeshProUGUI text = Instantiate(prefab, objectiveContainer).GetComponentInChildren<TextMeshProUGUI>();
            text.text = item;
        }

    }
}
