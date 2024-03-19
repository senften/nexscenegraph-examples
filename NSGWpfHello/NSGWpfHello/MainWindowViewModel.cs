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

using System.Numerics;
using Common;
using NSG.Common.Wpf;
using Veldrid;
using Veldrid.SceneGraph;
using Veldrid.SceneGraph.InputAdapter;
using Veldrid.SceneGraph.Shaders.Standard;
using Veldrid.SceneGraph.Util;

namespace NSGWpfHello;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel() : base()
    {
        var root = Group.Create();
        root.NameString = "Root";

        var gridRoot = GridCubeScene.Build();

        root.AddChild(gridRoot);
        root.PipelineState = CreateSharedState();

        SceneRoot = root;
        CameraManipulator = TrackballManipulator.Create();
        EventHandler = new PickEventHandler();
        ClearColor = RgbaFloat.Blue;
        FsaaCount = TextureSampleCount.Count16;
            
        CameraManipulator.SetHomePosition(
            new Vector3(0, 0, 20),
            Vector3.Zero,
            Vector3.UnitX);
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