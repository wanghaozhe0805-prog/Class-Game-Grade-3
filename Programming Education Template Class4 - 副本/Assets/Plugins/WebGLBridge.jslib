mergeInto(LibraryManager.library, {
    SendMessageToVue: function (ptr) {
        // 将C#传来的字符串指针转为JS字符串
        var message = UTF8ToString(ptr);
        // 调用前端全局回调或postMessage
        if (typeof window.OnUnityMessage === 'function') {
            window.OnUnityMessage(message);
        } else {
            window.postMessage({ type: 'unity', message: message }, '*');
        }
    }
}); 