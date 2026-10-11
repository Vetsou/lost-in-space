shader_type spatial;

render_mode depth_draw_always, diffuse_toon;

uniform vec4 edge_color : source_color = vec4(0.31, 0.73, 1.0, 1.0);
uniform float line_thickness : hint_range(0.001, 0.5) = 0.3;
uniform float scale : hint_range(1.0, 10.0) = 5.0;

vec2 hash2(vec2 p) {
	p = fract(p * vec2(123.34, 456.21));
	p += dot(p, p + 45.32);
	return fract(vec2(p.x * p.y, (p.x + p.y) * 54.32));
}

vec2 voronoi2D(vec2 uv) {
	vec2 id = floor(uv);
	vec2 frac = fract(uv);

	float F1 = 8.0;
	float F2 = 8.0;

	for (int y = -1; y <= 1; y++) {
		for (int x = -1; x <= 1; x++) {
			vec2 pos = vec2(float(x), float(y));

			vec2 offset = hash2(id + pos);

			vec2 r = pos + offset - frac;
			float dist_sq = dot(r, r);

			if (dist_sq < F1) {
				F2 = F1;
				F1 = dist_sq;
			} else if (dist_sq < F2) {
				F2 = dist_sq;
			}
		}
	}

	return vec2(sqrt(F1), sqrt(F2));
}

void vertex()
{
	VERTEX.y += sin(TIME + VERTEX.x + VERTEX.z) * 0.025;
}

void fragment()
{
	vec2 vor2d = voronoi2D(UV * scale);

	float f1 = vor2d.x;
	float f2 = vor2d.y;
	float edge = f2 - f1;

	float lines = 1.0 - smoothstep(0.0, line_thickness, edge);

	vec3 final_color = mix(COLOR.rgb, edge_color.rgb, lines);

	ALBEDO = final_color;
	ALPHA = COLOR.a;
}
