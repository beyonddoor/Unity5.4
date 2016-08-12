using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Graphs
{
	public class Styles
	{
		public enum Color
		{
			Grey,
			Gray = 0,
			Blue,
			Aqua,
			Green,
			Yellow,
			Orange,
			Red
		}

		public static GUIStyle graphBackground;

		public static GUIContent connectionTexture;

		public static GUIContent selectedConnectionTexture;

		public static GUIStyle varPinIn;

		public static GUIStyle varPinOut;

		public static GUIStyle varPinTooltip;

		public static GUIStyle targetPinIn;

		public static GUIStyle triggerPinIn;

		public static GUIStyle triggerPinOut;

		public static GUIStyle selectionRect;

		public static GUIStyle nodeGroupButton;

		public static GUIStyle nodeTitlebar;

		public static GUIStyle nodeAddButton;

		private static readonly Dictionary<string, GUIStyle> m_NodeStyleCache;

		static Styles()
		{
			Styles.graphBackground = "flow background";
			Styles.connectionTexture = Styles.FindContent("flow connection texture.png");
			Styles.selectedConnectionTexture = Styles.FindContent("flow selected connection texture.png");
			Styles.varPinIn = "flow varPin in";
			Styles.varPinOut = "flow varPin out";
			Styles.varPinTooltip = "flow varPin tooltip";
			Styles.targetPinIn = "flow target in";
			Styles.triggerPinIn = "flow triggerPin in";
			Styles.triggerPinOut = "flow triggerPin out";
			Styles.selectionRect = "SelectionRect";
			Styles.nodeTitlebar = "flow node titlebar";
			Styles.nodeAddButton = "Label";
			Styles.m_NodeStyleCache = new Dictionary<string, GUIStyle>();
			Styles.nodeGroupButton = new GUIStyle(EditorStyles.toolbarButton)
			{
				alignment = TextAnchor.MiddleLeft
			};
		}

		private static GUIContent FindContent(string contentName)
		{
			Texture texture = null;
			if (EditorGUIUtility.isProSkin)
			{
				texture = (EditorGUIUtility.Load("Graph/Dark/" + contentName) as Texture);
			}
			if (!texture)
			{
				texture = (EditorGUIUtility.Load("Graph/Light/" + contentName) as Texture);
			}
			if (!texture)
			{
				Debug.LogError("Unable to load " + contentName);
				return new GUIContent(contentName);
			}
			return new GUIContent(texture);
		}

		public static GUIStyle GetNodeStyle(string styleName, Styles.Color color, bool on)
		{
			string text = string.Format("flow {0} {1}{2}", styleName, (int)color, (!on) ? string.Empty : " on");
			if (!Styles.m_NodeStyleCache.ContainsKey(text))
			{
				Styles.m_NodeStyleCache[text] = text;
			}
			return Styles.m_NodeStyleCache[text];
		}
	}
}
