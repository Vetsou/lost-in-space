shader_type spatial;

render_mode diffuse_toon, depth_prepass_alpha;

uniform float rotation_speed : hint_range(0.0, 10.0) = 2.0;
uniform vec4 glass_color : source_color = vec4(0.2, 1.0, 0.2, 0.5);
uniform vec4 energy_color : source_color = vec4(0.0, 1.0, 0.0, 1.0);
uniform vec4 cap_color : source_color = vec4(0.2, 0.2, 0.2, 1.0);

void vertex()
{
	float angle = TIME * rotation_speed;
	float c = cos(angle);
	float s = sin(angle);
	mat2 rotation_matrix = mat2(vec2(c, -s), vec2(s, c));

	VERTEX.xz *= rotation_matrix;
	NORMAL.xz *= rotation_matrix;
}

void fragment()
{
	vec4 colors[3] = {cap_color, energy_color, glass_color};
	float emiss_multiplier[3] = {0.0, 1.0, 0.0};
	int region = int(step(0.33, UV.y) + step(0.66, UV.y));

	ALBEDO = colors[region].rgb;
	EMISSION = emiss_multiplier[region] * colors[region].rgb;
	ALPHA = colors[region].a;
}
