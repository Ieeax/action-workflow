# Introduction
A library for creating workflows based on/splited into multiple actions. The project itself is currently based on .NET Standard 2.0.

# Getting Started
A workflow consists of multiple actions which are each represented as their own class and executed for/in a specific context (`T`). To create such an action, the class needs to inherit from the interface `IAction<T>` as shown underneath:
```csharp
public class CustomAction : IAction<ContextObject>
{
    public Task ExecuteAsync(ContextObject context)
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
ActionSequenceExecutionResult result = await sequence.ExecuteAsync(context);
```

The library also supports dependency-injection (through constructor), the possibility to export and import results from other actions and with it the dependency on other actions.
Further documentation will come in the future.

# Issues
When encountering any bugs/problems feel free to create an issue.
