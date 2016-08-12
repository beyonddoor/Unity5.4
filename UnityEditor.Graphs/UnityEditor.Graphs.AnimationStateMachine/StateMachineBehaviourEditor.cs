using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	[CustomEditor(typeof(StateMachineBehaviour), true)]
	internal class StateMachineBehaviourEditor : Editor
	{
		internal static bool ValidateMenucommand(MenuCommand command)
		{
			if (!AnimatorController.CanAddStateMachineBehaviours())
			{
				return false;
			}
			StateMachineBehaviour behaviour = command.context as StateMachineBehaviour;
			StateMachineBehaviourContext[] array = AnimatorController.FindStateMachineBehaviourContext(behaviour);
			for (int i = 0; i < array.Length; i++)
			{
				AnimatorController animatorController = array[i].animatorController;
				AnimatorState animatorState = array[i].animatorObject as AnimatorState;
				AnimatorStateMachine animatorStateMachine = array[i].animatorObject as AnimatorStateMachine;
				StateMachineBehaviour[] array2 = null;
				if (animatorState != null)
				{
					array2 = ((!(animatorController != null)) ? animatorState.behaviours : animatorController.GetStateEffectiveBehaviours(animatorState, array[i].layerIndex));
				}
				else if (animatorStateMachine != null)
				{
					array2 = animatorStateMachine.behaviours;
				}
				if (array2 != null)
				{
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j] == null)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		[MenuItem("CONTEXT/StateMachineBehaviour/Remove", true)]
		internal static bool ValidateRemove(MenuCommand command)
		{
			return StateMachineBehaviourEditor.ValidateMenucommand(command);
		}

		[MenuItem("CONTEXT/StateMachineBehaviour/Remove")]
		internal static void Remove(MenuCommand command)
		{
			StateMachineBehaviour stateMachineBehaviour = command.context as StateMachineBehaviour;
			StateMachineBehaviourContext[] array = AnimatorController.FindStateMachineBehaviourContext(stateMachineBehaviour);
			for (int i = 0; i < array.Length; i++)
			{
				AnimatorController animatorController = array[i].animatorController;
				AnimatorState animatorState = array[i].animatorObject as AnimatorState;
				AnimatorStateMachine animatorStateMachine = array[i].animatorObject as AnimatorStateMachine;
				if (animatorState != null)
				{
					StateMachineBehaviour[] behaviours = (!(animatorController != null)) ? animatorState.behaviours : animatorController.GetStateEffectiveBehaviours(animatorState, array[i].layerIndex);
					ArrayUtility.Remove<StateMachineBehaviour>(ref behaviours, stateMachineBehaviour);
					if (animatorController != null)
					{
						animatorController.SetStateEffectiveBehaviours(animatorState, array[i].layerIndex, behaviours);
					}
					else
					{
						animatorState.behaviours = behaviours;
					}
				}
				else if (animatorStateMachine != null)
				{
					StateMachineBehaviour[] behaviours2 = animatorStateMachine.behaviours;
					ArrayUtility.Remove<StateMachineBehaviour>(ref behaviours2, stateMachineBehaviour);
					animatorStateMachine.behaviours = behaviours2;
				}
			}
			UnityEngine.Object.DestroyImmediate(stateMachineBehaviour, true);
		}

		[MenuItem("CONTEXT/StateMachineBehaviour/Move Up", true)]
		internal static bool ValidateMoveUp(MenuCommand command)
		{
			return StateMachineBehaviourEditor.ValidateMenucommand(command);
		}

		[MenuItem("CONTEXT/StateMachineBehaviour/Move Up")]
		internal static void MoveUp(MenuCommand command)
		{
			StateMachineBehaviour stateMachineBehaviour = command.context as StateMachineBehaviour;
			StateMachineBehaviourContext[] array = AnimatorController.FindStateMachineBehaviourContext(stateMachineBehaviour);
			for (int i = 0; i < array.Length; i++)
			{
				AnimatorController animatorController = array[i].animatorController;
				AnimatorState animatorState = array[i].animatorObject as AnimatorState;
				AnimatorStateMachine animatorStateMachine = array[i].animatorObject as AnimatorStateMachine;
				if (animatorState != null)
				{
					StateMachineBehaviour[] array2 = (!(animatorController != null)) ? animatorState.behaviours : animatorController.GetStateEffectiveBehaviours(animatorState, array[i].layerIndex);
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j] == stateMachineBehaviour && j > 0)
						{
							array2[j] = array2[j - 1];
							array2[j - 1] = stateMachineBehaviour;
							break;
						}
					}
					if (animatorController != null)
					{
						animatorController.SetStateEffectiveBehaviours(animatorState, array[i].layerIndex, array2);
					}
					else
					{
						animatorState.behaviours = array2;
					}
				}
				else if (animatorStateMachine != null)
				{
					StateMachineBehaviour[] behaviours = animatorStateMachine.behaviours;
					for (int k = 0; k < behaviours.Length; k++)
					{
						if (behaviours[k] == stateMachineBehaviour && k > 0)
						{
							behaviours[k] = behaviours[k - 1];
							behaviours[k - 1] = stateMachineBehaviour;
							break;
						}
					}
					animatorStateMachine.behaviours = behaviours;
				}
			}
		}

		[MenuItem("CONTEXT/StateMachineBehaviour/Move Down", true)]
		internal static bool ValidateMoveDown(MenuCommand command)
		{
			return StateMachineBehaviourEditor.ValidateMenucommand(command);
		}

		[MenuItem("CONTEXT/StateMachineBehaviour/Move Down")]
		internal static void MoveDown(MenuCommand command)
		{
			StateMachineBehaviour stateMachineBehaviour = command.context as StateMachineBehaviour;
			StateMachineBehaviourContext[] array = AnimatorController.FindStateMachineBehaviourContext(stateMachineBehaviour);
			for (int i = 0; i < array.Length; i++)
			{
				AnimatorController animatorController = array[i].animatorController;
				AnimatorState animatorState = array[i].animatorObject as AnimatorState;
				AnimatorStateMachine animatorStateMachine = array[i].animatorObject as AnimatorStateMachine;
				if (animatorState != null)
				{
					StateMachineBehaviour[] array2 = (!(animatorController != null)) ? animatorState.behaviours : animatorController.GetStateEffectiveBehaviours(animatorState, array[i].layerIndex);
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j] == stateMachineBehaviour && j < array2.Length - 1)
						{
							array2[j] = array2[j + 1];
							array2[j + 1] = stateMachineBehaviour;
							break;
						}
					}
					if (animatorController != null)
					{
						animatorController.SetStateEffectiveBehaviours(animatorState, array[i].layerIndex, array2);
					}
					else
					{
						animatorState.behaviours = array2;
					}
				}
				else if (animatorStateMachine != null)
				{
					StateMachineBehaviour[] behaviours = animatorStateMachine.behaviours;
					for (int k = 0; k < behaviours.Length; k++)
					{
						if (behaviours[k] == stateMachineBehaviour && k < behaviours.Length - 1)
						{
							behaviours[k] = behaviours[k + 1];
							behaviours[k + 1] = stateMachineBehaviour;
							break;
						}
					}
					animatorStateMachine.behaviours = behaviours;
				}
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			SerializedProperty iterator = base.serializedObject.GetIterator();
			bool enterChildren = true;
			while (iterator.NextVisible(enterChildren))
			{
				using (new EditorGUI.DisabledScope(iterator.name == "m_Script"))
				{
					EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
					enterChildren = false;
				}
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
