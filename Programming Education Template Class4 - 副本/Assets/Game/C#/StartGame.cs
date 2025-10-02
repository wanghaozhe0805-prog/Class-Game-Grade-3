using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    private WebGLBridge _webGLBridge;
    
    void Start()
    { 
        _webGLBridge = FindObjectOfType<WebGLBridge>();
        _webGLBridge.OnGameLoad("Level Free");
    }
}