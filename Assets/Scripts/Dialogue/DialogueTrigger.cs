using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using RPG.Control;
using RPG.Movement;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class NodeTrigger
    {
        public string Name;
        public string DialogueText;
        public UnityEvent DialogueEvent;
        public NodeTrigger(string Name, string DialogueText)
        {
            this.Name = Name;
            this.DialogueText = DialogueText;
        }
    }

    public class DialogueTrigger : MonoBehaviour, IRaycastable
    {
        [SerializeField]
        DialogueContainer dialogue;
        [SerializeField]
        CursorType cursor;
        [SerializeField]
        float DistanceToInteract;
        [SerializeField]
        DialogueTriggerEvent trigger;

        [System.Serializable]
        public class DialogueTriggerEvent : UnityEvent<DialogueContainer, List<NodeTrigger>> { }

        public List<NodeTrigger> nodes = null;
        bool cantTalk = false;

        public CursorType GetCursorType()
        {
            return cursor;
        }

        public void disable()
        {
            cantTalk = true;
        }

        public bool HandleRaycast(PlayerController callingCon)
        {
            if (cantTalk) return false;
            if (Vector3.Distance(transform.position, callingCon.transform.position) > DistanceToInteract) return false;
            //if (!callingCon.GetComponent<Move>().CanMoveTo(transform.position)) return false;

            if (Input.GetMouseButtonDown(0))
            {
               trigger.Invoke(dialogue, nodes);
            }

            return true;
        }

        private void OnValidate()
        {
            if (dialogue == null) return;
            if (nodes.Count == dialogue.NodeLinks.Count)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Name != dialogue.NodeLinks[i].TargetNodeGuid || nodes[i].DialogueText != dialogue.NodeLinks[i].PortName) break;
                    if (i == nodes.Count - 1) return;
                }
            }

            nodes.Clear();
            foreach (NodeLinkData linkData in dialogue.NodeLinks)
            {
                NodeTrigger node = new NodeTrigger(linkData.TargetNodeGuid, linkData.PortName);
                nodes.Add(node);
            }
        }


    }

}