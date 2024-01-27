using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public static class EventArgsPool
{
    private static Dictionary<Type, EventArgsBase> pool = new Dictionary<Type, EventArgsBase>();
    public static T Get<T>() where T : EventArgsBase
    {
        var eventType = typeof(T);
        if (pool.ContainsKey(eventType))
        {
            return (T)pool[eventType];
        }
        return (T)Activator.CreateInstance(eventType);
    }

    public static void Release(EventArgsBase evtArgs)
    {
        evtArgs.Clear();
        if (pool.ContainsKey(evtArgs.GetType()))
        {
            return;
        }
        pool.Add(evtArgs.GetType(), evtArgs);
    }
}
