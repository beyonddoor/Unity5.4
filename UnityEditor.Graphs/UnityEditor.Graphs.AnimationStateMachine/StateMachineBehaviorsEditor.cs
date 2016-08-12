using System;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class StateMachineBehaviorsEditor
	{
		internal class Styles
		{
			public readonly GUIContent addBehaviourLabel = new GUIContent("Add Behaviour");

			public GUIStyle addBehaviourButtonStyle = "LargeButton";
		}

		private static StateMachineBehaviorsEditor.Styles s_Styles;

		private UnityEngine.Object m_Target;

		private UnityEditor.Animations.AnimatorController m_ControllerContext;

		private int m_LayerIndexContext;

		private Editor[] m_BehavioursEditor;

		internal static StateMachineBehaviorsEditor.Styles styles
		{
			get
			{
				StateMachineBehaviorsEditor.Styles arg_17_0;
				if ((arg_17_0 = StateMachineBehaviorsEditor.s_Styles) == null)
				{
					arg_17_0 = (StateMachineBehaviorsEditor.s_Styles = new StateMachineBehaviorsEditor.Styles());
				}
				return arg_17_0;
			}
		}

		public AnimatorState state
		{
			get
			{
				return this.m_Target as AnimatorState;
			}
		}

		public AnimatorStateMachine stateMachine
		{
			get
			{
				return this.m_Target as AnimatorStateMachine;
			}
		}

		protected StateMachineBehaviour[] effectiveBehaviours
		{
			get
			{
				if (this.state)
				{
					return (!this.m_ControllerContext) ? this.state.behaviours : this.m_ControllerContext.GetStateEffectiveBehaviours(this.state, this.m_LayerIndexContext);
				}
				if (this.stateMachine)
				{
					return this.stateMachine.behaviours;
				}
				return null;
			}
		}

		public StateMachineBehaviorsEditor(AnimatorState state, Editor host)
		{
			this.m_Target = state;
		}

		public StateMachineBehaviorsEditor(AnimatorStateMachine stateMachine, Editor host)
		{
			this.m_Target = stateMachine;
		}

		public void OnEnable()
		{
			this.m_ControllerContext = ((!(AnimatorControllerTool.tool != null)) ? null : AnimatorControllerTool.tool.animatorController);
			this.m_LayerIndexContext = ((!(AnimatorControllerTool.tool != null)) ? -1 : AnimatorControllerTool.tool.selectedLayerIndex);
		}

		public void OnDisable()
		{
		}

		public void OnDestroy()
		{
		}

		public void OnInspectorGUI()
		{
			StateMachineBehaviour[] effectiveBehaviours = this.effectiveBehaviours;
			if (!this.IsEditorsValid(effectiveBehaviours))
			{
				this.BuildEditorList(effectiveBehaviours);
			}
			EditorGUI.BeginChangeCheck();
			Editor[] behavioursEditor = this.m_BehavioursEditor;
			for (int i = 0; i < behavioursEditor.Length; i++)
			{
				Editor editor = behavioursEditor[i];
				EditorGUI.BeginChangeCheck();
				bool flag = true;
				bool flag2 = !(editor.target is InvalidStateMachineBehaviour);
				if (flag2)
				{
					flag = InternalEditorUtility.GetIsInspectorExpanded(editor.target);
				}
				flag = EditorGUILayout.InspectorTitlebar(flag, editor.target, true);
				if (flag2 && EditorGUI.EndChangeCheck())
				{
					InternalEditorUtility.SetIsInspectorExpanded(editor.target, flag);
				}
				if (flag || !flag2)
				{
					editor.OnInspectorGUI();
				}
			}
			this.AddBehaviourButton();
		}

		protected bool IsEditorsValid(StateMachineBehaviour[] behaviours)
		{
			if (this.m_BehavioursEditor == null || (this.m_BehavioursEditor != null && this.m_BehavioursEditor.Length != behaviours.Length))
			{
				return false;
			}
			for (int i = 0; i < this.m_BehavioursEditor.Length; i++)
			{
				if (this.m_BehavioursEditor[i].target != behaviours[i])
				{
					return false;
				}
			}
			return true;
		}

		protected void BuildEditorList(StateMachineBehaviour[] behaviours)
		{
			this.m_BehavioursEditor = new Editor[behaviours.Length];
			for (int i = 0; i < behaviours.Length; i++)
			{
				if (behaviours[i] != null && behaviours[i] is StateMachineBehaviour)
				{
					this.m_BehavioursEditor[i] = Editor.CreateEditor(behaviours[i]);
				}
				else
				{
					InvalidStateMachineBehaviour invalidStateMachineBehaviour = ScriptableObject.CreateInstance<InvalidStateMachineBehaviour>();
					invalidStateMachineBehaviour.monoScript = this.GetBehaviourMonoScript(i);
					if (invalidStateMachineBehaviour.monoScript != null)
					{
						invalidStateMachineBehaviour.name = invalidStateMachineBehaviour.monoScript.name;
					}
					invalidStateMachineBehaviour.hideFlags = HideFlags.HideAndDontSave;
					invalidStateMachineBehaviour.controller = this.m_ControllerContext;
					invalidStateMachineBehaviour.state = this.state;
					invalidStateMachineBehaviour.stateMachine = this.stateMachine;
					invalidStateMachineBehaviour.layerIndex = this.m_LayerIndexContext;
					invalidStateMachineBehaviour.behaviourIndex = i;
					this.m_BehavioursEditor[i] = Editor.CreateEditor(invalidStateMachineBehaviour);
				}
			}
		}

		protected void AddBehaviourButton()
		{
			EditorGUILayout.Space();
			Rect rect = GUILayoutUtility.GetRect(StateMachineBehaviorsEditor.styles.addBehaviourLabel, StateMachineBehaviorsEditor.styles.addBehaviourButtonStyle, null);
			rect.x += (rect.width - 230f) / 2f;
			rect.width = 230f;
			bool flag = UnityEditor.Animations.AnimatorController.CanAddStateMachineBehaviours();
			bool flag2;
			using (new EditorGUI.DisabledScope(!flag))
			{
				flag2 = EditorGUI.ButtonMouseDown(rect, StateMachineBehaviorsEditor.styles.addBehaviourLabel, FocusType.Passive, StateMachineBehaviorsEditor.styles.addBehaviourButtonStyle);
			}
			if (flag2 && AddStateMachineBehaviourComponentWindow.Show(rect, this.m_ControllerContext, this.m_LayerIndexContext, new UnityEngine.Object[]
			{
				this.m_Target
			}))
			{
				GUIUtility.ExitGUI();
			}
			EditorGUILayout.Space();
			if (!flag)
			{
				EditorGUILayout.HelpBox("Please fix compile errors before creating new state machine behaviours", MessageType.Error, true);
			}
		}

		protected MonoScript GetBehaviourMonoScript(int index)
		{
			if (this.state)
			{
				return (!(this.m_ControllerContext != null)) ? this.state.GetBehaviourMonoScript(index) : this.m_ControllerContext.GetBehaviourMonoScript(this.state, this.m_LayerIndexContext, index);
			}
			if (this.stateMachine)
			{
				return this.stateMachine.GetBehaviourMonoScript(index);
			}
			return null;
		}
	}
}
