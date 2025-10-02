using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 _rotateDir;
    private Animator _animator;
    public LayerMask layerMaskWall;
    public LayerMask layerMaskObject;

    private List<Vector2> _rotateDirList = new List<Vector2>
    {
        Vector2.right,
        Vector2.up,
        Vector2.left,
        Vector2.down,
    };

    public List<GameObject> modes;
    private GameObject _faceObject;
    private bool _isFaceWall;
    public Action OnPlayCollectAnimationAction;

    private void Awake()
    {
        _rotateDir = _rotateDirList[0];
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        WallRayInspect();
    }

    public Animator GetAnimator()
    {
        return _animator;
    }

    public Vector2 GetDir()
    {
        return _rotateDir;
    }

    public void SwitchRotateDir(int dir)
    {
        int offex = dir >= 0 ? 1 : -1;
        int index = _rotateDirList.IndexOf(_rotateDir);
        int cur = index + offex;
        if (cur < 0)
        {
            cur = _rotateDirList.Count - 1;
        }
        else if (cur > _rotateDirList.Count - 1)
        {
            cur = 0;
        }
        Debug.Log(cur);
        _rotateDir = _rotateDirList[cur];
        foreach (var mode in modes)
        {
            mode.SetActive(false);
        }
        modes[cur].SetActive(true);
    }

    private void WallRayInspect()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _rotateDir, 1f, layerMaskWall);
        _isFaceWall = hit.collider != null;
    }

    public bool IsFaceWall()
    {
        return _isFaceWall;
    }

    public void ObjectRayInspect()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _rotateDir, 1f, layerMaskObject);
        _faceObject = hit.collider != null ? hit.collider.gameObject : null;
    }

    public GameObject GetFaceObject()
    {
        return _faceObject;
    }

    public void OnPlayCollectAnimation()
    {
        OnPlayCollectAnimationAction?.Invoke();
    }

    public void OnPlayerMove()
    {
        VoiceManager.Instance.Play(VoiceType.Move, true);
    }
}