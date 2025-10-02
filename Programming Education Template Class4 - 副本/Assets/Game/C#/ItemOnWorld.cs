using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class ItemOnWorld : MonoBehaviour
{
    protected Healthe health;
    [SerializeField] private int damage;
    [SerializeField] private int hp;
    [SerializeField] public List<ItemBase> items;
    protected GameObject Player;

    public virtual void Start()
    {
        health = GetComponent<Healthe>();
        Player = GameObject.FindGameObjectWithTag("Player");
        health.OnDead += OnDead;
        health.SetHpMax(hp);
    }

    public virtual void OnCollect()
    {
        health.Damage(damage);
        VoiceManager.Instance.Play(VoiceType.Hit, true);
        OnCollectAnimation();
    }

    public virtual void OnSpawnAnimation()
    {
        transform.localScale = new Vector3(1, 0, 1);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScale(new Vector3(0, 1.3f, 0), 0.2f).SetEase(Ease.InExpo));
        mySequence.Append(transform.DOScale(new Vector3(1, 1, 1), 0.2f));
        mySequence.Play();
    }

    protected abstract void OnCollectAnimation(Action onComplete = null);

    protected abstract void OnDeadAnimation(Action onComplete);

    protected virtual void OnDead()
    {
        OnDeadAnimation(() =>
        {
            InventoryManager inventoryManager = Player.GetComponent<InventoryManager>();
            inventoryManager.AddItem(items);
            VoiceManager.Instance.Play(VoiceType.Collect, true);
            Destroy(gameObject);
        });
    }
}