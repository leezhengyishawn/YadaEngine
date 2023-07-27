#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;
layout (location = 3) in vec3 camPos;
layout (location = 4) in vec2 uvTiling;

layout (set = 1, binding = 0) uniform sampler2D albedoMap;
layout (set = 1, binding = 1) uniform sampler2D normalMap;
layout (set = 1, binding = 2) uniform sampler2D ORM;

struct LightType
{
    vec4 color;
    vec4 pos;
    vec4 dir;
    vec4 cutOff;
};

#define NumberOfLights 128

layout (set = 2, binding = 0) uniform Lights
{
    LightType light[NumberOfLights];
}lights;

layout (location = 0) out vec4 outFragColor;

const float PI = 3.14159265359;
  
vec3 getNormalFromMap(vec2 uvCoord);
float DistributionGGX(float NDotH, float roughness);
float GeometrySchlickGGX(float NDotV, float roughness);
float GeometrySmith(float NDotV, float NotL, float roughness);
vec3 fresnelSchlick(float cosTheta, vec3 F0);
void StepSpecular(float perceptualRoughness, inout vec3 specularTerm);
float Fd_Lambert();

// Light Type Value
// 0: Point Light
// 1: Spot Light
// 2: Directional Light

void main() 
{
    vec2 uvCoord = inUV * uvTiling;
    // outFragColor = vec4(texture(albedoMap, inUV).xyz, 1.0); // Check for map
    uint lightCount = floatBitsToUint(lights.light[0].color.x); // First light is use for light manager
    
    vec3 albedo     = pow(texture(albedoMap, uvCoord).rgb, vec3(2.2));
    vec3 normal     = getNormalFromMap(uvCoord);
    vec3 ORM        = texture(ORM, uvCoord).rgb;
    float ao        = ORM.r;
    float roughness = ORM.g;
    float metallic  = ORM.b;
    
    vec3 N = normalize(normal);
    vec3 V = normalize(camPos - inPos.xyz);
    
    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, albedo, metallic);
	           
    // reflectance equation
    vec3 Lo = vec3(0.0);
    for(uint i = 1; i < lightCount + 1; ++i) 
    {
        float intensity = lights.light[i].color.w;                // color.w holds light intensity
        uint lightType =  floatBitsToUint(lights.light[i].pos.w); // pos.w holds type light
        vec3 L;         // Light Direction
        vec3 H;         // Half-Vector
        vec3 radiance;  // Radiance

        if(lightType == 0) // Point Light
        {
            L = normalize(lights.light[i].pos.xyz - inPos);
            float distance    = length(lights.light[i].pos.xyz - inPos);
            float attenuation = 1.0 / (
            lights.light[i].dir.x * 1.0 +                   // Constant
            lights.light[i].dir.y * distance +              // Linear
            lights.light[i].dir.z * distance * distance);   // Quadratic
            radiance = lights.light[i].color.xyz * attenuation * intensity;
        }
        else if(lightType == 1) // Soft Spot Light
        {
            L = normalize(lights.light[i].pos.xyz - inPos);
            float cutOff = lights.light[i].cutOff.x;
            float outerCutOff = lights.light[i].cutOff.y;

            float theta = dot(L, normalize(-lights.light[i].dir.xyz)); 
            float epsilon = (cutOff - outerCutOff);
            float spotIntensity = clamp((theta - outerCutOff) / epsilon, 0.0, 1.0);

            radiance = lights.light[i].color.xyz * intensity * spotIntensity;
        }
        else // Directional Light
        {
            L = normalize(-lights.light[i].dir.xyz);
            radiance = lights.light[i].color.xyz * intensity;
        }

        // calculate per-light radiance
        H = normalize(V + L);

        float NDotV = abs(dot(N, V)) + 1e-5;
        float NDotL = clamp(dot(N, L), 0.0, 1.0);
        float NDotH = clamp(dot(N, H), 0.0, 1.0);
        float LDotH = clamp(dot(L, H), 0.0, 1.0);

        // cook-torrance brdf
        float NDF = DistributionGGX(NDotH, roughness);
        float G   = GeometrySmith(NDotV, NDotL, roughness);
        vec3 F    = fresnelSchlick(LDotH, F0);
        
        vec3 kS = F;
        vec3 kD = vec3(1.0) - kS;
        kD *= 1.0 - metallic;	  
        
        vec3 numerator    = NDF * G * F;
        float denominator = 4.0 * max(NDotV, 0.0) * max(NDotL, 0.0);
        vec3 specular     = numerator / max(denominator, 0.001);  

        StepSpecular(roughness, specular);
           
        Lo += (kD * albedo / PI + specular) * radiance * NDotL ; 
    }   
    
    vec3 ambient = vec3(0.03f) * albedo * ao;
    vec3 color = ambient + Lo;
	
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0/2.2));  
    
    outFragColor = vec4(color, 1.0);
}

vec3 getNormalFromMap(vec2 uvCoord)
{
    vec3 tangentNormal = texture(normalMap, uvCoord).xyz * 2.0 - 1.0;

    vec3 Q1  = dFdx(inPos);
    vec3 Q2  = dFdy(inPos);
    vec2 st1 = dFdx(inUV);
    vec2 st2 = dFdy(inUV);

    vec3 N   = normalize(inNormal);
    vec3 T  = normalize(Q1*st2.t - Q2*st1.t);
    vec3 B  = -normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);

    return normalize(TBN * tangentNormal);
}

float DistributionGGX(float NDotH, float roughness)
{
    float a      = roughness * roughness;
    float a2     = a * a;

    float NdotH2 = NDotH * NDotH;
	
    float num   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
	
    return num / denom;
}

float GeometrySchlickGGX(float NDotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num   = NDotV;
    float denom = NDotV * (1.0 - k) + k;
	
    return num / denom;
}
float GeometrySmith(float NDotV, float NDotL, float roughness)
{
    float NdotV = NDotV;
    float NdotL = NDotL;
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}  

void StepSpecular(float perceptualRoughness, inout vec3 specularTerm)
{
    float smoothness = 1 - perceptualRoughness;
    vec3 lobe1 = step(smoothness, specularTerm) * smoothness;
    vec3 lobe2 = step(8.0, specularTerm) * 8.0;
    specularTerm = lobe1 + lobe2;
}

float DirectSpecularToon(float NoH, float LoH2, 
    float perceptualRoughness, float roughness2MinusOne, float roughness2, float normalizationTerm)
{
    // GGX Distribution multiplied by combined approximation of Visibility and Fresnel
    // BRDFspec = (D * V * F) / 4.0
    // D = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2
    // V * F = 1.0 / ( LoH^2 * (roughness + 0.5) )

    // Final BRDFspec = roughness^2 / ( NoH^2 * (roughness^2 - 1) + 1 )^2 * (LoH^2 * (roughness + 0.5) * 4.0)
    // We further optimize a few light invariant terms
    // brdfData.normalizationTerm = (roughness + 0.5) * 4.0 rewritten as roughness * 4.0 + 2.0 to a fit a MAD.
    float d = NoH * NoH * roughness2MinusOne + 1.00001f;
    float specularTerm = roughness2 / ((d * d) * max(0.1, LoH2) * normalizationTerm);


    return specularTerm;
}

float Fd_Lambert() 
{
    return 1.0 / PI;
}