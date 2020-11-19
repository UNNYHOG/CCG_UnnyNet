using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class ObjectsPool
{
    private class LoadedObject
    {
        protected readonly Object _originalObject;
        private readonly List<Object> _instantiatedObjects;

        public static LoadedObject Instantiate(Object originalObject)
        {
            switch (originalObject)
            {
                case Sprite _:
                    return new LoadedSprite(originalObject);
                case GameObject _:
                    return new LoadedGameObject(originalObject);
                default:
                    return new LoadedObject(originalObject);
            }
        }

        public LoadedObject(Object originalObject)
        {
            _originalObject = originalObject;
            _instantiatedObjects = new List<Object>();
        }

        public virtual Object GetInstantiatedObject()
        {
            Object newObj;
            if (_instantiatedObjects.Count > 0)
            {
                int lastIndex = _instantiatedObjects.Count - 1;
                newObj = _instantiatedObjects[lastIndex];
                _instantiatedObjects.RemoveAt(lastIndex);
            }
            else
            {
                newObj = Object.Instantiate(_originalObject);
            }

            _hashToLoadedObjects.Add(GetUniqueObjectId(newObj), this);
            return newObj;
        }

        public virtual void ReturnObject(Object obj)
        {
            _instantiatedObjects.Add(obj);
        }
    }

    private class LoadedGameObject : LoadedObject
    {
        public LoadedGameObject(Object originalObject) : base(originalObject)
        {
        }

        public override void ReturnObject(Object obj)
        {
            base.ReturnObject(obj);
            (obj as GameObject)?.transform.SetParent(m_PoolHolder);
        }
    }

    private class LoadedSprite : LoadedObject
    {
        public LoadedSprite(Object originalObject) : base(originalObject)
        {
        }

        public override Object GetInstantiatedObject()
        {
            return _originalObject;
        }
    }

    private static readonly Transform m_PoolHolder;

    private static readonly Dictionary<string, LoadedObject> _loadedObjects = new Dictionary<string, LoadedObject>(16);
    private static readonly Dictionary<string, List<Action<Object>>> _loadingQueue =
        new Dictionary<string, List<Action<Object>>>(16);
    private static readonly Dictionary<int, LoadedObject> _hashToLoadedObjects = new Dictionary<int, LoadedObject>(16);

    static ObjectsPool()
    {
        GameObject parent = new GameObject("Pool");
        m_PoolHolder = parent.transform;
        Object.DontDestroyOnLoad(parent);
        parent.SetActive(false);
    }

    public static void GetSprite(string name, Action<Sprite> callback)
    {
        CheckAndPrepareObject<Sprite>(name, o => callback(o as Sprite));
    }

    public static void GetInstantiatedObject(string name, Action<Object> callback)
    {
        CheckAndPrepareObject<Object>(name, callback);
    }

    public static void PreloadObject(string name, Action callback)
    {
        CheckAndPrepareObject<Object>(name, obj =>
        {
            ReturnObject(obj);
            callback?.Invoke();
        });
    }

    private static void CheckAndPrepareObject<T>(string name, Action<Object> callback) where T : Object
    {
        if (_loadedObjects.TryGetValue(name, out var value))
        {
            callback(value?.GetInstantiatedObject());
        }
        else
        {
            if (_loadingQueue.TryGetValue(name, out var queue))
            {
                queue?.Add(callback);
            }
            else
            {
                var newQueue = new List<Action<Object>> {callback};
                _loadingQueue.Add(name, newQueue);

                Addressables.LoadAssetAsync<T>(name).Completed += result =>
                {
                    if (result.Status == AsyncOperationStatus.Succeeded)
                    {
                        var loadedObject = LoadedObject.Instantiate(result.Result);
                        _loadedObjects.Add(name, loadedObject);
                        foreach (var action in newQueue)
                        {
                            var instantiatedObject = loadedObject?.GetInstantiatedObject();
                            action(instantiatedObject);
                        }

                        _loadingQueue.Remove(name);
                    }
                    else
                    {
                        Debug.LogError("Couldn't load asset by name " + name);
                        foreach (var action in newQueue)
                            action(null);
                    }
                };
            }
        }
    }

    public static void ReturnObject(Object obj)
    {
        if (obj == null) return;

        if (!_hashToLoadedObjects.TryGetValue(GetUniqueObjectId(obj), out var loadedObject)) return;

        loadedObject.ReturnObject(obj);
        _hashToLoadedObjects.Remove(GetUniqueObjectId(obj));
    }

    public static void ReturnAllObjects()
    {
        //TODO we might want to save some objects here
        _hashToLoadedObjects.Clear();
    }

    private static int GetUniqueObjectId(Object obj)
    {
        return obj.GetInstanceID();
    }
}
