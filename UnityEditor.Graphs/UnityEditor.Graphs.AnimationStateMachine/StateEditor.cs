using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	[CanEditMultipleObjects, CustomEditor(typeof(AnimatorState))]
	internal class StateEditor : Editor
	{
		private const int kParameterMaxWitdh = 100;

		private SerializedProperty m_Name;

		private SerializedProperty m_Speed;

		private SerializedProperty m_CycleOffset;

		private SerializedProperty m_FootIK;

		private SerializedProperty m_WriteDefaultValues;

		private SerializedProperty m_Mirror;

		private SerializedProperty m_Tag;

		private SerializedProperty m_SpeedParameter;

		private SerializedProperty m_MirrorParameter;

		private SerializedProperty m_CycleOffsetParameter;

		private SerializedProperty m_SpeedParameterActive;

		private SerializedProperty m_CycleOffsetParameterActive;

		private SerializedProperty m_MirrorParameterActive;

		private SourceNodeTransitionEditor m_TransitionsEditor;

		private StateMachineBehaviorsEditor m_BehavioursEditor;

		private AnimatorController m_ControllerContext;

		private int m_LayerIndexContext;

		private AnimatorState state
		{
			get
			{
				return this.target as AnimatorState;
			}
		}

		private AnimatorController controllerContext
		{
			get
			{
				if (this.m_ControllerContext == null)
				{
					this.m_ControllerContext = ((!(AnimatorControllerTool.tool != null)) ? null : AnimatorControllerTool.tool.animatorController);
				}
				return this.m_ControllerContext;
			}
		}

		private void Init()
		{
			if (this.m_TransitionsEditor == null)
			{
				this.m_TransitionsEditor = new SourceNodeTransitionEditor(this.state, this);
			}
			if (this.m_BehavioursEditor == null)
			{
				this.m_BehavioursEditor = new StateMachineBehaviorsEditor(this.state, this);
			}
		}

		public void OnEnable()
		{
			this.Init();
			this.m_LayerIndexContext = ((!(AnimatorControllerTool.tool != null)) ? 0 : AnimatorControllerTool.tool.selectedLayerIndex);
			this.m_Name = base.serializedObject.FindProperty("m_Name");
			this.m_Speed = base.serializedObject.FindProperty("m_Speed");
			this.m_CycleOffset = base.serializedObject.FindProperty("m_CycleOffset");
			this.m_FootIK = base.serializedObject.FindProperty("m_IKOnFeet");
			this.m_WriteDefaultValues = base.serializedObject.FindProperty("m_WriteDefaultValues");
			this.m_Mirror = base.serializedObject.FindProperty("m_Mirror");
			this.m_Tag = base.serializedObject.FindProperty("m_Tag");
			this.m_SpeedParameter = base.serializedObject.FindProperty("m_SpeedParameter");
			this.m_CycleOffsetParameter = base.serializedObject.FindProperty("m_CycleOffsetParameter");
			this.m_MirrorParameter = base.serializedObject.FindProperty("m_MirrorParameter");
			this.m_SpeedParameterActive = base.serializedObject.FindProperty("m_SpeedParameterActive");
			this.m_CycleOffsetParameterActive = base.serializedObject.FindProperty("m_CycleOffsetParameterActive");
			this.m_MirrorParameterActive = base.serializedObject.FindProperty("m_MirrorParameterActive");
			this.m_TransitionsEditor.OnEnable();
			this.m_BehavioursEditor.OnEnable();
		}

		public void OnDisable()
		{
			this.m_TransitionsEditor.OnDisable();
			this.m_BehavioursEditor.OnDisable();
		}

		public void OnDestroy()
		{
			this.m_TransitionsEditor.OnDestroy();
			this.m_BehavioursEditor.OnDestroy();
		}

		internal override void OnHeaderTitleGUI(Rect titleRect, string header)
		{
			base.serializedObject.Update();
			Rect position = titleRect;
			position.height = 16f;
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_Name.hasMultipleDifferentValues;
			string text = EditorGUI.DelayedTextField(position, this.m_Name.stringValue, EditorStyles.textField);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(text) && text != this.m_Name.stringValue)
			{
				AnimatorStateMachine animatorStateMachine = this.controllerContext.FindEffectiveRootStateMachine(this.m_LayerIndexContext);
				UnityEngine.Object[] targets = base.targets;
				for (int i = 0; i < targets.Length; i++)
				{
					UnityEngine.Object @object = targets[i];
					AnimatorStateMachine animatorStateMachine2 = animatorStateMachine.FindStateMachine(@object as AnimatorState);
					if (animatorStateMachine2 != null)
					{
						ObjectNames.SetNameSmart(@object, animatorStateMachine2.MakeUniqueStateName(text));
					}
				}
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		internal override void OnHeaderControlsGUI()
		{
			base.serializedObject.Update();
			EditorGUIUtility.labelWidth = 30f;
			EditorGUILayout.PropertyField(this.m_Tag, new GUIContent("Tag"), new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		internal override void OnHeaderIconGUI(Rect iconRect)
		{
			Texture2D miniThumbnail = AssetPreview.GetMiniThumbnail(this.target);
			GUI.Label(iconRect, miniThumbnail);
		}

		private List<string> CollectParameters(AnimatorController controller, AnimatorControllerParameterType type)
		{
			List<string> list = new List<string>();
			if (controller != null)
			{
				AnimatorControllerParameter[] parameters = controller.parameters;
				for (int i = 0; i < parameters.Length; i++)
				{
					AnimatorControllerParameter animatorControllerParameter = parameters[i];
					if (animatorControllerParameter.type == type)
					{
						list.Add(animatorControllerParameter.name);
					}
				}
			}
			return list;
		}

		private void OnParametrizedValueGUI(string name, SerializedProperty value, SerializedProperty valueParameter, SerializedProperty valueParameterActive, AnimatorControllerParameterType parameterType)
		{
			EditorGUILayout.PropertyField(value, new GUILayoutOption[0]);
			if (this.controllerContext != null)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				List<string> list = this.CollectParameters(this.controllerContext, parameterType);
				if (list.Count == 0 && valueParameterActive.boolValue)
				{
					EditorGUILayout.HelpBox(string.Format("Must have at least one Parameter of type {0} in the AnimatorController", parameterType.ToString()), MessageType.Error);
				}
				else
				{
					if (valueParameterActive.boolValue && valueParameter.stringValue == string.Empty && list.Count > 0)
					{
						valueParameter.stringValue = list[0];
					}
					using (new EditorGUI.DisabledScope(!valueParameterActive.boolValue))
					{
						EditorGUI.BeginChangeCheck();
						string stringValue = EditorGUILayout.TextFieldDropDown(new GUIContent("Multiplier", "Parameter used as multiplier for speed."), valueParameter.stringValue, list.ToArray());
						if (EditorGUI.EndChangeCheck())
						{
							valueParameter.stringValue = stringValue;
						}
					}
				}
				EditorGUI.indentLevel--;
				valueParameterActive.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Parameter", "Use an AnimatorController's parameter to modulate this property at runtime."), valueParameterActive.boolValue, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(100f)
				});
				EditorGUILayout.EndHorizontal();
			}
		}

		private void OnParametrizedValueGUIOverride(string name, SerializedProperty value, SerializedProperty valueParameter, SerializedProperty valueParameterActive, AnimatorControllerParameterType parameterType)
		{
			if (this.controllerContext != null)
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				if (valueParameterActive.boolValue)
				{
					List<string> list = this.CollectParameters(this.controllerContext, parameterType);
					if (list.Count == 0 && valueParameterActive.boolValue)
					{
						EditorGUILayout.HelpBox(string.Format("Must have at least one Parameter of type {0} in the AnimatorController", parameterType.ToString()), MessageType.Error);
					}
					else
					{
						if (valueParameterActive.boolValue && valueParameter.stringValue == string.Empty && list.Count > 0)
						{
							valueParameter.stringValue = list[0];
						}
						EditorGUI.BeginChangeCheck();
						string stringValue = EditorGUILayout.TextFieldDropDown(new GUIContent(name), valueParameter.stringValue, list.ToArray());
						if (EditorGUI.EndChangeCheck())
						{
							valueParameter.stringValue = stringValue;
						}
					}
				}
				else
				{
					EditorGUILayout.PropertyField(value, new GUILayoutOption[0]);
				}
				valueParameterActive.boolValue = EditorGUILayout.ToggleLeft(new GUIContent("Parameter", "Override this constant value with an AnimatorController's parameter to animate this property at runtime."), valueParameterActive.boolValue, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(100f)
				});
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.PropertyField(value, new GUILayoutOption[0]);
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			if (this.controllerContext && !base.serializedObject.isEditingMultipleObjects)
			{
				Motion motion = this.controllerContext.GetStateEffectiveMotion(this.state, this.m_LayerIndexContext);
				EditorGUI.BeginChangeCheck();
				motion = (EditorGUILayout.ObjectField("Motion", motion, typeof(Motion), false, new GUILayoutOption[0]) as Motion);
				if (EditorGUI.EndChangeCheck())
				{
					this.controllerContext.SetStateEffectiveMotion(this.state, motion, this.m_LayerIndexContext);
				}
			}
			this.OnParametrizedValueGUI("Speed", this.m_Speed, this.m_SpeedParameter, this.m_SpeedParameterActive, AnimatorControllerParameterType.Float);
			this.OnParametrizedValueGUIOverride("Mirror", this.m_Mirror, this.m_MirrorParameter, this.m_MirrorParameterActive, AnimatorControllerParameterType.Bool);
			this.OnParametrizedValueGUIOverride("Cycle Offset", this.m_CycleOffset, this.m_CycleOffsetParameter, this.m_CycleOffsetParameterActive, AnimatorControllerParameterType.Float);
			EditorGUILayout.PropertyField(this.m_FootIK, GUIContent.Temp("Foot IK"), new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_WriteDefaultValues, GUIContent.Temp("Write Defaults"), new GUILayoutOption[0]);
			if (!base.serializedObject.isEditingMultipleObjects)
			{
				this.m_TransitionsEditor.OnInspectorGUI();
			}
			if (!base.serializedObject.isEditingMultipleObjects)
			{
				this.m_BehavioursEditor.OnInspectorGUI();
			}
			base.serializedObject.ApplyModifiedProperties();
		}

		public override bool HasPreviewGUI()
		{
			return this.m_TransitionsEditor != null && this.m_TransitionsEditor.HasPreviewGUI();
		}

		public override void OnPreviewSettings()
		{
			if (this.m_TransitionsEditor != null)
			{
				this.m_TransitionsEditor.OnPreviewSettings();
			}
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_TransitionsEditor != null)
			{
				this.m_TransitionsEditor.OnInteractivePreviewGUI(r, background);
			}
		}
	}
}
