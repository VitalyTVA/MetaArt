using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MetaArt.Xamarin {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SketchView : ContentView {
        public SketchView() {
            InitializeComponent();
        }
        public void SetSketch(object sketch) { 

        }
    }
}