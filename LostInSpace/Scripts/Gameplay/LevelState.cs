using Godot;
using LostInSpace.Scripts.Gameplay.Data;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Gameplay.Platforms.Variants;

namespace LostInSpace.Scripts.Gameplay;

public class LevelState
{
	public string LevelId { get; private set; }
	public Vector2I MapSize { get; private set; }
	public Vector2I PlayerStart { get; private set; }

	private IPlatform[] _platforms;
	private CollectibleId[] _collectibles;
	private readonly Dictionary<byte, (Vector2I a, Vector2I b)> _teleporters = [];

	public ushort PrimaryPointsTotal { get; private set; }
	public ushort OptionalPointsTotal { get; private set; }

	public void Initialize(LevelData data)
	{
		MapSize = new Vector2I(data.Width, data.Height);
		LevelId = data.LevelId;
		PlayerStart = new Vector2I(data.PlayerPositionX, data.PlayerPositionY);

		_platforms = new IPlatform[MapSize.X * MapSize.Y];
		_collectibles = new CollectibleId[MapSize.X * MapSize.Y];

		PrimaryPointsTotal = 0;
		OptionalPointsTotal = 0;

		var teleporters = new List<(byte, Vector2I)>();

		for (int x = 0; x < MapSize.X; x++)
		{
			for (int y = 0; y < MapSize.Y; y++)
			{
				var pos = new Vector2I(x, y);
				int idx = GridToIndex(pos);

				if (data.Platforms[y, x] != 0)
				{
					IPlatform platform = PlatformFactory.CreatePlatform(data.Platforms[y, x]);
					_platforms[idx] = platform;

					if (platform is TeleporterPlatform tp)
					{
						teleporters.Add((tp.TeleportLinkId, pos));
					}
				}

				CollectibleId id = data.Collectibles[y, x];
				if (id != CollectibleId.None)
				{
					_collectibles[idx] = id;

					switch (id)
					{
						case CollectibleId.Primary:
							PrimaryPointsTotal++;
							break;
						case CollectibleId.Optional:
							OptionalPointsTotal++;
							break;
					}
				}
			}
		}

		InitializeTeleporters(teleporters);
	}

	private void InitializeTeleporters(IReadOnlyList<(byte linkId, Vector2I position)> teleporters)
	{
		_teleporters.Clear();

		var groups = new Dictionary<byte, List<Vector2I>>();
		foreach ((byte linkId, Vector2I position) in teleporters)
		{
			if (!groups.TryGetValue(linkId, out List<Vector2I> list))
			{
				list = [];
				groups[linkId] = list;
			}
			list.Add(position);
		}

		foreach ((byte linkId, List<Vector2I> positions) in groups)
		{
			if (positions.Count != 2)
			{
				throw new InvalidOperationException(
					$"Teleporter link id {linkId} has {positions.Count} tiles; expected exactly 2.");
			}
			_teleporters[linkId] = (positions[0], positions[1]);
		}
	}

	public bool IsValid(Vector2I pos) =>
		pos.X >= 0 && pos.Y >= 0 && pos.X < MapSize.X && pos.Y < MapSize.Y;

	public IPlatform GetPlatform(Vector2I pos) =>
		IsValid(pos) ? _platforms[GridToIndex(pos)] : null;
	public void RemovePlatform(Vector2I pos) => _platforms[GridToIndex(pos)] = null;
	public CollectibleId GetCollectible(Vector2I pos) =>
		IsValid(pos) ? _collectibles[GridToIndex(pos)] : CollectibleId.None;
	public void ConsumeCollectible(Vector2I pos) => _collectibles[GridToIndex(pos)] = CollectibleId.None;

	public (Vector2I a, Vector2I b) GetTeleporterLinkPositions(byte linkId) =>
		_teleporters.TryGetValue(linkId, out (Vector2I a, Vector2I b) p) ? p
			: throw new ArgumentException($"Teleporter link id '{linkId}' does not exist");

	private int GridToIndex(Vector2I pos) => pos.Y * MapSize.X + pos.X;
}
