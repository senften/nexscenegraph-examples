using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using ReactiveUI;
using Splat;

namespace NSGReactiveUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        // Locator.CurrentMutable.Register(() =>
        //     new DiagramControlViewModel(), typeof(DiagramControlViewModel));
        // Locator.CurrentMutable.Register(() =>
        //     new DataFrameViewModel(), typeof(DataFrameViewModel));

        // A helper method that will register all classes that derive off IViewFor 
        // into our dependency injection container. ReactiveUI uses Splat for it's 
        // dependency injection by default, but you can override this if you like.
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
    }
}
