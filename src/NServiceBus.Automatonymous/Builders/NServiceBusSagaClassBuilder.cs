using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceBus.Automatonymous.Builders
{
    internal class NServiceBusSagaClassBuilder
    {
        private readonly HashSet<string> _using = new HashSet<string>();
        public NServiceBusSagaClassBuilder AddUsing(string @namespace)
        {
            _using.Add(@namespace);
            return this;
        }
        
        public NServiceBusSagaClassBuilder AddUsing(IEnumerable<string>  namespaces)
        {
            foreach (var @namespace in namespaces)
            {
                _using.Add(@namespace);    
            }
            
            return this;
        }

        private string? _namespace;

        public NServiceBusSagaClassBuilder SetNamespace(string @namespace)
        {
            _namespace = @namespace;
            return this;
        }

        public string Name => _name ?? string.Empty;
        
        private string? _name;
        public NServiceBusSagaClassBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        private string? _baseType;
        public NServiceBusSagaClassBuilder SetBaseType(string @baseTye)
        {
            _baseType = baseTye;
            return this;
        }
        
        private readonly HashSet<string> _interfaces = new HashSet<string>();
        public NServiceBusSagaClassBuilder AddInterfaces(IEnumerable<string> interfaces)
        {
            foreach (var @interface in interfaces)
            {
                _interfaces.Add(@interface);
            }
            return this;
        }


        private readonly List<string> _methods = new List<string>();

        public NServiceBusSagaClassBuilder AddMethods(IEnumerable<string> methods)
        {
            _methods.AddRange(methods);
            return this;
        }
        
        public NServiceBusSagaClassBuilder AddMethod(string method)
        {
            _methods.Add(method);
            return this;
        }
        
        public string Build()
        {
            var sb = new StringBuilder();
            foreach (var @using in _using)
            {
                sb.AppendLine($"using {@using};");
            }

            sb.AppendLine($"namespace {_namespace}");
            sb.Append("{");

            sb.Append(IndentSource($"{Environment.NewLine}public class {_name} : {_baseType}, {string.Join(", ",_interfaces)}{Environment.NewLine}{{", 1));
            
            foreach (var method in _methods)
            {
                sb.Append(IndentSource($"{Environment.NewLine}{method}", 2));
            }
            
            sb.AppendLine(IndentSource($"{Environment.NewLine}}}", 1));
            sb.AppendLine("}");

            return sb.ToString();
        }
        
        private static string IndentSource(string source, int numIndentations)
        {
            return source.Replace(Environment.NewLine, $"{Environment.NewLine}{new string(' ', 4 * numIndentations)}"); // 4 spaces per indentation.
        }
    }
}