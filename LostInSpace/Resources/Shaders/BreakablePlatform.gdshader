shader_type spatial;

render_mode depth_draw_always;

uniform float scatter_amount : hint_range(0.0, 1.0) = 0.0;

void vertex()
{
	VERTEX.y += sin(TIME + VERTEX.x + VERTEX.z) * 0.025;
	VERTEX.xz *= 1.0 + scatter_amount * 5.0;
	VERTEX.y -= scatter_amount * 2.0;
}

void fragment()
{
	ALBEDO = COLOR.rgb;
	ALPHA = clamp(COLOR.a - scatter_amount * 5.0, 0, 1);
}
