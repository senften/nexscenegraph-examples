using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using Splat;

namespace NSGReactiveUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private bool _isOrthoGraphic = false;
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel();

        // would rather have this DI but Splat does not do DI out-of-the-box
        // need a simple autofac example...
        var locator = Locator.Current.GetService<IViewLocator>();

        this.WhenActivated(disposableRegistration =>
        {
            // this.OneWayBind(ViewModel,
            //         vm => vm.DiagramControlViewModel,
            //         view => view.DiagramView.Content,
            //         contentViewModel => ContentViewSelector(contentViewModel, locator))
            //     .DisposeWith(disposableRegistration);
            // this.OneWayBind(ViewModel,
            //         vm => vm.DataFrameViewModel,
            //         view => view.GridView.Content,
            //         contentViewModel => ContentViewSelector(contentViewModel, locator))
            //     .DisposeWith(disposableRegistration);
            this.OneWayBind(ViewModel,
                vm => vm.SceneRoot,
                view => view.VSGElement.SceneRoot);
            this.OneWayBind(ViewModel,
                vm => vm.CameraManipulator,
                view => view.VSGElement.CameraManipulator);
            this.OneWayBind(ViewModel,
                vm => vm.ClearColor,
                view => view.VSGElement.ClearColor);
            this.OneWayBind(ViewModel,
                vm => vm.FsaaCount,
                view => view.VSGElement.FsaaCount);
            this.OneWayBind(ViewModel,
                vm => vm.EventHandler,
                view => view.VSGElement.EventHandler);
        });
    }

    // these should likely be ReactiveUI commands in the viewmodel that are bound in the view backing
    // PlayCommand = ReactiveCommand.Create(Play, canPlay); // defined in viewmodel
    // bound in view backing
    // this.BindCommand(ViewModel,
    //     vm => vm.PlayCommand,
    // v => v.TimeLinePlay)
    // .DisposeWith(d);
    private void ChangeCameraButton_OnClick(object sender, RoutedEventArgs e)
    {
        var camera = VSGElement.GetCamera();

        var width = camera.Width;
        var height = camera.Height;
        var dist = camera.Distance;
        if (!_isOrthoGraphic)
        {
            ViewModel.SetCameraOrthographic(VSGElement.GetUiActionAdapter(), VSGElement.GetCamera());
            _isOrthoGraphic = true;
        }
        else
        {
            ViewModel.SetCameraPerspective(VSGElement.GetUiActionAdapter(), VSGElement.GetCamera());
            _isOrthoGraphic = false;
        }
    }

    private void ChangeCameraViewButton_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel.ChangeCamera(VSGElement.GetUiActionAdapter(), VSGElement.GetCamera());
    }

    private void ViewAllButton_OnClick(object sender, RoutedEventArgs e)
    {
        VSGElement.CameraManipulator.ViewAll(VSGElement.GetUiActionAdapter());
    }
}