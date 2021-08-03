using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes.Internals.Extensions;

namespace NServiceBus.Automatonymous.Generators
{
    public class SagaGenerator
    {
        private readonly Type _saga;
        private readonly Type _baseSaga;

        public SagaGenerator(Type saga)
        {
            _saga = saga;
            _baseSaga = typeof(NServiceBusSaga<,>).MakeGenericType(_saga, _saga.BaseType!.GenericTypeArguments[0]);
        }

        public Type Generate()
        {
            var typeName = $"{_saga.Name}NServiceBusSaga";
            var assemblyName = new AssemblyName($"{typeName}Assembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            
            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{typeName}Module");
            const TypeAttributes typeAttributes = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit;

            var events = GetEvents();

            var interfaces = new List<Type>();
            interfaces.AddRange(events
                .Where(x => x.HasAttribute<StartSagaAttribute>())
                .Select(x => typeof(IAmStartedByMessages<>).MakeGenericType(x.PropertyType.GenericTypeArguments[0]))
                .ToArray());
            
            interfaces.AddRange(events
                .Where(x => !x.HasAttribute<StartSagaAttribute>())
                .Select(x => typeof(IHandleMessages<>).MakeGenericType(x.PropertyType.GenericTypeArguments[0]))
                .ToArray());
            
            var typeBuilder = moduleBuilder.DefineType(typeName, typeAttributes, _baseSaga, interfaces.ToArray());
            
            CreateConstructor(typeBuilder);

            foreach (var @event in events)
            {
                ImplementHandle(typeBuilder, @event);
            }
            
            return typeBuilder.CreateType()!;
        }

        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            var construct = _baseSaga.GetConstructors(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance)[0];
            var defineConstructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig |
                                                                  MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.Standard, new[] {_saga});
            var generator =  defineConstructor.GetILGenerator();
            
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, construct);
            generator.Emit(OpCodes.Ret);
        }

        private List<PropertyInfo> GetEvents() 
            => _saga.GetProperties().Where(x => x.PropertyType.IsInterface
                                                && x.PropertyType.IsGenericType
                                                && x.PropertyType.HasInterface(typeof(Event))).ToList();
        
        private void ImplementHandle(TypeBuilder typeBuilder, PropertyInfo propertyInfo)
        {
            var execute = _baseSaga.GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance)!;
            var methodBuilder = typeBuilder.DefineMethod(nameof(IHandleMessages<object>.Handle), 
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                typeof(Task), new[] {propertyInfo.PropertyType.GenericTypeArguments[0], typeof(IMessageHandlerContext)});

            var generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Ldarg_1);
            generator.EmitCall(OpCodes.Call, execute, null);
            generator.Emit(OpCodes.Ret);
        }
    }
}