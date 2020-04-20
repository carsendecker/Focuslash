using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public Dictionary<GameObject, ObjectPool> Pools = new Dictionary<GameObject, ObjectPool>();
    
    void Awake()
    {
        Services.ObjectPools = this;
    }

    /// <summary>
    /// Creates a pool of objectToPool, and adds [initialAmount] of them into the pool.
    /// </summary>
    public ObjectPool Create(GameObject objectToPool, int initialAmount)
    {
        if (!Pools.ContainsKey(objectToPool))
            Pools.Add(objectToPool, new ObjectPool(objectToPool, initialAmount));
        
        //TODO: Add more objects to pool if initalAmount is bigger than current amount in pool
        
        return Pools[objectToPool];
    }

    /// <summary>
    /// Gets a gameobject from its pool, sets it active, then returns it.
    /// TODO: Add overload for instantiating at a position, rotation, etc like Instantiate()
    /// </summary>
    public GameObject Spawn(GameObject objectToSpawn, bool setToActive = true)
    {
        try
        {
            GameObject obj = Pools[objectToSpawn].Get();
            
            if(setToActive)
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
    
    public ObjectPool(GameObject objectToPool, int initialAmount)
    {
        pooledObject = objectToPool;
        objectList = new List<GameObject>(initialAmount);
        AddToPool(initialAmount);
    }

    /// <summary>
    /// Instantiates and adds certain amount of objects to the pool
    /// </summary>
    public void AddToPool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newObj = GameObject.Instantiate(pooledObject);
            newObj.SetActive(false);
            objectList.Add(newObj);
        }
    }

    public GameObject AddToPool()
    {
        GameObject newObj = GameObject.Instantiate(pooledObject);
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
