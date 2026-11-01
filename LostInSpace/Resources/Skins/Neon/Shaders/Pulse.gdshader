shader_type spatial;

render_mode depth_draw_always;

uniform sampler2D albedo_texture : source_color, hint_default_white;
uniform vec4 glow_color : source_color = vec4(0.0, 1.0, 1.0, 1.0);
uniform float glow_strength = 1.0;
uniform float pulse_speed = 2.0;

void vertex()
{
	UV = VERTEX.xz + 0.5;
}

void fragment()
{
	vec4 tex = texture(albedo_texture, UV);
	float pulse = 0.5 + 0.5 * sin(TIME * pulse_speed);
	ALBEDO = COLOR.rgb * tex.rgb;
	EMISSION = glow_color.rgb * tex.rgb * glow_strength * pulse;
	ALPHA = COLOR.a * tex.a;
}
