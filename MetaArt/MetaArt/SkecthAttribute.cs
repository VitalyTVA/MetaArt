using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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