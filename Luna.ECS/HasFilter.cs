namespace LunaECS.Utilities.ECS;

public class HasFilter(params Type[] has) : IFilter
{
  public Type[] Has { get; set; } = has;

  public bool Matches(List<IComponent> components) => Has.All(type => components.Any(component => component.GetType() == type));
}