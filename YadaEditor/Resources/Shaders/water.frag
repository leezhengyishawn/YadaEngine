#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV;
layout (location = 3) in vec3 camPos;
layout (location = 4) in flat vec3 inAmbient;
layout (location = 5) in vec3 inColor;
layout (location = 6) in vec4 inShadowCoord;
layout (location = 7) in flat vec4 inFoamData;
layout (location = 8) in flat vec2 inNearFar;
layout (location = 9) in flat float inFoamColorScale;

layout (location = 10) in flat vec2 inFogNearFar;
layout (location = 11) in flat vec3 inFogColor;
layout (location = 12) in flat uint inFlags;
layout (location = 13) in vec3 inViewPos;

layout (set = 1, binding = 0) uniform sampler2D noiseMap;
layout (set = 1, binding = 1) uniform sampler2D distortionMap;
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


layout (set = 3, binding = 1) uniform sampler2D shadowMap;

layout (set = 4, binding = 0) uniform sampler2D depthMap;

layout (location = 0) out vec4 outFragColor;

const float PI = 3.14159265359;
  
float DistributionGGX(vec3 N, vec3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness);
vec3 fresnelSchlick(float cosTheta, vec3 F0);
vec3 textureProj(vec4 shadowCoord, vec2 off);
float linearizeDepth(float d,float zNear,float zFar);

// Light Type Value
// 0: Point Light
// 1: Spot Light
// 2: Directional Light

#define SMOOTHSTEP_AA 0.01

void main() 
{
    // outFragColor = vec4(texture(albedoMap, inUV).xyz, 1.0); // Check for map
    uint lightCount = floatBitsToUint(lights.light[0].color.x); // First light is use for light manager
    bool fogOn = bool(inFlags & 0x1);

    float viewSpaceLength = length(inViewPos);
    if(fogOn && viewSpaceLength > inFogColor.y)
        outFragColor = vec4(inFogColor, 1.0);

    vec3 albedo     = pow(inColor, vec3(2.2));
    vec3 normal     = inNormal;

    vec3 ORM        = texture(ORM, inUV).rgb;
    float ao        = 1.0f; //ORM.r;
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
        bool shadow = false;
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
            shadow = true;
        }

        // calculate per-light radiance
        H = normalize(V + L);
        
        // cook-torrance brdf
        float NDF = DistributionGGX(N, H, roughness);
        float G   = GeometrySmith(N, V, L, roughness);
        vec3 F    = fresnelSchlick(max(dot(H, V), 0.0), F0);
        
        vec3 kS = F;
        vec3 kD = vec3(1.0) - kS;
        kD *= 1.0 - metallic;      
        
        vec3 numerator    = NDF * G * F;
        float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
        vec3 specular     = numerator / max(denominator, 0.001);  
        
        vec3 shadowColor = vec3(1.0f);

        if(shadow)
            shadowColor = textureProj(inShadowCoord, vec2(0.0));

        // add to outgoing radiance Lo
        float NdotL = max(dot(N, L), 0.0);                
        Lo += (kD * albedo / PI + specular) * radiance * NdotL * shadowColor;
    }   

    // Ambient Lighting
    vec3 ambient = vec3(inAmbient) * albedo * ao;
    vec3 color = ambient + Lo;
    
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0/2.2));  
    
    // Water Foam
    // Get Data
    float timePass = inFoamData.x;
    float distortion = inFoamData.y;
    float foamThreshold = inFoamData.z;
    float foamDist = inFoamData.w;

    // Get Previous Depth
    vec2 screenUV = gl_FragCoord.xy / textureSize(depthMap, 0);
    float existingDepth = texture(depthMap, screenUV).r;
    float screenDepth = gl_FragCoord.z / gl_FragCoord.w;

     existingDepth = linearizeDepth(existingDepth, inNearFar.x, inNearFar.y);

    float depthDifference = existingDepth - screenDepth;
    float foamDepthDifference = clamp(depthDifference/foamDist, 0.0f, 1.0f);
    float surfaceNoiseCutoff = foamDepthDifference * foamThreshold;

    vec2 distotUV = texture(distortionMap, inUV).xy * 2.0f - vec2(1.0f);
    distotUV *= distortion;

    vec2 noiseUV = vec2(inUV.x + timePass, inUV.y + timePass) + distotUV;
    float surfaceNoiseSample = texture(noiseMap, noiseUV).r;
    surfaceNoiseSample = surfaceNoiseSample > surfaceNoiseCutoff ? 1.0f : 0.0f;

    
//    float depthDifference = existingDepth - screenDepth;
//    float leading = depthDifference < foamDist ?  depthDifference / foamDist : 0.0;
//    float falloff = 1.0 - (depthDifference / foamDist) + 0.5f;

    // Aliasing
    surfaceNoiseSample = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);
    color += vec3(surfaceNoiseSample) * inFoamColorScale;

    // Apply Fog
    if(fogOn && viewSpaceLength > inFogNearFar.x)
    {
        float factor = (viewSpaceLength - inFogNearFar.x) / (inFogNearFar.y - inFogNearFar.x);
        factor = clamp(factor, 0.0, 1.0);
        color = mix(color, inFogColor, factor);
    }

    // Final Color
    outFragColor = vec4(color, 1.0);
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

vec3 textureProj(vec4 shadowCoord, vec2 off)
{    
//    shadowCoord /= shadowCoord.w;
//    if ( shadowCoord.z > -1.0 && shadowCoord.z < 1.0 ) 
//    {
//        float dist = texture( shadowMap, shadowCoord.xy + off ).r;
//        if ( shadowCoord.w > 0.0 && dist < shadowCoord.z ) 
//        {
//            shadow = 0.0f;
//        }
//    }
//    return shadow;
    vec3 shadCoord = inShadowCoord.xyz / inShadowCoord.w;
    shadCoord.xy = shadCoord.xy * 0.5f + 0.5f; // 0 to 1
    float currentDepth = shadCoord.z;

//    float shadow = 1.0f;

//
//    float closestDepth = texture(shadowMap, shadCoord.xy).r; 
//    // get depth of current fragment from light's perspective
//    
//    // check whether current frag pos is in shadow
//     if ( currentDepth > closestDepth) 
//         shadow = inAmbient;

    vec3 shadow = vec3(0.0);
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float closestDepth = texture(shadowMap, shadCoord.xy + vec2(x, y) * texelSize).r; 
            shadow += currentDepth > closestDepth ? inAmbient : vec3(1.0f);        
        }    
    }
    shadow *= 0.11111111111111111111111111111111f; // aka /= 9.0f

    return shadow;
}

float linearizeDepth(float d,float zNear,float zFar)
{
    return zNear * zFar / (zFar + d * (zNear - zFar));
}

