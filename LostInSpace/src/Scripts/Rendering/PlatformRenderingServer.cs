using System.Collections.Generic;
using Godot;

public partial class PlatformRenderingServer : Node3D
{
	private readonly Dictionary<Platform, Rid> instances = [];
	public void RenderPlatform(Platform platform)
	{
		if (instances.ContainsKey(platform))
		{
			return;
		}

		PlatformVisualData data = platform.VisualData;

		Rid instance = RenderingServer.InstanceCreate();
		instances[platform] = instance;

		Rid scenario = GetWorld3D().Scenario;

		RenderingServer.InstanceSetScenario(instance, scenario);
		RenderingServer.InstanceSetBase(instance, data.mesh.GetRid());

		RenderingServer.InstanceGeometrySetMaterialOverride(instance, data.material.GetRid());
		RenderingServer.InstanceGeometrySetShaderParameter(instance, "tint", data.albedo);

		RenderingServer.InstanceSetTransform(instance, data.transform);
	}

	public void FreePlatform(Platform platform)
	{
		if (!instances.TryGetValue(platform, out Rid instance))
		{
			return;
		}

		RenderingServer.FreeRid(instance);
		instances.Remove(platform);
	}

	public void UpdatePlatformShader(Platform instance, string param, Variant value) => RenderingServer.InstanceGeometrySetShaderParameter(instances[instance], param, value);

	public void ClearLevel()
	{
		foreach (Rid instance in instances.Values)
		{
			RenderingServer.FreeRid(instance);
		}
		instances.Clear();
	}

	public override void _ExitTree() => ClearLevel();
}
