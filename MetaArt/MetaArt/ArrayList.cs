using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaArt.ProcessingCompatibility {

    public class ArrayList<T> : List<T> {
        //TODO make extension methods?
        public void add(T item) => Add(item);
        public int size() => Count;
        public T get(int i) => this[i];
        public T set(int i, T val) => this[i] = val;
        public void remove(int i) => RemoveAt(i);
        public bool isEmpty() => Count == 0;
    }
}
