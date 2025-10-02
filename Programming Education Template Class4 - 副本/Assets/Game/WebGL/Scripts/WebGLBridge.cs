using System;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Serialization;

public class WebGLBridge : MonoBehaviour
{
    [Header("引用设置")] private CommandManager _commandManager;
    public EventSentSystem EventSentSystem = new EventSentSystem();
    public EventSentSystem<string> EventSentSystemString = new EventSentSystem<string>();

    // 定义反馈消息类型
    public enum FeedbackType
    {
        Init,
        Success,
        Error,
        Warning,
        Value
    }

    // 定义反馈数据结构
    [System.Serializable]
    public class FeedbackData<T>
    {
        public string methodName;
        public string message;
        public FeedbackType type;
        public string timestamp;
        public T value;

        public FeedbackData(string method, string msg, FeedbackType feedbackType, T value)
        {
            methodName = method;
            message = msg;
            type = feedbackType;
            this.value = value;
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SendFeedbackToParent(string type, string data);
#else
    // 编辑器模式下的模拟方法
    private void SendFeedbackToParent(string type, string data)
    {
        Debug.Log($"[Editor] Would send feedback: type={type}, data={data}");
    }
#endif

    void Awake()
    {
    }

    public void AddEvent()
    {
        EventSentSystem.AddEvent(BehaviorType.MoveForward.ToString());
        EventSentSystem.AddEvent(BehaviorType.RotateLeft.ToString());
        EventSentSystem.AddEvent(BehaviorType.RotateRight.ToString());
        EventSentSystem.AddEvent(BehaviorType.Check.ToString());
        EventSentSystemString.AddEvent(BehaviorType.CollectObject.ToString());
        EventSentSystemString.AddEvent(BehaviorType.FindItemCount.ToString());
        EventSentSystemString.AddEvent(BehaviorType.Compositing.ToString());
        EventSentSystemString.AddEvent(BehaviorType.SetActiveComposePage.ToString());
        EventSentSystemString.AddEvent(BehaviorType.PlaceItem.ToString());
        EventSentSystem.AddEvent(BehaviorType.FillSoil.ToString());
        EventSentSystem.AddEvent("GameReset");
        EventSentSystemString.AddEvent("GameLoad");
    }

    public void RegisterEvent()
    {
        _commandManager = FindObjectOfType<CommandManager>();
        _commandManager.EventSentSystemOnEndString.RegisterEvent(BehaviorType.Check.ToString(), OnCheckEnd);
        _commandManager.EventSentSystemOnEndInt.RegisterEvent(BehaviorType.FindItemCount.ToString(),
            OnFindItemCountEnd);
    }

    // 发送反馈到前端
    private void SendFeedback<T>(string methodName, string message, FeedbackType type, T value = default)
    {
        var feedbackData = new FeedbackData<T>(methodName, message, type, value);
        string jsonData = JsonUtility.ToJson(feedbackData);

        // 发送到父窗口
        SendFeedbackToParent(type.ToString().ToLower(), jsonData);

        // 同时记录到Unity控制台
        string logMessage = $"[WebGLBridge] {methodName}: {message}";
        switch (type)
        {
            case FeedbackType.Success:
                Debug.Log(logMessage);
                break;
            case FeedbackType.Warning:
                Debug.LogWarning(logMessage);
                break;
            case FeedbackType.Error:
                Debug.LogError(logMessage);
                break;
        }
    }

    private void Inspect<T>(string methodName, string message, Action action, T value = default)
    {
        if (_commandManager != null)
        {
            try
            {
                action?.Invoke();
                SendFeedback<T>(methodName, message + "成功", FeedbackType.Success, value);
            }
            catch (System.Exception e)
            {
                SendFeedback<T>(methodName, message + $"失败: {e.Message}", FeedbackType.Error);
            }
        }
        else
        {
            SendFeedback<T>(methodName, "玩家控制器未找到", FeedbackType.Error);
        }
    }


    public void OnMoveForward()
    {
        void Action()
        {
            EventSentSystem.DoEvent(BehaviorType.MoveForward.ToString());
        }

        Inspect<string>(BehaviorType.MoveForward.ToString(), "添加移动指令", Action);
    }

    public void OnRotateLeft()
    {
        void Action()
        {
            EventSentSystem.DoEvent(BehaviorType.RotateLeft.ToString());
        }

        Inspect<string>(BehaviorType.RotateLeft.ToString(), "添加左转指令", Action);
    }

    public void OnRotateRight()
    {
        void Action()
        {
            EventSentSystem.DoEvent(BehaviorType.RotateRight.ToString());
        }

        Inspect<string>(BehaviorType.RotateRight.ToString(), "添加右转指令", Action);
    }

    public void OnCheck()
    {
        EventSentSystem.DoEvent(BehaviorType.Check.ToString());
        //Inspect<string>(BehaviorType.Check.ToString(), "添加检查指令", Action);
    }

    private void OnCheckEnd(string message)
    {
        Inspect<string>(BehaviorType.Check.ToString(), "检查结束", () => { }, message);
    }

    public void OnCollectObject(string message)
    {
        void Action()
        {
            EventSentSystemString.DoEvent(BehaviorType.CollectObject.ToString(), message);
        }

        Inspect(BehaviorType.CollectObject.ToString(), "收集指令", Action, "");
    }

    public void OnFindItemCount(string message)
    {
        EventSentSystemString.DoEvent(BehaviorType.FindItemCount.ToString(), message);
        //Inspect(BehaviorType.FindItemCount.ToString(), "查找数量指令", () => { }, "");
    }

    private void OnFindItemCountEnd(int message)
    {
        Inspect<int>(BehaviorType.FindItemCount.ToString(), "查找数量结束", () => { }, message);
    }

    public void OnCompositing(string message)
    {
        void Action()
        {
            EventSentSystemString.DoEvent(BehaviorType.Compositing.ToString(), message);
        }

        Inspect(BehaviorType.Compositing.ToString(), "合成指令", Action, "");
    }

    public void OnControlComposePage(string isActive)
    {
        void Action()
        {
            EventSentSystemString.DoEvent(BehaviorType.SetActiveComposePage.ToString(), isActive);
        }

        Inspect<string>(BehaviorType.SetActiveComposePage.ToString(), "设置合成页面指令", Action);
    }

    public void OnPlaceItem(string message)
    {
        void Action()
        {
            EventSentSystemString.DoEvent(BehaviorType.PlaceItem.ToString(), message);
        }

        Inspect<string>(BehaviorType.PlaceItem.ToString(), "放置指令", Action);
    }

    public void OnFillSoil()
    {
        void Action()
        {
            EventSentSystem.DoEvent(BehaviorType.FillSoil.ToString());
        }

        Inspect(BehaviorType.FillSoil.ToString(), "填充土壤指令", Action, "");
    }

    public void OnGameReset()
    {
        void Action()
        {
            EventSentSystem.DoEvent("OnGameReset");
        }

        Inspect<string>("OnGameReset", "游戏重置", Action);
    }

    public void OnGameLoad(string sceneName)
    {
        if (_commandManager != null)
        {
            try
            {
                EventSentSystemString.DoEvent("GameLoad", sceneName);
                SendFeedback<string>("GameLoad", "场景加初始化" + "成功", FeedbackType.Init, "");
                SendFeedback<string>("GameLoad", "场景加载" + "成功", FeedbackType.Success, "");
            }
            catch (System.Exception e)
            {
                SendFeedback<string>("GameLoad", "场景加载" + $"失败: {e.Message}", FeedbackType.Error);
            }
        }
        else
        {
            SendFeedback<string>("GameLoad", "玩家控制器未找到", FeedbackType.Error);
        }
    }
}