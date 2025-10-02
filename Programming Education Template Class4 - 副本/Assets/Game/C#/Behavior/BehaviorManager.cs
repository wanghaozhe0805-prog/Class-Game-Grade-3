using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BehaviorManager : MonoBehaviour
{
    private List<Behavior> _mBehaviors;
    private bool _isRun;
    private int _currentIndex;
    public EventSentSystem EventSentSystem = new EventSentSystem();

    private void Awake()
    {
        _mBehaviors = new List<Behavior>();
        EventSentSystem.AddEvent("OnBehaviorRunEnd");
    }

    private void Start()
    {
    }

    private void Update()
    {
        RunBehavior();
    }

    public void AddBehavior(BehaviorType type, Action<Behavior> callback = null)
    {
        Behavior behavior = GetBehaviorType(type);
        _mBehaviors.Add(behavior);
        callback?.Invoke(behavior);
    }

    // public void RunBehavior()
    // {
    //     if (_mBehaviors.Count == 0 || _mBehaviors == null) return;
    //     for (int i = 0; i < _mBehaviors.Count; i++)
    //     {
    //         int next = i + 1;
    //         if (next <= _mBehaviors.Count - 1)
    //         {
    //             _mBehaviors[i].OnEnd += _mBehaviors[next].Perform;
    //         }
    //         else
    //         {
    //             //全部行为结束
    //             _mBehaviors[i].OnEnd += () =>
    //                 _mBehaviors.Clear();
    //         }
    //     }
    //
    //     _mBehaviors[0].Perform();
    // }

    private void RunBehavior()
    {
        if (_mBehaviors.Count > 0&& !_isRun)
        {
            _mBehaviors[_currentIndex].OnNextBehavior = () =>
            {
                _isRun = false;
                int next = _currentIndex + 1;
                if (next <= _mBehaviors.Count - 1)
                {
                    _currentIndex++;
                }
                else
                {
                    _currentIndex = 0;
                    _mBehaviors.Clear();
                    EventSentSystem.DoEvent("OnBehaviorRunEnd");
                }
            };
            _isRun = true;
            _mBehaviors[_currentIndex].Perform();
        }
    }

    private Behavior GetBehaviorType(BehaviorType type)
    {
        switch (type)
        {
            case BehaviorType.MoveForward:
                return new Move();
            case BehaviorType.RotateLeft:
                return new RotateLeft();
            case BehaviorType.RotateRight:
                return new RotateRight();
            case BehaviorType.Check:
                return new Check();
            case BehaviorType.CollectObject:
                return new CollectObject();
            case BehaviorType.FindItemCount:
                return new FindItemCount();
            case BehaviorType.Compositing:
                return new Compositing();
            case BehaviorType.SetActiveComposePage:
                return new SetActiveComposePage();
            case BehaviorType.PlaceItem:
                return new PlaceItem();
            case BehaviorType.FillSoil:
                return new FillSoil();
            default:
                return null;
        }
    }
}