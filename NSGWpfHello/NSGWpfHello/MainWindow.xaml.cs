using System.Text;
using System.Windows;
using Veldrid.SceneGraph;
using Veldrid.SceneGraph.InputAdapter;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NSGWpfHello;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainWindowViewModel _viewModel;
    private bool _isOrthoGraphic = false;

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
        if (!_isOrthoGraphic)
        {
            _viewModel.SetCameraOrthographic(VSGElement.GetUiActionAdapter(),
                VSGElement.GetCamera()); //OrthographicCameraOperations.ConvertFromPerspectiveToOrthographic(VSGElement.GetCamera()));
            _isOrthoGraphic = true;
        }
        else
        {
            _viewModel.SetCameraPerspective(VSGElement.GetUiActionAdapter(),
                VSGElement.GetCamera()); //PerspectiveCameraOperations.ConvertFromOrthographicToPerspective(VSGElement.GetCamera()));
            _isOrthoGraphic = false;
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