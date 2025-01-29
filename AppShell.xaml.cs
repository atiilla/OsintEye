using Microsoft.Maui.Controls;
using MauiApp1.Views;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
        }
    }
}
