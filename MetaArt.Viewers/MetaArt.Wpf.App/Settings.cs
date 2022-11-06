using System;
using System.Linq;

namespace MetaArt.Wpf {
    partial class Settings {
        [System.Configuration.UserScopedSetting()]
        [System.Diagnostics.DebuggerNonUserCode()]
        public List<KeyValue> SketchValues {
            get {
                return (List<KeyValue>)this["SketchValues"];
            }
            set {
                this["SketchValues"] = value;
            }
        }
    }
    [Serializable]
    public class KeyValue {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
