shader_type spatial;

render_mode diffuse_toon;

uniform float lights_speed : hint_range(0.01, 5) = 2.0;

uniform vec3 base_color : source_color = vec3(0.2, 0.2, 0.2);
uniform vec3 rim_color : source_color = vec3(1.0, 1.0, 1.0);

varying vec4 custom;

void vertex()
{
	VERTEX.y += sin(TIME + VERTEX.x + VERTEX.z) * 0.025;
	custom = INSTANCE_CUSTOM;
}

void fragment()
{
	float regionX = step(0.5, UV.x);
	float regionY = step(0.5, UV.y);

	EMISSION = custom.rgb * (1.0 - regionX) * (1.0 - regionY) * (sin(TIME * lights_speed) * 0.5 + 0.5);
	ALBEDO = COLOR.rgb * regionY * (1.0 - regionX) + rim_color * regionX * (1.0 - regionY) + base_color * regionX * regionY;
}
