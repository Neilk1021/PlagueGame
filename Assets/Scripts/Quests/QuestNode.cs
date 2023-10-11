#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
#endif
namespace RPG.Questing
{
#if UNITY_EDITOR
    public class QuestNode : Node
    {
        public string GUID;

        public string Description;

        public Sprite Icon;

        public bool EntryPoint = false;
    }
#endif
}