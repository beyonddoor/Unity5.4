using System;
using UnityEngine;

namespace UnityEditor.Graphs
{
	internal interface IAnimatorControllerSubEditor
	{
		RenameOverlay renameOverlay
		{
			get;
		}

		void Init(IAnimatorControllerEditor host);

		void OnEnable();

		void OnDisable();

		void OnDestroy();

		void OnFocus();

		void OnLostFocus();

		void OnGUI(Rect rect);

		void OnToolbarGUI();

		void OnEvent();

		void ReleaseKeyboardFocus();

		bool HasKeyboardControl();

		void GrabKeyboardFocus();

		void ResetUI();
	}
}
