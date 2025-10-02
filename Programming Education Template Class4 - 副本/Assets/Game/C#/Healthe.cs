using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthe : MonoBehaviour
{
    private int _hp;
    private int _maxHp;
    public Action OnDead;
    private bool _isDead;

    public void SetHpMax(int hp)
    {
        _maxHp = hp;
        _hp = _maxHp;
    }

    public void Treatment(int hp)
    {
        _hp += hp;
        if (_hp > _maxHp)
        {
            _hp = _maxHp;
        }
    }
    
    public void Damage(int hp)
    {
        _hp -= hp;
        if (_hp <= 0&& !_isDead)
        {
            _isDead = true;
            OnDead?.Invoke();
            Debug.Log("Dead");
        }
    }
}