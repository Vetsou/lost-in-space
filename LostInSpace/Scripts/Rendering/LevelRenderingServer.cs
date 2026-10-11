using Godot;
using LostInSpace.Scripts.Gameplay.Data;

namespace LostInSpace.Scripts.Rendering;

public partial class LevelRenderingServer : Node3D
{
	private class BatchData
	{
		public Rid rid;
		public Rid instanceRid;
		public int count = 0;
		public List<Vector2I> reverseLookup = [];
		public VisualData visualData;
	}

	private readonly Dictionary<VisualData, BatchData> _multimeshBatches = [];
	private readonly Dictionary<Vector2I, (BatchData batch, int index)> _platformLookup = [];
	private readonly Dictionary<Vector2I, (BatchData batch, int index)> _collectibleLookup = [];
	private int _batchSize = 0;

	public void InitLevel(Vector2I mapSize)
	{
		ClearLevel();
		_batchSize = mapSize.X * mapSize.Y;
	}

	public void ClearLevel()
	{
		foreach (BatchData batch in _multimeshBatches.Values)
		{
			RenderingServer.FreeRid(batch.instanceRid);
			RenderingServer.FreeRid(batch.rid);
		}

		_multimeshBatches.Clear();
		_platformLookup.Clear();
		_collectibleLookup.Clear();
		_batchSize = 0;
	}

	public void RenderPlatform(Vector2I pos, VisualData visualData)
	{
		if (_platformLookup.ContainsKey(pos))
		{
			return;
		}

		(BatchData batch, int index) = RenderInstance(pos, visualData, 0);
		_platformLookup[pos] = (batch, index);
		UpdatePlatformColor(pos, visualData.Albedo);
	}

	public void RenderCollectible(Vector2I pos, VisualData visualData)
	{
		if (_collectibleLookup.ContainsKey(pos))
		{
			return;
		}

		(BatchData batch, int index) = RenderInstance(pos, visualData, LevelConstants.POINT_HOVER_HEIGHT);
		_collectibleLookup[pos] = (batch, index);
		UpdateCollectibleColor(pos, visualData.Albedo);
	}

	private (BatchData, int) RenderInstance(Vector2I pos, VisualData data, float height)
	{
		if (!_multimeshBatches.TryGetValue(data, out BatchData batch))
		{
			batch = CreateBatch(data);
		}

		int index = batch.count;

		batch.reverseLookup.Add(pos);
		batch.count++;
		RenderingServer.MultimeshSetVisibleInstances(batch.rid, batch.count);

		Transform3D transform = Transform3D.Identity;
		transform.Origin = pos.GridToWorld(height);

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

		RenderingServer.MultimeshAllocateData(batch.rid, _batchSize, RenderingServer.MultimeshTransformFormat.Transform3D, true, true);
		RenderingServer.MultimeshSetMesh(batch.rid, data.Mesh.GetRid());
		RenderingServer.MultimeshSetVisibleInstances(batch.rid, 0);

		RenderingServer.InstanceSetBase(batch.instanceRid, batch.rid);
		RenderingServer.InstanceSetScenario(batch.instanceRid, GetWorld3D().Scenario);

		RenderingServer.InstanceGeometrySetMaterialOverride(batch.instanceRid, data.Material.GetRid());

		_multimeshBatches.Add(data, batch);

		return batch;
	}

	public void FreePlatform(Vector2I pos) => FreeInstance(pos, _platformLookup);

	public void FreeCollectible(Vector2I pos) => FreeInstance(pos, _collectibleLookup);

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
			_multimeshBatches.Remove(batch.visualData);
			RenderingServer.FreeRid(batch.instanceRid);
			RenderingServer.FreeRid(batch.rid);
		}
	}

	public void UpdatePlatformColor(Vector2I pos, Color color)
	{
		if (!_platformLookup.TryGetValue(pos, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetColor(lookup.batch.rid, lookup.index, color);
	}

	public void UpdatePlatformData(Vector2I pos, Color color)
	{
		if (!_platformLookup.TryGetValue(pos, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetCustomData(lookup.batch.rid, lookup.index, color);
	}

	public void UpdateCollectibleColor(Vector2I pos, Color color)
	{
		if (!_collectibleLookup.TryGetValue(pos, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetColor(lookup.batch.rid, lookup.index, color);
	}

	public void UpdateCollectibleData(Vector2I pos, Color color)
	{
		if (!_collectibleLookup.TryGetValue(pos, out (BatchData batch, int index) lookup))
		{
			return;
		}
		RenderingServer.MultimeshInstanceSetCustomData(lookup.batch.rid, lookup.index, color);
	}

	public override void _ExitTree() => ClearLevel();
}
