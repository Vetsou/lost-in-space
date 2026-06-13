using Godot;
using LostInSpace.Scripts.Gameplay.Platforms;

namespace LostInSpace.Scripts.Rendering;

public partial class PlatformRenderingServer : Node3D
{
	private readonly Dictionary<Platform, Rid> _instances = [];

	public void RenderPlatform(Platform platform)
	{
		if (_instances.ContainsKey(platform))
		{
			return;
		}

		PlatformVisualData data = platform.VisualData;

		Rid instance = RenderingServer.InstanceCreate();
		_instances[platform] = instance;

		Rid scenario = GetWorld3D().Scenario;

		RenderingServer.InstanceSetScenario(instance, scenario);
		RenderingServer.InstanceSetBase(instance, data.Mesh.GetRid());

		RenderingServer.InstanceGeometrySetMaterialOverride(instance, data.Material.GetRid());
		RenderingServer.InstanceGeometrySetShaderParameter(instance, "tint", data.Albedo);

		RenderingServer.InstanceSetTransform(instance, data.Transform);
	}

	public void FreePlatform(Platform platform)
	{
		if (!_instances.TryGetValue(platform, out Rid instance))
		{
			return;
		}

		RenderingServer.FreeRid(instance);
		_instances.Remove(platform);
	}

	public void UpdatePlatformShader(Platform instance, string param, Variant value) =>
		RenderingServer.InstanceGeometrySetShaderParameter(_instances[instance], param, value);

	public void ClearLevel()
	{
		foreach (Rid instance in _instances.Values)
		{
			RenderingServer.FreeRid(instance);
		}
		_instances.Clear();
	}

	public override void _ExitTree() => ClearLevel();
}
