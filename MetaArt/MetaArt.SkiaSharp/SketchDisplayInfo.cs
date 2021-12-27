using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MetaArt.Skia {
    public record SketchDisplayInfo(Type Type, string Name, string Category, string? Description) {
        public static List<SketchDisplayInfo> LoadSketches(Assembly asm) {
            var types = asm.GetTypes();
            var provider = (ISkecthesProvider)Activator.CreateInstance(types.Single(x => typeof(ISkecthesProvider).IsAssignableFrom(x)))!;
            var sketches = provider.Groups
                .SelectMany(x => x.Sketches.Select(y => new SketchDisplayInfo(y.Type, y.Name, x.Name, y.Description)))
                .ToList();

            var skecthTypes = asm
                .GetTypes()
                .Where(x => typeof(SketchBase).IsAssignableFrom(x) && !sketches.Any(y => y.Type == x));
            sketches.AddRange(skecthTypes.Select(x => new SketchDisplayInfo(x, x.Name, "misc", null)));
            return sketches;
        }


        public bool DescriptionVisibility => Description != null;
    }
}
