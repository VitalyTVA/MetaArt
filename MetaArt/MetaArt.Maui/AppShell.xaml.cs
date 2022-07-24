namespace MetaArt.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("sketch", typeof(SkecthPage));
    }
}

