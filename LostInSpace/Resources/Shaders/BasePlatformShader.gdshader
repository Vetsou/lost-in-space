shader_type spatial;

void vertex()
{

}

void fragment()
{
	ALBEDO = COLOR.rgb;
	ALPHA = COLOR.a;
}
