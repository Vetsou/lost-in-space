shader_type spatial;

render_mode depth_draw_always, diffuse_toon;

uniform float vortex_radius : hint_range(0.1, 0.275) = 0.275; // Can be controlled through custom.a for open/close animation
uniform int vortex_arm_count : hint_range(2, 10) = 5;
uniform float vortex_spin_speed = 3.0;
uniform float vortex_distortion = 10.0;
uniform float rim_color_multiplier : hint_range(0.1, 5.0) = 5.0;

uniform float circle_thickness : hint_range(0.01, 0.2) = 0.05;
uniform float shadow_radius : hint_range(0.01, 0.3) = 0.3;

uniform float distortion_strength : hint_range(0.2, 3.0) = 0.9;
uniform float distortion_size : hint_range(0.01, 1.0) = 0.05;


varying vec3 v_local_pos;
varying vec4 v_instance_custom;

void vertex()
{
	VERTEX.y += sin(TIME + VERTEX.x + VERTEX.z) * 0.025;
	v_local_pos = VERTEX;
	v_instance_custom = INSTANCE_CUSTOM;
}

void fragment()
{
	vec3 final_color = COLOR.rgb;

	vec4 vortex_color = v_instance_custom;

	vec2 uv = v_local_pos.xz;
	float dist = length(uv);
	float distorted_dist = pow(dist, distortion_strength) + (dist * distortion_size);
	vec3 rim_color = vortex_color.rgb * rim_color_multiplier;

	float double_radius = vortex_radius * 2.0;
	float inner_radius = vortex_radius - circle_thickness;

	//spinny star
	float angle = atan(uv.y, uv.x) + TIME * vortex_spin_speed + distorted_dist * vortex_distortion;

	float arms = abs(cos(angle * float(vortex_arm_count) * 0.5));

	float star_shape = vortex_radius / (distorted_dist + arms * 0.2);
	star_shape = smoothstep(0.5, 1.0, star_shape);

	float alpha = star_shape * smoothstep(double_radius, 0.0, distorted_dist);

	final_color = mix(final_color, vortex_color.rgb * star_shape * 2.0, alpha);

	//inward circle
	float progress = fract(TIME);
	float inv_progress = 1.0 - progress;
	float scaled_radius = vortex_radius * inv_progress;
	float scaled_inner_radius = inner_radius * inv_progress;

	float outer_circle = smoothstep(scaled_radius * 2.0, scaled_radius - 0.01, distorted_dist);
	float inner_circle = smoothstep(scaled_inner_radius * 2.0, scaled_inner_radius - 0.01, distorted_dist);

	final_color = mix(final_color, rim_color, outer_circle - inner_circle);

	//rim
	float rim_outer_circle = smoothstep(double_radius, vortex_radius - 0.01, distorted_dist);
	float rim_inner_circle = smoothstep(inner_radius * 2.0, inner_radius - 0.01, distorted_dist);

	final_color = mix(final_color, rim_color, rim_outer_circle - rim_inner_circle);

	ALBEDO = final_color;
	ALPHA = COLOR.a;
}
