using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor.Graphs
{
	[EditorWindowTitle(title = "Parameters"), InitializeOnLoad]
	internal class ParameterControllerEditor : EditorWindow, IAnimatorControllerEditor
	{
		private const float kToolbarHeight = 17f;

		[SerializeField]
		private Animator m_PreviewAnimator;

		[SerializeField]
		private AnimatorController m_AnimatorController;

		public static ParameterControllerEditor tool;

		private ParameterControllerView m_Editor;

		public Animator previewAnimator
		{
			get
			{
				return this.m_PreviewAnimator;
			}
		}

		public AnimatorController animatorController
		{
			get
			{
				return this.m_AnimatorController;
			}
			set
			{
				this.m_AnimatorController = value;
				if (this.m_PreviewAnimator && AnimatorController.GetEffectiveAnimatorController(this.m_PreviewAnimator) != this.m_AnimatorController)
				{
					this.m_PreviewAnimator = null;
				}
			}
		}

		protected IAnimatorControllerSubEditor editor
		{
			get
			{
				if (this.m_Editor == null)
				{
					this.m_Editor = new ParameterControllerView();
				}
				return this.m_Editor;
			}
		}

		public bool liveLink
		{
			get
			{
				return EditorApplication.isPlaying && this.previewAnimator != null && this.previewAnimator.enabled && this.previewAnimator.gameObject.activeInHierarchy;
			}
		}

		[MenuItem("Window/Animator Parameter", false, 2012)]
		public static void DoWindow()
		{
			EditorWindow.GetWindow<ParameterControllerEditor>();
		}

		public void OnSelectionChange()
		{
			this.DetectAnimatorControllerFromSelection();
			this.DetectPreviewObjectFromSelection();
			this.editor.ResetUI();
			base.Repaint();
		}

		public void OnProjectChange()
		{
			this.DetectAnimatorControllerFromSelection();
			this.editor.ResetUI();
			base.Repaint();
		}

		private void OnControllerChange()
		{
			if (this.m_PreviewAnimator != null && AnimatorController.GetEffectiveAnimatorController(this.m_PreviewAnimator) != this.animatorController)
			{
				this.animatorController = AnimatorController.GetEffectiveAnimatorController(this.m_PreviewAnimator);
				this.editor.ResetUI();
			}
		}

		public void OnInvalidateAnimatorController()
		{
			this.editor.ResetUI();
			base.Repaint();
		}

		private void DetectPreviewObjectFromSelection()
		{
			if (Selection.activeGameObject)
			{
				Animator component = Selection.activeGameObject.GetComponent<Animator>();
				if (component && !AssetDatabase.Contains(Selection.activeGameObject))
				{
					this.m_PreviewAnimator = component;
					this.editor.ResetUI();
				}
			}
		}

		private void DetectAnimatorControllerFromSelection()
		{
			AnimatorController animatorController = null;
			if (Selection.activeObject == null && this.animatorController == null)
			{
				this.animatorController = null;
			}
			if (Selection.activeObject is AnimatorController && EditorUtility.IsPersistent(Selection.activeObject))
			{
				animatorController = (Selection.activeObject as AnimatorController);
			}
			if (Selection.activeGameObject)
			{
				Animator component = Selection.activeGameObject.GetComponent<Animator>();
				if (component)
				{
					AnimatorController effectiveAnimatorController = AnimatorController.GetEffectiveAnimatorController(component);
					if (effectiveAnimatorController)
					{
						animatorController = effectiveAnimatorController;
					}
				}
			}
			if (animatorController != null && animatorController != this.animatorController)
			{
				this.animatorController = animatorController;
				this.editor.ResetUI();
			}
		}

		private void DetectAnimatorControllerFromTool()
		{
			if (this.animatorController != null)
			{
				return;
			}
			if (AnimatorControllerTool.tool == null)
			{
				return;
			}
			this.animatorController = AnimatorControllerTool.tool.animatorController;
		}

		protected void Init()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			base.wantsMouseMove = true;
			this.editor.Init(this);
			ParameterControllerEditor.tool = this;
		}

		private void OnEnable()
		{
			this.Init();
			this.DetectAnimatorControllerFromSelection();
			this.DetectAnimatorControllerFromTool();
			this.editor.OnEnable();
		}

		public void OnFocus()
		{
			this.DetectAnimatorControllerFromSelection();
			this.DetectAnimatorControllerFromTool();
			this.DetectPreviewObjectFromSelection();
			this.editor.OnFocus();
		}

		private void OnLostFocus()
		{
			this.editor.OnLostFocus();
		}

		private void OnDisable()
		{
			this.editor.OnDisable();
		}

		public void OnGUI()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[]
			{
				GUILayout.Height(17f)
			});
			GUILayout.Space(10f);
			GUILayout.FlexibleSpace();
			this.editor.OnToolbarGUI();
			GUILayout.EndHorizontal();
			Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
			this.editor.OnEvent();
			this.editor.OnGUI(rect);
		}

		public void OnInspectorUpdate()
		{
			if (this.liveLink)
			{
				base.Repaint();
			}
		}

		public void ResetUI()
		{
		}

		virtual void Repaint()
		{
			base.Repaint();
		}
	}
}
