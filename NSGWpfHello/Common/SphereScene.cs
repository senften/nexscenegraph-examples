using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;
using Veldrid.SceneGraph;
using Veldrid.SceneGraph.PipelineStates;
using Veldrid.SceneGraph.Shaders;
using Veldrid.SceneGraph.Util.Shape;
using Veldrid.SceneGraph.VertexTypes;

namespace Common
{
    public static class SphereScene
    {
        public static IGroup Build(uint instanceCount, float range = 50, float baseSize = 0.01f,
            float maxScaleFactor = 20)
        {

            var instanceData = TestInstanceSet(range, maxScaleFactor, instanceCount);

            return Build(instanceData, baseSize);
        }

        public static IGroup Build(InstanceData[] instanceData, float baseSize = 0.01f)
        {
            var root = Group.Create();

            var sphereHints = TessellationHints.Create();
            sphereHints.SetDetailRatio(0.3f);
            var sphereGeode = Geode.Create();
            var sphereShape = Sphere.Create(Vector3.Zero, baseSize);

            var sphereInstanceData = VertexBuffer<InstanceData>.Create();
            sphereInstanceData.VertexData = instanceData;

            var vertexLayoutPerInstance = new VertexLayoutDescription(
                new VertexElementDescription("InstancePosition", VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float3),
                new VertexElementDescription("InstanceScale", VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float3),
                new VertexElementDescription("InstanceVisible", VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float1)
            )
            {
                InstanceStepRate = 1
            };

            var sphereDrawable =
                ShapeDrawable<Position3Texture2Color3Normal3>.Create(
                    sphereShape,
                    sphereHints,
                    new[] {new Vector3(1.0f, 0.0f, 0.0f)},
                    (uint)instanceData.Length);

            sphereDrawable.VertexLayouts.Add(vertexLayoutPerInstance);
            sphereDrawable.InstanceVertexBuffer = sphereInstanceData;
            sphereDrawable.InitialBoundingBox = BoundingBox.Create(-50, -50, -10, 50, 50, 10);
            sphereDrawable.CullingActive = false;

            var sphereMaterial = InstancedSphereMaterial.Create(
                PhongMaterialParameters.Create(
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 0.0f, 1.0f),
                    new Vector3(0.0f, 0.0f, 0.0f),
                    5f),
                PhongHeadlight.Create(PhongLightParameters.Create(
                    new Vector3(0.5f, 0.5f, 0.5f),
                    new Vector3(1.0f, 1.0f, 1.0f),
                    new Vector3(1.0f, 1.0f, 1.0f),
                    1f,
                    0)));

            var pipelineState = sphereMaterial.CreatePipelineState();

            sphereDrawable.PipelineState = pipelineState;
            root.AddChild(sphereDrawable);
            return root;
        }

        private static InstanceData[] TestInstanceSet(float range, float maxScaleFactor, uint instanceCount)
        {
            var random = new Random();
            var instanceData = new InstanceData[instanceCount];

            for (var i = 0; i < instanceCount; ++i)
            {
                var xPos = -range + (float) random.NextDouble() * (range*2); // -range - range
                var yPos = -range + (float) random.NextDouble() * (range * 2);
                var zPos = -range + (float) random.NextDouble() * (range * 2);

                var scale = (float) (maxScaleFactor * random.NextDouble());

                instanceData[i] = new InstanceData(new Vector3(xPos, yPos, zPos), scale * Vector3.One);
            }

            return instanceData;
        }

        public struct InstanceData
        {
            public static uint Size { get; } = (uint) Unsafe.SizeOf<InstanceData>();

            public Vector3 Position;
            public Vector3 Scale;
            public float Visibility;

            public InstanceData(Vector3 position, Vector3 scale)
            {
                Position = position;
                Scale = scale;
                Visibility = 1.0f;
            }
        }
    }

    public class InstancedSphereMaterial : PhongMaterial
    {
        private InstancedSphereMaterial(IPhongMaterialParameters p, PhongLight light0, bool overrideColor)
            : base(p, light0, overrideColor)
        {
        }

        public new static IPhongMaterial Create(IPhongMaterialParameters p, PhongLight light0,
            bool overrideColor = true)
        {
            return new InstancedSphereMaterial(p, light0, overrideColor);
        }

        public override IPipelineState CreatePipelineState()
        {
            var pso = PipelineState.Create();
            var vtxShader =
                new ShaderDescription(ShaderStages.Vertex,
                    ReadEmbeddedAssetBytes(@"Common.Assets.Shaders.LargeSphereCount-vertex.glsl"), "main");

            var frgShader =
                new ShaderDescription(ShaderStages.Fragment,
                    ReadEmbeddedAssetBytes(@"Common.Assets.Shaders.LargeSphereCount-fragment.glsl"), "main");

            pso.ShaderSet = ShaderSet.Create("PhongShader", vtxShader, frgShader);

            pso.AddUniform(CreateLightSourceUniform());
            pso.AddUniform(CreateMaterialUniform());

            return pso;
        }
    }
}