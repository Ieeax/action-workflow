# Introduction
A library for creating workflows based on/splited into multiple actions. The project itself is currently based on .NET Standard 2.0.

# Getting Started

## Basic Usage
A workflow consists of multiple actions which are each represented as their own class and executed for/in a specific context (`T`). To create such an action, the class needs to inherit from the interface `IAction<T>` as shown underneath:
```csharp
public class CustomAction : IAction<ContextObject>
{
    public Task ExecuteAsync(ContextObject context, CancellationToken cancellationToken)
    {
        // Implement your logic here
    }
}
```

To associate different actions with each other, we then need to create an `ActionSequenceFactory<T>` and pass all types of the actions for our workflow:
```csharp
var seqFactory = ActionSequenceFactory<ContextObject>.CreateFactory(
    new List<Type>()
    {
        typeof(CustomAction),
        // ...
    })

// We can also use the corresponding builder to create the factory
var builder = new ActionSequenceFactoryBuilder<ContextObject>();
builder.AddAction<CustomAction>();
// ...

seqFactory = builder.ToFactory();
```

This factory allows us to create as many instances of your workflow as we wish, completely independent from each other:
```csharp
var sequence = seqFactory.Create();
```

Finally, to execute the created workflow, we need to call the `ExecuteAsync` method on it:
```csharp
// Create some context object which gets passed to all actions
var context = new ContextObject();

// Execute the workflow/action-sequence
ActionSequenceExecutionResult result = await sequence.ExecuteAsync(context, CancellationToken.None);
```

## Dependency Injection
Actions can make use of the built-in dependency-injection through their constructor:
```csharp
public CustomAction(ISomeService someService)
{
    _someService = someService;
}
```

To provide your own services, an `IServiceProvider` can be passed when creating a `ActionSequence<T>` from a `ActionSequenceFactory<T>`:
```csharp
var sequence = seqFactory.Create(serviceProvider);
```

> **Note**: There is currently one built-in service, named `IActionContext`. The service is scoped to the current action and provides the ability to access functions (e.g. export of objects) and informations (e.g. name or description) about it.

## Exports/Imports
As noted above, actions can export objects (using the `IActionContext` service) and also import these from previous actions.
Here's a simple example of the two types of exports:
```csharp
public class CustomAction : IAction<ContextObject>
{
    private readonly IActionContext _context;

    public CustomAction(IActionContext context)
    {
        _context = context;
    }
    
    public Task ExecuteAsync(ContextObject context, CancellationToken cancellationToken)
    {
        // Export the object as default export (= without name)
        // -> Each type can only be exported once as default export
        _context.Export(new SomeActionExport(/* ... */));

        // Export the object as named export
        // -> Combination of type and name has to be unique
        _context.Export(new SomeActionExport(/* ... */), "ExportWithName");
        
        return Task.CompletedTask;
    }
}
```

Now, to import `SomeActionExport` from another action, we need to specify the import in the constructor and add the `FromImport` attribute before the parameter. This attribute indicates (to the underlying dependency-injection) that we want to import this object from a previous action:
```csharp
public AnotherCustomAction([FromImport] SomeActionExport import, [FromImport("ExportWithName")] SomeActionExport namedImport)
{
    // ...
}
```
Imports also introduces the dependency of different actions on each other. Actions without imports will be executed first, then the next ones which are possible with the exports of the last action(s), etc.

> **Note**: Circular dependencies between multiple actions are not supported! If the workflow/action-sequence cannot be fully executed, `ActionSequenceExecutionResult.Partial` is returned.

# Issues
When encountering any bugs/problems feel free to create an issue.