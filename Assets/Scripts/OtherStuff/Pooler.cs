using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
    [SerializeField] protected GameObject _prefab;
    Queue<PooledObject> _prefabs;
    int _initialCount = 10;

    void Awake()
    {
        _prefabs = new Queue<PooledObject>();
    }

    void Start()
    {
        for (int i = 0; i < _initialCount; i++)
        {
            _prefabs.Enqueue(Create());
        }
    }

    public PooledObject Get()
    {
        if (_prefabs.Count > 0)
            return _prefabs.Dequeue();
        else
            return Create();
    }

    public void Return(PooledObject obj)
    {
        _prefabs.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }

    protected virtual PooledObject Create()
    {
        GameObject obj = Instantiate(_prefab);
        PooledObject pooled = obj.GetComponent<PooledObject>();
        pooled.Init(this);
        return pooled;
    }
}
