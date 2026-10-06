shader_type spatial;

render_mode blend_mix, depth_draw_never, depth_test_disabled, cull_disabled, unshaded;

uniform float vortex_radius : hint_range(0.1, 0.275) = 0.275; //alpha
uniform vec4 vortex_color : source_color = vec4(1.0, 1.0, 1.0, 1.0); //rgb
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

float hash(vec2 p) {
    p = fract(p * vec2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return fract(p.x * p.y);
}

void vertex() {
    v_local_pos = VERTEX;
}

void fragment() {
	if (!FRONT_FACING) {
        discard;
    }

    vec2 uv = v_local_pos.xz;
    float dist = length(uv);
	float distorted_dist = pow(dist, distortion_strength) + (dist * distortion_size);
	vec3 rim_color = vortex_color.rgb * rim_color_multiplier;

	//shadow
	vec3 final_color = vec3(0.0);
	float final_alpha = smoothstep(shadow_radius, 0.0, distorted_dist);

	//spinny star
    float angle = atan(uv.y, uv.x) + TIME * vortex_spin_speed + distorted_dist * vortex_distortion;

	float arms = abs(cos(angle * float(vortex_arm_count) * 0.5));

    float star_shape = vortex_radius / (distorted_dist + arms * 0.2);
    star_shape = smoothstep(0.5, 1.0, star_shape);

    float alpha = star_shape * smoothstep(vortex_radius * 2.0, 0.0, distorted_dist);

	final_color = mix(final_color, vortex_color.rgb * star_shape * 2.0, alpha);
	final_alpha = max(final_alpha, alpha);

	//inward circle
	float progress = fract(TIME);
	float outer_circle = smoothstep(vortex_radius * (1.0 - progress) * 2.0, vortex_radius * (1.0 - progress) - 0.01, distorted_dist);
	float inner_circle = smoothstep((vortex_radius - circle_thickness) * (1.0 - progress) * 2.0, (vortex_radius - circle_thickness) * (1.0 - progress) - 0.01, distorted_dist);

	final_color = mix(final_color, rim_color, outer_circle - inner_circle);
	final_alpha = max(final_alpha, outer_circle - inner_circle);

	//rim
	float rim_outer_circle = smoothstep(vortex_radius * 2.0, vortex_radius - 0.01, distorted_dist);
	float rim_inner_circle = smoothstep((vortex_radius - circle_thickness) * 2.0, (vortex_radius - circle_thickness) - 0.01, distorted_dist);

	final_color = mix(final_color, rim_color, rim_outer_circle - rim_inner_circle);
	final_alpha = max(final_alpha, rim_outer_circle - rim_inner_circle);

    ALBEDO = final_color;
    ALPHA = final_alpha;
}
