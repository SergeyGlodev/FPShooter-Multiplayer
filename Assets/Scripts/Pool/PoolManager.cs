using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Serializable] public class PoolRule
    {
        public GameObject prefab;
        public int amount;
    }

    [SerializeField] private List<PoolRule> poolRules = new List<PoolRule>();

    public static PoolManager Instance => instance;

    private static PoolManager instance;
    private Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();
    

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = FindObjectOfType<PoolManager>();
        }
        else
        {
            Destroy(gameObject);
        }
        Initialize();
    }

    public void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();

        GameObject poolHolder = new GameObject(prefab.name + " Pool");
        poolHolder.transform.parent = transform;

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<ObjectInstance>());

            for (int i = 0; i < poolSize; i++)
            {
                ObjectInstance newObject = new ObjectInstance(Instantiate(prefab));
                poolDictionary[poolKey].Enqueue(newObject);
                newObject.SetParent(poolHolder.transform);
            }
        }
    }

    public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();
        if (poolDictionary.ContainsKey(poolKey))
        {
            ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(objectToReuse);

            objectToReuse.Reuse(position, rotation);
        }
    }

    private void Initialize()
    {
        foreach (PoolRule rule in poolRules)
        {
            CreatePool(rule.prefab, rule.amount);
        }
    }


    public class ObjectInstance
    {
        public readonly GameObject gameObject;
        readonly Transform transform;
        readonly bool hasBoolObjectComponent;
        readonly PoolObject poolObjectScript;

        public ObjectInstance(GameObject objectInstance)
        {
            gameObject = objectInstance;
            transform = gameObject.transform;
            gameObject.SetActive(false);

            if (gameObject.GetComponent<PoolObject>())
            {
                hasBoolObjectComponent = true;
                poolObjectScript = gameObject.GetComponent<PoolObject>();
            }
        }

        public void Reuse(Vector3 position, Quaternion rotation)
        {
            if (hasBoolObjectComponent)
            {
                poolObjectScript.OnObjectReuse();
            }
            gameObject.SetActive(true);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
        }

        public void ReuseOnline(Vector3 position, Quaternion rotation)
        {
            if (hasBoolObjectComponent)
            {
                poolObjectScript.OnObjectReuse();
            }

            gameObject.SetActive(true);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
        }

        public void SetParent(Transform parent)
        {
            transform.parent = parent;
        }
    }
}
