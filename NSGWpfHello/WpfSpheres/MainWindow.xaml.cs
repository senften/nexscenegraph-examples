using System.Windows;

namespace WpfSpheres;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainWindowViewModel _viewModel;
    private bool _isOrthographic = false;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
    }

    private void ChangeCameraButton_OnClick(object sender, RoutedEventArgs e)
    {
        var camera = VSGElement.GetCamera();

        var width = camera.Width;
        var height = camera.Height;
        var dist = camera.Distance;
        if (!_isOrthographic)
        {
            _viewModel.SetCameraOrthographic(VSGElement.GetUiActionAdapter(),
                VSGElement.GetCamera());
            _isOrthographic = true;
        }
        else
        {
            _viewModel.SetCameraPerspective(VSGElement.GetUiActionAdapter(),
                VSGElement.GetCamera());
            _isOrthographic = false;
        }
    }

    private void ChangeCameraViewButton_OnClick(object sender, RoutedEventArgs e)
    {
        _viewModel.ChangeCamera(VSGElement.GetUiActionAdapter(), VSGElement.GetCamera());
    }

    private void ViewAllButton_OnClick(object sender, RoutedEventArgs e)
    {
        VSGElement.CameraManipulator.ViewAll(VSGElement.GetUiActionAdapter());
    }
}