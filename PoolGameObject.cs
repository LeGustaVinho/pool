using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LegendaryTools
{
    public class PoolGameObject : PoolObject<GameObject>
    {
        private readonly GameObject original;

        public PoolGameObject(GameObject original) : base()
        {
            this.original = original;
        }

        protected override GameObject NewObject()
        {
            if (original != null)
            {
                GameObject obj = GameObject.Instantiate(original);
                obj.name = original.name + " # " + (ActiveInstances.Count + InactiveInstances.Count);

                GameObjectPoolReference reference = obj.GetComponent<GameObjectPoolReference>();
                if (reference == null)
                {
                    reference = obj.AddComponent<GameObjectPoolReference>();
                    reference.PrefabID = original.GetHashCode();
                }

                NotifyOnConstruct(obj);
                return obj;
            }
            
            throw new Exception("[PoolGameObject] -> Original prefab cannot be null.");
        }

        public override GameObject CreateAs()
        {
            GameObject obj =  base.CreateAs();
            obj.SetActive(true);
            NotifyOnCreate(obj);
            return obj;
        }

        public GameObject Create(Transform parent)
        {
            GameObject obj = CreateAs();
            obj.transform.SetParent(parent);
            return obj;
        }
        
        public GameObject Create(Vector3 position, Quaternion rotation)
        {
            GameObject obj = CreateAs();
            Transform objTransform = obj.transform;
            objTransform.position = position;
            objTransform.rotation = rotation;
            return obj;
        }
        
        public GameObject Create(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject obj = CreateAs();
            Transform objTransform = obj.transform;
            objTransform.SetParent(parent);
            objTransform.position = position;
            objTransform.rotation = rotation;
            return obj;
        }

        public override void Recycle(GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(null);
            NotifyOnRecycle(instance);
            
            base.Recycle(instance);
        }
        
        public override void RecycleAllActive()
        {
            for (int i = ActiveInstances.Count - 1; i >= 0; i--)
            {
                Recycle(ActiveInstances[i]);
            }
        }

        /// <summary>
        /// It deallocates all instances (active and inactive) of this Pool, literally destroying the objects.
        /// </summary>
        public override void Clear()
        {
            List<GameObject> instances = new List<GameObject>(ActiveInstances.Count + InactiveInstances.Count);
            instances.AddRange(ActiveInstances);
            instances.AddRange(InactiveInstances);
            
            foreach (GameObject t in instances)
            {
                if (t != null)
                {
                    Object.Destroy(t);
                }
            }
            
            base.Clear();
        }

        private void NotifyOnConstruct(GameObject obj)
        {
            Component[] comps = obj.GetComponents<Component>();
            foreach (Component t in comps)
            {
                if (t is IPoolable poolable)
                {
                    poolable.OnConstruct();
                }
            }
        }

        private void NotifyOnCreate(GameObject obj)
        {
            Component[] comps = obj.GetComponents<Component>();
            foreach (Component t in comps)
            {
                if (t is IPoolable poolable)
                {
                    poolable.OnCreate();
                }
            }
        }

        private void NotifyOnRecycle(GameObject obj)
        {
            Component[] comps = obj.GetComponents<Component>();
            foreach (Component t in comps)
            {
                if (t is IPoolable poolable)
                {
                    poolable.OnRecycle();
                }
            }
        }
    }
}