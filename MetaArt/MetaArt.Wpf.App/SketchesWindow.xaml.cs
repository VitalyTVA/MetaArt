using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MetaArt.Wpf {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SketchesWindow : Window {
        public SketchesWindow() {
            InitializeComponent();

            var asm = Assembly.LoadFile(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "MetaArt.Sketches.dll"));

            var types = asm.GetTypes();
            var provider = (ISkecthesProvider)Activator.CreateInstance(types.Single(x => typeof(ISkecthesProvider).IsAssignableFrom(x)))!;
            var sketches = provider.Groups
                .SelectMany(x => x.Sketches.Select(y => new SketchDisplayInfo(y.Type, y.Name, x.Name, y.Description)))
                .ToArray();

            var skecthTypes = asm
                .GetTypes()
                .Where(x => typeof(SketchBase).IsAssignableFrom(x) && !sketches.Any(y => y.Type == x))
                .ToHashSet();
            if(skecthTypes.Any()) {
                throw new InvalidOperationException();
            }

            btn.Focus();
            Closed+= async (o, e) => await img.Stop();

            ICollectionView view = CollectionViewSource.GetDefaultView(sketches);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SketchDisplayInfo.Category)));

            list.ItemsSource = view;
            //list.SelectedIndex = 0;
            list.SelectionChanged += (o, e) => { img.Run(((SketchDisplayInfo)list.SelectedItem).Type); };
            IsVisibleChanged += SketchesWindow_IsVisibleChanged;

        }
        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            img.UpdateLocation();
        }
        protected override void OnDeactivated(EventArgs e) {
            base.OnDeactivated(e);
            //img.HideIfNotA();
        }
        private void SketchesWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            
        }

        void Button_Click(object sender, RoutedEventArgs e) {
            //img.Run(((SketchInfo)list.SelectedItem).Type);
        }
        protected override void OnLocationChanged(EventArgs e) {
            base.OnLocationChanged(e);
            img.UpdateLocation();
        }
    }
    record SketchDisplayInfo(Type Type, string Name, string Category, string? Description) {
        public Visibility DescriptionVisibility => Description != null ? Visibility.Visible : Visibility.Collapsed;
    }
}
