using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	[CustomEditor(typeof(EntryNode))]
	internal class EntryNodeInspector : Editor
	{
		private SerializedProperty m_DefaultState;

		private SourceNodeTransitionEditor m_TransitionsEditor;

		private void Init()
		{
			if (this.m_TransitionsEditor == null)
			{
				this.m_TransitionsEditor = new SourceNodeTransitionEditor((this.target as EntryNode).undoableObject as AnimatorStateMachine, TransitionType.eEntry, this);
			}
		}

		public void OnEnable()
		{
			this.Init();
			this.m_TransitionsEditor.OnEnable();
		}

		public override void OnInspectorGUI()
		{
			this.Init();
			GUI.enabled = true;
			EntryNode entryNode = this.target as EntryNode;
			if (entryNode.stateMachine != null)
			{
				AnimatorState defaultState = entryNode.stateMachine.defaultState;
				EditorGUILayout.LabelField("Default state ", (!(defaultState != null)) ? "Not set" : defaultState.name, EditorStyles.boldLabel, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				this.m_TransitionsEditor.OnInspectorGUI();
			}
		}

		public void OnDisable()
		{
			this.m_TransitionsEditor.OnDisable();
		}

		public void OnDestroy()
		{
			this.m_TransitionsEditor.OnDestroy();
		}
	}
}
