namespace UnityEngine.SceneManagement
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct Scene
    {
        private int m_Handle;
        internal int handle
        {
            get
            {
                return this.m_Handle;
            }
        }
        public bool IsValid()
        {
            return IsValidInternal(this.handle);
        }

        public string path
        {
            get
            {
                return GetPathInternal(this.handle);
            }
        }
        public string name
        {
            get
            {
                return GetNameInternal(this.handle);
            }
            internal set
            {
                SetNameInternal(this.handle, value);
            }
        }
        public bool isLoaded
        {
            get
            {
                return GetIsLoadedInternal(this.handle);
            }
        }
        public int buildIndex
        {
            get
            {
                return GetBuildIndexInternal(this.handle);
            }
        }
        public bool isDirty
        {
            get
            {
                return GetIsDirtyInternal(this.handle);
            }
        }
        public int rootCount
        {
            get
            {
                return GetRootCountInternal(this.handle);
            }
        }
        public GameObject[] GetRootGameObjects()
        {
            List<GameObject> rootGameObjects = new List<GameObject>(this.rootCount);
            this.GetRootGameObjects(rootGameObjects);
            return rootGameObjects.ToArray();
        }

        public void GetRootGameObjects(List<GameObject> rootGameObjects)
        {
            if (rootGameObjects.Capacity < this.rootCount)
            {
                rootGameObjects.Capacity = this.rootCount;
            }
            rootGameObjects.Clear();
            if (!this.IsValid())
            {
                throw new ArgumentException("The scene is invalid.");
            }
            if (!this.isLoaded)
            {
                throw new ArgumentException("The scene is not loaded.");
            }
            if (this.rootCount != 0)
            {
                GetRootGameObjectsInternal(this.handle, rootGameObjects);
            }
        }

        public override int GetHashCode()
        {
            return this.m_Handle;
        }

        public override bool Equals(object other)
        {
            if (!(other is Scene))
            {
                return false;
            }
            Scene scene = (Scene) other;
            return (this.handle == scene.handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool IsValidInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetPathInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetNameInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetNameInternal(int sceneHandle, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetIsLoadedInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetIsDirtyInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetBuildIndexInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetRootCountInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void GetRootGameObjectsInternal(int sceneHandle, object resultRootList);
        public static bool operator ==(Scene lhs, Scene rhs)
        {
            return (lhs.handle == rhs.handle);
        }

        public static bool operator !=(Scene lhs, Scene rhs)
        {
            return (lhs.handle != rhs.handle);
        }
    }
}

