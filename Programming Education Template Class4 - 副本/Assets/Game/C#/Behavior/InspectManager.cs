using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectManager : MonoBehaviour
{
    public Button addButton;
    public Button clearButton;
    public GameObject prefab;
    public Transform parent;
    private WebGLBridge _webGLBridge;
    private BehaviorManager _behaviorManager;
    public Dropdown methodDropdown;
    public Dropdown messageDropdown;

    private List<string> _messageTypeList = new List<string>()
    {
        MessageType.Tree.ToString(),
        MessageType.Rock.ToString(),
        MessageType.Signboard.ToString(),
        MessageType.GoddessStatue.ToString(),
        MessageType.Wood.ToString(),
        MessageType.WoodenBarrel.ToString(),
        MessageType.Box.ToString(),
        MessageType.Shrub.ToString(),
        MessageType.StonePillar.ToString(),
        MessageType.Monuments.ToString(),
        MessageType.House.ToString(),
        MessageType.None.ToString(),
        MessageType.True.ToString(),
        MessageType.False.ToString(),
        MessageType.Water.ToString(),
        MessageType.RewardChest.ToString(),
    };

    private List<string> _methodTypeList = new List<string>()
    {
        MethodType.OnMoveForward.ToString(),
        MethodType.OnRotateLeft.ToString(),
        MethodType.OnRotateRight.ToString(),
        MethodType.OnCollectObject.ToString(),
        MethodType.OnPlaceItem.ToString(),
        MethodType.OnCompositing.ToString(),
        MethodType.OnControlComposePage.ToString(),
        MethodType.OnCheck.ToString(),
        MethodType.OnFindItemCount.ToString(),
        MethodType.OnFillSoil.ToString(),
    };

    public enum MethodType
    {
        OnMoveForward,
        OnRotateLeft,
        OnRotateRight,
        OnCollectObject,
        OnPlaceItem,
        OnCompositing,
        OnControlComposePage,
        OnCheck,
        OnFindItemCount,
        OnFillSoil,
    }

    public enum MessageType
    {
        None,
        Tree,
        Rock,
        Signboard,
        GoddessStatue,
        Wood,
        WoodenBarrel,
        Box,
        Shrub,
        StonePillar,
        Monuments,
        House,
        True,
        False,
        Water,
        RewardChest,
    }

    private void Start()
    {
        _webGLBridge = FindObjectOfType<WebGLBridge>();
        _behaviorManager = FindObjectOfType<BehaviorManager>();
        methodDropdown.AddOptions(_methodTypeList);
        messageDropdown.AddOptions(_messageTypeList);
        addButton.onClick.AddListener(AddCommand);
        clearButton.onClick.AddListener(ClearCommandButton);
    }

    private void ClearCommandButton()
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private void AddCommand()
    {
        string message = messageDropdown.options[messageDropdown.value].text;
        string method = methodDropdown.options[methodDropdown.value].text;
        switch (method)
        {
            case "OnMoveForward":
                message = "";
                _webGLBridge.OnMoveForward();
                break;
            case "OnRotateLeft":
                message = "";
                _webGLBridge.OnRotateLeft();
                break;
            case "OnRotateRight":
                message = "";
                _webGLBridge.OnRotateRight();
                break;
            case "OnCollectObject":
                _webGLBridge.OnCollectObject(message);
                break;
            case "OnPlaceItem":
                _webGLBridge.OnPlaceItem(message);
                break;
            case "OnCompositing":
                _webGLBridge.OnCompositing(message);
                break;
            case "OnControlComposePage":
                _webGLBridge.OnControlComposePage(message);
                break;
            case "OnCheck":
                message = "";
                _webGLBridge.OnCheck();
                break;
            case "OnFindItemCount":
                _webGLBridge.OnFindItemCount(message);
                break;
            case "OnFillSoil":
                _webGLBridge.OnFillSoil();
                break;
        }

        GameObject go = Instantiate(prefab, parent);
        go.GetComponentInChildren<Text>().text = method + "(" + message + ")";
    }
}