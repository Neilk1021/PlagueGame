using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;

public class QuestListUI : MonoBehaviour
{
    [SerializeField] QuestItemUI questPrefab;
    QuestList questList;
    List<GameObject> objects = new List<GameObject>();


    private void Start()
    {
        questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        questList.onUpdate += ReloadUI;
        ReloadUI();
    }

    private void ReloadUI()
    {
        foreach(GameObject object_ in objects)
        {
            Destroy(object_);
        }
        objects.Clear();

        QuestList ql = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        foreach (QuestStatus quest in ql.GetStatus())
        {
            QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, transform);
            objects.Add(uiInstance.gameObject);
            uiInstance.Init(quest);
        }
    }
}
