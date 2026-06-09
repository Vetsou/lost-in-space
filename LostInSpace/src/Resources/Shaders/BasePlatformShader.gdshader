shader_type spatial;

instance uniform vec4 tint : source_color = vec4(1.0);

void vertex()
{
	
}

void fragment()
{
	ALBEDO = tint.rgb;
	ALPHA = tint.a;
}