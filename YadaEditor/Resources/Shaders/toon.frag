#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;
layout (location = 3) in vec3 camPos;

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
const float glossiness = 32;
const vec3 specColor = vec3(0.9,0.9,0.9);
const vec3 ambientColor = vec3(0.4,0.4,0.4);
const vec3 rimColor = vec3(1,1,1);
const float rimAmount = 0.716;
const float rimThresHold = 0.1;
  
vec3 getNormalFromMap();
float DistributionGGX(vec3 N, vec3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness);
vec3 fresnelSchlick(float cosTheta, vec3 F0);

// Light Type Value
// 0: Point Light
// 1: Spot Light
// 2: Directional Light

void main() 
{
    // outFragColor = vec4(texture(albedoMap, inUV).xyz, 1.0); // Check for map
    uint lightCount = floatBitsToUint(lights.light[0].color.x); // First light is use for light manager
    
    vec3 albedo     = texture(albedoMap, inUV).rgb;//pow(texture(albedoMap, inUV).rgb, vec3(2.2));
    vec3 normal     = inNormal;//getNormalFromMap();
    
    vec3 N = normalize(normal);
    vec3 V = normalize(camPos - inPos.xyz);
	           
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

        float lightIntensity = smoothstep(0, 0.01, dot(L, N));
        vec3 lightColor = lightIntensity * radiance;
        

        // calculate per-light radiance
        H = normalize(V + L);
        float NdotH = dot(H, N);
        float specularIntensity = pow(NdotH * lightIntensity, glossiness * glossiness);
        float specularSmooth = smoothstep(0.005, 0.01, specularIntensity);
        vec3 specular = specularSmooth * specColor;

        float rimDot = 1.0 - dot(V, N);
        float rimIntensity = rimDot * pow(dot(N, L), rimThresHold);
        rimIntensity = smoothstep(rimAmount - 0.01, rimAmount + 0.01, rimIntensity);
        vec3 rim = rimIntensity * rimColor;

        Lo += albedo * (ambientColor + lightColor + specular + rim);
    }
    
    outFragColor = vec4(Lo, 1.0);
}

vec3 getNormalFromMap()
{
    vec3 tangentNormal = texture(normalMap, inUV).xyz * 2.0 - 1.0;

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

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a      = roughness * roughness;
    float a2     = a * a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;
	
    float num   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
	
    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
	
    return num / denom;
}
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
    float ggx1  = GeometrySchlickGGX(NdotL, roughness);
	
    return ggx1 * ggx2;
}

vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}  
