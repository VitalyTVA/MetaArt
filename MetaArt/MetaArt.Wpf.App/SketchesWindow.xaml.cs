using MetaArt.Skia;
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

            var sketches = new[] {
                "MetaArt.ThatButtonAgain.dll",
                "MetaArt.Sketches.dll",
                "MetaArt.Sketches.Skia.dll",
            }
            .SelectMany(x => SketchDisplayInfo.LoadSketches(Assembly.LoadFile(System.IO.Path.Combine(Directory.GetCurrentDirectory(), x)))
            .ToList());

            btn.Focus();
            Closed += async (o, e) => {
                SaveState();
                await img.Stop();
            };

            ICollectionView view = CollectionViewSource.GetDefaultView(sketches);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SketchDisplayInfo.Category)));

            list.ItemsSource = view;
            //list.SelectedIndex = 0;
            list.SelectionChanged += (o, e) => {
                var sketchDisplayInfo = GetCurrentSketch()!;
                img.Run(sketchDisplayInfo.Type, sketchDisplayInfo.Parameters); 
            };
            IsVisibleChanged += SketchesWindow_IsVisibleChanged;

            LoadState();
        }

        SketchDisplayInfo? GetCurrentSketch() {
            return list.SelectedItem as SketchDisplayInfo;
        }

        void SaveState() {
            Settings.Default.WindowSize = new System.Drawing.Size((int)Width, (int)Height);
            Settings.Default.WindowLocation = new System.Drawing.Point((int)Left, (int)Top);
            Settings.Default.CurrentSketchTypeFullName = GetCurrentSketch()?.Type.FullName;
            Settings.Default.Save();
        }

        void LoadState() {
            var size = Settings.Default.WindowSize;
            if(!size.IsEmpty) {
                Width = size.Width;
                Height = size.Height;
            }
            var location = Settings.Default.WindowLocation;
            Left = location.X;
            Top = location.Y;

            var sketchTypeFullName = Settings.Default.CurrentSketchTypeFullName;
            Dispatcher.BeginInvoke(new Action(() => {
                list.SelectedItem = list.ItemsSource.Cast<SketchDisplayInfo>().FirstOrDefault(x => x.Type.FullName == sketchTypeFullName);
            }), DispatcherPriority.ContextIdle);
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
}
