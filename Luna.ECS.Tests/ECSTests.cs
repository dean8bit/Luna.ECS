using System.Linq.Expressions;
using Luna.ECS;

namespace Luna.ECS.Tests;

[TestClass]
public sealed class ECSTests
{
    [TestMethod]
    public void ECSTest1()
    {
        var ecs = new World();
        var system = new System1(new HasFilter(new Type[] { typeof(Component1) }));
        ecs.AddSystem(system);
        var entity = ecs.CreateEntity();
        var component = new Component1();
        ecs.AddComponent(entity, component);
        var findComponent = ecs.GetComponent<Component1>(entity).First();
        Assert.AreEqual(findComponent.Value, 1234);
        ecs.Update(0.1f);
        findComponent = ecs.GetComponent<Component1>(entity).First();
        Assert.AreEqual(findComponent.Value, 4321);
        Assert.IsTrue(system.DidAdd);
        Assert.IsTrue(system.DidInit);
        Assert.IsTrue(system.DidPreProcess);
        Assert.IsTrue(system.DidProcess);
        Assert.IsTrue(system.DidPostProcess);
        ecs.RemoveEntity(entity);
        Assert.IsFalse(system.DidRemove);

    }

    public class Component1 : IComponent
    {
        public int Value { get; set; } = 1234;
    }

    public class System1 : System
    {
        public bool DidAdd = false;
        public bool DidInit = false;
        public bool DidRemove = false;
        public bool DidPostProcess = false;
        public bool DidPreProcess = false;
        public bool DidProcess = false;
        public System1(IFilter filter) : base(filter)
        {
        }

        public override void Init()
        {
            DidInit = true;
        }

        public override void OnAdd(int entity)
        {
            DidAdd = true;
        }

        public override void OnRemove(int entity)
        {
            DidRemove = true;
        }

        public override void PostProcess(float deltaTime)
        {
            DidPostProcess = true;
        }

        public override void PreProcess(float deltaTime)
        {
            DidPreProcess = true;
        }

        public override void Process(int entity, float deltaTime)
        {
            DidProcess = true;
            var component = World.GetComponent<Component1>(entity).First();
            component.Value = 4321;
        }
    }
}
