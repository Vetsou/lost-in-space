shader_type spatial;

render_mode depth_draw_always, diffuse_toon;

uniform float scatter_amount : hint_range(0.0, 1.0) = 0.0;

void vertex()
{
	VERTEX.y += sin(TIME + VERTEX.x + VERTEX.z) * 0.025;
	VERTEX.y -= scatter_amount * 0.5;
	VERTEX.xz *= 1.0 + scatter_amount * 2.0;

	//scatter animation direction (based on custom_data)
	vec4 custom = round(INSTANCE_CUSTOM);

	vec3 local_dir = vec3(
		dot(MODEL_MATRIX[0].xyz, custom.xyz),
		dot(MODEL_MATRIX[1].xyz, custom.xyz),
		dot(MODEL_MATRIX[2].xyz, custom.xyz)
	);

	VERTEX += local_dir * scatter_amount * 2.0;
}

void fragment()
{
	ALBEDO = COLOR.rgb;
	ALPHA = clamp(COLOR.a - scatter_amount, 0, 1);
}
