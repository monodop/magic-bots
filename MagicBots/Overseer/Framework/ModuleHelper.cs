using MagicBots.Overseer.Framework.Cooldowns;
using MagicBots.Overseer.Framework.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MagicBots.Overseer.Framework
{
    public static class ModuleHelper
    {
        public static bool IsLoadableModule(TypeInfo info)
        {
            return info.IsClass && !info.IsAbstract && info.IsSubclassOf(typeof(OverseerModule));
        }

        public static bool IsLoadableCooldown(TypeInfo info)
        {
            return info.IsClass && !info.IsAbstract && info
                .GetInterfaces().Any(x =>
                    x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IBaseCooldownProcessor<>));
        }

        public static bool IsLoadableTrigger(TypeInfo info)
        {
            return info.IsClass && !info.IsAbstract && info
                .GetInterfaces().Any(x =>
                    x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IBaseTriggerProcessor<>));
        }

        public static bool IsTriggerableModule(TypeInfo info)
        {
            return info.DeclaredMethods.Any(IsTriggerableMethod);
        }

        public static bool IsTriggerableMethod(MethodInfo info)
        {
            return info.GetCustomAttribute<TriggerAttribute>() != null;
        }

        public static IList<Type> GetServiceTypes(IEnumerable<TypeInfo> types)
        {
            string methodName = "GetServices";
            var foundTypes = new HashSet<Type>();
            
            foreach(Type type in types)
            {
                MethodInfo? info = type.GetMethod(
                    methodName, 
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                if (info == null)
                    continue;
                
                IList<Type> services = (IList<Type>) info.Invoke(null, new object[] { } )!;

                foreach(Type service in services)
                {
                    foundTypes.Add(service);
                }
            }

            return foundTypes.ToList();
        }
    }
}