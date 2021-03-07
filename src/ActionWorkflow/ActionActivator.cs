using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace ActionWorkflow
{
    public static class ActionActivator
    {
        private static readonly Dictionary<Type, ActionInfo> _cachedActionInfos = new Dictionary<Type, ActionInfo>();
        private static readonly object _cachedActionInfosLock = new object();

        public static ActionInfo GetActionInfo(Type actionType)
        {
            if (actionType == null)
            {
                throw new ArgumentNullException(nameof(actionType));
            }

            lock (_cachedActionInfosLock)
            {
                if (_cachedActionInfos.TryGetValue(actionType, out var actionInfo))
                {
                    return actionInfo;
                }

                if (actionType.IsAbstract)
                {
                    throw new InvalidOperationException($"An action cannot be an interface or abstract class.");
                }

                // Ensure type inherits from action interface
                var inheritsFromInterface = false;
                foreach (var curInterfaceType in actionType.GetInterfaces())
                {
                    if (curInterfaceType.IsGenericType
                        && curInterfaceType.GetGenericTypeDefinition() == typeof(IAction<>))
                    {
                        inheritsFromInterface = true;
                        break;
                    }
                }

                if (!inheritsFromInterface)
                {
                    throw new InvalidOperationException($"Type \"{actionType.FullName}\" does not inherit from \"{typeof(IAction<>).FullName}\".");
                }

                if (!TryGetActionConstructor(actionType, out var constructorInfo))
                {
                    throw new InvalidOperationException($"No matching constructor for type \"{actionType.FullName}\" was found.");
                }

                var metadataAttr = actionType.GetCustomAttribute<ActionAttribute>(false);

                var imports = GetActionImportsFromConstructor(constructorInfo);
                actionInfo = new ActionInfo(
                    actionType,
                    metadataAttr?.FriendlyName,
                    metadataAttr?.Description,
                    constructorInfo,
                    imports);

                _cachedActionInfos.Add(actionType, actionInfo);

                return actionInfo;
            }
        }

        public static object CreateInstance(ActionInfo actionInfo, IServiceProvider serviceProvider, IExportProvider exportProvider)
        {
            if (actionInfo == null) throw new ArgumentNullException(nameof(actionInfo));
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            if (exportProvider == null) throw new ArgumentNullException(nameof(exportProvider));

            var parameters = actionInfo.Constructor.GetParameters();
            var parameterValues = new object[parameters.Length];
            var parameterValuesSet = new bool[parameters.Length];

            foreach (var curImport in actionInfo.Imports)
            {
                var importValue = exportProvider.GetExport(curImport.Type, curImport.Name);
                if (importValue == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Unable to resolve import for type \"{0}\"{1} while attempting to activate \"{2}\".", 
                            curImport.Type.FullName, 
                            curImport.Name == null ? string.Empty : $" and name \"{curImport.Name}\"",
                            actionInfo.ActionType.FullName));
                }

                parameterValues[curImport.ConstructorIndex] = importValue;
                parameterValuesSet[curImport.ConstructorIndex] = true;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (!parameterValuesSet[i])
                {
                    var value = serviceProvider.GetService(parameters[i].ParameterType);
                    if (value == null)
                    {
                        if (!TryGetParameterDefaultValue(parameters[i], out var defaultValue))
                        {
                            throw new InvalidOperationException($"Unable to resolve service for type \"{parameters[i].ParameterType}\" while attempting to activate \"{actionInfo.ActionType.FullName}\".");
                        }

                        parameterValues[i] = defaultValue;
                    }
                    else
                    {
                        parameterValues[i] = value;
                    }
                }
            }

            try
            {
                return actionInfo.Constructor.Invoke(parameterValues);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                // The above line will always throw, but the compiler requires we throw explicitly.
                throw;
            }
        }
        
        private static bool TryGetActionConstructor(Type instanceType, out ConstructorInfo constructor)
        {
            constructor = null;
            
            if (!instanceType.GetTypeInfo().IsAbstract)
            {
                constructor = instanceType
                    .GetTypeInfo()
                    .DeclaredConstructors
                    .FirstOrDefault(c => !c.IsStatic && c.IsPublic);
            }

            return constructor != null;
        }

        private static List<ActionImportInfo> GetActionImportsFromConstructor(ConstructorInfo constructor)
        {
            var result = new List<ActionImportInfo>();
            var parameters = constructor.GetParameters();

            int parameterIndex = 0;
            foreach (var curParameter in parameters)
            {
                if (curParameter.IsDefined(typeof(FromImportAttribute), false))
                {
                    var attribute = curParameter.GetCustomAttribute<FromImportAttribute>(false);

                    result.Add(
                        new ActionImportInfo(curParameter.ParameterType, attribute.Name, parameterIndex));
                }

                parameterIndex++;
            }

            return result;
        }

        private static bool TryGetParameterDefaultValue(ParameterInfo parameter, out object defaultValue)
        {
            defaultValue = null;

            if (!parameter.HasDefaultValue)
            {
                return false;
            }

            defaultValue = parameter.DefaultValue;

            // Workaround for https://github.com/dotnet/runtime/issues/18599
            if (defaultValue == null && parameter.ParameterType.IsValueType)
            {
                defaultValue = Activator.CreateInstance(parameter.ParameterType);
            }

            // Handle nullable enums
            if (defaultValue != null &&
                parameter.ParameterType.IsGenericType &&
                parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type underlyingType = Nullable.GetUnderlyingType(parameter.ParameterType);
                if (underlyingType != null && underlyingType.IsEnum)
                {
                    defaultValue = Enum.ToObject(underlyingType, defaultValue);
                }
            }

            return true;
        }
    }
}