shader_type spatial;

render_mode depth_draw_always, diffuse_toon;

varying float v_scatter_amount;

void vertex()
{
	vec4 custom = INSTANCE_CUSTOM; // XY - direction, Z - scatter_amount, W - health
	vec3 dir = vec3(custom.x, 0, custom.y);
	float time_wave = TIME + VERTEX.x + VERTEX.z;
	float inv_health = 1.0 / custom.a;
	v_scatter_amount = custom.z;

	VERTEX.y += sin(time_wave) * 0.025;
	VERTEX.y -= v_scatter_amount * 0.5;
	VERTEX.xz *= 1.0 + v_scatter_amount * 2.0;
	VERTEX.xz *= 1.0 + (sin(time_wave * (10.0 * inv_health)) * 0.01 * inv_health);

	vec3 local_dir = vec3(
		dot(MODEL_MATRIX[0].xyz, dir),
		dot(MODEL_MATRIX[1].xyz, dir),
		dot(MODEL_MATRIX[2].xyz, dir)
	);

	VERTEX += local_dir * v_scatter_amount * 2.0;
}

void fragment()
{
	ALBEDO = COLOR.rgb;
	ALPHA = clamp(COLOR.a - v_scatter_amount, 0.0, 1.0);
}
