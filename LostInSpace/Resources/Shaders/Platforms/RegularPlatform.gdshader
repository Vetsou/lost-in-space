shader_type spatial;

render_mode depth_draw_always, diffuse_toon;

void vertex()
{
	VERTEX.y += sin(TIME + VERTEX.x + VERTEX.z) * 0.025;
}

void fragment()
{
	ALBEDO = COLOR.rgb;
	ALPHA = COLOR.a;
}
