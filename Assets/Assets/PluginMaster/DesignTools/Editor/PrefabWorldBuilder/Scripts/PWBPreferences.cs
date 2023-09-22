/*
Copyright (c) 2022 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2022.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEngine;
using System.Linq;

namespace PluginMaster
{
    public class PWBPreferences : UnityEditor.EditorWindow
    {
        #region COMMON
        private int _tab = 0;
        private Vector2 _mainScrollPosition = Vector2.zero;
        [UnityEditor.MenuItem("Tools/Plugin Master/Prefab World Builder/Preferences...", false, 1250)]
        public static void ShowWindow() => GetWindow<PWBPreferences>("PWB Preferences");

        private void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                _tab = GUILayout.Toolbar(_tab, new string[] { "General", "Shortcuts" });
                GUILayout.FlexibleSpace();
            }
            using (var scrollView = new UnityEditor.EditorGUILayout.ScrollViewScope(_mainScrollPosition,
                false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, UnityEditor.EditorStyles.helpBox))
            {
                _mainScrollPosition = scrollView.scrollPosition;
                if (_tab == 0) GeneralSettings();
                else Shortcuts();
            }
            UpdateCombination();
        }
        #endregion

        #region GENERAL SETTINGS
        private bool _undoGroupOpen = true;
        private bool _dataGroupOpen = true;
        private bool _autoSaveGroupOpen = true;
        private bool _unsavedChangesGroupOpen = true;
        private bool _gizmosGroupOpen = true;
        private bool _toolbarGroupOpen = true;
        private bool _pinToolGroupOpen = true;
        private bool _thumbnailsGroupOpen = true;

        private void GeneralSettings()
        {
            _dataGroupOpen
                = UnityEditor.EditorGUILayout.BeginFoldoutHeaderGroup(_dataGroupOpen, "Data Settings");
            if (_dataGroupOpen) DataGroup();
            UnityEditor.EditorGUILayout.EndFoldoutHeaderGroup();

            _autoSaveGroupOpen
                = UnityEditor.EditorGUILayout.BeginFoldoutHeaderGroup(_autoSaveGroupOpen, "Auto-Save Settings");
            if (_autoSaveGroupOpen) AutoSaveGroup();
            UnityEditor.EditorGUILayout.EndFoldoutHeaderGroup();

            _undoGroupOpen = UnityEditor.EditorGUILayout.BeginFoldoutHeaderGroup(_undoGroupOpen, "Undo Settings");
            if (_undoGroupOpen) UndoGroup();
            UnityEditor.EditorGUILayout.EndFoldoutHeaderGroup();

            _unsavedChangesGroupOpen = UnityEditor.EditorGUILayout.BeginFoldoutHeaderGroup(_unsavedChangesGroupOpen,
                "Unsaved Changes");
            if (_unsavedChangesGroupOpen) UnsavedChangesGroup();
            UnityEditor.EditorGUILayout.EndFoldoutHeaderGroup();

            _gizmosGroupOpen = UnityEditor.EditorGUILayout.BeginFoldoutHeaderGroup(_gizmosGroupOpen, "Gizmos");
            if (_gizmosGroupOpen) GizmosGroup();
            UnityEditor.EditorGUILayout.EndFoldoutHeaderGroup();

            _toolbarGroupOpen = UnityEditor.EditorGUILayout.BeginFoldoutHeaderGroup(_toolbarGroupOpen, "Toolbar");
            if (_toolbarGroupOpen) ToolbarGroup();
            UnityEditor.EditorGUILayout.EndFoldoutHeaderGroup();

            _pinToolGroupOpen = UnityEditor.EditorGUILayout.BeginFoldoutHeaderGroup(_pinToolGroupOpen, "Pin Tool");
            if (_pinToolGroupOpen) PinToolGroup();
            UnityEditor.EditorGUILayout.EndFoldoutHeaderGroup();

            _thumbnailsGroupOpen = UnityEditor.EditorGUILayout.BeginFoldoutHeaderGroup(_thumbnailsGroupOpen, "Thumnails");
            if (_thumbnailsGroupOpen) ThumbnailsGroup();
            UnityEditor.EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DataGroup()
        {
            using (new GUILayout.HorizontalScope(UnityEditor.EditorStyles.helpBox))
            {
                UnityEditor.EditorGUIUtility.labelWidth = 90;
                UnityEditor.EditorGUILayout.LabelField("Data directory:"
                    , PWBSettings.dataDir, UnityEditor.EditorStyles.textField);
                if (GUILayout.Button("...", GUILayout.Width(29), GUILayout.Height(20)))
                {
                    var directory = UnityEditor.EditorUtility.OpenFolderPanel("Select data directory...",
                    PWBSettings.dataDir, "Data");
                    if (System.IO.Directory.Exists(directory)) PWBSettings.dataDir = directory;
                }
            }
        }

        private void AutoSaveGroup()
        {
            using (new GUILayout.HorizontalScope(UnityEditor.EditorStyles.helpBox))
            {
                GUILayout.Label("Auto-Save Every:");
                PWBCore.staticData.autoSavePeriodMinutes
                    = UnityEditor.EditorGUILayout.IntSlider(PWBCore.staticData.autoSavePeriodMinutes, 1, 10);
                GUILayout.Label("minutes");
                GUILayout.FlexibleSpace();
            }
        }

        private void UndoGroup()
        {
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                {
                    PWBCore.staticData.undoBrushProperties
                        = UnityEditor.EditorGUILayout.ToggleLeft("Undo Brush properties changes",
                        PWBCore.staticData.undoBrushProperties);
                    if (check.changed && !PWBCore.staticData.undoBrushProperties) BrushProperties.ClearUndo();
                }
                using (var check = new UnityEditor.EditorGUI.ChangeCheckScope())
                {
                    PWBCore.staticData.undoPalette = UnityEditor.EditorGUILayout.ToggleLeft("Undo Palette changes",
                        PWBCore.staticData.undoPalette);
                    if (check.changed && !PWBCore.staticData.undoPalette) PrefabPalette.ClearUndo();
                }
            }
        }

        private static readonly string[] _unsavedChangesActionNames = { "Ask if want to save", "Save", "Discard" };
        private void UnsavedChangesGroup()
        {
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                UnityEditor.EditorGUIUtility.labelWidth = 45;
                PWBCore.staticData.unsavedChangesAction = (PWBData.UnsavedChangesAction)
                    UnityEditor.EditorGUILayout.Popup("Action:",
                    (int)PWBCore.staticData.unsavedChangesAction, _unsavedChangesActionNames);
            }
        }

        private void GizmosGroup()
        {
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                UnityEditor.EditorGUIUtility.labelWidth = 110;
                PWBCore.staticData.controPointSize = UnityEditor.EditorGUILayout.IntSlider("Control Point Size:",
                    PWBCore.staticData.controPointSize, 1, 3);

            }
        }

        private void ToolbarGroup()
        {
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                PWBCore.staticData.closeAllWindowsWhenClosingTheToolbar
                        = UnityEditor.EditorGUILayout.ToggleLeft("Close all windows when closing the toolbar",
                        PWBCore.staticData.closeAllWindowsWhenClosingTheToolbar);
            }
        }

        private void PinToolGroup()
        {
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                UnityEditor.EditorGUIUtility.labelWidth = 150;
                PinManager.rotationSnapValue = UnityEditor.EditorGUILayout.Slider("Rotation snap value (Deg):",
                    PinManager.rotationSnapValue, 0f, 360f);
            }
        }

        private void ThumbnailsGroup()
        {
            using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
            {
                PWBCore.staticData.thumbnailLayer = UnityEditor.EditorGUILayout.IntField("Thumbnail Layer:",
                    PWBCore.staticData.thumbnailLayer);
            }
        }
        #endregion

        #region SHORTCUTS
        private bool _pinCategory = true;
        private bool _brushCategory = false;
        private bool _gravityCategory = false;
        private bool _lineCategory = false;
        private bool _shapeCategory = false;
        private bool _tilingCategory = false;
        private bool _selectionCategory = false;
        private bool _gridCategory = false;

        private PWBShortcut _selectedShortcut = null;
        private static Texture2D _warningTexture = null;
        private static Texture2D warningTexture
        {
            get
            {
                if (_warningTexture == null) _warningTexture = Resources.Load<Texture2D>("Sprites/Warning");
                return _warningTexture;
            }
        }

        private UnityEditor.IMGUI.Controls.MultiColumnHeaderState _multiColumnHeaderState;
        private UnityEditor.IMGUI.Controls.MultiColumnHeader _multiColumnHeader;
        private UnityEditor.IMGUI.Controls.MultiColumnHeaderState.Column[] _columns;

        private void InitializeMultiColumn()
        {
            _columns = new UnityEditor.IMGUI.Controls.MultiColumnHeaderState.Column[]
            {
                new UnityEditor.IMGUI.Controls.MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = false,
                    autoResize = true,
                    minWidth = 200,
                    maxWidth = 400,
                    width = 300,
                    canSort = false,
                    headerContent = new GUIContent("Command"),
                    headerTextAlignment = TextAlignment.Left,
                },
                new UnityEditor.IMGUI.Controls.MultiColumnHeaderState.Column()
                {
                    allowToggleVisibility = false,
                    autoResize = true,
                    minWidth = 80,
                    maxWidth = 200,
                    width = 100,
                    canSort = false,
                    headerContent = new GUIContent("Shortcut"),
                    headerTextAlignment = TextAlignment.Left,
                }
            };
            _multiColumnHeaderState = new UnityEditor.IMGUI.Controls.MultiColumnHeaderState(columns: _columns);
            _multiColumnHeader = new UnityEditor.IMGUI.Controls.MultiColumnHeader(state: _multiColumnHeaderState);
            _multiColumnHeader.visibleColumnsChanged += (multiColumnHeader) => multiColumnHeader.ResizeToFit();
            _multiColumnHeader.ResizeToFit();
        }

        private static readonly Color _lighterColor = Color.white * 0.3f;
        private static readonly Color _darkerColor = Color.white * 0.1f;

        private void SelectProfileItem(object value)
        {
            PWBSettings.selectedProfileIdx = (int)value;
            Repaint();
        }

        private void Shortcuts()
        {
            if (_multiColumnHeader == null) InitializeMultiColumn();
            void SelectCategory(ref bool category)
            {
                _gridCategory = false;
                _pinCategory = false;
                _brushCategory = false;
                _gravityCategory = false;
                _lineCategory = false;
                _shapeCategory = false;
                _tilingCategory = false;
                _selectionCategory = false;
                category = true;
            }
            string shortcutString(PWBShortcut shortcut)
            {
                if ((object)shortcut == (object)_selectedShortcut) return string.Empty;
                return shortcut.keyCombination.ToString();
            }
            GUIStyle shortcutStyle(PWBShortcut shortcut)
            {
                if ((object)shortcut == (object)_selectedShortcut) return UnityEditor.EditorStyles.textField;
                return UnityEditor.EditorStyles.label;
            }

            var categoryButton = new GUIStyle(UnityEditor.EditorStyles.toolbarButton);
            categoryButton.alignment = TextAnchor.UpperLeft;

            using (new GUILayout.HorizontalScope(UnityEditor.EditorStyles.helpBox))
            {
                GUILayout.Label("Profile:");
                if (GUILayout.Button(PWBSettings.shortcuts.profileName,
                    UnityEditor.EditorStyles.popup, GUILayout.MinWidth(100)))
                {
                    GUI.FocusControl(null);
                    var menu = new UnityEditor.GenericMenu();
                    var profileNames = PWBSettings.shotcutProfileNames;
                    for (int i = 0; i < profileNames.Length; ++i)
                        menu.AddItem(new GUIContent(profileNames[i]),
                            PWBSettings.selectedProfileIdx == i, SelectProfileItem, i);
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("Factory Reset Selected Profile"), false, PWBSettings.ResetSelectedProfile);
                    menu.ShowAsContext();
                }
                GUILayout.FlexibleSpace();
            }

            using (new GUILayout.HorizontalScope())
            {
                const int categoryColumnW = 100;
                using (new GUILayout.VerticalScope(GUILayout.Width(categoryColumnW)))
                {
                    if (GUILayout.Toggle(_pinCategory, "Pin", categoryButton)) SelectCategory(ref _pinCategory);
                    if (GUILayout.Toggle(_brushCategory, "Brush", categoryButton)) SelectCategory(ref _brushCategory);
                    if (GUILayout.Toggle(_gravityCategory, "Gravity", categoryButton)) SelectCategory(ref _gravityCategory);
                    if (GUILayout.Toggle(_lineCategory, "Line", categoryButton)) SelectCategory(ref _lineCategory);
                    if (GUILayout.Toggle(_shapeCategory, "Shape", categoryButton)) SelectCategory(ref _shapeCategory);
                    if (GUILayout.Toggle(_tilingCategory, "Tiling", categoryButton)) SelectCategory(ref _tilingCategory);
                    if (GUILayout.Toggle(_selectionCategory, "Selection", categoryButton))
                        SelectCategory(ref _selectionCategory);
                    if (GUILayout.Toggle(_gridCategory, "Grid", categoryButton)) SelectCategory(ref _gridCategory);
                }
                GUILayout.Space(2);
                using (new GUILayout.VerticalScope())
                {
                    var minX = categoryColumnW + 10;
                    var shorcutPanelRect = new Rect(minX, 28, position.width - categoryColumnW - 20, position.height);

                    float columnHeight = UnityEditor.EditorGUIUtility.singleLineHeight;
                    Rect columnRectPrototype = new Rect(shorcutPanelRect) { height = columnHeight };

                    _multiColumnHeader.OnGUI(rect: columnRectPrototype, xScroll: 0.0f);

                    int row = 0;
                    void ShortcutRow(PWBShortcut shortcut, PWBShortcut prevShortcut = null)
                    {
                        Rect rowRect = new Rect(columnRectPrototype);

                        rowRect.y += columnHeight * (++row);
                        UnityEditor.EditorGUI.DrawRect(rowRect, row % 2 == 0 ? _darkerColor : _lighterColor);

                        Rect columnRect = _multiColumnHeader.GetColumnRect(0);
                        columnRect.y = rowRect.y;

                        var cellRect = _multiColumnHeader.GetCellRect(0, columnRect);
                        cellRect.x += minX;
                        UnityEditor.EditorGUI.LabelField(cellRect, new GUIContent(shortcut.name));

                        ////////////////
                        columnRect = _multiColumnHeader.GetColumnRect(1);
                        columnRect.y = rowRect.y;

                        cellRect = _multiColumnHeader.GetCellRect(1, columnRect);
                        cellRect.x += minX;
                        cellRect.width -= 20;
                        UnityEditor.EditorGUI.LabelField(cellRect, new GUIContent(shortcutString(shortcut)),
                            shortcutStyle(shortcut));

                        if (cellRect.Contains(Event.current.mousePosition)
                            && Event.current.type == EventType.MouseDown)
                        {
                            if (Event.current.button == 0)
                            {
                                _selectedShortcut = shortcut;
                                Repaint();
                            }
                            else if (Event.current.button == 1)
                            {
                                void ResetToDefault()
                                {
                                    PWBSettings.ResetShortcutToDefault(shortcut);
                                    PWBSettings.UpdateShrotcutsConflictsAndSaveFile();
                                }
                                void Remove()
                                {
                                    shortcut.keyCombination.SetCombination(KeyCode.None);
                                    PWBSettings.UpdateShrotcutsConflictsAndSaveFile();
                                }
                                var menu = new UnityEditor.GenericMenu();
                                menu.AddItem(new GUIContent("Reset to default"), false, ResetToDefault);
                                menu.AddItem(new GUIContent("Disable shortcut"), false, Remove);
                                menu.ShowAsContext();
                            }
                        }
                        if (!shortcut.conflicted) return;
                        cellRect.x += cellRect.width;
                        cellRect.width = 20;
                        UnityEditor.EditorGUI.LabelField(cellRect, new GUIContent(warningTexture));
                    }

                    void EditModeRows()
                    {
                        ShortcutRow(PWBSettings.shortcuts.editModeToggle);
                        ShortcutRow(PWBSettings.shortcuts.editModeDeleteItemAndItsChildren);
                        ShortcutRow(PWBSettings.shortcuts.editModeDeleteItemButNotItsChildren);
                        ShortcutRow(PWBSettings.shortcuts.editModeSelectParent);
                    }
                    void SelectionRows()
                    {
                        ShortcutRow(PWBSettings.shortcuts.selectionRotate90XCW);
                        ShortcutRow(PWBSettings.shortcuts.selectionRotate90XCCW);
                        ShortcutRow(PWBSettings.shortcuts.selectionRotate90YCW);
                        ShortcutRow(PWBSettings.shortcuts.selectionRotate90YCCW);
                        ShortcutRow(PWBSettings.shortcuts.selectionRotate90ZCW);
                        ShortcutRow(PWBSettings.shortcuts.selectionRotate90ZCCW);
                    }
                    
                    if (_pinCategory)
                    {
                        ShortcutRow(PWBSettings.shortcuts.pinMoveHandlesUp);
                        ShortcutRow(PWBSettings.shortcuts.pinMoveHandlesDown);
                        ShortcutRow(PWBSettings.shortcuts.pinSelectNextHandle);
                        ShortcutRow(PWBSettings.shortcuts.pinSelectPivotHandle);
                        ShortcutRow(PWBSettings.shortcuts.pinToggleRepeatItem);
                        ShortcutRow(PWBSettings.shortcuts.pinResetScale);

                        ShortcutRow(PWBSettings.shortcuts.pinRotate90YCW);
                        ShortcutRow(PWBSettings.shortcuts.pinRotate90YCCW);
                        ShortcutRow(PWBSettings.shortcuts.pinRotateAStepYCW);
                        ShortcutRow(PWBSettings.shortcuts.pinRotateAStepYCCW);

                        ShortcutRow(PWBSettings.shortcuts.pinRotate90XCW);
                        ShortcutRow(PWBSettings.shortcuts.pinRotate90XCCW);
                        ShortcutRow(PWBSettings.shortcuts.pinRotateAStepXCW);
                        ShortcutRow(PWBSettings.shortcuts.pinRotateAStepXCCW);

                        ShortcutRow(PWBSettings.shortcuts.pinRotate90ZCW);
                        ShortcutRow(PWBSettings.shortcuts.pinRotate90ZCCW);
                        ShortcutRow(PWBSettings.shortcuts.pinRotateAStepZCW);
                        ShortcutRow(PWBSettings.shortcuts.pinRotateAStepZCCW);

                        ShortcutRow(PWBSettings.shortcuts.pinResetRotation);

                        ShortcutRow(PWBSettings.shortcuts.pinAdd1UnitToSurfDist);
                        ShortcutRow(PWBSettings.shortcuts.pinSubtract1UnitFromSurfDist);
                        ShortcutRow(PWBSettings.shortcuts.pinAdd01UnitToSurfDist);
                        ShortcutRow(PWBSettings.shortcuts.pinSubtract01UnitFromSurfDist);

                        ShortcutRow(PWBSettings.shortcuts.pinResetSurfDist);
                    }
                    else if (_brushCategory)
                    {
                        ShortcutRow(PWBSettings.shortcuts.brushUpdatebrushstroke);
                    }
                    else if (_gravityCategory)
                    {
                        ShortcutRow(PWBSettings.shortcuts.brushUpdatebrushstroke);

                        ShortcutRow(PWBSettings.shortcuts.pinAdd1UnitToSurfDist);
                        ShortcutRow(PWBSettings.shortcuts.pinSubtract1UnitFromSurfDist);
                        ShortcutRow(PWBSettings.shortcuts.pinAdd01UnitToSurfDist);
                        ShortcutRow(PWBSettings.shortcuts.pinSubtract01UnitFromSurfDist);
                    }
                    else if (_lineCategory)
                    {
                        ShortcutRow(PWBSettings.shortcuts.lineSelectAllPoints);
                        ShortcutRow(PWBSettings.shortcuts.lineDeselectAllPoints);
                        ShortcutRow(PWBSettings.shortcuts.lineToggleCurve);
                        ShortcutRow(PWBSettings.shortcuts.lineToggleClosed);
                        EditModeRows();
                    }
                    else if (_shapeCategory)
                    {
                        EditModeRows();
                    }
                    else if (_tilingCategory)
                    {
                        SelectionRows();
                        EditModeRows();
                    }
                    else if (_selectionCategory)
                    {
                        ShortcutRow(PWBSettings.shortcuts.selectionTogglePositionHandle);
                        ShortcutRow(PWBSettings.shortcuts.selectionToggleRotationHandle);
                        ShortcutRow(PWBSettings.shortcuts.selectionToggleScaleHandle);
                        ShortcutRow(PWBSettings.shortcuts.selectionEditCustomHandle);
                        SelectionRows();
                    }
                    else if (_gridCategory)
                    {
                        ShortcutRow(PWBSettings.shortcuts.gridEnableShortcuts);
                        ShortcutRow(PWBSettings.shortcuts.gridToggle, PWBSettings.shortcuts.gridEnableShortcuts);
                        ShortcutRow(PWBSettings.shortcuts.gridToggleSnaping, PWBSettings.shortcuts.gridEnableShortcuts);
                        ShortcutRow(PWBSettings.shortcuts.gridToggleLock, PWBSettings.shortcuts.gridEnableShortcuts);
                        ShortcutRow(PWBSettings.shortcuts.gridSetOriginPosition, PWBSettings.shortcuts.gridEnableShortcuts);
                        ShortcutRow(PWBSettings.shortcuts.gridSetOriginRotation, PWBSettings.shortcuts.gridEnableShortcuts);
                        ShortcutRow(PWBSettings.shortcuts.gridSetSize, PWBSettings.shortcuts.gridEnableShortcuts);
                    }
                    GUILayout.Space((row + 2) * columnHeight);
                    if (_gridCategory)
                    {
                        UnityEditor.EditorGUILayout.HelpBox("These shortcuts work in two steps."
                        + "\nFirst you have to activate the shortcuts with "
                        + PWBSettings.shortcuts.gridEnableShortcuts.keyCombination
                        + ".\nFor example to toggle the grid you have to press "
                        + PWBSettings.shortcuts.gridEnableShortcuts.keyCombination + " and then "
                        + PWBSettings.shortcuts.gridToggle.keyCombination + ".",
                       UnityEditor.MessageType.Info);
                    }
                }
            }
        }

        private void UpdateCombination()
        {
            if (_selectedShortcut == null) return;
            if (Event.current == null) return;
            if (Event.current.type != EventType.KeyDown) return;
            if (Event.current.keyCode == KeyCode.Escape)
            {
                Repaint();
                _selectedShortcut = null;
                return;
            }
            if (Event.current.keyCode < KeyCode.Space || Event.current.keyCode > KeyCode.F15) return;
            var combi = new PWBKeyCombination(Event.current.keyCode, Event.current.modifiers);

            void SetCombination()
            {
                _selectedShortcut.keyCombination.SetCombination(Event.current.keyCode, Event.current.modifiers);
                PWBSettings.UpdateShrotcutsConflictsAndSaveFile();
            }
            if (PWBSettings.shortcuts.CheckConflicts(combi, _selectedShortcut, out string conflicts))
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Binding Conflict", "The key " + combi.ToString()
                    + " is already assigned to: \n" + conflicts + "\n Do you want to create the conflict?",
                    "Create Conflict", "Cancel"))
                    SetCombination();
            }
            else SetCombination();
            _selectedShortcut = null;
            Repaint();
        }
        #endregion
    }
}
