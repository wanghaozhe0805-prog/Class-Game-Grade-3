using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Random2DGeneratorWindow : EditorWindow
{
    private int spawnCount = 10;
    private List<GameObject> prefabs = new List<GameObject>();
    private Vector2 minMaxScale = new Vector2(0.5f, 2f);
    private Vector2Int gridSize = new Vector2Int(50, 50);
    private bool useUniformScale = true;
    private Vector2 minMaxScaleX = new Vector2(0.5f, 2f);
    private Vector2 minMaxScaleY = new Vector2(0.5f, 2f);
    
    private Vector2 scrollPosition;
    
    [MenuItem("Tools/2D Random Generator")]
    public static void ShowWindow()
    {
        GetWindow<Random2DGeneratorWindow>("2D随机生成器");
    }
    
    private void OnGUI()
    {
        GUILayout.Space(10);
        
        // 基本设置
        EditorGUILayout.LabelField("生成设置", EditorStyles.boldLabel);
        spawnCount = EditorGUILayout.IntField("生成数量", spawnCount);
        gridSize = EditorGUILayout.Vector2IntField("生成区域大小", gridSize);
        
        GUILayout.Space(10);
        
        // 缩放设置
        EditorGUILayout.LabelField("缩放设置", EditorStyles.boldLabel);
        useUniformScale = EditorGUILayout.Toggle("统一缩放", useUniformScale);
        
        if (useUniformScale)
        {
            EditorGUILayout.MinMaxSlider("缩放范围", ref minMaxScale.x, ref minMaxScale.y, 0.1f, 5f);
            EditorGUILayout.LabelField($"最小: {minMaxScale.x:F2}, 最大: {minMaxScale.y:F2}");
        }
        else
        {
            EditorGUILayout.MinMaxSlider("X轴缩放范围", ref minMaxScaleX.x, ref minMaxScaleX.y, 0.1f, 5f);
            EditorGUILayout.LabelField($"X轴 - 最小: {minMaxScaleX.x:F2}, 最大: {minMaxScaleX.y:F2}");
            
            EditorGUILayout.MinMaxSlider("Y轴缩放范围", ref minMaxScaleY.x, ref minMaxScaleY.y, 0.1f, 5f);
            EditorGUILayout.LabelField($"Y轴 - 最小: {minMaxScaleY.x:F2}, 最大: {minMaxScaleY.y:F2}");
        }
        
        GUILayout.Space(10);
        
        // 预制体列表
        EditorGUILayout.LabelField("预制体列表", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox("添加需要随机生成的预制体，至少需要1个", MessageType.Info);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
        
        for (int i = 0; i < prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            prefabs[i] = (GameObject)EditorGUILayout.ObjectField($"预制体 {i + 1}", prefabs[i], typeof(GameObject), false);
            
            if (GUILayout.Button("移除", GUILayout.Width(60)))
            {
                prefabs.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        if (GUILayout.Button("添加预制体"))
        {
            prefabs.Add(null);
        }
        
        GUILayout.Space(20);
        
        // 生成按钮
        EditorGUI.BeginDisabledGroup(prefabs.Count == 0 || prefabs[0] == null);
        if (GUILayout.Button("生成物体", GUILayout.Height(30)))
        {
            GenerateObjects();
        }
        EditorGUI.EndDisabledGroup();
        
        if (GUILayout.Button("清除生成的物体", GUILayout.Height(25)))
        {
            ClearGeneratedObjects();
        }
        
        // 显示状态信息
        if (prefabs.Count == 0 || prefabs[0] == null)
        {
            EditorGUILayout.HelpBox("请至少添加一个预制体", MessageType.Warning);
        }
    }
    
    private void GenerateObjects()
    {
        if (prefabs.Count == 0 || prefabs[0] == null)
        {
            EditorUtility.DisplayDialog("错误", "请至少添加一个有效的预制体", "确定");
            return;
        }
        
        // 创建父物体来组织生成的物体
        GameObject parentObject = new GameObject("GeneratedObjects");
        Undo.RegisterCreatedObjectUndo(parentObject, "Generate 2D Objects");
        
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();
        int attempts = 0;
        int maxAttempts = spawnCount * 100; // 防止无限循环
        
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2Int randomPos = GetRandomPosition(usedPositions, ref attempts, maxAttempts);
            
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning($"无法找到更多不重复的位置，已生成 {i} 个物体");
                break;
            }
            
            usedPositions.Add(randomPos);
            
            // 随机选择预制体
            GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Count)];
            if (selectedPrefab == null) continue;
            
            // 实例化预制体
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
            newObject.transform.position = new Vector3(randomPos.x, randomPos.y, 0);
            newObject.transform.SetParent(parentObject.transform);
            
            // 设置随机缩放
            Vector3 randomScale = useUniformScale ? 
                Vector3.one * Random.Range(minMaxScale.x, minMaxScale.y) :
                new Vector3(
                    Random.Range(minMaxScaleX.x, minMaxScaleX.y),
                    Random.Range(minMaxScaleY.x, minMaxScaleY.y),
                    1f
                );
            
            newObject.transform.localScale = randomScale;
            
            // 记录Undo操作
            Undo.RegisterCreatedObjectUndo(newObject, "Generate 2D Object");
        }
        
        Debug.Log($"成功生成 {usedPositions.Count} 个物体");
        Selection.activeGameObject = parentObject;
    }
    
    private Vector2Int GetRandomPosition(HashSet<Vector2Int> usedPositions, ref int attempts, int maxAttempts)
    {
        Vector2Int randomPos;
        
        do
        {
            randomPos = new Vector2Int(
                Random.Range(-gridSize.x / 2, gridSize.x / 2 + 1),
                Random.Range(-gridSize.y / 2, gridSize.y / 2 + 1)
            );
            attempts++;
            
            if (attempts >= maxAttempts)
            {
                Debug.LogError("达到最大尝试次数，无法找到不重复的位置");
                return randomPos;
            }
            
        } while (usedPositions.Contains(randomPos));
        
        return randomPos;
    }
    
    private void ClearGeneratedObjects()
    {
        GameObject[] generatedObjects = GameObject.FindGameObjectsWithTag("Generated");
        GameObject parent = GameObject.Find("GeneratedObjects");
        
        if (parent != null)
        {
            Undo.DestroyObjectImmediate(parent);
        }
        
        // 清理可能没有父物体的生成物体
        foreach (GameObject obj in generatedObjects)
        {
            if (obj != null)
            {
                Undo.DestroyObjectImmediate(obj);
            }
        }
        
        Debug.Log("已清除所有生成的物体");
    }
}