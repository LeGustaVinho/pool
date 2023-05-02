# Object Pool

### Intent

Improve performance and memory use by reusing objects from a fixed pool instead of allocating and freeing them individually.
Motivation

We're working on the visual effects for our game. When the hero casts a spell, we want a shimmer of sparkles to burst across the screen. This calls for a particle system, an engine that spawns little sparkly graphics and animates them until they wink out of existence.

Since a single wave of the wand could cause hundreds of particles to be spawned, our system needs to be able to create them very quickly. More importantly, we need to make sure that creating and destroying these particles doesn’t cause memory fragmentation.

### The Pattern

Define a pool class that maintains a collection of reusable objects. Each object supports an “in use” query to tell if it is currently “alive”. When the pool is initialized, it creates the entire collection of objects up front (usually in a single contiguous allocation) and initializes them all to the “not in use” state.

When you want a new object, ask the pool for one. It finds an available object, initializes it to “in use”, and returns it. When the object is no longer needed, it is set back to the “not in use” state. This way, objects can be freely created and destroyed without needing to allocate memory or other resources.

### When to Use It

This pattern is used widely in games for obvious things like game entities and visual effects, but it is also used for less visible data structures such as currently playing sounds. Use Object Pool when:

- You need to frequently create and destroy objects.
- Objects are similar in size.
- Allocating objects on the heap is slow or could lead to memory fragmentation.
- Each object encapsulates a resource such as a database or network connection that is expensive to acquire and could be reused.

Functionalities:

- Uses the same API for Instantiate and Destroy to manage GameObjects/Prefabs
- Lets you manage pure C# objects in pool
- Notifies the pool object when it is being created and recycled
- Easy to use, no initialization or configuration
- Allows you to pre-populate the pool

Dependencies:

- [Legendary Tools - Common](https://github.com/LeGustaVinho/legendary-tools-common "Legendary Tools - Common")

### How to use

#### Instantiating Objects 

```csharp
GameObject newPoolableGO = Pool.Instantiate(prefab);
GameObject newPoolableGO = Pool.Instantiate(prefab, parent);
GameObject newPoolableGO = Pool.Instantiate(prefab, position, rotation);
GameObject newPoolableGO = Pool.Instantiate(prefab, position, rotation, parent);
```
#### Destroying Objects 
```csharp
Pool.Destroy(newPoolableGO);
```

#### Pre-Populating Pool
```csharp
GameObject notYetPooled = Instantiate(prefab); //Normal Unity Instantiate
Pool.AddInstance(newPoolableGO);
```

#### Pool Clean Up (Literally destroying and freeing up memory)
```csharp
Pool.ClearPool(poolableGO);
```

#### Receving Pool events on Poolable Objects
All components of that prefab that implement the IPoolable interface will receive messages from the Pool.
```csharp
    public class GameObjectPoolReference : MonoBehaviour, IPoolable
    {        
        public virtual void OnConstruct()
        {
			//Called when the Pool literally instantiates this GameObject/Prefab, occurs before Start()
        }

        public virtual void OnCreate()
        {
			//Called when the Pool activates this instance, it occurs before the OnEnable()
        }

        public virtual void OnRecycle()
        {
			//Called when the Pool deactivates this instance, it occurs before OnDisable()
        }
    }
```

