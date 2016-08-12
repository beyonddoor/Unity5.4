using System;
using UnityEngine;

namespace UnityEditor.Graphs
{
	internal class ProxyNode : Node
	{
		[SerializeField]
		private bool m_IsIn;

		public bool isIn
		{
			get
			{
				return this.m_IsIn;
			}
		}

		public static ProxyNode Instance(bool isIn)
		{
			ProxyNode proxyNode = Node.Instance<ProxyNode>();
			proxyNode.Init(isIn);
			return proxyNode;
		}

		public void Init(bool isIn)
		{
			this.m_IsIn = isIn;
		}
	}
}
