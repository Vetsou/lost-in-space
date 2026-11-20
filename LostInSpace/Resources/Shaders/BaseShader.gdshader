shader_type spatial;

render_mode diffuse_toon;

uniform sampler2D albedo_texture : source_color, hint_default_white;

void vertex()
{

}

void fragment()
{
	vec4 tex = texture(albedo_texture, UV);
	ALBEDO = COLOR.rgb * tex.rgb;
}
