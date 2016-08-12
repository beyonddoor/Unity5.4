using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEditor.Graphs.AnimationBlendTree;
using UnityEditor.Graphs.AnimationStateMachine;
using UnityEngine;

namespace UnityEditor.Graphs
{
	[EditorWindowTitle(title = "Animator", icon = "UnityEditor.Graphs.AnimatorControllerTool")]
	internal class AnimatorControllerTool : EditorWindow, IAnimatorControllerEditor
	{
		[Serializable]
		private class BreadCrumbElement
		{
			[SerializeField]
			private UnityEngine.Object m_Target;

			public UnityEngine.Object target
			{
				get
				{
					return this.m_Target;
				}
			}

			public string name
			{
				get
				{
					return (!this.m_Target) ? string.Empty : this.m_Target.name;
				}
			}

			public BreadCrumbElement(UnityEngine.Object target)
			{
				this.m_Target = target;
			}
		}

		private class Styles
		{
			public readonly GUIStyle nameLabel = new GUIStyle("miniLabel");

			public readonly GUIStyle liveLinkLabel = new GUIStyle("miniLabel");

			public readonly GUIStyle bottomBarDarkBg = "In BigTitle";

			public readonly GUIContent layers = EditorGUIUtility.TextContent("Layers|Click to edit controller's layers.");

			public readonly GUIContent parameters = EditorGUIUtility.TextContent("Parameters|Click to edit controller's parameters.");

			public readonly GUIStyle invisibleButton = "InvisibleButton";

			public readonly GUIStyle lockButtonStyle = "IN LockButton";

			public readonly GUIStyle breadCrumbLeft = "GUIEditor.BreadcrumbLeft";

			public readonly GUIStyle breadCrumbMid = "GUIEditor.BreadcrumbMid";

			public readonly GUIContent visibleON = EditorGUIUtility.IconContent("animationvisibilitytoggleon");

			public readonly GUIContent visibleOFF = EditorGUIUtility.IconContent("animationvisibilitytoggleoff");

			public Styles()
			{
				this.nameLabel.alignment = TextAnchor.MiddleRight;
				this.nameLabel.padding = new RectOffset(0, 0, 0, 0);
				this.nameLabel.margin = new RectOffset(0, 0, 0, 0);
				this.liveLinkLabel.alignment = TextAnchor.MiddleLeft;
				this.liveLinkLabel.padding = new RectOffset(0, 0, 0, 0);
				this.liveLinkLabel.margin = new RectOffset(0, 0, 0, 0);
			}
		}

		private class ScopedPreventWarnings : IDisposable
		{
			private bool m_WasLoggingWarning;

			private Animator m_Animator;

			public ScopedPreventWarnings(Animator animator)
			{
				this.m_Animator = animator;
				if (this.m_Animator)
				{
					this.m_WasLoggingWarning = this.m_Animator.logWarnings;
					this.m_Animator.logWarnings = false;
				}
			}

			public virtual void Dispose()
			{
				if (this.m_Animator)
				{
					this.m_Animator.logWarnings = this.m_WasLoggingWarning;
				}
			}
		}

		private const float kToolbarHeight = 17f;

		private const float kBottombarHeight = 18f;

		private const int sLayerTab = 0;

		private const int sParameterTab = 1;

		[SerializeField]
		private Animator m_PreviewAnimator;

		[SerializeField]
		private AnimatorController m_AnimatorController;

		public static Action graphDirtyCallback;

		[SerializeField]
		private List<AnimatorControllerTool.BreadCrumbElement> m_BreadCrumbs;

		[SerializeField]
		public UnityEditor.Graphs.AnimationStateMachine.Graph stateMachineGraph;

		[SerializeField]
		public UnityEditor.Graphs.AnimationStateMachine.GraphGUI stateMachineGraphGUI;

		[SerializeField]
		public UnityEditor.Graphs.AnimationBlendTree.Graph blendTreeGraph;

		[SerializeField]
		public UnityEditor.Graphs.AnimationBlendTree.GraphGUI blendTreeGraphGUI;

		[SerializeField]
		private bool m_AutoLiveLink = true;

		[SerializeField]
		private bool m_MiniTool;

		[SerializeField]
		private bool m_SerializedLocked;

		public static AnimatorControllerTool tool;

		protected SplitterState m_VerticalSplitter;

		private static AnimatorControllerTool.Styles s_Styles;

		[SerializeField]
		private int m_CurrentEditor;

		[SerializeField]
		private LayerControllerView m_LayerEditor;

		private ParameterControllerView m_ParameterEditor;

		public Animator previewAnimator
		{
			get
			{
				return this.m_PreviewAnimator;
			}
		}

		public AnimatorController animatorController
		{
			get
			{
				return this.m_AnimatorController;
			}
			set
			{
				if (this.IsPreviewController(value))
				{
					return;
				}
				this.m_AnimatorController = value;
				this.editor.ResetUI();
				this.ResetBreadCrumbs();
				if (this.m_PreviewAnimator)
				{
					if (this.m_PreviewAnimator.runtimeAnimatorController != null && this.m_PreviewAnimator.runtimeAnimatorController != this.m_AnimatorController)
					{
						this.m_PreviewAnimator = null;
					}
					else if (!AnimatorController.FindAnimatorControllerPlayable(this.m_PreviewAnimator, this.m_AnimatorController).IsValid())
					{
						this.m_PreviewAnimator = null;
					}
				}
			}
		}

		public int selectedLayerIndex
		{
			get
			{
				return (this.m_LayerEditor == null) ? 0 : this.m_LayerEditor.selectedLayerIndex;
			}
			set
			{
				if (this.m_LayerEditor != null)
				{
					this.m_LayerEditor.selectedLayerIndex = value;
				}
			}
		}

		public bool autoLiveLink
		{
			get
			{
				return this.m_AutoLiveLink;
			}
		}

		public bool miniTool
		{
			get
			{
				return this.m_MiniTool;
			}
			set
			{
				this.m_MiniTool = value;
			}
		}

		public bool isLocked
		{
			get
			{
				return !(this.animatorController == null) && this.m_SerializedLocked;
			}
			set
			{
				this.m_SerializedLocked = value;
			}
		}

		public bool liveLink
		{
			get
			{
				return EditorApplication.isPlaying && this.m_PreviewAnimator != null && this.m_PreviewAnimator.enabled && this.m_PreviewAnimator.gameObject.activeInHierarchy;
			}
		}

		protected GraphGUI activeGraphGUI
		{
			get
			{
				if (this.m_BreadCrumbs.Count == 0)
				{
					return null;
				}
				if (this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target is AnimatorStateMachine)
				{
					return this.stateMachineGraphGUI;
				}
				return this.blendTreeGraphGUI;
			}
		}

		protected int currentEditor
		{
			get
			{
				return this.m_CurrentEditor;
			}
			set
			{
				this.editor.OnDisable();
				this.m_CurrentEditor = value;
				this.editor.OnEnable();
			}
		}

		protected IAnimatorControllerSubEditor editor
		{
			get
			{
				int currentEditor = this.m_CurrentEditor;
				if (currentEditor != 0)
				{
					if (currentEditor == 1)
					{
						return this.m_ParameterEditor;
					}
				}
				return this.m_LayerEditor;
			}
		}

		[MenuItem("Window/Animator", false, 2012)]
		public static void DoWindow()
		{
			EditorWindow.GetWindow<AnimatorControllerTool>(new Type[]
			{
				typeof(SceneView)
			});
		}

		public void OnSelectionChange()
		{
			if (!this.isLocked)
			{
				this.DetectAnimatorControllerFromSelection();
				this.DetectPreviewObjectFromSelection();
				base.Repaint();
			}
		}

		public void OnFocus()
		{
			this.Init();
			this.DetectAnimatorControllerFromSelection();
			this.DetectPreviewObjectFromSelection();
			this.editor.OnFocus();
		}

		public void OnProjectChange()
		{
			this.DetectAnimatorControllerFromSelection();
		}

		public void OnInvalidateAnimatorController()
		{
			this.editor.ResetUI();
			base.Repaint();
			if (this.stateMachineGraph != null)
			{
				if (this.stateMachineGraph.activeStateMachine == null)
				{
					this.RebuildGraph();
				}
				this.stateMachineGraph.ReadNodePositions();
			}
			if (this.blendTreeGraph != null)
			{
				this.blendTreeGraph.rootBlendTree = null;
			}
		}

		private bool IsPreviewController(AnimatorController controller)
		{
			return controller && controller.name == string.Empty && controller.layers.Length > 0 && controller.layers[0].name == "preview" && controller.hideFlags == HideFlags.DontSave;
		}

		private void DetectAnimatorControllerFromSelection()
		{
			AnimatorController animatorController = null;
			if (Selection.activeObject == null && this.animatorController == null)
			{
				this.animatorController = null;
			}
			if (Selection.activeObject is AnimatorController && EditorUtility.IsPersistent(Selection.activeObject))
			{
				animatorController = (Selection.activeObject as AnimatorController);
			}
			if (Selection.activeGameObject)
			{
				Animator component = Selection.activeGameObject.GetComponent<Animator>();
				if (component && !AnimatorController.FindAnimatorControllerPlayable(component, this.animatorController).IsValid())
				{
					AnimatorController effectiveAnimatorController = AnimatorController.GetEffectiveAnimatorController(component);
					if (effectiveAnimatorController)
					{
						animatorController = effectiveAnimatorController;
					}
				}
			}
			if (animatorController != null && animatorController != this.animatorController)
			{
				if (this.IsPreviewController(animatorController))
				{
					return;
				}
				this.animatorController = animatorController;
				if (this.animatorController == null)
				{
					return;
				}
			}
		}

		public void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			this.Init();
			this.DetectAnimatorControllerFromSelection();
			BlendTreeInspector.blendParameterInputChanged = (Action<BlendTree>)Delegate.Combine(BlendTreeInspector.blendParameterInputChanged, new Action<BlendTree>(this.BlendParameterInputChanged));
			BlendTreeInspector.currentController = this.m_AnimatorController;
			BlendTreeInspector.currentAnimator = this.m_PreviewAnimator;
			this.editor.OnEnable();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void OnDisable()
		{
			BlendTreeInspector.blendParameterInputChanged = (Action<BlendTree>)Delegate.Remove(BlendTreeInspector.blendParameterInputChanged, new Action<BlendTree>(this.BlendParameterInputChanged));
			this.editor.OnDisable();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void UndoRedoPerformed()
		{
			this.StateDirty();
			this.stateMachineGraphGUI.SyncGraphToUnitySelection();
			this.blendTreeGraphGUI.SyncGraphToUnitySelection();
		}

		private void OnLostFocus()
		{
			this.editor.OnLostFocus();
		}

		public void BlendTreeHierarchyChanged(BlendTree blendTree)
		{
			this.blendTreeGraph.BuildFromBlendTree(this.blendTreeGraph.rootBlendTree);
			base.Repaint();
		}

		public void BlendParameterInputChanged(BlendTree blendTree)
		{
			base.Repaint();
		}

		private void StateDirty()
		{
			this.RebuildGraph();
		}

		private bool ValidateBreadCrumbs()
		{
			int count = this.m_BreadCrumbs.Count;
			if (count <= 1)
			{
				return true;
			}
			UnityEngine.Object target = this.m_BreadCrumbs.First<AnimatorControllerTool.BreadCrumbElement>().target;
			int i;
			for (i = 1; i < count; i++)
			{
				UnityEngine.Object target2 = this.m_BreadCrumbs[i].target;
				if (target is AnimatorStateMachine)
				{
					if (target2 is AnimatorState && !(target as AnimatorStateMachine).HasState(target2 as AnimatorState))
					{
						break;
					}
					if (target2 is AnimatorStateMachine && !(target as AnimatorStateMachine).HasStateMachine(target2 as AnimatorStateMachine))
					{
						break;
					}
				}
				else
				{
					if (target is BlendTree && target2 is BlendTree && !(target as BlendTree).HasChild(target2 as BlendTree, true))
					{
						break;
					}
					if (target is AnimatorState && target2 is BlendTree && !((target as AnimatorState).motion as BlendTree).HasChild(target2 as BlendTree, true))
					{
						break;
					}
				}
				target = this.m_BreadCrumbs[i].target;
			}
			if (i < count)
			{
				this.m_BreadCrumbs.RemoveRange(i, count - i);
				return false;
			}
			return true;
		}

		public void RebuildGraph()
		{
			if (!this.ValidateBreadCrumbs())
			{
				this.UpdateStateMachineSelection();
			}
			this.stateMachineGraph.RebuildGraph();
			if (AnimatorControllerTool.graphDirtyCallback != null)
			{
				AnimatorControllerTool.graphDirtyCallback();
			}
		}

		private void Init()
		{
			if (this.m_LayerEditor == null)
			{
				this.m_LayerEditor = new LayerControllerView();
			}
			this.m_LayerEditor.Init(this);
			if (this.m_ParameterEditor == null)
			{
				this.m_ParameterEditor = new ParameterControllerView();
				this.m_ParameterEditor.Init(this);
			}
			if (this.stateMachineGraph == null)
			{
				this.stateMachineGraph = ScriptableObject.CreateInstance<UnityEditor.Graphs.AnimationStateMachine.Graph>();
				this.stateMachineGraph.hideFlags = HideFlags.HideAndDontSave;
			}
			if (this.stateMachineGraphGUI == null)
			{
				this.stateMachineGraphGUI = (this.stateMachineGraph.GetEditor() as UnityEditor.Graphs.AnimationStateMachine.GraphGUI);
			}
			if (this.blendTreeGraph == null)
			{
				this.blendTreeGraph = ScriptableObject.CreateInstance<UnityEditor.Graphs.AnimationBlendTree.Graph>();
				this.blendTreeGraph.hideFlags = HideFlags.HideAndDontSave;
			}
			if (this.blendTreeGraphGUI == null)
			{
				this.blendTreeGraphGUI = (this.blendTreeGraph.GetEditor() as UnityEditor.Graphs.AnimationBlendTree.GraphGUI);
			}
			if (this.m_BreadCrumbs == null)
			{
				this.m_BreadCrumbs = new List<AnimatorControllerTool.BreadCrumbElement>();
				this.ResetBreadCrumbs();
			}
			AnimatorControllerTool.tool = this;
		}

		public void ResetUI()
		{
			this.ResetBreadCrumbs();
		}

		private void DetectPreviewObjectFromSelection()
		{
			if (Selection.activeGameObject)
			{
				Animator component = Selection.activeGameObject.GetComponent<Animator>();
				if (component && (AnimatorController.GetEffectiveAnimatorController(component) != null || component.runtimeAnimatorController != null) && !AssetDatabase.Contains(Selection.activeGameObject))
				{
					this.m_PreviewAnimator = component;
				}
			}
		}

		private void ResetBreadCrumbs()
		{
			this.m_BreadCrumbs.Clear();
			if (this.animatorController == null || this.animatorController.layers.Length == 0)
			{
				return;
			}
			if (this.animatorController.isAssetBundled)
			{
				return;
			}
			if (this.selectedLayerIndex == -1)
			{
				this.selectedLayerIndex = 0;
			}
			if (this.selectedLayerIndex < this.animatorController.layers.Length)
			{
				int syncedLayerIndex = this.animatorController.layers[this.selectedLayerIndex].syncedLayerIndex;
				AnimatorStateMachine animatorStateMachine = (syncedLayerIndex != -1) ? this.animatorController.layers[syncedLayerIndex].stateMachine : this.animatorController.layers[this.selectedLayerIndex].stateMachine;
				if (animatorStateMachine == null)
				{
					return;
				}
				this.AddBreadCrumb(animatorStateMachine);
				this.stateMachineGraphGUI.ClearSelection();
				this.blendTreeGraphGUI.ClearSelection();
			}
			base.Repaint();
		}

		public void AddBreadCrumb(UnityEngine.Object target)
		{
			this.m_BreadCrumbs.Add(new AnimatorControllerTool.BreadCrumbElement(target));
			this.stateMachineGraphGUI.CenterGraph();
			this.blendTreeGraphGUI.CenterGraph();
		}

		private void UpdateStateMachineSelection()
		{
			AnimatorStateMachine activeStateMachine = this.stateMachineGraph.activeStateMachine;
			AnimatorStateMachine animatorStateMachine = this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target as AnimatorStateMachine;
			if (animatorStateMachine != activeStateMachine && animatorStateMachine != null)
			{
				this.stateMachineGraph.SetStateMachines(animatorStateMachine, this.GetParentStateMachine(), this.m_BreadCrumbs.First<AnimatorControllerTool.BreadCrumbElement>().target as AnimatorStateMachine);
				this.stateMachineGraphGUI.ClearSelection();
				this.blendTreeGraphGUI.ClearSelection();
				InspectorWindow.RepaintAllInspectors();
			}
		}

		public void GoToBreadCrumbTarget(UnityEngine.Object target)
		{
			int num = this.m_BreadCrumbs.FindIndex((AnimatorControllerTool.BreadCrumbElement o) => o.target == target);
			while (this.m_BreadCrumbs.Count > num + 1)
			{
				this.m_BreadCrumbs.RemoveAt(num + 1);
			}
			this.stateMachineGraphGUI.CenterGraph();
			this.blendTreeGraphGUI.CenterGraph();
			this.UpdateStateMachineSelection();
		}

		public void AddLayer(string layerName)
		{
			Undo.RegisterCompleteObjectUndo(this.animatorController, "Layer Added");
			this.animatorController.AddLayer(layerName);
			this.selectedLayerIndex = this.animatorController.layers.Length - 1;
		}

		public void AddNewLayer()
		{
			this.AddLayer("New Layer");
		}

		private void GoToParentBreadcrumb()
		{
			if (this.m_BreadCrumbs.Count <= 1)
			{
				return;
			}
			this.m_BreadCrumbs.RemoveAt(this.m_BreadCrumbs.Count - 1);
			this.stateMachineGraphGUI.CenterGraph();
			this.blendTreeGraphGUI.CenterGraph();
			this.UpdateStateMachineSelection();
		}

		public void BuildBreadCrumbsFromSMHierarchy(IEnumerable<AnimatorStateMachine> hierarchy)
		{
			this.m_BreadCrumbs.Clear();
			foreach (AnimatorStateMachine current in hierarchy)
			{
				this.AddBreadCrumb(current);
			}
		}

		private void DoGraphBottomBar(Rect nameRect)
		{
			GUILayout.BeginArea(nameRect);
			GUILayout.BeginHorizontal(AnimatorControllerTool.s_Styles.bottomBarDarkBg, new GUILayoutOption[0]);
			if (this.liveLink && this.previewAnimator != null)
			{
				GUILayout.Label(this.previewAnimator.name, AnimatorControllerTool.s_Styles.liveLinkLabel, new GUILayoutOption[0]);
			}
			if (this.animatorController != null)
			{
				string text = "Assets/";
				string text2 = AssetDatabase.GetAssetPath(this.animatorController);
				if (text2.StartsWith(text))
				{
					text2 = text2.Remove(0, text.Length);
				}
				GUILayout.Label(text2, AnimatorControllerTool.s_Styles.nameLabel, new GUILayoutOption[0]);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void SetCurrentLayer(object i)
		{
			this.selectedLayerIndex = (int)i;
		}

		private void DoGraphToolbar(Rect toolbarRect)
		{
			GUILayout.BeginArea(toolbarRect);
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			if (this.miniTool)
			{
				if (GUILayout.Button(AnimatorControllerTool.s_Styles.visibleOFF, AnimatorControllerTool.s_Styles.invisibleButton, new GUILayoutOption[0]))
				{
					this.miniTool = false;
				}
				GUILayout.Space(10f);
			}
			AnimatorControllerTool.BreadCrumbElement[] array = this.m_BreadCrumbs.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				AnimatorControllerTool.BreadCrumbElement breadCrumbElement = array[i];
				if (this.miniTool && i == 0)
				{
					if (EditorGUILayout.ButtonMouseDown(new GUIContent(breadCrumbElement.name), FocusType.Keyboard, EditorStyles.toolbarPopup, new GUILayoutOption[0]))
					{
						AnimatorControllerLayer[] layers = this.animatorController.layers;
						GenericMenu genericMenu = new GenericMenu();
						for (int j = 0; j < layers.Length; j++)
						{
							genericMenu.AddItem(new GUIContent(layers[j].name), false, new GenericMenu.MenuFunction2(this.SetCurrentLayer), j);
						}
						genericMenu.AddSeparator(string.Empty);
						genericMenu.AddItem(new GUIContent("Create New Layer"), false, new GenericMenu.MenuFunction(this.AddNewLayer));
						genericMenu.ShowAsContext();
					}
				}
				else
				{
					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle(i == array.Length - 1, breadCrumbElement.name, (i != 0) ? AnimatorControllerTool.s_Styles.breadCrumbMid : AnimatorControllerTool.s_Styles.breadCrumbLeft, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						this.GoToBreadCrumbTarget(breadCrumbElement.target);
					}
				}
			}
			GUILayout.FlexibleSpace();
			using (new EditorGUI.DisabledScope(this.animatorController == null))
			{
				if (Unsupported.IsDeveloperBuild() && GUILayout.Button("Select Graph", EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					if (this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target is AnimatorStateMachine)
					{
						Selection.activeObject = this.stateMachineGraph;
					}
					else
					{
						Selection.activeObject = this.blendTreeGraph;
					}
				}
			}
			AnimatorControllerTool.BreadCrumbElement breadCrumbElement2 = this.m_BreadCrumbs.LastOrDefault<AnimatorControllerTool.BreadCrumbElement>();
			if (breadCrumbElement2 != null && breadCrumbElement2.target is AnimatorStateMachine)
			{
				this.m_AutoLiveLink = GUILayout.Toggle(this.m_AutoLiveLink, "Auto Live Link", EditorStyles.toolbarButton, new GUILayoutOption[0]);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Lock"), this.isLocked, new GenericMenu.MenuFunction(this.FlipLocked));
		}

		private void FlipLocked()
		{
			this.isLocked = !this.isLocked;
			if (!this.isLocked)
			{
				this.OnSelectionChange();
			}
		}

		private void ShowButton(Rect position)
		{
			if (AnimatorControllerTool.s_Styles == null)
			{
				AnimatorControllerTool.s_Styles = new AnimatorControllerTool.Styles();
			}
			bool flag = GUI.Toggle(position, this.isLocked, GUIContent.none, AnimatorControllerTool.s_Styles.lockButtonStyle);
			if (flag != this.isLocked)
			{
				this.FlipLocked();
			}
		}

		private void OnControllerChange()
		{
			if (this.m_PreviewAnimator != null)
			{
				if (EditorApplication.isPlaying && !this.m_PreviewAnimator.runtimeAnimatorController)
				{
					return;
				}
				AnimatorController effectiveAnimatorController = AnimatorController.GetEffectiveAnimatorController(this.m_PreviewAnimator);
				if (effectiveAnimatorController != null && effectiveAnimatorController != this.animatorController)
				{
					this.animatorController = effectiveAnimatorController;
				}
			}
		}

		public void OnGUIEditor(Rect editorRect)
		{
			this.editor.OnEvent();
			GUILayout.BeginArea(editorRect);
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[]
			{
				GUILayout.Height(17f)
			});
			GUILayout.Space(10f);
			GUILayout.FlexibleSpace();
			this.editor.OnToolbarGUI();
			GUILayout.EndHorizontal();
			this.editor.OnGUI(editorRect);
			GUILayout.EndArea();
			if (Event.current.type == EventType.ContextClick && editorRect.Contains(Event.current.mousePosition))
			{
				Event.current.Use();
			}
		}

		public void OnGUIEditorToolbar(Rect topToolBarRect)
		{
			GUILayout.BeginArea(topToolBarRect);
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[]
			{
				GUILayout.Height(17f)
			});
			GUILayout.Space(4f);
			EditorGUI.BeginChangeCheck();
			GUILayout.Toggle(this.currentEditor == 0, AnimatorControllerTool.s_Styles.layers, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.currentEditor = 0;
			}
			GUILayout.Space(4f);
			EditorGUI.BeginChangeCheck();
			GUILayout.Toggle(this.currentEditor == 1, AnimatorControllerTool.s_Styles.parameters, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.currentEditor = 1;
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(AnimatorControllerTool.s_Styles.visibleON, AnimatorControllerTool.s_Styles.invisibleButton, new GUILayoutOption[0]))
			{
				this.miniTool = true;
			}
			GUILayout.Space(4f);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		public void OnGUI()
		{
			using (new AnimatorControllerTool.ScopedPreventWarnings(this.previewAnimator))
			{
				EventType type = Event.current.type;
				int button = Event.current.button;
				base.autoRepaintOnSceneChange = true;
				if (AnimatorControllerTool.s_Styles == null)
				{
					AnimatorControllerTool.s_Styles = new AnimatorControllerTool.Styles();
				}
				this.OnControllerChange();
				BlendTreeInspector.currentController = this.m_AnimatorController;
				BlendTreeInspector.currentAnimator = this.m_PreviewAnimator;
				if (this.miniTool)
				{
					Rect paneRect = new Rect(0f, 0f, base.position.width, base.position.height);
					this.OnGUIGraph(paneRect);
				}
				else
				{
					if (this.m_VerticalSplitter == null || this.m_VerticalSplitter.realSizes.Length != 2)
					{
						this.m_VerticalSplitter = new SplitterState(new int[]
						{
							(int)(base.position.width * 0.25f),
							(int)(base.position.width * 0.75f)
						}, new int[]
						{
							150,
							100
						}, null);
					}
					SplitterGUILayout.BeginHorizontalSplit(this.m_VerticalSplitter, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(true),
						GUILayout.ExpandHeight(true)
					});
					SplitterGUILayout.EndHorizontalSplit();
					int num = this.m_VerticalSplitter.realSizes[0];
					int num2 = this.m_VerticalSplitter.realSizes[1];
					Rect rect = new Rect(0f, 0f, (float)num, base.position.height);
					Rect paneRect2 = new Rect((float)num, 0f, (float)num2, base.position.height);
					Rect topToolBarRect = new Rect(rect.x, rect.y, rect.width, 17f);
					this.OnGUIEditorToolbar(topToolBarRect);
					Rect editorRect = new Rect(0f, 17f, rect.width, rect.height - 17f);
					this.OnGUIEditor(editorRect);
					if (Event.current.type == EventType.MouseDown && this.editor.HasKeyboardControl())
					{
						this.editor.ReleaseKeyboardFocus();
					}
					else if (this.activeGraphGUI != null && type == EventType.MouseDown && type != Event.current.type)
					{
						this.activeGraphGUI.ClearSelection();
					}
					this.OnGUIGraph(paneRect2);
				}
				if (Event.current.type == EventType.MouseDown)
				{
					GUIUtility.keyboardControl = 0;
					EditorGUI.EndEditingActiveTextField();
				}
				if (this.activeGraphGUI != null && type == EventType.MouseDown && button == 0 && this.activeGraphGUI.selection.Count == 0 && this.activeGraphGUI.edgeGUI.edgeSelection.Count == 0)
				{
					this.activeGraphGUI.DoBackgroundClickAction();
				}
			}
		}

		private void OnGUIGraph(Rect paneRect)
		{
			Rect toolbarRect = new Rect(paneRect.x, paneRect.y, paneRect.width, 17f);
			Rect rect = new Rect(paneRect.x, 17f, paneRect.width, paneRect.height - 17f - 18f);
			Rect nameRect = new Rect(paneRect.x, paneRect.height - 18f, paneRect.width, 18f);
			EventType type = Event.current.type;
			this.DoGraphToolbar(toolbarRect);
			if (this.animatorController != null)
			{
				if (this.animatorController.isAssetBundled)
				{
					GUILayout.BeginArea(rect);
					GUILayout.Label("Cannot show controller from asset bundle", new GUILayoutOption[0]);
					GUILayout.EndArea();
					return;
				}
				BlendTreeInspector.parentBlendTree = null;
				if (this.m_BreadCrumbs.Count > 0)
				{
					if (this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target is AnimatorStateMachine)
					{
						this.StateMachineView(rect);
					}
					if (this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target is AnimatorState)
					{
						AnimatorState state = this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target as AnimatorState;
						this.BlendTreeView(rect);
						Motion stateEffectiveMotion = AnimatorControllerTool.tool.animatorController.GetStateEffectiveMotion(state, AnimatorControllerTool.tool.selectedLayerIndex);
						BlendTreeInspector.parentBlendTree = (stateEffectiveMotion as BlendTree);
					}
					if (this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target is BlendTree)
					{
						this.BlendTreeView(rect);
						BlendTreeInspector.parentBlendTree = (this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target as BlendTree);
					}
				}
				if (this.activeGraphGUI != null && type == EventType.MouseDown && Event.current.type == type && Event.current.clickCount == 1)
				{
					this.activeGraphGUI.ClearSelection();
				}
				if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && Event.current.button == 0)
				{
					this.GoToParentBreadcrumb();
					Event.current.Use();
				}
			}
			else
			{
				this.StateMachineView(rect);
			}
			this.DoGraphBottomBar(nameRect);
		}

		private void BlendTreeView(Rect position)
		{
			this.blendTreeGraph.previewAvatar = this.m_PreviewAnimator;
			Motion motion;
			if (this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target is AnimatorState)
			{
				motion = AnimatorControllerTool.tool.animatorController.GetStateEffectiveMotion(this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target as AnimatorState, AnimatorControllerTool.tool.selectedLayerIndex);
			}
			else
			{
				motion = (this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target as Motion);
			}
			this.blendTreeGraph.rootBlendTree = (motion as BlendTree);
			this.blendTreeGraphGUI.BeginGraphGUI(this, position);
			this.blendTreeGraphGUI.OnGraphGUI();
			this.blendTreeGraphGUI.EndGraphGUI();
		}

		private void StateMachineView(Rect position)
		{
			if (this.animatorController != null)
			{
				this.stateMachineGraph.SetStateMachines(this.m_BreadCrumbs.Last<AnimatorControllerTool.BreadCrumbElement>().target as AnimatorStateMachine, this.GetParentStateMachine(), this.m_BreadCrumbs.First<AnimatorControllerTool.BreadCrumbElement>().target as AnimatorStateMachine);
			}
			this.stateMachineGraphGUI.BeginGraphGUI(this, position);
			if (this.animatorController != null)
			{
				this.stateMachineGraphGUI.OnGraphGUI();
			}
			this.stateMachineGraphGUI.EndGraphGUI();
		}

		private AnimatorStateMachine GetParentStateMachine()
		{
			if (this.m_BreadCrumbs.Count == 1)
			{
				return null;
			}
			return this.m_BreadCrumbs[this.m_BreadCrumbs.Count - 2].target as AnimatorStateMachine;
		}

		virtual void Repaint()
		{
			base.Repaint();
		}
	}
}
