shader_type spatial;

render_mode depth_draw_always, diffuse_toon;

void vertex()
{

}

void fragment()
{
	ALBEDO = COLOR.rgb;
	ALPHA = COLOR.a;
}
