using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MetaArt {
    public class SkecthInfo {
        public SkecthInfo(Type type, string? name = null, string? description = null) {
            Type = type;
            Name = name ?? type.Name;
            Description = description;
        }
        public Type Type { get; }
        public string Name { get; }
        public string? Description { get; }
    }
    public class SketchGroup {
        public static SketchGroup FromNamespace(string @namespace, Assembly assembly) { 
            var sketches = assembly.GetTypes()
                .Where(x => x.Name == @namespace)
                .Select(x => new SkecthInfo(x))
                .OrderBy(x => x.Name)
                .ToArray();
            return new SketchGroup { Name = @namespace, Sketches = sketches };
        }
        public string Name { get; init; } = null!;
        public ICollection<SkecthInfo> Sketches { get; init; } = null!;
    }
    public interface ISkecthesProvider {
        ICollection<SketchGroup> Groups { get; }
    }
}

namespace System.Runtime.CompilerServices {
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}