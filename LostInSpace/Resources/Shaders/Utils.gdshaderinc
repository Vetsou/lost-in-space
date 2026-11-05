float hash(vec2 p) {
	p = fract(p * vec2(123.34, 456.21));
	p += dot(p, p + 45.32);
	return fract(p.x * p.y);
}

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
