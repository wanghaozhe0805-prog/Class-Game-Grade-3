using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingID : MonoBehaviour
{
    [SerializeField]
    private Transform root;

    private void Awake()
    {
        if (root == null)
            root = transform;
    }
    public Transform GetRoot()
    {
        return root;
    }
}