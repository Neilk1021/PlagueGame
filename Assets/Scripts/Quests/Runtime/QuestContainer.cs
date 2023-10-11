using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestContainer : ScriptableObject
{
    public List<QuestLinkData> NodeLinks = new List<QuestLinkData>();
    public List<QuestNodeData> QuestNodeData = new List<QuestNodeData>();
}
