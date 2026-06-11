using Godot;
using LostInSpace.Scripts.Gameplay;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Platforms;

namespace LostInSpace.Scripts.Rendering;

public partial class PlatformRenderingServer : Node3D
{
	private class BatchData
	{
		public Rid rid;
		public Rid instanceRid;
		public int count = 0;
		public List<Platform> reverseLookup = [];
	}
	private readonly Dictionary<PlatformVisualData, BatchData> multimeshBatches = [];
	private readonly Dictionary<Platform, (BatchData batch, int index)> batchLookup = [];

	public void RenderPlatform(Platform platform)
	{
		PlatformVisualData data = platform.VisualData;

		if (!multimeshBatches.TryGetValue(data, out BatchData batch))
		{
			batch = CreateBatch(data);
		}

		int index = batch.count;

		batchLookup.Add(platform, (batch, index));
		batch.reverseLookup.Add(platform);
		batch.count++;
		RenderingServer.MultimeshSetVisibleInstances(batch.rid, batch.count);

		Transform3D transform = Transform3D.Identity;
		transform.Origin = new Vector3(platform.Position.X * LevelConstants.SPACING, 0, platform.Position.Y * LevelConstants.SPACING);

		UpdatePlatformColor(platform, data.Albedo);

		RenderingServer.MultimeshInstanceSetTransform(batch.rid, index, transform);
	}

	private BatchData CreateBatch(PlatformVisualData data)
	{
		BatchData batch = new()
		{
			rid = RenderingServer.MultimeshCreate(),
			instanceRid = RenderingServer.InstanceCreate()
		};

		RenderingServer.MultimeshAllocateData(batch.rid, Level.MapSize.X*Level.MapSize.Y, RenderingServer.MultimeshTransformFormat.Transform3D, true, true);
		RenderingServer.MultimeshSetMesh(batch.rid, data.Mesh.GetRid());
		RenderingServer.MultimeshSetVisibleInstances(batch.rid, 0);

		RenderingServer.InstanceSetBase(batch.instanceRid, batch.rid);
		RenderingServer.InstanceSetScenario(batch.instanceRid, GetWorld3D().Scenario);

		RenderingServer.InstanceGeometrySetMaterialOverride(batch.instanceRid, data.Material.GetRid());

		multimeshBatches.Add(data, batch);

		return batch;
	}

	public void FreePlatform(Platform platform)
	{
		(BatchData batch, int index) = batchLookup[platform];
		int lastIndex = batch.count - 1;

		if (index != lastIndex)
		{
			RenderingServer.MultimeshInstanceSetTransform(batch.rid, index, RenderingServer.MultimeshInstanceGetTransform(batch.rid, lastIndex));
			RenderingServer.MultimeshInstanceSetColor(batch.rid, index, RenderingServer.MultimeshInstanceGetColor(batch.rid, lastIndex));
			RenderingServer.MultimeshInstanceSetCustomData(batch.rid, index, RenderingServer.MultimeshInstanceGetCustomData(batch.rid, lastIndex));

			Platform movedPlatform = batch.reverseLookup[lastIndex];
			batchLookup[movedPlatform] = (batch, index);
			batch.reverseLookup[index] = movedPlatform;

			batchLookup.Remove(platform);
			batch.reverseLookup.RemoveAt(lastIndex);
		}

		batch.count--;
		RenderingServer.MultimeshSetVisibleInstances(batch.rid, batch.count);

		if (batch.count == 0)
		{
			multimeshBatches.Remove(platform.VisualData);
			RenderingServer.FreeRid(batch.instanceRid);
			RenderingServer.FreeRid(batch.rid);
		}
	}

	public void ClearLevel()
	{
		foreach (BatchData batch in multimeshBatches.Values)
		{
			RenderingServer.FreeRid(batch.instanceRid);
			RenderingServer.FreeRid(batch.rid);
		}

		multimeshBatches.Clear();
		batchLookup.Clear();
	}

	public void UpdatePlatformColor(Platform platform, Color color)
	{
		(BatchData batch, int index) = batchLookup[platform];
		RenderingServer.MultimeshInstanceSetColor(batch.rid, index, color);
	}

	public void UpdatePlatformData(Platform platform, Color color)
	{
		(BatchData batch, int index) = batchLookup[platform];
		RenderingServer.MultimeshInstanceSetCustomData(batch.rid, index, color);
	}

	public override void _ExitTree() => ClearLevel();
}
