using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using FileAccess = Godot.FileAccess;

public partial class Level : Scene
{
	[Export] private Node PlatformContainer;
	[Export] private Player Player;
	[Export] public PackedScene PointScene;
	[Export] public string PlatformFolder;
	[Export] public string LevelPath;

	private Dictionary<PlatformType, PackedScene> platformScenes = new();
	private static readonly Dictionary<Vector2I, IPlatform> tileMap = [];

	private const float spacing = 1;
	private int pointCounter;

	private int gridWidth;
	private int gridHeight;
	private static Vector2 offset;

	public override void _Ready()
	{
		LoadPlatformScenes();
		LoadLevel(LevelPath);
		Player.Init(this);
	}

	private void LoadPlatformScenes()
	{
		var dir = DirAccess.Open(PlatformFolder);
		dir.ListDirBegin();
		string fileName = dir.GetNext();

		while (fileName != "")
		{
			if (!dir.CurrentIsDir() && fileName.EndsWith(".tscn"))
			{
				string name = Path.GetFileNameWithoutExtension(fileName);

				if (Enum.TryParse(name, out PlatformType type))
				{
					string fullPath = PlatformFolder + fileName;
					var scene = GD.Load<PackedScene>(fullPath);
					platformScenes[type] = scene;
				}
				else
				{
					GD.PushWarning($"Skipping '{fileName}': No matching PlatformType enum found.");
				}
			}
			fileName = dir.GetNext();
		}

		dir.ListDirEnd();

		if (platformScenes.Count == 0)
		{
			GD.PrintErr("No platform scenes loaded! Check folder and naming.");
		}
	}

	private void LoadLevel(string path)
	{
		string jsonContent = FileAccess.GetFileAsString(path);
		var parser = new Json();
		var error = parser.Parse(jsonContent);

		if (error != Error.Ok)
		{
			GD.PrintErr($"Failed to parse JSON: {parser.GetErrorMessage()}");
			return;
		}

		var root = (Godot.Collections.Dictionary)parser.Data;
		var layoutData = (Godot.Collections.Array)root["layout"];

		gridHeight = layoutData.Count;
		gridWidth = ((Godot.Collections.Array)layoutData[0]).Count;
		offset = new Vector2((gridWidth - 1) / 2.0f, (gridHeight - 1) / 2.0f);

		for (int y = 0; y < layoutData.Count; y++)
		{
			var row = (Godot.Collections.Array)layoutData[y];
			for (int x = 0; x < row.Count; x++)
			{
				int cellValue = (int)row[x];

				int baseType = PlatformData.GetPlatformType(cellValue);
				bool hasPoint = PlatformData.HasPoints(cellValue);

				var position = new Vector3((x - offset.X) * spacing, 0, (y - offset.Y) * spacing);

				if (Enum.IsDefined(typeof(PlatformType), baseType))
				{
					PlatformType type = (PlatformType)baseType;

					if (platformScenes.TryGetValue(type, out PackedScene scene))
					{
						InstantiateScene(scene, position);
					}
					else
					{
						GD.PushWarning($"No scene found for PlatformType: {type}");
					}
				}
				if (hasPoint && PointScene != null)
				{
					InstantiateScene(PointScene, position);
				}
			}
		}
	}

	private void InstantiateScene(PackedScene scene, Vector3 position)
	{
		Node3D pointInstance = scene.Instantiate<Node3D>();
		pointInstance.Position = position;
		AddChild(pointInstance);
	}

	public static IPlatform GetTile(Vector2I pos) => tileMap.TryGetValue(pos, out IPlatform tile) ? tile : null;

	public static Vector3 GridToWorld(Vector2I pos) => new ((pos.X - offset.X) * spacing, 0, (pos.Y - offset.Y) * spacing);

	// TODO: Temporary, should make win UI
	public void Win()
	{
		if (pointCounter != 0)
		{
			return;
		}
		ChangeScene(SceneId.MainMenu);
	}
}
