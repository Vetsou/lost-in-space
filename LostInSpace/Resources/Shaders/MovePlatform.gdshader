shader_type spatial;

render_mode depth_draw_always, diffuse_toon;

uniform float base_wind_speed = 4.0;
uniform float wind_density = 20.0;
uniform vec4 wind_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float trail_length_multiplier : hint_range(0.1, 2.0) = 1.0;
uniform float trail_width : hint_range(0.01, 0.3) = 0.03;

varying vec3 v_world_pos;
varying vec4 v_instance_custom;

float hash(vec2 p) {
	p = fract(p * vec2(123.34, 456.21));
	p += dot(p, p + 45.32);
	return fract(p.x * p.y);
}

void vertex()
{
	VERTEX.y += sin(TIME + VERTEX.x + VERTEX.z) * 0.025;

	v_world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	v_instance_custom = INSTANCE_CUSTOM;
}

void fragment() {
	int wind_direction = int(round(v_instance_custom.r * 255.0)) % 4;

	bool is_z_axis = wind_direction >= 2;
	bool invert_movement = (wind_direction % 2) == 1;

	float normal_direction = is_z_axis ? v_world_pos.x : v_world_pos.z;
	float movement_base = is_z_axis ? v_world_pos.z : v_world_pos.x;
	float movement_direction = invert_movement ? -movement_base : movement_base;

	float lane_id = floor(normal_direction * wind_density);

	float rand_speed = mix(0.7, 1.4, hash(vec2(lane_id, 1.23)));
	float rand_time_offset = hash(vec2(lane_id, 4.56)) * 10.0;
	float rand_trail_len = mix(0.4, 1.2, hash(vec2(lane_id, 7.89))) * trail_length_multiplier;

	float current_speed = base_wind_speed * rand_speed;
	float wind_time = TIME * current_speed + rand_time_offset;
	float streak_progress = fract(movement_direction - wind_time);

	float trail_head = smoothstep(0.0, rand_trail_len, streak_progress);
	float trail_tail = 1.0 - smoothstep(1.0 - trail_width, 1.0, streak_progress);
	float star_shape = trail_head * trail_tail;

	float lane_offcenter = fract(normal_direction * wind_density) - 0.5;
	float line_thickness = smoothstep(trail_width, 0.0, abs(lane_offcenter));

	float trail = star_shape * line_thickness;

	ALBEDO = mix(COLOR.rgb, wind_color.rgb, wind_color.a * trail);
	ALPHA = COLOR.a;
}
