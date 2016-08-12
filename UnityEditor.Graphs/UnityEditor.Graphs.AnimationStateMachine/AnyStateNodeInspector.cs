using System;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	[CustomEditor(typeof(AnyStateNode))]
	internal class AnyStateNodeInspector : Editor
	{
		private SourceNodeTransitionEditor m_TransitionsEditor;

		private void Init()
		{
			if (this.m_TransitionsEditor == null)
			{
				this.m_TransitionsEditor = new SourceNodeTransitionEditor(TransitionType.eAnyState, this);
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
			this.m_TransitionsEditor.OnInspectorGUI();
		}

		public void OnDisable()
		{
			this.m_TransitionsEditor.OnDisable();
		}

		public void OnDestroy()
		{
			this.m_TransitionsEditor.OnDestroy();
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
