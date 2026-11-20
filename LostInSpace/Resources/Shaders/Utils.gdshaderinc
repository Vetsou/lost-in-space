float hash(vec2 p) {
	float pdot = dot(p, vec2(127.1, 311.7));

	return fract(sin(pdot) * 43758.5453123);
}

vec2 hash2(vec2 p) {
	p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));

	return fract(sin(p) * 43758.5453123);
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
			float dist = max(abs(r.x), abs(r.y));

			if (dist < F1) {
				F2 = F1;
				F1 = dist;
			} else if (dist < F2) {
				F2 = dist;
			}
		}
	}

	return vec2(F1, F2);
}
