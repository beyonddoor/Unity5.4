using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Graphs
{
	[Serializable]
	internal class LayerControllerView : IAnimatorControllerSubEditor
	{
		private class Styles
		{
			public readonly GUIContent addIcon = EditorGUIUtility.IconContent("Toolbar Plus");

			public readonly GUIStyle invisibleButton = "InvisibleButton";

			public readonly GUIContent settingsIcon = EditorGUIUtility.IconContent("SettingsIcon");

			public readonly GUIContent settings = EditorGUIUtility.TextContent("Settings|Click to change layer settings.");

			public readonly GUIStyle elementBackground = "RL Element";

			public readonly GUIStyle label = "label";

			public readonly GUIStyle layerLabel = new GUIStyle("miniLabel");

			public readonly GUIContent sync = EditorGUIUtility.TextContent("S|Layer is a Synchronized layer.");

			public readonly GUIContent syncTime = EditorGUIUtility.TextContent("S+T|Layer is a Synchronized layer and will take control of the duration for this Synced Layer.");

			public readonly GUIContent ik = EditorGUIUtility.TextContent("IK|When active, the layer will have an IK pass when evaluated. It will trigger an OnAnimatorIK callback.");

			public readonly GUIContent additive = EditorGUIUtility.TextContent("A|Additive Layer.");

			public readonly GUIContent mask = EditorGUIUtility.TextContent("M|Layer has an AvatarMask.");

			public readonly Color progressBackground = new Color(0.24f, 0.24f, 0.24f);

			public readonly Color progressLiveLink = new Color(0.298039228f, 0.698039234f, 1f);

			public readonly Color progressEdit = new Color(0.545098066f, 0.545098066f, 0.545098066f);

			public Styles()
			{
				this.settingsIcon.tooltip = this.settings.tooltip;
				this.layerLabel.alignment = TextAnchor.MiddleCenter;
				this.layerLabel.fontStyle = FontStyle.Bold;
				this.layerLabel.padding = new RectOffset(0, 0, 0, 0);
				this.layerLabel.margin = new RectOffset(0, 0, 0, 0);
			}
		}

		private const float kSpacing = 4f;

		private const int kSliderThickness = 1;

		private const float kElementHeight = 40f;

		private int m_LastSelectedIndex;

		[SerializeField]
		private int m_SelectedLayerIndex;

		private static LayerControllerView.Styles s_Styles;

		private ReorderableList m_LayerList;

		private Vector2 m_LayerScroll;

		private RenameOverlay m_RenameOverlay;

		private bool m_HadKeyFocusAtMouseDown;

		private IAnimatorControllerEditor m_Host;

		public int selectedLayerIndex
		{
			get
			{
				if (this.m_Host.animatorController != null && this.m_SelectedLayerIndex >= this.m_Host.animatorController.layers.Length)
				{
					this.m_SelectedLayerIndex = this.m_Host.animatorController.layers.Length - 1;
					this.m_LayerList.index = this.m_SelectedLayerIndex;
					this.m_Host.ResetUI();
				}
				return this.m_SelectedLayerIndex;
			}
			set
			{
				this.m_SelectedLayerIndex = value;
				this.m_LayerList.index = this.m_SelectedLayerIndex;
				this.m_Host.ResetUI();
			}
		}

		public RenameOverlay renameOverlay
		{
			get
			{
				if (this.m_RenameOverlay == null)
				{
					this.m_RenameOverlay = new RenameOverlay();
				}
				return this.m_RenameOverlay;
			}
		}

		public void OnEnable()
		{
			this.ResetUI();
		}

		public void OnDisable()
		{
			if (this.renameOverlay.IsRenaming())
			{
				this.RenameEnd();
			}
		}

		public void OnDestroy()
		{
		}

		public void OnFocus()
		{
		}

		public void OnLostFocus()
		{
			if (this.renameOverlay.IsRenaming())
			{
				this.renameOverlay.EndRename(true);
				this.RenameEnd();
			}
		}

		public void ReleaseKeyboardFocus()
		{
			this.m_LayerList.ReleaseKeyboardFocus();
		}

		public void GrabKeyboardFocus()
		{
			this.m_LayerList.GrabKeyboardFocus();
		}

		public bool HasKeyboardControl()
		{
			return this.m_LayerList.HasKeyboardControl();
		}

		public void ResetUI()
		{
			if (AnimatorControllerTool.tool == null || AnimatorControllerTool.tool.animatorController == null || AnimatorControllerTool.tool.animatorController.layers == null)
			{
				this.m_SelectedLayerIndex = 0;
				this.m_LastSelectedIndex = -1;
				this.m_LayerList.index = this.selectedLayerIndex;
				return;
			}
			if (this.selectedLayerIndex > AnimatorControllerTool.tool.animatorController.layers.Length)
			{
				this.m_SelectedLayerIndex = AnimatorControllerTool.tool.animatorController.layers.Length - 1;
			}
			if (this.m_LastSelectedIndex > AnimatorControllerTool.tool.animatorController.layers.Length)
			{
				this.m_LastSelectedIndex = AnimatorControllerTool.tool.animatorController.layers.Length - 1;
			}
			this.m_LayerList.index = this.m_SelectedLayerIndex;
			this.m_LayerScroll = Vector2.zero;
		}

		public void Init(IAnimatorControllerEditor host)
		{
			this.m_Host = host;
			if (this.m_LayerList == null)
			{
				this.m_LayerList = new ReorderableList((!(this.m_Host.animatorController != null)) ? new UnityEditor.Animations.AnimatorControllerLayer[0] : this.m_Host.animatorController.layers, typeof(UnityEditor.Animations.AnimatorControllerLayer), true, false, false, false);
				this.m_LayerList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.OnReorderLayer);
				this.m_LayerList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawLayer);
				this.m_LayerList.drawElementBackgroundCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawLayerBackground);
				this.m_LayerList.onMouseUpCallback = new ReorderableList.SelectCallbackDelegate(this.OnMouseUpLayer);
				this.m_LayerList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.OnSelectLayer);
				this.m_LayerList.index = this.selectedLayerIndex;
				this.m_LayerList.headerHeight = 0f;
				this.m_LayerList.footerHeight = 0f;
				this.m_LayerList.elementHeight = 40f;
			}
		}

		private void OnReorderLayer(ReorderableList reorderablelist)
		{
			UnityEditor.Animations.AnimatorControllerLayer[] layers = this.m_Host.animatorController.layers;
			UnityEditor.Animations.AnimatorControllerLayer[] array = reorderablelist.list as UnityEditor.Animations.AnimatorControllerLayer[];
			for (int i = 0; i < array.Length; i++)
			{
				int syncedLayerIndex = array[i].syncedLayerIndex;
				if (syncedLayerIndex != -1)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (layers[syncedLayerIndex].name == array[j].name)
						{
							array[i].syncedLayerIndex = j;
						}
					}
				}
			}
			string layerName = layers[this.selectedLayerIndex].name;
			int num = Array.FindIndex<UnityEditor.Animations.AnimatorControllerLayer>(array, (UnityEditor.Animations.AnimatorControllerLayer layer) => layer.name == layerName);
			if (num != -1)
			{
				this.selectedLayerIndex = num;
			}
			Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Layer reordering");
			this.m_Host.animatorController.layers = array;
			this.m_Host.Repaint();
		}

		private void DeleteLayer()
		{
			this.OnRemoveLayer(this.m_LayerList.index);
		}

		private void OnDrawLayer(Rect rect, int index, bool selected, bool focused)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseUp && current.button == 1 && rect.Contains(current.mousePosition))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteLayer));
				genericMenu.ShowAsContext();
				Event.current.Use();
			}
			UnityEditor.Animations.AnimatorControllerLayer animatorControllerLayer = this.m_LayerList.list[index] as UnityEditor.Animations.AnimatorControllerLayer;
			rect.yMin += 4f;
			rect.yMax -= 4f;
			Vector2 vector = LayerControllerView.s_Styles.invisibleButton.CalcSize(LayerControllerView.s_Styles.settingsIcon);
			Rect rect2 = new Rect(rect.xMax - vector.x - 4f, rect.yMin + 1f, vector.x, rect.height - 16f);
			Rect rect3 = rect2;
			if (GUI.Button(rect3, LayerControllerView.s_Styles.settingsIcon, LayerControllerView.s_Styles.invisibleButton))
			{
				Rect buttonRect = rect3;
				buttonRect.x += 15f;
				if (LayerSettingsWindow.ShowAtPosition(buttonRect, animatorControllerLayer, index, this.m_Host.animatorController))
				{
					GUIUtility.ExitGUI();
				}
			}
			if (animatorControllerLayer.syncedLayerIndex != -1)
			{
				Vector2 vector2 = LayerControllerView.s_Styles.layerLabel.CalcSize((!animatorControllerLayer.syncedLayerAffectsTiming) ? LayerControllerView.s_Styles.sync : LayerControllerView.s_Styles.syncTime);
				rect3 = new Rect(rect3.xMin - vector2.x - 4f, rect.yMin, vector2.x, rect.height - 16f);
				GUI.Label(rect3, (!animatorControllerLayer.syncedLayerAffectsTiming) ? LayerControllerView.s_Styles.sync : LayerControllerView.s_Styles.syncTime, LayerControllerView.s_Styles.layerLabel);
			}
			if (animatorControllerLayer.iKPass)
			{
				Vector2 vector3 = LayerControllerView.s_Styles.layerLabel.CalcSize(LayerControllerView.s_Styles.ik);
				rect3 = new Rect(rect3.xMin - vector3.x - 4f, rect.yMin, vector3.x, rect.height - 16f);
				GUI.Label(rect3, LayerControllerView.s_Styles.ik, LayerControllerView.s_Styles.layerLabel);
			}
			if (animatorControllerLayer.blendingMode == UnityEditor.Animations.AnimatorLayerBlendingMode.Additive)
			{
				Vector2 vector4 = LayerControllerView.s_Styles.layerLabel.CalcSize(LayerControllerView.s_Styles.additive);
				rect3 = new Rect(rect3.xMin - vector4.x - 4f, rect.yMin, vector4.x, rect.height - 16f);
				GUI.Label(rect3, LayerControllerView.s_Styles.additive, LayerControllerView.s_Styles.layerLabel);
			}
			if (animatorControllerLayer.avatarMask != null)
			{
				Vector2 vector5 = LayerControllerView.s_Styles.layerLabel.CalcSize(LayerControllerView.s_Styles.mask);
				rect3 = new Rect(rect3.xMin - vector5.x - 4f, rect.yMin, vector5.x, rect.height - 16f);
				GUI.Label(rect3, LayerControllerView.s_Styles.mask, LayerControllerView.s_Styles.layerLabel);
			}
			Rect rect4 = Rect.MinMaxRect(rect.xMin, rect.yMin, rect3.xMin - 4f, rect.yMax - 16f);
			float num = (float)LayerControllerView.s_Styles.label.padding.left;
			Rect rect5 = Rect.MinMaxRect(rect.xMin + num, rect.yMax - 11f, rect2.xMax, rect.yMax - 9f);
			bool flag = this.renameOverlay.IsRenaming() && this.renameOverlay.userData == index && !this.renameOverlay.isWaitingForDelay;
			if (flag)
			{
				if (rect4.width >= 0f && rect4.height >= 0f)
				{
					rect4.x -= 2f;
					this.renameOverlay.editFieldRect = rect4;
				}
				if (!this.renameOverlay.OnGUI())
				{
					this.RenameEnd();
				}
			}
			else
			{
				GUI.Label(rect4, animatorControllerLayer.name, LayerControllerView.s_Styles.label);
			}
			if (Event.current.type == EventType.Repaint)
			{
				float num2 = (index != 0) ? ((!this.m_Host.liveLink) ? animatorControllerLayer.defaultWeight : this.m_Host.previewAnimator.GetLayerWeight(index)) : 1f;
				Rect rect6 = rect5;
				rect6.width *= num2;
				EditorGUI.DrawRect(rect5, LayerControllerView.s_Styles.progressBackground);
				EditorGUI.DrawRect(rect6, (!this.m_Host.liveLink) ? LayerControllerView.s_Styles.progressEdit : LayerControllerView.s_Styles.progressLiveLink);
			}
		}

		private void OnDrawLayerBackground(Rect rect, int index, bool selected, bool focused)
		{
			if (Event.current.type == EventType.Repaint)
			{
				GUI.Box(Rect.MinMaxRect(rect.xMin + 1f, rect.yMin, rect.xMax - 3f, rect.yMax), string.Empty);
				LayerControllerView.s_Styles.elementBackground.Draw(rect, false, selected, selected, focused);
			}
		}

		private void OnMouseUpLayer(ReorderableList list)
		{
			if (this.m_HadKeyFocusAtMouseDown && list.index == this.m_LastSelectedIndex && Event.current.button == 0)
			{
				UnityEditor.Animations.AnimatorControllerLayer animatorControllerLayer = list.list[list.index] as UnityEditor.Animations.AnimatorControllerLayer;
				this.renameOverlay.BeginRename(animatorControllerLayer.name, list.index, 0.1f);
			}
			else if (AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine != null && !Selection.Contains(AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine))
			{
				Selection.objects = new List<UnityEngine.Object>
				{
					AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine
				}.ToArray();
			}
			this.m_LastSelectedIndex = list.index;
		}

		private void OnSelectLayer(ReorderableList list)
		{
			if (this.selectedLayerIndex != list.index)
			{
				this.selectedLayerIndex = list.index;
				this.m_Host.ResetUI();
				Selection.objects = new List<UnityEngine.Object>
				{
					AnimatorControllerTool.tool.stateMachineGraph.activeStateMachine
				}.ToArray();
			}
		}

		private void RenameEnd()
		{
			if (this.renameOverlay.userAcceptedRename)
			{
				string text = (!string.IsNullOrEmpty(this.renameOverlay.name)) ? this.renameOverlay.name : this.renameOverlay.originalName;
				if (text != this.renameOverlay.originalName)
				{
					int userData = this.renameOverlay.userData;
					UnityEditor.Animations.AnimatorControllerLayer animatorControllerLayer = this.m_LayerList.list[userData] as UnityEditor.Animations.AnimatorControllerLayer;
					AnimatorStateMachine stateMachine = animatorControllerLayer.stateMachine;
					text = this.m_Host.animatorController.MakeUniqueLayerName(text);
					if (stateMachine != null)
					{
						ObjectNames.SetNameSmart(stateMachine, text);
					}
					Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Layer renamed");
					animatorControllerLayer.name = text;
					this.m_Host.animatorController.layers = (this.m_LayerList.list as UnityEditor.Animations.AnimatorControllerLayer[]);
				}
			}
			this.m_LayerList.GrabKeyboardFocus();
			this.renameOverlay.Clear();
		}

		public void OnToolbarGUI()
		{
			if (LayerControllerView.s_Styles == null)
			{
				LayerControllerView.s_Styles = new LayerControllerView.Styles();
			}
			using (new EditorGUI.DisabledScope(this.m_Host.animatorController == null))
			{
				if (GUILayout.Button(LayerControllerView.s_Styles.addIcon, LayerControllerView.s_Styles.invisibleButton, new GUILayoutOption[0]))
				{
					AnimatorControllerTool animatorControllerTool = this.m_Host as AnimatorControllerTool;
					if (animatorControllerTool != null && animatorControllerTool.animatorController != null)
					{
						animatorControllerTool.AddNewLayer();
						this.m_LayerList.list = animatorControllerTool.animatorController.layers;
						this.m_LayerList.index = animatorControllerTool.selectedLayerIndex;
						this.selectedLayerIndex = this.m_LayerList.index;
						if (this.renameOverlay.IsRenaming())
						{
							this.RenameEnd();
						}
						this.renameOverlay.BeginRename(animatorControllerTool.animatorController.layers[this.selectedLayerIndex].name, this.selectedLayerIndex, 0.1f);
					}
				}
			}
		}

		private void KeyboardHandling()
		{
			if (!this.m_LayerList.HasKeyboardControl())
			{
				return;
			}
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.KeyDown)
			{
				if (type == EventType.ExecuteCommand)
				{
					if (current.commandName == "SoftDelete" || current.commandName == "Delete")
					{
						current.Use();
						this.OnRemoveLayer(this.m_LayerList.index);
					}
				}
			}
			else
			{
				KeyCode keyCode = Event.current.keyCode;
				if (keyCode != KeyCode.Home)
				{
					if (keyCode != KeyCode.End)
					{
						if (keyCode == KeyCode.Delete)
						{
							current.Use();
							this.OnRemoveLayer(this.m_LayerList.index);
						}
					}
					else
					{
						current.Use();
						this.m_LayerList.index = (this.m_SelectedLayerIndex = this.m_LayerList.count - 1);
						this.m_Host.ResetUI();
					}
				}
				else
				{
					current.Use();
					this.m_LayerList.index = (this.m_SelectedLayerIndex = 0);
					this.m_Host.ResetUI();
				}
			}
		}

		protected void OnRemoveLayer(int layerIndex)
		{
			int count = this.m_LayerList.list.Count;
			List<int> list = new List<int>();
			for (int i = count - 1; i >= 0; i--)
			{
				UnityEditor.Animations.AnimatorControllerLayer animatorControllerLayer = this.m_LayerList.list[i] as UnityEditor.Animations.AnimatorControllerLayer;
				if (animatorControllerLayer.syncedLayerIndex == layerIndex || i == layerIndex)
				{
					list.Add(i);
				}
			}
			if (list.Count > 0)
			{
				if (list.Count == count)
				{
					if (list.Count == 1)
					{
						Debug.LogError("You cannot remove all layers from an AnimatorController.");
					}
					else if (list.Count > 1)
					{
						Debug.LogError("Deleting this layer will also delete all the layers that are synchronized on it. This operation cannot be performed because it would remove all the layers on this controller.");
					}
				}
				else if (list.Count == 1 || EditorUtility.DisplayDialog("Deleting synchronized layer", "Deleting this layer will also delete all the layers that are synchronized on it. Are you sure you want to delete it?", "Delete", "Cancel"))
				{
					this.m_Host.animatorController.RemoveLayers(list);
					this.ResetUI();
					this.m_Host.ResetUI();
				}
			}
		}

		public void OnEvent()
		{
			this.renameOverlay.OnEvent();
		}

		public void OnGUI(Rect rect)
		{
			if (LayerControllerView.s_Styles == null)
			{
				LayerControllerView.s_Styles = new LayerControllerView.Styles();
			}
			this.KeyboardHandling();
			if (this.m_Host.animatorController != null)
			{
				this.m_LayerList.list = this.m_Host.animatorController.layers;
			}
			else if (this.m_LayerList.list.Count != 0)
			{
				this.m_LayerList.list = new UnityEditor.Animations.AnimatorControllerLayer[0];
			}
			Event current = Event.current;
			if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
			{
				this.m_HadKeyFocusAtMouseDown = this.m_LayerList.HasKeyboardControl();
			}
			this.m_LayerList.draggable = !this.m_Host.liveLink;
			this.m_LayerScroll = GUILayout.BeginScrollView(this.m_LayerScroll, new GUILayoutOption[0]);
			this.m_LayerList.DoLayoutList();
			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace();
		}
	}
}
