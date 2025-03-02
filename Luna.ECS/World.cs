namespace LunaECS.Utilities.ECS;

public class World
{
  private int _entityCounter = 0;
  private int EntityCounter { get { return ++_entityCounter; } }
  private List<int> DestroyedEntities { get; set; } = [];
  public Dictionary<int, List<IComponent>> Entities { get; private set; } = [];
  public List<System> Systems { get; private set; } = [];

  private void RemoveDestroyedEntities()
  {
    foreach (var entity in DestroyedEntities)
    {
      Entities.Remove(entity);
      foreach (var system in Systems)
      {
        system.Entities.Remove(entity);
      }
    }
    DestroyedEntities = new List<int>();
  }

  private void UpdateEntity(int entity)
  {
    foreach (var system in Systems)
    {
      UpdateSystemEntity(system, entity);
    }
  }

  private void UpdateSystem(System system)
  {
    foreach (var entity in Entities)
    {
      UpdateSystemEntity(system, entity.Key);
    }
  }

  private void UpdateSystemEntity(System system, int entity)
  {
    var matches = system.Filter.Matches(Entities[entity]);
    var contains = system.Entities.Contains(entity);
    if (!contains && matches) // Entity needs to be registered in system
    {
      system.Entities.Add(entity);
      system.OnAdd(entity);
    }
    else if (contains && !matches) // Entity is no longer needed in system
    {
      system.Entities.Remove(entity);
      system.OnRemove(entity);
    }
  }

  public bool AddComponent(int entity, IComponent component)
  {
    if (!Entities.TryGetValue(entity, out List<IComponent>? value)) return false;
    value.Add(component);
    UpdateEntity(entity);
    return true;
  }

  public List<System> GetEntitySystems(int entity)
  {
    List<System> entitySystems = [];
    foreach (var system in Systems)
    {
      if (system.Entities.Contains(entity))
      {
        entitySystems.Add(system);
      }
    }
    return entitySystems;
  }

  public void RemoveSystem(System system) => this.Systems.Remove(system);

  public void AddSystem(System system)
  {
    if (Systems.Contains(system)) return;
    Systems.Add(system);
    system.World = this;
    UpdateSystem(system);
    system.Init();
  }

  public int CreateEntity()
  {
    int id = EntityCounter;
    Entities.Add(id, []);
    return id;
  }

  public List<IComponent> GetComponent(int entity)
  {
    if (!Entities.TryGetValue(entity, out var components)) return [];
    return [.. components];
  }

  public List<T> GetComponent<T>(int entity) where T : IComponent
  {
    if (!Entities.TryGetValue(entity, out List<IComponent>? value)) return [];
    return [.. value.OfType<T>()];
  }

  public bool IsEntityDestroyed(int entity) => DestroyedEntities.Contains(entity);

  public void RemoveComponent(int entity, IComponent component)
  {
    if (!Entities.TryGetValue(entity, out List<IComponent>? value)) return;
    value.Remove(component);
    UpdateEntity(entity);
  }

  public void RemoveEntity(int entity)
  {
    if (Entities.Remove(entity))
      DestroyedEntities.Add(entity);
  }

  public void Update(float deltaTime)
  {
    RemoveDestroyedEntities();
    foreach (var system in Systems.ToList())
    {
      if (system.Destroyed)
      {
        Systems.Remove(system);
        continue;
      }
      if (!system.Enabled) continue;
      system.Update(deltaTime);
    }
  }
}