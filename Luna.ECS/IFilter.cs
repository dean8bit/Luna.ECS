namespace LunaECS.Utilities.ECS;

public interface IFilter
{
  public bool Matches(List<IComponent> components);
}
