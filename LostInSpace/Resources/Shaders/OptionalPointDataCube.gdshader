shader_type spatial;

render_mode depth_draw_always, diffuse_toon;

uniform float rotation_speed : hint_range(0.0, 10.0) = 2.0;
uniform vec3 cube_color : source_color = vec3(0.2, 0.2, 0.2);
uniform vec3 energy_color : source_color = vec3(1.0, 1.0, 0.0);

uniform float line_count = 2.0;
uniform float line_thickness : hint_range(0.001, 0.4) = 0.05;
uniform float line_speed : hint_range(0.01, 5.0) = 0.5;
uniform float glow_size : hint_range(0.01, 1.0) = 0.1;

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
	float half_thickness = line_thickness * 0.5;
	float blur = 0.01;
	float data_time = fract(TIME * line_speed);

	vec2 scaled_uv = UV * line_count;
	vec2 local_uv = fract(scaled_uv);
	vec2 dist_to_line_center = abs(local_uv - 0.5);

	vec2 line_masks = smoothstep(half_thickness + blur, half_thickness, dist_to_line_center);
	float combined_line_mask = max(line_masks.x, line_masks.y);

	vec2 dist_to_point = abs(UV.yx - data_time);
	vec2 glow_masks = smoothstep(glow_size, 0.0, dist_to_point);
	float combined_glow_mask = max(glow_masks.x, glow_masks.y);

	ALBEDO = mix(cube_color.rgb, energy_color.rgb, combined_line_mask);
	EMISSION = vec3(1.0, 1.0, 1.0) * combined_glow_mask * combined_line_mask;
	ALPHA = COLOR.a;
}
