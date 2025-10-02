using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class SortingManager : MonoBehaviour
{
    public float radius;
    private int _currentSortLayer;

    // 可配置的排序层偏移量，可以根据需要调整
    public int baseSortingOrder = 0;

    private void Update()
    {
        Sort();
    }

    // 在Scene视图中显示检测范围（可选，便于调试）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Sort()
    {
        // 获取范围内的所有碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        if (colliders.Length == 0) return;

        // 过滤出有Renderer组件的物体，并按Y坐标从大到小排序
        var sortableObjects = new List<SortableObject>();

        for (int i = 0; i < colliders.Length; i++)
        {
            GameObject obj = colliders[i].gameObject;
            Renderer renderer = obj.GetComponent<Renderer>();
            SortingGroup sortingGroup = obj.GetComponent<SortingGroup>();
            SortingID sortingID = obj.GetComponent<SortingID>();
            if (sortingID == null) continue;
            if (renderer != null || sortingGroup != null)
            {
                sortableObjects.Add(new SortableObject
                {
                    gameObject = obj,
                    renderer = renderer,
                    sortingGroup = sortingGroup,
                    positionY = sortingID.GetRoot().position.y
                });
            }
        }

        // 按Y坐标从大到小排序（Y值越大，排序层级越小）
        sortableObjects = sortableObjects.OrderByDescending(x => x.positionY).ToList();

        _currentSortLayer = baseSortingOrder;

        // 设置排序层级
        for (int i = 0; i < sortableObjects.Count; i++)
        {
            SetOrder(_currentSortLayer, sortableObjects[i].gameObject);
            _currentSortLayer++;
        }
    }

    private void SetOrder(int index, GameObject obj)
    {
        Renderer render = obj.GetComponent<Renderer>();
        SortingGroup sortingGroup = obj.GetComponent<SortingGroup>();
        if (render != null)
        {
            render.sortingOrder = index;
        }
        else if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = index;
        }
    }

    // 辅助结构体，用于存储排序信息
    private struct SortableObject
    {
        public GameObject gameObject;
        public Renderer renderer;
        public SortingGroup sortingGroup;
        public float positionY;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}