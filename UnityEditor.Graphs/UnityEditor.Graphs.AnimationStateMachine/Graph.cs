using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class Graph : UnityEditor.Graphs.Graph
	{
		[NonSerialized]
		public AnimatorStateMachine rootStateMachine;

		[NonSerialized]
		public AnimatorStateMachine parentStateMachine;

		[NonSerialized]
		private AnimatorStateMachine m_ActiveStateMachine;

		private readonly Dictionary<AnimatorState, StateNode> m_StateNodeLookup = new Dictionary<AnimatorState, StateNode>();

		private readonly Dictionary<AnimatorStateMachine, StateMachineNode> m_StateMachineNodeLookup = new Dictionary<AnimatorStateMachine, StateMachineNode>();

		private readonly Dictionary<AnimatorState, AnimatorStateMachine> m_StateMachineProxyLookup = new Dictionary<AnimatorState, AnimatorStateMachine>();

		private readonly Dictionary<AnimatorStateMachine, AnimatorStateMachine> m_StateMachineLookup = new Dictionary<AnimatorStateMachine, AnimatorStateMachine>();

		private readonly Dictionary<string, EdgeInfo> m_ConnectedSlotsCache = new Dictionary<string, EdgeInfo>();

		[NonSerialized]
		private AnyStateNode m_AnyStateNode;

		[NonSerialized]
		private EntryNode m_EntryNode;

		[NonSerialized]
		private ExitNode m_ExitNode;

		private int m_StateCount;

		private int m_StateMachineCount;

		private int m_TransitionCount;

		private AnimatorState m_DefaultState;

		public AnimatorStateMachine activeStateMachine
		{
			get
			{
				return this.m_ActiveStateMachine;
			}
		}

		internal override UnityEditor.Graphs.GraphGUI GetEditor()
		{
			GraphGUI graphGUI = ScriptableObject.CreateInstance<GraphGUI>();
			graphGUI.graph = this;
			graphGUI.hideFlags = HideFlags.HideAndDontSave;
			return graphGUI;
		}

		public void RebuildGraph()
		{
			if (this.activeStateMachine != null)
			{
				this.BuildGraphFromStateMachine(this.activeStateMachine);
			}
			else
			{
				this.Clear(false);
			}
		}

		public void BuildGraphFromStateMachine(AnimatorStateMachine stateMachine)
		{
			Assert.IsNotNull(stateMachine);
			Assert.IsNotNull(this.rootStateMachine);
			this.Clear(false);
			this.m_ActiveStateMachine = stateMachine;
			this.CreateStateMachineLookup();
			this.CreateNodes();
			this.CreateEdges();
			this.m_StateCount = this.m_ActiveStateMachine.states.Length;
			this.m_StateMachineCount = this.m_ActiveStateMachine.stateMachines.Length;
			this.m_TransitionCount = this.rootStateMachine.transitionCount;
			this.m_DefaultState = this.rootStateMachine.defaultState;
		}

		private void CreateEdges(Node srcNode, Node dstNode, TransitionEditionContext context)
		{
			if (srcNode == null || dstNode == null)
			{
				return;
			}
			string key = Graph.GenerateConnectionKey(srcNode, dstNode);
			if (this.m_ConnectedSlotsCache.ContainsKey(key))
			{
				this.m_ConnectedSlotsCache[key].Add(context);
				return;
			}
			if (srcNode == dstNode)
			{
				if (context.sourceState != null && !this.HasState(this.activeStateMachine, context.sourceState, false))
				{
					return;
				}
				if (context.sourceStateMachine != null && !this.HasStateMachine(this.activeStateMachine, context.sourceStateMachine, false))
				{
					return;
				}
			}
			Slot fromSlot = srcNode.outputSlots.First<Slot>();
			Slot toSlot = dstNode.inputSlots.First<Slot>();
			this.Connect(fromSlot, toSlot);
			this.m_ConnectedSlotsCache.Add(key, new EdgeInfo(context));
		}

		private void CreateSelectorEdges(AnimatorTransition transition, AnimatorStateMachine owner, AnimatorStateMachine sourceStateMachine)
		{
			Node node = this.FindNodeForEdges(sourceStateMachine);
			if (node is EntryNode)
			{
				return;
			}
			Node dstNode = null;
			if (transition.destinationStateMachine != null)
			{
				dstNode = this.FindNodeForEdges(transition.destinationStateMachine);
			}
			else if (transition.destinationState)
			{
				dstNode = this.FindNodeForEdges(transition.destinationState);
			}
			else if (transition.isExit)
			{
				dstNode = this.m_ExitNode;
			}
			StateMachineNode stateMachineNode = node as StateMachineNode;
			if (transition.isExit && node && stateMachineNode != null && stateMachineNode.stateMachine != sourceStateMachine)
			{
				return;
			}
			this.CreateEdges(node, dstNode, new TransitionEditionContext(transition, null, sourceStateMachine, owner));
		}

		public string GetStatePath(AnimatorState state)
		{
			string text = state.name;
			AnimatorStateMachine stateMachine;
			if (this.m_StateMachineProxyLookup.TryGetValue(state, out stateMachine))
			{
				text = this.GetStateMachinePath(stateMachine) + "." + text;
			}
			return text;
		}

		public string GetStateMachinePath(AnimatorStateMachine stateMachine)
		{
			string text = stateMachine.name;
			AnimatorStateMachine animatorStateMachine;
			while (this.m_StateMachineLookup.TryGetValue(stateMachine, out animatorStateMachine))
			{
				text = animatorStateMachine.name + "." + text;
				stateMachine = animatorStateMachine;
			}
			return text;
		}

		private void CreateStateEdges(AnimatorState sourceState, AnimatorStateTransition transition)
		{
			Node node = this.FindNodeForEdges(sourceState);
			Node node2 = this.m_ExitNode;
			if (transition.destinationStateMachine != null)
			{
				node2 = this.FindNodeForEdges(transition.destinationStateMachine);
			}
			else if (transition.destinationState)
			{
				node2 = this.FindNodeForEdges(transition.destinationState);
			}
			if (node2 is EntryNode)
			{
				return;
			}
			if (!transition.isExit || (transition.isExit && node is StateNode))
			{
				this.CreateEdges(node, node2, new TransitionEditionContext(transition, sourceState, null, null));
			}
		}

		private void CreateAnyStateEdges(AnimatorStateTransition transition)
		{
			bool flag = transition == null;
			bool flag2 = transition.destinationState && transition.destinationState.FindParent(this.rootStateMachine) != this.activeStateMachine;
			bool flag3 = transition.destinationStateMachine && this.rootStateMachine.FindParent(transition.destinationStateMachine) != this.activeStateMachine;
			bool flag4 = transition.destinationStateMachine && transition.destinationStateMachine != this.activeStateMachine;
			if (flag || flag2 || (flag3 && flag4))
			{
				return;
			}
			Node dstNode = (!transition.destinationState) ? this.FindNodeForEdges(transition.destinationStateMachine) : this.FindNodeForEdges(transition.destinationState);
			this.CreateEdges(this.m_AnyStateNode, dstNode, new TransitionEditionContext(transition, null, null, this.rootStateMachine));
		}

		private void CreateEntryEdges(AnimatorTransition transition, AnimatorStateMachine owner)
		{
			Node dstNode;
			if (transition.destinationStateMachine != null)
			{
				dstNode = this.FindNodeForEdges(transition.destinationStateMachine);
			}
			else
			{
				dstNode = this.FindNodeForEdges(transition.destinationState);
			}
			this.CreateEdges(this.m_EntryNode, dstNode, new TransitionEditionContext(transition, null, null, owner));
		}

		private void CreateDefaultStateEdge(AnimatorState defaultState, AnimatorStateMachine owner)
		{
			if (defaultState != null)
			{
				Node node = this.FindNodeForEdges(defaultState);
				if (node != null)
				{
					this.CreateEdges(this.m_EntryNode, node, new TransitionEditionContext(null, null, null, owner));
				}
			}
		}

		private void CreateEdges()
		{
			this.m_ConnectedSlotsCache.Clear();
			List<ChildAnimatorState> statesRecursive = this.rootStateMachine.statesRecursive;
			foreach (ChildAnimatorState current in statesRecursive)
			{
				AnimatorState state = current.state;
				AnimatorStateTransition[] transitions = state.transitions;
				AnimatorStateTransition[] array = transitions;
				for (int i = 0; i < array.Length; i++)
				{
					AnimatorStateTransition animatorStateTransition = array[i];
					if (animatorStateTransition != null)
					{
						this.CreateStateEdges(state, animatorStateTransition);
					}
				}
			}
			List<AnimatorStateTransition> anyStateTransitionsRecursive = this.rootStateMachine.anyStateTransitionsRecursive;
			foreach (AnimatorStateTransition current2 in anyStateTransitionsRecursive)
			{
				if (current2 != null)
				{
					this.CreateAnyStateEdges(current2);
				}
			}
			List<ChildAnimatorStateMachine> stateMachinesRecursive = this.rootStateMachine.stateMachinesRecursive;
			stateMachinesRecursive.Add(new ChildAnimatorStateMachine
			{
				stateMachine = this.rootStateMachine
			});
			foreach (ChildAnimatorStateMachine current3 in stateMachinesRecursive)
			{
				ChildAnimatorStateMachine[] stateMachines = current3.stateMachine.stateMachines;
				ChildAnimatorStateMachine[] array2 = stateMachines;
				for (int j = 0; j < array2.Length; j++)
				{
					ChildAnimatorStateMachine childAnimatorStateMachine = array2[j];
					AnimatorTransition[] stateMachineTransitions = current3.stateMachine.GetStateMachineTransitions(childAnimatorStateMachine.stateMachine);
					AnimatorTransition[] array3 = stateMachineTransitions;
					for (int k = 0; k < array3.Length; k++)
					{
						AnimatorTransition transition = array3[k];
						this.CreateSelectorEdges(transition, current3.stateMachine, childAnimatorStateMachine.stateMachine);
					}
				}
			}
			AnimatorTransition[] entryTransitions = this.activeStateMachine.entryTransitions;
			AnimatorTransition[] array4 = entryTransitions;
			for (int l = 0; l < array4.Length; l++)
			{
				AnimatorTransition transition2 = array4[l];
				this.CreateEntryEdges(transition2, this.activeStateMachine);
			}
			this.CreateDefaultStateEdge(this.activeStateMachine.defaultState, this.activeStateMachine);
		}

		private static string GenerateConnectionKey(UnityEditor.Graphs.Node srcNode, UnityEditor.Graphs.Node dstNode)
		{
			return srcNode.GetInstanceID() + "->" + dstNode.GetInstanceID();
		}

		public void SetStateMachines(AnimatorStateMachine stateMachine, AnimatorStateMachine parent, AnimatorStateMachine root)
		{
			this.rootStateMachine = root;
			this.parentStateMachine = parent;
			if (stateMachine != this.m_ActiveStateMachine)
			{
				this.BuildGraphFromStateMachine(stateMachine);
			}
		}

		public EdgeInfo GetEdgeInfo(Edge edge)
		{
			if (edge.toSlot == null)
			{
				return null;
			}
			return this.m_ConnectedSlotsCache[Graph.GenerateConnectionKey(edge.fromSlot.node, edge.toSlot.node)];
		}

		public Node FindNodeForEdges(AnimatorState state)
		{
			return this.FindNode(state);
		}

		public Node FindNode(AnimatorState state)
		{
			if (this.m_StateNodeLookup.ContainsKey(state))
			{
				return this.m_StateNodeLookup[state];
			}
			Node node = this.FindStateMachineNodeFromState(state, this.activeStateMachine);
			if (node != null)
			{
				return node;
			}
			if (this.parentStateMachine && this.HasState(this.rootStateMachine, state, true))
			{
				return this.m_StateMachineNodeLookup[this.parentStateMachine];
			}
			return null;
		}

		public Node FindNodeForEdges(AnimatorStateMachine stateMachine)
		{
			if (stateMachine == this.activeStateMachine)
			{
				return this.m_EntryNode;
			}
			return this.FindNode(stateMachine);
		}

		public Node FindNode(AnimatorStateMachine stateMachine)
		{
			if (stateMachine == null)
			{
				return null;
			}
			if (stateMachine == this.activeStateMachine)
			{
				return null;
			}
			if (this.m_StateMachineNodeLookup.ContainsKey(stateMachine))
			{
				return this.m_StateMachineNodeLookup[stateMachine];
			}
			ChildAnimatorStateMachine[] stateMachines = this.activeStateMachine.stateMachines;
			for (int i = 0; i < stateMachines.Length; i++)
			{
				ChildAnimatorStateMachine childAnimatorStateMachine = stateMachines[i];
				if (this.HasStateMachine(childAnimatorStateMachine.stateMachine, stateMachine, true))
				{
					return this.m_StateMachineNodeLookup[childAnimatorStateMachine.stateMachine];
				}
			}
			if (this.parentStateMachine)
			{
				return this.m_StateMachineNodeLookup[this.parentStateMachine];
			}
			return null;
		}

		private void CreateStateMachineLookup()
		{
			this.m_StateMachineProxyLookup.Clear();
			this.m_StateMachineLookup.Clear();
			this.FillStateMachineLookupFromStateMachine(this.rootStateMachine);
		}

		private Node FindStateMachineNodeFromState(AnimatorState state, AnimatorStateMachine stateMachine)
		{
			AnimatorStateMachine key = null;
			if (this.m_StateMachineProxyLookup.TryGetValue(state, out key))
			{
				AnimatorStateMachine animatorStateMachine = null;
				while (this.m_StateMachineLookup.TryGetValue(key, out animatorStateMachine))
				{
					if (animatorStateMachine == stateMachine)
					{
						return this.m_StateMachineNodeLookup[key];
					}
					if (animatorStateMachine == this.rootStateMachine)
					{
						return this.m_StateMachineNodeLookup[this.parentStateMachine];
					}
					key = animatorStateMachine;
				}
			}
			return null;
		}

		private bool HasState(AnimatorStateMachine stateMachine, AnimatorState state, bool recursive)
		{
			AnimatorStateMachine animatorStateMachine = null;
			bool result = false;
			if (this.m_StateMachineProxyLookup.TryGetValue(state, out animatorStateMachine))
			{
				if (stateMachine == animatorStateMachine)
				{
					result = true;
				}
				else if (recursive)
				{
					result = this.HasStateMachine(stateMachine, animatorStateMachine, recursive);
				}
			}
			return result;
		}

		private bool HasStateMachine(AnimatorStateMachine stateMachineParent, AnimatorStateMachine stateMachineChild, bool recursive)
		{
			AnimatorStateMachine animatorStateMachine = null;
			bool result = false;
			while (this.m_StateMachineLookup.TryGetValue(stateMachineChild, out animatorStateMachine))
			{
				if (animatorStateMachine == stateMachineParent)
				{
					result = true;
					break;
				}
				if (animatorStateMachine == this.rootStateMachine)
				{
					result = false;
					break;
				}
				if (!recursive)
				{
					result = false;
					break;
				}
				stateMachineChild = animatorStateMachine;
			}
			return result;
		}

		private void FillStateMachineLookupFromStateMachine(AnimatorStateMachine stateMachine)
		{
			ChildAnimatorStateMachine[] stateMachines = stateMachine.stateMachines;
			for (int i = 0; i < stateMachines.Length; i++)
			{
				ChildAnimatorStateMachine childAnimatorStateMachine = stateMachines[i];
				this.FillStateMachineLookupFromStateMachine(childAnimatorStateMachine.stateMachine);
				this.m_StateMachineLookup.Add(childAnimatorStateMachine.stateMachine, stateMachine);
			}
			ChildAnimatorState[] states = stateMachine.states;
			for (int j = 0; j < states.Length; j++)
			{
				ChildAnimatorState childAnimatorState = states[j];
				this.m_StateMachineProxyLookup.Add(childAnimatorState.state, stateMachine);
			}
		}

		private void CreateNodes()
		{
			this.m_StateNodeLookup.Clear();
			this.m_StateMachineNodeLookup.Clear();
			ChildAnimatorState[] states = this.activeStateMachine.states;
			for (int i = 0; i < states.Length; i++)
			{
				ChildAnimatorState state = states[i];
				this.CreateNodeFromState(state);
			}
			ChildAnimatorStateMachine[] stateMachines = this.activeStateMachine.stateMachines;
			for (int j = 0; j < stateMachines.Length; j++)
			{
				ChildAnimatorStateMachine subStateMachine = stateMachines[j];
				this.CreateNodeFromStateMachine(subStateMachine);
			}
			this.CreateAnyStateNode();
			this.CreateEntryExitNodes();
			if (this.parentStateMachine)
			{
				this.CreateParentStateMachineNode();
			}
		}

		private void CreateParentStateMachineNode()
		{
			StateMachineNode stateMachineNode = this.CreateAndAddNode<StateMachineNode>("(Up) ", this.activeStateMachine.parentStateMachinePosition);
			stateMachineNode.stateMachine = this.parentStateMachine;
			stateMachineNode.style = "node hex";
			if (this.rootStateMachine.defaultState && this.HasState(this.parentStateMachine, this.rootStateMachine.defaultState, true) && !this.HasState(this.activeStateMachine, this.rootStateMachine.defaultState, true))
			{
				stateMachineNode.color = Styles.Color.Orange;
			}
			this.m_StateMachineNodeLookup.Add(this.parentStateMachine, stateMachineNode);
		}

		private void CreateAnyStateNode()
		{
			this.m_AnyStateNode = this.CreateAndAddNode<AnyStateNode>("Any State", this.activeStateMachine.anyStatePosition);
			this.m_AnyStateNode.color = Styles.Color.Aqua;
		}

		private void CreateEntryExitNodes()
		{
			this.m_EntryNode = this.CreateAndAddNode<EntryNode>("Entry", this.activeStateMachine.entryPosition);
			this.m_EntryNode.color = Styles.Color.Green;
			this.m_EntryNode.stateMachine = this.activeStateMachine;
			this.m_ExitNode = this.CreateAndAddNode<ExitNode>("Exit", this.activeStateMachine.exitPosition);
			this.m_ExitNode.color = Styles.Color.Red;
		}

		private void CreateNodeFromStateMachine(ChildAnimatorStateMachine subStateMachine)
		{
			StateMachineNode stateMachineNode = this.CreateAndAddNode<StateMachineNode>(string.Empty, subStateMachine.position);
			stateMachineNode.stateMachine = subStateMachine.stateMachine;
			stateMachineNode.style = "node hex";
			if (this.rootStateMachine.defaultState && this.HasState(subStateMachine.stateMachine, this.rootStateMachine.defaultState, true))
			{
				stateMachineNode.color = Styles.Color.Orange;
			}
			this.m_StateMachineNodeLookup.Add(subStateMachine.stateMachine, stateMachineNode);
		}

		private void CreateNodeFromState(ChildAnimatorState state)
		{
			StateNode stateNode = this.CreateAndAddNode<StateNode>(string.Empty, state.position);
			stateNode.state = state.state;
			if (this.rootStateMachine.defaultState == state.state)
			{
				stateNode.color = Styles.Color.Orange;
			}
			this.m_StateNodeLookup.Add(state.state, stateNode);
		}

		private T CreateAndAddNode<T>(string name, Vector3 position) where T : Node
		{
			T t = ScriptableObject.CreateInstance<T>();
			t.hideFlags = HideFlags.HideAndDontSave;
			t.name = name;
			t.position = new Rect(position.x, position.y, 0f, 0f);
			t.AddInputSlot("In");
			t.AddOutputSlot("Out");
			this.AddNode(t);
			return t;
		}

		public void ReadNodePositions()
		{
			using (List<UnityEditor.Graphs.Node>.Enumerator enumerator = this.nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Node node = (Node)enumerator.Current;
					if (node is StateNode)
					{
						this.ReadStatePosition(node as StateNode);
					}
					if (node is StateMachineNode)
					{
						this.ReadStateMachinePosition(node as StateMachineNode);
					}
					if (node is AnyStateNode)
					{
						this.ReadAnyStatePosition(node as AnyStateNode);
					}
				}
			}
		}

		private void ReadAnyStatePosition(AnyStateNode anyStateNode)
		{
			anyStateNode.position.x = this.activeStateMachine.anyStatePosition.x;
			anyStateNode.position.y = this.activeStateMachine.anyStatePosition.y;
		}

		private void ReadStateMachinePosition(StateMachineNode stateMachineNode)
		{
			Vector2 vector;
			if (stateMachineNode.stateMachine == this.parentStateMachine)
			{
				vector = this.activeStateMachine.parentStateMachinePosition;
			}
			else
			{
				vector = this.activeStateMachine.GetStateMachinePosition(stateMachineNode.stateMachine);
			}
			stateMachineNode.position.x = vector.x;
			stateMachineNode.position.y = vector.y;
		}

		private void ReadStatePosition(StateNode stateNode)
		{
			Vector2 vector = this.activeStateMachine.GetStatePosition(stateNode.state);
			stateNode.position.x = vector.x;
			stateNode.position.y = vector.y;
		}

		public bool DisplayDirty()
		{
			return this.activeStateMachine.states.Length != this.m_StateCount || this.activeStateMachine.stateMachines.Length != this.m_StateMachineCount || this.rootStateMachine.transitionCount != this.m_TransitionCount || this.rootStateMachine.defaultState != this.m_DefaultState;
		}
	}
}
