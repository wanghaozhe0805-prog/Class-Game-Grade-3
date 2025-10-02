using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI")] public GameObject endPageSuccessful;
    public GameObject endPageFailed;

    [Header("关卡选择")] public LevelType currentLevelType;

    public enum LevelType
    {
        Level1,
        Level2,
        Level3,
        LevelFree,
        Level4,
    }

    private Level _currentLevel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        LoadLevel();
    }

    private void Update()
    {
        _currentLevel?.OnInspect();
    }

    private void LoadLevel()
    {
        switch (currentLevelType)
        {
            case LevelType.Level1:
                _currentLevel = new Level1();
                break;
            case LevelType.Level2:
                _currentLevel = new Level2();
                break;
            case LevelType.Level3:
                _currentLevel = new Level3();
                break;
            case LevelType.Level4:
                _currentLevel = new Level4();
                break;
            case LevelType.LevelFree:
                _currentLevel = new LevelFree();
                break;
        }
    }

    public void EndGame(bool successful)
    {
        if (successful)
        {
            endPageSuccessful.SetActive(true);
            AddAction.instance.OpenPageBySize(endPageSuccessful);
            VoiceManager.Instance.Play(VoiceType.Successful, true);
        }
        else
        {
            endPageFailed.SetActive(true);
            AddAction.instance.OpenPageBySize(endPageSuccessful);
            VoiceManager.Instance.Play(VoiceType.Fail, true);
        }
    }
}

public abstract class Level
{
    protected LevelManager LevelManager;
    protected GameObject player;

    public enum LevelState
    {
        Null,
        OnPlay,
        OnSuccessful,
        OnFailed,
    }

    protected LevelState State = LevelState.Null;

    public Level()
    {
        LevelManager = GameObject.FindObjectOfType<LevelManager>();
        player = GameObject.FindWithTag("Player");
        SwitchState(LevelState.OnPlay);
    }


    public void SwitchState(LevelState state)
    {
        if (State == state) return;
        State = state;
        StateEnter();
    }

    private void StateEnter()
    {
        switch (State)
        {
            case LevelState.OnPlay:
                OnEnterPlay();
                break;
            case LevelState.OnSuccessful:
                AddAction.instance.DelayPlay(OnEnterSuccessful, 1f);
                break;
            case LevelState.OnFailed:
                AddAction.instance.DelayPlay(OnEnterFailed, 1f);
                break;
        }
    }

    private void StateUpdate()
    {
        switch (State)
        {
            case LevelState.OnPlay:
                break;
            case LevelState.OnSuccessful:
                break;
            case LevelState.OnFailed:
                break;
        }
    }

    public abstract void OnInspect();

    protected abstract void OnEnterPlay();
    protected abstract void OnEnterSuccessful();
    protected abstract void OnEnterFailed();
}

public class Level1 : Level
{
    private InventoryManager _inventoryManager;

    private Dictionary<ObjectId.Id, int> _objectIdCount = new Dictionary<ObjectId.Id, int>()
    {
        { ObjectId.Id.Wood, 3 },
        { ObjectId.Id.Rock, 5 },
        { ObjectId.Id.Shrub, 3 },
    };

    public Level1() : base()
    {
        _inventoryManager = player.GetComponent<InventoryManager>();
    }

    public override void OnInspect()
    {
        bool isSuccess = true;
        foreach (var item in _objectIdCount)
        {
            int count = _inventoryManager.FindItemCount(item.Key.ToString());
            if (count < item.Value)
            {
                isSuccess = false;
            }
        }

        SwitchState(isSuccess ? LevelState.OnSuccessful : LevelState.OnFailed);
    }

    protected override void OnEnterPlay()
    {
    }

    protected override void OnEnterSuccessful()
    {
        LevelManager.EndGame(true);
        Debug.Log("Level1 Success");
    }

    protected override void OnEnterFailed()
    {
        //LevelManager.EndGame(false);
    }
}
public class Level2 : Level
{
    private InventoryManager _inventoryManager;

    private Dictionary<ObjectId.Id, int> _objectIdCount = new Dictionary<ObjectId.Id, int>()
    {
        { ObjectId.Id.House, 1 },
        { ObjectId.Id.GoddessStatue, 1 },
        { ObjectId.Id.Monuments, 1 },
    };

    public Level2() : base()
    {
        _inventoryManager = player.GetComponent<InventoryManager>();
    }

    public override void OnInspect()
    {
        bool isSuccess = true;
        foreach (var item in _objectIdCount)
        {
            int count = _inventoryManager.FindItemCount(item.Key.ToString());
            if (count < item.Value)
            {
                isSuccess = false;
            }
        }

        SwitchState(isSuccess ? LevelState.OnSuccessful : LevelState.OnFailed);
    }

    protected override void OnEnterPlay()
    {
    }

    protected override void OnEnterSuccessful()
    {
        LevelManager.EndGame(true);
        Debug.Log("Level2 Success");
    }

    protected override void OnEnterFailed()
    {
        // LevelManager.EndGame(false);
    }
}
public class Level3 : Level
{
    private PlaceManager _placeManager;

    private List<ObjectId.Id> _objectNeed = new List<ObjectId.Id>()
    {
        ObjectId.Id.Tree,
        ObjectId.Id.Tree,
        ObjectId.Id.Shrub,
        ObjectId.Id.Shrub,
        ObjectId.Id.House,
    };

    public Level3() : base()
    {
        _placeManager = GameObject.FindObjectOfType<PlaceManager>();
        _placeManager.OnPlaceItem += (id) =>
        {
            if (_objectNeed.Contains(id))
            {
                _objectNeed.Remove(id);
            }
        };
    }

    public override void OnInspect()
    {
        bool isSuccess = _objectNeed.Count == 0;
        SwitchState(isSuccess ? LevelState.OnSuccessful : LevelState.OnFailed);
    }

    protected override void OnEnterPlay()
    {
    }

    protected override void OnEnterSuccessful()
    {
        LevelManager.EndGame(true);
        Debug.Log("Level3 Success");
    }

    protected override void OnEnterFailed()
    {
        // LevelManager.EndGame(false);
    }
}

public class Level4 : Level
{
    private List<RewardChest> _rewardChests;
    public override void OnInspect()
    {
        bool isSuccess = _rewardChests.Count == 0;
        SwitchState(isSuccess ? LevelState.OnSuccessful : LevelState.OnFailed);
    }

    protected override void OnEnterPlay()
    {
        _rewardChests=new List<RewardChest>(GameObject.FindObjectsOfType<RewardChest>());
        foreach (var rewardChest in _rewardChests)
        {
            rewardChest.OnOpenChest += () =>
            {
                _rewardChests.Remove(rewardChest);
            };
        }
    }

    protected override void OnEnterSuccessful()
    {
        LevelManager.EndGame(true);
      
    }

    protected override void OnEnterFailed()
    {
       
    }
}
public class LevelFree : Level
{
    public override void OnInspect()
    {
    }

    protected override void OnEnterPlay()
    {
    }

    protected override void OnEnterSuccessful()
    {
    }

    protected override void OnEnterFailed()
    {
    }
}