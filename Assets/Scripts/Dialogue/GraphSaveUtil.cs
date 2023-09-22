#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System;
using UnityEngine.UIElements;
#endif

namespace RPG.Dialogue
{
    #if UNITY_EDITOR
    public class GraphSaveUtil
    {
        private DialogueGraphView _targetGraphView;
        private DialogueContainer _container;

        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

        public static GraphSaveUtil GetInstance(DialogueGraphView targetGraphView)
        {
            return new GraphSaveUtil
            {
                _targetGraphView = targetGraphView
            };
        }

        public void SaveGraph(string fileName)
        {
            if (!Edges.Any()) return;

            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            for (int i = 0; i < connectedPorts.Length; i++)
            {
                var outputNode = connectedPorts[i].output.node as DialogueNode;
                var inputNode = connectedPorts[i].input.node as DialogueNode;

                dialogueContainer.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.GUID,
                    TargetNodeGuid = inputNode.GUID,
                    PortName = connectedPorts[i].output.portName
                });
            }

            foreach (var dialogueNode in Nodes.Where(node=>!node.EntryPoint))
            {
                if(dialogueNode.CharacterName != null)
                Debug.Log(dialogueNode.CharacterName);
                dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
                {
                    GUID = dialogueNode.GUID,
                    DialogueText= dialogueNode.DialogueText,
                    Position = dialogueNode.GetPosition().position,
                    CharacterPortrait = dialogueNode.CharacterPortrait,
                    CharacterName = dialogueNode.CharacterName
                });
            }

            if (!AssetDatabase.IsValidFolder("Assets/Scripts/Dialogue/Resources/"))
                AssetDatabase.CreateFolder("Dialogue", "Resources");

            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Scripts/Dialogue/Resources/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            _container = Resources.Load<DialogueContainer>(fileName);
            if (_container == null)
            {
                EditorUtility.DisplayDialog("File not found", "Target dialogue graph file does not exist", "ok");
                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }

        private void ConnectNodes()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                var connections = _container.NodeLinks.Where(x => (x.BaseNodeGuid == Nodes[i].GUID)).ToList();
                for (int j = 0; j < connections.Count; j++)
                {
                    var targetNodeGuid = connections[j].TargetNodeGuid;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);

                    LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(_container.DialogueNodeData.First(x => x.GUID == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));

                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };

            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);

            _targetGraphView.Add(tempEdge);
        }

        private void CreateNodes()
        {
            foreach (var nodeData in _container.DialogueNodeData)
            {
                var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText, nodeData.CharacterName, nodeData.CharacterPortrait);
                tempNode.GUID = nodeData.GUID;
                _targetGraphView.AddElement(tempNode);

                var nodePorts = _container.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.GUID).ToList();
                nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ClearGraph()
        {
            Nodes.Find(x => x.EntryPoint).GUID = _container.NodeLinks[0].BaseNodeGuid;

            foreach (var node in Nodes)
            {
                if (node.EntryPoint) continue;
                Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

                _targetGraphView.RemoveElement(node);
            }
        }
    }
#endif
}
