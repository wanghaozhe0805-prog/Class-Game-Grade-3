using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OtherItemOnWorld : ItemOnWorld
{
    private SpriteRenderer _renderer;
    private Color _deadColor = new Color(1, 1, 1, 0);

    public override void Start()
    {
        base.Start();
        _renderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnCollectAnimation(Action onComplete = null)
    {
        transform.DOShakeRotation(0.5f, 15, 10, 10, false);
    }

    protected override void OnDeadAnimation(Action onComplete)
    {
        _renderer.DOColor(_deadColor, 2).OnComplete(() => { onComplete?.Invoke(); });
    }
}
