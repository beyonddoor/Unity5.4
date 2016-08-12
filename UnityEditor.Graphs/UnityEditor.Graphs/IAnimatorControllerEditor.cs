using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs
{
	internal interface IAnimatorControllerEditor
	{
		Animator previewAnimator
		{
			get;
		}

		AnimatorController animatorController
		{
			get;
			set;
		}

		bool liveLink
		{
			get;
		}

		void Repaint();

		void ResetUI();
	}
}
