using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	[CustomEditor(typeof(InvalidStateMachineBehaviour), false)]
	internal class InvalidStateMachineBehaviourEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			InvalidStateMachineBehaviour invalidStateMachineBehaviour = this.target as InvalidStateMachineBehaviour;
			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUILayout.ObjectField("Script", invalidStateMachineBehaviour.monoScript, typeof(MonoScript), false, new GUILayoutOption[0]);
			}
			if (invalidStateMachineBehaviour.monoScript != null && !invalidStateMachineBehaviour.monoScript.GetScriptTypeWasJustCreatedFromComponentMenu())
			{
				GUIContent gUIContent = EditorGUIUtility.TextContent("The associated script can not be loaded.\nPlease fix any compile errors\nand assign a valid script.");
				EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, true);
			}
		}

		[MenuItem("CONTEXT/InvalidStateMachineBehaviour/Remove")]
		private static void Remove(MenuCommand command)
		{
			InvalidStateMachineBehaviour invalidStateMachineBehaviour = command.context as InvalidStateMachineBehaviour;
			AnimatorController controller = invalidStateMachineBehaviour.controller;
			AnimatorState state = invalidStateMachineBehaviour.state;
			AnimatorStateMachine stateMachine = invalidStateMachineBehaviour.stateMachine;
			if (state != null)
			{
				if (controller != null)
				{
					controller.RemoveStateEffectiveBehaviour(state, invalidStateMachineBehaviour.layerIndex, invalidStateMachineBehaviour.behaviourIndex);
				}
				else
				{
					state.RemoveBehaviour(invalidStateMachineBehaviour.behaviourIndex);
				}
			}
			else if (stateMachine != null)
			{
				stateMachine.RemoveBehaviour(invalidStateMachineBehaviour.behaviourIndex);
			}
			UnityEngine.Object.DestroyImmediate(invalidStateMachineBehaviour, true);
		}
	}
}
