using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSentSystem
{
    private Dictionary<string, Action> _eventDictionary = new Dictionary<string, Action>();

    public void AddEvent(string eventName)
    {
        if (_eventDictionary.ContainsKey(eventName)) return;
        _eventDictionary.Add(eventName, null);
    }

    public void RegisterEvent(string eventName, Action action)
    {
        _eventDictionary[eventName] += action;
    }

    public void UnRegisterEvent(string eventName, Action action)
    {
        _eventDictionary[eventName] -= action;
    }

    public void ClearEvent()
    {
        _eventDictionary.Clear();
    }

    public void DoEvent(string eventName)
    {
        _eventDictionary[eventName]?.Invoke();
    }
}

public class EventSentSystem<T>
{
    private Dictionary<string, Action<T>> _eventDictionary = new Dictionary<string, Action<T>>();

    public void AddEvent(string eventName)
    {
        if (_eventDictionary.ContainsKey(eventName)) return;
        _eventDictionary.Add(eventName, null);
    }

    public void RegisterEvent(string eventName, Action<T> action)
    {
        _eventDictionary[eventName] += action;
    }

    public void UnRegisterEvent(string eventName, Action<T> action)
    {
        _eventDictionary[eventName] -= action;
    }

    public void ClearEvent()
    {
        _eventDictionary.Clear();
    }

    public void DoEvent(string eventName, T t)
    {
        _eventDictionary[eventName]?.Invoke(t);
    }
}

public class EventSentSystem<T1, T2>
{ 
    private Dictionary<string, Action<T1, T2>> _eventDictionary = new Dictionary<string, Action<T1, T2>>();
    public void AddEvent(string eventName)
    {
        if (_eventDictionary.ContainsKey(eventName)) return;
        _eventDictionary.Add(eventName, null);
    }
    public void RegisterEvent(string eventName, Action<T1, T2> action)
    {
        _eventDictionary[eventName] += action;
    }
    public void UnRegisterEvent(string eventName, Action<T1, T2> action)
    {
        _eventDictionary[eventName] -= action;
    }
    public void ClearEvent()
    {
        _eventDictionary.Clear();
    }
    public void DoEvent(string eventName, T1 t1, T2 t2)
    {
        _eventDictionary[eventName]?.Invoke(t1, t2);
    }
}