//
// Copyright 2018-2019 Sean Spicer 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System.ComponentModel;
using System.Numerics;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using Common;
using NSG.Common.Wpf;
using NSG.Common.Wpf.Properties;
using ReactiveUI;
using Veldrid;
using Veldrid.SceneGraph;
using Veldrid.SceneGraph.InputAdapter;
using Veldrid.SceneGraph.Shaders.Standard;
using Veldrid.SceneGraph.Util;

namespace NSGReactiveUI;

public class ReactiveViewModelBase : ReactiveObject, INotifyPropertyChanged
{
    private IGroup _sceneRoot;

    public IGroup SceneRoot
    {
        get => _sceneRoot;
        set => this.RaiseAndSetIfChanged(ref _sceneRoot, value);
    }

    private ICameraManipulator _cameraManipulator;

    public ICameraManipulator CameraManipulator
    {
        get => _cameraManipulator;
        set => this.RaiseAndSetIfChanged(ref _cameraManipulator, value);
    }

    private IUiEventHandler _eventHandler;

    public IUiEventHandler EventHandler
    {
        get => _eventHandler;
        set => this.RaiseAndSetIfChanged(ref _eventHandler, value);
    }

    private RgbaFloat _clearColor;

    public RgbaFloat ClearColor
    {
        get => _clearColor;
        set => this.RaiseAndSetIfChanged(ref _clearColor, value);
    }

    private TextureSampleCount _fssaCount;

    public TextureSampleCount FsaaCount
    {
        get => _fssaCount;
        set => this.RaiseAndSetIfChanged(ref _fssaCount, value);
    }

    protected ReactiveViewModelBase()
    {
        FsaaCount = TextureSampleCount.Count16;
        EventHandler = new FrameCaptureEventHandler();
        SceneRoot = Group.Create();
        SceneRoot.NameString = "Root";
    }

}

public class MainWindowViewModel : ReactiveViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }
    
    public MainWindowViewModel() : base()
    {
        var gridRoot = GridCubeScene.Build();

        SceneRoot.AddChild(gridRoot);
        SceneRoot.PipelineState = CreateSharedState();

        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            // SceneRoot = root;
            CameraManipulator = TrackballManipulator.Create();
            EventHandler = new PickEventHandler();
            ClearColor = RgbaFloat.Blue;
            FsaaCount = TextureSampleCount.Count16;
            CameraManipulator.SetHomePosition(
                new Vector3(0, 0, 20),
                Vector3.Zero,
                Vector3.UnitX);

            CompositeDisposable t = disposables; // FIXME
        });
    }

    private static IPipelineState CreateSharedState()
    {
        var pso = PipelineState.Create();
    
        pso.ShaderSet = Vertex3Color4Shader.Instance.ShaderSet;
    
        return pso;
    }

    private int _camPosIdx = 0;
    public void ChangeCamera(IUiActionAdapter uiActionAdapter, ICamera camera)
    {
        Vector3 eye;
        Vector3 center;
        Vector3 up;

        var lookDistance = 1f;
        if (CameraManipulator is TrackballManipulator trackballManipulator)
        {
            lookDistance = trackballManipulator.Distance;
        }
            
        camera.ProjectionMatrix.GetLookAt(out eye, out center, out up, lookDistance);

        var dist = (center - eye).Length();
            
        switch (_camPosIdx)
        {
            case 0:
                eye = new Vector3(dist, 0, 0);
                center = Vector3.Zero;
                up = new Vector3(0, 0, 1);
                break;
            case 1:
                eye = new Vector3(0, dist, 0);
                center = Vector3.Zero;
                up = new Vector3(0, 0, 1);
                break;
            case 2:
                eye = new Vector3(0, 0, dist);
                center = Vector3.Zero;
                up = new Vector3(1, 0, 0);
                break;
        }
            
        CameraManipulator.SetHomePosition(eye, center, up);
        CameraManipulator.Home(uiActionAdapter);

        _camPosIdx++;
        if (_camPosIdx > 2) _camPosIdx = 0;
    }

    public void SetCameraOrthographic(IUiActionAdapter uiActionAdapter, ICamera camera)
    {
        CameraManipulator.SetCameraOrthographic(camera, uiActionAdapter);
            
    }

    public void SetCameraPerspective(IUiActionAdapter uiActionAdapter, ICamera camera)
    {
        CameraManipulator.SetCameraPerspective(camera, uiActionAdapter);
    }

}