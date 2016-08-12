using System;
using System.Runtime.InteropServices;

namespace UnityEditor.Collaboration
{
	[StructLayout(LayoutKind.Sequential)]
	internal class ProgressInfo
	{
		public enum ProgressType : uint
		{
			None,
			Count,
			Percent,
			Both
		}

		private string m_Title;

		private string m_ExtraInfo;

		private ProgressInfo.ProgressType m_ProgressType;

		private int m_Percentage;

		private int m_CurrentCount;

		private int m_TotalCount;

		private int m_Completed;

		private int m_Cancelled;

		private int m_CanCancel;

		private string m_LastErrorString;

		private ulong m_LastError;

		public string title
		{
			get
			{
				return this.m_Title;
			}
		}

		public string extraInfo
		{
			get
			{
				return this.m_ExtraInfo;
			}
		}

		public int currentCount
		{
			get
			{
				return this.m_CurrentCount;
			}
		}

		public int totalCount
		{
			get
			{
				return this.m_TotalCount;
			}
		}

		public bool completed
		{
			get
			{
				return this.m_Completed != 0;
			}
		}

		public bool cancelled
		{
			get
			{
				return this.m_Cancelled != 0;
			}
		}

		public bool canCancel
		{
			get
			{
				return this.m_CanCancel != 0;
			}
		}

		public string lastErrorString
		{
			get
			{
				return this.m_LastErrorString;
			}
		}

		public ulong lastError
		{
			get
			{
				return this.m_LastError;
			}
		}

		public int percentComplete
		{
			get
			{
				if (this.m_ProgressType == ProgressInfo.ProgressType.Percent || this.m_ProgressType == ProgressInfo.ProgressType.Both)
				{
					return this.m_Percentage;
				}
				if (this.m_ProgressType != ProgressInfo.ProgressType.Count)
				{
					return 0;
				}
				if (this.m_TotalCount == 0)
				{
					return 0;
				}
				return this.m_CurrentCount * 100 / this.m_TotalCount;
			}
		}

		public bool isProgressTypeCount
		{
			get
			{
				return this.m_ProgressType == ProgressInfo.ProgressType.Count || this.m_ProgressType == ProgressInfo.ProgressType.Both;
			}
		}

		public bool isProgressTypePercent
		{
			get
			{
				return this.m_ProgressType == ProgressInfo.ProgressType.Percent || this.m_ProgressType == ProgressInfo.ProgressType.Both;
			}
		}

		public bool errorOccured
		{
			get
			{
				return this.m_LastError != 0uL;
			}
		}

		private ProgressInfo()
		{
		}
	}
}
