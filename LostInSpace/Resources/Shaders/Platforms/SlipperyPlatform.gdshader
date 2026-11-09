shader_type spatial;

render_mode diffuse_toon;

#include "../Utils.gdshaderinc"

uniform vec4 edge_color : source_color = vec4(0.85, 0.95, 1.0, 1.0);
uniform float line_thickness : hint_range(0.001, 0.5) = 0.05;
uniform float scale : hint_range(1.0, 50.0) = 8.0;

varying vec3 v_world_pos;

void vertex()
{
	VERTEX.y += sin(TIME + VERTEX.x + VERTEX.z) * 0.025;
	v_world_pos = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment()
{

	vec2 vor2d = voronoi2D(v_world_pos.xz * scale);

	float edge = vor2d.y - vor2d.x;

	float cracks = 1.0 - smoothstep(0.0, line_thickness, edge);

	vec3 final_color = mix(COLOR.rgb, edge_color.rgb, cracks);

	ALBEDO = final_color;
	SPECULAR = 1.0 * (1.0 - cracks);
	ROUGHNESS = mix(0.8, 0.1, 1.0 - cracks);
}
