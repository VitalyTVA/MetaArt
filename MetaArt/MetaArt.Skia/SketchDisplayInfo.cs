using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MetaArt.Skia {
    public record SketchDisplayInfo(Type Type, string Name, string Category, string? Description, object[]? Parameters) {
        public static List<SketchDisplayInfo> LoadSketches(Assembly asm) {
            List<SketchDisplayInfo> sketches = new();
            var types = asm.GetTypes();
            var providerType = types.SingleOrDefault(x => typeof(ISkecthesProvider).IsAssignableFrom(x));
            if(providerType != null) {
                var provider = (ISkecthesProvider)Activator.CreateInstance(providerType)!;
                sketches.AddRange(provider.Groups
                    .SelectMany(x => x.Sketches.Select(y => new SketchDisplayInfo(y.Type, y.Name, x.Name, y.Description, y.Parameters))));
            }

            var skecthTypes = asm
                .GetTypes()
                .Where(x => SketchGroup.IsSketchType(x) && !sketches.Any(y => y.Type == x));
            sketches.AddRange(skecthTypes.Select(x => new SketchDisplayInfo(x, x.Name, asm.GetName().Name, null, null)));
            return sketches;
        }


        public bool DescriptionVisibility => Description != null;
    }
}
