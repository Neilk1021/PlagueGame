#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
#endif

namespace RPG.Questing
{
#if UNITY_EDITOR
    public class QuestGraph : EditorWindow
    {
        public QuestGraphView _graphView;
        private string _fileName = "New Quest";

        [MenuItem("Graph/Quest Graph")]
        public static void OpenQuestGraphWindow()
        {
            var window = GetWindow<QuestGraph>();
            window.titleContent = new GUIContent("Quest Graph");

        }

        private void OnEnable()
        {
            ConstructGraph();
            GenerateToolBar();
            GenerateMiniMap();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        private void ConstructGraph()
        {
            _graphView = new QuestGraphView
            {
                name = "Quest Graph"
            };

            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolBar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

            var nodeCreateButton = new Button(() =>
            {
                _graphView.CreateNode("Quest Node");
            });
            nodeCreateButton.text = "Create Node";
            toolbar.Add(nodeCreateButton);

            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid title name", "Please enter a valid name.", "ok");
                return;
            }

            var saveUtility = QuestSaveUtil.GetInstance(_graphView);

            if (save)
                saveUtility.SaveGraph(_fileName);
            else
                saveUtility.LoadGraph(_fileName);
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap() { anchored = true };
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            _graphView.Add(miniMap);
        }
    }
#endif
}
