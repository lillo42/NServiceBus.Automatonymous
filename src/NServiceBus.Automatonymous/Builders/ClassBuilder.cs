using System.Collections.Generic;
using System.Text;

namespace NServiceBus.Automatonymous.Builders
{
    public class ClassBuilder
    {
        private readonly HashSet<string> _using = new HashSet<string>();
        public ClassBuilder AddUsing(string @namespace)
        {
            _using.Add(@namespace);
            return this;
        }
        
        public ClassBuilder AddUsing(IEnumerable<string>  namespaces)
        {
            foreach (var @namespace in namespaces)
            {
                _using.Add(@namespace);    
            }
            
            return this;
        }

        private string? _namespace;

        public ClassBuilder SetNamespace(string @namespace)
        {
            _namespace = @namespace;
            return this;
        }

        public string Name => _name ?? string.Empty;
        
        private string? _name;
        public ClassBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        private string? _baseType;
        public ClassBuilder SetBaseType(string @baseTye)
        {
            _baseType = baseTye;
            return this;
        }
        
        private readonly HashSet<string> _interfaces = new HashSet<string>();
        public ClassBuilder AddInterfaces(IEnumerable<string> interfaces)
        {
            foreach (var @interface in interfaces)
            {
                _interfaces.Add(@interface);
            }
            return this;
        }


        private readonly List<string> _methods = new List<string>();

        public ClassBuilder AddMethods(IEnumerable<string> methods)
        {
            _methods.AddRange(methods);
            return this;
        }
        
        public ClassBuilder AddMethod(string method)
        {
            _methods.Add(method);
            return this;
        }
        

        private const string Tab = "    ";
        public string Build()
        {
            var sb = new StringBuilder();
            foreach (var @using in _using)
            {
                sb.AppendLine($"using {@using};");
            }
            
            
            sb.AppendLine($"namespace {_namespace}");
            sb.AppendLine("{");

            sb.AppendLine($"public class {_name} : {_baseType}, {string.Join(',',_interfaces)}")
                .AppendLine("{");
            
            foreach (var method in _methods)
            {
                sb.AppendLine(method);
            }
            
            sb.AppendLine("}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}