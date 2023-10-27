using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;

namespace RPG.Quests.UI
{
    public class QuestTooltipSpawner : MonoBehaviour
    {
        public GameObject QtPrefab;
        GameObject activePrefab;

        public void spawnQt(QuestStatus quest)
        {
            if (activePrefab != null)
            {
                Destroy(activePrefab);
            }
            
            activePrefab = Instantiate(QtPrefab, transform);
            activePrefab.transform.position = activePrefab.transform.position + new Vector3(1200, 0, 0);
            StartCoroutine("loadUI");

            activePrefab.GetComponent<QuestToolTipUI>().Init(quest);
        }

        public void destroyQt()
        {
            if (activePrefab != null)
            {
                Destroy(activePrefab);
            }
        }

        IEnumerator loadUI()
        {
            yield return new WaitForSecondsRealtime(0.025f);
            if(activePrefab!=null)
            activePrefab.transform.position = activePrefab.transform.position + new Vector3(-1200, 0, 0);
        }
    }

}