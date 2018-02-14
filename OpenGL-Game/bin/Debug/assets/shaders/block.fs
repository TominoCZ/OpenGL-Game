#version 400 core

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector;

out vec4 out_Color;

uniform sampler2D textureSampler;
uniform vec3 lightColor;

void main(void){
	vec4 pixelColor = texture(textureSampler, pass_textureCoords);
	if(pixelColor.a==0)discard;

	vec3 unitNormal = normalize(surfaceNormal);
	vec3 unitNormalLightVector = normalize(toLightVector);
	
	float nDot1 = dot(unitNormal, unitNormalLightVector);
	float brightness = 0.3 + (max(nDot1, 0.0) * 0.7);
	
	vec3 diffuse = brightness * lightColor;
	
	out_Color = vec4(diffuse, 1.0) * pixelColor;
}