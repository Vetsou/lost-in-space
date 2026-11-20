shader_type spatial;

render_mode diffuse_toon;

void vertex()
{
	VERTEX.y += sin(TIME + VERTEX.x + VERTEX.z) * 0.025;
}

void fragment()
{
	ALBEDO = COLOR.rgb;
}
