using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Tree : ItemOnWorld
{
    private SpriteRenderer[] _renderers;
    private Color _deadColor = new Color(1, 1, 1, 0);

    public override void Start()
    {
        base.Start();
        _renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    protected override void OnCollectAnimation(Action onComplete = null)
    {
        transform.DOShakeRotation(0.5f, 15, 10, 10, false);
    }

    protected override void OnDeadAnimation(Action onComplete)
    {
        float posX = GameObject.FindWithTag("Player").transform.position.x;
        float dir= transform.position.x > posX ? -1 : 1;
        transform.DOKill();
        transform.DORotate(new Vector3(0, 0, 90*dir), 2);
        _renderers[0].DOColor(_deadColor, 2).OnComplete(()=>{ onComplete?.Invoke();});
        foreach (var  rend in _renderers)
        {
            rend.DOColor(_deadColor, 2);
        }
    }
}