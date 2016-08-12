﻿namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct Resolution
    {
        private int m_Width;
        private int m_Height;
        private int m_RefreshRate;
        public int width
        {
            get
            {
                return this.m_Width;
            }
            set
            {
                this.m_Width = value;
            }
        }
        public int height
        {
            get
            {
                return this.m_Height;
            }
            set
            {
                this.m_Height = value;
            }
        }
        public int refreshRate
        {
            get
            {
                return this.m_RefreshRate;
            }
            set
            {
                this.m_RefreshRate = value;
            }
        }
        public override string ToString()
        {
            object[] args = new object[] { this.m_Width, this.m_Height, this.m_RefreshRate };
            return UnityString.Format("{0} x {1} @ {2}Hz", args);
        }
    }
}

