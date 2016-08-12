using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class InvalidStateMachineBehaviour : ScriptableObject
	{
		public MonoScript monoScript;

		[HideInInspector]
		public AnimatorController controller;

		[HideInInspector]
		public AnimatorState state;

		[HideInInspector]
		public AnimatorStateMachine stateMachine;

		[HideInInspector]
		public int layerIndex;

		[HideInInspector]
		public int behaviourIndex;
	}
}
