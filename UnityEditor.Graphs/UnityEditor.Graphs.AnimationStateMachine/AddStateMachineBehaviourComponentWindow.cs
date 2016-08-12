using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs.AnimationStateMachine
{
	internal class AddStateMachineBehaviourComponentWindow : EditorWindow
	{
		internal enum Language
		{
			JavaScript,
			CSharp,
			Boo
		}

		private class Element : IComparable
		{
			public GUIContent content;

			public string name
			{
				get
				{
					return this.content.text;
				}
			}

			public Element(string name)
			{
				this.content = new GUIContent(name);
			}

			public int CompareTo(object o)
			{
				return this.name.CompareTo((o as AddStateMachineBehaviourComponentWindow.Element).name);
			}

			public virtual bool OnGUI(bool selected)
			{
				Rect rect = GUILayoutUtility.GetRect(this.content, AddStateMachineBehaviourComponentWindow.s_Styles.componentButton, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				if (Event.current.type == EventType.Repaint)
				{
					AddStateMachineBehaviourComponentWindow.s_Styles.componentButton.Draw(rect, this.content, false, false, selected, selected);
				}
				if (Event.current.type == EventType.MouseMove)
				{
					return rect.Contains(Event.current.mousePosition);
				}
				if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
				{
					Event.current.Use();
					this.Create();
					return true;
				}
				return false;
			}

			public virtual bool CanShow()
			{
				return false;
			}

			public virtual bool IsShow()
			{
				return false;
			}

			public virtual void Show()
			{
			}

			public virtual void Hide()
			{
			}

			public virtual void Create()
			{
			}
		}

		private class ScriptElement : AddStateMachineBehaviourComponentWindow.Element
		{
			private MonoScript script;

			public ScriptElement(string scriptName, MonoScript scriptObject) : base(scriptName)
			{
				this.script = scriptObject;
			}

			public override void Create()
			{
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.AddBehaviour(this.script);
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.Close();
			}
		}

		private class NewScriptElement : AddStateMachineBehaviourComponentWindow.Element
		{
			private const string kResourcesTemplatePath = "Resources/ScriptTemplates";

			private char[] kInvalidPathChars = new char[]
			{
				'<',
				'>',
				':',
				'"',
				'|',
				'?',
				'*',
				'\0'
			};

			private char[] kPathSepChars = new char[]
			{
				'/',
				'\\'
			};

			private string m_Directory = string.Empty;

			public bool isShow;

			public bool isStateMachine;

			private string extension
			{
				get
				{
					return "cs";
				}
			}

			private string templatePath
			{
				get
				{
					string path = Path.Combine(EditorApplication.applicationContentsPath, "Resources/ScriptTemplates");
					if (this.isStateMachine)
					{
						return Path.Combine(path, "86-C# Script-NewSubStateMachineBehaviourScript.cs.txt");
					}
					return Path.Combine(path, "86-C# Script-NewStateMachineBehaviourScript.cs.txt");
				}
			}

			public NewScriptElement(bool stateMachine) : base("New Script")
			{
				this.isStateMachine = stateMachine;
			}

			public override bool CanShow()
			{
				return true;
			}

			public override bool IsShow()
			{
				return this.isShow;
			}

			public override void Show()
			{
				this.isShow = true;
			}

			public override void Hide()
			{
				this.isShow = false;
			}

			public override bool OnGUI(bool selected)
			{
				if (!this.IsShow())
				{
					bool result = base.OnGUI(selected);
					if (Event.current.type == EventType.Repaint)
					{
						Rect lastRect = GUILayoutUtility.GetLastRect();
						Rect position = new Rect(lastRect.x + lastRect.width - 13f, lastRect.y + 4f, 13f, 13f);
						AddStateMachineBehaviourComponentWindow.s_Styles.rightArrow.Draw(position, false, false, false, false);
					}
					return result;
				}
				GUILayout.Label("Name", EditorStyles.label, new GUILayoutOption[0]);
				EditorGUI.FocusTextInControl("NewScriptName");
				GUI.SetNextControlName("NewScriptName");
				AddStateMachineBehaviourComponentWindow.className = EditorGUILayout.TextField(AddStateMachineBehaviourComponentWindow.className, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				bool flag = this.CanCreate();
				if (!flag && AddStateMachineBehaviourComponentWindow.className != string.Empty)
				{
					GUILayout.Label(this.GetError(), EditorStyles.helpBox, new GUILayoutOption[0]);
				}
				GUILayout.FlexibleSpace();
				using (new EditorGUI.DisabledScope(!flag))
				{
					if (GUILayout.Button("Create and Add", new GUILayoutOption[0]))
					{
						this.Create();
					}
				}
				EditorGUILayout.Space();
				return false;
			}

			public bool CanCreate()
			{
				return AddStateMachineBehaviourComponentWindow.className.Length > 0 && !File.Exists(this.TargetPath()) && !this.ClassAlreadyExists() && !this.ClassNameIsInvalid() && !this.InvalidTargetPath();
			}

			private string GetError()
			{
				string result = string.Empty;
				if (AddStateMachineBehaviourComponentWindow.className != string.Empty)
				{
					if (File.Exists(this.TargetPath()))
					{
						result = "A script called \"" + AddStateMachineBehaviourComponentWindow.className + "\" already exists at that path.";
					}
					else if (this.ClassAlreadyExists())
					{
						result = "A class called \"" + AddStateMachineBehaviourComponentWindow.className + "\" already exists.";
					}
					else if (this.ClassNameIsInvalid())
					{
						result = "The script name may only consist of a-z, A-Z, 0-9, _.";
					}
					else if (this.InvalidTargetPath())
					{
						result = "The folder path contains invalid characters.";
					}
				}
				return result;
			}

			public override void Create()
			{
				if (!this.IsShow())
				{
					this.Show();
					AddStateMachineBehaviourComponentWindow.className = AddStateMachineBehaviourComponentWindow.searchName;
					return;
				}
				if (!this.CanCreate())
				{
					return;
				}
				this.CreateScript();
				MonoScript monoScript = AssetDatabase.LoadAssetAtPath(this.TargetPath(), typeof(MonoScript)) as MonoScript;
				monoScript.SetScriptTypeWasJustCreatedFromComponentMenu();
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.AddBehaviour(monoScript);
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.Close();
			}

			private bool InvalidTargetPath()
			{
				return this.m_Directory.IndexOfAny(this.kInvalidPathChars) >= 0 || this.TargetDir().Split(this.kPathSepChars, StringSplitOptions.None).Contains(string.Empty);
			}

			public string TargetPath()
			{
				return Path.Combine(this.TargetDir(), AddStateMachineBehaviourComponentWindow.className + "." + this.extension);
			}

			private string TargetDir()
			{
				return Path.Combine("Assets", this.m_Directory.Trim(this.kPathSepChars));
			}

			private bool ClassNameIsInvalid()
			{
				return !CodeGenerator.IsValidLanguageIndependentIdentifier(AddStateMachineBehaviourComponentWindow.className);
			}

			private bool ClassExists(string className)
			{
				return AppDomain.CurrentDomain.GetAssemblies().Any((Assembly a) => a.GetType(className, false) != null);
			}

			private bool ClassAlreadyExists()
			{
				return !(AddStateMachineBehaviourComponentWindow.className == string.Empty) && this.ClassExists(AddStateMachineBehaviourComponentWindow.className);
			}

			private void CreateScript()
			{
				ProjectWindowUtil.CreateScriptAssetFromTemplate(this.TargetPath(), this.templatePath);
				AssetDatabase.Refresh();
			}
		}

		private class Styles
		{
			public GUIStyle header = new GUIStyle(EditorStyles.inspectorBig);

			public GUIStyle componentButton = new GUIStyle("PR Label");

			public GUIStyle background = "grey_border";

			public GUIStyle rightArrow = "AC RightArrow";

			public GUIStyle leftArrow = "AC LeftArrow";

			public GUIContent searchContent = new GUIContent("Search");

			public GUIContent behaviourContent = new GUIContent("Behaviour");

			public Styles()
			{
				this.header.font = EditorStyles.boldLabel.font;
				this.componentButton.alignment = TextAnchor.MiddleLeft;
				this.componentButton.padding.left -= 15;
				this.componentButton.fixedHeight = 20f;
			}
		}

		private const int kHeaderHeight = 30;

		private const int kWindowHeight = 320;

		private const int kHelpHeight = 0;

		private const string kStateMachineBehaviourSearch = "StateMachineBehaviourSearchString";

		private static AddStateMachineBehaviourComponentWindow.Styles s_Styles;

		private static AddStateMachineBehaviourComponentWindow s_AddComponentWindow;

		private static long s_LastClosedTime;

		private string m_ClassName = string.Empty;

		private AnimatorController m_Controller;

		private int m_LayerIndex;

		private UnityEngine.Object[] m_Targets;

		private Vector2 m_ScrollPosition;

		private AddStateMachineBehaviourComponentWindow.Element[] m_Tree;

		private AddStateMachineBehaviourComponentWindow.Element[] m_SearchTree;

		private string m_Search;

		private int m_SelectedIndex;

		internal static string className
		{
			get
			{
				return AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_ClassName;
			}
			set
			{
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_ClassName = value;
			}
		}

		internal static AnimatorController controller
		{
			get
			{
				return AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_Controller;
			}
		}

		internal static int layerIndex
		{
			get
			{
				return AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_LayerIndex;
			}
		}

		internal static UnityEngine.Object[] targets
		{
			get
			{
				return AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_Targets;
			}
		}

		internal static string searchName
		{
			get
			{
				return AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_Search;
			}
			set
			{
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_Search = value;
			}
		}

		private int selectedIndex
		{
			get
			{
				return this.m_SelectedIndex;
			}
			set
			{
				AddStateMachineBehaviourComponentWindow.Element[] activeTree = this.activeTree;
				this.m_SelectedIndex = Math.Min(Math.Max(value, 0), activeTree.Length - 1);
			}
		}

		private AddStateMachineBehaviourComponentWindow.Element activeElement
		{
			get
			{
				AddStateMachineBehaviourComponentWindow.Element[] activeTree = this.activeTree;
				if (this.selectedIndex < 0 || this.selectedIndex >= activeTree.Length)
				{
					return null;
				}
				return activeTree[this.selectedIndex];
			}
		}

		private AddStateMachineBehaviourComponentWindow.Element[] activeTree
		{
			get
			{
				return (!(this.m_Search == string.Empty)) ? this.m_SearchTree : this.m_Tree;
			}
		}

		private GUIContent activeHeader
		{
			get
			{
				if (this.activeElement != null && this.activeElement.IsShow())
				{
					return this.activeElement.content;
				}
				if (this.activeTree == this.m_SearchTree)
				{
					return AddStateMachineBehaviourComponentWindow.s_Styles.searchContent;
				}
				return AddStateMachineBehaviourComponentWindow.s_Styles.behaviourContent;
			}
		}

		private void OnEnable()
		{
			AddStateMachineBehaviourComponentWindow.s_AddComponentWindow = this;
			this.m_SelectedIndex = 0;
			this.m_Search = EditorPrefs.GetString("StateMachineBehaviourSearchString", string.Empty);
			this.m_Tree = null;
			this.m_SearchTree = null;
		}

		private void OnDisable()
		{
			AddStateMachineBehaviourComponentWindow.s_LastClosedTime = DateTime.Now.Ticks / 10000L;
			AddStateMachineBehaviourComponentWindow.s_AddComponentWindow = null;
		}

		internal static bool Show(Rect rect, AnimatorController controller, int layerIndex, UnityEngine.Object[] targets)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(AddStateMachineBehaviourComponentWindow));
			if (array.Length > 0)
			{
				((EditorWindow)array[0]).Close();
				return false;
			}
			long num = DateTime.Now.Ticks / 10000L;
			if (num >= AddStateMachineBehaviourComponentWindow.s_LastClosedTime + 50L)
			{
				Event.current.Use();
				if (AddStateMachineBehaviourComponentWindow.s_AddComponentWindow == null)
				{
					AddStateMachineBehaviourComponentWindow.s_AddComponentWindow = ScriptableObject.CreateInstance<AddStateMachineBehaviourComponentWindow>();
				}
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_Controller = controller;
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_LayerIndex = layerIndex;
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.m_Targets = targets;
				AddStateMachineBehaviourComponentWindow.s_AddComponentWindow.Init(rect);
				return true;
			}
			return false;
		}

		private void Init(Rect rect)
		{
			rect = GUIUtility.GUIToScreenRect(rect);
			this.CreateBehaviourTree();
			base.ShowAsDropDown(rect, new Vector2(rect.width, 320f));
			base.Focus();
			this.m_Parent.AddToAuxWindowList();
			base.wantsMouseMove = true;
		}

		private void CreateBehaviourTree()
		{
			List<AddStateMachineBehaviourComponentWindow.Element> list = new List<AddStateMachineBehaviourComponentWindow.Element>();
			MonoScript[] array = Resources.FindObjectsOfTypeAll(typeof(MonoScript)) as MonoScript[];
			for (int i = 0; i < array.Length; i++)
			{
				Type @class = array[i].GetClass();
				if (@class != null && !@class.IsAbstract && @class.IsSubclassOf(typeof(StateMachineBehaviour)))
				{
					list.Add(new AddStateMachineBehaviourComponentWindow.ScriptElement(array[i].name, array[i]));
				}
			}
			list.Add(new AddStateMachineBehaviourComponentWindow.NewScriptElement(AddStateMachineBehaviourComponentWindow.targets[0] is AnimatorStateMachine));
			this.m_Tree = list.ToArray();
			this.CreateSearchTree(this.m_Search);
		}

		private void CreateSearchTree(string newSearch)
		{
			string[] array = newSearch.ToLower().Split(new char[]
			{
				' '
			});
			List<AddStateMachineBehaviourComponentWindow.Element> list = new List<AddStateMachineBehaviourComponentWindow.Element>();
			List<AddStateMachineBehaviourComponentWindow.Element> list2 = new List<AddStateMachineBehaviourComponentWindow.Element>();
			AddStateMachineBehaviourComponentWindow.Element[] tree = this.m_Tree;
			for (int i = 0; i < tree.Length; i++)
			{
				AddStateMachineBehaviourComponentWindow.Element element = tree[i];
				if (!(element is AddStateMachineBehaviourComponentWindow.NewScriptElement))
				{
					string text = element.name.ToLower().Replace(" ", string.Empty);
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
					if (flag)
					{
						if (flag2)
						{
							list.Add(element);
						}
						else
						{
							list2.Add(element);
						}
					}
				}
			}
			list.Sort();
			list2.Sort();
			List<AddStateMachineBehaviourComponentWindow.Element> list3 = new List<AddStateMachineBehaviourComponentWindow.Element>();
			list3.AddRange(list);
			list3.AddRange(list2);
			list3.Add(this.m_Tree[this.m_Tree.Length - 1]);
			this.m_SearchTree = list3.ToArray();
			this.selectedIndex = 0;
		}

		public void OnGUI()
		{
			if (AddStateMachineBehaviourComponentWindow.s_Styles == null)
			{
				AddStateMachineBehaviourComponentWindow.s_Styles = new AddStateMachineBehaviourComponentWindow.Styles();
			}
			GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, AddStateMachineBehaviourComponentWindow.s_Styles.background);
			this.HandleKeyboard();
			GUILayout.Space(7f);
			AddStateMachineBehaviourComponentWindow.Element activeElement = this.activeElement;
			if (!(activeElement is AddStateMachineBehaviourComponentWindow.NewScriptElement) || !activeElement.IsShow())
			{
				EditorGUI.FocusTextInControl("StateMachineBehaviourSearch");
			}
			Rect rect = GUILayoutUtility.GetRect(10f, 20f);
			rect.x += 8f;
			rect.width -= 16f;
			GUI.SetNextControlName("StateMachineBehaviourSearch");
			EditorGUI.BeginChangeCheck();
			bool disabled = activeElement is AddStateMachineBehaviourComponentWindow.NewScriptElement && activeElement.IsShow();
			string search;
			using (new EditorGUI.DisabledScope(disabled))
			{
				search = EditorGUI.SearchField(rect, this.m_Search);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.m_Search = search;
				EditorPrefs.SetString("StateMachineBehaviourSearchString", this.m_Search);
				this.CreateSearchTree(this.m_Search);
			}
			Rect position = base.position;
			position.x = 0f;
			position.y = 30f;
			position.height -= 30f;
			position.width -= 2f;
			GUILayout.BeginArea(position);
			Rect rect2 = GUILayoutUtility.GetRect(10f, 25f);
			GUI.Label(rect2, this.activeHeader, AddStateMachineBehaviourComponentWindow.s_Styles.header);
			if (this.activeElement is AddStateMachineBehaviourComponentWindow.NewScriptElement && this.activeElement.IsShow())
			{
				this.activeElement.OnGUI(false);
			}
			else
			{
				this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
				for (int i = 0; i < this.activeTree.Length; i++)
				{
					if (this.activeTree[i].OnGUI(i == this.selectedIndex))
					{
						this.selectedIndex = i;
						base.Repaint();
					}
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();
		}

		private void HandleKeyboard()
		{
			Event current = Event.current;
			if (current.type == EventType.KeyDown)
			{
				if (this.activeElement.IsShow())
				{
					if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
					{
						this.activeElement.Create();
						current.Use();
						GUIUtility.ExitGUI();
					}
					if (current.keyCode == KeyCode.Escape)
					{
						this.activeElement.Hide();
						current.Use();
					}
				}
				else
				{
					if (current.keyCode == KeyCode.DownArrow)
					{
						this.selectedIndex++;
						current.Use();
					}
					if (current.keyCode == KeyCode.UpArrow)
					{
						this.selectedIndex--;
						current.Use();
					}
					if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
					{
						this.activeElement.Create();
						current.Use();
					}
					if (this.m_Search == string.Empty && current.keyCode == KeyCode.Escape)
					{
						base.Close();
						current.Use();
					}
					if (this.m_Search == string.Empty && current.keyCode == KeyCode.RightArrow)
					{
						this.activeElement.Create();
						current.Use();
					}
				}
			}
		}

		internal void AddBehaviour(MonoScript stateMachineBehaviour)
		{
			UnityEngine.Object[] targets = AddStateMachineBehaviourComponentWindow.targets;
			for (int i = 0; i < targets.Length; i++)
			{
				UnityEngine.Object @object = targets[i];
				AnimatorState animatorState = @object as AnimatorState;
				AnimatorStateMachine animatorStateMachine = @object as AnimatorStateMachine;
				if (animatorState || animatorStateMachine)
				{
					int num = AnimatorController.CreateStateMachineBehaviour(stateMachineBehaviour);
					if (num == 0)
					{
						Debug.LogError("Could not create state machine behaviour " + stateMachineBehaviour.name);
						return;
					}
					string format = "Add Behaviour '{0}' to state '{1}'";
					if (animatorState != null)
					{
						if (AddStateMachineBehaviourComponentWindow.controller != null)
						{
							Undo.RegisterCompleteObjectUndo(AddStateMachineBehaviourComponentWindow.controller, string.Format(format, stateMachineBehaviour.name, animatorState.name));
							Undo.RegisterCompleteObjectUndo(animatorState, string.Format(format, stateMachineBehaviour.name, animatorState.name));
							AddStateMachineBehaviourComponentWindow.controller.AddStateEffectiveBehaviour(animatorState, AddStateMachineBehaviourComponentWindow.layerIndex, num);
						}
						else
						{
							Undo.RegisterCompleteObjectUndo(animatorState, string.Format(format, stateMachineBehaviour.name, animatorState.name));
							animatorState.AddBehaviour(num);
						}
						AssetDatabase.AddInstanceIDToAssetWithRandomFileId(num, animatorState, true);
					}
					else if (animatorStateMachine != null)
					{
						Undo.RegisterCompleteObjectUndo(animatorStateMachine, string.Format(format, stateMachineBehaviour.name, animatorStateMachine.name));
						animatorStateMachine.AddBehaviour(num);
						AssetDatabase.AddInstanceIDToAssetWithRandomFileId(num, animatorStateMachine, true);
					}
				}
			}
		}
	}
}
