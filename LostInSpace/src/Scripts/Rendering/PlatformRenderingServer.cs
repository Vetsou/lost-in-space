using System;
using System.Collections.Generic;
using Godot;

public partial class PlatformRenderingServer : Node3D
{
	private readonly List<Rid> instances = [];
	public void RenderPlatform(IPlatform platform)
	{
		PlatformVisualData data = platform.VisualData;

		Rid instance = RenderingServer.InstanceCreate();
		instances.Add(instance);

		Rid scenario = GetWorld3D().Scenario;

		RenderingServer.InstanceSetScenario(instance, scenario);
		RenderingServer.InstanceSetBase(instance, data.mesh.GetRid());

		RenderingServer.InstanceSetTransform(instance, data.transform);

	}

	public void ClearLevel()
	{
		foreach (Rid instance in instances)
		{
			RenderingServer.FreeRid(instance);
		}
		instances.Clear();
	}

	public override void _ExitTree() => ClearLevel();
}
