using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActionWorkflow
{
    public class ActionSequenceFactory<T>
    {
        private static readonly Dictionary<Type, ActionInfo> _cachedActionInfos = new Dictionary<Type, ActionInfo>();
        private readonly IList<ActionInfo> _actionInfos;

        private ActionSequenceFactory(IList<ActionInfo> actionInfos)
        {
            _actionInfos = actionInfos;
        }

        private static bool TryGetActionConstructor(Type instanceType, out ConstructorInfo constructor)
        {
            var seenPreferred = false;
            ConstructorInfo bestConstructor = null;

            if (!instanceType.GetTypeInfo().IsAbstract)
            {
                foreach (var curConstructor in instanceType
                    .GetTypeInfo()
                    .DeclaredConstructors
                    .Where(c => !c.IsStatic && c.IsPublic))
                {
                    var isPreferred = curConstructor.IsDefined(typeof(ActivatorUtilitiesConstructorAttribute), false);

                    if (isPreferred)
                    {
                        if (seenPreferred)
                        {
                            throw new InvalidOperationException($"Multiple constructors were marked with {nameof(ActivatorUtilitiesConstructorAttribute)}.");
                        }

                        bestConstructor = curConstructor;
                        seenPreferred = true;
                    }
                }
            }

            constructor = bestConstructor;
            return constructor != null;
        }

        private static List<Type> GetTypesToImport(ConstructorInfo constructor)
        {
            var result = new List<Type>();
            var parameters = constructor.GetParameters();

            foreach (var curParameter in parameters)
            {
                var paramType = curParameter.ParameterType.GetTypeInfo();

                if (paramType.IsDefined(typeof(ActionExportAttribute), false))
                {
                    result.Add(paramType);
                }
            }

            return result;
        }

        public static ActionInfo GetActionInfo(Type actionType)
        {
            if (actionType == null)
            {
                throw new ArgumentNullException(nameof(actionType));
            }

            if (_cachedActionInfos.TryGetValue(actionType, out var actionInfo))
            {
                return actionInfo;
            }

            if (!typeof(IAction<T>).IsAssignableFrom(actionType))
            {
                throw new InvalidOperationException($"Type \"{actionType.FullName}\" does not inherit from \"{typeof(IAction<T>).FullName}\".");
            }

            if (!TryGetActionConstructor(actionType, out var constructorInfo))
            {
                throw new InvalidOperationException($"No constructor for type \"{actionType.FullName}\" was found.");
            }

            var metadataAttr = actionType.GetCustomAttribute<ActionAttribute>(false);

            var imports = GetTypesToImport(constructorInfo);
            actionInfo = new ActionInfo(
                actionType, 
                metadataAttr?.FriendlyName, 
                metadataAttr?.Description, 
                imports);

            _cachedActionInfos.Add(actionType, actionInfo);

            return actionInfo;
        }

        public static ActionSequenceFactory<T> CreateFactory(ICollection<Type> actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }

            var actionInfos = new List<ActionInfo>(actions.Count);

            foreach (var curActionType in actions)
            {
                actionInfos.Add(GetActionInfo(curActionType));
            }

            return new ActionSequenceFactory<T>(actionInfos);
        }

        public ActionSequence<T> Create(IExportProvider exportProvider = null)
            => new ActionSequence<T>(_actionInfos, exportProvider);

        public ActionSequence<T> Create(IServiceProvider serviceProvider, IExportProvider exportProvider = null)
            => new ActionSequence<T>(_actionInfos, serviceProvider, exportProvider);
    }
}