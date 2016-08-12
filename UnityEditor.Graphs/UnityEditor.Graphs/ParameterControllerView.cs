using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Graphs
{
	internal class ParameterControllerView : IAnimatorControllerSubEditor
	{
		private class Element : IComparable
		{
			protected const float kParameterValueWidth = 48f;

			protected const float kSpacing = 2f;

			public UnityEngine.AnimatorControllerParameter m_Parameter;

			public ParameterControllerView m_Host;

			public string name
			{
				get
				{
					return this.m_Parameter.name;
				}
				set
				{
					this.m_Parameter.name = value;
				}
			}

			public Element(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host)
			{
				this.m_Host = host;
				this.m_Parameter = parameter;
			}

			public int CompareTo(object o)
			{
				return this.name.CompareTo((o as ParameterControllerView.Element).name);
			}

			public virtual void OnGUI(Rect rect, int index)
			{
				Rect rect2 = new Rect(rect.xMax - 48f - 2f, rect.y, 48f, rect.height);
				Rect rect3 = Rect.MinMaxRect(rect.x, rect.yMin, rect2.xMin - 2f, rect.yMax);
				bool flag = this.m_Host.renameOverlay.IsRenaming() && this.m_Host.renameOverlay.userData == index && !this.m_Host.renameOverlay.isWaitingForDelay;
				if (flag)
				{
					if (rect3.width >= 0f && rect3.height >= 0f)
					{
						rect3.x -= 2f;
						this.m_Host.renameOverlay.editFieldRect = rect3;
					}
					if (!this.m_Host.renameOverlay.OnGUI())
					{
						this.m_Host.RenameEnd();
					}
				}
				else
				{
					GUI.Label(rect3, this.name);
				}
				bool flag2 = this.m_Host.m_Host.liveLink && this.m_Host.m_Host.previewAnimator.IsParameterControlledByCurve(this.name);
				using (new EditorGUI.DisabledScope(this.m_Host.m_Host.liveLink && flag2))
				{
					this.OnSpecializedGUI(rect2);
				}
			}

			public virtual void OnSpecializedGUI(Rect rect)
			{
			}
		}

		private class FloatElement : ParameterControllerView.Element
		{
			public float value
			{
				get
				{
					if (this.m_Host.m_Host.liveLink)
					{
						return this.m_Host.m_Host.previewAnimator.GetFloat(base.name);
					}
					return this.m_Parameter.defaultFloat;
				}
				set
				{
					if (this.m_Host.m_Host.liveLink)
					{
						this.m_Host.m_Host.previewAnimator.SetFloat(base.name, value);
					}
					else
					{
						Undo.RegisterCompleteObjectUndo(this.m_Host.m_Host.animatorController, "Parameter default value changed");
						this.m_Parameter.defaultFloat = value;
					}
				}
			}

			public FloatElement(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host) : base(parameter, host)
			{
			}

			public override void OnSpecializedGUI(Rect rect)
			{
				EditorGUI.BeginChangeCheck();
				float value = EditorGUI.FloatField(rect, this.value);
				if (EditorGUI.EndChangeCheck())
				{
					this.value = value;
				}
			}
		}

		private class BoolElement : ParameterControllerView.Element
		{
			public bool value
			{
				get
				{
					if (this.m_Host.m_Host.liveLink)
					{
						return this.m_Host.m_Host.previewAnimator.GetBool(base.name);
					}
					return this.m_Parameter.defaultBool;
				}
				set
				{
					if (this.m_Host.m_Host.liveLink)
					{
						this.m_Host.m_Host.previewAnimator.SetBool(base.name, value);
					}
					else
					{
						Undo.RegisterCompleteObjectUndo(this.m_Host.m_Host.animatorController, "Parameter default value changed");
						this.m_Parameter.defaultBool = value;
					}
				}
			}

			public BoolElement(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host) : base(parameter, host)
			{
			}

			public override void OnSpecializedGUI(Rect rect)
			{
				EditorGUI.BeginChangeCheck();
				bool value = GUI.Toggle(rect, this.value, string.Empty);
				if (EditorGUI.EndChangeCheck())
				{
					this.value = value;
				}
			}
		}

		private class IntElement : ParameterControllerView.Element
		{
			public int value
			{
				get
				{
					if (this.m_Host.m_Host.liveLink)
					{
						return this.m_Host.m_Host.previewAnimator.GetInteger(base.name);
					}
					return this.m_Parameter.defaultInt;
				}
				set
				{
					if (this.m_Host.m_Host.liveLink)
					{
						this.m_Host.m_Host.previewAnimator.SetInteger(base.name, value);
					}
					else
					{
						Undo.RegisterCompleteObjectUndo(this.m_Host.m_Host.animatorController, "Parameter default value changed");
						this.m_Parameter.defaultInt = value;
					}
				}
			}

			public IntElement(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host) : base(parameter, host)
			{
			}

			public override void OnSpecializedGUI(Rect rect)
			{
				EditorGUI.BeginChangeCheck();
				int value = EditorGUI.IntField(rect, this.value);
				if (EditorGUI.EndChangeCheck())
				{
					this.value = value;
				}
			}
		}

		private class TriggerElement : ParameterControllerView.BoolElement
		{
			public TriggerElement(UnityEngine.AnimatorControllerParameter parameter, ParameterControllerView host) : base(parameter, host)
			{
			}

			public override void OnSpecializedGUI(Rect rect)
			{
				EditorGUI.BeginChangeCheck();
				bool value = GUI.Toggle(rect, base.value, string.Empty, ParameterControllerView.s_Styles.triggerButton);
				if (EditorGUI.EndChangeCheck())
				{
					base.value = value;
				}
			}
		}

		private class Styles
		{
			public GUIContent searchContent = new GUIContent("Search");

			public GUIContent iconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add a new parameter");

			public readonly GUIStyle evenBackground = "CN EntryBackEven";

			public readonly GUIStyle oddBackground = "CN EntryBackodd";

			public readonly GUIStyle elementBackground = new GUIStyle("RL Element");

			public readonly GUIStyle invisibleButton = "InvisibleButton";

			public readonly GUIStyle triggerButton = "Radio";
		}

		private enum SearchMode
		{
			Name,
			Float,
			Int,
			Bool,
			Trigger
		}

		private const float kElementHeight = 24f;

		private static ParameterControllerView.Styles s_Styles;

		protected ReorderableList m_ParameterList;

		protected Vector2 m_ScrollPosition;

		protected int m_LastSelectedIndex;

		protected RenameOverlay m_RenameOverlay;

		private ParameterControllerView.Element[] m_Tree;

		private ParameterControllerView.Element[] m_SearchTree;

		private string m_Search = string.Empty;

		private bool m_HadKeyFocusAtMouseDown;

		private int m_SearchMode;

		private IAnimatorControllerEditor m_Host;

		public RenameOverlay renameOverlay
		{
			get
			{
				return this.m_RenameOverlay;
			}
		}

		private ParameterControllerView.Element[] activeTree
		{
			get
			{
				if (this.m_Tree == null)
				{
					this.CreateParameterList();
				}
				return (!(this.m_Search == string.Empty) || this.m_SearchMode != 0) ? this.m_SearchTree : this.m_Tree;
			}
		}

		private ParameterControllerView.Element CreateElement(UnityEngine.AnimatorControllerParameter parameter)
		{
			UnityEngine.AnimatorControllerParameterType type = parameter.type;
			UnityEngine.AnimatorControllerParameterType animatorControllerParameterType = type;
			switch (animatorControllerParameterType)
			{
			case UnityEngine.AnimatorControllerParameterType.Float:
				return new ParameterControllerView.FloatElement(parameter, this);
			case (UnityEngine.AnimatorControllerParameterType)2:
				IL_21:
				if (animatorControllerParameterType != UnityEngine.AnimatorControllerParameterType.Trigger)
				{
					return null;
				}
				return new ParameterControllerView.TriggerElement(parameter, this);
			case UnityEngine.AnimatorControllerParameterType.Int:
				return new ParameterControllerView.IntElement(parameter, this);
			case UnityEngine.AnimatorControllerParameterType.Bool:
				return new ParameterControllerView.BoolElement(parameter, this);
			}
			goto IL_21;
		}

		public void OnFocus()
		{
		}

		public void ResetUI()
		{
			this.RebuildList();
		}

		public void OnEnable()
		{
			this.m_Search = string.Empty;
			this.m_Tree = null;
			this.m_SearchTree = null;
			this.m_SearchMode = 0;
			this.m_ScrollPosition = new Vector2(0f, 0f);
			this.m_RenameOverlay = new RenameOverlay();
			this.m_LastSelectedIndex = -1;
			this.m_ParameterList = new ReorderableList(this.m_Tree, typeof(ParameterControllerView.Element), true, false, false, false);
			this.m_ParameterList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.OnReorderParameter);
			this.m_ParameterList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawParameter);
			this.m_ParameterList.drawElementBackgroundCallback = new ReorderableList.ElementCallbackDelegate(this.OnDrawBackgroundParameter);
			this.m_ParameterList.onMouseUpCallback = new ReorderableList.SelectCallbackDelegate(this.OnMouseUpParameter);
			this.m_ParameterList.index = 0;
			this.m_ParameterList.headerHeight = 0f;
			this.m_ParameterList.elementHeight = 24f;
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void OnDisable()
		{
			if (this.renameOverlay == null)
			{
				return;
			}
			if (this.renameOverlay.IsRenaming())
			{
				this.RenameEnd();
			}
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void OnDestroy()
		{
		}

		public void OnLostFocus()
		{
			if (this.renameOverlay == null)
			{
				return;
			}
			if (this.renameOverlay.IsRenaming())
			{
				this.renameOverlay.EndRename(true);
				this.RenameEnd();
			}
		}

		public void UndoRedoPerformed()
		{
			this.RebuildList();
			this.m_Host.Repaint();
		}

		private void DeleteParameter()
		{
			this.OnRemoveParameter(this.m_ParameterList.index);
		}

		private void OnMouseUpParameter(ReorderableList list)
		{
			if (!this.m_Host.liveLink && this.m_HadKeyFocusAtMouseDown && list.index == this.m_LastSelectedIndex && Event.current.button == 0)
			{
				ParameterControllerView.Element element = list.list[list.index] as ParameterControllerView.Element;
				this.renameOverlay.BeginRename(element.name, list.index, 0.1f);
			}
			this.m_LastSelectedIndex = list.index;
		}

		private void OnDrawBackgroundParameter(Rect rect, int index, bool selected, bool focused)
		{
			if (Event.current.type == EventType.Repaint)
			{
				GUIStyle gUIStyle = (index % 2 != 0) ? ParameterControllerView.s_Styles.evenBackground : ParameterControllerView.s_Styles.oddBackground;
				gUIStyle = ((!selected && !focused) ? gUIStyle : ParameterControllerView.s_Styles.elementBackground);
				gUIStyle.Draw(rect, false, selected, selected, focused);
			}
		}

		private void OnDrawParameter(Rect rect, int index, bool selected, bool focused)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseUp && current.button == 1 && rect.Contains(current.mousePosition))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteParameter));
				genericMenu.ShowAsContext();
				Event.current.Use();
			}
			if (index >= this.m_ParameterList.list.Count)
			{
				return;
			}
			ParameterControllerView.Element element = this.m_ParameterList.list[index] as ParameterControllerView.Element;
			rect.yMin += 2f;
			rect.yMax -= 3f;
			element.OnGUI(rect, index);
		}

		private void ResetTextFields()
		{
			EditorGUI.EndEditingActiveTextField();
			GUIUtility.keyboardControl = 0;
		}

		private void OnReorderParameter(ReorderableList reorderablelist)
		{
			ParameterControllerView.Element[] array = new ParameterControllerView.Element[reorderablelist.list.Count];
			reorderablelist.list.CopyTo(array, 0);
			UnityEngine.AnimatorControllerParameter[] array2 = new UnityEngine.AnimatorControllerParameter[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null || array[i].m_Parameter == null)
				{
					return;
				}
				array2[i] = array[i].m_Parameter;
			}
			Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Parameter reordering");
			this.m_Host.animatorController.parameters = array2;
			this.RebuildList();
		}

		private void OnAddParameter(Rect buttonRect)
		{
			GenericMenu genericMenu = new GenericMenu();
			foreach (object current in Enum.GetValues(typeof(UnityEngine.AnimatorControllerParameterType)))
			{
				genericMenu.AddItem(new GUIContent(current.ToString()), false, new GenericMenu.MenuFunction2(this.AddParameterMenu), current);
			}
			genericMenu.DropDown(buttonRect);
		}

		private void OnRemoveParameter(int index)
		{
			if (this.m_Host.liveLink)
			{
				return;
			}
			if (this.m_ParameterList.list.Count <= 0)
			{
				return;
			}
			ParameterControllerView.Element element = this.m_ParameterList.list[index] as ParameterControllerView.Element;
			List<UnityEngine.Object> list = this.m_Host.animatorController.CollectObjectsUsingParameter(element.name).ToList<UnityEngine.Object>();
			bool flag = false;
			if (list.Count > 0)
			{
				string title = "Delete parameter " + element.name + "?";
				string text = "It is used by : \n";
				foreach (UnityEngine.Object current in list)
				{
					AnimatorTransitionBase animatorTransitionBase = current as AnimatorTransitionBase;
					if (animatorTransitionBase != null && animatorTransitionBase.destinationState != null)
					{
						text = text + "Transition to " + animatorTransitionBase.destinationState.name + "\n";
					}
					else if (animatorTransitionBase != null && animatorTransitionBase.destinationStateMachine != null)
					{
						text = text + "Transition to " + animatorTransitionBase.destinationStateMachine.name + "\n";
					}
					else
					{
						text = text + current.name + "\n";
					}
				}
				if (EditorUtility.DisplayDialog(title, text, "Delete", "Cancel"))
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				this.ResetTextFields();
				list.Add(this.m_Host.animatorController);
				Undo.RegisterCompleteObjectUndo(list.ToArray(), "Parameter removed");
				this.m_Host.animatorController.RemoveParameter(element.m_Parameter);
				this.RebuildList();
				this.m_ParameterList.GrabKeyboardFocus();
			}
		}

		protected void RebuildList()
		{
			this.CreateParameterList();
			this.CreateSearchParameterList(this.m_Search, this.m_SearchMode);
		}

		protected void RenameEnd()
		{
			if (this.renameOverlay.userAcceptedRename)
			{
				string text = (!string.IsNullOrEmpty(this.renameOverlay.name)) ? this.renameOverlay.name : this.renameOverlay.originalName;
				if (text != this.renameOverlay.originalName)
				{
					int userData = this.renameOverlay.userData;
					ParameterControllerView.Element element = this.m_ParameterList.list[userData] as ParameterControllerView.Element;
					text = this.m_Host.animatorController.MakeUniqueParameterName(text);
					Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Parameter renamed");
					this.m_Host.animatorController.RenameParameter(element.name, text);
					element.name = text;
					UnityEngine.AnimatorControllerParameter[] parameters = this.m_Host.animatorController.parameters;
					parameters[userData] = element.m_Parameter;
					this.m_Host.animatorController.parameters = parameters;
				}
			}
			this.m_ParameterList.GrabKeyboardFocus();
			this.renameOverlay.Clear();
		}

		public void Init(IAnimatorControllerEditor host)
		{
			this.m_Host = host;
		}

		protected void CreateParameterList()
		{
			List<ParameterControllerView.Element> list = new List<ParameterControllerView.Element>();
			if (this.m_Host.animatorController != null)
			{
				UnityEngine.AnimatorControllerParameter[] parameters = this.m_Host.animatorController.parameters;
				for (int i = 0; i < parameters.Length; i++)
				{
					list.Add(this.CreateElement(parameters[i]));
				}
			}
			this.m_Tree = list.ToArray();
		}

		protected void CreateSearchParameterList(string newSearch, int searchMode)
		{
			string[] array = newSearch.ToLower().Split(new char[]
			{
				' '
			});
			List<ParameterControllerView.Element> list = new List<ParameterControllerView.Element>();
			List<ParameterControllerView.Element> list2 = new List<ParameterControllerView.Element>();
			ParameterControllerView.Element[] tree = this.m_Tree;
			int i = 0;
			while (i < tree.Length)
			{
				ParameterControllerView.Element element = tree[i];
				if (searchMode == 0)
				{
					goto IL_76;
				}
				string a = element.m_Parameter.type.ToString();
				if (!(a != ((ParameterControllerView.SearchMode)searchMode).ToString()))
				{
					goto IL_76;
				}
				IL_10E:
				i++;
				continue;
				IL_76:
				string text = element.name;
				text = text.ToLower().Replace(" ", string.Empty);
				bool flag = true;
				bool flag2 = false;
				for (int j = 0; j < array.Length; j++)
				{
					string value = array[j];
					if (!text.Contains(value))
					{
						flag = false;
						break;
					}
					if (j == 0 && text.StartsWith(value))
					{
						flag2 = true;
					}
				}
				if (!flag)
				{
					goto IL_10E;
				}
				if (flag2)
				{
					list.Add(element);
					goto IL_10E;
				}
				list2.Add(element);
				goto IL_10E;
			}
			list.Sort();
			list2.Sort();
			List<ParameterControllerView.Element> list3 = new List<ParameterControllerView.Element>();
			list3.AddRange(list);
			list3.AddRange(list2);
			this.m_SearchTree = list3.ToArray();
		}

		private void AddParameterMenu(object value)
		{
			Undo.RegisterCompleteObjectUndo(this.m_Host.animatorController, "Parameter Added");
			UnityEngine.AnimatorControllerParameterType animatorControllerParameterType = (UnityEngine.AnimatorControllerParameterType)((int)value);
			string name = "New " + animatorControllerParameterType.ToString();
			this.m_Host.animatorController.AddParameter(name, animatorControllerParameterType);
			this.RebuildList();
			this.m_ParameterList.index = this.m_Tree.Length - 1;
			if (this.renameOverlay.IsRenaming())
			{
				this.RenameEnd();
			}
			this.renameOverlay.BeginRename(this.m_Host.animatorController.parameters[this.m_ParameterList.index].name, this.m_ParameterList.index, 0.1f);
		}

		public void OnToolbarGUI()
		{
			if (ParameterControllerView.s_Styles == null)
			{
				ParameterControllerView.s_Styles = new ParameterControllerView.Styles();
			}
			if (this.m_Host == null)
			{
				return;
			}
			using (new EditorGUI.DisabledScope(this.m_Host.animatorController == null))
			{
				string[] names = Enum.GetNames(typeof(ParameterControllerView.SearchMode));
				int searchMode = this.m_SearchMode;
				GUI.SetNextControlName("ParameterSearch");
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape && GUI.GetNameOfFocusedControl() == "ParameterSearch")
				{
					this.m_Search = string.Empty;
					this.CreateSearchParameterList(this.m_Search, this.m_SearchMode);
				}
				EditorGUI.BeginChangeCheck();
				string search = EditorGUILayout.ToolbarSearchField(this.m_Search, names, ref searchMode, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_Search = search;
					this.m_SearchMode = searchMode;
					this.CreateSearchParameterList(this.m_Search, this.m_SearchMode);
				}
				GUILayout.Space(10f);
				using (new EditorGUI.DisabledScope(this.m_Host.liveLink))
				{
					Rect rect = GUILayoutUtility.GetRect(ParameterControllerView.s_Styles.iconToolbarPlusMore, ParameterControllerView.s_Styles.invisibleButton);
					if (GUI.Button(rect, ParameterControllerView.s_Styles.iconToolbarPlusMore, ParameterControllerView.s_Styles.invisibleButton))
					{
						this.OnAddParameter(rect);
					}
				}
			}
		}

		private void DoParameterList()
		{
			int num = this.m_ParameterList.index;
			this.m_ParameterList.list = this.activeTree;
			if (num >= this.activeTree.Length)
			{
				num = this.activeTree.Length - 1;
			}
			this.m_ParameterList.index = num;
			this.m_ParameterList.draggable = (!this.m_Host.liveLink && this.activeTree == this.m_Tree);
			string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
			EditorGUI.kFloatFieldFormatString = "f1";
			this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			this.m_ParameterList.DoLayoutList();
			if (EditorGUI.EndChangeCheck() && !this.m_Host.liveLink)
			{
				ParameterControllerView.Element[] tree = this.m_Tree;
				UnityEngine.AnimatorControllerParameter[] array = new UnityEngine.AnimatorControllerParameter[tree.Length];
				for (int i = 0; i < tree.Length; i++)
				{
					array[i] = tree[i].m_Parameter;
				}
				this.m_Host.animatorController.parameters = array;
			}
			GUILayout.EndScrollView();
			EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
		}

		private void KeyboardHandling()
		{
			if (this.m_ParameterList == null)
			{
				return;
			}
			if (!this.m_ParameterList.HasKeyboardControl())
			{
				return;
			}
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.KeyDown)
			{
				if (type == EventType.ExecuteCommand)
				{
					if (current.commandName == "SoftDelete" || current.commandName == "Delete")
					{
						current.Use();
						this.OnRemoveParameter(this.m_ParameterList.index);
					}
				}
			}
			else
			{
				KeyCode keyCode = Event.current.keyCode;
				if (keyCode != KeyCode.Home)
				{
					if (keyCode != KeyCode.End)
					{
						if (keyCode == KeyCode.Delete)
						{
							current.Use();
							this.OnRemoveParameter(this.m_ParameterList.index);
						}
					}
					else
					{
						current.Use();
						this.m_ParameterList.index = this.m_ParameterList.count - 1;
					}
				}
				else
				{
					current.Use();
					this.m_ParameterList.index = 0;
				}
			}
		}

		public void OnEvent()
		{
			if (this.renameOverlay == null)
			{
				return;
			}
			this.renameOverlay.OnEvent();
		}

		public void OnGUI(Rect rect)
		{
			if (ParameterControllerView.s_Styles == null)
			{
				ParameterControllerView.s_Styles = new ParameterControllerView.Styles();
			}
			Event current = Event.current;
			if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
			{
				this.m_HadKeyFocusAtMouseDown = this.m_ParameterList.HasKeyboardControl();
			}
			this.KeyboardHandling();
			if (this.m_Host.animatorController == null)
			{
				return;
			}
			this.DoParameterList();
		}

		public void ReleaseKeyboardFocus()
		{
			this.m_ParameterList.ReleaseKeyboardFocus();
			this.m_Host.Repaint();
		}

		public void GrabKeyboardFocus()
		{
			this.m_ParameterList.GrabKeyboardFocus();
			this.m_Host.Repaint();
		}

		public bool HasKeyboardControl()
		{
			return this.m_ParameterList.HasKeyboardControl();
		}
	}
}
