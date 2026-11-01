using GdUnit4;
using GdUnit4.Asserts;
using Godot;
using LostInSpace.Scripts.Gameplay.Platforms;
using LostInSpace.Scripts.Rendering;
using static GdUnit4.Assertions;
using FileAccess = Godot.FileAccess;

namespace LostInSpace.Tests.L0.Gameplay;

[TestSuite]
[RequireGodotRuntime]
public class PlatformStyleTests
{
	private const string DefaultStylePath = "res://Resources/Skins/Default/Style.json";
	private const string FixtureDir = "user://platform_style_test";
	private const string FixturePath = $"{FixtureDir}/Style.json";
	private const string NeonStylePath = "res://Resources/Skins/Neon/Style.json";
	private const string Model = "Models/Box.tres";
	private const string ShaderFile = "Test.gdshader";

	[Before]
	public void CreateFixtureAssets()
	{
		DirAccess.MakeDirRecursiveAbsolute($"{FixtureDir}/Models");
		ResourceSaver.Save(new BoxMesh(), $"{FixtureDir}/{Model}");
	}

	[TestCase]
	public void LoadOrDefault_WithMissingFile_ShouldUseDefaultStyle()
	{
		// Act
		var style = PlatformStyle.LoadOrDefault("user://does_not_exist.json");

		// Assert
		Color expected = PlatformStyle.Load(DefaultStylePath).GetVisualData(PlatformType.Goal).Albedo;
		AssertThat(style.GetVisualData(PlatformType.Goal).Albedo).IsEqual(expected);
	}

	[TestCase]
	public void LoadOrDefault_WithBrokenAsset_ShouldUseDefaultStyle()
	{
		// Arrange
		WriteFixture(BuildStyle(PlatformType.Goal, """{ "Model": "Models/Missing.tres", "Color": "#123456" }"""));

		// Act
		var style = PlatformStyle.LoadOrDefault(FixturePath);

		// Assert
		Color expected = PlatformStyle.Load(DefaultStylePath).GetVisualData(PlatformType.Goal).Albedo;
		AssertThat(style.GetVisualData(PlatformType.Goal).Albedo).IsEqual(expected);
	}

	[TestCase]
	public void LoadOrDefault_WithValidStyle_ShouldUseThatStyle()
	{
		// Arrange
		WriteFixture(BuildStyle(PlatformType.Goal, $$"""{ "Model": "{{Model}}", "Color": "#FF0000" }"""));

		// Act
		var style = PlatformStyle.LoadOrDefault(FixturePath);

		// Assert
		AssertThat(style.GetVisualData(PlatformType.Goal).Albedo).IsEqual(new Color(1, 0, 0));
	}

	[TestCase]
	public void DefaultStyle_ShouldLoadEveryType_WithSharedVisuals()
	{
		// Act
		var style = PlatformStyle.Load(DefaultStylePath);

		// Assert
		foreach (PlatformType type in Enum.GetValues<PlatformType>())
		{
			VisualData visual = style.GetVisualData(type);
			AssertThat(visual.Mesh).IsNotNull();
			AssertThat(visual.Material).IsNotNull();
			AssertThat(ReferenceEquals(visual, style.GetVisualData(type))).IsTrue();
		}
	}

	[TestCase]
	public void Style_WithoutOptionalFields_ShouldUseDefaults()
	{
		// Arrange
		WriteFixture(BuildStyle(PlatformType.Regular, $$"""{ "Model": "{{Model}}" }"""));

		// Act
		var style = PlatformStyle.Load(FixturePath);

		// Assert
		AssertThat(style.GetVisualData(PlatformType.Regular).Albedo).IsEqual(Colors.White);
		AssertThat(style.GetAudioData(PlatformType.Regular).EnterSound).IsNull();
		AssertThat(style.GetAudioData(PlatformType.Regular).ExitSound).IsNull();
	}

	[TestCase]
	public void Style_WithMissingFile_ShouldThrow()
	{
		// Act & Assert
		IExceptionAssert ex = AssertThrown(() => { PlatformStyle.Load("user://does_not_exist.json"); });
		ex.HasMessage("Style file 'user://does_not_exist.json' does not exist");
	}

	[TestCase]
	public void Style_WithMissingType_ShouldThrow()
	{
		// Arrange
		WriteFixture($$"""{ "Platforms": { "Regular": { "Model": "{{Model}}" } } }""");

		// Act
		string message = null;
		try
		{
			PlatformStyle.Load(FixturePath);
		}
		catch (Exception e)
		{
			message = e.Message;
		}

		// Assert
		AssertThat(message).IsEqual($"Style file '{FixturePath}' is invalid: Style is missing platform type Goal");
	}

	[TestCase]
	public void Style_WithMissingModelAsset_ShouldThrow()
	{
		// Arrange
		WriteFixture(BuildStyle(PlatformType.Goal, """{ "Model": "Models/Missing.tres" }"""));

		// Act & Assert
		IExceptionAssert ex = AssertThrown(() => { PlatformStyle.Load(FixturePath); });
		ex.HasMessage($"Style '{FixturePath}', platform Goal: asset '{FixtureDir}/Models/Missing.tres' does not exist");
	}

	[TestCase]
	public void Style_WithAbsoluteAssetPath_ShouldThrow()
	{
		// Arrange
		WriteFixture(BuildStyle(PlatformType.Goal, """{ "Model": "res://Resources/Models/BasePoint.tres" }"""));

		// Act & Assert
		IExceptionAssert ex = AssertThrown(() => { PlatformStyle.Load(FixturePath); });
		ex.HasMessage($"Style '{FixturePath}', platform Goal: asset 'res://Resources/Models/BasePoint.tres' must be relative to the style folder");
	}

	[TestCase]
	public void Style_WithWrongSoundAssetType_ShouldThrow()
	{
		// Arrange
		WriteFixture(BuildStyle(PlatformType.Goal, $$"""{ "Model": "{{Model}}", "Sounds": { "OnEnter": "{{Model}}" } }"""));

		// Act & Assert
		IExceptionAssert ex = AssertThrown(() => { PlatformStyle.Load(FixturePath); });
		ex.HasMessage($"Style '{FixturePath}', platform Goal: asset '{FixtureDir}/{Model}' is not a AudioStream");
	}

	[TestCase]
	public void Style_WithWrongTextureAssetType_ShouldThrow()
	{
		// Arrange
		WriteFixture(BuildStyle(PlatformType.Goal, $$"""{ "Model": "{{Model}}", "ShaderParameters": { "albedo_texture": "{{Model}}" } }"""));

		// Act & Assert
		IExceptionAssert ex = AssertThrown(() => { PlatformStyle.Load(FixturePath); });
		ex.HasMessage($"Style '{FixturePath}', platform Goal: asset '{FixtureDir}/{Model}' is not a Texture2D");
	}

	[TestCase]
	public void Style_WithInvalidColor_ShouldThrow()
	{
		// Arrange
		WriteFixture(BuildStyle(PlatformType.Goal, $$"""{ "Model": "{{Model}}", "Color": "notacolor" }"""));

		// Act & Assert
		IExceptionAssert ex = AssertThrown(() => { PlatformStyle.Load(FixturePath); });
		ex.HasMessage($"Style '{FixturePath}', platform Goal: invalid color 'notacolor'");
	}

	[TestCase]
	public void Style_WithCustomShader_ShouldApplyTypedParameters()
	{
		// Arrange
		WriteShader("""
			shader_type spatial;
			uniform float speed;
			uniform int count;
			uniform bool enabled;
			uniform vec2 scroll;
			uniform vec3 offset;
			uniform vec4 tint_raw;
			uniform vec4 glow : source_color;
			uniform vec4 rim : source_color;
			""");
		WriteFixture(BuildStyle(PlatformType.Goal, $$"""
			{
				"Model": "{{Model}}",
				"Shader": "{{ShaderFile}}",
				"ShaderParameters": {
					"speed": 2,
					"count": 3,
					"enabled": true,
					"scroll": [1, 2.5],
					"offset": [1, 2, 3],
					"tint_raw": [1, 2, 3, 4],
					"glow": "#FF0000",
					"rim": [0, 1, 0]
				}
			}
			"""));

		// Act
		var style = PlatformStyle.Load(FixturePath);

		// Assert
		var material = (ShaderMaterial)style.GetVisualData(PlatformType.Goal).Material;
		AssertThat(material.Shader.ResourcePath).IsEqual($"{FixtureDir}/{ShaderFile}");
		AssertThat(material.GetShaderParameter("speed").AsSingle()).IsEqual(2f);
		AssertThat(material.GetShaderParameter("count").AsInt32()).IsEqual(3);
		AssertThat(material.GetShaderParameter("enabled").AsBool()).IsTrue();
		AssertThat(material.GetShaderParameter("scroll").AsVector2()).IsEqual(new Vector2(1, 2.5f));
		AssertThat(material.GetShaderParameter("offset").AsVector3()).IsEqual(new Vector3(1, 2, 3));
		AssertThat(material.GetShaderParameter("tint_raw").AsVector4()).IsEqual(new Vector4(1, 2, 3, 4));
		AssertThat(material.GetShaderParameter("glow").AsColor()).IsEqual(new Color(1, 0, 0));
		AssertThat(material.GetShaderParameter("rim").AsColor()).IsEqual(new Color(0, 1, 0));
	}

	[TestCase]
	public void Style_WithEmptyShaderParameters_ShouldShareBaseMaterial()
	{
		// Arrange
		WriteFixture(BuildStyle(PlatformType.Goal, $$"""{ "Model": "{{Model}}", "ShaderParameters": {} }"""));

		// Act
		var style = PlatformStyle.Load(FixturePath);

		// Assert
		AssertThat(ReferenceEquals(style.GetVisualData(PlatformType.Goal).Material, style.GetVisualData(PlatformType.Regular).Material)).IsTrue();
	}

	[TestCase]
	public void Style_WithUnknownShaderParameter_ShouldThrow()
	{
		// Arrange
		WriteShader("shader_type spatial;\nuniform float speed;\n");
		WriteFixture(BuildStyle(PlatformType.Goal, $$"""{ "Model": "{{Model}}", "Shader": "{{ShaderFile}}", "ShaderParameters": { "sped": 1 } }"""));

		// Act & Assert
		IExceptionAssert ex = AssertThrown(() => { PlatformStyle.Load(FixturePath); });
		ex.HasMessage($"Style '{FixturePath}', platform Goal: shader '{FixtureDir}/{ShaderFile}' has no uniform 'sped'");
	}

	[TestCase]
	public void Style_WithWrongShaderParameterType_ShouldThrow()
	{
		// Arrange
		WriteShader("shader_type spatial;\nuniform vec2 scroll;\n");
		WriteFixture(BuildStyle(PlatformType.Goal, $$"""{ "Model": "{{Model}}", "Shader": "{{ShaderFile}}", "ShaderParameters": { "scroll": [1, 2, 3] } }"""));

		// Act & Assert
		IExceptionAssert ex = AssertThrown(() => { PlatformStyle.Load(FixturePath); });
		ex.HasMessage($"Style '{FixturePath}', platform Goal: parameter 'scroll' value '[1,2,3]' is not valid for a Vector2 uniform");
	}

	[TestCase]
	public void Style_WithNonSpatialShader_ShouldThrow()
	{
		// Arrange
		WriteShader("shader_type canvas_item;\n");
		WriteFixture(BuildStyle(PlatformType.Goal, $$"""{ "Model": "{{Model}}", "Shader": "{{ShaderFile}}" }"""));

		// Act & Assert
		IExceptionAssert ex = AssertThrown(() => { PlatformStyle.Load(FixturePath); });
		ex.HasMessage($"Style '{FixturePath}', platform Goal: shader '{FixtureDir}/{ShaderFile}' is not a spatial shader");
	}

	[TestCase]
	public void NeonStyle_ShouldLoadTexturesShadersAndSounds()
	{
		// Act
		var style = PlatformStyle.Load(NeonStylePath);

		// Assert
		foreach (PlatformType type in Enum.GetValues<PlatformType>())
		{
			var material = (ShaderMaterial)style.GetVisualData(type).Material;
			AssertThat(material.GetShaderParameter("albedo_texture").AsGodotObject() is Texture2D).IsTrue();
			AssertThat(material.Shader.ResourcePath).StartsWith("res://Resources/Skins/Neon/Shaders/");
			AssertThat(style.GetAudioData(type).EnterSound != null || style.GetAudioData(type).ExitSound != null).IsTrue();
		}
	}

	[TestCase]
	public void ExtractMesh_ShouldFindNestedMesh()
	{
		// Arrange
		var root = new Node3D();
		var parent = new Node3D();
		var meshInstance = new MeshInstance3D { Mesh = new BoxMesh() };
		root.AddChild(parent);
		parent.AddChild(meshInstance);
		parent.Owner = root;
		meshInstance.Owner = root;

		var scene = new PackedScene();
		scene.Pack(root);
		root.Free();

		// Act
		Mesh extracted = PlatformStyle.ExtractMesh(scene);

		// Assert
		AssertThat(extracted is BoxMesh).IsTrue();
	}

	[TestCase]
	public void ExtractMesh_WithoutMesh_ShouldReturnNull()
	{
		// Arrange
		var root = new Node3D();
		var scene = new PackedScene();
		scene.Pack(root);
		root.Free();

		// Act & Assert
		AssertThat(PlatformStyle.ExtractMesh(scene)).IsNull();
	}

	private static string BuildStyle(PlatformType overridden, string overrideEntry)
	{
		IEnumerable<string> entries = Enum.GetValues<PlatformType>().Select(type =>
			type == overridden
				? $"\"{type}\": {overrideEntry}"
				: $$"""
				"{{type}}": { "Model": "{{Model}}" }
				""");

		return $$"""{ "Platforms": { {{string.Join(", ", entries)}} } }""";
	}

	private static void WriteFixture(string json) => WriteFile(FixturePath, json);

	private static void WriteShader(string code)
	{
		WriteFile($"{FixtureDir}/{ShaderFile}", code);
		ResourceLoader.Load($"{FixtureDir}/{ShaderFile}", null, ResourceLoader.CacheMode.Replace);
	}

	private static void WriteFile(string path, string content)
	{
		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(content);
	}
}
