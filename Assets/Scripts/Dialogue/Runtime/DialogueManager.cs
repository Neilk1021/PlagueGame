using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPG.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] Image CharSprite;
        [SerializeField] GameObject Frame;
        [SerializeField] GameObject DialogueCont;
        [SerializeField] TextMeshProUGUI DialogueBox, NameBox;
        [SerializeField] Vector2[] DialogueBoxPos;
        [SerializeField] Vector2[] NameBoxPos;
        [SerializeField] Button[] Choices; 

        static NodeLinkData currentLink;
        static DialogueNodeData currentNode;
        static string CurrentGuid;

        List<NodeLinkData> LinkChoices;
        List<NodeTrigger> nodes;

        [SerializeField]DialogueContainer currentDialogue;

        private static bool isStart(NodeLinkData nodeLinkData)
        {
            if(nodeLinkData.PortName == "Next")
            {
                currentLink = nodeLinkData;
                return true;
            }
            return false;
        }

        private static bool isChoice(NodeLinkData nodeLinkData)
        {
            return nodeLinkData.BaseNodeGuid == currentNode.GUID;
        }

        private static bool isNextNode(DialogueNodeData nodeLinkData)
        {
            if (nodeLinkData.GUID == CurrentGuid)
            {
                currentNode = nodeLinkData;
                return true;
            }
            return false;
        }

        private static bool isNode(DialogueNodeData dialogueNodeData)
        {
            if(dialogueNodeData.GUID == currentLink.TargetNodeGuid)
            {
                currentNode = dialogueNodeData;
                return true;
            }
            return false;
        }

        void ResetButtons()
        {
            for (int i = 0; i < Choices.Length; i++)
            {
                Choices[i].gameObject.SetActive(false);
                Choices[i].onClick.RemoveAllListeners();
            }
        }

        public void makeChoice(string GUID)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Name != GUID) continue;
                nodes[i].DialogueEvent?.Invoke();
            }

            CurrentGuid = GUID;
            currentDialogue.DialogueNodeData.Find(isNextNode);
            LinkChoices = currentDialogue.NodeLinks.FindAll(isChoice);
            UpdateDialogue();
        }

        public void UpdateDialogue()
        {
            DialogueBox.text = currentNode.DialogueText;
            NameBox.text = currentNode.CharacterName;
            if (currentNode.CharacterPortrait != null) {
                Frame.SetActive(true);
                CharSprite.sprite = currentNode.CharacterPortrait;
                DialogueBox.rectTransform.position = DialogueBoxPos[0];
                NameBox.rectTransform.position = NameBoxPos[0];
            }
            else {
                Frame.SetActive(false);
                DialogueBox.rectTransform.anchoredPosition = DialogueBoxPos[1];
                NameBox.rectTransform.anchoredPosition = NameBoxPos[1];
            }

            ResetButtons();
            for (int i = 0; i < LinkChoices.Count; i++)
            {
                string GUID = LinkChoices[i].TargetNodeGuid;
                Choices[i].gameObject.SetActive(true);
                Choices[i].GetComponentInChildren<TextMeshProUGUI>().text = LinkChoices[i].PortName;
                Choices[i].onClick.AddListener(() =>
                {
                    makeChoice(GUID);
                });
            }

            if(LinkChoices.Count <= 0)
            {
                Choices[0].gameObject.SetActive(true);
                Choices[0].GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
                Choices[0].onClick.AddListener(() =>
                {
                    DialogueCont.SetActive(false);
                });
            }
        }

        public void RunDialogue(DialogueContainer dialogueContainer, List<NodeTrigger> nodes)
        {
            this.nodes = nodes;
            DialogueCont.SetActive(true);
            currentDialogue = dialogueContainer;
            dialogueContainer.NodeLinks.Find(isStart);
            dialogueContainer.DialogueNodeData.Find(isNode);
            LinkChoices = dialogueContainer.NodeLinks.FindAll(isChoice);
            UpdateDialogue();
        }
    }
}
