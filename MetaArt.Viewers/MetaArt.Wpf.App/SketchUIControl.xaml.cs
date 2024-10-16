﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetaArt.Skia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MetaArt.Wpf {
    /// <summary>
    /// Interaction logic for SketchUIControl.xaml
    /// </summary>
    public partial class SketchUIControl : System.Windows.Controls.UserControl {
        readonly Painter painter;
        public SketchUIControl(Painter painter) {
            InitializeComponent();
            this.painter = painter;
            var itemsSource = painter.UIElements
                .Select<UIElementInfo, UIElementViewModel>(x => {
                    return x switch {
                        UICommandInfo c => new CommandViewModel(c),
                        UICaptionInfo c => new CaptionViewModel(c),
                        UIChoiceInfo c => new ChoiceViewModel(c, x => painter.OnChoiceChanged(() => c.Changed(x))),
                        _ => throw new NotImplementedException(), 
                    };
                })
                .ToList();
            painter.UIElementChanged += (o, e) => { 
                itemsSource.ForEach(x => x.OnChanged()); 
            };
            uiElements.ItemsSource = itemsSource;
        }
    }


    public abstract class UIElementViewModel : ObservableObject {
        public virtual void OnChanged() { }
    }
    public class CaptionViewModel : UIElementViewModel {
        readonly UICaptionInfo caption;
        public string Name { get; }
        public string? Text { get; private set; }
        public CaptionViewModel(UICaptionInfo caption) {
            Name = caption.Caption;
            this.caption = caption;
            OnChanged();
        }
        public override void OnChanged() {
            Text = caption.uiCaption.Text;
            OnPropertyChanged(nameof(Text));
            base.OnChanged();
        }
    }
    public class CommandViewModel : UIElementViewModel {
        public ICommand Command { get; }
        public string Name { get; }
        public CommandViewModel(UICommandInfo command) {
            Command = new RelayCommand(command.Execute);
            Name = command.Caption;
        }
    }
    public class ChoiceViewModel : UIElementViewModel {

        public ChoiceViewModel(UIChoiceInfo choice, Action<ChoiceElement<object>> changed) {
            Source = choice.Source;
            selected = Source.First();
            this.changed = changed;
        }
        public ChoiceElement<object>[] Source { get; }
        ChoiceElement<object> selected;
        private readonly Action<ChoiceElement<object>> changed;

        public ChoiceElement<object> Selected {
            get => selected;
            set {
                if(SetProperty(ref selected, value))
                    changed(value);
            }
        }
    }
    public class UIElementTemplateSelector : DataTemplateSelector {
        public DataTemplate? Caption { get; set; }
        public DataTemplate? Command { get; set; }
        public DataTemplate? Choice { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            return (item switch {
                CaptionViewModel => Caption,
                CommandViewModel => Command,
                ChoiceViewModel => Choice,
                _ => throw new NotImplementedException(),
            })!;
        }
    }
}
