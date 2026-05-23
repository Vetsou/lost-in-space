using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using FileAccess = Godot.FileAccess;

public class LevelLoader
{
	private PackedScene PointScene;
	private string PlatformFolder = "res://src/Objects/Platforms/"; // TODO probably shouldn't hardcode this here but who cares :P

	private Dictionary<PlatformType, PackedScene> platformScenes = new();
	public static readonly Dictionary<Vector2I, IPlatform> tileMap = [];

	private int gridWidth;
	private int gridHeight;
	public static Vector2 offset;

	public LevelLoader(PackedScene pointScene)
	{
		PointScene = pointScene;
	}

	public void LoadPlatformScenes()
	{
		var dir = DirAccess.Open(PlatformFolder);
		if (dir == null)
		{
			GD.PrintErr($"Cannot access platform folder: {PlatformFolder}");
			return;
		}

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
					PackedScene scene = GD.Load<PackedScene>(fullPath);
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

	public void LoadLevel(string levelPath, Node platformParent)
	{
		string jsonContent = FileAccess.GetFileAsString(levelPath);
		var parser = new Json();
		Error error = parser.Parse(jsonContent);

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

				var position = new Vector3((x - offset.X) * PlatformData.Spacing, 0, (y - offset.Y) * PlatformData.Spacing);

				if (Enum.IsDefined(typeof(PlatformType), baseType))
				{
					var type = (PlatformType)baseType;
					IPlatform tile;
					switch (type)
					{
						case PlatformType.Regular:
							tile = new Regular();
							break;
						case PlatformType.Goal:
							tile = new Goal();
							break;
						default:
							tile = new Regular();
							break;
					}
					tileMap[new Vector2I(x, y)] = tile;

					if (platformScenes.TryGetValue(type, out PackedScene scene))
					{
						InstantiateScene(scene, platformParent, position);
					}
					else
					{
						GD.PushWarning($"No scene found for PlatformType: {type}");
					}

					if (hasPoint && PointScene != null)
					{
						tile.Point = (Point) InstantiateScene(PointScene, platformParent, position);
					}
				}
			}
		}
	}

	private static Node3D InstantiateScene(PackedScene scene, Node parent, Vector3 position)
	{
		Node3D sceneInstance = scene.Instantiate<Node3D>();
		sceneInstance.Position = position;
		parent.AddChild(sceneInstance);
		return sceneInstance;
	}
}
