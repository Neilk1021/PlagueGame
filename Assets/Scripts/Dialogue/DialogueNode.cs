#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
#endif
namespace RPG.Dialogue
{
#if UNITY_EDITOR
    public class DialogueNode : Node
    {
        public string GUID;

        public string DialogueText;
        public string CharacterName;

        public Sprite CharacterPortrait;

        public bool EntryPoint = false;
    }
#endif
}