namespace UnityEngine.SceneManagement
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public class SceneManager
    {
        public static  event UnityAction<Scene, Scene> activeSceneChanged;

        public static  event UnityAction<Scene, LoadSceneMode> sceneLoaded;

        public static  event UnityAction<Scene> sceneUnloaded;

        public static Scene CreateScene(string sceneName)
        {
            Scene scene;
            INTERNAL_CALL_CreateScene(sceneName, out scene);
            return scene;
        }

        public static Scene GetActiveScene()
        {
            Scene scene;
            INTERNAL_CALL_GetActiveScene(out scene);
            return scene;
        }

        [Obsolete("Use SceneManager.sceneCount and SceneManager.GetSceneAt(int index) to loop the all scenes instead.")]
        public static Scene[] GetAllScenes()
        {
            Scene[] sceneArray = new Scene[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                sceneArray[i] = GetSceneAt(i);
            }
            return sceneArray;
        }

        public static Scene GetSceneAt(int index)
        {
            Scene scene;
            INTERNAL_CALL_GetSceneAt(index, out scene);
            return scene;
        }

        public static Scene GetSceneByName(string name)
        {
            Scene scene;
            INTERNAL_CALL_GetSceneByName(name, out scene);
            return scene;
        }

        public static Scene GetSceneByPath(string scenePath)
        {
            Scene scene;
            INTERNAL_CALL_GetSceneByPath(scenePath, out scene);
            return scene;
        }

        [RequiredByNativeCode]
        private static void Internal_ActiveSceneChanged(Scene previousActiveScene, Scene newActiveScene)
        {
            if (activeSceneChanged != null)
            {
                activeSceneChanged(previousActiveScene, newActiveScene);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_CreateScene(string sceneName, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetActiveScene(out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetSceneAt(int index, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetSceneByName(string name, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetSceneByPath(string scenePath, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MergeScenes(ref Scene sourceScene, ref Scene destinationScene);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MoveGameObjectToScene(GameObject go, ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_SetActiveScene(ref Scene scene);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_UnloadScene(ref Scene scene);
        [RequiredByNativeCode]
        private static void Internal_SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (sceneLoaded != null)
            {
                sceneLoaded(scene, mode);
            }
        }

        [RequiredByNativeCode]
        private static void Internal_SceneUnloaded(Scene scene)
        {
            if (sceneUnloaded != null)
            {
                sceneUnloaded(scene);
            }
        }

        [ExcludeFromDocs]
        public static void LoadScene(int sceneBuildIndex)
        {
            LoadSceneMode single = LoadSceneMode.Single;
            LoadScene(sceneBuildIndex, single);
        }

        [ExcludeFromDocs]
        public static void LoadScene(string sceneName)
        {
            LoadSceneMode single = LoadSceneMode.Single;
            LoadScene(sceneName, single);
        }

        public static void LoadScene(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
        {
            LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, true);
        }

        public static void LoadScene(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
        {
            LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, true);
        }

        [ExcludeFromDocs]
        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex)
        {
            LoadSceneMode single = LoadSceneMode.Single;
            return LoadSceneAsync(sceneBuildIndex, single);
        }

        [ExcludeFromDocs]
        public static AsyncOperation LoadSceneAsync(string sceneName)
        {
            LoadSceneMode single = LoadSceneMode.Single;
            return LoadSceneAsync(sceneName, single);
        }

        public static AsyncOperation LoadSceneAsync(int sceneBuildIndex, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
        {
            return LoadSceneAsyncNameIndexInternal(null, sceneBuildIndex, mode == LoadSceneMode.Additive, false);
        }

        public static AsyncOperation LoadSceneAsync(string sceneName, [DefaultValue("LoadSceneMode.Single")] LoadSceneMode mode)
        {
            return LoadSceneAsyncNameIndexInternal(sceneName, -1, mode == LoadSceneMode.Additive, false);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, bool isAdditive, bool mustCompleteNextFrame);
        public static void MergeScenes(Scene sourceScene, Scene destinationScene)
        {
            INTERNAL_CALL_MergeScenes(ref sourceScene, ref destinationScene);
        }

        public static void MoveGameObjectToScene(GameObject go, Scene scene)
        {
            INTERNAL_CALL_MoveGameObjectToScene(go, ref scene);
        }

        public static bool SetActiveScene(Scene scene)
        {
            return INTERNAL_CALL_SetActiveScene(ref scene);
        }

        public static bool UnloadScene(int sceneBuildIndex)
        {
            return UnloadSceneNameIndexInternal(string.Empty, sceneBuildIndex);
        }

        public static bool UnloadScene(string sceneName)
        {
            return UnloadSceneNameIndexInternal(sceneName, -1);
        }

        public static bool UnloadScene(Scene scene)
        {
            return INTERNAL_CALL_UnloadScene(ref scene);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex);

        public static int sceneCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int sceneCountInBuildSettings { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

