using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Chobitech.Unity
{
    public static class ChobiUnity
    {
        public static async void LoadAddressableGameObjectAsync(object key, UnityAction<GameObject> onLoaded)
        {
            var handler = Addressables.LoadAssetAsync<GameObject>(key);
            var gObj = await handler.Task;

            Addressables.Release(handler);

            onLoaded?.Invoke(gObj);
        }

        public static IEnumerator LoadAddressableGameObjectProcess(object key, UnityAction<GameObject> onLoaded)
        {
            var handler = Addressables.LoadAssetAsync<GameObject>(key);
            yield return handler;

            GameObject gObj = null;

            if (handler.Status == AsyncOperationStatus.Succeeded)
            {
                gObj = handler.Result;
            }

            Addressables.Release(handler);

            onLoaded?.Invoke(gObj);
        }

        public static void Activate<T>(this T t, bool active) where T : MonoBehaviour
        {
            t.gameObject.SetActive(active);
        }


        public static void AddToLocalPosition(this Transform tr, Vector3 gap)
        {
            var p = tr.localPosition;
            p += gap;
            tr.localPosition = p;
        }

        public static void AddToGlobalPosition(this Transform tr, Vector3 gap)
        {
            var p = tr.position;
            p += gap;
            tr.position = p;
        }


        public static void SetPropertyWithMaterialPropertyBlock<T>(this T sr, MaterialPropertyBlock srcBlock, UnityAction<MaterialPropertyBlock> action) where T : Renderer
        {
            var mpb = srcBlock ?? new MaterialPropertyBlock();
            sr.GetPropertyBlock(mpb);
            action(mpb);
            sr.SetPropertyBlock(mpb);
        }
        public static void SetPropertyWithMaterialPropertyBlock<T>(this T sr, UnityAction<MaterialPropertyBlock> action) where T : Renderer
        {
            sr.SetPropertyWithMaterialPropertyBlock(null, action);
        }

        public static void SetBool(this MaterialPropertyBlock mpb, string name, bool b)
        {
            mpb.SetFloat(name, b ? 1 : 0);
        }
        public static void SetBool(this MaterialPropertyBlock mpb, int nameId, bool b)
        {
            mpb.SetFloat(nameId, b ? 1 : 0);
        }



        //--- 2024/11/07 added
        private static IEnumerator ConvertUnityAction(UnityAction action)
        {
            action();
            yield break;
        }
        public static IEnumerator ToIEnumerator(this UnityAction action)
        {
            return ConvertUnityAction(action);
        }
        public static UnityAction ToUnityAction(this IEnumerator process)
        {
            return () =>
            {
                while (process.MoveNext())
                {
                }
            };
        }
    }
}
