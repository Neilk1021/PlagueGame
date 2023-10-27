using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Quests.UI
{
    public class QuestUIContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        QuestTooltipSpawner qt;

        public void OnPointerEnter(PointerEventData eventData)
        {
            QuestStatus quest = GetComponent<QuestItemUI>().GetQuestStatus();
            qt.spawnQt(quest);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            qt.destroyQt();
        }

        void Awake()
        {
            qt = gameObject.GetComponentInParent<QuestTooltipSpawner>();
        }

    }

}