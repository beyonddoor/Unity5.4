using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs
{
	[InitializeOnLoad]
	internal class LayerSettingsWindow : EditorWindow
	{
		private class Styles
		{
			public readonly GUIContent weight = EditorGUIUtility.TextContent("Weight|Change layer default weight.");

			public readonly GUIContent blending = EditorGUIUtility.TextContent("Blending|Choose between Override and Additive layer.");

			public readonly GUIContent sync = EditorGUIUtility.TextContent("Sync|Synchronize this layer with another layer.");

			public readonly GUIContent timing = EditorGUIUtility.TextContent("Timing|When active, the layer will take control of the duration of the Synced Layer.");

			public readonly GUIContent ik = EditorGUIUtility.TextContent("IK Pass|When active, the layer will have an IK pass when evaluated. It will trigger an OnAnimatorIK callback.");

			public readonly GUIContent sourceLayer = EditorGUIUtility.TextContent("Source Layer|Specifies the source of the Synced Layer.");

			public readonly GUIContent mask = EditorGUIUtility.TextContent("Mask|The AvatarMask that is used to mask the animation on the given layer.");
		}

		private static LayerSettingsWindow.Styles s_Styles;

		private AnimatorControllerLayer m_Layer;

		private AnimatorController m_Controller;

		private int m_LayerIndex;

		public static LayerSettingsWindow s_LayerSettingsWindow;

		private static long s_LastClosedTime;

		public AnimatorControllerLayer layer
		{
			get
			{
				return this.m_Layer;
			}
		}

		public int layerIndex
		{
			get
			{
				return this.m_LayerIndex;
			}
		}

		private Vector2 windowSize
		{
			get
			{
				return new Vector2(250f, EditorGUIUtility.singleLineHeight * 7f);
			}
		}

		private void OnEnable()
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		private void OnDisable()
		{
			LayerSettingsWindow.s_LayerSettingsWindow = null;
			LayerSettingsWindow.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		private void UndoRedoPerformed()
		{
			AnimatorControllerLayer[] layers = this.m_Controller.layers;
			if (this.m_LayerIndex < layers.Length)
			{
				this.m_Layer = layers[this.m_LayerIndex];
				base.Repaint();
			}
			else
			{
				this.m_Layer = null;
				Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
				base.Close();
			}
		}

		internal static bool ShowAtPosition(Rect buttonRect, AnimatorControllerLayer layer, int layerIndex, AnimatorController controller)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num >= LayerSettingsWindow.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (LayerSettingsWindow.s_LayerSettingsWindow == null)
				{
					LayerSettingsWindow.s_LayerSettingsWindow = ScriptableObject.CreateInstance<LayerSettingsWindow>();
				}
				LayerSettingsWindow.s_LayerSettingsWindow.m_Layer = layer;
				LayerSettingsWindow.s_LayerSettingsWindow.m_LayerIndex = layerIndex;
				LayerSettingsWindow.s_LayerSettingsWindow.m_Controller = controller;
				LayerSettingsWindow.s_LayerSettingsWindow.Init(buttonRect);
				return true;
			}
			return false;
		}

		private void Init(Rect buttonRect)
		{
			buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
			base.ShowAsDropDown(buttonRect, this.windowSize, new PopupLocationHelper.PopupLocation[]
			{
				PopupLocationHelper.PopupLocation.Right
			});
			base.Focus();
			this.m_Parent.AddToAuxWindowList();
			base.wantsMouseMove = true;
		}

		private void OnEditorUpdate()
		{
		}

		internal void OnGUI()
		{
			if (LayerSettingsWindow.s_Styles == null)
			{
				LayerSettingsWindow.s_Styles = new LayerSettingsWindow.Styles();
			}
			if (this.m_Layer == null)
			{
				return;
			}
			bool flag = false;
			GUI.Box(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, new GUIStyle("grey_border"));
			AnimatorControllerLayer[] layers = this.m_Controller.layers;
			EditorGUIUtility.labelWidth = 100f;
			using (new EditorGUI.DisabledScope(this.m_LayerIndex == 0))
			{
				if (AnimatorControllerTool.tool.liveLink)
				{
					float value = (this.m_LayerIndex != 0) ? AnimatorControllerTool.tool.previewAnimator.GetLayerWeight(this.m_LayerIndex) : 1f;
					AnimatorControllerTool.tool.previewAnimator.SetLayerWeight(this.m_LayerIndex, EditorGUILayout.Slider(LayerSettingsWindow.s_Styles.weight, value, 0f, 1f, new GUILayoutOption[0]));
				}
				else
				{
					EditorGUI.BeginChangeCheck();
					float num = (this.m_LayerIndex != 0) ? this.m_Layer.defaultWeight : 1f;
					num = EditorGUILayout.Slider(LayerSettingsWindow.s_Styles.weight, num, 0f, 1f, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck() && this.m_LayerIndex != 0)
					{
						this.m_Layer.defaultWeight = num;
						flag = true;
					}
				}
			}
			EditorGUI.BeginChangeCheck();
			this.m_Layer.avatarMask = (EditorGUILayout.ObjectField(LayerSettingsWindow.s_Styles.mask, this.m_Layer.avatarMask, typeof(AvatarMask), false, new GUILayoutOption[0]) as AvatarMask);
			this.m_Layer.blendingMode = (AnimatorLayerBlendingMode)EditorGUILayout.EnumPopup(LayerSettingsWindow.s_Styles.blending, this.m_Layer.blendingMode, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				flag = true;
			}
			int selectedIndex = 0;
			List<GUIContent> list = new List<GUIContent>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < layers.Length; i++)
			{
				AnimatorControllerLayer animatorControllerLayer = layers[i];
				if (this.m_LayerIndex != i && animatorControllerLayer.syncedLayerIndex == -1)
				{
					list.Add(new GUIContent(animatorControllerLayer.name));
					list2.Add(i);
					if (i == this.m_Layer.syncedLayerIndex)
					{
						selectedIndex = list.Count - 1;
					}
				}
			}
			using (new EditorGUI.DisabledScope(list.Count == 0))
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				bool flag2 = EditorGUILayout.Toggle(LayerSettingsWindow.s_Styles.sync, this.m_Layer.syncedLayerIndex > -1, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				GUI.enabled = (flag2 && this.m_Layer.blendingMode == AnimatorLayerBlendingMode.Override);
				EditorGUI.BeginChangeCheck();
				this.m_Layer.syncedLayerAffectsTiming = EditorGUILayout.Toggle(LayerSettingsWindow.s_Styles.timing, this.m_Layer.syncedLayerAffectsTiming, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					flag = true;
				}
				GUI.enabled = true;
				GUILayout.EndHorizontal();
				if (flag2)
				{
					int num2 = EditorGUILayout.Popup(LayerSettingsWindow.s_Styles.sourceLayer, selectedIndex, list.ToArray(), new GUILayoutOption[0]);
					if (num2 < list2.Count && list2[num2] != this.m_Layer.syncedLayerIndex)
					{
						this.m_Layer.syncedLayerIndex = list2[num2];
						flag = true;
					}
				}
				else if (this.m_Layer.syncedLayerIndex != -1)
				{
					this.m_Layer.syncedLayerIndex = -1;
					flag = true;
				}
			}
			EditorGUI.BeginChangeCheck();
			this.m_Layer.iKPass = EditorGUILayout.Toggle(LayerSettingsWindow.s_Styles.ik, this.m_Layer.iKPass, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				flag = true;
			}
			if (flag)
			{
				Undo.RegisterCompleteObjectUndo(this.m_Controller, "Layer settings Changed");
				layers[this.m_LayerIndex] = this.m_Layer;
				this.m_Controller.layers = layers;
				this.m_Layer = this.m_Controller.layers[this.m_LayerIndex];
				AnimatorControllerTool.tool.ResetUI();
			}
		}
	}
}
