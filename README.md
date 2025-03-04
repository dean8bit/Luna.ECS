## Luna.ECS

A simple no frills entity component system

### Example usage

    var world = new World();
    world.AddSystem(new TestSystem());
    
    var entity = world.CreateEntity();
    world.AddComponent(entity, new TestComponent())
    
    var typedComponents = world.GetComponent<TestComponent>(entity)
    var allComponents = world.GetComponent(entity)
    
    world.Update(deltaTime);
    world.RemoveEntity(entity);

---

-- Used as part of a personal game engine. Extracted this module out as an MIT licensed tool.
