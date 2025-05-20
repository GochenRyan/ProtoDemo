using System.Collections.Generic;
using UnityEngine;
using ZeroPass;

public class DamageTextPool : MonoBehaviour
{
    public static DamageTextPool _instance;

    public static DamageTextPool Instance
    {
        get
        {
            if (_instance == null)
            {
                var gameObject = new GameObject(nameof(DamageTextPool));
                DontDestroyOnLoad(gameObject);
                _instance = gameObject.AddComponent<DamageTextPool>();
            }

            return _instance;
        }
    }

    public static void DestroyInstance()
    {
        _instance.DeleteObject();
        _instance = null;
    }

    public int initialPoolSize = 10;

    private Queue<DamageTextController> objectPool = new Queue<DamageTextController>();

    void Awake()
    {
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewInstance();
        }
    }

    DamageTextController CreateNewInstance()
    {
        var damageTextPrefab = Resources.Load<GameObject>("Prefabs/DamageText");
        GameObject obj = Instantiate(damageTextPrefab, transform);
        obj.SetActive(false);
        DamageTextController controller = obj.GetComponent<DamageTextController>();
        objectPool.Enqueue(controller);
        return controller;
    }

    public DamageTextController GetFromPool()
    {
        if (objectPool.Count == 0)
        {
            CreateNewInstance();
        }

        DamageTextController controller = objectPool.Dequeue();
        controller.gameObject.SetActive(true);
        return controller;
    }

    public void ReturnToPool(DamageTextController controller)
    {
        controller.ResetState();
        controller.gameObject.SetActive(false);
        objectPool.Enqueue(controller);
    }
}