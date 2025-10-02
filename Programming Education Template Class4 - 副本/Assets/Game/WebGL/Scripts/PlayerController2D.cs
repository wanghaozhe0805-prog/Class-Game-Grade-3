using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    
    [Header("边界设置")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;
    
    private Vector3 initialPosition;
    private Rigidbody2D rb;
    private WebGLEventBridge eventBridge;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // 设置刚体属性
        rb.gravityScale = 0f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        
        // 记录初始位置
        initialPosition = transform.position;
        eventBridge = FindObjectOfType<WebGLEventBridge>();
    }
    
    void Update()
    {
        HandleKeyboardInput();
    }
    
    void HandleKeyboardInput()
    {
        // 获取输入
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // 移动
        Vector2 movement = new Vector2(horizontalInput, verticalInput);
        if (movement.magnitude > 0)
        {
            MovePlayer(movement);
        }
        
        // 重置位置
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
    }
    
    public void MovePlayer(Vector2 direction)
    {
        Debug.Log($"PlayerController2D.MovePlayer called, direction: {direction}");
        // 标准化方向向量
        direction = direction.normalized;
        
        // 计算新位置
        Vector3 newPosition = transform.position + new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;
        
        // 应用边界限制
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        
        // 移动物体
        transform.position = newPosition;
        // 移动后通知前端
        if (eventBridge != null)
        {
            //eventBridge.OnPlayerMoved(transform.position);
        }
    }
    
    public void ResetPosition()
    {
        Debug.Log("PlayerController2D.ResetPosition called");
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
    }
    
    // WebGL按钮控制方法
    public void OnMoveForward()
    {
        Debug.Log("PlayerController2D.OnMoveForward called from WebGLBridge");
        MovePlayer(Vector2.up);
    }
    
    public void OnMoveBackward()
    {
        Debug.Log("PlayerController2D.OnMoveBackward called from WebGLBridge");
        MovePlayer(Vector2.down);
    }
    
    public void OnMoveLeft()
    {
        Debug.Log("PlayerController2D.OnMoveLeft called from WebGLBridge");
        MovePlayer(Vector2.left);
    }
    
    public void OnMoveRight()
    {
        Debug.Log("PlayerController2D.OnMoveRight called from WebGLBridge");
        MovePlayer(Vector2.right);
    }
    
    public void OnResetPosition()
    {
        Debug.Log("PlayerController2D.OnResetPosition called from WebGLBridge");
        ResetPosition();
    }
} 