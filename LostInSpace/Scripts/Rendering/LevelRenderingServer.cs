using Godot;
using LostInSpace.Scripts.Gameplay.Data;

namespace LostInSpace.Scripts.Rendering;

public partial class LevelRenderingServer : Node3D
{
	private const float POINT_HOVER_HEIGHT = 0.5f;
	private class BatchData
	{
		public Rid rid;
		public Rid instanceRid;
		public int count = 0;
		public List<Vector2I> reverseLookup = [];
		public VisualData visualData;
	}
	private readonly Dictionary<VisualData, BatchData> multimeshBatches = [];
	private readonly Dictionary<Vector2I, (BatchData batch, int index)> platformLookup = [];
	private readonly Dictionary<Vector2I, (BatchData batch, int index)> pointLookup = [];
	private int batchSize = 0;

	public void SetBatchSize(int size) => batchSize = size;

	public void RenderPlatform(Vector2I pos, VisualData data)
	{
		if (platformLookup.ContainsKey(pos))
		{
			return;
		}

		(BatchData batch, int index) = RenderInstance(pos, data, 0);

		platformLookup[pos] = (batch, index);

		UpdatePlatformColor(pos, data.Albedo);
	}

	public void RenderPoint(Vector2I pos, VisualData data)
	{
		if (pointLookup.ContainsKey(pos))
		{
			return;
		}

		(BatchData batch, int index) = RenderInstance(pos, data, POINT_HOVER_HEIGHT);

		pointLookup[pos] = (batch, index);

		UpdatePointColor(pos, data.Albedo);
	}

	private (BatchData, int) RenderInstance(Vector2I pos, VisualData data, float height)
	{
		if (!multimeshBatches.TryGetValue(data, out BatchData batch))
		{
			batch = CreateBatch(data);
		}

		int index = batch.count;

		batch.reverseLookup.Add(pos);
		batch.count++;
		RenderingServer.MultimeshSetVisibleInstances(batch.rid, batch.count);

		Transform3D transform = Transform3D.Identity;
		transform.Origin = new Vector3(pos.X * LevelConstants.SPACING, height, pos.Y * LevelConstants.SPACING);

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

	public void FreePlatform(Vector2I pos) => FreeInstance(pos, platformLookup);

	public void FreePoint(Vector2I pos) => FreeInstance(pos, pointLookup);

	private void FreeInstance(Vector2I pos, Dictionary<Vector2I, (BatchData, int)> lookupDict)
	{
		if (!lookupDict.TryGetValue(pos, out (BatchData batch, int index) lookup))
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

			Vector2I movedPlatform = batch.reverseLookup[lastIndex];
			lookupDict[movedPlatform] = (batch, index);
			batch.reverseLookup[index] = movedPlatform;
		}

		lookupDict.Remove(pos);
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

	public void UpdatePlatformColor(Vector2I pos, Color color)
	{
		if (!platformLookup.TryGetValue(pos, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetColor(lookup.batch.rid, lookup.index, color);
	}

	public void UpdatePlatformData(Vector2I pos, Color color)
	{
		if (!platformLookup.TryGetValue(pos, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetCustomData(lookup.batch.rid, lookup.index, color);
	}

	public void UpdatePointColor(Vector2I pos, Color color)
	{
		if (!pointLookup.TryGetValue(pos, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetColor(lookup.batch.rid, lookup.index, color);
	}

	public void UpdatePointData(Vector2I pos, Color color)
	{
		if (!pointLookup.TryGetValue(pos, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetCustomData(lookup.batch.rid, lookup.index, color);
	}

	public override void _ExitTree() => ClearLevel();
}
