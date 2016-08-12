namespace UnityEngine.WSA
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Tile
    {
        private string m_TileId;
        private static Tile s_MainTile;

        private Tile(string tileId)
        {
            this.m_TileId = tileId;
        }

        public static Tile CreateOrUpdateSecondary(SecondaryTileData data)
        {
            string[] sargs = MakeSecondaryTileSargs(data);
            bool[] bargs = MakeSecondaryTileBargs(data);
            Color32 backgroundColor = data.backgroundColor;
            string str = CreateOrUpdateSecondaryTile(sargs, bargs, ref backgroundColor, (int) data.foregroundText);
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return new Tile(str);
        }

        public static Tile CreateOrUpdateSecondary(SecondaryTileData data, Rect area)
        {
            string[] sargs = MakeSecondaryTileSargs(data);
            bool[] bargs = MakeSecondaryTileBargs(data);
            Color32 backgroundColor = data.backgroundColor;
            string str = CreateOrUpdateSecondaryTileArea(sargs, bargs, ref backgroundColor, (int) data.foregroundText, area);
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return new Tile(str);
        }

        public static Tile CreateOrUpdateSecondary(SecondaryTileData data, Vector2 pos)
        {
            string[] sargs = MakeSecondaryTileSargs(data);
            bool[] bargs = MakeSecondaryTileBargs(data);
            Color32 backgroundColor = data.backgroundColor;
            string str = CreateOrUpdateSecondaryTilePoint(sargs, bargs, ref backgroundColor, (int) data.foregroundText, pos);
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return new Tile(str);
        }

        [ThreadAndSerializationSafe]
        private static string CreateOrUpdateSecondaryTile(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText)
        {
            return INTERNAL_CALL_CreateOrUpdateSecondaryTile(sargs, bargs, ref backgroundColor, foregroundText);
        }

        [ThreadAndSerializationSafe]
        private static string CreateOrUpdateSecondaryTileArea(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText, Rect area)
        {
            return INTERNAL_CALL_CreateOrUpdateSecondaryTileArea(sargs, bargs, ref backgroundColor, foregroundText, ref area);
        }

        [ThreadAndSerializationSafe]
        private static string CreateOrUpdateSecondaryTilePoint(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText, Vector2 pos)
        {
            return INTERNAL_CALL_CreateOrUpdateSecondaryTilePoint(sargs, bargs, ref backgroundColor, foregroundText, ref pos);
        }

        public void Delete()
        {
            DeleteSecondary(this.m_TileId);
        }

        public void Delete(Rect area)
        {
            DeleteSecondaryArea(this.m_TileId, area);
        }

        public void Delete(Vector2 pos)
        {
            DeleteSecondaryPos(this.m_TileId, pos);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        public static extern void DeleteSecondary(string tileId);
        public static void DeleteSecondary(string tileId, Rect area)
        {
            DeleteSecondary(tileId, area);
        }

        public static void DeleteSecondary(string tileId, Vector2 pos)
        {
            DeleteSecondaryPos(tileId, pos);
        }

        [ThreadAndSerializationSafe]
        private static void DeleteSecondaryArea(string tileId, Rect area)
        {
            INTERNAL_CALL_DeleteSecondaryArea(tileId, ref area);
        }

        [ThreadAndSerializationSafe]
        private static void DeleteSecondaryPos(string tileId, Vector2 pos)
        {
            INTERNAL_CALL_DeleteSecondaryPos(tileId, ref pos);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        public static extern bool Exists(string tileId);
        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        private static extern string[] GetAllSecondaryTiles();
        public static Tile[] GetSecondaries()
        {
            string[] allSecondaryTiles = GetAllSecondaryTiles();
            Tile[] tileArray = new Tile[allSecondaryTiles.Length];
            for (int i = 0; i < allSecondaryTiles.Length; i++)
            {
                tileArray[i] = new Tile(allSecondaryTiles[i]);
            }
            return tileArray;
        }

        public static Tile GetSecondary(string tileId)
        {
            if (Exists(tileId))
            {
                return new Tile(tileId);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        public static extern string GetTemplate(TileTemplate templ);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern bool HasUserConsent(string tileId);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string INTERNAL_CALL_CreateOrUpdateSecondaryTile(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string INTERNAL_CALL_CreateOrUpdateSecondaryTileArea(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText, ref Rect area);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string INTERNAL_CALL_CreateOrUpdateSecondaryTilePoint(string[] sargs, bool[] bargs, ref Color32 backgroundColor, int foregroundText, ref Vector2 pos);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DeleteSecondaryArea(string tileId, ref Rect area);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DeleteSecondaryPos(string tileId, ref Vector2 pos);
        private static bool[] MakeSecondaryTileBargs(SecondaryTileData data)
        {
            return new bool[] { data.backgroundColorSet, data.lockScreenDisplayBadgeAndTileText, data.roamingEnabled, data.showNameOnSquare150x150Logo, data.showNameOnSquare310x310Logo, data.showNameOnWide310x150Logo };
        }

        private static string[] MakeSecondaryTileSargs(SecondaryTileData data)
        {
            return new string[] { data.arguments, data.displayName, data.lockScreenBadgeLogo, data.phoneticName, data.square150x150Logo, data.square30x30Logo, data.square310x310Logo, data.square70x70Logo, data.tileId, data.wide310x150Logo };
        }

        public void PeriodicBadgeUpdate(string uri, float interval)
        {
            PeriodicBadgeUpdate(this.m_TileId, uri, interval);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern void PeriodicBadgeUpdate(string tileId, string uri, float interval);
        public void PeriodicUpdate(string uri, float interval)
        {
            PeriodicUpdate(this.m_TileId, uri, interval);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern void PeriodicUpdate(string tileId, string uri, float interval);
        public void RemoveBadge()
        {
            RemoveBadge(this.m_TileId);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern void RemoveBadge(string tileId);
        public void StopPeriodicBadgeUpdate()
        {
            StopPeriodicBadgeUpdate(this.m_TileId);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ThreadAndSerializationSafe, WrapperlessIcall]
        private static extern void StopPeriodicBadgeUpdate(string tileId);
        public void StopPeriodicUpdate()
        {
            StopPeriodicUpdate(this.m_TileId);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern void StopPeriodicUpdate(string tileId);
        public void Update(string xml)
        {
            Update(this.m_TileId, xml);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern void Update(string tileId, string xml);
        public void Update(string medium, string wide, string large, string text)
        {
            UpdateImageAndText(this.m_TileId, medium, wide, large, text);
        }

        public void UpdateBadgeImage(string image)
        {
            UpdateBadgeImage(this.m_TileId, image);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern void UpdateBadgeImage(string tileId, string image);
        public void UpdateBadgeNumber(float number)
        {
            UpdateBadgeNumber(this.m_TileId, number);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern void UpdateBadgeNumber(string tileId, float number);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, ThreadAndSerializationSafe]
        private static extern void UpdateImageAndText(string tileId, string medium, string wide, string large, string text);

        public bool exists
        {
            get
            {
                return Exists(this.m_TileId);
            }
        }

        public bool hasUserConsent
        {
            get
            {
                return HasUserConsent(this.m_TileId);
            }
        }

        public string id
        {
            get
            {
                return this.m_TileId;
            }
        }

        public static Tile main
        {
            get
            {
                if (s_MainTile == null)
                {
                    s_MainTile = new Tile(string.Empty);
                }
                return s_MainTile;
            }
        }
    }
}

