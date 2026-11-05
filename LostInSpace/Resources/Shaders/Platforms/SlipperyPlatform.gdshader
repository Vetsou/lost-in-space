shader_type spatial;

render_mode depth_draw_always, diffuse_toon;

#include "../Utils.gdshaderinc"

uniform vec4 edge_color : source_color = vec4(0.31, 0.73, 1.0, 1.0);
uniform float line_thickness : hint_range(0.001, 0.5) = 0.4;
uniform float scale : hint_range(1.0, 10.0) = 6.0;

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
