namespace LunaECS.Utilities.ECS;

public abstract class System(IFilter filter)
{
  public bool Destroyed { get; private set; } = false;
  public bool Enabled { get; set; } = true;
  public IFilter Filter { get; set; } = filter;
  public int Priority { get; set; } = 0;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  //Supressed because it gets assigned to at object add into world
  public World World { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public List<int> Entities { get; set; } = [];
  public void Destroy() => Destroyed = true;

  public void Update(float deltaTime)
  {
    PreProcess(deltaTime);
    foreach (var entity in Entities.ToList())
    {
      if (Destroyed || !Enabled) break;
      if (World.IsEntityDestroyed(entity)) continue;
      Process(entity, deltaTime);
    }
    if (Destroyed || !Enabled) return;
    PostProcess(deltaTime);
  }
  public abstract void Init();
  public abstract void OnAdd(int entity);
  public abstract void OnRemove(int entity);
  public abstract void PreProcess(float deltaTime);
  public abstract void Process(int entity, float deltaTime);
  public abstract void PostProcess(float deltaTime);
}