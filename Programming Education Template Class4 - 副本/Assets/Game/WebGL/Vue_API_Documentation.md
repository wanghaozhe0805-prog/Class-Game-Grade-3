# Vue + Unity WebGL API 文档

## 概述

本文档提供Vue前端与Unity WebGL集成的完整API接口和消息通信示例。

## API接口定义

### Unity方法接口

| 方法名 | 参数 | 返回值 | 描述 | 反馈类型 |
|--------|------|--------|------|----------|
| `OnMoveForward` | 无 | 无 | 向前移动 | Success/Error |
| `OnMoveBackward` | 无 | 无 | 向后移动 | Success/Error |
| `OnMoveLeft` | 无 | 无 | 向左移动 | Success/Error |
| `OnMoveRight` | 无 | 无 | 向右移动 | Success/Error |
| `OnResetPosition` | 无 | 无 | 重置位置 | Success/Error |

### 调用格式
```javascript
unityInstance.SendMessage('WebGLBridge', '方法名');
```

## 消息通信协议

### 1. 前端 → Unity 通信
```javascript
// 调用Unity方法
unityInstance.SendMessage('WebGLBridge', 'OnMoveForward');
```

### 2. Unity → 前端 通信

#### 反馈消息格式
```javascript
{
    type: 'unity_feedback',
    feedbackType: 'success|error|warning',
    data: JSON.stringify({
        methodName: 'OnMoveForward',
        message: '向前移动成功',
        type: 'Success',
        timestamp: '2024-01-15 14:30:25'
    }),
    timestamp: '2024-01-15T14:30:25.123Z'
}
```

## Vue组件示例

### UnityController.vue
```vue
<template>
  <div class="unity-controller">
    <!-- Unity画布 -->
    <canvas ref="unityCanvas" width="800" height="600"></canvas>
    
    <!-- 控制按钮 -->
    <div class="controls">
      <button @click="callUnityMethod('OnMoveForward')" :disabled="!isUnityReady">
        向上移动
      </button>
      <button @click="callUnityMethod('OnMoveBackward')" :disabled="!isUnityReady">
        向下移动
      </button>
      <button @click="callUnityMethod('OnMoveLeft')" :disabled="!isUnityReady">
        向左移动
      </button>
      <button @click="callUnityMethod('OnMoveRight')" :disabled="!isUnityReady">
        向右移动
      </button>
      <button @click="callUnityMethod('OnResetPosition')" :disabled="!isUnityReady">
        重置位置
      </button>
    </div>
    
    <!-- 反馈消息 -->
    <div class="feedback">
      <div v-for="msg in feedbackMessages" :key="msg.timestamp" 
           :class="['feedback-item', msg.type.toLowerCase()]">
        <strong>{{ msg.methodName }}</strong>: {{ msg.message }}
        <small>{{ formatTime(msg.timestamp) }}</small>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'UnityController',
  data() {
    return {
      unityInstance: null,
      isUnityReady: false,
      feedbackMessages: []
    }
  },
  
  mounted() {
    this.initUnity();
    this.setupMessageListener();
  },
  
  methods: {
    // 初始化Unity
    async initUnity() {
      try {
        const config = {
          dataUrl: '/unity/Build/unity.data',
          frameworkUrl: '/unity/Build/unity.framework.js',
          codeUrl: '/unity/Build/unity.wasm'
        };
        
        this.unityInstance = await createUnityInstance(
          this.$refs.unityCanvas,
          config
        );
        
        this.isUnityReady = true;
        console.log('Unity初始化成功');
      } catch (error) {
        console.error('Unity初始化失败:', error);
      }
    },
    
    // 设置消息监听
    setupMessageListener() {
      window.addEventListener('message', (event) => {
        const message = event.data;
        
        if (message && message.type === 'unity_feedback') {
          try {
            const feedbackData = JSON.parse(message.data);
            this.addFeedback(feedbackData);
          } catch (error) {
            console.error('解析反馈数据失败:', error);
          }
        }
      });
    },
    
    // 调用Unity方法
    callUnityMethod(methodName) {
      if (!this.isUnityReady) {
        console.warn('Unity未就绪');
        return;
      }
      
      try {
        this.unityInstance.SendMessage('WebGLBridge', methodName);
      } catch (error) {
        console.error(`调用方法失败: ${methodName}`, error);
      }
    },
    
    // 添加反馈消息
    addFeedback(data) {
      this.feedbackMessages.unshift(data);
      
      // 限制消息数量
      if (this.feedbackMessages.length > 20) {
        this.feedbackMessages.pop();
      }
    },
    
    // 格式化时间
    formatTime(timestamp) {
      return new Date(timestamp).toLocaleString('zh-CN');
    }
  }
}
</script>

<style scoped>
.unity-controller {
  padding: 20px;
}

.controls {
  margin: 20px 0;
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.controls button {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  background: #007bff;
  color: white;
}

.controls button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.feedback {
  max-height: 300px;
  overflow-y: auto;
  border: 1px solid #ddd;
  padding: 10px;
}

.feedback-item {
  margin-bottom: 10px;
  padding: 10px;
  border-radius: 4px;
  border-left: 4px solid;
}

.feedback-item.success {
  background: #d4edda;
  border-left-color: #28a745;
}

.feedback-item.error {
  background: #f8d7da;
  border-left-color: #dc3545;
}

.feedback-item.warning {
  background: #fff3cd;
  border-left-color: #ffc107;
}
</style>

## 使用示例

### 1. 成功反馈示例
```javascript
// 前端调用
unityInstance.SendMessage('WebGLBridge', 'OnMoveForward');

// Unity发送成功反馈
{
    type: 'unity_feedback',
    feedbackType: 'success',
    data: '{"methodName":"OnMoveForward","message":"向前移动成功","type":"Success","timestamp":"2024-01-15 14:30:25"}',
    timestamp: '2024-01-15T14:30:25.123Z'
}
```

### 2. 失败反馈示例
```javascript
// 前端调用
unityInstance.SendMessage('WebGLBridge', 'OnMoveForward');

// Unity发送失败反馈
{
    type: 'unity_feedback',
    feedbackType: 'error',
    data: '{"methodName":"OnMoveForward","message":"玩家控制器未找到","type":"Error","timestamp":"2024-01-15 14:30:25"}',
    timestamp: '2024-01-15T14:30:25.123Z'
}
```

## 错误处理

### 1. Unity未就绪
```javascript
if (!this.isUnityReady) {
  console.warn('Unity未就绪，无法调用方法');
  return;
}
```

### 2. 方法调用失败
```javascript
try {
  this.unityInstance.SendMessage('WebGLBridge', methodName);
} catch (error) {
  console.error('方法调用失败:', error);
  // 显示错误提示
}
```

### 3. 消息解析失败
```javascript
try {
  const feedbackData = JSON.parse(message.data);
  this.addFeedback(feedbackData);
} catch (error) {
  console.error('解析反馈数据失败:', error);
}
```

## 最佳实践

1. **错误处理**: 始终使用try-catch包装Unity方法调用
2. **状态管理**: 检查Unity是否就绪再调用方法
3. **消息限制**: 限制反馈消息数量，避免内存泄漏
4. **用户体验**: 提供加载状态和操作反馈
5. **调试**: 添加详细的控制台日志

## 常见问题

**Q: Unity WebGL加载失败**
A: 检查文件路径，确保所有Unity构建文件都在正确位置

**Q: 方法调用无响应**
A: 确认Unity实例已初始化，检查方法名是否正确

**Q: 反馈消息未收到**
A: 检查消息监听器设置，确认Unity端反馈系统正常工作 