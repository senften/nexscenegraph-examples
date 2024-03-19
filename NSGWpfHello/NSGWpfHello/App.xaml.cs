using System.Configuration;
using System.Data;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace NSGWpfHello;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DisableWpfTabletSupport();

        // var mw = new MainWindow();
        // mw.Show();
    }

    private static void DisableWpfTabletSupport()
    {
        // Get a collection of the tablet devices for this window.    
        var devices = System.Windows.Input.Tablet.TabletDevices;

        if (devices.Count <= 0) return;
        
        // Get the Type of InputManager.  
        var inputManagerType = typeof(System.Windows.Input.InputManager);
        
        // Call the StylusLogic method on the InputManager.Current instance.  
        var stylusLogic = inputManagerType.InvokeMember("StylusLogic",
            BindingFlags.GetProperty | BindingFlags.Instance |
            BindingFlags.NonPublic,
            null, InputManager.Current, null);


        if (stylusLogic == null) return;
            
        // Get the type of the stylusLogic returned 
        // from the call to StylusLogic.  
        var stylusLogicType = stylusLogic.GetType();
            
        // Loop until there are no more devices to remove.  
        while (devices.Count > 0)
        {
            // Remove the first tablet device in the devices collection.  
            stylusLogicType.InvokeMember("OnTabletRemoved",
                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                null, stylusLogic, new object[] { (uint)0 });
        }
    }
}