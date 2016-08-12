using System;

namespace UnityEditor
{
	internal class SavedBool
	{
		private bool m_Value;

		private string m_Name;

		private bool m_Loaded;

		public bool value
		{
			get
			{
				this.Load();
				return this.m_Value;
			}
			set
			{
				this.Load();
				if (this.m_Value == value)
				{
					return;
				}
				this.m_Value = value;
				EditorPrefs.SetBool(this.m_Name, value);
			}
		}

		public SavedBool(string name, bool value)
		{
			this.m_Name = name;
			this.m_Loaded = false;
		}

		private void Load()
		{
			if (this.m_Loaded)
			{
				return;
			}
			this.m_Loaded = true;
			this.m_Value = EditorPrefs.GetBool(this.m_Name, this.value);
		}

		public static implicit operator bool(SavedBool s)
		{
			return s.value;
		}
	}
}
