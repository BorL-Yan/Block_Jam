using System;
using System.Collections.Generic;
using DG.Tweening;
using Lib;
using UnityEngine;

public class BlockJamParticals : SingletonScene<BlockJamParticals>
{
    [SerializeField] private List<ParticalSettings> _particles;
    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    
    private void Init()
    {
        foreach (var item in _particles)
        {
            item.Init(this.gameObject);
        }    
    }

    public GameObject GetPartical(BlockColor color)
    {
        return _particles.Find(p => p.Color == color).Get();
    }
}

[Serializable]
public class ParticalSettings
{
    public BlockColor Color;
    public GameObject Partical;
    public int InitialCount;
    public float ParticalDuration;
    public PoolBase<GameObject> Pool {get; private set;}

    public void Init(GameObject root)
    {
        Pool = new PoolBase<GameObject>(
            PreloadFunc,
            obj => obj.gameObject.SetActive(true),
            obj => obj.gameObject.SetActive(false),
            (uint)InitialCount);
            
        GameObject PreloadFunc()
        {
            GameObject obj = GameObject.Instantiate(Partical);
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(root.transform);
            return obj;
        }
    }
    
    public GameObject Get()
    {
        GameObject obj = Pool.Get();
        
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(ParticalDuration)
            .AppendCallback(()=>Pool.Return(obj));
        return obj;
    }
}

