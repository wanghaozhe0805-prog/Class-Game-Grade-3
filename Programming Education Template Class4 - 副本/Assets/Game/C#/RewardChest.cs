using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChest : ItemOnWorld
{
    public Action OnOpenChest;
    public Sprite openChestSprite;

    protected override void OnCollectAnimation(Action onComplete = null)
    {
        
    }

    protected override void OnDeadAnimation(Action onComplete)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = openChestSprite;
        OnOpenChest?.Invoke();
        VoiceManager.Instance.Play(VoiceType.Collect, true);
    }
}