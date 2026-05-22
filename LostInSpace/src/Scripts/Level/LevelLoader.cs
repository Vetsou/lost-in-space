using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using FileAccess = Godot.FileAccess;

public partial class LevelLoader : Node3D
{
	[Export] public PackedScene PointScene;
	[Export] public string PlatformFolder;
	[Export] public string LevelPath;

	private const float spacing = 1;

	private Dictionary<PlatformType, PackedScene> platformScenes = new();

	public override void _Ready()
	{
		LoadPlatformScenes();
		LoadLevel(LevelPath);
	}

	private void LoadPlatformScenes()
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

				if (Enum.TryParse<PlatformType>(name, out PlatformType type))
				{
					string fullPath = PlatformFolder + fileName;
					var scene = GD.Load<PackedScene>(fullPath);
					platformScenes[type] = scene;
					GD.Print($"Loaded platform: {type} -> {fullPath}");
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

		for (int y = 0; y < layoutData.Count; y++)
		{
			var row = (Godot.Collections.Array)layoutData[y];
			for (int x = 0; x < row.Count; x++)
			{
				int cellValue = (int)row[x];
				int baseType = (cellValue >> 1);
				bool hasPoint = (cellValue & 1) != 0;

				var position = new Vector3((x - row.Count) * spacing, 0, (y - layoutData.Count) * spacing);

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

		GD.Print($"Level loaded: {layoutData.Count}x{((Godot.Collections.Array)layoutData[0]).Count} tiles");
	}

	private void InstantiateScene(PackedScene scene, Vector3 position)
	{
		Node3D pointInstance = scene.Instantiate<Node3D>();
		pointInstance.Position = position;
		AddChild(pointInstance);
	}
}
