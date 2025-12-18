#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace FHH.Logic.Editor
{
    [Serializable]
    public sealed class HierarchyPathRef
    {
        public string ScenePath;
        public List<string> PathTokens = new List<string>(); // names root->leaf

        public static HierarchyPathRef From(GameObject go)
        {
            var r = new HierarchyPathRef { ScenePath = go.scene.path ?? string.Empty };
            var t = go.transform; var stack = new List<string>();
            while (t != null) { stack.Add(t.name); t = t.parent; }
            stack.Reverse(); r.PathTokens = stack; return r;
        }
    }

    [Serializable]
    public sealed class HierarchyRevealerData
    {
        public List<HierarchyPathRef> Targets = new List<HierarchyPathRef>();
        public bool AutoSave = true;
    }

    internal static class HierarchyRevealerStore
    {
        private const string _prefsKey = "Tools.HierarchyRevealer.v3";
        private static HierarchyRevealerData _data;

        public static HierarchyRevealerData Data { get { if (_data == null) Load(); return _data; } }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Load()
        {
            var json = EditorPrefs.GetString(_prefsKey, string.Empty);
            _data = string.IsNullOrEmpty(json) ? new HierarchyRevealerData() : (JsonUtility.FromJson<HierarchyRevealerData>(json) ?? new HierarchyRevealerData());
        }

        public static void Save()
        {
            if (_data == null) _data = new HierarchyRevealerData();
            EditorPrefs.SetString(_prefsKey, JsonUtility.ToJson(_data));
        }

        public static void ResetCache() { _data = null; } // used by runtime init

        public static void Add(GameObject go)
        {
            if (go == null) return;
            var entry = HierarchyPathRef.From(go);

            var exists = Data.Targets.Any(t => t.ScenePath == entry.ScenePath &&
                                               t.PathTokens.Count == entry.PathTokens.Count &&
                                               !t.PathTokens.Where((p, i) => p != entry.PathTokens[i]).Any());
            if (!exists)
            {
                Data.Targets.Add(entry);
                if (Data.AutoSave) Save();
            }
        }

        public static void RemoveAt(int index)
        {
            if (index < 0 || index >= Data.Targets.Count) return;
            Data.Targets.RemoveAt(index);
            if (Data.AutoSave) Save();
        }

        public static void Clear() { Data.Targets.Clear(); Save(); }

        public static GameObject Resolve(HierarchyPathRef tr)
        {
            if (tr == null || tr.PathTokens == null || tr.PathTokens.Count == 0) return null;

            var scenes = Enumerable.Range(0, SceneManager.sceneCount).Select(SceneManager.GetSceneAt);

            if (!string.IsNullOrEmpty(tr.ScenePath))
            {
                var sc = scenes.FirstOrDefault(s => s.path == tr.ScenePath);
                if (sc.IsValid() && sc.isLoaded)
                {
                    var go = ResolveInScene(sc, tr.PathTokens);
                    if (go != null) return go;
                }
            }

            foreach (var sc in scenes)
            {
                if (!sc.isLoaded) continue;
                var go = ResolveInScene(sc, tr.PathTokens);
                if (go != null) return go;
            }
            return null;
        }

        private static GameObject ResolveInScene(Scene scene, List<string> tokens)
        {
            if (!scene.IsValid() || !scene.isLoaded || tokens == null || tokens.Count == 0) return null;

            var roots = scene.GetRootGameObjects();
            var current = roots.FirstOrDefault(r => r.name == tokens[0]);
            if (current == null) return null;

            for (int i = 1; i < tokens.Count; i++)
            {
                var name = tokens[i];
                current = current.transform.Cast<Transform>().Select(c => c.gameObject).FirstOrDefault(g => g.name == name);
                if (current == null) return null;
            }
            return current;
        }
    }


    internal static class HierarchyRevealerHooks
    {
        private static bool _registered = false;
        private static UnityEngine.Object _pendingPing = null;

        [InitializeOnLoadMethod]
        private static void EditorLoad()
        {
            Unregister();
            Register();
        }

        // Replace the following methods inside HierarchyRevealerHooks

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit()
        {
            _registered = false;
            _pendingPing = null;

            // Ensure all static event handlers are deregistered on domain reload
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeReload;
            EditorApplication.quitting -= OnQuitting;
            EditorApplication.delayCall -= PingNow; // Fix UDR0003: cleanup static callback

            HierarchyRevealerStore.ResetCache(); // ensure fresh after domain reload
        }

        private static void Register()
        {
            if (_registered) return;

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeReload;
            EditorApplication.quitting += OnQuitting;

            _registered = true;
        }

        private static void Unregister()
        {
            if (!_registered) return;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeReload;
            EditorApplication.quitting -= OnQuitting;

            _registered = false;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
                HierarchyRevealerStore.Save();
        }

        private static void OnBeforeReload()
        {
            HierarchyRevealerStore.Save();

            // Extra safety: also remove delayCall to avoid stale callbacks
            EditorApplication.delayCall -= PingNow;

            Unregister();
        }

        private static void OnQuitting()
        {
            HierarchyRevealerStore.Save();

            // Extra safety: also remove delayCall to avoid stale callbacks
            EditorApplication.delayCall -= PingNow;

            Unregister();
        }
        
        public static void RequestPing(UnityEngine.Object obj)
        {
            _pendingPing = obj;
            EditorApplication.delayCall -= PingNow;
            EditorApplication.delayCall += PingNow;
        }
        
        private static void PingNow()
        {
            var obj = _pendingPing;
            _pendingPing = null;
            EditorApplication.delayCall -= PingNow;
            if (obj != null) EditorGUIUtility.PingObject(obj);
        }
    }

    public static class HierarchyRevealer
    {
        [MenuItem("Tools/Reveal Targets In Hierarchy _%#E", priority = 1)]
        public static void RevealFromMenu() { RevealConfiguredTargets(); }

        public static void RevealConfiguredTargets()
        {
            var resolved = HierarchyRevealerStore.Data.Targets
                .Select(HierarchyRevealerStore.Resolve)
                .Where(go => go != null)
                .Distinct()
                .ToArray();

            if (resolved.Length == 0)
            {
                Debug.Log("[HierarchyRevealer] No resolvable targets. Scene not loaded or objects renamed?");
                return;
            }

            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            Selection.objects = resolved;
            HierarchyRevealerHooks.RequestPing(resolved[0]);
        }
    }

    [EditorToolbarElement("Tools/Reveal Targets", typeof(SceneView))]
    public sealed class RevealTargetsToolbarButton : EditorToolbarButton
    {
        public RevealTargetsToolbarButton()
        {
            text = "Reveal Targets";
            tooltip = "Selects configured targets and expands the Hierarchy (Edit & Play).";
            clicked += HierarchyRevealer.RevealConfiguredTargets;
        }
    }

    public sealed class HierarchyRevealerWindow : EditorWindow
    {
        private VisualElement _root;
        private ListView _listView;
        private Toggle _autoSaveToggle;

        private sealed class RowHandlers
        {
            public EventCallback<ClickEvent> PingCb;
            public EventCallback<ClickEvent> RemoveCb;
        }

        [MenuItem("Tools/Hierarchy Revealer Window", priority = 0)]
        public static void ShowWindow()
        {
            var wnd = GetWindow<HierarchyRevealerWindow>();
            wnd.titleContent = new GUIContent("Hierarchy Revealer");
            wnd.Show();
        }

        private void OnEnable()
        {
            _root = rootVisualElement;
            _root.style.paddingLeft = 8; _root.style.paddingRight = 8;
            _root.style.paddingTop = 8;  _root.style.paddingBottom = 8;
            BuildUI();
        }

        private void BuildUI()
        {
            _root.Clear();

            var header = new Label("Hierarchy Revealer");
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.fontSize = 14;
            header.style.marginBottom = 6;
            _root.Add(header);

            var help = new HelpBox("Assign targets below. Changes auto-save (Edit & Play). Click Reveal or use the toolbar button / hotkey.", HelpBoxMessageType.Info);
            _root.Add(help);

            _autoSaveToggle = new Toggle("Auto-Save Changes") { value = HierarchyRevealerStore.Data.AutoSave };
            _autoSaveToggle.RegisterValueChangedCallback(e => { HierarchyRevealerStore.Data.AutoSave = e.newValue; HierarchyRevealerStore.Save(); });
            _root.Add(_autoSaveToggle);

            _listView = new ListView
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                selectionType = SelectionType.None,
                showBorder = true,
                reorderable = false,
                style = { height = 220, marginTop = 6 }
            };

            _listView.makeItem = () =>
            {
                var row = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center, paddingLeft = 4, paddingRight = 4 } };
                var nameLabel = new Label { style = { flexGrow = 1 } };
                var pingBtn = new Button { text = "Ping", style = { width = 60, marginRight = 4 } };
                var removeBtn = new Button { text = "Remove", style = { width = 80 } };
                row.Add(nameLabel); row.Add(pingBtn); row.Add(removeBtn);
                row.userData = new Tuple<Label, Button, Button, RowHandlers>(nameLabel, pingBtn, removeBtn, new RowHandlers());
                return row;
            };

            _listView.bindItem = (row, i) =>
            {
                var tuple = (Tuple<Label, Button, Button, RowHandlers>)row.userData;
                var nameLabel = tuple.Item1;
                var pingBtn = tuple.Item2;
                var removeBtn = tuple.Item3;
                var handlers = tuple.Item4;

                if (handlers.PingCb != null)  pingBtn.UnregisterCallback(handlers.PingCb);
                if (handlers.RemoveCb != null) removeBtn.UnregisterCallback(handlers.RemoveCb);

                var entry = HierarchyRevealerStore.Data.Targets.ElementAtOrDefault(i);
                var go = entry != null ? HierarchyRevealerStore.Resolve(entry) : null;

                nameLabel.text = go != null ? go.name : "<Missing>";

                handlers.PingCb = (ClickEvent _) => { if (go != null) EditorGUIUtility.PingObject(go); };
                handlers.RemoveCb = (ClickEvent _) =>
                {
                    HierarchyRevealerStore.RemoveAt(i);
                    RefreshListView();
                };

                pingBtn.RegisterCallback(handlers.PingCb);
                removeBtn.RegisterCallback(handlers.RemoveCb);
            };

            _root.Add(_listView);
            RefreshListView();

            var addRow = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 8 } };
            var addField = new ObjectField("Add Target") { objectType = typeof(GameObject), allowSceneObjects = true, style = { flexGrow = 1 } };
            var addBtn = new Button { text = "Add", style = { width = 80, marginLeft = 6 } };
            addBtn.clicked += () =>
            {
                var go = addField.value as GameObject;
                if (go != null)
                {
                    HierarchyRevealerStore.Add(go);
                    addField.value = null;
                    RefreshListView();
                }
            };
            addRow.Add(addField); addRow.Add(addBtn);
            _root.Add(addRow);

            _root.RegisterCallback<DragUpdatedEvent>(e =>
            {
                if (DragAndDrop.objectReferences.Any(o => o is GameObject))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    e.StopImmediatePropagation();
                }
            });
            _root.RegisterCallback<DragPerformEvent>(e =>
            {
                foreach (var o in DragAndDrop.objectReferences)
                    if (o is GameObject go) HierarchyRevealerStore.Add(go);
                RefreshListView();
                e.StopImmediatePropagation();
            });

            var actions = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 8 } };
            var revealBtn = new Button(() =>
            {
                HierarchyRevealerStore.Save();
                HierarchyRevealer.RevealConfiguredTargets();
            }) { text = "Reveal Now" };
            revealBtn.style.flexGrow = 1;

            var saveBtn = new Button(HierarchyRevealerStore.Save) { text = "Save" };
            saveBtn.style.width = 90; saveBtn.style.marginLeft = 6;

            var clearBtn = new Button(() =>
            {
                HierarchyRevealerStore.Clear();
                RefreshListView();
            }) { text = "Clear" };
            clearBtn.style.width = 90; clearBtn.style.marginLeft = 6;

            actions.Add(revealBtn); actions.Add(saveBtn); actions.Add(clearBtn);
            _root.Add(actions);

            var footer = new Label("Hotkey: Tools ▸ Tools ▸ Reveal Targets In Hierarchy (Ctrl/Cmd+Shift+E).");
            footer.style.marginTop = 8; footer.style.opacity = 0.8f; footer.style.fontSize = 10;
            _root.Add(footer);
        }

        private void RefreshListView()
        {
            _listView.itemsSource = HierarchyRevealerStore.Data.Targets.ToList();
            _listView.Rebuild();
        }
    }
}
#endif