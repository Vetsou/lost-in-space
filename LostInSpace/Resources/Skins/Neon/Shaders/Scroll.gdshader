shader_type spatial;

render_mode depth_draw_always;

uniform sampler2D albedo_texture : source_color, repeat_enable, hint_default_white;
uniform vec2 scroll_speed = vec2(0.5, 0.0);
uniform float glow_strength = 1.0;

void vertex()
{
	UV = VERTEX.xz + 0.5;
}

void fragment()
{
	vec4 tex = texture(albedo_texture, UV + TIME * scroll_speed);
	ALBEDO = COLOR.rgb * tex.rgb;
	EMISSION = COLOR.rgb * tex.rgb * glow_strength;
	ALPHA = COLOR.a * tex.a;
}
