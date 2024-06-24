using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject ObjectVariant;
    public int PoolSize = 4;
    public List<GameObject> PoolObjects = new();

    void Awake()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject poolObject = Instantiate(ObjectVariant);
            poolObject.SetActive(false);
            PoolObjects.Add(poolObject);
        }
    }

    public GameObject GetPoolObject()
    {
        GameObject obj = PoolObjects[0];
        PoolObjects.RemoveAt(0);
        return obj;
    }

    public void ReturnPoolObject(GameObject obj)
    {
        obj.SetActive(false);
        PoolObjects.Add(obj);
    }

}
