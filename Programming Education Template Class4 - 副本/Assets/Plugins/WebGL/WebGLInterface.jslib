mergeInto(LibraryManager.library, {
    SendToWeb: function(message) {
        var messageStr = UTF8ToString(message);
        
        // 发送消息到前端
        if (typeof window !== 'undefined') {
            // 发送到父窗口（如果存在）
            if (window.parent && window.parent !== window) {
                window.parent.postMessage(messageStr, '*');
            }
            
            // 发送到当前窗口
            window.postMessage(messageStr, '*');
            
            // 记录到控制台
            console.log('Unity -> Web:', messageStr);
        }
    },
    
    LogToWeb: function(message) {
        var messageStr = UTF8ToString(message);
        
        // 记录到控制台
        if (typeof console !== 'undefined') {
            console.log('Unity Log:', messageStr);
        }
        
        // 可以在这里添加自定义的日志处理逻辑
        // 例如发送到日志服务器或显示在页面上
    },

    UnitySendMessage: function (eventname, data) {
      window.ReportReady(UTF8ToString(eventname), UTF8ToString(data));
    },

    SendFeedbackToParent: function (type, data) {
        var typeStr = UTF8ToString(type);
        var dataStr = UTF8ToString(data);
        
        // 构造反馈消息对象
        var feedbackMessage = {
            type: 'unity_feedback',
            feedbackType: typeStr,
            data: dataStr,
            timestamp: new Date().toISOString()
        };
        
        // 发送到父窗口（如果存在）
        if (typeof window !== 'undefined' && window.parent && window.parent !== window) {
            window.parent.postMessage(feedbackMessage, '*');
        }
        
        // 发送到当前窗口
        if (typeof window !== 'undefined') {
            window.postMessage(feedbackMessage, '*');
        }
        
        // 记录到控制台
        console.log('Unity Feedback -> Web:', feedbackMessage);
    }
}); 