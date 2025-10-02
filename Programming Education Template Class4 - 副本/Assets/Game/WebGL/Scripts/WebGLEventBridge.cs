using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLEventBridge : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UnitySendMessage(string eventname, string data);
#endif

    // 供Unity内部调用，向前端发送事件
    public void SendEventToJS(string eventName, string jsonData)
    {
// #if UNITY_WEBGL && !UNITY_EDITOR
//         UnitySendMessage(eventName, jsonData);
// #else
//         Debug.Log($"[Editor] Would send event: {eventName}, data: {jsonData}");
// #endif
    }

    // 示例：物体移动后调用
    public void OnPlayerMoved(Vector3 pos)
    {
        var data = JsonUtility.ToJson(new { x = pos.x, y = pos.y, z = pos.z });
        SendEventToJS("OnPlayerMoved", data);
    }

    public void OnMoveForward(Vector3 pos)
    {
        var data = JsonUtility.ToJson(new { x = pos.x, y = pos.y, z = pos.z });
        SendEventToJS("OnMoveForward", data);
    }
    public void OnRotateRight(Vector2 dir)
    {
        var data = JsonUtility.ToJson(new { x = dir.x, y = dir.y });
        SendEventToJS("OnRotateRight", data);
    }
    public void OnRotateLeft(Vector2 dir)
    {
        var data = JsonUtility.ToJson(new { x = dir.x, y = dir.y });
        SendEventToJS("OnRotateLeft", data);
    }

    public void GetAhead(bool isHave)
    {
        var data = JsonUtility.ToJson(isHave);
        SendEventToJS("GetAhead", data);
    }
    public void OnDamage(bool isHave)
    {
        var data = JsonUtility.ToJson(isHave);
        SendEventToJS("OnDamage", data);
    }

    public void OnShear(bool isHave)
    {
        var data = JsonUtility.ToJson(isHave);
        SendEventToJS("OnShear", data);
    }

    public void OnGameReset()
    {
        SendEventToJS("OnGameReset", "");
    }
    
    
} 