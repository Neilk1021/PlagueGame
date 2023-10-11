using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Questing;

public class QuestUI : MonoBehaviour
{
    [SerializeField] GameObject detail;
    [SerializeField] TextMeshProUGUI Description;
    QuestContainer questContainer; 

    string QuestDesc;
    string QuestName;

    void Initialize(QuestContainer qc)
    {
        this.questContainer = qc;
        QuestName = questContainer.name;
    }
    public void changeVisibility()
    {
        if(detail.activeInHierarchy == true)
        {
            detail.SetActive(false);
            return;
        }

        detail.SetActive(true);
        Description.text = QuestDesc;
    }
}
