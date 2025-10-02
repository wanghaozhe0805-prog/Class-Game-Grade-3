# Vue + Unity WebGL 集成 API 文档

## 概述

本文档提供了Vue前端与Unity WebGL游戏集成的完整解决方案，包括方法调用接口、消息通信机制和实际使用示例。

## 目录

1. [API接口定义](#api接口定义)
2. [消息通信协议](#消息通信协议)
3. [Vue组件示例](#vue组件示例)
4. [完整项目示例](#完整项目示例)
5. [错误处理](#错误处理)
6. [最佳实践](#最佳实践)

---

## API接口定义

### Unity方法接口

| 方法名 | 参数 | 返回值 | 描述 | 反馈类型 |
|--------|------|--------|------|----------|
| `OnMoveForward` | 无 | 无 | 向前移动 | Success/Error |
| `OnMoveBackward` | 无 | 无 | 向后移动 | Success/Error |
| `OnMoveLeft` | 无 | 无 | 向左移动 | Success/Error |
| `OnMoveRight` | 无 | 无 | 向右移动 | Success/Error |
| `OnResetPosition` | 无 | 无 | 重置位置 | Success/Error |
| `GetPlayerPosition` | 无 | string | 获取玩家位置 | 无反馈 |

### 调用格式

```javascript
// 基本调用格式
unityInstance.SendMessage('WebGLBridge', '方法名');

// 示例
unityInstance.SendMessage('WebGLBridge', 'OnMoveForward');
```

---

## 消息通信协议

### 1. 前端 → Unity 通信

#### 调用消息格式
```javascript
// 通过Unity WebGL实例调用
unityInstance.SendMessage('WebGLBridge', 'OnMoveForward');
```

#### 参数说明
- `gameObjectName`: 固定为 `'WebGLBridge'`
- `methodName`: Unity中的方法名

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

#### 消息字段说明

| 字段 | 类型 | 描述 |
|------|------|------|
| `type` | string | 固定值 `'unity_feedback'` |
| `feedbackType` | string | 反馈类型：`success`、`error`、`warning` |
| `data` | string | JSON字符串，包含详细反馈信息 |
| `timestamp` | string | ISO格式时间戳 |

#### 反馈数据结构
```javascript
{
    methodName: 'OnMoveForward',    // 调用的方法名
    message: '向前移动成功',         // 反馈消息
    type: 'Success',                // 反馈类型
    timestamp: '2024-01-15 14:30:25' // 时间戳
}
```

---

## Vue组件示例

### 1. Unity控制器组件

```vue
<template>
  <div class="unity-controller">
    <div class="unity-canvas-container">
      <canvas ref="unityCanvas" width="800" height="600"></canvas>
    </div>
    
    <div class="control-panel">
      <h3>游戏控制</h3>
      
      <!-- 移动控制 -->
      <div class="movement-controls">
        <button 
          @click="callUnityMethod('OnMoveForward')"
          :disabled="!isUnityReady"
          class="control-btn btn-up"
        >
          向上移动
        </button>
        <button 
          @click="callUnityMethod('OnMoveBackward')"
          :disabled="!isUnityReady"
          class="control-btn btn-down"
        >
          向下移动
        </button>
        <button 
          @click="callUnityMethod('OnMoveLeft')"
          :disabled="!isUnityReady"
          class="control-btn btn-left"
        >
          向左移动
        </button>
        <button 
          @click="callUnityMethod('OnMoveRight')"
          :disabled="!isUnityReady"
          class="control-btn btn-right"
        >
          向右移动
        </button>
      </div>
      
      <!-- 其他控制 -->
      <div class="other-controls">
        <button 
          @click="callUnityMethod('OnResetPosition')"
          :disabled="!isUnityReady"
          class="control-btn btn-reset"
        >
          重置位置
        </button>
        <button 
          @click="getPlayerPosition"
          :disabled="!isUnityReady"
          class="control-btn btn-info"
        >
          获取位置
        </button>
      </div>
      
      <!-- 状态显示 -->
      <div class="status-display">
        <p>Unity状态: {{ unityStatus }}</p>
        <p v-if="playerPosition">玩家位置: {{ playerPosition }}</p>
      </div>
    </div>
    
    <!-- 反馈消息显示 -->
    <div class="feedback-panel">
      <h3>反馈消息</h3>
      <div class="feedback-tabs">
        <button 
          @click="activeTab = 'success'"
          :class="{ active: activeTab === 'success' }"
        >
          成功 ({{ successMessages.length }})
        </button>
        <button 
          @click="activeTab = 'error'"
          :class="{ active: activeTab === 'error' }"
        >
          错误 ({{ errorMessages.length }})
        </button>
        <button 
          @click="activeTab = 'warning'"
          :class="{ active: activeTab === 'warning' }"
        >
          警告 ({{ warningMessages.length }})
        </button>
      </div>
      
      <div class="feedback-content">
        <div v-if="activeTab === 'success'" class="feedback-list">
          <div 
            v-for="msg in successMessages" 
            :key="msg.timestamp"
            class="feedback-item success"
          >
            <div class="feedback-header">
              <span class="method-name">{{ msg.methodName }}</span>
              <span class="timestamp">{{ formatTime(msg.timestamp) }}</span>
            </div>
            <div class="feedback-message">{{ msg.message }}</div>
          </div>
        </div>
        
        <div v-if="activeTab === 'error'" class="feedback-list">
          <div 
            v-for="msg in errorMessages" 
            :key="msg.timestamp"
            class="feedback-item error"
          >
            <div class="feedback-header">
              <span class="method-name">{{ msg.methodName }}</span>
              <span class="timestamp">{{ formatTime(msg.timestamp) }}</span>
            </div>
            <div class="feedback-message">{{ msg.message }}</div>
          </div>
        </div>
        
        <div v-if="activeTab === 'warning'" class="feedback-list">
          <div 
            v-for="msg in warningMessages" 
            :key="msg.timestamp"
            class="feedback-item warning"
          >
            <div class="feedback-header">
              <span class="method-name">{{ msg.methodName }}</span>
              <span class="timestamp">{{ formatTime(msg.timestamp) }}</span>
            </div>
            <div class="feedback-message">{{ msg.message }}</div>
          </div>
        </div>
      </div>
      
      <div class="feedback-actions">
        <button @click="clearFeedback" class="clear-btn">清空所有反馈</button>
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
      unityStatus: '未初始化',
      playerPosition: '',
      activeTab: 'success',
      successMessages: [],
      errorMessages: [],
      warningMessages: [],
      maxMessages: 50 // 最大消息数量
    }
  },
  
  mounted() {
    this.initUnity();
    this.setupMessageListener();
  },
  
  beforeUnmount() {
    this.removeMessageListener();
  },
  
  methods: {
    // 初始化Unity
    async initUnity() {
      try {
        this.unityStatus = '正在加载...';
        
        // 这里需要根据实际的Unity WebGL构建文件路径进行调整
        const unityConfig = {
          dataUrl: '/unity/Build/unity.data',
          frameworkUrl: '/unity/Build/unity.framework.js',
          codeUrl: '/unity/Build/unity.wasm',
          streamingAssetsUrl: '/unity/StreamingAssets',
          companyName: 'YourCompany',
          productName: 'YourGame',
          productVersion: '1.0.0',
          devicePixelRatio: 1
        };
        
        // 加载Unity WebGL
        this.unityInstance = await createUnityInstance(
          this.$refs.unityCanvas,
          unityConfig,
          (progress) => {
            console.log(`Unity加载进度: ${Math.round(progress * 100)}%`);
          }
        );
        
        this.isUnityReady = true;
        this.unityStatus = '已就绪';
        console.log('Unity初始化成功');
        
      } catch (error) {
        this.unityStatus = '初始化失败';
        console.error('Unity初始化失败:', error);
        this.addFeedback('error', {
          methodName: 'UnityInit',
          message: `Unity初始化失败: ${error.message}`,
          type: 'Error',
          timestamp: new Date().toISOString()
        });
      }
    },
    
    // 设置消息监听器
    setupMessageListener() {
      this.messageHandler = this.handleUnityMessage.bind(this);
      window.addEventListener('message', this.messageHandler);
    },
    
    // 移除消息监听器
    removeMessageListener() {
      if (this.messageHandler) {
        window.removeEventListener('message', this.messageHandler);
      }
    },
    
    // 处理Unity消息
    handleUnityMessage(event) {
      const message = event.data;
      
      // 检查是否是Unity反馈消息
      if (message && message.type === 'unity_feedback') {
        console.log('收到Unity反馈:', message);
        
        try {
          const feedbackData = JSON.parse(message.data);
          this.addFeedback(message.feedbackType, feedbackData);
        } catch (error) {
          console.error('解析反馈数据失败:', error);
        }
      }
    },
    
    // 调用Unity方法
    callUnityMethod(methodName) {
      if (!this.isUnityReady || !this.unityInstance) {
        this.addFeedback('error', {
          methodName: methodName,
          message: 'Unity未就绪',
          type: 'Error',
          timestamp: new Date().toISOString()
        });
        return;
      }
      
      try {
        console.log(`调用Unity方法: ${methodName}`);
        this.unityInstance.SendMessage('WebGLBridge', methodName);
      } catch (error) {
        console.error(`调用Unity方法失败: ${methodName}`, error);
        this.addFeedback('error', {
          methodName: methodName,
          message: `调用失败: ${error.message}`,
          type: 'Error',
          timestamp: new Date().toISOString()
        });
      }
    },
    
    // 获取玩家位置
    getPlayerPosition() {
      if (!this.isUnityReady || !this.unityInstance) {
        this.addFeedback('error', {
          methodName: 'GetPlayerPosition',
          message: 'Unity未就绪',
          type: 'Error',
          timestamp: new Date().toISOString()
        });
        return;
      }
      
      try {
        // 注意：GetPlayerPosition方法需要特殊处理，因为它返回字符串
        // 这里需要通过其他方式获取位置信息
        console.log('获取玩家位置');
        // 可以通过监听玩家移动事件来获取位置
      } catch (error) {
        console.error('获取玩家位置失败:', error);
      }
    },
    
    // 添加反馈消息
    addFeedback(type, data) {
      const messageList = this[`${type}Messages`];
      
      // 添加到列表开头
      messageList.unshift(data);
      
      // 限制消息数量
      if (messageList.length > this.maxMessages) {
        messageList.pop();
      }
      
      // 触发Vue响应式更新
      this.$forceUpdate();
    },
    
    // 清空反馈消息
    clearFeedback() {
      this.successMessages = [];
      this.errorMessages = [];
      this.warningMessages = [];
    },
    
    // 格式化时间
    formatTime(timestamp) {
      try {
        const date = new Date(timestamp);
        return date.toLocaleString('zh-CN');
      } catch (error) {
        return timestamp;
      }
    }
  }
}
</script>

<style scoped>
.unity-controller {
  display: flex;
  gap: 20px;
  padding: 20px;
  background: #f5f5f5;
  min-height: 100vh;
}

.unity-canvas-container {
  flex: 1;
  text-align: center;
}

.unity-canvas-container canvas {
  border: 2px solid #ddd;
  border-radius: 8px;
  max-width: 100%;
}

.control-panel {
  flex: 0 0 300px;
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.movement-controls {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
  margin-bottom: 20px;
}

.other-controls {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 20px;
}

.control-btn {
  padding: 12px 16px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.3s;
}

.control-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.2);
}

.control-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-up { background: #4CAF50; color: white; }
.btn-down { background: #2196F3; color: white; }
.btn-left { background: #FF9800; color: white; }
.btn-right { background: #9C27B0; color: white; }
.btn-reset { background: #f44336; color: white; }
.btn-info { background: #607D8B; color: white; }

.status-display {
  background: #f8f9fa;
  padding: 15px;
  border-radius: 6px;
  margin-top: 20px;
}

.status-display p {
  margin: 5px 0;
  font-size: 14px;
}

.feedback-panel {
  flex: 0 0 400px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  overflow: hidden;
}

.feedback-panel h3 {
  margin: 0;
  padding: 20px;
  background: #f8f9fa;
  border-bottom: 1px solid #dee2e6;
}

.feedback-tabs {
  display: flex;
  border-bottom: 1px solid #dee2e6;
}

.feedback-tabs button {
  flex: 1;
  padding: 12px;
  border: none;
  background: #f8f9fa;
  cursor: pointer;
  transition: background-color 0.3s;
}

.feedback-tabs button.active {
  background: white;
  border-bottom: 2px solid #007bff;
}

.feedback-content {
  max-height: 400px;
  overflow-y: auto;
}

.feedback-list {
  padding: 10px;
}

.feedback-item {
  margin-bottom: 10px;
  padding: 12px;
  border-radius: 6px;
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

.feedback-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 5px;
}

.method-name {
  font-weight: bold;
  color: #333;
}

.timestamp {
  font-size: 12px;
  color: #666;
}

.feedback-message {
  color: #555;
  font-size: 14px;
}

.feedback-actions {
  padding: 15px;
  border-top: 1px solid #dee2e6;
}

.clear-btn {
  width: 100%;
  padding: 10px;
  background: #6c757d;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.clear-btn:hover {
  background: #5a6268;
}
</style>
```

### 2. Unity服务类

```javascript
// services/UnityService.js
export class UnityService {
  constructor() {
    this.unityInstance = null;
    this.isReady = false;
    this.listeners = new Map();
    this.setupMessageListener();
  }
  
  // 初始化Unity
  async initUnity(canvas, config) {
    try {
      this.unityInstance = await createUnityInstance(canvas, config);
      this.isReady = true;
      this.emit('ready');
      return this.unityInstance;
    } catch (error) {
      this.emit('error', error);
      throw error;
    }
  }
  
  // 调用Unity方法
  callMethod(methodName, gameObjectName = 'WebGLBridge') {
    if (!this.isReady || !this.unityInstance) {
      throw new Error('Unity未就绪');
    }
    
    try {
      this.unityInstance.SendMessage(gameObjectName, methodName);
      return true;
    } catch (error) {
      console.error(`调用Unity方法失败: ${methodName}`, error);
      throw error;
    }
  }
  
  // 设置消息监听器
  setupMessageListener() {
    this.messageHandler = this.handleMessage.bind(this);
    window.addEventListener('message', this.messageHandler);
  }
  
  // 处理消息
  handleMessage(event) {
    const message = event.data;
    
    if (message && message.type === 'unity_feedback') {
      try {
        const feedbackData = JSON.parse(message.data);
        this.emit('feedback', {
          type: message.feedbackType,
          data: feedbackData,
          timestamp: message.timestamp
        });
      } catch (error) {
        console.error('解析反馈数据失败:', error);
      }
    }
  }
  
  // 事件监听
  on(event, callback) {
    if (!this.listeners.has(event)) {
      this.listeners.set(event, []);
    }
    this.listeners.get(event).push(callback);
  }
  
  // 移除事件监听
  off(event, callback) {
    if (this.listeners.has(event)) {
      const callbacks = this.listeners.get(event);
      const index = callbacks.indexOf(callback);
      if (index > -1) {
        callbacks.splice(index, 1);
      }
    }
  }
  
  // 触发事件
  emit(event, data) {
    if (this.listeners.has(event)) {
      this.listeners.get(event).forEach(callback => {
        callback(data);
      });
    }
  }
  
  // 销毁
  destroy() {
    if (this.messageHandler) {
      window.removeEventListener('message', this.messageHandler);
    }
    this.listeners.clear();
  }
}

// 创建单例实例
export const unityService = new UnityService();
```

### 3. 使用示例

```vue
<!-- App.vue -->
<template>
  <div id="app">
    <UnityController />
  </div>
</template>

<script>
import UnityController from './components/UnityController.vue'

export default {
  name: 'App',
  components: {
    UnityController
  }
}
</script>
```

---

## 完整项目示例

### 项目结构
```
vue-unity-project/
├── public/
│   └── unity/
│       ├── Build/
│       │   ├── unity.data
│       │   ├── unity.framework.js
│       │   └── unity.wasm
│       └── StreamingAssets/
├── src/
│   ├── components/
│   │   └── UnityController.vue
│   ├── services/
│   │   └── UnityService.js
│   ├── App.vue
│   └── main.js
├── package.json
└── README.md
```

### package.json
```json
{
  "name": "vue-unity-integration",
  "version": "1.0.0",
  "scripts": {
    "serve": "vue-cli-service serve",
    "build": "vue-cli-service build",
    "lint": "vue-cli-service lint"
  },
  "dependencies": {
    "vue": "^3.2.0"
  },
  "devDependencies": {
    "@vue/cli-service": "^5.0.0"
  }
}
```

---

## 错误处理

### 1. Unity初始化错误
```javascript
try {
  await unityService.initUnity(canvas, config);
} catch (error) {
  console.error('Unity初始化失败:', error);
  // 显示错误提示
  this.showError('Unity加载失败，请刷新页面重试');
}
```

### 2. 方法调用错误
```javascript
try {
  unityService.callMethod('OnMoveForward');
} catch (error) {
  console.error('方法调用失败:', error);
  // 显示错误提示
  this.showError('操作失败，请稍后重试');
}
```

### 3. 消息解析错误
```javascript
window.addEventListener('message', (event) => {
  try {
    const message = event.data;
    if (message && message.type === 'unity_feedback') {
      const feedbackData = JSON.parse(message.data);
      // 处理反馈
    }
  } catch (error) {
    console.error('消息处理失败:', error);
  }
});
```

---

## 最佳实践

### 1. 错误处理
- 始终使用try-catch包装Unity方法调用
- 提供用户友好的错误提示
- 记录详细的错误日志

### 2. 性能优化
- 限制反馈消息数量，避免内存泄漏
- 使用防抖处理频繁的方法调用
- 合理设置Unity WebGL的内存限制

### 3. 用户体验
- 提供加载状态指示
- 禁用未就绪时的按钮
- 显示操作反馈和结果

### 4. 代码组织
- 使用服务类封装Unity相关逻辑
- 组件化设计，便于复用
- 统一的消息处理机制

### 5. 调试技巧
- 使用浏览器控制台查看消息传输
- 添加详细的日志记录
- 使用Vue DevTools调试组件状态

---

## 常见问题

### Q1: Unity WebGL加载失败
**A**: 检查文件路径是否正确，确保所有Unity构建文件都在正确位置。

### Q2: 方法调用无响应
**A**: 确认Unity实例已初始化，检查方法名是否正确。

### Q3: 反馈消息未收到
**A**: 检查消息监听器是否正确设置，确认Unity端反馈系统正常工作。

### Q4: 跨域问题
**A**: 确保Unity WebGL和Vue应用在同一域名下，或配置正确的CORS策略。

---

## 技术支持

如有问题，请检查：
1. Unity控制台错误信息
2. 浏览器控制台JavaScript错误
3. 网络面板中的文件加载状态
4. Vue组件的数据绑定状态 