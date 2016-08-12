using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationBlendTree
{
	internal class Graph : UnityEditor.Graphs.Graph
	{
		internal const float kNodeWidth = 200f;

		private const float kNodeHeight = 50f;

		private const float kNodeHorizontalPadding = 70f;

		private const float kNodeVerticalPadding = 5f;

		private const float kSectionVerticalPadding = 15f;

		private const float kLeafHorizontalOffset = 20f;

		private const float kParameterVerticalOffset = 20f;

		private Dictionary<string, float> m_ParameterValues = new Dictionary<string, float>();

		[SerializeField]
		private BlendTree m_RootBlendTree;

		private Node m_RootNode;

		[SerializeField]
		public Animator previewAvatar;

		private float m_VerticalLeafOffset;

		public BlendTree rootBlendTree
		{
			get
			{
				return this.m_RootBlendTree;
			}
			set
			{
				if (this.m_RootBlendTree != value)
				{
					this.m_RootBlendTree = value;
					this.BuildFromBlendTree(this.m_RootBlendTree);
				}
			}
		}

		public Node rootNode
		{
			get
			{
				return this.m_RootNode;
			}
			private set
			{
				this.m_RootNode = value;
			}
		}

		public bool liveLink
		{
			get
			{
				return EditorApplication.isPlaying && this.previewAvatar != null && this.previewAvatar.enabled && this.previewAvatar.gameObject.activeInHierarchy;
			}
		}

		public void BuildFromBlendTree(BlendTree blendTree)
		{
			this.Clear(true);
			if (blendTree == null)
			{
				return;
			}
			this.CreateNodeFromBlendTreeRecursive(blendTree, null);
			this.PopulateParameterValues();
			this.AutoArrangeNodePositions();
		}

		public override void Clear(bool destroyNodes)
		{
			base.Clear(destroyNodes);
			this.m_ParameterValues.Clear();
			this.m_RootBlendTree = null;
			this.m_RootNode = null;
		}

		public override void WakeUp(bool force)
		{
			base.WakeUp(force);
			this.PopulateParameterValues();
		}

		public void AutoArrangeNodePositions()
		{
			this.m_VerticalLeafOffset = 5f;
			this.ArrangeNodeRecursive(this.m_RootNode, 0);
		}

		internal override UnityEditor.Graphs.GraphGUI GetEditor()
		{
			GraphGUI graphGUI = ScriptableObject.CreateInstance<GraphGUI>();
			graphGUI.graph = this;
			graphGUI.hideFlags = HideFlags.HideAndDontSave;
			return graphGUI;
		}

		private T GetObjectFromSelection<T>() where T : UnityEngine.Object
		{
			return Selection.activeObject as T;
		}

		private T GetComponentFromSelection<T>() where T : Component
		{
			GameObject activeGameObject = Selection.activeGameObject;
			if (activeGameObject)
			{
				return activeGameObject.GetComponent<T>();
			}
			return (T)((object)null);
		}

		private void ArrangeNodeRecursive(Node node, int depth)
		{
			if (node.isLeaf && node.parent != null)
			{
				Rect position = node.position;
				position.y = this.m_VerticalLeafOffset;
				position.x = (float)depth * 270f + 70f;
				position.x += Mathf.PingPong(((float)node.childIndex + 0.5f) / (float)node.parent.children.Count, 0.5f) * 20f;
				node.position = position;
				this.m_VerticalLeafOffset += position.height + 5f;
			}
			else if (node.children.Count != 0)
			{
				float verticalLeafOffset = this.m_VerticalLeafOffset;
				float num = float.PositiveInfinity;
				float num2 = float.NegativeInfinity;
				foreach (Node current in node.children)
				{
					this.ArrangeNodeRecursive(current, depth + 1);
					num = Mathf.Min(num, current.position.y);
					num2 = Mathf.Max(num2, current.position.y);
				}
				Rect position2 = node.position;
				position2.y = (num + num2) * 0.5f;
				position2.x = (float)depth * 270f + 70f;
				node.position = position2;
				this.m_VerticalLeafOffset = Mathf.Max(15f + this.m_VerticalLeafOffset, verticalLeafOffset + position2.height + 15f);
			}
		}

		public void SetParameterValue(string parameterName, float parameterValue)
		{
			this.m_ParameterValues[parameterName] = parameterValue;
			this.SetParameterValueRecursive(this.m_RootBlendTree, parameterName, parameterValue);
			if (this.liveLink)
			{
				this.previewAvatar.SetFloat(parameterName, parameterValue);
			}
		}

		private void SetParameterValueRecursive(BlendTree blendTree, string parameterName, float parameterValue)
		{
			blendTree.SetInputBlendValue(parameterName, parameterValue);
			int childCount = blendTree.GetChildCount();
			for (int i = 0; i < childCount; i++)
			{
				BlendTree blendTree2 = blendTree.GetChildMotion(i) as BlendTree;
				if (!(blendTree2 == null))
				{
					this.SetParameterValueRecursive(blendTree2, parameterName, parameterValue);
				}
			}
		}

		public float GetParameterValue(string parameterName)
		{
			if (this.m_ParameterValues.ContainsKey(parameterName))
			{
				return this.m_ParameterValues[parameterName];
			}
			Debug.LogError("parameter name does not exist.");
			return 0f;
		}

		public void PopulateParameterValues()
		{
			if (this.m_RootBlendTree == null)
			{
				return;
			}
			for (int i = 0; i < this.m_RootBlendTree.recursiveBlendParameterCount; i++)
			{
				string recursiveBlendParameter = this.m_RootBlendTree.GetRecursiveBlendParameter(i);
				if (this.liveLink)
				{
					this.SetParameterValue(recursiveBlendParameter, this.previewAvatar.GetFloat(recursiveBlendParameter));
				}
				else
				{
					this.SetParameterValue(recursiveBlendParameter, this.m_RootBlendTree.GetInputBlendValue(recursiveBlendParameter));
				}
			}
		}

		private void CreateNodeFromBlendTreeRecursive(BlendTree blendTree, Node parentNode)
		{
			Node node = this.CreateNode(blendTree, blendTree.name);
			if (parentNode)
			{
				node.parent = parentNode;
				Slot fromSlot = parentNode.AddOutputSlot(blendTree.name);
				Slot toSlot = node.AddInputSlot((parentNode.motion as BlendTree).name);
				this.Connect(fromSlot, toSlot);
			}
			else
			{
				this.m_RootBlendTree = blendTree;
				this.m_RootNode = node;
			}
			int childCount = blendTree.GetChildCount();
			for (int i = 0; i < childCount; i++)
			{
				Motion childMotion = blendTree.GetChildMotion(i);
				if (childMotion == null)
				{
					this.CreateEmptySlot(node);
				}
				else if (childMotion is BlendTree)
				{
					this.CreateNodeFromBlendTreeRecursive(childMotion as BlendTree, node);
				}
				else
				{
					if (!(childMotion is AnimationClip))
					{
						throw new NotImplementedException("Unknown Motion type:" + childMotion.GetType());
					}
					this.CreateNodeFromAnimationClip(childMotion as AnimationClip, node);
				}
			}
		}

		private void CreateEmptySlot(Node parentNode)
		{
			parentNode.AddOutputSlot(string.Empty);
		}

		private void CreateNodeFromAnimationClip(AnimationClip clip, Node parentNode)
		{
			Node node = this.CreateNode(clip, clip.name);
			node.parent = parentNode;
			Slot fromSlot = parentNode.AddOutputSlot(clip.name);
			Slot toSlot = node.AddInputSlot((parentNode.motion as BlendTree).name);
			this.Connect(fromSlot, toSlot);
		}

		private Node CreateNode(Motion motion, string name)
		{
			Node node = UnityEditor.Graphs.Node.Instance<Node>();
			node.hideFlags = HideFlags.HideAndDontSave;
			node.name = name;
			node.motion = motion;
			BlendTree blendTree = motion as BlendTree;
			float num = 0f;
			if (blendTree != null)
			{
				num = (float)blendTree.recursiveBlendParameterCount * 20f;
			}
			node.position = new Rect(0f, 0f, 200f, 50f + num);
			this.AddNode(node);
			return node;
		}

		public void RemoveNodeMotions(IEnumerable<UnityEditor.Graphs.Node> nodes)
		{
			foreach (UnityEditor.Graphs.Node current in nodes)
			{
				Node node = current as Node;
				if (!(this.m_RootBlendTree == node.motion))
				{
					if (!(node.motion == null))
					{
						if (node.parent)
						{
							BlendTree blendTree = node.parent.motion as BlendTree;
							int index = Graph.FindMotionIndexOnBlendTree(blendTree, node.motion);
							blendTree.RemoveChild(index);
						}
						BlendTree blendTree2 = node.motion as BlendTree;
						if (blendTree2 && MecanimUtilities.AreSameAsset(this.m_RootBlendTree, blendTree2))
						{
							MecanimUtilities.DestroyBlendTreeRecursive(blendTree2);
						}
					}
				}
			}
		}

		private static int FindMotionIndexOnBlendTree(BlendTree blendTree, Motion motion)
		{
			int childCount = blendTree.GetChildCount();
			for (int i = 0; i < childCount; i++)
			{
				Motion childMotion = blendTree.GetChildMotion(i);
				if (childMotion == motion)
				{
					return i;
				}
			}
			return -1;
		}

		public Node FindNode(Motion motion)
		{
			return this.nodes.Cast<Node>().FirstOrDefault((Node node) => node.motion == motion);
		}
	}
}
