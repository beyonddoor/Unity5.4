using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationBlendTree
{
	public class Node : UnityEditor.Graphs.Node
	{
		private Node m_Parent;

		[NonSerialized]
		private Animator m_Animator;

		private GameObject m_PreviewInstance;

		[NonSerialized]
		private AnimatorController m_Controller;

		[NonSerialized]
		private AnimatorStateMachine m_StateMachine;

		[NonSerialized]
		private AnimatorState m_State;

		private bool m_ControllerIsDirty;

		public List<Node> children = new List<Node>();

		public Motion motion;

		public float weight = 1f;

		public bool controllerDirty
		{
			get
			{
				return this.m_ControllerIsDirty;
			}
		}

		public BlendTree blendTree
		{
			get
			{
				return this.motion as BlendTree;
			}
		}

		public Animator animator
		{
			get
			{
				if (this.m_Animator == null && this.blendTree != null)
				{
					GameObject original = (GameObject)EditorGUIUtility.Load("Avatar/DefaultAvatar.fbx");
					if (this.m_PreviewInstance == null)
					{
						this.m_PreviewInstance = EditorUtility.InstantiateForAnimatorPreview(original);
					}
					this.m_Animator = this.m_PreviewInstance.GetComponent<Animator>();
					Renderer[] componentsInChildren = this.m_PreviewInstance.GetComponentsInChildren<Renderer>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Renderer renderer = componentsInChildren[i];
						renderer.enabled = false;
					}
					this.CreateStateMachine();
				}
				return this.m_Animator;
			}
		}

		public Node parent
		{
			get
			{
				return this.m_Parent;
			}
			set
			{
				if (this.m_Parent != null)
				{
					this.m_Parent.children.Remove(this);
				}
				this.m_Parent = value;
				this.m_Parent.children.Add(this);
			}
		}

		public int childIndex
		{
			get
			{
				if (this.m_Parent == null)
				{
					return -1;
				}
				return this.m_Parent.children.IndexOf(this);
			}
		}

		public bool isLeaf
		{
			get
			{
				return this.children.Count == 0;
			}
		}

		public Color weightColor
		{
			get
			{
				return Color.Lerp(new Color(0.8f, 0.8f, 0.8f, 1f), Color.white, Mathf.Pow(this.weight, 0.5f));
			}
		}

		public Color weightEdgeColor
		{
			get
			{
				return Color.Lerp(Color.white, new Color(0.42f, 0.7f, 1f, 1f), Mathf.Pow(this.weight, 0.5f));
			}
		}

		private void ClearStateMachine()
		{
			if (this.m_Animator != null)
			{
				AnimatorController.SetAnimatorController(this.m_Animator, null);
			}
			UnityEngine.Object.DestroyImmediate(this.m_Controller);
			UnityEngine.Object.DestroyImmediate(this.m_State);
			this.m_StateMachine = null;
			this.m_Controller = null;
			this.m_State = null;
		}

		public void CreateParameters()
		{
			for (int i = 0; i < this.blendTree.recursiveBlendParameterCount; i++)
			{
				this.m_Controller.AddParameter(this.blendTree.GetRecursiveBlendParameter(i), AnimatorControllerParameterType.Float);
			}
		}

		private void CreateStateMachine()
		{
			if (this.m_Animator != null && this.m_Controller == null)
			{
				this.m_Controller = new AnimatorController();
				this.m_Controller.pushUndo = false;
				this.m_Controller.AddLayer("node");
				this.m_StateMachine = this.m_Controller.layers[0].stateMachine;
				this.m_StateMachine.pushUndo = false;
				this.CreateParameters();
				this.m_State = this.m_StateMachine.AddState("node", default(Vector3));
				this.m_State.pushUndo = false;
				this.m_State.motion = this.motion;
				this.m_State.hideFlags = HideFlags.DontSave;
				this.m_Controller.hideFlags = HideFlags.DontSave;
				this.m_StateMachine.hideFlags = HideFlags.DontSave;
				AnimatorController.SetAnimatorController(this.m_Animator, this.m_Controller);
				this.m_Animator.Update(0f);
				AnimatorController expr_FD = this.m_Controller;
				expr_FD.OnAnimatorControllerDirty = (Action)Delegate.Combine(expr_FD.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
				this.m_ControllerIsDirty = false;
			}
		}

		private void OnDestroy()
		{
			if (this.m_Controller != null)
			{
				AnimatorController expr_17 = this.m_Controller;
				expr_17.OnAnimatorControllerDirty = (Action)Delegate.Remove(expr_17.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
			}
			UnityEngine.Object.DestroyImmediate(this.m_PreviewInstance);
		}

		protected virtual void ControllerDirty()
		{
			this.m_ControllerIsDirty = true;
		}

		public void UpdateAnimator()
		{
			if (this.animator)
			{
				if (this.m_ControllerIsDirty)
				{
					this.ClearStateMachine();
					this.CreateStateMachine();
				}
				int recursiveBlendParameterCount = this.blendTree.recursiveBlendParameterCount;
				if (this.m_Controller.parameters.Length < recursiveBlendParameterCount)
				{
					return;
				}
				for (int i = 0; i < recursiveBlendParameterCount; i++)
				{
					string recursiveBlendParameter = this.blendTree.GetRecursiveBlendParameter(i);
					float inputBlendValue = this.blendTree.GetInputBlendValue(recursiveBlendParameter);
					this.animator.SetFloat(recursiveBlendParameter, inputBlendValue);
				}
				this.animator.EvaluateController();
			}
		}
	}
}
