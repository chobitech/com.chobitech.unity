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
    }
}
