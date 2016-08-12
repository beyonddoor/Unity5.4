﻿namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class TerrainData : UnityEngine.Object
    {
        private static readonly int kMaximumAlphamapResolution = Internal_GetMaximumAlphamapResolution();
        private static readonly int kMaximumBaseMapResolution = Internal_GetMaximumBaseMapResolution();
        private static readonly int kMaximumDetailPatchCount = Internal_GetMaximumDetailPatchCount();
        private static readonly int kMaximumDetailResolutionPerPatch = Internal_GetMaximumDetailResolutionPerPatch();
        private static readonly int kMaximumResolution = Internal_GetMaximumResolution();
        private static readonly int kMinimumAlphamapResolution = Internal_GetMinimumAlphamapResolution();
        private static readonly int kMinimumBaseMapResolution = Internal_GetMinimumBaseMapResolution();
        private static readonly int kMinimumDetailResolutionPerPatch = Internal_GetMinimumDetailResolutionPerPatch();

        public TerrainData()
        {
            this.Internal_Create(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void AddTree(out TreeInstance tree);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void AddUser(GameObject user);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern int GetAdjustedSize(int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float[,,] GetAlphamaps(int x, int y, int width, int height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Texture2D GetAlphamapTexture(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int[,] GetDetailLayer(int xBase, int yBase, int width, int height, int layer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetHeight(int x, int y);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float[,] GetHeights(int xBase, int yBase, int width, int height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetInterpolatedHeight(float x, float y);
        public Vector3 GetInterpolatedNormal(float x, float y)
        {
            Vector3 vector;
            INTERNAL_CALL_GetInterpolatedNormal(this, x, y, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetSteepness(float x, float y);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int[] GetSupportedLayers(int xBase, int yBase, int totalWidth, int totalHeight);
        public TreeInstance GetTreeInstance(int index)
        {
            TreeInstance instance;
            INTERNAL_CALL_GetTreeInstance(this, index, out instance);
            return instance;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool HasUser(GameObject user);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetInterpolatedNormal(TerrainData self, float x, float y, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetTreeInstance(TerrainData self, int index, out TreeInstance value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_RemoveTrees(TerrainData self, ref Vector2 position, float radius, int prototypeIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetTreeInstance(TerrainData self, int index, ref TreeInstance instance);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void Internal_Create([Writable] TerrainData terrainData);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_heightmapScale(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_size(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_wavingGrassTint(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetMaximumAlphamapResolution();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetMaximumBaseMapResolution();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetMaximumDetailPatchCount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetMaximumDetailResolutionPerPatch();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetMaximumResolution();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetMinimumAlphamapResolution();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetMinimumBaseMapResolution();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetMinimumDetailResolutionPerPatch();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_size(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_wavingGrassTint(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetAlphamaps(int x, int y, int width, int height, float[,,] map);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetDetailLayer(int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, int[,] data);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetDetailResolution(int patchCount, int resolutionPerPatch);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetHeights(int xBase, int yBase, int width, int height, float[,] heights);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetHeightsDelayLOD(int xBase, int yBase, int width, int height, float[,] heights);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool NeedUpgradeScaledTreePrototypes();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RecalculateBasemapIfDirty();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RecalculateTreePositions();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RefreshPrototypes();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RemoveDetailPrototype(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RemoveTreePrototype(int index);
        internal int RemoveTrees(Vector2 position, float radius, int prototypeIndex)
        {
            return INTERNAL_CALL_RemoveTrees(this, ref position, radius, prototypeIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RemoveUser(GameObject user);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void ResetDirtyDetails();
        public void SetAlphamaps(int x, int y, float[,,] map)
        {
            if (map.GetLength(2) != this.alphamapLayers)
            {
                object[] args = new object[] { this.alphamapLayers };
                throw new Exception(UnityString.Format("Float array size wrong (layers should be {0})", args));
            }
            this.Internal_SetAlphamaps(x, y, map.GetLength(1), map.GetLength(0), map);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetBasemapDirty(bool dirty);
        public void SetDetailLayer(int xBase, int yBase, int layer, int[,] details)
        {
            this.Internal_SetDetailLayer(xBase, yBase, details.GetLength(1), details.GetLength(0), layer, details);
        }

        public void SetDetailResolution(int detailResolution, int resolutionPerPatch)
        {
            if (detailResolution < 0)
            {
                Debug.LogWarning("detailResolution must not be negative.");
                detailResolution = 0;
            }
            if ((resolutionPerPatch < kMinimumDetailResolutionPerPatch) || (resolutionPerPatch > kMaximumDetailResolutionPerPatch))
            {
                Debug.LogWarning(string.Concat(new object[] { "resolutionPerPatch is clamped to the range of [", kMinimumDetailResolutionPerPatch, ", ", kMaximumDetailResolutionPerPatch, "]." }));
                resolutionPerPatch = Math.Min(kMaximumDetailResolutionPerPatch, Math.Max(resolutionPerPatch, kMinimumDetailResolutionPerPatch));
            }
            int num = detailResolution / resolutionPerPatch;
            if (num > kMaximumDetailPatchCount)
            {
                Debug.LogWarning("Patch count (detailResolution / resolutionPerPatch) is clamped to the range of [0, " + kMaximumDetailPatchCount + "].");
                num = Math.Min(kMaximumDetailPatchCount, Math.Max(num, 0));
            }
            this.Internal_SetDetailResolution(num, resolutionPerPatch);
        }

        public void SetHeights(int xBase, int yBase, float[,] heights)
        {
            if (heights == null)
            {
                throw new NullReferenceException();
            }
            if (((((xBase + heights.GetLength(1)) > this.heightmapWidth) || ((xBase + heights.GetLength(1)) < 0)) || (((yBase + heights.GetLength(0)) < 0) || (xBase < 0))) || ((yBase < 0) || ((yBase + heights.GetLength(0)) > this.heightmapHeight)))
            {
                object[] args = new object[] { xBase + heights.GetLength(1), yBase + heights.GetLength(0), this.heightmapWidth, this.heightmapHeight };
                throw new ArgumentException(UnityString.Format("X or Y base out of bounds. Setting up to {0}x{1} while map size is {2}x{3}", args));
            }
            this.Internal_SetHeights(xBase, yBase, heights.GetLength(1), heights.GetLength(0), heights);
        }

        public void SetHeightsDelayLOD(int xBase, int yBase, float[,] heights)
        {
            if (heights == null)
            {
                throw new ArgumentNullException("heights");
            }
            int length = heights.GetLength(0);
            int width = heights.GetLength(1);
            if (((xBase < 0) || ((xBase + width) < 0)) || ((xBase + width) > this.heightmapWidth))
            {
                object[] args = new object[] { xBase, xBase + width, this.heightmapWidth };
                throw new ArgumentException(UnityString.Format("X out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", args));
            }
            if (((yBase < 0) || ((yBase + length) < 0)) || ((yBase + length) > this.heightmapHeight))
            {
                object[] objArray2 = new object[] { yBase, yBase + length, this.heightmapHeight };
                throw new ArgumentException(UnityString.Format("Y out of bounds - trying to set {0}-{1} but the terrain ranges from 0-{2}", objArray2));
            }
            this.Internal_SetHeightsDelayLOD(xBase, yBase, width, length, heights);
        }

        public void SetTreeInstance(int index, TreeInstance instance)
        {
            INTERNAL_CALL_SetTreeInstance(this, index, ref instance);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void UpgradeScaledTreePrototype();

        public int alphamapHeight
        {
            get
            {
                return this.alphamapResolution;
            }
        }

        public int alphamapLayers { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int alphamapResolution
        {
            get
            {
                return this.Internal_alphamapResolution;
            }
            set
            {
                int num = value;
                if ((value < kMinimumAlphamapResolution) || (value > kMaximumAlphamapResolution))
                {
                    Debug.LogWarning(string.Concat(new object[] { "alphamapResolution is clamped to the range of [", kMinimumAlphamapResolution, ", ", kMaximumAlphamapResolution, "]." }));
                    num = Math.Min(kMaximumAlphamapResolution, Math.Max(value, kMinimumAlphamapResolution));
                }
                this.Internal_alphamapResolution = num;
            }
        }

        private int alphamapTextureCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Texture2D[] alphamapTextures
        {
            get
            {
                Texture2D[] texturedArray = new Texture2D[this.alphamapTextureCount];
                for (int i = 0; i < texturedArray.Length; i++)
                {
                    texturedArray[i] = this.GetAlphamapTexture(i);
                }
                return texturedArray;
            }
        }

        public int alphamapWidth
        {
            get
            {
                return this.alphamapResolution;
            }
        }

        public int baseMapResolution
        {
            get
            {
                return this.Internal_baseMapResolution;
            }
            set
            {
                int num = value;
                if ((value < kMinimumBaseMapResolution) || (value > kMaximumBaseMapResolution))
                {
                    Debug.LogWarning(string.Concat(new object[] { "baseMapResolution is clamped to the range of [", kMinimumBaseMapResolution, ", ", kMaximumBaseMapResolution, "]." }));
                    num = Math.Min(kMaximumBaseMapResolution, Math.Max(value, kMinimumBaseMapResolution));
                }
                this.Internal_baseMapResolution = num;
            }
        }

        public int detailHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public DetailPrototype[] detailPrototypes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int detailResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal int detailResolutionPerPatch { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int detailWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int heightmapHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int heightmapResolution
        {
            get
            {
                return this.Internal_heightmapResolution;
            }
            set
            {
                int num = value;
                if ((value < 0) || (value > kMaximumResolution))
                {
                    Debug.LogWarning("heightmapResolution is clamped to the range of [0, " + kMaximumResolution + "].");
                    num = Math.Min(kMaximumResolution, Math.Max(value, 0));
                }
                this.Internal_heightmapResolution = num;
            }
        }

        public Vector3 heightmapScale
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_heightmapScale(out vector);
                return vector;
            }
        }

        public int heightmapWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        private int Internal_alphamapResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        private int Internal_baseMapResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        private int Internal_heightmapResolution { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 size
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_size(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_size(ref value);
            }
        }

        public SplatPrototype[] splatPrototypes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float thickness { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int treeInstanceCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public TreeInstance[] treeInstances { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TreePrototype[] treePrototypes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float wavingGrassAmount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float wavingGrassSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float wavingGrassStrength { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Color wavingGrassTint
        {
            get
            {
                Color color;
                this.INTERNAL_get_wavingGrassTint(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_wavingGrassTint(ref value);
            }
        }
    }
}

