using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Platforms;

namespace LostInSpace.Scripts.Rendering;

public partial class LevelRenderingServer : Node3D
{
	private const float POINT_HOVER_HEIGHT = 0.5f;
	private class BatchData
	{
		public Rid rid;
		public Rid instanceRid;
		public int count = 0;
		public List<Platform> reverseLookup = [];
		public VisualData visualData;
	}
	private readonly Dictionary<VisualData, BatchData> multimeshBatches = [];
	private readonly Dictionary<Platform, (BatchData batch, int index)> platformLookup = [];
	private readonly Dictionary<Platform, (BatchData batch, int index)> pointLookup = [];
	private int batchSize = 0;

	public void SetBatchSize(int size) => batchSize = size;

	public void RenderPlatform(Platform platform)
	{
		if (platformLookup.ContainsKey(platform))
		{
			return;
		}

		VisualData data = platform.VisualData;
		(BatchData batch, int index) = RenderInstance(platform, data, 0);

		platformLookup[platform] = (batch, index);

		UpdatePlatformColor(platform, data.Albedo);
	}

	public void RenderPoint(Platform platform, VisualData data)
	{
		if (pointLookup.ContainsKey(platform))
		{
			return;
		}

		(BatchData batch, int index) = RenderInstance(platform, data, POINT_HOVER_HEIGHT);

		pointLookup[platform] = (batch, index);

		UpdatePointColor(platform, data.Albedo);
	}

	private (BatchData, int) RenderInstance(Platform platform, VisualData data, float height)
	{
		if (!multimeshBatches.TryGetValue(data, out BatchData batch))
		{
			batch = CreateBatch(data);
		}

		int index = batch.count;

		batch.reverseLookup.Add(platform);
		batch.count++;
		RenderingServer.MultimeshSetVisibleInstances(batch.rid, batch.count);

		Transform3D transform = Transform3D.Identity;
		transform.Origin = new Vector3(platform.Position.X * LevelConstants.SPACING, height, platform.Position.Y * LevelConstants.SPACING);

		RenderingServer.MultimeshInstanceSetTransform(batch.rid, index, transform);

		return (batch, index);
	}

	private BatchData CreateBatch(VisualData data)
	{
		BatchData batch = new()
		{
			rid = RenderingServer.MultimeshCreate(),
			instanceRid = RenderingServer.InstanceCreate(),
			visualData = data
		};

		RenderingServer.MultimeshAllocateData(batch.rid, batchSize, RenderingServer.MultimeshTransformFormat.Transform3D, true, true);
		RenderingServer.MultimeshSetMesh(batch.rid, data.Mesh.GetRid());
		RenderingServer.MultimeshSetVisibleInstances(batch.rid, 0);

		RenderingServer.InstanceSetBase(batch.instanceRid, batch.rid);
		RenderingServer.InstanceSetScenario(batch.instanceRid, GetWorld3D().Scenario);

		RenderingServer.InstanceGeometrySetMaterialOverride(batch.instanceRid, data.Material.GetRid());

		multimeshBatches.Add(data, batch);

		return batch;
	}

	public void FreePlatform(Platform platform) => FreeInstance(platform, platformLookup);

	public void FreePoint(Platform platform) => FreeInstance(platform, pointLookup);

	private void FreeInstance(Platform platform, Dictionary<Platform, (BatchData, int)> lookupDict)
	{
		if (!lookupDict.TryGetValue(platform, out (BatchData batch, int index) lookup))
		{
			return;
		}
		(BatchData batch, int index) = lookup;

		int lastIndex = batch.count - 1;
		if (index != lastIndex)
		{
			RenderingServer.MultimeshInstanceSetTransform(batch.rid, index, RenderingServer.MultimeshInstanceGetTransform(batch.rid, lastIndex));
			RenderingServer.MultimeshInstanceSetColor(batch.rid, index, RenderingServer.MultimeshInstanceGetColor(batch.rid, lastIndex));
			RenderingServer.MultimeshInstanceSetCustomData(batch.rid, index, RenderingServer.MultimeshInstanceGetCustomData(batch.rid, lastIndex));

			Platform movedPlatform = batch.reverseLookup[lastIndex];
			lookupDict[movedPlatform] = (batch, index);
			batch.reverseLookup[index] = movedPlatform;
		}

		lookupDict.Remove(platform);
		batch.reverseLookup.RemoveAt(lastIndex);
		batch.count--;
		RenderingServer.MultimeshSetVisibleInstances(batch.rid, batch.count);
		if (batch.count == 0)
		{
			multimeshBatches.Remove(batch.visualData);
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
		platformLookup.Clear();
		pointLookup.Clear();
	}

	public void UpdatePlatformColor(Platform platform, Color color)
	{
		if (!platformLookup.TryGetValue(platform, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetColor(lookup.batch.rid, lookup.index, color);
	}

	public void UpdatePlatformData(Platform platform, Color color)
	{
		if (!platformLookup.TryGetValue(platform, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetCustomData(lookup.batch.rid, lookup.index, color);
	}

	public void UpdatePointColor(Platform platform, Color color)
	{
		if (!pointLookup.TryGetValue(platform, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetColor(lookup.batch.rid, lookup.index, color);
	}

	public void UpdatePointData(Platform platform, Color color)
	{
		if (!pointLookup.TryGetValue(platform, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetCustomData(lookup.batch.rid, lookup.index, color);
	}

	public override void _ExitTree() => ClearLevel();
}
