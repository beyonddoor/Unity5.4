namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DrivenRectTransformTracker
    {
        private List<RectTransform> m_Tracked;
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool CanRecordModifications();
        public void Add(UnityEngine.Object driver, RectTransform rectTransform, DrivenTransformProperties drivenProperties)
        {
            if (this.m_Tracked == null)
            {
                this.m_Tracked = new List<RectTransform>();
            }
            rectTransform.drivenByObject = driver;
            rectTransform.drivenProperties |= drivenProperties;
            if (!Application.isPlaying && CanRecordModifications())
            {
                RuntimeUndo.RecordObject(rectTransform, "Driving RectTransform");
            }
            this.m_Tracked.Add(rectTransform);
        }

        public void Clear()
        {
            if (this.m_Tracked != null)
            {
                for (int i = 0; i < this.m_Tracked.Count; i++)
                {
                    if (this.m_Tracked[i] != null)
                    {
                        if (!Application.isPlaying && CanRecordModifications())
                        {
                            RuntimeUndo.RecordObject(this.m_Tracked[i], "Driving RectTransform");
                        }
                        this.m_Tracked[i].drivenByObject = null;
                    }
                }
                this.m_Tracked.Clear();
            }
        }
    }
}

