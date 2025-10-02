# Unity WebGL 反馈系统使用说明

## 概述

本项目为Unity WebGL游戏增加了完整的方法调用反馈系统，每次前端调用Unity方法后，Unity会主动向前端发送成功或失败的消息，实现双向通信。

## 系统架构

### 1. Unity端 (C#)

#### WebGLBridge.cs
- **功能**: 接收前端方法调用并发送反馈
- **主要组件**:
  - `FeedbackType` 枚举: 定义反馈类型 (Success, Error, Warning)
  - `FeedbackData` 类: 反馈数据结构
  - `SendFeedback()` 方法: 发送反馈到前端
  - 所有控制方法都包含try-catch和反馈机制

#### WebGLInterface.jslib
- **功能**: JavaScript桥接层
- **新增方法**: `SendFeedbackToParent()` - 使用`window.parent.postMessage`发送反馈

### 2. 前端端 (JavaScript)

#### 消息格式
```javascript
{
    type: 'unity_feedback',
    feedbackType: 'success|error|warning',
    data: JSON.stringify({
        methodName: 'OnMoveForward',
        message: '向前移动成功',
        type: 'Success',
        timestamp: '2024-01-01 12:00:00'
    }),
    timestamp: '2024-01-01T12:00:00.000Z'
}
```

## 使用方法

### 1. Unity端设置

1. **确保场景中有WebGLBridge组件**
   ```csharp
   // 在场景中创建GameObject并添加WebGLBridge组件
   // 或者通过代码查找
   WebGLBridge bridge = FindObjectOfType<WebGLBridge>();
   ```

2. **方法调用会自动发送反馈**
   ```csharp
   // 前端调用: unityInstance.SendMessage('WebGLBridge', 'OnMoveForward');
   // Unity会自动发送成功或失败反馈
   ```

### 2. 前端端设置

#### 监听反馈消息
```javascript
window.addEventListener('message', function(event) {
    const message = event.data;
    
    // 检查是否是Unity反馈消息
    if (message && message.type === 'unity_feedback') {
        console.log('收到Unity反馈:', message);
        
        try {
            const feedbackData = JSON.parse(message.data);
            handleFeedback(message.feedbackType, feedbackData);
        } catch (error) {
            console.error('解析反馈数据失败:', error);
        }
    }
});
```

#### 处理反馈消息
```javascript
function handleFeedback(type, data) {
    switch (type) {
        case 'success':
            console.log(`✅ ${data.methodName}: ${data.message}`);
            // 显示成功提示
            showSuccessMessage(data.message);
            break;
            
        case 'error':
            console.error(`❌ ${data.methodName}: ${data.message}`);
            // 显示错误提示
            showErrorMessage(data.message);
            break;
            
        case 'warning':
            console.warn(`⚠️ ${data.methodName}: ${data.message}`);
            // 显示警告提示
            showWarningMessage(data.message);
            break;
    }
}
```

### 3. 调用Unity方法

```javascript
// 确保Unity实例已初始化
if (unityInstance && unityInstance.SendMessage) {
    // 调用Unity方法
    unityInstance.SendMessage('WebGLBridge', 'OnMoveForward');
    
    // Unity会自动发送反馈消息
    // 前端通过message事件监听器接收
}
```

## 反馈类型说明

### Success (成功)
- **触发条件**: 方法执行成功
- **示例消息**: "向前移动成功"
- **处理建议**: 显示成功提示，更新UI状态

### Error (错误)
- **触发条件**: 
  - 方法执行失败
  - 组件未找到
  - 异常发生
- **示例消息**: "玩家控制器未找到", "移动失败: 边界限制"
- **处理建议**: 显示错误提示，记录日志

### Warning (警告)
- **触发条件**: 方法执行但有警告信息
- **示例消息**: "移动受限，已达到边界"
- **处理建议**: 显示警告提示

## 扩展功能

### 1. 添加新的反馈方法

在`WebGLBridge.cs`中添加新方法：
```csharp
public void OnCustomAction()
{
    Debug.Log("WebGLBridge.OnCustomAction called from JS");
    if (playerController != null)
    {
        try
        {
            // 执行自定义逻辑
            playerController.CustomAction();
            SendFeedback("OnCustomAction", "自定义操作成功", FeedbackType.Success);
        }
        catch (System.Exception e)
        {
            SendFeedback("OnCustomAction", $"操作失败: {e.Message}", FeedbackType.Error);
        }
    }
    else
    {
        SendFeedback("OnCustomAction", "玩家控制器未找到", FeedbackType.Error);
    }
}
```

### 2. 自定义反馈数据结构

修改`FeedbackData`类添加更多字段：
```csharp
[System.Serializable]
public class FeedbackData
{
    public string methodName;
    public string message;
    public FeedbackType type;
    public string timestamp;
    public Vector3 playerPosition; // 新增：玩家位置
    public float executionTime;    // 新增：执行时间
    
    public FeedbackData(string method, string msg, FeedbackType feedbackType)
    {
        methodName = method;
        message = msg;
        type = feedbackType;
        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        // 获取玩家位置
        if (playerController != null)
        {
            playerPosition = playerController.transform.position;
        }
    }
}
```

### 3. 前端UI组件

参考`feedback_example.html`创建美观的反馈显示界面：
- 成功/错误/警告分类显示
- 实时更新
- 可清空历史记录
- 时间戳显示

## 调试技巧

### 1. Unity端调试
```csharp
// 在SendFeedback方法中添加详细日志
Debug.Log($"[WebGLBridge] 发送反馈: {methodName} - {message} ({type})");
```

### 2. 前端端调试
```javascript
// 监听所有postMessage消息
window.addEventListener('message', function(event) {
    console.log('收到消息:', event.data);
});
```

### 3. 浏览器控制台
- 查看Unity发送的反馈消息
- 检查消息格式是否正确
- 验证JSON解析是否成功

## 注意事项

1. **跨域限制**: 确保Unity WebGL和前端页面在同一域名下
2. **消息大小**: 避免发送过大的数据，影响性能
3. **错误处理**: 始终包含try-catch块处理异常
4. **性能优化**: 避免频繁发送反馈消息，可考虑批量发送
5. **兼容性**: 确保目标浏览器支持postMessage API

## 示例文件

- `WebGLBridge.cs`: 完整的反馈系统实现
- `WebGLInterface.jslib`: JavaScript桥接层
- `feedback_example.html`: 前端示例页面
- `README_反馈系统.md`: 本文档

## 技术支持

如有问题，请检查：
1. Unity控制台是否有错误信息
2. 浏览器控制台是否有JavaScript错误
3. 网络面板中是否有消息传输
4. WebGLBridge组件是否正确挂载 