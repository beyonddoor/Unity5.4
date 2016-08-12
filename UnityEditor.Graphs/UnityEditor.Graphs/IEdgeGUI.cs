using System;
using System.Collections.Generic;

namespace UnityEditor.Graphs
{
	public interface IEdgeGUI
	{
		GraphGUI host
		{
			get;
			set;
		}

		List<int> edgeSelection
		{
			get;
			set;
		}

		void DoEdges();

		void DoDraggedEdge();

		void BeginSlotDragging(Slot slot, bool allowStartDrag, bool allowEndDrag);

		void SlotDragging(Slot slot, bool allowEndDrag, bool allowMultiple);

		void EndSlotDragging(Slot slot, bool allowMultiple);

		void EndDragging();

		Edge FindClosestEdge();
	}
}
