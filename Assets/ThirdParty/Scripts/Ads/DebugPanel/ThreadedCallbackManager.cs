using UnityEngine;
using System.Collections.Generic;
using System;

public class ThreadedCallbackManager : MonoBehaviour
{
    private static ThreadedCallbackManager instance;
    private static readonly object lockObject = new object();
    private static List<Action> callbackQueue = new List<Action>();
    private static List<Action> executedCallbacks = new List<Action>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
        {

            Destroy(gameObject);

        }
    }

    public static void AddCallback(Action callback)
    {
        lock (lockObject)
        {
            callbackQueue.Add(callback);
        }
    }

    private void Update()
    {
        lock (lockObject)
        {
            executedCallbacks.AddRange(callbackQueue);
            callbackQueue.Clear();
        }

        for (int i = 0; i < executedCallbacks.Count; i++)
        {
            executedCallbacks[i].Invoke();
        }

        executedCallbacks.Clear();
    }
}
