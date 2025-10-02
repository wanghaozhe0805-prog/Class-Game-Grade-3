using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BehaviorType
{
    GameRun,
    MoveForward,
    RotateLeft,
    RotateRight,
    Check,
    CollectObject,
    FindItemCount,
    Compositing,
    SetActiveComposePage,
    PlaceItem,
    FillSoil
}

public abstract class Behavior
{
    public Action OnEnd;
    public Action OnStart;
    public Action OnNextBehavior;
    protected GameObject player;
    protected PlayerController control;
    protected Animator animator;

    protected Behavior()
    {
        player = GameObject.FindWithTag("Player");
        control = player.GetComponent<PlayerController>();
        animator = control.GetAnimator();
    }

    public virtual void Perform()
    {
        OnStart?.Invoke();
    }

    protected virtual void End()
    {
        OnEnd?.Invoke();
        OnNextBehavior?.Invoke();
    }
}

public class Move : Behavior
{
    private float _interTime = 0.5f;
    private BehaviorManager _behaviorManager;

    public Move() : base()
    {
        _behaviorManager = GameObject.FindObjectOfType<BehaviorManager>();
        _behaviorManager.EventSentSystem.RegisterEvent("OnBehaviorRunEnd", () => { animator.SetInteger("State", 0); });
    }

    public override void Perform()
    {
        base.Perform();
        PlayerController controller = player.GetComponent<PlayerController>();
        bool isFaceWall = controller.IsFaceWall();
        if (!isFaceWall)
        {
            Vector3 rotateDir = control.GetDir();
            Vector2 pos = rotateDir + player.transform.position;
            animator.SetInteger("State", 2);
            player.transform.DOMove(pos, 1).SetEase(Ease.Linear).OnComplete(End);
        }
        else
        {
            animator.SetInteger("State", 2);
            AddAction.instance.DelayPlay(End, _interTime);
        }
    }

    protected override void End()
    {
        //animator.SetInteger("State", 0);
        base.End();
    }
}

public class RotateLeft : Behavior
{
    private Vector2 _param;

    public override void Perform()
    {
        base.Perform();
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.SwitchRotateDir(1);
        animator.SetInteger("State", 0);
        VoiceManager.Instance.Play(VoiceType.Move, true);
        AddAction.instance.DelayPlay(End, 1);
    }
}

public class RotateRight : Behavior
{
    private Vector2 _param;

    public override void Perform()
    {
        base.Perform();
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.SwitchRotateDir(-1);
        animator.SetInteger("State", 0);
        VoiceManager.Instance.Play(VoiceType.Move, true);
        AddAction.instance.DelayPlay(End, 1);
    }
}

public class Check : Behavior
{
    private GameObject _object;

    public override void Perform()
    {
        base.Perform();
        PlayerController controller = player.GetComponent<PlayerController>();
        animator.SetInteger("State", 0);
        controller.ObjectRayInspect();
        _object = controller.GetFaceObject();
        End();
    }

    public string GetObjectId()
    {
        if (_object == null) return "";
        ObjectId objectId = _object.GetComponent<ObjectId>();
        if (objectId == null) return "";
        return objectId.GetId().ToString();
    }
}

public class CollectObject : Behavior
{
    private string _objectId;
    private GameObject _object;

    public void CompareTag(string id)
    {
        _objectId = id;
    }

    public override void Perform()
    {
        base.Perform();
        PlayerController controller = player.GetComponent<PlayerController>();
        animator.SetTrigger("Slash1H");
        controller.OnPlayCollectAnimationAction = () =>
        {
            VoiceManager.Instance.Play(VoiceType.Attack, true);
            controller.ObjectRayInspect();
            _object = controller.GetFaceObject();
            if (_object != null)
            {
                ObjectId objectId = _object.GetComponent<ObjectId>();
                ItemOnWorld itemOnWorld = _object.GetComponent<ItemOnWorld>();
                if (objectId != null && itemOnWorld != null) End();
                if (objectId.CompareId(_objectId))
                {
                    itemOnWorld.OnCollect();
                }
            }

            End();
        };
    }
}

public class FindItemCount : Behavior
{
    private string _itemId;
    private int _count;

    public void SetItemId(string itemId)
    {
        _itemId = itemId;
    }

    public override void Perform()
    {
        base.Perform();
        ComposeManager composeManager = player.GetComponent<ComposeManager>();
        Debug.Log(_itemId);
        _count = composeManager.FindItemCount(_itemId);
        Debug.Log(_count);
        End();
    }

    public int GetCount()
    {
        return _count;
    }
}

public class Compositing : Behavior
{
    private string _itemId;

    public void SetItemId(string itemId)
    {
        _itemId = itemId;
    }

    public override void Perform()
    {
        base.Perform();
        ComposeManager composeManager = player.GetComponent<ComposeManager>();
        composeManager.Compositing(_itemId);
        VoiceManager.Instance.Play(VoiceType.Collect, true);
        End();
    }
}

public class SetActiveComposePage : Behavior
{
    private bool _isActive;

    public void SetIsActive(string isActive)
    {
        try
        {
            _isActive = bool.Parse(isActive);
        }
        catch (Exception e)
        {
            End();
        }
    }

    public override void Perform()
    {
        base.Perform();
        ComposeManager composeManager = player.GetComponent<ComposeManager>();
        composeManager.SetActiveComposePage(_isActive);
        VoiceManager.Instance.Play(VoiceType.UIShow, true);
        End();
    }
}

public class PlaceItem : Behavior
{
    private string _itemId;
    private InventoryManager _inventoryManager;
    private PlaceManager _placeManager;

    public void SetItemId(string itemId)
    {
        _itemId = itemId;
    }

    public override void Perform()
    {
        base.Perform();
        animator.SetTrigger("Slash1H");
        PlayerController controller = player.GetComponent<PlayerController>();
        _inventoryManager = player.GetComponent<InventoryManager>();
        _placeManager = player.GetComponent<PlaceManager>();
        controller.OnPlayCollectAnimationAction = () =>
        {
            ItemBase itemBase = _inventoryManager.FindItem(_itemId);
            VoiceManager.Instance.Play(VoiceType.Place, true);
            if (itemBase.prefabs != null && _inventoryManager.FindItemCount(itemBase) > 0)
            {
                List<GameObject> prefabs = itemBase.prefabs;
                int index = Random.Range(0, prefabs.Count);
                _placeManager.PlaceItem(prefabs[index]);
                _inventoryManager.RemoveItem(itemBase);
            }

            End();
        };
    }
}

public class FillSoil : Behavior
{
    private GameObject _object;
    private GameObject _soilPrefab;

    public override void Perform()
    {
        base.Perform();
        PlayerController controller = player.GetComponent<PlayerController>();
        animator.SetTrigger("Slash1H");
        controller.OnPlayCollectAnimationAction = () =>
        {
            VoiceManager.Instance.Play(VoiceType.Place, true);
            controller.ObjectRayInspect();
            _object = controller.GetFaceObject();
            if (_object != null)
            {
                _soilPrefab = Resources.Load<GameObject>("Prefab/Soil");
                ObjectId objectId = _object.GetComponent<ObjectId>();
                if (objectId != null)
                {
                    if (objectId.CompareId("Water"))
                    {
                        GameObject.Destroy(_object);
                        Vector3 pos = _object.transform.position;
                        GameObject soil = GameObject.Instantiate(_soilPrefab);
                        soil.transform.position = pos;
                    }
                }
            }

            End();
        };
    }
}