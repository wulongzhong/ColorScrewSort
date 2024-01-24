using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : IDisposable
{
    private bool disposed = true;
    Dictionary<string, AsyncOperationHandle<SceneInstance>> dicCache = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

    public AsyncOperationHandle<SceneInstance> LoadScene(string path, LoadSceneMode loadSceneMode)
    {
        if (dicCache.ContainsKey(path))
        {
            return (dicCache[path]);
        }

        var handle = Addressables.LoadSceneAsync(path, loadSceneMode, false);
        dicCache.Add(path, handle);
        return handle;
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                foreach (var v in dicCache.Values)
                {
                    Addressables.UnloadSceneAsync(v);
                }
            }
            disposed = true;
        }
    }
    ~SceneLoader()
    {
        Dispose(false);
    }
}
