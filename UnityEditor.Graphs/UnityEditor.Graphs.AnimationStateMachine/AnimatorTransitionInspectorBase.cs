using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal abstract class AnimatorTransitionInspectorBase : Editor
	{
		private class Styles
		{
			public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");

			public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");

			public GUIContent errorIcon = EditorGUIUtility.IconContent("console.erroricon.sml");

			public GUIContent previewTitle = new GUIContent("Preview");

			public readonly GUIStyle background = new GUIStyle("IN Label");

			public readonly GUIStyle draggingHandle = "WindowBottomResize";

			public readonly GUIStyle headerBackground = "TE Toolbar";

			public readonly GUIStyle footerBackground = "preLabel";

			public readonly GUIStyle boxBackground = "TE NodeBackground";

			public readonly GUIStyle preButton = "preButton";

			public GUIStyle preBackground = "preBackground";
		}

		private const int toggleColumnWidth = 30;

		protected ReorderableList m_TransitionList;

		protected SerializedObject m_SerializedTransition;

		protected SerializedProperty m_Name;

		protected SerializedProperty m_Conditions;

		protected ReorderableList m_ConditionList;

		protected UnityEditor.Animations.AnimatorController m_Controller;

		protected Animator m_PreviewObject;

		protected int m_LayerIndex;

		protected TransitionEditionContext[] m_TransitionContexts;

		protected static List<AnimatorConditionMode> m_intModes;

		protected static List<AnimatorConditionMode> m_floatModes;

		private static AnimatorTransitionInspectorBase.Styles s_Styles;

		public bool showTransitionList = true;

		private bool m_SingleTransitionHeader;

		private bool m_SyncTransitionContexts;

		public virtual void OnEnable()
		{
			this.m_TransitionList = new ReorderableList(base.targets, typeof(AnimatorTransitionBase), false, true, false, true);
			this.m_TransitionList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.OnSelectTransition);
			this.m_TransitionList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.OnRemoveTransition);
			this.m_TransitionList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawTransitionElement);
			this.m_TransitionList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(AnimatorTransitionInspectorBase.DrawTransitionHeaderCommon);
			this.m_TransitionList.index = 0;
			this.m_PreviewObject = ((!AnimatorControllerTool.tool) ? null : AnimatorControllerTool.tool.previewAnimator);
			this.m_Controller = ((!AnimatorControllerTool.tool) ? null : AnimatorControllerTool.tool.animatorController);
			if (this.m_Controller)
			{
				this.m_LayerIndex = AnimatorControllerTool.tool.selectedLayerIndex;
				UnityEditor.Animations.AnimatorController expr_F7 = this.m_Controller;
				expr_F7.OnAnimatorControllerDirty = (Action)Delegate.Combine(expr_F7.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
			}
			if (AnimatorTransitionInspectorBase.m_intModes == null)
			{
				AnimatorTransitionInspectorBase.m_intModes = new List<AnimatorConditionMode>();
				AnimatorTransitionInspectorBase.m_intModes.Add(AnimatorConditionMode.Greater);
				AnimatorTransitionInspectorBase.m_intModes.Add(AnimatorConditionMode.Less);
				AnimatorTransitionInspectorBase.m_intModes.Add(AnimatorConditionMode.Equals);
				AnimatorTransitionInspectorBase.m_intModes.Add(AnimatorConditionMode.NotEqual);
			}
			if (AnimatorTransitionInspectorBase.m_floatModes == null)
			{
				AnimatorTransitionInspectorBase.m_floatModes = new List<AnimatorConditionMode>();
				AnimatorTransitionInspectorBase.m_floatModes.Add(AnimatorConditionMode.Greater);
				AnimatorTransitionInspectorBase.m_floatModes.Add(AnimatorConditionMode.Less);
			}
			this.m_SyncTransitionContexts = true;
		}

		public virtual void OnDisable()
		{
			if (this.m_Controller)
			{
				UnityEditor.Animations.AnimatorController expr_16 = this.m_Controller;
				expr_16.OnAnimatorControllerDirty = (Action)Delegate.Remove(expr_16.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
			}
		}

		public virtual void OnDestroy()
		{
		}

		protected virtual void ControllerDirty()
		{
		}

		public void SetTransitionContext(TransitionEditionContext context)
		{
			this.m_TransitionContexts = new TransitionEditionContext[]
			{
				context
			};
			this.m_SerializedTransition = null;
			this.SetTransitionToInspect(base.targets[0] as AnimatorTransitionBase);
			this.m_SyncTransitionContexts = false;
		}

		protected void SyncTransitionContexts()
		{
			if (this.m_SyncTransitionContexts)
			{
				this.ComputeTransitionContexts();
				this.SetTransitionToInspect(base.targets[0] as AnimatorTransitionBase);
				this.m_SyncTransitionContexts = false;
			}
		}

		protected void ComputeTransitionContexts()
		{
			this.m_TransitionContexts = new TransitionEditionContext[base.targets.Length];
			Graph graph = (!AnimatorControllerTool.tool) ? null : AnimatorControllerTool.tool.stateMachineGraph;
			GraphGUI graphGUI = (!AnimatorControllerTool.tool) ? null : AnimatorControllerTool.tool.stateMachineGraphGUI;
			for (int i = 0; i < base.targets.Length; i++)
			{
				AnimatorTransitionBase animatorTransitionBase = base.targets[i] as AnimatorTransitionBase;
				this.m_TransitionContexts[i] = new TransitionEditionContext(animatorTransitionBase, null, null, null);
				if (graph != null && graphGUI != null)
				{
					foreach (int current in graphGUI.edgeGUI.edgeSelection)
					{
						EdgeInfo edgeInfo = graph.GetEdgeInfo(graph.edges[current]);
						foreach (TransitionEditionContext current2 in edgeInfo.transitions)
						{
							if (current2.transition == animatorTransitionBase)
							{
								this.m_TransitionContexts[i] = current2;
							}
						}
					}
				}
			}
		}

		public static void DrawTransitionHeaderCommon(Rect rect)
		{
			rect.xMax -= 60f;
			GUI.Label(rect, "Transitions");
			rect.xMin = rect.xMax;
			rect.width = 30f;
			GUI.Label(rect, "Solo");
			rect.xMin = rect.xMax;
			rect.width = 30f;
			GUI.Label(rect, "Mute");
		}

		public static void DrawTransitionElementCommon(Rect rect, TransitionEditionContext transitionContext, bool selected, bool focused)
		{
			rect.xMax -= 60f;
			if (transitionContext.transition == null)
			{
				GUI.Label(rect, new GUIContent("Not Found"));
				return;
			}
			bool flag = transitionContext.transition.solo;
			bool flag2 = transitionContext.transition.mute;
			GUI.Label(rect, new GUIContent(transitionContext.displayName, transitionContext.fullName));
			rect.xMin = rect.xMax;
			rect.width = 30f;
			flag = GUI.Toggle(rect, flag, string.Empty);
			rect.xMin = rect.xMax;
			rect.width = 30f;
			flag2 = GUI.Toggle(rect, flag2, string.Empty);
			if (flag != transitionContext.transition.solo)
			{
				Undo.RegisterCompleteObjectUndo(transitionContext.transition, "Solo changed");
				transitionContext.transition.solo = flag;
			}
			if (flag2 != transitionContext.transition.mute)
			{
				Undo.RegisterCompleteObjectUndo(transitionContext.transition, "Mute changed");
				transitionContext.transition.mute = flag2;
			}
		}

		private void DrawTransitionElement(Rect rect, int index, bool selected, bool focused)
		{
			AnimatorTransitionInspectorBase.DrawTransitionElementCommon(rect, this.m_TransitionContexts[index], selected, focused);
		}

		protected void OnRemoveTransition(ReorderableList list)
		{
			int index = list.index;
			if (list.index >= list.list.Count - 1)
			{
				list.index = list.list.Count - 1;
			}
			this.m_TransitionContexts[index].Remove(true);
			AnimatorControllerTool.tool.RebuildGraph();
			GUIUtility.ExitGUI();
		}

		private void OnSelectTransition(ReorderableList list)
		{
			this.SetTransitionToInspect(base.targets[list.index] as AnimatorTransitionBase);
		}

		protected virtual void InitSerializedProperties()
		{
			this.m_Conditions = this.m_SerializedTransition.FindProperty("m_Conditions");
			this.m_Name = this.m_SerializedTransition.FindProperty("m_Name");
		}

		protected virtual void SetTransitionToInspect(AnimatorTransitionBase transition)
		{
			if ((this.m_SerializedTransition != null && this.m_SerializedTransition.targetObject == transition) || transition == null)
			{
				return;
			}
			this.m_SerializedTransition = new SerializedObject(transition);
			if (this.m_SerializedTransition == null)
			{
				return;
			}
			this.InitSerializedProperties();
			this.m_ConditionList = new ReorderableList(this.m_SerializedTransition, this.m_Conditions);
			this.m_ConditionList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawConditionsElement);
			this.m_ConditionList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddConditionInList);
			this.m_ConditionList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawConditionsHeader);
		}

		internal override void OnHeaderIconGUI(Rect iconRect)
		{
			Texture2D miniThumbnail = AssetPreview.GetMiniThumbnail(this.target);
			GUI.Label(iconRect, miniThumbnail);
		}

		internal override void OnHeaderTitleGUI(Rect titleRect, string header)
		{
			this.SyncTransitionContexts();
			if (!this.m_SingleTransitionHeader)
			{
				Rect position = titleRect;
				position.height = 16f;
				EditorGUI.LabelField(position, this.m_TransitionContexts[this.m_TransitionList.index].displayName);
				position.y += 18f;
				string arg = (base.targets.Length != 1) ? "Transitions" : "AnimatorTransitionBase";
				EditorGUI.LabelField(position, base.targets.Length + " " + arg);
				return;
			}
			Rect position2 = titleRect;
			position2.height = 16f;
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_Name.hasMultipleDifferentValues;
			string name = EditorGUI.DelayedTextField(position2, this.m_Name.stringValue, EditorStyles.textField);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				ObjectNames.SetNameSmart(this.m_SerializedTransition.targetObject, name);
			}
		}

		internal override void OnHeaderControlsGUI()
		{
			if (!this.m_SingleTransitionHeader)
			{
				base.OnHeaderControlsGUI();
				return;
			}
			GUILayout.Label(this.m_TransitionContexts[this.m_TransitionList.index].displayName, new GUILayoutOption[0]);
		}

		internal override void DrawHeaderHelpAndSettingsGUI(Rect r)
		{
			if (!this.m_SingleTransitionHeader)
			{
				base.DrawHeaderHelpAndSettingsGUI(r);
				return;
			}
			if (Help.HasHelpForObject(this.m_SerializedTransition.targetObject) && GUI.Button(new Rect(r.width - 36f, r.y + 5f, 14f, 14f), EditorGUI.GUIContents.helpIcon, EditorStyles.inspectorTitlebarText))
			{
				Help.ShowHelpForObject(this.m_SerializedTransition.targetObject);
			}
			Rect position = new Rect(r.width - 18f, r.y + 5f, 14f, 14f);
			if (EditorGUI.ButtonMouseDown(position, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Native, EditorStyles.inspectorTitlebarText))
			{
				EditorUtility.DisplayObjectContextMenu(position, this.m_SerializedTransition.targetObject, 0);
			}
		}

		public override GUIContent GetPreviewTitle()
		{
			return AnimatorTransitionInspectorBase.s_Styles.previewTitle;
		}

		public override void DrawPreview(Rect previewPosition)
		{
			this.OnInteractivePreviewGUI(previewPosition, AnimatorTransitionInspectorBase.s_Styles.preBackground);
		}

		protected virtual void DoPreview()
		{
		}

		protected virtual void DoErrorAndWarning()
		{
		}

		public override void OnInspectorGUI()
		{
			this.SyncTransitionContexts();
			AnimatorTransitionInspectorBase.InitStyles();
			if (this.showTransitionList)
			{
				this.m_TransitionList.DoLayoutList();
			}
			if (this.m_SerializedTransition == null)
			{
				return;
			}
			this.m_SerializedTransition.Update();
			this.m_SingleTransitionHeader = true;
			Editor.DrawHeaderGUI(this, string.Empty);
			this.m_SingleTransitionHeader = false;
			this.DoPreview();
			EditorGUI.indentLevel = 0;
			GUILayout.Space(10f);
			if (this.m_ConditionList != null)
			{
				this.m_ConditionList.DoLayoutList();
			}
			this.m_SerializedTransition.ApplyModifiedProperties();
			this.DoErrorAndWarning();
		}

		private static void InitStyles()
		{
			if (AnimatorTransitionInspectorBase.s_Styles == null)
			{
				AnimatorTransitionInspectorBase.s_Styles = new AnimatorTransitionInspectorBase.Styles();
			}
		}

		private void DrawConditionsHeader(Rect headerRect)
		{
			GUI.Label(headerRect, EditorGUIUtility.TempContent("Conditions"));
		}

		private void AddConditionInList(ReorderableList list)
		{
			AnimatorTransitionBase animatorTransitionBase = this.m_SerializedTransition.targetObject as AnimatorTransitionBase;
			string parameter = string.Empty;
			AnimatorConditionMode mode = AnimatorConditionMode.Greater;
			if (this.m_Controller)
			{
				UnityEngine.AnimatorControllerParameter[] parameters = this.m_Controller.parameters;
				if (parameters.Length > 0)
				{
					parameter = parameters[0].name;
					mode = ((parameters[0].type != UnityEngine.AnimatorControllerParameterType.Float && parameters[0].type != UnityEngine.AnimatorControllerParameterType.Int) ? AnimatorConditionMode.If : AnimatorConditionMode.Greater);
				}
			}
			animatorTransitionBase.AddCondition(mode, 0f, parameter);
		}

		private void DrawConditionsElement(Rect rect, int index, bool selected, bool focused)
		{
			SerializedProperty arrayElementAtIndex = this.m_Conditions.GetArrayElementAtIndex(index);
			AnimatorConditionMode animatorConditionMode = (AnimatorConditionMode)arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue;
			int num = 3;
			Rect rect2 = new Rect(rect.x, rect.y + 2f, rect.width, rect.height - 5f);
			Rect position = rect2;
			position.xMax -= rect2.width / 2f + (float)num;
			Rect rect3 = rect2;
			rect3.xMin += rect2.width / 2f + (float)num;
			Rect rect4 = rect3;
			rect4.xMax -= rect3.width / 2f + (float)num;
			Rect position2 = rect3;
			position2.xMin += rect3.width / 2f + (float)num;
			string stringValue = arrayElementAtIndex.FindPropertyRelative("m_ConditionEvent").stringValue;
			int num2 = (!this.m_Controller) ? -1 : this.m_Controller.IndexOfParameter(stringValue);
			bool flag = false;
			List<string> list = new List<string>();
			UnityEngine.AnimatorControllerParameter[] array = null;
			if (this.m_Controller)
			{
				array = this.m_Controller.parameters;
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(array[i].name);
				}
			}
			string text = EditorGUI.DelayedTextFieldDropDown(position, stringValue, list.ToArray());
			if (stringValue != text)
			{
				num2 = this.m_Controller.IndexOfParameter(text);
				arrayElementAtIndex.FindPropertyRelative("m_ConditionEvent").stringValue = text;
				animatorConditionMode = AnimatorConditionMode.Greater;
				arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue = (int)animatorConditionMode;
				flag = true;
			}
			UnityEngine.AnimatorControllerParameterType animatorControllerParameterType = (num2 == -1) ? ((UnityEngine.AnimatorControllerParameterType)(-1)) : array[num2].type;
			if (num2 != -1 && (animatorControllerParameterType == UnityEngine.AnimatorControllerParameterType.Float || animatorControllerParameterType == UnityEngine.AnimatorControllerParameterType.Int))
			{
				List<AnimatorConditionMode> list2 = (animatorControllerParameterType != UnityEngine.AnimatorControllerParameterType.Float) ? AnimatorTransitionInspectorBase.m_intModes : AnimatorTransitionInspectorBase.m_floatModes;
				string[] array2 = new string[list2.Count];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = list2[j].ToString();
				}
				int num3 = -1;
				for (int k = 0; k < array2.Length; k++)
				{
					if (animatorConditionMode.ToString() == array2[k])
					{
						num3 = k;
					}
				}
				if (num3 == -1)
				{
					Vector2 vector = GUI.skin.label.CalcSize(AnimatorTransitionInspectorBase.s_Styles.errorIcon);
					Rect position3 = rect4;
					position3.xMax = position3.xMin + vector.x;
					rect4.xMin += vector.x;
					GUI.Label(position3, AnimatorTransitionInspectorBase.s_Styles.errorIcon);
				}
				EditorGUI.BeginChangeCheck();
				num3 = EditorGUI.Popup(rect4, num3, array2);
				if (EditorGUI.EndChangeCheck() || flag)
				{
					arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue = (int)list2[num3];
				}
				EditorGUI.BeginChangeCheck();
				float num4 = arrayElementAtIndex.FindPropertyRelative("m_EventTreshold").floatValue;
				if (animatorControllerParameterType == UnityEngine.AnimatorControllerParameterType.Float)
				{
					num4 = EditorGUI.FloatField(position2, num4);
				}
				else
				{
					num4 = (float)EditorGUI.IntField(position2, Mathf.FloorToInt(num4));
				}
				if (EditorGUI.EndChangeCheck() || flag)
				{
					arrayElementAtIndex.FindPropertyRelative("m_EventTreshold").floatValue = num4;
				}
			}
			else if (num2 != -1 && animatorControllerParameterType == UnityEngine.AnimatorControllerParameterType.Bool)
			{
				string[] displayedOptions = new string[]
				{
					"true",
					"false"
				};
				int num5 = (animatorConditionMode != AnimatorConditionMode.IfNot) ? 0 : 1;
				EditorGUI.BeginChangeCheck();
				num5 = EditorGUI.Popup(rect3, num5, displayedOptions);
				if (EditorGUI.EndChangeCheck() || flag)
				{
					arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue = ((num5 != 0) ? 2 : 1);
				}
			}
			else if (animatorControllerParameterType == UnityEngine.AnimatorControllerParameterType.Trigger)
			{
				if (flag)
				{
					arrayElementAtIndex.FindPropertyRelative("m_ConditionMode").intValue = 1;
				}
			}
			else
			{
				EditorGUI.LabelField(rect3, "Parameter does not exist in Controller");
			}
		}
	}
}
