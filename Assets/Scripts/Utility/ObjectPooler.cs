using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public Dictionary<string, ObjectPool> Pools = new Dictionary<string, ObjectPool>();
    
    void Awake()
    {
        Services.ObjectPools = this;
    }

    /// <summary>
    /// Creates a pool of objectToPool, and adds [initialAmount] of them into the pool.
    /// </summary>
    public ObjectPool Create(GameObject objectToPool, int initialAmount)
    {
        if (!Pools.ContainsKey(objectToPool.name))
            Pools.Add(objectToPool.name, new ObjectPool(objectToPool, initialAmount));
        
        //TODO: Add more objects to pool if initalAmount is bigger than current amount in pool
        
        return Pools[objectToPool.name];
    }

    /// <summary>
    /// Gets a gameobject from its pool, sets it active, then returns it.
    /// </summary>
    public GameObject Spawn(GameObject objectToSpawn) => Spawn(objectToSpawn, Vector3.zero, Quaternion.identity);

    public GameObject Spawn(GameObject objectToSpawn, Vector3 position, Quaternion rotation)
    {
        try
        {
            GameObject obj = Pools[objectToSpawn.name].Get();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }
        catch (KeyNotFoundException e)
        {
            throw new Exception("There is no object pool with the object " + objectToSpawn.name + "! Try making a pool first with Create().");
        }
    }
    
}

public class ObjectPool
{
    private List<GameObject> objectList;
    private GameObject pooledObject;
    private GameObject parentObject;
    
    public ObjectPool(GameObject objectToPool, int initialAmount)
    {
        pooledObject = objectToPool;
        objectList = new List<GameObject>(initialAmount);
        parentObject = new GameObject("-" + objectToPool.name + " Pool-");
        AddToPool(initialAmount);
    }

    /// <summary>
    /// Instantiates and adds certain amount of objects to the pool
    /// </summary>
    public void AddToPool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddToPool();
        }
    }

    public GameObject AddToPool()
    {
        GameObject newObj = GameObject.Instantiate(pooledObject, parentObject.transform);
        newObj.SetActive(false);
        objectList.Add(newObj);

        return newObj;
    }

    public GameObject Get()
    {
        foreach (GameObject obj in objectList)
        {
            if (!obj.activeSelf)
                return obj;
        }

        return AddToPool();
    }
}
