using Godot;
using LostInSpace.Scripts.Gameplay.Data;

namespace LostInSpace.Scripts.Rendering;

public enum RenderLayer
{
	Platforms,
	Collectibles
}

public class LevelRenderingServer(World3D world)
{
	private sealed class BatchData
	{
		public Rid rid;
		public Rid instanceRid;
		public int count = 0;
		public List<Vector2I> reverseLookup = [];
		public VisualData visualData;
	}

	private sealed class LayerData(float height)
	{
		public readonly float Height = height;
		public readonly Dictionary<Vector2I, (BatchData batch, int index)> Lookup = [];
	}

	private readonly Dictionary<RenderLayer, LayerData> _layers = new()
	{
		[RenderLayer.Platforms] = new LayerData(LevelConstants.PLATFORM_RENDER_HEIGHT),
		[RenderLayer.Collectibles] = new LayerData(LevelConstants.POINT_RENDER_HEIGHT),
	};
	private readonly Dictionary<VisualData, BatchData> _multimeshBatches = [];

	private readonly Rid _scenario = world.Scenario;
	private int _batchSize = 0;
	private const int _bufferSpan = 20; //Transform(12) + Color(4) + CustomData(4)
	private const int _customDataOffset = 16;

	public void CreateBuffer(Vector2I mapSize)
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

		foreach (LayerData layer in _layers.Values)
		{
			layer.Lookup.Clear();
		}

		_multimeshBatches.Clear();
		_batchSize = 0;
	}

	public void Render(RenderLayer layer, Vector2I pos, VisualData visualData)
	{
		LayerData layerData = _layers[layer];
		if (layerData.Lookup.ContainsKey(pos))
		{
			return;
		}

		(BatchData batch, int index) = RenderInstance(pos, visualData, layerData.Height);
		layerData.Lookup[pos] = (batch, index);
		RenderingServer.MultimeshInstanceSetColor(batch.rid, index, visualData.Albedo);
	}

	public void UpdateAllBatchCustomData(Dictionary<Vector2I, Func<Color>> instanceCustoms)
	{
		var batches = new Dictionary<Rid, List<(int index, Color data)>>();

		foreach ((Vector2I pos, Func<Color> dataFunc) in instanceCustoms)
		{
			LayerData layerData = _layers[RenderLayer.Platforms];
			if (!layerData.Lookup.TryGetValue(pos, out (BatchData batch, int index) lookup))
			{
				continue;
			}

			if (!batches.TryGetValue(lookup.batch.rid, out List<(int, Color)> indexedDataList))
			{
				indexedDataList = [];
				batches[lookup.batch.rid] = indexedDataList;
			}

			indexedDataList.Add((lookup.index, dataFunc()));
		}

		foreach (BatchData batch in _multimeshBatches.Values)
		{
			if (!batches.TryGetValue(batch.rid, out List<(int, Color)> indexedDataList))
			{
				continue;
			}

			float[] buffer = RenderingServer.MultimeshGetBuffer(batch.rid);
			foreach ((int index, Color data) in indexedDataList)
			{
				if (index + 3 < buffer.Length)
				{
					buffer[index * _bufferSpan + _customDataOffset + 0] = data.R;
					buffer[index * _bufferSpan + _customDataOffset + 1] = data.G;
					buffer[index * _bufferSpan + _customDataOffset + 2] = data.B;
					buffer[index * _bufferSpan + _customDataOffset + 3] = data.A;
				}
			}
			RenderingServer.MultimeshSetBuffer(batch.rid, buffer);
		}
	}

	public void Remove(RenderLayer layer, Vector2I pos) => FreeInstance(pos, _layers[layer].Lookup);

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

		transform = transform.RotatedLocal(Vector3.Up, (pos.X + pos.Y) % 4 * (Mathf.Pi / 2));

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
		RenderingServer.InstanceSetScenario(batch.instanceRid, _scenario);

		RenderingServer.InstanceGeometrySetMaterialOverride(batch.instanceRid, data.Material.GetRid());

		_multimeshBatches.Add(data, batch);

		return batch;
	}

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
}
