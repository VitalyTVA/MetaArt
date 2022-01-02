using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaArt.ProcessingCompatibility {
    public class ArrayList<T> : List<T> {
        public void add(T item) => Add(item);
    }
}
