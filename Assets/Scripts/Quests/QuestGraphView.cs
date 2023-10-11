#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

namespace RPG.Questing
{
#if UNITY_EDITOR
    public class QuestGraphView : GraphView
    {
        public readonly Vector2 defaultNodeSize = new Vector2(150, 200);

        public QuestGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryNode());
        }

        private Port GeneratePort(QuestNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        public void CreateNode(string nodeName)
        {
            AddElement(CreateQuestNode(nodeName));
        }

        private QuestNode GenerateEntryNode()
        {
            var node = new QuestNode
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                Description = "EntryPoint",
                EntryPoint = true
            };

            var generatedPort = GeneratePort(node, Direction.Output);
            generatedPort.portName = "Next";
            node.outputContainer.Add(generatedPort);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, 100, 150));

            return node;
        }

        public QuestNode CreateQuestNode(string NodeName, Sprite sprite = null)
        {
            var QuestNode = new QuestNode
            {
                title = NodeName,
                Icon = sprite,
                Description = NodeName,
                GUID = Guid.NewGuid().ToString()
            };

            var inputPort = GeneratePort(QuestNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            QuestNode.inputContainer.Add(inputPort);

            QuestNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            var button = new Button(() => { AddChoicePort(QuestNode); });
            button.text = "New Choice";
            QuestNode.titleContainer.Add(button);

            var QuestField = new TextField("Quest Desc");
            QuestField.value = NodeName;
            QuestField.RegisterValueChangedCallback(evt =>
            {
                QuestNode.Description = evt.newValue;
                QuestNode.title = evt.newValue;
            });

            var SpriteField = new ObjectField("Icon");
            SpriteField.objectType = typeof(Sprite);
            SpriteField.value = sprite;
            SpriteField.RegisterValueChangedCallback(evt =>
            {
                QuestNode.Icon = (Sprite)evt.newValue;
            });

            QuestNode.inputContainer.Add(QuestField);
            QuestNode.inputContainer.Add(SpriteField);

            QuestNode.RefreshExpandedState();
            QuestNode.RefreshPorts();

            QuestNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

            return QuestNode;
        }

        public void AddChoicePort(QuestNode questNode, string overriddenPortName = "")
        {
            var generatedPort = GeneratePort(questNode, Direction.Output);

            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);


            var outPortCount = questNode.outputContainer.Query("connector").ToList().Count;

            var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outPortCount + 1}" : overriddenPortName;

            var textField = new TextField
            {
                name = string.Empty,
                value = choicePortName
            };

            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);

            var deleteButton = new Button(() => RemovePort(questNode, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);

            generatedPort.portName = choicePortName;
            questNode.outputContainer.Add(generatedPort);

            questNode.RefreshPorts();
            questNode.RefreshExpandedState();
        }

        private void RemovePort(QuestNode questNode, Port generatedPort)
        {
            var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

            if (!targetEdge.Any())
            {
                questNode.outputContainer.Remove(generatedPort);
                questNode.RefreshPorts();
                questNode.RefreshExpandedState();
                return;
            }

            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());

            questNode.outputContainer.Remove(generatedPort);
            questNode.RefreshPorts();
            questNode.RefreshExpandedState();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                if (startPort != port && startPort.node != port.node)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }
    }
#endif
}
