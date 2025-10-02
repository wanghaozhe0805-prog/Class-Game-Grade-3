using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [Header("玩家设置")]
    public Color playerColor = Color.blue;
    public Vector2 playerSize = new Vector2(1f, 1f);
    
    [Header("自动设置")]
    public bool autoSetup = true;
    
    void Start()
    {
        if (autoSetup)
        {
            SetupPlayer();
        }
    }
    
    [ContextMenu("设置玩家")]
    public void SetupPlayer()
    {
        // 添加必要的组件
        SetupComponents();
        
        // 设置外观
        SetupAppearance();
        
        // 设置物理
        SetupPhysics();
        
        Debug.Log("玩家设置完成！");
    }
    
    void SetupComponents()
    {
        // 添加PlayerController2D组件
        if (GetComponent<PlayerController2D>() == null)
        {
            gameObject.AddComponent<PlayerController2D>();
        }
        
        // 添加Rigidbody2D组件
        if (GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.drag = 0f;
            rb.angularDrag = 0f;
        }
        
        // 添加Collider2D组件
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = playerSize;
        }
    }
    
    void SetupAppearance()
    {
        // 添加SpriteRenderer组件
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // 创建默认的白色方块精灵
        spriteRenderer.sprite = CreateDefaultSprite();
        spriteRenderer.color = playerColor;
        
        // 设置排序层级
        spriteRenderer.sortingOrder = 1;
    }
    
    void SetupPhysics()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.drag = 0f;
            rb.angularDrag = 0f;
        }
    }
    
    Sprite CreateDefaultSprite()
    {
        // 创建一个简单的白色方块纹理
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // 创建精灵
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        sprite.name = "DefaultPlayerSprite";
        
        return sprite;
    }
    
    // 在编辑器中创建玩家预制体的静态方法
    #if UNITY_EDITOR
    [System.Serializable]
    public static class PlayerCreator
    {
        [UnityEditor.MenuItem("GameObject/2D Object/Player Controller")]
        public static void CreatePlayer()
        {
            GameObject player = new GameObject("Player");
            player.AddComponent<PlayerSetup>();
            player.AddComponent<PlayerController2D>();
            
            // 设置位置
            player.transform.position = Vector3.zero;
            
            // 选中创建的对象
            UnityEditor.Selection.activeGameObject = player;
            
            Debug.Log("玩家对象创建完成！");
        }
    }
    #endif
} 