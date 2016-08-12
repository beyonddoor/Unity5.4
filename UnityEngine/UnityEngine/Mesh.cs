namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class Mesh : UnityEngine.Object
    {
        public Mesh()
        {
            Internal_Create(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);
        private bool CheckCanAccessSubmesh(int submesh, bool errorAboutTriangles)
        {
            if (!this.canAccess)
            {
                this.PrintErrorCantAccessMeshForIndices();
                return false;
            }
            if ((submesh >= 0) && (submesh < this.subMeshCount))
            {
                return true;
            }
            if (errorAboutTriangles)
            {
                this.PrintErrorBadSubmeshIndexTriangles();
            }
            else
            {
                this.PrintErrorBadSubmeshIndexIndices();
            }
            return false;
        }

        private bool CheckCanAccessSubmeshIndices(int submesh)
        {
            return this.CheckCanAccessSubmesh(submesh, false);
        }

        private bool CheckCanAccessSubmeshTriangles(int submesh)
        {
            return this.CheckCanAccessSubmesh(submesh, true);
        }

        [ExcludeFromDocs]
        public void Clear()
        {
            bool keepVertexLayout = true;
            this.Clear(keepVertexLayout);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Clear([UnityEngine.Internal.DefaultValue("true")] bool keepVertexLayout);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearBlendShapes();
        [ExcludeFromDocs]
        public void CombineMeshes(CombineInstance[] combine)
        {
            bool useMatrices = true;
            bool mergeSubMeshes = true;
            this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
        }

        [ExcludeFromDocs]
        public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes)
        {
            bool useMatrices = true;
            this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CombineMeshes(CombineInstance[] combine, [UnityEngine.Internal.DefaultValue("true")] bool mergeSubMeshes, [UnityEngine.Internal.DefaultValue("true")] bool useMatrices);
        internal static int DefaultDimensionForChannel(InternalShaderChannel channel)
        {
            if ((channel == InternalShaderChannel.Vertex) || (channel == InternalShaderChannel.Normal))
            {
                return 3;
            }
            if ((channel >= InternalShaderChannel.TexCoord0) && (channel <= InternalShaderChannel.TexCoord3))
            {
                return 2;
            }
            if ((channel != InternalShaderChannel.Tangent) && (channel != InternalShaderChannel.Color))
            {
                throw new ArgumentException("DefaultDimensionForChannel called for bad channel", "channel");
            }
            return 4;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Array ExtractArrayFromList(object list);
        private T[] GetAllocArrayFromChannel<T>(InternalShaderChannel channel)
        {
            return this.GetAllocArrayFromChannel<T>(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel));
        }

        private T[] GetAllocArrayFromChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim)
        {
            if (this.canAccess)
            {
                if (this.HasChannel(channel))
                {
                    return (T[]) this.GetAllocArrayFromChannelImpl(channel, format, dim);
                }
            }
            else
            {
                this.PrintErrorCantAccessMesh(channel);
            }
            return new T[0];
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Array GetAllocArrayFromChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetArrayFromChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetBlendShapeFrameCount(int shapeIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetBlendShapeIndex(string blendShapeName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetBlendShapeName(int shapeIndex);
        public int[] GetIndices(int submesh)
        {
            return (!this.CheckCanAccessSubmeshIndices(submesh) ? new int[0] : this.GetIndicesImpl(submesh));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern int[] GetIndicesImpl(int submesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern MeshTopology GetTopology(int submesh);
        public int[] GetTriangles(int submesh)
        {
            return (!this.CheckCanAccessSubmeshTriangles(submesh) ? new int[0] : this.GetTrianglesImpl(submesh));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern int[] GetTrianglesImpl(int submesh);
        internal InternalShaderChannel GetUVChannel(int uvIndex)
        {
            if ((uvIndex < 0) || (uvIndex > 3))
            {
                throw new ArgumentException("GetUVChannel called for bad uvIndex", "uvIndex");
            }
            return (InternalShaderChannel) (3 + uvIndex);
        }

        public void GetUVs(int channel, List<Vector2> uvs)
        {
            this.GetUVsImpl<Vector2>(channel, uvs, 2);
        }

        public void GetUVs(int channel, List<Vector3> uvs)
        {
            this.GetUVsImpl<Vector3>(channel, uvs, 3);
        }

        public void GetUVs(int channel, List<Vector4> uvs)
        {
            this.GetUVsImpl<Vector4>(channel, uvs, 4);
        }

        private void GetUVsImpl<T>(int uvIndex, List<T> uvs, int dim)
        {
            if (uvs == null)
            {
                throw new ArgumentException("The result uvs list cannot be null", "uvs");
            }
            if ((uvIndex < 0) || (uvIndex > 3))
            {
                throw new ArgumentException("The uv index is invalid (must be in [0..3]", "uvIndex");
            }
            uvs.Clear();
            InternalShaderChannel uVChannel = this.GetUVChannel(uvIndex);
            if (!this.canAccess)
            {
                this.PrintErrorCantAccessMesh(uVChannel);
            }
            else if (this.HasChannel(uVChannel))
            {
                if (this.vertexCount > uvs.Capacity)
                {
                    uvs.Capacity = this.vertexCount;
                }
                this.GetUVsInternal<T>(uvs, uvIndex, dim);
            }
        }

        private void GetUVsInternal<T>(List<T> uvs, int uvIndex, int dim)
        {
            InternalShaderChannel uVChannel = this.GetUVChannel(uvIndex);
            ResizeList(uvs, this.vertexCount);
            this.GetArrayFromChannelImpl(uVChannel, InternalVertexChannelType.Float, dim, ExtractArrayFromList(uvs));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool HasChannel(InternalShaderChannel channel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create([Writable] Mesh mono);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_bounds(ref Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void MarkDynamic();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Optimize();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void PrintErrorBadSubmeshIndexIndices();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void PrintErrorBadSubmeshIndexTriangles();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void PrintErrorCantAccessMesh(InternalShaderChannel channel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void PrintErrorCantAccessMeshForIndices();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RecalculateBounds();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RecalculateNormals();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void ResizeList(object list, int size);
        private int SafeLength(Array values)
        {
            return ((values == null) ? 0 : values.Length);
        }

        private int SafeLength<T>(List<T> values)
        {
            return ((values == null) ? 0 : values.Count);
        }

        private void SetArrayForChannel<T>(InternalShaderChannel channel, T[] values)
        {
            this.SetSizedArrayForChannel(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel), values, this.SafeLength(values));
        }

        private void SetArrayForChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim, T[] values)
        {
            this.SetSizedArrayForChannel(channel, format, dim, values, this.SafeLength(values));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetArrayForChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values, int arraySize);
        public void SetColors(List<Color> inColors)
        {
            this.SetListForChannel<Color>(InternalShaderChannel.Color, inColors);
        }

        public void SetColors(List<Color32> inColors)
        {
            this.SetListForChannel<Color32>(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1, inColors);
        }

        [ExcludeFromDocs]
        public void SetIndices(int[] indices, MeshTopology topology, int submesh)
        {
            bool calculateBounds = true;
            this.SetIndices(indices, topology, submesh, calculateBounds);
        }

        public void SetIndices(int[] indices, MeshTopology topology, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds)
        {
            if (this.CheckCanAccessSubmeshIndices(submesh))
            {
                this.SetIndicesImpl(submesh, topology, indices, this.SafeLength(indices), calculateBounds);
            }
        }

        [ExcludeFromDocs]
        private void SetIndicesImpl(int submesh, MeshTopology topology, Array indices, int arraySize)
        {
            bool calculateBounds = true;
            this.SetIndicesImpl(submesh, topology, indices, arraySize, calculateBounds);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetIndicesImpl(int submesh, MeshTopology topology, Array indices, int arraySize, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds);
        private void SetListForChannel<T>(InternalShaderChannel channel, List<T> values)
        {
            this.SetSizedArrayForChannel(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel), ExtractArrayFromList(values), this.SafeLength<T>(values));
        }

        private void SetListForChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim, List<T> values)
        {
            this.SetSizedArrayForChannel(channel, format, dim, ExtractArrayFromList(values), this.SafeLength<T>(values));
        }

        public void SetNormals(List<Vector3> inNormals)
        {
            this.SetListForChannel<Vector3>(InternalShaderChannel.Normal, inNormals);
        }

        private void SetSizedArrayForChannel(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values, int valuesCount)
        {
            if (this.canAccess)
            {
                this.SetArrayForChannelImpl(channel, format, dim, values, valuesCount);
            }
            else
            {
                this.PrintErrorCantAccessMesh(channel);
            }
        }

        public void SetTangents(List<Vector4> inTangents)
        {
            this.SetListForChannel<Vector4>(InternalShaderChannel.Tangent, inTangents);
        }

        [ExcludeFromDocs]
        public void SetTriangles(int[] triangles, int submesh)
        {
            bool calculateBounds = true;
            this.SetTriangles(triangles, submesh, calculateBounds);
        }

        [ExcludeFromDocs]
        public void SetTriangles(List<int> triangles, int submesh)
        {
            bool calculateBounds = true;
            this.SetTriangles(triangles, submesh, calculateBounds);
        }

        public void SetTriangles(int[] triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds)
        {
            if (this.CheckCanAccessSubmeshTriangles(submesh))
            {
                this.SetTrianglesImpl(submesh, triangles, this.SafeLength(triangles), calculateBounds);
            }
        }

        public void SetTriangles(List<int> triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds)
        {
            if (this.CheckCanAccessSubmeshTriangles(submesh))
            {
                this.SetTrianglesImpl(submesh, ExtractArrayFromList(triangles), this.SafeLength<int>(triangles), calculateBounds);
            }
        }

        [ExcludeFromDocs]
        private void SetTrianglesImpl(int submesh, Array triangles, int arraySize)
        {
            bool calculateBounds = true;
            this.SetTrianglesImpl(submesh, triangles, arraySize, calculateBounds);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetTrianglesImpl(int submesh, Array triangles, int arraySize, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds);
        public void SetUVs(int channel, List<Vector2> uvs)
        {
            this.SetUvsImpl<Vector2>(channel, 2, uvs);
        }

        public void SetUVs(int channel, List<Vector3> uvs)
        {
            this.SetUvsImpl<Vector3>(channel, 3, uvs);
        }

        public void SetUVs(int channel, List<Vector4> uvs)
        {
            this.SetUvsImpl<Vector4>(channel, 4, uvs);
        }

        private void SetUvsImpl<T>(int uvIndex, int dim, List<T> uvs)
        {
            if ((uvIndex < 0) || (uvIndex > 3))
            {
                Debug.LogError("The uv index is invalid (must be in [0..3]");
            }
            else
            {
                this.SetListForChannel<T>(this.GetUVChannel(uvIndex), InternalVertexChannelType.Float, dim, uvs);
            }
        }

        public void SetVertices(List<Vector3> inVertices)
        {
            this.SetListForChannel<Vector3>(InternalShaderChannel.Vertex, inVertices);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void UploadMeshData(bool markNoLogerReadable);

        public Matrix4x4[] bindposes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int blendShapeCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public BoneWeight[] boneWeights { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_bounds(ref value);
            }
        }

        internal bool canAccess { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Color[] colors
        {
            get
            {
                return this.GetAllocArrayFromChannel<Color>(InternalShaderChannel.Color);
            }
            set
            {
                this.SetArrayForChannel<Color>(InternalShaderChannel.Color, value);
            }
        }

        public Color32[] colors32
        {
            get
            {
                return this.GetAllocArrayFromChannel<Color32>(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1);
            }
            set
            {
                this.SetArrayForChannel<Color32>(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1, value);
            }
        }

        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3[] normals
        {
            get
            {
                return this.GetAllocArrayFromChannel<Vector3>(InternalShaderChannel.Normal);
            }
            set
            {
                this.SetArrayForChannel<Vector3>(InternalShaderChannel.Normal, value);
            }
        }

        public int subMeshCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector4[] tangents
        {
            get
            {
                return this.GetAllocArrayFromChannel<Vector4>(InternalShaderChannel.Tangent);
            }
            set
            {
                this.SetArrayForChannel<Vector4>(InternalShaderChannel.Tangent, value);
            }
        }

        public int[] triangles
        {
            get
            {
                if (this.canAccess)
                {
                    return this.GetTrianglesImpl(-1);
                }
                this.PrintErrorCantAccessMeshForIndices();
                return new int[0];
            }
            set
            {
                if (this.canAccess)
                {
                    this.SetTrianglesImpl(-1, value, this.SafeLength(value));
                }
                else
                {
                    this.PrintErrorCantAccessMeshForIndices();
                }
            }
        }

        public Vector2[] uv
        {
            get
            {
                return this.GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord0);
            }
            set
            {
                this.SetArrayForChannel<Vector2>(InternalShaderChannel.TexCoord0, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property Mesh.uv1 has been deprecated. Use Mesh.uv2 instead (UnityUpgradable) -> uv2", true)]
        public Vector2[] uv1
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public Vector2[] uv2
        {
            get
            {
                return this.GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord1);
            }
            set
            {
                this.SetArrayForChannel<Vector2>(InternalShaderChannel.TexCoord1, value);
            }
        }

        public Vector2[] uv3
        {
            get
            {
                return this.GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord2);
            }
            set
            {
                this.SetArrayForChannel<Vector2>(InternalShaderChannel.TexCoord2, value);
            }
        }

        public Vector2[] uv4
        {
            get
            {
                return this.GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord3);
            }
            set
            {
                this.SetArrayForChannel<Vector2>(InternalShaderChannel.TexCoord3, value);
            }
        }

        public int vertexCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3[] vertices
        {
            get
            {
                return this.GetAllocArrayFromChannel<Vector3>(InternalShaderChannel.Vertex);
            }
            set
            {
                this.SetArrayForChannel<Vector3>(InternalShaderChannel.Vertex, value);
            }
        }

        internal enum InternalShaderChannel
        {
            Vertex,
            Normal,
            Color,
            TexCoord0,
            TexCoord1,
            TexCoord2,
            TexCoord3,
            Tangent
        }

        internal enum InternalVertexChannelType
        {
            Color = 2,
            Float = 0
        }
    }
}

