using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using GreenPipes.Internals.Extensions;

namespace NServiceBus.Automatonymous.Extensions
{
    internal static class TypeExtensions
    {
        public static bool ClosesType(this Type type, Type openType, [NotNullWhen(true)]out Type[]? arguments)
        {
            if (ClosesType(type, openType, out Type? closedType))
            {
                arguments = closedType.GetGenericArguments().Where(x => !x.IsGenericParameter).ToArray();
                return true;
            }

            arguments = default;
            return false;
        }

        private static bool ClosesType(this Type type, Type openType, [NotNullWhen(true)]out Type? closedType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (openType == null)
                throw new ArgumentNullException(nameof(openType));

            if (!openType.IsOpenGeneric())
                throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

            if (openType.GetTypeInfo().IsInterface)
            {
                if (!openType.IsOpenGeneric())
                    throw new ArgumentException("The interface type must be an open generic interface: " + openType.Name);

                var interfaceType = type.GetInterface(openType);
                if (interfaceType == null)
                {
                    closedType = default;
                    return false;
                }

                var typeInfo = interfaceType.GetTypeInfo();
                if (!typeInfo.IsGenericTypeDefinition && !typeInfo.ContainsGenericParameters)
                {
                    closedType = typeInfo;
                    return true;
                }

                closedType = default;
                return false;
            }

            var baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                var baseTypeInfo = baseType.GetTypeInfo();
                if (baseTypeInfo.IsGenericType && baseTypeInfo.GetGenericTypeDefinition() == openType)
                {
                    if (!baseTypeInfo.IsGenericTypeDefinition && !baseTypeInfo.ContainsGenericParameters)
                    {
                        closedType = baseTypeInfo;
                        return true;
                    }

                    closedType = default;
                    return false;
                }

                if (!baseTypeInfo.IsGenericType && baseType == openType)
                {
                    closedType = baseTypeInfo;
                    return true;
                }

                baseType = baseTypeInfo.BaseType;
            }

            closedType = default;
            return false;
        }
    }
}