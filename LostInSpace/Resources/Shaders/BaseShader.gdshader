shader_type spatial;

render_mode depth_draw_always;

uniform sampler2D albedo_texture : source_color, hint_default_white;

void vertex()
{

}

void fragment()
{
	vec4 tex = texture(albedo_texture, UV);
	ALBEDO = COLOR.rgb * tex.rgb;
	ALPHA = COLOR.a * tex.a;
}
