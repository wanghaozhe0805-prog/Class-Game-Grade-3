using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommandManager : MonoBehaviour
{
    private BehaviorManager _mBehaviorManager;
    private WebGLBridge _webGLBridge;
    public EventSentSystem EventSentSystemOnStart = new EventSentSystem();
    public EventSentSystem EventSentSystemOnEnd = new EventSentSystem();
    public EventSentSystem<string> EventSentSystemOnEndString = new EventSentSystem<string>();
    public EventSentSystem<int> EventSentSystemOnEndInt = new EventSentSystem<int>();

    private void Awake()
    {
        _mBehaviorManager = FindObjectOfType<BehaviorManager>();
        _webGLBridge = FindObjectOfType<WebGLBridge>();
        _webGLBridge.AddEvent();
        AddEvent();
        _webGLBridge.RegisterEvent();
        RegisterEvent();
    }

    private void AddEvent()
    {
        EventSentSystemOnStart.AddEvent(BehaviorType.Check.ToString());
        EventSentSystemOnStart.AddEvent(BehaviorType.CollectObject.ToString());
        EventSentSystemOnStart.AddEvent(BehaviorType.FindItemCount.ToString());
        EventSentSystemOnStart.AddEvent(BehaviorType.Compositing.ToString());
        EventSentSystemOnStart.AddEvent(BehaviorType.SetActiveComposePage.ToString());
        EventSentSystemOnStart.AddEvent(BehaviorType.PlaceItem.ToString());
        
        EventSentSystemOnEndString.AddEvent(BehaviorType.Check.ToString());
        EventSentSystemOnEnd.AddEvent(BehaviorType.CollectObject.ToString());
        EventSentSystemOnEndInt.AddEvent(BehaviorType.FindItemCount.ToString());
        EventSentSystemOnEnd.AddEvent(BehaviorType.Compositing.ToString());
        EventSentSystemOnEnd.AddEvent(BehaviorType.SetActiveComposePage.ToString());
        EventSentSystemOnEnd.AddEvent(BehaviorType.PlaceItem.ToString());
    }

    private void RegisterEvent()
    {
        _webGLBridge.EventSentSystem.RegisterEvent(BehaviorType.MoveForward.ToString(), OnMoveForward);
        _webGLBridge.EventSentSystem.RegisterEvent(BehaviorType.RotateLeft.ToString(), OnRotateLeft);
        _webGLBridge.EventSentSystem.RegisterEvent(BehaviorType.RotateRight.ToString(), OnRotateRight);
        _webGLBridge.EventSentSystem.RegisterEvent(BehaviorType.Check.ToString(), OnCheck);
        _webGLBridge.EventSentSystemString.RegisterEvent(BehaviorType.CollectObject.ToString(), OnCollectObject);
        _webGLBridge.EventSentSystemString.RegisterEvent(BehaviorType.FindItemCount.ToString(), OnFindItemCount);
        _webGLBridge.EventSentSystemString.RegisterEvent(BehaviorType.Compositing.ToString(), OnCompositing);
        _webGLBridge.EventSentSystemString.RegisterEvent(BehaviorType.SetActiveComposePage.ToString(), OnSetActiveComposePage);
        _webGLBridge.EventSentSystemString.RegisterEvent(BehaviorType.PlaceItem.ToString(), OnPlaceItem);
        _webGLBridge.EventSentSystem.RegisterEvent(BehaviorType.FillSoil.ToString(), OnFillSoil);
        _webGLBridge.EventSentSystem.RegisterEvent("GameReset", GameReset);
        _webGLBridge.EventSentSystemString.RegisterEvent("GameLoad", GameLoad);
    }
    

    private void OnMoveForward()
    {
        _mBehaviorManager.AddBehavior(BehaviorType.MoveForward, null);
    }

    private void OnRotateLeft()
    {
        _mBehaviorManager.AddBehavior(BehaviorType.RotateLeft, null);
    }

    private void OnRotateRight()
    {
        _mBehaviorManager.AddBehavior(BehaviorType.RotateRight, null);
    }

    private void OnCheck()
    {
        void CallBack(Behavior behavior)
        {
            behavior.OnStart += () => EventSentSystemOnStart.DoEvent(BehaviorType.Check.ToString());
            Check check = behavior as Check;
            behavior.OnEnd += () =>
                EventSentSystemOnEndString.DoEvent(BehaviorType.Check.ToString(), check.GetObjectId());
        }

        _mBehaviorManager.AddBehavior(BehaviorType.Check, CallBack);
    }

    private void OnCollectObject(string objectId)
    {
        void CallBack(Behavior behavior)
        {
            CollectObject collectObject = behavior as CollectObject;
            collectObject.CompareTag(objectId);
            behavior.OnStart += () => EventSentSystemOnStart.DoEvent(BehaviorType.CollectObject.ToString());
            behavior.OnEnd += () => EventSentSystemOnEnd.DoEvent(BehaviorType.CollectObject.ToString());
        }

        _mBehaviorManager.AddBehavior(BehaviorType.CollectObject, CallBack);
    }

    private void OnFindItemCount(string objectId)
    {
        void CallBack(Behavior behavior)
        {
            FindItemCount findItemCount = behavior as FindItemCount;
            findItemCount.SetItemId(objectId);
            behavior.OnStart += () => EventSentSystemOnStart.DoEvent(BehaviorType.FindItemCount.ToString());
            behavior.OnEnd += () =>
                EventSentSystemOnEndInt.DoEvent(BehaviorType.FindItemCount.ToString(), findItemCount.GetCount());
        }

        _mBehaviorManager.AddBehavior(BehaviorType.FindItemCount, CallBack);
    }

    private void OnCompositing(string objectId)
    {
        void CallBack(Behavior behavior)
        {
            Compositing compositing = behavior as Compositing;
            compositing.SetItemId(objectId);
            behavior.OnStart += () => EventSentSystemOnStart.DoEvent(BehaviorType.Compositing.ToString());
            behavior.OnEnd += () => EventSentSystemOnEnd.DoEvent(BehaviorType.Compositing.ToString());
        }
        _mBehaviorManager.AddBehavior(BehaviorType.Compositing, CallBack);
    }
    private void OnSetActiveComposePage(string isActive)
    {
        void CallBack(Behavior behavior)
        {
            SetActiveComposePage setActiveComposePage = behavior as SetActiveComposePage;
            setActiveComposePage.SetIsActive(isActive);
            behavior.OnStart += () => EventSentSystemOnStart.DoEvent(BehaviorType.SetActiveComposePage.ToString());
            behavior.OnEnd += () => EventSentSystemOnEnd.DoEvent(BehaviorType.SetActiveComposePage.ToString());
        }

        _mBehaviorManager.AddBehavior(BehaviorType.SetActiveComposePage, CallBack);
    }
    private void OnPlaceItem(string objectId)
    {
        void CallBack(Behavior behavior)
        {
            PlaceItem placeItem = behavior as PlaceItem;
            placeItem.SetItemId(objectId);
            behavior.OnStart += () => EventSentSystemOnStart.DoEvent(BehaviorType.PlaceItem.ToString());
            behavior.OnEnd += () => EventSentSystemOnEnd.DoEvent(BehaviorType.PlaceItem.ToString());
        }
        _mBehaviorManager.AddBehavior(BehaviorType.PlaceItem, CallBack);
    }

    private void OnFillSoil()
    {
        _mBehaviorManager.AddBehavior(BehaviorType.FillSoil, null);
    }

    private void GameReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GameLoad(string sceneName)
    {
        //string str = "Game/Scenes/"+ sceneName;
        SceneManager.LoadScene(sceneName);
    }
}