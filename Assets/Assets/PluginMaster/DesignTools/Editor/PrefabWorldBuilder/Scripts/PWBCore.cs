/*
Copyright (c) 2020 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2020.

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
    #region CORE
    public static class PWBCore
    {
        public const string PARENT_COLLIDER_NAME = "PluginMasterPrefabPaintTempMeshColliders";
        private static GameObject _parentCollider = null;
        private static GameObject parentCollider
        {
            get
            {
                if (_parentCollider == null)
                {
                    _parentCollider = new GameObject(PWBCore.PARENT_COLLIDER_NAME);
                    _parentColliderId = _parentCollider.GetInstanceID();
                    _parentCollider.hideFlags = HideFlags.HideAndDontSave;
                }
                return _parentCollider;
            }
        }
        private static int _parentColliderId = -1;
        public static int parentColliderId => _parentColliderId;
        #region DATA
        private static PWBData _staticData = null;
        public static bool staticDataWasInitialized => _staticData != null;
        public static PWBData staticData
        {
            get
            {
                if (_staticData != null) return _staticData;
                _staticData = new PWBData();
                return _staticData;
            }
        }

        public static void LoadFromFile()
        {
            var text = PWBData.ReadDataText();
            if (text == null)
            {
                _staticData = new PWBData();
                _staticData.Save();
            }
            else
            {
                if (!ApplicationEventHandler.hierarchyLoaded) return;
                _staticData = JsonUtility.FromJson<PWBData>(text);
                foreach (var palette in PaletteManager.paletteData)
                    foreach (var brush in palette.brushes)
                        foreach (var item in brush.items) item.InitializeParentSettings(brush);
            }
        }

        public static void SetSavePending()
        {
            AutoSave.QuickSave();
            staticData.SetSavePending();
        }

        #endregion
        #region TEMP COLLIDERS
        private static System.Collections.Generic.Dictionary<int, GameObject> _tempCollidersIds
            = new System.Collections.Generic.Dictionary<int, GameObject>();
        private static System.Collections.Generic.Dictionary<int, GameObject> _tempCollidersTargets
            = new System.Collections.Generic.Dictionary<int, GameObject>();
        private static System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>
            _tempCollidersTargetParentsIds
            = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
        private static System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>
            _tempCollidersTargetChildrenIds
            = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
        public static bool CollidersContains(GameObject[] selection, string colliderName)
        {
            int objId;
            if (!int.TryParse(colliderName, out objId)) return false;
            foreach (var obj in selection)
                if (obj.GetInstanceID() == objId)
                    return true;
            return false;
        }

        public static bool IsTempCollider(int instanceId) => _tempCollidersIds.ContainsKey(instanceId);

        public static GameObject GetGameObjectFromTempColliderId(int instanceId)
        {
            if (_tempCollidersIds[instanceId] == null) UpdateTempColliders();
            if (!_tempCollidersIds.ContainsKey(instanceId)) return null;
            return _tempCollidersIds[instanceId];
        }

        public static void UpdateTempColliders()
        {
            DestroyTempColliders();
            PWBIO.UpdateOctree();
            var allTransforms = GameObject.FindObjectsOfType<Transform>();
            foreach (var transform in allTransforms)
            {
                if (!transform.gameObject.activeInHierarchy) continue;
                if (transform.parent != null) continue;
                AddTempCollider(transform.gameObject);
            }
        }

        public static void AddTempCollider(GameObject obj)
        {
            void AddParentsIds(GameObject target)
            {
                var parents = target.GetComponentsInParent<Transform>();
                foreach (var parent in parents)
                {
                    if (!_tempCollidersTargetParentsIds.ContainsKey(target.GetInstanceID()))
                        _tempCollidersTargetParentsIds.Add(target.GetInstanceID(), new System.Collections.Generic.List<int>());
                    _tempCollidersTargetParentsIds[target.GetInstanceID()].Add(parent.gameObject.GetInstanceID());
                    if (!_tempCollidersTargetChildrenIds.ContainsKey(parent.gameObject.GetInstanceID()))
                        _tempCollidersTargetChildrenIds.Add(parent.gameObject.GetInstanceID(),
                            new System.Collections.Generic.List<int>());
                    _tempCollidersTargetChildrenIds[parent.gameObject.GetInstanceID()].Add(target.GetInstanceID());
                }
            }

            void CreateTempCollider(GameObject target, Mesh mesh)
            {
                var differentVertices = new System.Collections.Generic.List<Vector3>();
                foreach (var vertex in mesh.vertices)
                {
                    if (!differentVertices.Contains(vertex)) differentVertices.Add(vertex);
                    if (differentVertices.Count >= 3) break;
                }
                if (differentVertices.Count < 3) return;
                if (_tempCollidersTargets.ContainsKey(target.GetInstanceID())) return;
                var name = target.GetInstanceID().ToString();
                var tempObj = new GameObject(name);
                tempObj.hideFlags = HideFlags.HideAndDontSave;
                _tempCollidersIds.Add(tempObj.GetInstanceID(), target);
                tempObj.transform.SetParent(parentCollider.transform);
                tempObj.transform.position = target.transform.position;
                tempObj.transform.rotation = target.transform.rotation;
                tempObj.transform.localScale = target.transform.lossyScale;
                _tempCollidersTargets.Add(target.GetInstanceID(), tempObj);
                AddParentsIds(target);

                MeshUtils.AddCollider(mesh, tempObj);
            }

            bool ObjectIsActiveAndWithoutCollider(GameObject go)
            {
                if (!go.activeInHierarchy) return false;
                var collider = go.GetComponent<Collider>();
                if (collider == null) return true;
                if (collider is MeshCollider)
                {
                    var meshCollider = collider as MeshCollider;
                    if (meshCollider.sharedMesh == null) return true;
                }
                return collider.isTrigger;
            }

            var meshFilters = obj.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                if (!ObjectIsActiveAndWithoutCollider(meshFilter.gameObject)) continue;
                CreateTempCollider(meshFilter.gameObject, meshFilter.sharedMesh);
            }

            var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinnedMeshRenderers)
            {
                if (!ObjectIsActiveAndWithoutCollider(renderer.gameObject)) continue;
                CreateTempCollider(renderer.gameObject, renderer.sharedMesh);
            }

            var spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                var target = spriteRenderer.gameObject;
                if (!target.activeInHierarchy) continue;
                if (_tempCollidersTargets.ContainsKey(target.GetInstanceID())) return;
                var name = spriteRenderer.gameObject.GetInstanceID().ToString();
                var tempObj = new GameObject(name);
                tempObj.hideFlags = HideFlags.HideAndDontSave;
                _tempCollidersIds.Add(tempObj.GetInstanceID(), spriteRenderer.gameObject);
                tempObj.transform.SetParent(parentCollider.transform);
                tempObj.transform.position = spriteRenderer.transform.position;
                tempObj.transform.rotation = spriteRenderer.transform.rotation;
                tempObj.transform.localScale = spriteRenderer.transform.lossyScale;
                _tempCollidersTargets.Add(target.GetInstanceID(), tempObj);
                AddParentsIds(target);
                var boxCollider = tempObj.AddComponent<BoxCollider>();
                boxCollider.size = (Vector3)(spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit)
                    + new Vector3(0f, 0f, 0.01f);
                var collider = spriteRenderer.GetComponent<Collider2D>();
                if (collider != null && !collider.isTrigger) continue;
                tempObj = new GameObject(name);
                tempObj.hideFlags = HideFlags.HideAndDontSave;
                _tempCollidersIds.Add(tempObj.GetInstanceID(), spriteRenderer.gameObject);
                tempObj.transform.SetParent(parentCollider.transform);
                tempObj.transform.position = spriteRenderer.transform.position;
                tempObj.transform.rotation = spriteRenderer.transform.rotation;
                tempObj.transform.localScale = spriteRenderer.transform.lossyScale;
                var boxCollider2D = tempObj.AddComponent<BoxCollider2D>();
                boxCollider2D.size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
            }
        }

        public static void DestroyTempCollider(int objId)
        {
            if (!_tempCollidersTargets.ContainsKey(objId)) return;
            var temCollider = _tempCollidersTargets[objId];
            if (temCollider == null) return;
            var tempId = temCollider.GetInstanceID();
            _tempCollidersIds.Remove(tempId);
            _tempCollidersTargets.Remove(objId);
            _tempCollidersTargetParentsIds.Remove(objId);
            Object.DestroyImmediate(temCollider);
        }
        public static void DestroyTempColliders()
        {
            _tempCollidersIds.Clear();
            _tempCollidersTargets.Clear();
            _tempCollidersTargetParentsIds.Clear();
            _tempCollidersTargetChildrenIds.Clear();
            var parentObj = GameObject.Find(PWBCore.PARENT_COLLIDER_NAME);
            if (parentObj != null) Object.DestroyImmediate(parentObj);
            _parentColliderId = -1;
        }


        public static void UpdateTempCollidersTransforms(GameObject[] objects)
        {
            foreach (var obj in objects)
            {
                var parentId = obj.GetInstanceID();
                bool isParent = false;
                foreach (var childId in _tempCollidersTargetParentsIds.Keys)
                {
                    var parentsIds = _tempCollidersTargetParentsIds[childId];
                    if (parentsIds.Contains(parentId))
                    {
                        isParent = true;
                        break;
                    }
                }
                if (!isParent) continue;
                foreach (var id in _tempCollidersTargetChildrenIds[parentId])
                {
                    var tempCollider = _tempCollidersTargets[id];
                    if (tempCollider == null) continue;
                    var childObj = (GameObject)UnityEditor.EditorUtility.InstanceIDToObject(id);
                    if (childObj == null) continue;
                    tempCollider.transform.position = childObj.transform.position;
                    tempCollider.transform.rotation = childObj.transform.rotation;
                    tempCollider.transform.localScale = childObj.transform.lossyScale;
                }
            }
        }

        public static void SetActiveTempColliders(GameObject[] objects, bool value)
        {
            foreach (var obj in objects)
            {
                if (!obj.activeInHierarchy) continue;
                var parentId = obj.GetInstanceID();
                bool isParent = false;
                foreach (var childId in _tempCollidersTargetParentsIds.Keys)
                {
                    var parentsIds = _tempCollidersTargetParentsIds[childId];
                    if (parentsIds.Contains(parentId))
                    {
                        isParent = true;
                        break;
                    }
                }
                if (!isParent) continue;
                foreach (var id in _tempCollidersTargetChildrenIds[parentId])
                {
                    var tempCollider = _tempCollidersTargets[id];
                    if (tempCollider == null) continue;
                    var childObj = (GameObject)UnityEditor.EditorUtility.InstanceIDToObject(id);
                    if (childObj == null) continue;
                    tempCollider.SetActive(value);
                    tempCollider.transform.position = childObj.transform.position;
                    tempCollider.transform.rotation = childObj.transform.rotation;
                    tempCollider.transform.localScale = childObj.transform.lossyScale;
                }
            }
        }

        public static GameObject[] GetTempColliders(GameObject obj)
        {
            var parentId = obj.GetInstanceID();
            bool isParent = false;
            foreach (var childId in _tempCollidersTargetParentsIds.Keys)
            {
                var parentsIds = _tempCollidersTargetParentsIds[childId];
                if (parentsIds.Contains(parentId))
                {
                    isParent = true;
                    break;
                }
            }
            if (!isParent) return null;
            var tempColliders = new System.Collections.Generic.List<GameObject>();
            foreach (var id in _tempCollidersTargetChildrenIds[parentId])
            {
                var tempCollider = _tempCollidersTargets[id];
                if (tempCollider == null) continue;
                tempColliders.Add(tempCollider);
            }
            return tempColliders.ToArray();
        }
        #endregion
    }

    [System.Serializable]
    public class PWBData
    {
        public const string DATA_DIR = "Data";
        public const string FILE_NAME = "PWBData";
        public const string FULL_FILE_NAME = FILE_NAME + ".txt";
        public const string RELATIVE_TOOL_DIR = "PluginMaster/DesignTools/Editor/PrefabWorldBuilder";
        public const string RELATIVE_RESOURCES_DIR = RELATIVE_TOOL_DIR + "/Resources";
        public const string RELATIVE_DATA_DIR = RELATIVE_RESOURCES_DIR + "/" + DATA_DIR;
        public const string PALETTES_DIR = "Palettes";
        public const string VERSION = "3.2";
        [SerializeField] private string _version = VERSION;
        [SerializeField] private string _rootDirectory = null;
        [SerializeField] private int _autoSavePeriodMinutes = 1;
        [SerializeField] private bool _undoBrushProperties = true;
        [SerializeField] private bool _undoPalette = true;
        [SerializeField] private int _controlPointSize = 1;
        [SerializeField] private bool _closeAllWindowsWhenClosingTheToolbar = false;
        [SerializeField] private int _thumbnailLayer = 7;
        public enum UnsavedChangesAction { ASK, SAVE, DISCARD }
        [SerializeField] private UnsavedChangesAction _unsavedChangesAction = UnsavedChangesAction.ASK;
        [SerializeField] private PaletteManager _paletteManager = PaletteManager.instance;

        [SerializeField] private PinManager pinManager = PinManager.instance as PinManager;
        [SerializeField] private BrushManager _brushManager = BrushManager.instance as BrushManager;
        [SerializeField] private GravityToolManager _gravityToolManager = GravityToolManager.instance as GravityToolManager;
        [SerializeField] private LineManager _lineManager = LineManager.instance as LineManager;
        [SerializeField] private ShapeManager _shapeManager = ShapeManager.instance as ShapeManager;
        [SerializeField] private TilingManager _tilingManager = TilingManager.instance as TilingManager;
        [SerializeField] private ReplacerManager _replacerManager = ReplacerManager.instance as ReplacerManager;
        [SerializeField] private EraserManager _eraserManager = EraserManager.instance as EraserManager;

        [SerializeField]
        private SelectionToolManager _selectionToolManager = SelectionToolManager.instance as SelectionToolManager;
        [SerializeField] private ExtrudeManager _extrudeSettings = ExtrudeManager.instance as ExtrudeManager;
        [SerializeField] private MirrorManager _mirrorManager = MirrorManager.instance as MirrorManager;

        [SerializeField] private SnapManager _snapManager = new SnapManager();
        private bool _savePending = false;
        private bool _saving = false;

        public static string palettesDirectory
        {
            get
            {
                var dir = PWBSettings.dataDir + "/" + PALETTES_DIR;
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string dataPath => PWBSettings.dataDir + "/" + FULL_FILE_NAME;

        public string version => _version;
        public int autoSavePeriodMinutes
        {
            get => _autoSavePeriodMinutes;
            set
            {
                value = Mathf.Clamp(value, 1, 10);
                if (_autoSavePeriodMinutes == value) return;
                _autoSavePeriodMinutes = value;
                Save();
            }
        }

        public bool undoBrushProperties
        {
            get => _undoBrushProperties;
            set
            {
                if (_undoBrushProperties == value) return;
                _undoBrushProperties = value;
                Save();
            }
        }

        public bool undoPalette
        {
            get => _undoPalette;
            set
            {
                if (_undoPalette == value) return;
                _undoPalette = value;
                Save();
            }
        }

        public int controPointSize
        {
            get => _controlPointSize;
            set
            {
                if (_controlPointSize == value) return;
                _controlPointSize = value;
                Save();
            }
        }

        public bool closeAllWindowsWhenClosingTheToolbar
        {
            get => _closeAllWindowsWhenClosingTheToolbar;
            set
            {
                if (_closeAllWindowsWhenClosingTheToolbar == value) return;
                _closeAllWindowsWhenClosingTheToolbar = value;
                Save();
            }
        }

        public int thumbnailLayer
        {
            get => _thumbnailLayer;
            set
            {
                value = Mathf.Clamp(value, 0, 31);
                if (_thumbnailLayer == value) return;
                _thumbnailLayer = value;
                Save();
            }
        }

        public UnsavedChangesAction unsavedChangesAction
        {
            get => _unsavedChangesAction;
            set
            {
                if (_unsavedChangesAction == value) return;
                _unsavedChangesAction = value;
                Save();
            }
        }
        public void SetSavePending() => _savePending = true;
        public bool saving => _saving;
        public bool VersionUpdate()
        {
            var currentText = ReadDataText();
            if (currentText == null) return false;
            var dataVersion = JsonUtility.FromJson<PWBDataVersion>(currentText);
            bool V1_9()
            {
                if (dataVersion.IsOlderThan("1.10"))
                {
                    var v1_9_data = JsonUtility.FromJson<V1_9_PWBData>(currentText);
                    var v1_9_sceneItems = v1_9_data._lineManager._unsavedProfile._sceneLines;
                    if (v1_9_sceneItems == null || v1_9_sceneItems.Length == 0) return false;
                    foreach (var v1_9_sceneData in v1_9_sceneItems)
                    {
                        var v1_9_sceneLines = v1_9_sceneData._lines;
                        if (v1_9_sceneItems == null || v1_9_sceneItems.Length == 0) return false;
                        foreach (var v1_9_sceneLine in v1_9_sceneLines)
                        {
                            if (v1_9_sceneLines == null || v1_9_sceneLines.Length == 0) return false;
                            var lineData = new LineData(v1_9_sceneLine._id, v1_9_sceneLine._data._controlPoints,
                                v1_9_sceneLine._objectPoses, v1_9_sceneLine._initialBrushId,
                                v1_9_sceneLine._data._closed, v1_9_sceneLine._settings);
                            LineManager.instance.AddPersistentItem(v1_9_sceneData._sceneGUID, lineData);
                        }
                    }
                    return true;
                }
                return false;
            }
            var updated = V1_9();

            if (dataVersion.IsOlderThan("2.9"))
            {
                var v2_8_data = JsonUtility.FromJson<V2_8_PWBData>(currentText);
                if (v2_8_data._paletteManager._paletteData.Length > 0) PaletteManager.ClearPaletteList();
                foreach (var paletteData in v2_8_data._paletteManager._paletteData)
                {
                    paletteData.version = VERSION;
                    PaletteManager.AddPalette(paletteData);
                }
                var textAssets = Resources.LoadAll<TextAsset>(FILE_NAME);
                for (int i = 0; i < textAssets.Length; ++i)
                {
                    var assetPath = UnityEditor.AssetDatabase.GetAssetPath(textAssets[i]);
                    UnityEditor.AssetDatabase.DeleteAsset(assetPath);
                }
                PWBCore.staticData.Save(false);

                PrefabPalette.RepainWindow();
                updated = true;
            }
            return updated;
        }

        public void UpdateRootDirectory()
        {
            var directories = System.IO.Directory.GetDirectories(Application.dataPath, "PrefabWorldBuilder",
                System.IO.SearchOption.AllDirectories).Where(d => d.Contains(RELATIVE_TOOL_DIR)).ToArray();
            if (directories.Length == 0)
            {
                _rootDirectory = Application.dataPath + "/" + RELATIVE_TOOL_DIR;
                System.IO.Directory.CreateDirectory(_rootDirectory);
            }
            else _rootDirectory = System.IO.Directory.GetParent(directories[0]).FullName;
        }

        private string rootDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_rootDirectory)) UpdateRootDirectory();
                return _rootDirectory;
            }
        }

        public void Save() => Save(true);

        public void Save(bool updateVersion)
        {
            _saving = true;
            if (updateVersion) VersionUpdate();
            _version = VERSION;
            var jsonString = JsonUtility.ToJson(this);
            System.IO.File.WriteAllText(dataPath, jsonString);
            UnityEditor.AssetDatabase.Refresh();
            _savePending = false;
            _saving = false;
        }

        public static string ReadDataText()
        {
            var fullFilePath = dataPath;
            if (!System.IO.File.Exists(fullFilePath)) PWBCore.staticData.Save(false);
            return System.IO.File.ReadAllText(fullFilePath);
        }

        public void SaveIfPending() { if (_savePending) Save(); }

        public string documentationPath
        {
            get
            {
                var absolutePath = rootDirectory + "/Documentation/Prefab World Builder Documentation.pdf";
                var relativepath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
                return relativepath;
            }
        }
    }
    #endregion

    #region SHORTCUTS
    [System.Serializable]
    public class PWBKeyCombination : System.IEquatable<PWBKeyCombination>
    {
        [SerializeField] private KeyCode _keyCode = KeyCode.None;
        [SerializeField] private EventModifiers _modifiers = EventModifiers.None;

        public KeyCode keycode => _keyCode;
        public EventModifiers modifiers => _modifiers;
        public PWBKeyCombination(KeyCode keyCode, EventModifiers modifiers = EventModifiers.None)
            => SetCombination(keyCode, modifiers);

        public void SetCombination(KeyCode keyCode, EventModifiers modifiers = EventModifiers.None)
        {
            _keyCode = keyCode;
            _modifiers = modifiers & (EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);
        }
        public bool Equals(PWBKeyCombination other)
        {
            if (other == null) return false;
            return _keyCode == other._keyCode && _modifiers == other._modifiers;
        }
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (!(other is PWBKeyCombination otherCombination)) return false;
            return Equals(otherCombination);
        }
        public override int GetHashCode()
        {
            int hashCode = 822824530;
            hashCode = hashCode * -1521134295 + _modifiers.GetHashCode();
            hashCode = hashCode * -1521134295 + _keyCode.GetHashCode();
            return hashCode;
        }
        public static bool operator ==(PWBKeyCombination lhs, PWBKeyCombination rhs)
        {
            if ((object)lhs == null && (object)rhs == null) return true;
            if ((object)lhs == null || (object)rhs == null) return false;
            return lhs.Equals(rhs);
        }
        public static bool operator !=(PWBKeyCombination lhs, PWBKeyCombination rhs) => !(lhs == rhs);

        public bool control => (_modifiers & EventModifiers.Control) != 0;
        public bool alt => (_modifiers & EventModifiers.Alt) != 0;
        public bool shift => (_modifiers & EventModifiers.Shift) != 0;

        public override string ToString()
        {
            var result = string.Empty;
            if (keycode == KeyCode.None) return "Disabled";
            if (control) result = "Ctrl";
            if (alt) result += (result == string.Empty ? "Alt" : "+Alt");
            if (shift) result += (result == string.Empty ? "Shift" : "+Shift");
            if (result != string.Empty) result += "+";
            result += _keyCode;
            return result;
        }

        public bool Check()
        {
            if (Event.current == null) return false;
            if (keycode == KeyCode.None) return false;
            var currentModifiers = Event.current.modifiers
                & (EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);
            return Event.current.type == EventType.KeyDown
               && currentModifiers == modifiers
               && Event.current.keyCode == keycode;
        }

        public bool enabled => keycode != KeyCode.None;
    }

    [System.Serializable]
    public class PWBShortcut
    {
        public enum Group
        {
            NONE = 0,
            GLOBAL = 1,
            GRID = 2,
            PIN = 4,
            BRUSH = 8,
            GRAVITY = 16,
            LINE = 32,
            SHAPE = 64,
            TILING = 128,
            SELECTION = 256
        }

        [SerializeField] private string _name = null;
        [SerializeField] private Group _group = Group.NONE;
        [SerializeField]
        private PWBKeyCombination _keyCombination = new PWBKeyCombination(KeyCode.None, EventModifiers.None);
        [SerializeField] private bool _conflicted = false;

        public PWBShortcut(string name, Group group, KeyCode keyCode, EventModifiers modifiers = EventModifiers.None)
        {
            _name = name;
            _group = group;
            _keyCombination.SetCombination(keyCode, modifiers);
        }

        public string name => _name;

        public Group group => _group;
        public PWBKeyCombination keyCombination => _keyCombination;

        public bool conflicted { get => _conflicted; set => _conflicted = value; }
    }

    [System.Serializable]
    public class PWBShortcuts
    {
        #region PROFILE
        [SerializeField] private string _profileName = string.Empty;
        public string profileName { get => _profileName; set => _profileName = value; }
        public PWBShortcuts(string name) => _profileName = name;

        public static PWBShortcuts GetDefault(int i)
        {
            if (i == 0) return new PWBShortcuts("Default 1");
            else if (i == 1)
            {
                var d2 = new PWBShortcuts("Default 2");
                d2.pinMoveHandlesUp.keyCombination.SetCombination(KeyCode.PageUp);
                d2.pinMoveHandlesDown.keyCombination.SetCombination(KeyCode.PageDown);
                d2.pinSelectPivotHandle.keyCombination.SetCombination(KeyCode.Home);
                d2.pinSelectNextHandle.keyCombination.SetCombination(KeyCode.End);
                d2.pinResetScale.keyCombination.SetCombination(KeyCode.Home, EventModifiers.Control | EventModifiers.Shift);

                d2.pinRotate90YCW.keyCombination.SetCombination(KeyCode.LeftArrow, EventModifiers.Control);
                d2._pinRotate90YCCW.keyCombination.SetCombination(KeyCode.RightArrow, EventModifiers.Control);
                d2.pinRotateAStepYCW.keyCombination.SetCombination(KeyCode.LeftArrow,
                    EventModifiers.Control | EventModifiers.Shift);
                d2.pinRotateAStepYCCW.keyCombination.SetCombination(KeyCode.RightArrow,
                    EventModifiers.Control | EventModifiers.Shift);

                d2.pinRotate90XCW.keyCombination.SetCombination(KeyCode.LeftArrow,
                    EventModifiers.Control | EventModifiers.Alt);
                d2.pinRotate90XCCW.keyCombination.SetCombination(KeyCode.RightArrow,
                    EventModifiers.Control | EventModifiers.Alt);
                d2.pinRotateAStepXCW.keyCombination.SetCombination(KeyCode.LeftArrow,
                    EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);
                d2.pinRotateAStepXCCW.keyCombination.SetCombination(KeyCode.RightArrow,
                    EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);

                d2.pinResetRotation.keyCombination.SetCombination(KeyCode.Home, EventModifiers.Control);

                d2.pinAdd1UnitToSurfDist.keyCombination.SetCombination(KeyCode.UpArrow,
                    EventModifiers.Control | EventModifiers.Alt);
                d2.pinSubtract1UnitFromSurfDist.keyCombination.SetCombination(KeyCode.DownArrow,
                    EventModifiers.Control | EventModifiers.Alt);
                d2.pinAdd01UnitToSurfDist.keyCombination.SetCombination(KeyCode.UpArrow,
                    EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);
                d2.pinSubtract01UnitFromSurfDist.keyCombination.SetCombination(KeyCode.DownArrow,
                    EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);

                d2.lineToggleCurve.keyCombination.SetCombination(KeyCode.PageDown);
                d2.lineToggleClosed.keyCombination.SetCombination(KeyCode.End);

                d2.selectionRotate90XCW.keyCombination.SetCombination(KeyCode.PageUp,
                    EventModifiers.Control | EventModifiers.Shift);
                d2.selectionRotate90XCCW.keyCombination.SetCombination(KeyCode.PageDown,
                    EventModifiers.Control | EventModifiers.Shift);
                d2.selectionRotate90YCW.keyCombination.SetCombination(KeyCode.LeftArrow,
                    EventModifiers.Control | EventModifiers.Alt);
                d2.selectionRotate90YCCW.keyCombination.SetCombination(KeyCode.RightArrow,
                    EventModifiers.Control | EventModifiers.Alt);
                d2.selectionRotate90ZCW.keyCombination.SetCombination(KeyCode.UpArrow,
                   EventModifiers.Control | EventModifiers.Alt);
                d2.selectionRotate90ZCCW.keyCombination.SetCombination(KeyCode.DownArrow,
                    EventModifiers.Control | EventModifiers.Alt);

                return d2;
            }
            return null;
        }
        #endregion

        #region GRID
        [SerializeField]
        private PWBShortcut _gridEnableShortcuts = new PWBShortcut("Enable grid shorcuts",
           PWBShortcut.Group.GLOBAL | PWBShortcut.Group.GRID, KeyCode.G, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _gridToggle = new PWBShortcut("Toggle grid",
            PWBShortcut.Group.GRID, KeyCode.G, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _gridToggleSnapping = new PWBShortcut("Toggle snapping",
            PWBShortcut.Group.GRID, KeyCode.H, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _gridToggleLock = new PWBShortcut("Toggle grid lock",
            PWBShortcut.Group.GRID, KeyCode.L, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _gridSetOriginPosition = new PWBShortcut("Set the origin to the active gameobject position",
            PWBShortcut.Group.GRID, KeyCode.W, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _gridSetOriginRotation = new PWBShortcut("Set the grid rotation to the active gameobject rotation",
            PWBShortcut.Group.GRID, KeyCode.E, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _gridSetSize = new PWBShortcut("Set the snap value to the size of the active gameobject",
            PWBShortcut.Group.GRID, KeyCode.R, EventModifiers.Control);
        public PWBShortcut gridEnableShortcuts => _gridEnableShortcuts;
        public PWBShortcut gridToggle => _gridToggle;
        public PWBShortcut gridToggleSnaping => _gridToggleSnapping;
        public PWBShortcut gridToggleLock => _gridToggleLock;
        public PWBShortcut gridSetOriginPosition => _gridSetOriginPosition;
        public PWBShortcut gridSetOriginRotation => _gridSetOriginRotation;
        public PWBShortcut gridSetSize => _gridSetSize;
        #endregion

        #region PIN
        [SerializeField]
        private PWBShortcut _pinMoveHandlesUp = new PWBShortcut("Move handles up",
           PWBShortcut.Group.PIN, KeyCode.U, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _pinMoveHandlesDown = new PWBShortcut("Move handles down",
           PWBShortcut.Group.PIN, KeyCode.J, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _pinSelectNextHandle = new PWBShortcut("Select the next handle as active",
           PWBShortcut.Group.PIN, KeyCode.Y, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _pinSelectPivotHandle = new PWBShortcut("Set the pivot as the active handle",
           PWBShortcut.Group.PIN, KeyCode.T, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _pinToggleRepeatItem = new PWBShortcut("Toggle repeat item option",
           PWBShortcut.Group.PIN, KeyCode.T, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _pinResetScale = new PWBShortcut("Reset scale",
          PWBShortcut.Group.PIN, KeyCode.Period, EventModifiers.Control | EventModifiers.Shift);
       
        [SerializeField]
        private PWBShortcut _pinRotate90YCW = new PWBShortcut("Rotate 90º clockwise around Y axis",
          PWBShortcut.Group.PIN, KeyCode.Q, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _pinRotate90YCCW = new PWBShortcut("Rotate 90º counterclockwise around Y axis",
          PWBShortcut.Group.PIN, KeyCode.W, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _pinRotateAStepYCW = new PWBShortcut("Rotate clockwise in small steps around the Y axis",
        PWBShortcut.Group.PIN, KeyCode.Q, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _pinRotateAStepYCCW = new PWBShortcut("Rotate counterclockwise in small steps around the Y axis",
        PWBShortcut.Group.PIN, KeyCode.W, EventModifiers.Control | EventModifiers.Shift);

        [SerializeField]
        private PWBShortcut _pinRotate90XCW = new PWBShortcut("Rotate 90º clockwise around X axis",
          PWBShortcut.Group.PIN, KeyCode.K, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _pinRotate90XCCW = new PWBShortcut("Rotate 90º counterclockwise around X axis",
          PWBShortcut.Group.PIN, KeyCode.L, EventModifiers.Control);
        [SerializeField]
        private PWBShortcut _pinRotateAStepXCW = new PWBShortcut("Rotate clockwise in small steps around the X axis",
        PWBShortcut.Group.PIN, KeyCode.K, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _pinRotateAStepXCCW = new PWBShortcut("Rotate counterclockwise in small steps around the X axis",
        PWBShortcut.Group.PIN, KeyCode.L, EventModifiers.Control | EventModifiers.Shift);

        [SerializeField]
        private PWBShortcut _pinRotate90ZCW = new PWBShortcut("Rotate 90º clockwise around Z axis",
          PWBShortcut.Group.PIN, KeyCode.Period, EventModifiers.Control | EventModifiers.Alt);
        [SerializeField]
        private PWBShortcut _pinRotate90ZCCW = new PWBShortcut("Rotate 90º counterclockwise around Z axis",
          PWBShortcut.Group.PIN, KeyCode.Comma, EventModifiers.Control | EventModifiers.Alt);
        [SerializeField]
        private PWBShortcut _pinRotateAStepZCW = new PWBShortcut("Rotate clockwise in small steps around the Z axis",
        PWBShortcut.Group.PIN, KeyCode.Period, EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _pinRotateAStepZCCW = new PWBShortcut("Rotate counterclockwise in small steps around the Z axis",
        PWBShortcut.Group.PIN, KeyCode.Comma, EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);

        [SerializeField]
        private PWBShortcut _pinResetRotation = new PWBShortcut("Reset rotation to zero",
         PWBShortcut.Group.PIN, KeyCode.M, EventModifiers.Control | EventModifiers.Shift);

        [SerializeField]
        private PWBShortcut _pinAdd1UnitToSurfDist = new PWBShortcut("Increase the distance from the surface by 1 unit",
          PWBShortcut.Group.PIN | PWBShortcut.Group.GRAVITY, KeyCode.U, EventModifiers.Control | EventModifiers.Alt);
        [SerializeField]
        private PWBShortcut _pinSubtract1UnitFromSurfDist = new PWBShortcut("Decrease the distance from the surface by 1 unit",
          PWBShortcut.Group.PIN | PWBShortcut.Group.GRAVITY, KeyCode.J, EventModifiers.Control | EventModifiers.Alt);
        [SerializeField]
        private PWBShortcut _pinAdd01UnitToSurfDist = new PWBShortcut("Increase the distance from the surface by 0.1 units",
         PWBShortcut.Group.PIN | PWBShortcut.Group.GRAVITY, KeyCode.U,
         EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _pinSubtract01UnitFromSurfDist
            = new PWBShortcut("Decrease the distance from the surface by 0.1 units",
          PWBShortcut.Group.PIN | PWBShortcut.Group.GRAVITY, KeyCode.J,
          EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift);

        [SerializeField]
        private PWBShortcut _pinResetSurfDist = new PWBShortcut("Reset the distance from the surface to zero",
         PWBShortcut.Group.PIN, KeyCode.G, EventModifiers.Control | EventModifiers.Shift);

        public PWBShortcut pinMoveHandlesUp => _pinMoveHandlesUp;
        public PWBShortcut pinMoveHandlesDown => _pinMoveHandlesDown;
        public PWBShortcut pinSelectNextHandle => _pinSelectNextHandle;
        public PWBShortcut pinSelectPivotHandle => _pinSelectPivotHandle;
        public PWBShortcut pinToggleRepeatItem => _pinToggleRepeatItem;
        public PWBShortcut pinResetScale => _pinResetScale;

        public PWBShortcut pinRotate90YCW => _pinRotate90YCW;
        public PWBShortcut pinRotate90YCCW => _pinRotate90YCCW;
        public PWBShortcut pinRotateAStepYCW => _pinRotateAStepYCW;
        public PWBShortcut pinRotateAStepYCCW => _pinRotateAStepYCCW;

        public PWBShortcut pinRotate90XCW => _pinRotate90XCW;
        public PWBShortcut pinRotate90XCCW => _pinRotate90XCCW;
        public PWBShortcut pinRotateAStepXCW => _pinRotateAStepXCW;
        public PWBShortcut pinRotateAStepXCCW => _pinRotateAStepXCCW;

        public PWBShortcut pinRotate90ZCW => _pinRotate90ZCW;
        public PWBShortcut pinRotate90ZCCW => _pinRotate90ZCCW;
        public PWBShortcut pinRotateAStepZCW => _pinRotateAStepZCW;
        public PWBShortcut pinRotateAStepZCCW => _pinRotateAStepZCCW;

        public PWBShortcut pinResetRotation => _pinResetRotation;

        public PWBShortcut pinAdd1UnitToSurfDist => _pinAdd1UnitToSurfDist;
        public PWBShortcut pinSubtract1UnitFromSurfDist => _pinSubtract1UnitFromSurfDist;
        public PWBShortcut pinAdd01UnitToSurfDist => _pinAdd01UnitToSurfDist;
        public PWBShortcut pinSubtract01UnitFromSurfDist => _pinSubtract01UnitFromSurfDist;

        public PWBShortcut pinResetSurfDist => _pinResetSurfDist;
        #endregion

        #region BRUSH & GRAVITY
        [SerializeField]
        private PWBShortcut _brushUpdatebrushstroke = new PWBShortcut("Update brushstroke",
          PWBShortcut.Group.BRUSH | PWBShortcut.Group.GRAVITY, KeyCode.Period, EventModifiers.Control | EventModifiers.Shift);
        public PWBShortcut brushUpdatebrushstroke => _brushUpdatebrushstroke;
        #endregion

        #region EDIT MODE
        [SerializeField]
        private PWBShortcut _editModeDeleteItemAndItsChildren
            = new PWBShortcut("Delete selected persistent item and its children",
           PWBShortcut.Group.LINE | PWBShortcut.Group.SHAPE | PWBShortcut.Group.TILING,
           KeyCode.Delete, EventModifiers.Alt);
        [SerializeField]
        private PWBShortcut _editModeDeleteItemButNotItsChildren
            = new PWBShortcut("Delete selected persistent item but not its children",
           PWBShortcut.Group.LINE | PWBShortcut.Group.SHAPE | PWBShortcut.Group.TILING,
           KeyCode.Delete, EventModifiers.Alt | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _editModeSelectParent = new PWBShortcut("Select parent object",
           PWBShortcut.Group.LINE | PWBShortcut.Group.SHAPE | PWBShortcut.Group.TILING,
           KeyCode.T, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _editModeToggle = new PWBShortcut("Toggle edit mode",
          PWBShortcut.Group.LINE | PWBShortcut.Group.SHAPE | PWBShortcut.Group.TILING,
          KeyCode.Period, EventModifiers.Alt | EventModifiers.Shift);
        public PWBShortcut editModeDeleteItemAndItsChildren => _editModeDeleteItemAndItsChildren;
        public PWBShortcut editModeDeleteItemButNotItsChildren => _editModeDeleteItemButNotItsChildren;
        public PWBShortcut editModeSelectParent => _editModeSelectParent;
        public PWBShortcut editModeToggle => _editModeToggle;
        #endregion

        #region LINE
        [SerializeField]
        private PWBShortcut _lineSelectAllPoints = new PWBShortcut("Select all points",
          PWBShortcut.Group.LINE, KeyCode.A, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _lineDeselectAllPoints = new PWBShortcut("Deselect all points",
          PWBShortcut.Group.LINE, KeyCode.D, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _lineToggleCurve = new PWBShortcut("Set the previous segment as a Curved or Straight Line",
          PWBShortcut.Group.LINE, KeyCode.Y, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _lineToggleClosed = new PWBShortcut("Close or open the line",
          PWBShortcut.Group.LINE, KeyCode.O, EventModifiers.Control | EventModifiers.Shift);
        public PWBShortcut lineSelectAllPoints => _lineSelectAllPoints;
        public PWBShortcut lineDeselectAllPoints => _lineDeselectAllPoints;
        public PWBShortcut lineToggleCurve => _lineToggleCurve;
        public PWBShortcut lineToggleClosed => _lineToggleClosed;
        #endregion

        #region TILING & SELECTION
        [SerializeField]
        private PWBShortcut _selectionRotate90XCW = new PWBShortcut("Rotate 90º clockwise around X axis",
          PWBShortcut.Group.TILING | PWBShortcut.Group.SELECTION, KeyCode.U, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _selectionRotate90XCCW = new PWBShortcut("Rotate 90º counterclockwise around X axis",
          PWBShortcut.Group.TILING | PWBShortcut.Group.SELECTION, KeyCode.J, EventModifiers.Control | EventModifiers.Shift);
        [SerializeField]
        private PWBShortcut _selectionRotate90YCW = new PWBShortcut("Rotate 90º clockwise around Y axis",
          PWBShortcut.Group.TILING | PWBShortcut.Group.SELECTION, KeyCode.K, EventModifiers.Control | EventModifiers.Alt);
        [SerializeField]
        private PWBShortcut _selectionRotate90YCCW = new PWBShortcut("Rotate 90º counterclockwise around Y axis",
          PWBShortcut.Group.TILING | PWBShortcut.Group.SELECTION, KeyCode.L, EventModifiers.Control | EventModifiers.Alt);
        [SerializeField]
        private PWBShortcut _selectionRotate90ZCW = new PWBShortcut("Rotate 90º clockwise around Z axis",
          PWBShortcut.Group.TILING | PWBShortcut.Group.SELECTION, KeyCode.U, EventModifiers.Control | EventModifiers.Alt);
        [SerializeField]
        private PWBShortcut _selectionRotate90ZCCW = new PWBShortcut("Rotate 90º counterclockwise around Z axis",
          PWBShortcut.Group.TILING | PWBShortcut.Group.SELECTION, KeyCode.J, EventModifiers.Control | EventModifiers.Alt);
        public PWBShortcut selectionRotate90XCW => _selectionRotate90XCW;
        public PWBShortcut selectionRotate90XCCW => _selectionRotate90XCCW;
        public PWBShortcut selectionRotate90YCW => _selectionRotate90YCW;
        public PWBShortcut selectionRotate90YCCW => _selectionRotate90YCCW;
        public PWBShortcut selectionRotate90ZCW => _selectionRotate90ZCW;
        public PWBShortcut selectionRotate90ZCCW => _selectionRotate90ZCCW;
        #endregion

        #region SELECTION
        [SerializeField]
        private PWBShortcut _selectionTogglePositionHandle = new PWBShortcut("Toggle position handle",
          PWBShortcut.Group.SELECTION, KeyCode.W);
        [SerializeField]
        private PWBShortcut _selectionToggleRotationHandle = new PWBShortcut("Toggle rotation handle",
          PWBShortcut.Group.SELECTION, KeyCode.E);
        [SerializeField]
        private PWBShortcut _selectionToggleScaleHandle = new PWBShortcut("Toggle scale handle",
          PWBShortcut.Group.SELECTION, KeyCode.R);
        [SerializeField]
        private PWBShortcut _selectionEditCustomHandle = new PWBShortcut("Edit custom handle",
          PWBShortcut.Group.SELECTION, KeyCode.U);
        public PWBShortcut selectionTogglePositionHandle => _selectionTogglePositionHandle;
        public PWBShortcut selectionToggleRotationHandle => _selectionToggleRotationHandle;
        public PWBShortcut selectionToggleScaleHandle => _selectionToggleScaleHandle;
        public PWBShortcut selectionEditCustomHandle => _selectionEditCustomHandle;
        #endregion

        #region CONFLICTS
        private PWBShortcut[] _shortcuts = null;
        public PWBShortcut[] shortcuts
        {
            get
            {
                if (_shortcuts == null)
                    _shortcuts = new PWBShortcut[]
                    {
                        /*/// GRID ///*/
                        _gridEnableShortcuts,
                        _gridToggle,
                        _gridToggleSnapping,
                        _gridToggleLock,
                        _gridSetOriginPosition,
                        _gridSetOriginRotation,
                        _gridSetSize,
                        /*/// PIN ///*/
                        _pinMoveHandlesUp,
                        _pinMoveHandlesDown,
                        _pinSelectNextHandle,
                        _pinSelectPivotHandle,
                        _pinToggleRepeatItem,
                        _pinResetScale,

                        _pinRotate90YCW,
                        _pinRotate90YCCW,
                        _pinRotateAStepYCW,
                        _pinRotateAStepYCCW,

                        _pinRotate90XCW,
                        _pinRotate90XCCW,
                        _pinRotateAStepXCW,
                        _pinRotateAStepXCCW,

                        _pinRotate90ZCW,
                        _pinRotate90ZCCW,
                        _pinRotateAStepZCW,
                        _pinRotateAStepZCCW,

                        _pinResetRotation,

                        _pinAdd1UnitToSurfDist,
                        _pinSubtract1UnitFromSurfDist,
                        _pinAdd01UnitToSurfDist,
                        _pinSubtract01UnitFromSurfDist,

                        _pinResetSurfDist,
                        /*/// BRUSH & GRAVITY ///*/
                        _brushUpdatebrushstroke,
                        /*/// EDIT MODE ///*/
                        _editModeDeleteItemAndItsChildren,
                        _editModeDeleteItemButNotItsChildren,
                        _editModeSelectParent,
                        editModeToggle,
                        /*/// LINE ///*/
                        _lineSelectAllPoints,
                        _lineDeselectAllPoints,
                        _lineToggleCurve,
                        _lineToggleClosed,
                        /*/// TILING & SELECTION ///*/
                        _selectionRotate90XCW,
                        _selectionRotate90XCCW,
                        _selectionRotate90YCW,
                        _selectionRotate90YCCW,
                        _selectionRotate90ZCW,
                        _selectionRotate90ZCCW,
                        /*/// SELECTION ///*/
                        _selectionTogglePositionHandle,
                        _selectionToggleRotationHandle,
                        _selectionToggleScaleHandle,
                        _selectionEditCustomHandle
                    };
                return _shortcuts;
            }
        }

        public void UpdateConficts()
        {
            foreach (var shortcut in shortcuts) shortcut.conflicted = false;
            for (int i = 0; i < shortcuts.Length; ++i)
            {
                var shortcut1 = shortcuts[i];
                if (shortcut1.conflicted) continue;
                if (shortcut1.keyCombination.keycode == KeyCode.None) continue;
                for (int j = i + 1; j < shortcuts.Length; ++j)
                {
                    var shortcut2 = shortcuts[j];
                    if (shortcut2.conflicted) continue;
                    if (shortcut2.keyCombination.keycode == KeyCode.None) continue;
                    if ((shortcut1.group & shortcut2.group) == 0 && (shortcut1.group & PWBShortcut.Group.GLOBAL) == 0
                        && (shortcut1.group & PWBShortcut.Group.GLOBAL) == 0) continue;
                    if (shortcut1 == gridEnableShortcuts && (shortcut2.group & PWBShortcut.Group.GRID) != 0) continue;

                    if (shortcut1.keyCombination == shortcut2.keyCombination)
                    {
                        shortcut1.conflicted = true;
                        shortcut2.conflicted = true;
                    }
                }
            }
        }

        public bool CheckConflicts(PWBKeyCombination combi, PWBShortcut target, out string conflicts)
        {
            conflicts = string.Empty;
            foreach (var shortcut in shortcuts)
            {
                if (target == shortcut) continue;
                if (target.keyCombination.keycode == KeyCode.None || shortcut.keyCombination.keycode == KeyCode.None) continue;
                if (combi == shortcut.keyCombination && ((target.group & shortcut.group) != 0
                    || (shortcut.group & PWBShortcut.Group.GLOBAL) != 0 || (target.group & PWBShortcut.Group.GLOBAL) != 0))
                {
                    if (shortcut == gridEnableShortcuts && (target.group & PWBShortcut.Group.GRID) != 0) continue;
                    if (target == gridEnableShortcuts && (shortcut.group & PWBShortcut.Group.GRID) != 0) continue;
                    if (conflicts != string.Empty) conflicts += "\n";
                    conflicts += shortcut.name;
                }
            }
            return conflicts != string.Empty;
        }
        #endregion
    }
    #endregion

    #region SETTINGS
    [System.Serializable]
    public class PWBSettings
    {
        #region COMMON
        private static string _settingsPath = null;
        private static PWBSettings _instance = null;
        private PWBSettings() { }

        private static PWBSettings instance
        {
            get
            {
                if (_instance == null) _instance = new PWBSettings();
                return _instance;
            }
        }
        private static string settingsPath
        {
            get
            {
                if (_settingsPath == null)
                    _settingsPath = System.IO.Directory.GetParent(Application.dataPath) + "/ProjectSettings/PWBSettings.txt";
                return _settingsPath;
            }
        }

        private void LoadFromFile()
        {
            if (!System.IO.File.Exists(settingsPath))
            {
                var files = System.IO.Directory.GetFiles(Application.dataPath,
                        PWBData.FULL_FILE_NAME, System.IO.SearchOption.AllDirectories);
                if (files.Length > 0) _dataDir = System.IO.Path.GetDirectoryName(files[0]);
                else
                {
                    _dataDir = Application.dataPath + "/" + PWBData.RELATIVE_DATA_DIR;
                    System.IO.Directory.CreateDirectory(_dataDir);
                }
                Save();
            }
            else
            {
                var settings = JsonUtility.FromJson<PWBSettings>(System.IO.File.ReadAllText(settingsPath));
                _dataDir = settings._dataDir;
                _shortcutProfiles = settings._shortcutProfiles;
                _selectedProfileIdx = settings._selectedProfileIdx;
            }
        }

        private void Save()
        {
            var jsonString = JsonUtility.ToJson(this);
            System.IO.File.WriteAllText(settingsPath, jsonString);
        }
        #endregion

        #region DATA DIR
        [SerializeField] private string _dataDir = null;
        private static bool _movingDir = false;
        public static bool movingDir => _movingDir;
        private static void CheckDataDir()
        {
            if (instance._dataDir == null) instance.LoadFromFile();
        }

        public static string dataDir
        {
            get
            {
                CheckDataDir();
                return instance._dataDir;
            }
            set
            {
                if (instance._dataDir == value) return;
                var currentDir = instance._dataDir;
                var newDir = value;
                void DeleteMeta(string path)
                {
                    var metapath = path + ".meta";
                    if (System.IO.File.Exists(metapath)) System.IO.File.Delete(metapath);
                }
                bool DeleteIfEmpty(string dirPath)
                {
                    if (System.IO.Directory.GetFiles(dirPath).Length != 0) return false;
                    System.IO.Directory.Delete(dirPath);
                    DeleteMeta(dirPath);
                    return true;
                }
                if (System.IO.Directory.Exists(currentDir))
                {
                    _movingDir = true;
                    var currentDataPath = currentDir + "/" + PWBData.FULL_FILE_NAME;
                    if (System.IO.File.Exists(currentDataPath))
                    {
                        var newDataPath = newDir + "/" + PWBData.FULL_FILE_NAME;
                        if (System.IO.File.Exists(newDataPath)) System.IO.File.Delete(newDataPath);
                        DeleteMeta(currentDataPath);
                        System.IO.File.Move(currentDataPath, newDataPath);

                        var currentPalettesDir = currentDir + "/" + PWBData.PALETTES_DIR;
                        if (System.IO.Directory.Exists(currentPalettesDir))
                        {
                            var newPalettesDir = newDir + "/" + PWBData.PALETTES_DIR;
                            if (!System.IO.Directory.Exists(newPalettesDir))
                                System.IO.Directory.CreateDirectory(newPalettesDir);
                            var palettesPaths = System.IO.Directory.GetFiles(currentPalettesDir, "*.txt");
                            foreach (var currentPalettePath in palettesPaths)
                            {
                                var fileName = System.IO.Path.GetFileName(currentPalettePath);
                                var newPalettePath = newPalettesDir + "/" + fileName;
                                if (System.IO.File.Exists(newPalettePath)) System.IO.File.Delete(newPalettePath);
                                DeleteMeta(currentPalettePath);

                                var paletteText = System.IO.File.ReadAllText(currentPalettePath);
                                var palette = JsonUtility.FromJson<PaletteData>(paletteText);
                                palette.filePath = newPalettePath;

                                System.IO.File.Move(currentPalettePath, newPalettePath);
                                System.IO.File.Delete(currentPalettePath);
                            }
                        }
                        if (DeleteIfEmpty(currentPalettesDir)) DeleteIfEmpty(currentDir);
                        UnityEditor.AssetDatabase.Refresh();
                    }
                    _movingDir = false;
                }
                instance._dataDir = value;
                instance.Save();
            }
        }
        #endregion

        #region SHORTCUTS
        [SerializeField]
        private System.Collections.Generic.List<PWBShortcuts> _shortcutProfiles
           = new System.Collections.Generic.List<PWBShortcuts>()
           {
                PWBShortcuts.GetDefault(0),
                PWBShortcuts.GetDefault(1)
           };
        [SerializeField] private int _selectedProfileIdx = 0;
        private PWBShortcuts selectedProfile
        {
            get
            {
                if (_selectedProfileIdx < 0 || _selectedProfileIdx > _shortcutProfiles.Count) _selectedProfileIdx = 0;
                return _shortcutProfiles[_selectedProfileIdx];
            }
        }

        public static PWBShortcuts shortcuts
        {
            get
            {
                CheckDataDir();
                return instance.selectedProfile;
            }
        }

        public static string[] shotcutProfileNames
        {
            get
            {
                CheckDataDir();
                return instance._shortcutProfiles.Select(p => p.profileName).ToArray();
            }
        }

        public static int selectedProfileIdx
        {
            get
            {
                CheckDataDir();
                return instance._selectedProfileIdx;
            }
            set
            {
                CheckDataDir();
                instance._selectedProfileIdx = value;
            }
        }

        public static void UpdateShrotcutsConflictsAndSaveFile()
        {
            CheckDataDir();
            shortcuts.UpdateConficts();
            instance.Save();
        }

        public static void SetDefaultShortcut(int shortcutIdx, int defaultIdx)
        {
            CheckDataDir();
            if (shortcutIdx < 0 || shortcutIdx > instance._shortcutProfiles.Count) return;
            instance._shortcutProfiles[shortcutIdx] = PWBShortcuts.GetDefault(defaultIdx);
        }

        public static void ResetSelectedProfile()
        {
            CheckDataDir();
            if (selectedProfileIdx == 1) instance._shortcutProfiles[1] = PWBShortcuts.GetDefault(1);
            else instance._shortcutProfiles[instance._selectedProfileIdx] = PWBShortcuts.GetDefault(0);
        }

        public static void ResetShortcutToDefault(PWBShortcut shortcut)
        {
            var defaultProfile = selectedProfileIdx == 1 ? PWBShortcuts.GetDefault(1) : PWBShortcuts.GetDefault(0);
            foreach (var ds in defaultProfile.shortcuts)
            {
                if (ds.group == shortcut.group && ds.name == shortcut.name)
                {
                    shortcut.keyCombination.SetCombination(ds.keyCombination.keycode, ds.keyCombination.modifiers);
                    return;
                }
            }
        }
        #endregion
    }
    #endregion

    #region HANDLERS
    [UnityEditor.InitializeOnLoad]
    public static class ApplicationEventHandler
    {
        private static bool _hierarchyLoaded = false;
        public static bool hierarchyLoaded => _hierarchyLoaded;
        static ApplicationEventHandler()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnStateChanged;
            UnityEditor.EditorApplication.quitting += PWBCore.staticData.Save;
            UnityEditor.EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        private static void OnHierarchyChanged()
        {
            if (!_hierarchyLoaded)
            {
                _hierarchyLoaded = true;
                return;
            }
            if (!PWBCore.staticData.saving) PWBCore.LoadFromFile();
            UnityEditor.EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        }

        private static void OnStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingEditMode
                || state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                PWBCore.staticData.SaveIfPending();
        }
    }

    public class DataReimportHandler : UnityEditor.AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (PWBSettings.movingDir) return;
            if (PWBCore.staticData.saving) return;
            if (PaletteManager.selectedPalette != null && PaletteManager.selectedPalette.saving) return;
            if (!PWBData.palettesDirectory.Contains(Application.dataPath)) return;
            var paths = new System.Collections.Generic.List<string>(importedAssets);
            paths.AddRange(deletedAssets);
            paths.AddRange(movedAssets);
            paths.AddRange(movedFromAssetPaths);

            var relativeDataPath = PWBSettings.dataDir.Replace(Application.dataPath, string.Empty);

            if (paths.Exists(p => p.Contains(relativeDataPath)))
            {
                PaletteManager.instance.LoadPaletteFiles();
                if (PrefabPalette.instance != null) PrefabPalette.instance.Reload();
                return;
            }
        }
    }

    #endregion

    #region AUTOSAVE
    [UnityEditor.InitializeOnLoad]
    public static class AutoSave
    {
        private static int _quickSaveCount = 3;

        static AutoSave()
        {
            PWBCore.staticData.UpdateRootDirectory();
            PeriodicSave();
            PeriodicQuickSave();
        }
        private async static void PeriodicSave()
        {
            if (PWBCore.staticDataWasInitialized)
            {
                await System.Threading.Tasks.Task.Delay(PWBCore.staticData.autoSavePeriodMinutes * 60000);
                PWBCore.staticData.SaveIfPending();
            }
            else await System.Threading.Tasks.Task.Delay(60000);
            PeriodicSave();
        }

        private async static void PeriodicQuickSave()
        {
            await System.Threading.Tasks.Task.Delay(300);
            ++_quickSaveCount;
            if (_quickSaveCount == 3 && PWBCore.staticDataWasInitialized) PWBCore.staticData.Save();
            PeriodicQuickSave();
        }

        public static void QuickSave() => _quickSaveCount = 0;
    }
    #endregion

    #region VERSION
    [System.Serializable]
    public class PWBDataVersion
    {
        [SerializeField] public string _version;
        public bool IsOlderThan(string value) => IsOlderThan(value, _version);

        public static bool IsOlderThan(string value, string referenceValue)
        {
            var intArray = GetIntArray(referenceValue);
            var otherIntArray = GetIntArray(value);
            var minLength = Mathf.Min(intArray.Length, otherIntArray.Length);
            for (int i = 0; i < minLength; ++i)
            {
                if (intArray[i] < otherIntArray[i]) return true;
                else if (intArray[i] > otherIntArray[i]) return false;
            }
            return false;
        }
        private static int[] GetIntArray(string value)
        {
            var stringArray = value.Split('.');
            if (stringArray.Length == 0) return new int[] { 1, 0 };
            var intArray = new int[stringArray.Length];
            for (int i = 0; i < intArray.Length; ++i) intArray[i] = int.Parse(stringArray[i]);
            return intArray;
        }
    }
    #endregion

    #region DATA 1.9
    [System.Serializable]
    public class V1_9_LineData
    {
        [SerializeField] public LinePoint[] _controlPoints;
        [SerializeField] public bool _closed;
    }

    [System.Serializable]
    public class V1_9_PersistentLineData
    {
        [SerializeField] public long _id;
        [SerializeField] public long _initialBrushId;
        [SerializeField] public V1_9_LineData _data;
        [SerializeField] public LineSettings _settings;
        [SerializeField] public ObjectPose[] _objectPoses;
    }

    [System.Serializable]
    public class V1_9_SceneLines
    {
        [SerializeField] public string _sceneGUID;
        [SerializeField] public V1_9_PersistentLineData[] _lines;
    }

    [System.Serializable]
    public class V1_9_Profile
    {
        [SerializeField] public V1_9_SceneLines[] _sceneLines;
    }

    [System.Serializable]
    public class V1_9_LineManager
    {
        [SerializeField] public V1_9_Profile _unsavedProfile;
    }

    [System.Serializable]
    public class V1_9_PWBData
    {
        [SerializeField] public V1_9_LineManager _lineManager;
    }
    #endregion

    #region DATA 2.8
    [System.Serializable]
    public class V2_8_PaletteManager
    {
        [SerializeField] public PaletteData[] _paletteData;
    }

    [System.Serializable]
    public class V2_8_PWBData
    {
        [SerializeField] public V2_8_PaletteManager _paletteManager;
    }
    #endregion
}