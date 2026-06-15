Shader "ACM/Aether"
{
    Properties
    {
        _TimeOfDay       ("Time of Day (0=midnight, 0.5=solar noon)", Range(0, 1)) = 0.0
        _AutoRotate      ("Auto Rotate",       Float)              = 0
        _AutoSpeed       ("Auto Speed",        Range(0, 100))      = 1.0
        _SkyRotation     ("Sky Rotation",      Range(0, 1))       = 0.0
        _NorthDirection  ("North Direction",   Float)              = 0.0

        _SunIntensity    ("Sun Intensity",     Range(0, 5))        = 1.0
        _RayleighStrength("Rayleigh Strength", Range(0, 3))        = 1.0
        _MieStrength     ("Mie Strength",      Range(0, 3))        = 1.0
        _SunSize         ("Sun Size",          Range(0.001, 0.2))  = 0.008
        _SunDiskIntensity ("Sun Disk Intensity", Range(0, 20))     = 4.0
        _SunDiskSoftness ("Sun Disk Softness", Range(0.001, 0.08)) = 0.012
        _SunriseColor    ("Sunrise Color",     Color)              = (1.0, 0.62, 0.40, 1)
        _SunsetColor     ("Sunset Color",      Color)              = (1.0, 0.38, 0.20, 1)
        _LowSunColorStrength ("Low Sun Color Strength", Range(0, 1)) = 0.85
        _Exposure        ("Exposure",          Range(0.1, 5))      = 1.0
        [Toggle] _UseSeasonalSun ("Use Seasonal Sun", Float) = 1
        _SolarDeclination ("Solar Declination", Range(-23.5, 23.5)) = 0.0

        _StarDensity     ("Star Density",      Range(0, 1))        = 0.04
        _StarScale       ("Star Scale",        Range(0, 200))      = 100.0
        _StarSeed        ("Star Seed",         Range(0, 1))        = 0.0
        _StarBrightness  ("Star Brightness",   Range(0, 10))       = 2.0
        _StarRotateSpeed ("Star Rotate Speed", Range(-1, 1))       = 0.02
        _StarAxisX       ("Rotation Axis X",   Float)              = 0.0
        _StarAxisY       ("Rotation Axis Y",   Float)              = 1.0
        _StarAxisZ       ("Rotation Axis Z",   Float)              = 0.5
        _Star1Color      ("Star Color 1",      Color)              = (1, 1, 1, 1)
        _Star2Color      ("Star Color 2",      Color)              = (0, 0.44, 1, 1)
        _Star2Amount     ("Star2 Amount",      Range(0, 1))        = 0.9
        _Star3Color      ("Star Color 3",      Color)              = (1, 0.65, 0, 1)
        _Star3Amount     ("Star3 Amount",      Range(0, 1))        = 0.9
        _StarFadeStrength("Star Fade Strength",Range(0, 1))        = 1.0
        _StarFadeHeight  ("Star Fade Height",  Range(0, 1))        = 0.0
        _StarGlowStrength("Star Glow Strength",Range(0, 3))        = 0.8
        _StarGlowSize    ("Star Glow Size",    Range(0.1, 6))      = 1.2
        _StarCrossThreshold("Star Cross Amount", Range(0, 1))      = 0.086
        _StarCrossRotation ("Star Cross Rotation",  Range(0, 1))   = 0.0
        _StarCrossSize     ("Star Cross Size",      Range(0.1, 12)) = 8.2
        _StarCrossLength   ("Star Cross Length",    Range(0.1, 16)) = 7.69
        _StarCrossOpacity  ("Star Cross Opacity",   Range(0, 1))   = 0.18
        _StarTwinkleRange  ("Star Twinkle Range",   Range(0, 1))   = 0.0
        [Toggle] _StarTwinkleInvert ("Star Twinkle Invert", Float) = 0
        _StarTwinkleSpeed  ("Star Twinkle Speed",   Range(0, 10))  = 0.0
        _StarTwinkleStrength ("Star Twinkle Strength", Range(0, 1)) = 0.5

        _MoonPhase       ("Moon Phase (0=new, 1=full)", Range(0, 1)) = 0.361
        _MoonSize        ("Moon Size",         Range(0.01, 0.3))   = 0.02
        _MoonColor       ("Moon Color",        Color)              = (0.95, 0.97, 1.0, 1)
        _MoonHaloColor   ("Moon Halo Color",   Color)              = (0.05, 0.08, 0.28, 1)
        _MoonHaloStrength("Moon Halo Strength",Range(0, 1))        = 0.4
        _MoonStarSuppress("Moon Star Suppression", Range(0, 1))    = 1.0
        _MoonStarRange   ("Moon Star Range (larger=wider)", Range(1, 20)) = 1.8
        [Toggle] _UseMoonPhaseSync ("Use Moon Phase Sync", Float) = 1
        [Toggle] _UseRealMoonPosition ("Use Real Moon Position", Float) = 1
        [HideInInspector] _AetherAutoTimeActive ("Aether Auto Time Active", Float) = 0
        [HideInInspector] _AetherAutoBaseTimeOfDay ("Aether Auto Base Time Of Day", Float) = 0
        [HideInInspector] _AetherAutoBaseSiderealRotation ("Aether Auto Base Sidereal Rotation", Float) = 0
        [HideInInspector] _AetherAutoStartTime ("Aether Auto Start Time", Float) = 0
        [HideInInspector] _AetherAutoTimeSpeed ("Aether Auto Time Speed", Float) = 1
        [HideInInspector] _AetherTimeControllerActive ("Aether Time Controller Active", Float) = 0
        [HideInInspector] _AetherTimeControllerOwner ("Aether Time Controller Owner", Float) = 0
        [HideInInspector] _AetherStartupFade ("Aether Startup Fade", Float) = 1
        _MoonRightAscension ("Moon Right Ascension", Range(0, 1)) = 0.0
        _MoonDeclination ("Moon Declination", Range(-30, 30)) = 0.0

        [Toggle] _UseMilkyWay ("Use Milky Way", Float) = 1
        [Toggle] _UseMilkyWayDensityMap ("Use Milky Way Density Map", Float) = 1
        _MilkyWayDensityMap ("Milky Way Density Map", 2D) = "black" {}
        [Toggle] _UseMilkyWayDustMap ("Use Milky Way Dust Map", Float) = 1
        _MilkyWayDustMap ("Milky Way Dust Map", 2D) = "black" {}
        _MilkyWayDustStrength ("Milky Way Dust Strength", Range(0, 1)) = 0.9
        _MilkyWayStrength ("Milky Way Strength", Range(0, 2)) = 0.035
        _MilkyWayWidth    ("Milky Way Width", Range(0.01, 0.4)) = 0.08
        _MilkyWayDetail   ("Milky Way Detail", Range(0, 2)) = 0.0
        _MilkyWaySaturation ("Milky Way Saturation", Range(0, 3)) = 1.0
        _MilkyWayCyanBoost  ("Milky Way Cyan Boost", Range(0, 1)) = 0.2
        _MilkyWayTintAmount ("Milky Way Tint Amount", Range(0, 1)) = 1.0
        _MilkyWayCoreWarmth ("Milky Way Core Warmth", Range(0, 1)) = 0.5
        _MilkyWayColor    ("Milky Way Color", Color) = (0.45, 0.55, 0.75, 1)

        [Toggle] _UseProceduralClouds ("Use Procedural Clouds", Float) = 1
        _CloudAmount       ("Cloud Amount",       Range(0, 1))     = 0.3
        _CloudScale        ("Cloud Scale",        Range(0.1, 10))  = 0.9
        _CloudSoftness     ("Cloud Softness",     Range(0.01, 1))  = 0.35
        _CloudOpacity      ("Cloud Opacity",      Range(0, 1))     = 0.6
        _CloudHeight       ("Cloud Height",       Range(-0.2, 1))  = 0.0
        _CloudSpeed        ("Cloud Speed",        Range(-2, 2))    = 0.03
        _CloudThickness    ("Cloud Thickness",    Range(0, 1))     = 1.0
        _CloudLayerDepth   ("Cloud Layer Depth",  Range(0, 1))     = 0.9
        _CloudHorizonFlatten ("Cloud Horizon Flatten", Range(0, 1)) = 0.0
        _CloudCurvatureDrop ("Cloud Curvature Drop", Range(0, 0.4)) = 0.05
        _CloudStarOcclusion("Cloud Star Occlusion", Range(0, 15))  = 3.5
        _CloudColor        ("Cloud Color",        Color)           = (0.72, 0.78, 0.88, 1)
        _CloudSunsetTint   ("Cloud Sunset Tint",  Color)           = (1.0, 0.42, 0.20, 1)
        _CloudSunsetStrength ("Cloud Sunset Strength", Range(0, 2)) = 0.85
        [Toggle] _UseCloudMoonHighlight ("Use Cloud Moon Highlight", Float) = 1
        _CloudMoonHighlightStrength ("Cloud Moon Highlight Strength", Range(0, 3)) = 0.35
        _CloudMoonHighlightRange ("Cloud Moon Highlight Range", Range(0.02, 1)) = 0.28
        _CloudMoonHighlightColor ("Cloud Moon Highlight Color", Color) = (0.55, 0.68, 1.0, 1)
        _CloudMoonHighlightAltitude ("Cloud Moon Highlight Altitude", Range(0, 1)) = 1.0

        [Toggle] _UseCityLight ("Use City Light", Float) = 0
        _CityLightColor      ("City Light Color", Color) = (1.0, 0.52, 0.25, 1)
        _CityLightStrength   ("City Light Strength", Range(0, 5)) = 0.8
        [Toggle] _CityLightUseDirection ("City Light Use Direction", Float) = 1
        _CityLightDirection  ("City Light Direction", Range(0, 1)) = 0.0
        _CityLightSpread     ("City Light Spread", Range(0.02, 1.2)) = 0.28
        _CityLightStarOcclusion ("City Light Star Occlusion", Range(0, 15)) = 1.0
        _CityLightCloudReflection ("City Light Cloud Reflection", Range(0, 3)) = 0.45

        [Toggle] _UseCatalogStars ("Use Catalog Stars", Float) = 1
        _CatalogStarDataMap    ("Catalog Star Data Map", 2D) = "black" {}
        _CatalogStarColorMap   ("Catalog Star Color Map", 2D) = "white" {}
        _CatalogStarCellMap    ("Catalog Star Cell Map", 2D) = "black" {}
        _CatalogStarCount      ("Catalog Star Count", Float) = 13943
        _CatalogDataMapWidth   ("Catalog Data Map Width", Float) = 256
        _CatalogCellLonCount   ("Catalog Cell Longitude Count", Float) = 64
        _CatalogCellLatCount   ("Catalog Cell Latitude Count", Float) = 32
        [HideInInspector] _CatalogCellExpanded ("Catalog Cell Expanded", Float) = 0
        _CatalogStarSize       ("Catalog Star Size", Range(0.05, 30)) = 1.41
        _CatalogStarSharpness  ("Catalog Star Sharpness", Range(0.1, 30)) = 1.0
        _CatalogStarGlow       ("Catalog Star Glow", Range(0, 1)) = 0.5
        _CatalogStarEmission   ("Catalog Star HDR Emission", Range(0, 20)) = 5.23
        _CatalogStarBrightness ("Catalog Star Brightness", Range(0, 10)) = 0.23
        _CatalogStarRotation   ("Catalog Star Rotation", Range(0, 1)) = 0.0
        [Toggle] _CatalogUseLocation ("Catalog Use Location", Float) = 1
        _CatalogLocationPreset ("Catalog Location Preset", Float) = 1
        _CatalogLatitude       ("Catalog Latitude", Range(-90, 90)) = 35.6895
        _CatalogLongitude      ("Catalog Longitude", Range(-180, 180)) = 139.6917
        _CatalogSiderealRotation ("Catalog Sidereal Rotation", Range(0, 1)) = 0.0
        [HideInInspector] _CatalogSiderealBaseTimeOfDay ("Catalog Sidereal Base Time Of Day", Float) = 0.0
        [Toggle] _UseOrionLines ("Use Orion Lines", Float) = 0
        _OrionLineColor        ("Orion Line Color", Color) = (0.45, 0.75, 1.0, 1)
        _OrionLineStrength     ("Orion Line Strength", Range(0, 5)) = 1.4
        _OrionLineWidth        ("Orion Line Width", Range(0.0001, 0.02)) = 0.003

        [Toggle] _UseMeteors ("Use Meteors", Float) = 0
        _MeteorFrequency       ("Meteor Frequency", Range(0, 60)) = 6.0
        _MeteorColor           ("Meteor Color", Color) = (0.75, 0.9, 1.0, 1)
        _MeteorIntensity       ("Meteor Intensity", Range(0, 20)) = 4.0
        _MeteorSpeed           ("Meteor Speed", Range(0.1, 5.0)) = 1.0
        _MeteorLength          ("Meteor Length", Range(0.02, 0.6)) = 0.2
        _MeteorWidth           ("Meteor Width", Range(0.0005, 0.02)) = 0.0005
        _MeteorDuration        ("Meteor Duration", Range(0.05, 3.0)) = 0.6
        _MeteorSeed            ("Meteor Seed", Range(0, 1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            float  _TimeOfDay;
            float  _AutoRotate;
            float  _AutoSpeed;
            float  _SkyRotation;
            float  _NorthDirection;
            float  _SunIntensity;
            float  _RayleighStrength;
            float  _MieStrength;
            float  _SunSize;
            float  _SunDiskIntensity;
            float  _SunDiskSoftness;
            float4 _SunriseColor;
            float4 _SunsetColor;
            float  _LowSunColorStrength;
            float  _Exposure;
            float  _UseSeasonalSun;
            float  _SolarDeclination;
            float  _StarDensity;
            float  _StarScale;
            float  _StarSeed;
            float  _StarBrightness;
            float  _StarRotateSpeed;
            float  _StarAxisX;
            float  _StarAxisY;
            float  _StarAxisZ;
            float4 _Star1Color;
            float4 _Star2Color;
            float  _Star2Amount;
            float4 _Star3Color;
            float  _Star3Amount;
            float  _StarFadeStrength;
            float  _StarFadeHeight;
            float  _StarGlowStrength;
            float  _StarGlowSize;
            float  _StarCrossThreshold;
            float  _StarCrossRotation;
            float  _StarCrossSize;
            float  _StarCrossLength;
            float  _StarCrossOpacity;
            float  _StarTwinkleRange;
            float  _StarTwinkleInvert;
            float  _StarTwinkleSpeed;
            float  _StarTwinkleStrength;
            float  _MoonPhase;
            float  _MoonSize;
            float4 _MoonColor;
            float4 _MoonHaloColor;
            float  _MoonHaloStrength;
            float  _MoonStarSuppress;
            float  _MoonStarRange;
            float  _UseMoonPhaseSync;
            float  _UseRealMoonPosition;
            float  _AetherAutoTimeActive;
            float  _AetherAutoBaseTimeOfDay;
            float  _AetherAutoBaseSiderealRotation;
            float  _AetherAutoStartTime;
            float  _AetherAutoTimeSpeed;
            float  _AetherStartupFade;
            float  _MoonRightAscension;
            float  _MoonDeclination;
            float  _UseMilkyWay;
            float  _UseMilkyWayDensityMap;
            sampler2D _MilkyWayDensityMap;
            float  _UseMilkyWayDustMap;
            sampler2D _MilkyWayDustMap;
            float  _MilkyWayDustStrength;
            float  _MilkyWayStrength;
            float  _MilkyWayWidth;
            float  _MilkyWayDetail;
            float  _MilkyWaySaturation;
            float  _MilkyWayCyanBoost;
            float  _MilkyWayTintAmount;
            float  _MilkyWayCoreWarmth;
            float4 _MilkyWayColor;
            float  _UseProceduralClouds;
            float  _CloudAmount;
            float  _CloudScale;
            float  _CloudSoftness;
            float  _CloudOpacity;
            float  _CloudHeight;
            float  _CloudSpeed;
            float  _CloudThickness;
            float  _CloudLayerDepth;
            float  _CloudHorizonFlatten;
            float  _CloudCurvatureDrop;
            float  _CloudStarOcclusion;
            float4 _CloudColor;
            float4 _CloudSunsetTint;
            float  _CloudSunsetStrength;
            float  _UseCloudMoonHighlight;
            float  _CloudMoonHighlightStrength;
            float  _CloudMoonHighlightRange;
            float4 _CloudMoonHighlightColor;
            float  _CloudMoonHighlightAltitude;
            float  _UseCityLight;
            float4 _CityLightColor;
            float  _CityLightStrength;
            float  _CityLightUseDirection;
            float  _CityLightDirection;
            float  _CityLightSpread;
            float  _CityLightStarOcclusion;
            float  _CityLightCloudReflection;
            float  _UseCatalogStars;
            sampler2D _CatalogStarDataMap;
            sampler2D _CatalogStarColorMap;
            sampler2D _CatalogStarCellMap;
            float4 _CatalogStarDataMap_TexelSize;
            float4 _CatalogStarCellMap_TexelSize;
            float  _CatalogStarCount;
            float  _CatalogDataMapWidth;
            float  _CatalogCellLonCount;
            float  _CatalogCellLatCount;
            float  _CatalogCellExpanded;
            float  _CatalogStarSize;
            float  _CatalogStarSharpness;
            float  _CatalogStarGlow;
            float  _CatalogStarEmission;
            float  _CatalogStarBrightness;
            float  _CatalogStarRotation;
            float  _CatalogUseLocation;
            float  _CatalogLocationPreset;
            float  _CatalogLatitude;
            float  _CatalogLongitude;
            float  _CatalogSiderealRotation;
            float  _CatalogSiderealBaseTimeOfDay;
            float  _UseOrionLines;
            float4 _OrionLineColor;
            float  _OrionLineStrength;
            float  _OrionLineWidth;
            float  _UseMeteors;
            float  _MeteorFrequency;
            float4 _MeteorColor;
            float  _MeteorIntensity;
            float  _MeteorSpeed;
            float  _MeteorLength;
            float  _MeteorWidth;
            float  _MeteorDuration;
            float  _MeteorSeed;

            static const float kMieG = 0.76;

            struct appdata { float4 vertex : POSITION; };

            struct v2f
            {
                float4 pos    : SV_POSITION;
                float3 rayDir : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos    = UnityObjectToClipPos(v.vertex);
                o.rayDir = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
                return o;
            }

            float miePhase(float cosTheta, float g)
            {
                float g2 = g * g;
                return (3.0 / (8.0 * UNITY_PI)) *
                       ((1.0 - g2) * (1.0 + cosTheta * cosTheta)) /
                       ((2.0 + g2) * pow(abs(1.0 + g2 - 2.0 * g * cosTheta), 1.5));
            }

            float2 northAxis()
            {
                float d = floor(_NorthDirection + 0.5);
                if (d < 0.5) return float2(0, 1);
                if (d < 1.5) return float2(0, -1);
                if (d < 2.5) return float2(1, 0);
                return float2(-1, 0);
            }

            float3 localToWorldHorizon(float3 localDir)
            {
                float2 north = northAxis();
                float2 east = float2(-north.y, north.x);
                float2 h = north * localDir.z + east * (-localDir.x);
                return float3(h.x, localDir.y, h.y);
            }

            float3 worldToLocalHorizon(float3 worldDir)
            {
                float2 north = northAxis();
                float2 east = float2(-north.y, north.x);
                float2 h = float2(worldDir.x, worldDir.z);
                return float3(-dot(h, east), worldDir.y, dot(h, north));
            }

            float aetherAutoElapsedSeconds()
            {
                return 0.0;
            }

            float getEffectiveTimeOfDay()
            {
                if (_AetherAutoTimeActive > 0.5)
                    return frac(_AetherAutoBaseTimeOfDay + aetherAutoElapsedSeconds() / 86400.0);

                return _AutoRotate > 0.5 ? frac(_Time.y * _AutoSpeed / 86400.0) : _TimeOfDay;
            }

            float getEffectiveCatalogSiderealRotation()
            {
                if (_AetherAutoTimeActive > 0.5)
                    return frac(_AetherAutoBaseSiderealRotation + aetherAutoElapsedSeconds() * 1.00273790935 / 86400.0);

                float timeDelta = getEffectiveTimeOfDay() - _CatalogSiderealBaseTimeOfDay;
                if (timeDelta > 0.5)
                    timeDelta -= 1.0;
                if (timeDelta < -0.5)
                    timeDelta += 1.0;

                return frac(_CatalogSiderealRotation + timeDelta * 1.00273790935);
            }

            float3 getSunDir(float timeOfDay, float skyRotation)
            {
                float angle    = (timeOfDay - 0.25) * UNITY_TWO_PI;
                float rotAngle = skyRotation * UNITY_TWO_PI;
                float sinR = sin(rotAngle), cosR = cos(rotAngle);
                float az = -cos(angle);
                return localToWorldHorizon(normalize(float3(
                    az * cosR - 0.3 * sinR,
                    sin(angle),
                    az * sinR + 0.3 * cosR
                )));
            }

            float3 getSeasonalSunDir(float timeOfDay, float skyRotation)
            {
                float lat = radians(_CatalogLatitude);
                float dec = radians(_SolarDeclination);
                float hourAngle = (timeOfDay - 0.5) * UNITY_TWO_PI;

                float sinLat = sin(lat), cosLat = cos(lat);
                float sinDec = sin(dec), cosDec = cos(dec);
                float sinH = sin(hourAngle), cosH = cos(hourAngle);

                float east  = cosDec * sinH;
                float up    =  sinLat * sinDec + cosLat * cosDec * cosH;
                float north =  cosLat * sinDec - sinLat * cosDec * cosH;

                float rotAngle = skyRotation * UNITY_TWO_PI;
                float sinR = sin(rotAngle), cosR = cos(rotAngle);
                return localToWorldHorizon(normalize(float3(
                    east * cosR - north * sinR,
                    up,
                    east * sinR + north * cosR
                )));
            }

            float3 getEquatorialLocalDir(float rightAscension01, float declinationDeg)
            {
                float lat = radians(_CatalogLatitude);
                float dec = radians(declinationDeg);
                float lst = frac(getEffectiveCatalogSiderealRotation() + _CatalogLongitude / 360.0) * UNITY_TWO_PI;
                float hourAngle = lst - rightAscension01 * UNITY_TWO_PI;

                float sinLat = sin(lat), cosLat = cos(lat);
                float sinDec = sin(dec), cosDec = cos(dec);
                float sinH = sin(hourAngle), cosH = cos(hourAngle);

                return normalize(float3(
                    cosDec * sinH,
                    sinLat * sinDec + cosLat * cosDec * cosH,
                    cosLat * sinDec - sinLat * cosDec * cosH
                ));
            }

            float3 getRealMoonDir()
            {
                return localToWorldHorizon(getEquatorialLocalDir(_MoonRightAscension, _MoonDeclination));
            }

            //水平面での太陽方向との一致度 (1=太陽側, -1=反対側)
            float horizonDot(float3 rayDir, float3 sunDir)
            {
                float2 sunH = float2(sunDir.x, sunDir.z);
                float2 rayH = float2(rayDir.x, rayDir.z);
                float sl = length(sunH), rl = length(rayH);
                return (sl > 0.001 && rl > 0.001)
                    ? dot(sunH / sl, rayH / rl) : 0.0;
            }

            float3 computeSunColor(float3 sunDir, float timeOfDay)
            {
                float lowSun = (1.0 - smoothstep(0.02, 0.34, sunDir.y))
                             * smoothstep(-0.12, 0.04, sunDir.y);
                float morning = 1.0 - smoothstep(0.22, 0.50, timeOfDay);
                float evening = smoothstep(0.50, 0.78, timeOfDay);
                float3 lowSunColor = lerp(_SunriseColor.rgb, _SunsetColor.rgb, saturate(evening / max(morning + evening, 0.001)));
                float3 daySunColor = float3(1.0, 0.95, 0.82);
                return lerp(daySunColor, lowSunColor, lowSun * saturate(_LowSunColorStrength));
            }

            float solarEclipseCoverage(float3 sunDir, float3 moonDir)
            {
                float sunRadius = max(_SunSize, 0.0001);
                float moonRadius = max(_MoonSize, 0.0001);
                float cosSeparation = clamp(dot(sunDir, moonDir), -1.0, 1.0);
                if (cosSeparation <= cos(sunRadius + moonRadius))
                    return 0.0;

                float separation = acos(cosSeparation);
                if (separation >= sunRadius + moonRadius)
                    return 0.0;

                float sunArea = UNITY_PI * sunRadius * sunRadius;
                if (separation <= abs(sunRadius - moonRadius))
                {
                    float inner = min(sunRadius, moonRadius);
                    return saturate((UNITY_PI * inner * inner) / max(sunArea, 0.000001));
                }

                float d2 = separation * separation;
                float s2 = sunRadius * sunRadius;
                float m2 = moonRadius * moonRadius;
                float angleS = acos(clamp((d2 + s2 - m2) / (2.0 * separation * sunRadius), -1.0, 1.0));
                float angleM = acos(clamp((d2 + m2 - s2) / (2.0 * separation * moonRadius), -1.0, 1.0));
                float root = sqrt(max(0.0,
                    (-separation + sunRadius + moonRadius)
                  * ( separation + sunRadius - moonRadius)
                  * ( separation - sunRadius + moonRadius)
                  * ( separation + sunRadius + moonRadius)));
                float overlap = s2 * angleS + m2 * angleM - 0.5 * root;
                return saturate(overlap / max(sunArea, 0.000001));
            }

            float sunEclipseDiskMask(float3 rayDir, float3 moonDir)
            {
                float cosM = dot(rayDir, moonDir);
                float dist = acos(clamp(cosM, -1.0, 1.0));
                float aa = max(fwidth(dist) * 0.75, 0.00002);
                return (1.0 - smoothstep(max(_MoonSize - aa, 0.0001), _MoonSize + aa, dist)) * step(0.0, cosM);
            }

            float3 computeSunDisk(float3 rayDir, float3 sunDir, float3 moonDir, float timeOfDay, float eclipse)
            {
                float eclipseAmount = saturate(eclipse);
                float cosTheta = dot(rayDir, sunDir);
                float dist = acos(saturate(cosTheta));
                float softness = min(max(_SunDiskSoftness, 0.0005), _SunSize * 0.95);
                float disk = 1.0 - smoothstep(max(_SunSize - softness, 0.0001), _SunSize, dist);
                float glare = pow(saturate(1.0 - dist / max(_SunSize * 4.2, 0.001)), 3.0) * 0.42;
                if (eclipseAmount > 0.0001)
                {
                    float moonMask = sunEclipseDiskMask(rayDir, moonDir);
                    disk *= 1.0 - moonMask;
                }
                glare *= lerp(1.0, 0.22 * 0.3, eclipseAmount);
                float horizonOffset = max(_SunSize * 1.4, 0.012);
                float diskVis = smoothstep(-horizonOffset, _SunSize * 0.85, sunDir.y);
                float glareVis = smoothstep(-horizonOffset * 0.55, max(_SunSize * 4.0, 0.035), sunDir.y);
                return computeSunColor(sunDir, timeOfDay) * (disk * diskVis + glare * glareVis) * _SunDiskIntensity;
            }

            float3 computeSky(float3 rayDir, float3 sunDir, float timeOfDay, float eclipse)
            {
                float cosTheta = dot(rayDir, sunDir);
                float hd       = horizonDot(rayDir, sunDir);

                //天頂・天底付近でhd不定になるので方向性を消す
                float horizWeight = saturate(1.0 - rayDir.y * rayDir.y * 2.5);

                float sunY      = sunDir.y;
                float dayT      = smoothstep(0.30, 0.62, sunY);
                float sunAboveT = smoothstep(-0.03, 0.20, sunY);
                float nightT    = 1.0 - smoothstep(-0.36, -0.18, sunY);
                float sunSide   = saturate(hd * 0.5 + 0.5);
                float antiSide  = saturate(-hd * 0.5 + 0.5);
                float midT      = smoothstep(-0.02, 0.36, rayDir.y);
                float topT      = smoothstep(0.24, 0.90, rayDir.y);

                float nightToNautical = smoothstep(-0.38, -0.22, sunY);
                float nauticalToCivil = smoothstep(-0.22, -0.07, sunY);
                float civilToSunset   = smoothstep(-0.07,  0.05, sunY);
                float sunsetToEvening = smoothstep( 0.03,  0.20, sunY);
                float eveningToDay    = smoothstep( 0.34,  0.62, sunY);
                float eveningTimeGate = smoothstep(16.0 / 24.0, 16.5 / 24.0, timeOfDay);
                float morningTimeGate = 1.0 - smoothstep(6.0 / 24.0, 6.35 / 24.0, timeOfDay);
                float warmTimeGate    = saturate(max(eveningTimeGate, morningTimeGate));

                float3 dayHorizon       = float3(0.30, 0.52, 0.78);
                float3 dayZenith        = float3(0.035, 0.16, 0.62);
                float3 eveningHorizon   = float3(0.78, 0.48, 0.30);
                float3 eveningMid       = float3(0.38, 0.35, 0.50);
                float3 eveningZenith    = float3(0.075, 0.145, 0.34);
                float3 sunsetHorizon    = float3(0.095, 0.120, 0.230);
                float3 sunsetMid        = float3(0.050, 0.095, 0.215);
                float3 sunsetZenith     = float3(0.006, 0.018, 0.075);
                float3 civilHorizon     = float3(0.045, 0.070, 0.145);
                float3 civilMid         = float3(0.026, 0.055, 0.135);
                float3 civilZenith      = float3(0.004, 0.014, 0.060);
                float3 nauticalHorizon  = float3(0.014, 0.030, 0.075);
                float3 nauticalMid      = float3(0.008, 0.022, 0.065);
                float3 nauticalZenith   = float3(0.002, 0.008, 0.034);
                float3 nightHorizon     = float3(0.004, 0.008, 0.024);
                float3 nightZenith      = float3(0.001, 0.004, 0.018);
                float3 antisolarHorizon = float3(0.002, 0.005, 0.022);
                float3 antisolarZenith  = float3(0.001, 0.003, 0.016);

                float3 nightSky    = lerp(nightHorizon, nightZenith, saturate(rayDir.y * 1.2 + 0.1));
                float3 nauticalSky = lerp(nauticalHorizon, nauticalMid, midT);
                       nauticalSky = lerp(nauticalSky, nauticalZenith, topT);
                float3 civilSky    = lerp(civilHorizon, civilMid, midT);
                       civilSky    = lerp(civilSky, civilZenith, topT);
                float3 sunsetSky   = lerp(sunsetHorizon, sunsetMid, midT);
                       sunsetSky   = lerp(sunsetSky, sunsetZenith, topT);
                float3 eveningSky  = lerp(eveningHorizon, eveningMid, midT);
                       eveningSky  = lerp(eveningSky, eveningZenith, topT);
                float3 daySky      = lerp(dayHorizon, dayZenith, saturate(rayDir.y * 1.4 + 0.25));
                float3 gatedEveningSky = lerp(daySky, eveningSky, warmTimeGate);

                float3 skyGrad = lerp(nightSky, nauticalSky, nightToNautical);
                       skyGrad = lerp(skyGrad, civilSky, nauticalToCivil);
                       skyGrad = lerp(skyGrad, sunsetSky, civilToSunset);
                       skyGrad = lerp(skyGrad, gatedEveningSky, sunsetToEvening);
                       skyGrad = lerp(skyGrad, daySky, eveningToDay);

                float nearHorizon = 1.0 - smoothstep(0.02, 0.30, abs(sunY));
                float belowHorizon = 1.0 - smoothstep(-0.08, 0.04, sunY);

                float sunsetHeightWhenHigh = 0.18;
                float sunsetHeightAtHorizon = 0.34;
                float sunsetHeightAfterSet = 0.16;
                float heightBeforeSet = lerp(sunsetHeightWhenHigh, sunsetHeightAtHorizon, nearHorizon);
                float sunsetHeight = lerp(heightBeforeSet, sunsetHeightAfterSet, belowHorizon);
                float sunsetVertical = pow(1.0 - smoothstep(-0.035, sunsetHeight, rayDir.y), 1.25);

                float sidePowerHigh = 4.8;
                float sidePowerHorizon = 2.6;
                float sidePowerAfterSet = 4.8;
                float sidePowerBeforeSet = lerp(sidePowerHigh, sidePowerHorizon, nearHorizon);
                float sidePower = lerp(sidePowerBeforeSet, sidePowerAfterSet, belowHorizon);
                float sunsetAzimuth = pow(sunSide, sidePower);

                float eveningHeightWarm = smoothstep(0.05, 0.32, sunY) * (1.0 - smoothstep(0.50, 0.72, sunY));
                float morningHeightWarm = smoothstep(-0.08, -0.02, sunY) * (1.0 - smoothstep(0.05, 0.14, sunY));
                float eveningWarm = saturate(eveningHeightWarm * eveningTimeGate);
                float morningWarm = saturate(morningHeightWarm * morningTimeGate);
                float eveningWash = eveningWarm * (0.18 + 0.20 * horizWeight);
                float morningWash = morningWarm * (0.03 + 0.10 * horizWeight);
                float3 eveningWashColor = lerp(float3(0.95, 0.54, 0.28), float3(0.72, 0.40, 0.28), topT);
                float3 morningWashColor = lerp(float3(0.78, 0.34, 0.24), float3(0.40, 0.30, 0.34), topT);
                       skyGrad = lerp(skyGrad, eveningWashColor, saturate(eveningWash));
                       skyGrad = lerp(skyGrad, morningWashColor, saturate(morningWash));

                float warmBand = sunsetAzimuth * sunsetVertical * max(nearHorizon, eveningWarm * 0.76) * horizWeight;
                float warmPeak = 1.0 - smoothstep(0.00, 0.22, abs(sunY));
                float3 warmHorizon = lerp(float3(0.92, 0.50, 0.24), float3(0.96, 0.38, 0.26), warmPeak);
                       skyGrad = lerp(skyGrad, warmHorizon, saturate(warmBand));

                       skyGrad *= _RayleighStrength;

                float azimuthWeight = 1.0 - smoothstep(0.78, 0.97, rayDir.y);
                float antiDarkBySunHeight = (1.0 - smoothstep(-0.08, 0.16, sunY)) * 0.65
                                          + (1.0 - smoothstep(-0.24, -0.06, sunY)) * 0.35;
                float settingRayleighKill = 1.0 - smoothstep(-0.02, 0.14, sunY);
                float submergedRayleighKill = 1.0 - smoothstep(-0.20, -0.06, sunY);
                      antiDarkBySunHeight = saturate(antiDarkBySunHeight + settingRayleighKill * 0.35 + submergedRayleighKill * 0.25);

                float antiVertical = 0.45 + (1.0 - smoothstep(-0.03, 0.30, rayDir.y)) * 0.55;
                      antiVertical = lerp(antiVertical, 0.72 + (1.0 - smoothstep(-0.03, 0.36, rayDir.y)) * 0.28, settingRayleighKill);

                float antiShape = lerp(pow(antiSide, 1.45), pow(antiSide, 0.85), settingRayleighKill);
                float antiDark = antiShape * antiDarkBySunHeight * antiVertical * (1.0 - sunAboveT * 0.35) * azimuthWeight;
                float3 antiSky     = lerp(antisolarHorizon, antisolarZenith, saturate(rayDir.y * 1.1 + 0.1));
                       skyGrad     = lerp(skyGrad, antiSky, saturate(antiDark));

                float3 sunColor = computeSunColor(sunDir, timeOfDay);

                float directSunScatter = smoothstep(-0.01, 0.10, sunY);
                float eclipseDay = eclipse * smoothstep(-0.03, 0.18, sunY);
                float sunTransmission = lerp(1.0, 0.08 * 0.3, eclipseDay);
                float mie      = miePhase(cosTheta, kMieG) * _MieStrength * lerp(0.035, 0.25, max(dayT, sunAboveT * 0.75)) * directSunScatter * sunTransmission;
                float atmBright = lerp(0.50, 1.0, max(dayT, sunAboveT * 0.72)) * (1.0 - nightT * 0.28);
                atmBright *= lerp(1.0, 0.36 * 0.3, eclipseDay);
                float3 sky     = (skyGrad + mie * sunColor) * _SunIntensity * atmBright;

                return sky;
            }

            float3 computeMoon(float3 rayDir, float3 sunDir, float3 moonDir)
            {
                float cosM = dot(rayDir, moonDir);
                float dist = acos(clamp(cosM, -1.0, 1.0));
                float frontMask = step(0.0, cosM);

                float3 up        = float3(0, 1, 0);
                float3 moonRight = normalize(cross(moonDir, up));
                float3 moonUp    = normalize(cross(moonRight, moonDir));

                float3 lightDir = sunDir - moonDir * dot(sunDir, moonDir);
                if (dot(lightDir, lightDir) > 0.0001)
                    moonRight = normalize(lightDir);
                moonUp = normalize(cross(moonRight, moonDir));

                float2 moonUv = float2(dot(rayDir, moonRight), dot(rayDir, moonUp)) / max(_MoonSize, 0.0001);
                float diskRadius = length(moonUv);

                float haloRadius = max(_MoonSize * lerp(2.5, 18.0, saturate(_MoonHaloStrength)), 0.001);
                float halo = pow(saturate(1.0 - dist / haloRadius), 2.2)
                           * saturate(_MoonHaloStrength) * 0.9;
                float aa = max(fwidth(dist) * 0.5, 0.000025);
                float uvAa = max(fwidth(diskRadius), aa / max(_MoonSize, 0.0001));
                float moonDisk = (1.0 - smoothstep(1.0 - uvAa, 1.0 + uvAa, diskRadius)) * frontMask;
                float chord = sqrt(saturate(1.0 - moonUv.y * moonUv.y));
                float phaseCurve = cos(saturate(_MoonPhase) * UNITY_PI);
                float terminator = phaseCurve * chord;
                float phaseBlur = uvAa + 0.046;
                float lightMask = smoothstep(terminator - phaseBlur, terminator + phaseBlur, moonUv.x);
                float crescent = moonDisk * lightMask;

                float phaseVis    = saturate(_MoonPhase * 8.0);
                float nightFactor = saturate(-sunDir.y * 5.0);
                float moonBodyVisibility = lerp(0.28, 1.0, nightFactor);

                return (halo * frontMask * _MoonHaloColor.rgb * nightFactor + crescent * _MoonColor.rgb * moonBodyVisibility) * phaseVis;
            }

            float moonOcclusion(float3 rayDir, float3 moonDir)
            {
                float cosM = dot(rayDir, moonDir);
                float dist = acos(clamp(cosM, -1.0, 1.0));
                float diskRadius = dist / max(_MoonSize, 0.0001);
                float uvAa = max(fwidth(diskRadius), 0.0005);
                return (1.0 - smoothstep(1.0 - uvAa, 1.0 + uvAa, diskRadius)) * step(0.0, cosM);
            }

            float moonStarSuppression(float3 rayDir, float3 moonDir)
            {
                float dist = acos(saturate(dot(rayDir, moonDir)));
                float inner = _MoonSize;
                float outer = _MoonSize * max(_MoonStarRange, 1.0) * 3.0;
                float fade = 1.0 - smoothstep(inner, outer, dist);
                return fade * saturate(_MoonStarSuppress);
            }

            float3 voronoiHash3D(float3 p, float3 seed)
            {
                p += seed;
                p = float3(
                    dot(p, float3(127.1, 311.7,  74.7)),
                    dot(p, float3(269.5, 183.3, 246.1)),
                    dot(p, float3(113.5, 271.9, 124.6))
                );
                return frac(sin(p) * 43758.5453);
            }

            float voronoi3D(float3 v, float time, float3 hashSeed)
            {
                float3 n = floor(v);
                float3 f = frac(v);
                float F1 = 8.0, F2 = 8.0;
                for (int k = -1; k <= 1; k++)
                for (int j = -1; j <= 1; j++)
                for (int ii = -1; ii <= 1; ii++)
                {
                    float3 g = float3(ii, j, k);
                    float3 o = voronoiHash3D(n + g, hashSeed);
                    o = sin(time + o * 6.2831) * 0.5 + 0.5;
                    float3 r = f - g - o;
                    float  d = dot(r, r);
                    if (d < F1) { F2 = F1; F1 = d; }
                    else if (d < F2) { F2 = d; }
                }
                return (F2 + F1) * 0.5;
            }

            void voronoiStarData(float3 v, float time, float3 hashSeed,
                                 out float nearestD, out float3 nearestR,
                                 out float randomPick, out float randomLum)
            {
                float3 n = floor(v);
                float3 f = frac(v);
                nearestD = 8.0;
                nearestR = float3(0, 0, 0);
                randomPick = 0.0;
                randomLum = 0.0;

                for (int k = -1; k <= 1; k++)
                for (int j = -1; j <= 1; j++)
                for (int ii = -1; ii <= 1; ii++)
                {
                    float3 g = float3(ii, j, k);
                    float3 h = voronoiHash3D(n + g, hashSeed);
                    float3 o = sin(time + h * 6.2831) * 0.5 + 0.5;
                    float3 r = f - g - o;
                    float  d = dot(r, r);
                    if (d < nearestD)
                    {
                        nearestD = d;
                        nearestR = r;
                        randomPick = h.x;
                        randomLum = frac(h.y * 1.37 + h.z * 0.73);
                    }
                }
            }

            float starTwinkleFactor(float brightness, float seed)
            {
                float strength = saturate(_StarTwinkleStrength);
                if (_StarTwinkleSpeed <= 0.001 || strength <= 0.001)
                    return 1.0;

                float b = smoothstep(0.035, 0.85, saturate(brightness));
                float range = saturate(_StarTwinkleRange);
                float brightLimit = lerp(0.86, -0.08, range);
                float brightMask = smoothstep(brightLimit, brightLimit + 0.16, b);
                float darkLimit = lerp(0.14, 1.20, range);
                float darkMask = 1.0 - smoothstep(darkLimit - 0.16, darkLimit, b);
                float mask = _StarTwinkleInvert > 0.5 ? darkMask : brightMask;

                float phase = seed * 37.719 + sin(seed * 19.17) * 2.7;
                float wave = sin(_Time.y * _StarTwinkleSpeed * 4.0 + phase) * 0.5 + 0.5;
                float fine = sin(_Time.y * _StarTwinkleSpeed * 9.7 + seed * 91.31) * 0.5 + 0.5;
                float baseAmount = _StarTwinkleInvert > 0.5
                    ? lerp(0.22, 0.14, b)
                    : lerp(0.10, 0.28, b);
                float amount = baseAmount * strength;
                return lerp(1.0, 1.0 - amount + (wave * 0.75 + fine * 0.25) * amount * 2.0, mask);
            }

            float3 starLayer(float3 dir, float3 scaledDir, float seedAngle, float3 hashSeed,
                             float density, float3 color, float amount)
            {
                float nearestD;
                float3 nearestR;
                float randomPick;
                float randomLum;
                voronoiStarData(scaledDir, seedAngle, hashSeed, nearestD, nearestR, randomPick, randomLum);

                float dist = sqrt(nearestD);
                float layerDensity = density + (1.0 - amount) * 1000.0;
                float densityChance = max(0.0001, saturate(_StarDensity * amount));
                float densityMask = smoothstep(1.0 - densityChance, 1.0, randomPick);
                if (densityMask <= 0.0)
                    return float3(0, 0, 0);

                float starMagnitude = smoothstep(0.0, 1.0, randomLum);
                float colorLuminance = saturate(dot(color, float3(0.299, 0.587, 0.114)));
                float crossPriority = saturate(colorLuminance * 0.82 + starMagnitude * 0.18);
                float core = pow(1.0 - saturate(dist * 1.25), layerDensity);
                float bright = lerp(0.18, 2.25, pow(starMagnitude, 1.35));

                float haloMask = smoothstep(0.50, 1.0, starMagnitude);
                float halo = 0.0;
                if (_StarGlowStrength > 0.001 && haloMask > 0.001)
                {
                    float haloRadius = max(0.012, _StarGlowSize * 0.035);
                    halo = exp2(-(dist * dist) / (haloRadius * haloRadius))
                         * _StarGlowStrength * haloMask * bright * 0.45;
                }

                float crossAmount = saturate(_StarCrossThreshold);
                float crossAmountCurve = pow(crossAmount, 2.4);
                float crossCutoff = lerp(1.04, -0.02, crossAmountCurve);
                float selectedByMagnitude = smoothstep(crossCutoff - 0.06, crossCutoff + 0.06, crossPriority);
                float crossMask = selectedByMagnitude * smoothstep(0.015, 0.12, crossAmount);
                float crossValue = 0.0;
                if (crossMask > 0.001 && _StarCrossOpacity > 0.001)
                {
                    float rot = _StarCrossRotation * UNITY_TWO_PI;
                    float3 up = abs(dir.y) < 0.96 ? float3(0, 1, 0) : float3(1, 0, 0);
                    float3 tangent = normalize(cross(up, dir));
                    float3 bitangent = cross(dir, tangent);
                    float2 p = float2(dot(nearestR, tangent), dot(nearestR, bitangent));
                    float2 rp = float2(
                        p.x * cos(rot) - p.y * sin(rot),
                        p.x * sin(rot) + p.y * cos(rot)
                    );
                    float major = max(abs(rp.x), abs(rp.y));
                    float minor = min(abs(rp.x), abs(rp.y));
                    float armLength = max(0.010, _StarCrossLength * 0.030);
                    float armWidth  = max(0.0014, _StarCrossSize * 0.0024);
                    float crossVisibleGate = smoothstep(0.0, 0.018, core * bright)
                                           + smoothstep(0.0, 0.55, starMagnitude)
                                           * pow(1.0 - saturate(dist / (armLength * 1.35)), 1.6);
                          crossVisibleGate = saturate(crossVisibleGate);
                    float crossGlowComp = 1.0 + saturate(_StarGlowStrength) * 0.45;
                    float crossCore = exp2(-(minor * minor) / (armWidth * armWidth))
                                    * exp2(-(major * major) / (armLength * armLength * 0.75));
                    float crossHaloWidth = armWidth * (3.0 + _StarGlowSize * 0.8);
                    float crossHaloLength = armLength * (1.15 + _StarGlowSize * 0.10);
                    float crossHalo = exp2(-(minor * minor) / (crossHaloWidth * crossHaloWidth))
                                    * exp2(-(major * major) / (crossHaloLength * crossHaloLength))
                                    * _StarGlowStrength * 0.18;
                    crossValue = (crossCore + crossHalo)
                               * crossVisibleGate * crossMask * _StarCrossOpacity * bright * crossGlowComp;
                }

                float twinkle = starTwinkleFactor(starMagnitude, randomPick);
                return color * (core * bright + halo + crossValue) * densityMask * twinkle;
            }

            float3 computeStars(float3 rayDir, float3 sunDir, float3 moonDir)
            {
                float3 dir     = normalize(rayDir);
                float3 axisRaw = float3(_StarAxisX, _StarAxisY, _StarAxisZ);
                float3 axis    = length(axisRaw) > 0.001 ? normalize(axisRaw) : float3(0, 1, 0);
                float  rotAngle = _Time.y * _StarRotateSpeed * 0.1;
                float  cosR = cos(rotAngle), sinR = sin(rotAngle);
                dir = dir * cosR + cross(axis, dir) * sinR + axis * dot(axis, dir) * (1.0 - cosR);

                float3 scaledDir = dir * _StarScale;
                float  seedAngle = radians((_StarSeed + 0.5) * 180.0);
                float  density   = (_StarDensity + 0.02) * 1000.0;

                float3 stars = starLayer(dir, scaledDir, seedAngle, float3(0, 0, 0),
                                         density, _Star1Color.rgb, 1.0);

                if (_Star2Amount >= 0.001)
                {
                    stars += starLayer(dir, scaledDir, seedAngle * 2.0, float3(31.4, 17.2, 53.1),
                                       density, _Star2Color.rgb, _Star2Amount);
                }
                if (_Star3Amount >= 0.001)
                {
                    stars += starLayer(dir, scaledDir, seedAngle * 3.0, float3(62.8, 34.4, 91.7),
                                       density, _Star3Color.rgb, _Star3Amount);
                }

                float horizFade = smoothstep(0.0, _StarFadeStrength * 2.0,
                                             -rayDir.y + (_StarFadeHeight * 3.0 - 1.0));
                stars = lerp(stars, float3(0,0,0), horizFade);

                float hd = horizonDot(rayDir, sunDir);
                float dawnFade = pow(saturate(hd * 0.5 + 0.5), 2.0)
                               * saturate(1.0 - abs(sunDir.y) * 5.0);
                stars = lerp(stars, float3(0,0,0), dawnFade);

                return stars * _StarBrightness;
            }

            float2 catalogDataUV(float index)
            {
                float x = fmod(index, _CatalogDataMapWidth);
                float y = floor(index / _CatalogDataMapWidth);
                return (float2(x, y) + 0.5) * _CatalogStarDataMap_TexelSize.xy;
            }

            float2 catalogCellUV(float cellX, float cellY)
            {
                cellX = fmod(cellX + _CatalogCellLonCount, _CatalogCellLonCount);
                cellY = clamp(cellY, 0.0, _CatalogCellLatCount - 1.0);
                return (float2(cellX, cellY) + 0.5) * _CatalogStarCellMap_TexelSize.xy;
            }

            float3 rotateAroundY(float3 dir, float rotation01)
            {
                float a = rotation01 * UNITY_TWO_PI;
                float s = sin(a), c = cos(a);
                return float3(dir.x * c - dir.z * s, dir.y, dir.x * s + dir.z * c);
            }

            float3 localHorizonToEquatorial(float3 localDir)
            {
                localDir = worldToLocalHorizon(localDir);
                float lat = radians(_CatalogLatitude);
                float sinLat = sin(lat), cosLat = cos(lat);

                float east = -localDir.x;
                float up = localDir.y;
                float north = localDir.z;

                float qx = up * cosLat - north * sinLat;
                float qy = north * cosLat + up * sinLat;
                float qz = east;

                float lst = frac(getEffectiveCatalogSiderealRotation() + _CatalogLongitude / 360.0) * UNITY_TWO_PI;
                float s = sin(lst), c = cos(lst);
                return normalize(float3(qx * c + qz * s, qy, qx * s - qz * c));
            }

            float3 catalogStarSample(float3 dir, float starIndex)
            {
                if (starIndex < 0.0 || starIndex >= _CatalogStarCount)
                    return float3(0, 0, 0);

                float2 uv = catalogDataUV(starIndex);
                float4 data = tex2Dlod(_CatalogStarDataMap, float4(uv, 0.0, 0.0));

                float3 starDir = normalize(data.xyz);
                float brightness = data.w;
                float d = 1.0 - saturate(dot(dir, starDir));
                float size = max(_CatalogStarSize, 0.001);
                float sharpness = max(_CatalogStarSharpness, 0.001);
                float sharpT = saturate((sharpness - 0.1) / 29.9);
                float sharpEffect = lerp(0.35, 6.0, pow(sharpT, 0.72));
                float sizeByMag = lerp(4500000.0, 900000.0, brightness) * sharpEffect;
                float glow = saturate(_CatalogStarGlow) * 0.3;
                float sharpGlowScale = lerp(1.35, 0.62, sharpT);
                float haloMask = smoothstep(0.45, 1.0, brightness);
                float haloWidth = lerp(0.010, 0.0022, glow) * sharpGlowScale;
                float contributionCull = 32.0;
                float coreCullD = size * contributionCull / max(sizeByMag, 0.000001);
                float haloCullD = (glow > 0.001 && haloMask > 0.001)
                    ? size * contributionCull / max(sizeByMag * haloWidth, 0.000001)
                    : 0.0;
                float bodyCullD = max(coreCullD, haloCullD);
                float3 color = float3(0, 0, 0);
                float mag = 7.0;
                float selected = 0.0;
                if (d > bodyCullD)
                {
                    if (_StarCrossThreshold <= 0.001 || _StarCrossOpacity <= 0.001)
                        return float3(0, 0, 0);

                    float4 colorData = tex2Dlod(_CatalogStarColorMap, float4(uv, 0.0, 0.0));
                    color = colorData.rgb;
                    mag = colorData.a;
                    float crossPriority = saturate((7.0 - mag) / 7.0);
                    selected = smoothstep(1.0 - _StarCrossThreshold, 1.0, crossPriority);
                    if (selected <= 0.001)
                        return float3(0, 0, 0);

                    float armLengthCull = max(0.00012, _StarCrossLength * 0.00070 * _CatalogStarSize) * 6.0;
                    float crossCullD = 1.0 - cos(min(armLengthCull, UNITY_PI));
                    if (d > crossCullD)
                        return float3(0, 0, 0);
                }
                else
                {
                    float4 colorData = tex2Dlod(_CatalogStarColorMap, float4(uv, 0.0, 0.0));
                    color = colorData.rgb;
                    mag = colorData.a;
                    if (_StarCrossThreshold > 0.001 && _StarCrossOpacity > 0.001)
                    {
                        float crossPriority = saturate((7.0 - mag) / 7.0);
                        selected = smoothstep(1.0 - _StarCrossThreshold, 1.0, crossPriority);
                    }
                }

                float core = exp2(-d * sizeByMag / size);
                core = pow(saturate(core), lerp(0.72, 8.0, sharpT));
                core *= lerp(1.0, 2.35, sharpT);
                float halo = 0.0;
                if (glow > 0.001 && haloMask > 0.001)
                {
                    halo = exp2(-d * sizeByMag * haloWidth / size)
                         * haloMask
                         * brightness * (glow * glow) * 0.42 * sharpGlowScale;
                }

                float crossFlare = 0.0;
                if (_StarCrossThreshold > 0.001 && selected > 0.001 && _StarCrossOpacity > 0.001)
                {
                    float3 up = abs(dir.y) < 0.96 ? float3(0, 1, 0) : float3(1, 0, 0);
                    float3 tangent = normalize(cross(up, starDir));
                    float3 bitangent = cross(starDir, tangent);
                    float2 p = float2(dot(dir - starDir, tangent), dot(dir - starDir, bitangent));
                    float rot = _StarCrossRotation * UNITY_TWO_PI;
                    float2 rp = float2(
                        p.x * cos(rot) - p.y * sin(rot),
                        p.x * sin(rot) + p.y * cos(rot)
                    );
                    float major = max(abs(rp.x), abs(rp.y));
                    float minor = min(abs(rp.x), abs(rp.y));
                    float armLength = max(0.00012, _StarCrossLength * 0.00070 * _CatalogStarSize);
                    float armWidth  = max(0.000012, _StarCrossSize * 0.000055 * _CatalogStarSize);
                    crossFlare = exp2(-(minor * minor) / (armWidth * armWidth))
                          * exp2(-(major * major) / (armLength * armLength))
                          * selected * _StarCrossOpacity * brightness;
                }

                float lum = brightness * brightness;
                float seed = frac(dot(starDir, float3(12.9898, 78.233, 37.719)) * 43758.5453);
                float twinkle = starTwinkleFactor(brightness, seed);
                return color * (core * lum * 3.6 + halo * lum + crossFlare) * twinkle;
            }

            float3 equatorialDir(float raDeg, float decDeg)
            {
                float ra = radians(raDeg);
                float dec = radians(decDeg);
                float cosDec = cos(dec);
                return normalize(float3(cosDec * cos(ra), sin(dec), cosDec * sin(ra)));
            }

            float3 catalogOverlayDir(float3 rayDir)
            {
                float3 dir = normalize(rayDir);
                float3 localDir = worldToLocalHorizon(dir);
                return _CatalogUseLocation > 0.5
                    ? localHorizonToEquatorial(dir)
                    : rotateAroundY(float3(-localDir.x, localDir.y, localDir.z), _CatalogStarRotation);
            }

            float orionLineSegment(float3 p, float3 a, float3 b, float width)
            {
                float3 ab = b - a;
                float t = saturate(dot(p - a, ab) / max(dot(ab, ab), 0.000001));
                float3 c = normalize(a + ab * t);
                float dist = length(p - c);
                return 1.0 - smoothstep(width, width * 1.75, dist);
            }

            float3 computeOrionLines(float3 rayDir)
            {
                if (_UseOrionLines < 0.5 || _OrionLineStrength <= 0.001)
                    return float3(0, 0, 0);

                float3 p = catalogOverlayDir(rayDir);
                float width = max(_OrionLineWidth, 0.00001);

                float3 betelgeuse = equatorialDir(88.7929,  7.4071);
                float3 bellatrix  = equatorialDir(81.2828,  6.3497);
                float3 mintaka    = equatorialDir(83.0017, -0.2991);
                float3 alnilam    = equatorialDir(84.0534, -1.2019);
                float3 alnitak    = equatorialDir(85.1897, -1.9426);
                float3 rigel      = equatorialDir(78.6345, -8.2016);
                float3 saiph      = equatorialDir(86.9391, -9.6696);

                float orionMask01 = orionLineSegment(p, betelgeuse, bellatrix, width);
                float orionMask02 = orionLineSegment(p, betelgeuse, alnitak, width);
                float orionMask03 = orionLineSegment(p, bellatrix, mintaka, width);
                float orionMask04 = orionLineSegment(p, mintaka, alnilam, width);
                float orionMask05 = orionLineSegment(p, alnilam, alnitak, width);
                float orionMask06 = orionLineSegment(p, mintaka, rigel, width);
                float orionMask07 = orionLineSegment(p, alnitak, saiph, width);
                float orionMask08 = orionLineSegment(p, rigel, saiph, width);
                float orionMask = orionMask01 + orionMask02 + orionMask03 + orionMask04
                                + orionMask05 + orionMask06 + orionMask07 + orionMask08;
                orionMask = saturate(orionMask);

                return _OrionLineColor.rgb * orionMask * _OrionLineStrength;
            }

            float3 computeMilkyWay(float3 rayDir)
            {
                if (_UseMilkyWay < 0.5 || _MilkyWayStrength <= 0.001)
                    return float3(0, 0, 0);

                float3 d = catalogOverlayDir(rayDir);
                if (_UseMilkyWayDensityMap > 0.5)
                {
                    float u = frac(atan2(d.z, d.x) / UNITY_TWO_PI + 1.0);
                    float v = asin(clamp(d.y, -1.0, 1.0)) / UNITY_PI + 0.5;
                    float4 densityData = tex2D(_MilkyWayDensityMap, float2(u, v));
                    float density = max(max(densityData.r, densityData.g), densityData.b);
                    float baseBand = smoothstep(0.52, 0.92, density);
                    float shaped = pow(baseBand, lerp(1.8, 0.75, saturate(_MilkyWayDetail)));
                    float3 catalogColor = density > 0.0001 ? densityData.rgb / density : 1.0;
                    float luminance = dot(catalogColor, float3(0.2126, 0.7152, 0.0722));
                    catalogColor = lerp(luminance.xxx, catalogColor, _MilkyWaySaturation);
                    catalogColor *= lerp(float3(1.0, 1.0, 1.0), float3(0.88, 1.06, 1.16), saturate(_MilkyWayCyanBoost));
                    catalogColor = lerp(saturate(catalogColor), _MilkyWayColor.rgb, saturate(_MilkyWayTintAmount));
                    float coreMask = smoothstep(0.68, 1.0, density) * shaped;
                    catalogColor = lerp(catalogColor, float3(1.0, 0.55, 0.78), coreMask * saturate(_MilkyWayCoreWarmth));
                    if (_UseMilkyWayDustMap > 0.5)
                    {
                        float dust = tex2D(_MilkyWayDustMap, float2(u, v)).r;
                        float dustLane = smoothstep(0.30, 0.88, dust);
                        shaped *= 1.0 - dustLane * saturate(_MilkyWayDustStrength);
                    }
                    return catalogColor * shaped * _MilkyWayStrength * 0.55;
                }

                float3 galacticNorth = equatorialDir(192.8595, 27.1283);
                float plane = abs(dot(d, galacticNorth));
                float width = max(_MilkyWayWidth, 0.001);
                float band = exp2(-(plane * plane) / (width * width));
                float n1 = sin(dot(d, float3(2.1, 5.7, 3.4)) * 5.0);
                float n2 = sin(dot(d, float3(-4.3, 1.9, 6.2)) * 8.0);
                float n3 = sin(dot(d, float3(7.1, -3.6, 2.8)) * 13.0);
                float cloud = (n1 * 0.5 + n2 * 0.32 + n3 * 0.18) * 0.5 + 0.5;
                cloud = smoothstep(0.18, 0.92, cloud);
                float detail = lerp(1.0, lerp(0.72, 1.22, cloud), saturate(_MilkyWayDetail));
                return _MilkyWayColor.rgb * band * detail * _MilkyWayStrength;
            }

            float cityLightMask(float3 rayDir, float3 sunDir)
            {
                if (_UseCityLight < 0.5 || _CityLightStrength <= 0.001)
                    return 0.0;

                float3 localDir = worldToLocalHorizon(normalize(rayDir));
                float verticalFade = 1.0 - smoothstep(0.0, max(_CityLightSpread, 0.001), localDir.y);
                verticalFade *= smoothstep(-0.08, 0.02, localDir.y);

                float cityAzimuth = _CityLightDirection * UNITY_TWO_PI;
                float2 cityDir = normalize(float2(-sin(cityAzimuth), cos(cityAzimuth)));
                float2 viewH = localDir.xz;
                float viewLen = max(length(viewH), 0.0001);
                float side = dot(viewH / viewLen, cityDir);
                float directionalBias = _CityLightUseDirection > 0.5
                    ? lerp(0.26, 1.0, pow(saturate(side * 0.5 + 0.5), 2.2))
                    : 1.0;

                float nightVisibility = 1.0 - smoothstep(-0.08, 0.16, sunDir.y);
                return saturate(verticalFade * directionalBias * nightVisibility);
            }

            float3 computeCityLight(float3 rayDir, float3 sunDir, float mask)
            {
                float horizonCore = 1.0 - smoothstep(-0.02, max(_CityLightSpread * 0.42, 0.001), worldToLocalHorizon(normalize(rayDir)).y);
                float glow = mask * lerp(0.55, 1.25, saturate(horizonCore));
                return _CityLightColor.rgb * glow * _CityLightStrength;
            }

            float meteorHash(float n)
            {
                return frac(sin(n) * 43758.5453123);
            }

            float3 meteorSkyDir(float seed)
            {
                float az = meteorHash(seed + 1.0) * UNITY_TWO_PI;
                float y = lerp(0.04, 0.88, pow(meteorHash(seed + 2.0), 0.55));
                float r = sqrt(max(0.0, 1.0 - y * y));
                return normalize(float3(sin(az) * r, y, cos(az) * r));
            }

            float3 meteorTangent(float3 center, float seed)
            {
                float3 refDir = abs(center.y) < 0.92 ? float3(0, 1, 0) : float3(1, 0, 0);
                float3 t1 = normalize(cross(refDir, center));
                float3 t2 = normalize(cross(center, t1));
                float a = meteorHash(seed + 3.0) * UNITY_TWO_PI;
                return normalize(t1 * cos(a) + t2 * sin(a));
            }

            float3 computeMeteors(float3 rayDir, float3 sunDir)
            {
                if (_UseMeteors < 0.5 || _MeteorFrequency <= 0.001 || _MeteorIntensity <= 0.001)
                    return float3(0, 0, 0);

                float period = max(60.0 / max(_MeteorFrequency, 0.001), 0.001);
                float activeDuration = min(max(_MeteorDuration, 0.04), max(period * 0.92, 0.04));
                float meteorTime = max(_Time.y + _MeteorSeed * 97.0, 0.0);
                float eventCycle = meteorTime / period;
                float eventNumber = floor(eventCycle);
                float eventIndex = eventNumber - floor(eventNumber / 8192.0) * 8192.0;
                float periodTime = frac(eventCycle) * period;
                float startOffset = meteorHash(eventIndex * 19.17 + _MeteorSeed * 113.0) * max(period - activeDuration, 0.0);
                float localTime = periodTime - startOffset;

                if (!(localTime >= 0.0 && localTime <= activeDuration))
                    return float3(0, 0, 0);

                float progress = saturate(localTime / activeDuration);
                float fadeIn = smoothstep(0.0, activeDuration * 0.12, localTime);
                float fadeOut = 1.0 - smoothstep(activeDuration * 0.70, activeDuration, localTime);
                float temporal = fadeIn * fadeOut;
                if (temporal <= 0.0001)
                    return float3(0, 0, 0);

                float seed = eventIndex * 13.37 + _MeteorSeed * 1000.0 + 17.0;
                float3 center = meteorSkyDir(seed);
                float3 tangent = meteorTangent(center, seed);
                float travel = (_MeteorLength * 2.8 + 0.08) * max(_MeteorSpeed, 0.01);
                float3 head = normalize(center + tangent * ((progress * 2.0 - 1.0) * travel * 0.5));

                float3 p = worldToLocalHorizon(normalize(rayDir));
                float3 axisSource = tangent - head * dot(tangent, head);
                float axisLength = length(axisSource);
                if (axisLength <= 0.0001)
                    return float3(0, 0, 0);

                float3 axis = axisSource / axisLength;
                float3 tailDir = -axis;
                float3 q = p - head * dot(p, head);
                float along = dot(q, tailDir);
                float sideDist = length(q - tailDir * along);

                float trailLength = max(_MeteorLength, 0.001);
                float width = max(_MeteorWidth, 0.0001);
                float trailT = saturate(along / trailLength);
                float behindMask = smoothstep(0.0, width * 2.0, along);
                float tailMask = 1.0 - smoothstep(trailLength * 0.18, trailLength, along);
                float widthScale = lerp(1.0, 0.25, trailT);
                float crossMask = exp2(-(sideDist * sideDist) / max(width * width * widthScale * widthScale, 0.00000001));
                float trailMask = crossMask * behindMask * tailMask;

                float headDist = length(q);
                float headMask = exp2(-(headDist * headDist) / max(width * width * 3.0, 0.00000001));

                float nightVisibility = 1.0 - smoothstep(-0.05, 0.12, sunDir.y);
                float horizonVisibility = smoothstep(-0.02, 0.08, p.y);
                float meteorMask = (trailMask + headMask * 1.4) * temporal * nightVisibility * horizonVisibility;

                return _MeteorColor.rgb * meteorMask * _MeteorIntensity;
            }

            float cloudHash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float cloudNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = cloudHash21(i);
                float b = cloudHash21(i + float2(1, 0));
                float c = cloudHash21(i + float2(0, 1));
                float d = cloudHash21(i + float2(1, 1));
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float cloudFbm(float2 p)
            {
                float v = 0.0;
                float a = 0.55;
                v += cloudNoise(p) * a; p = p * 2.03 + 17.7; a *= 0.50;
                v += cloudNoise(p) * a; p = p * 2.01 + 31.1; a *= 0.50;
                v += cloudNoise(p) * a; p = p * 2.05 + 11.4; a *= 0.50;
                v += cloudNoise(p) * a;
                return saturate(v);
            }

            float3 computeProceduralClouds(float3 rayDir, float3 sunDir, float3 moonDir, out float cloudAlpha, out float cloudOcclusion)
            {
                cloudAlpha = 0.0;
                cloudOcclusion = 0.0;
                if (_UseProceduralClouds < 0.5 || _CloudAmount <= 0.001 || _CloudOpacity <= 0.001)
                    return float3(0, 0, 0);

                float farT = saturate((0.34 - rayDir.y) / 0.42);
                float curvatureDrop = saturate(_CloudCurvatureDrop) * farT * farT;
                float3 localDir = worldToLocalHorizon(normalize(rayDir));
                float3 cloudLocalDir = normalize(float3(localDir.x, localDir.y + curvatureDrop, localDir.z));
                float heightMask = smoothstep(_CloudHeight - 0.16, _CloudHeight + 0.20, cloudLocalDir.y);
                float horizonMask = smoothstep(-0.05, 0.18, rayDir.y);
                float zenithSoft = 1.0 - smoothstep(0.96, 1.0, rayDir.y) * 0.35;
                float projection = max(abs(cloudLocalDir.y) + 0.18, 0.18);
                float2 uv = cloudLocalDir.xz / projection;
                uv *= max(_CloudScale, 0.001);
                uv += float2(_Time.y * _CloudSpeed, _Time.y * _CloudSpeed * 0.37);

                float flatten = saturate(_CloudHorizonFlatten) * farT;
                float2 farUv = lerp(uv, uv * 0.62 + float2(4.7, -2.1), flatten);
                float detailWeight = lerp(0.22, 0.04, flatten);

                float baseNoise = cloudFbm(farUv);
                float broadNoise = cloudFbm(uv * 0.34 + float2(8.2, 3.6));
                baseNoise = lerp(baseNoise, broadNoise, flatten * 0.55);
                float detail = cloudFbm(uv * 2.7 + 9.3);
                float density = saturate(baseNoise * (1.0 - detailWeight) + detail * detailWeight);
                float layerDepth = saturate(_CloudLayerDepth);
                float threshold = lerp(0.78, 0.28, saturate(_CloudAmount));
                float edge = max(_CloudSoftness, 0.01) * 0.35;
                float mask = smoothstep(threshold, threshold + edge, density);
                float frontAlpha = mask;
                float frontShade = 1.0;
                float backAlpha = 0.0;
                float backShade = 0.82;
                float highAlpha = 0.0;
                float highShade = 1.08;
                if (layerDepth > 0.001)
                {
                    float2 parallax = localDir.xz * lerp(0.44, 0.16, flatten) * layerDepth;
                    float2 uvBack = farUv + parallax + float2(7.1, -3.4) + _Time.y * _CloudSpeed * float2(0.42, 0.16);
                    float2 uvHigh = lerp(uv * 1.55, uv * 0.92, flatten) - parallax * 0.55 + float2(-11.6, 5.2) + _Time.y * _CloudSpeed * float2(-0.20, 0.48);
                    float backDensity = cloudFbm(uvBack);
                    float highDensity = cloudFbm(uvHigh);
                    float backMask = smoothstep(threshold + 0.04, threshold + edge + 0.04, backDensity);
                    float highMask = smoothstep(threshold + 0.10, threshold + edge + 0.10, highDensity);
                    backAlpha = backMask * 0.62 * layerDepth;
                    highAlpha = highMask * 0.42 * layerDepth;
                    density = lerp(density, max(density, max(backDensity * 0.88, highDensity * 0.72)), layerDepth * 0.45);
                }
                float combinedAlpha = 1.0 - (1.0 - frontAlpha) * (1.0 - backAlpha) * (1.0 - highAlpha);
                float premulShade = frontAlpha * frontShade
                                  + (1.0 - frontAlpha) * backAlpha * backShade
                                  + (1.0 - frontAlpha) * (1.0 - backAlpha) * highAlpha * highShade;
                float layerShade = premulShade / max(combinedAlpha, 0.0001);
                cloudAlpha = combinedAlpha * heightMask * horizonMask * zenithSoft * saturate(_CloudOpacity);

                float sunFacing = saturate(dot(rayDir, sunDir) * 0.5 + 0.5);
                float daylight = smoothstep(-0.08, 0.35, sunDir.y);
                float nightLight = 1.0 - daylight;
                float thickness = saturate(_CloudThickness);
                float sunsetHeight = 1.0 - smoothstep(0.02, 0.32, abs(sunDir.y));
                float sunsetAlive = sunsetHeight * (1.0 - smoothstep(0.36, 0.62, sunDir.y));
                float sunsetFacing = pow(sunFacing, 2.4);

                float eps = lerp(0.18, 0.05, saturate(_CloudScale / 10.0));
                float densityX = cloudFbm(uv + float2(eps, 0.0));
                float densityZ = cloudFbm(uv + float2(0.0, eps));
                float2 gradient = float2(density - densityX, density - densityZ);
                float3 cloudNormal = normalize(float3(gradient.x * 2.2 * thickness, 0.55, gradient.y * 2.2 * thickness));
                float3 localSun = worldToLocalHorizon(normalize(sunDir));
                float3 cloudLight = normalize(float3(localSun.x, abs(localSun.y) + 0.25, localSun.z));
                float diffuse = saturate(dot(cloudNormal, cloudLight));

                float underside = 1.0 - smoothstep(_CloudHeight + 0.02, _CloudHeight + 0.55, rayDir.y);
                float selfShadow = saturate((density - threshold) / max(edge + 0.001, 0.001));
                float shadow = lerp(1.0, 1.0 - selfShadow * 0.48 - underside * 0.32, thickness);
                float rim = pow(saturate(1.0 - abs(density - threshold) / max(edge * 2.0, 0.001)), 2.0)
                          * lerp(0.0, 0.28, thickness) * sunFacing;
                float sunsetRim = pow(saturate(1.0 - abs(density - threshold) / max(edge * 2.8, 0.001)), 2.0)
                                * sunsetFacing * sunsetAlive * saturate(_CloudSunsetStrength);
                float sunsetUnderside = underside * selfShadow * sunsetAlive * saturate(_CloudSunsetStrength * 0.55);
                float thickCore = smoothstep(0.12, 1.0, selfShadow);
                float thicknessOcclusion = lerp(mask, thickCore, thickness);
                cloudOcclusion = saturate(thicknessOcclusion * heightMask * horizonMask * zenithSoft * saturate(_CloudOpacity));

                float3 dayColor = _CloudColor.rgb
                                * lerp(0.55, 1.28, daylight)
                                * lerp(0.82, 1.24, lerp(sunFacing, diffuse, thickness))
                                * shadow
                                * layerShade;
                float3 nightColor = _CloudColor.rgb
                                  * float3(0.12, 0.16, 0.24)
                                  * nightLight
                                  * lerp(1.0, 0.60 + diffuse * 0.55, thickness)
                                  * shadow
                                  * layerShade;
                float3 sunsetColor = _CloudSunsetTint.rgb * (sunsetRim * 1.45 + sunsetUnderside * 0.55);
                float3 cloudColor = lerp(nightColor, dayColor, daylight)
                                  + _CloudColor.rgb * rim
                                  + sunsetColor;

                if (_UseCloudMoonHighlight > 0.5 && _CloudMoonHighlightStrength > 0.001)
                {
                    float moonFacing = dot(normalize(rayDir), normalize(moonDir));
                    float moonProximity = smoothstep(1.0 - max(_CloudMoonHighlightRange, 0.02), 1.0, moonFacing);
                    float moonAbove = smoothstep(-0.04, 0.18, moonDir.y);
                    moonAbove = lerp(1.0, moonAbove, saturate(_CloudMoonHighlightAltitude));
                    float moonPhaseLight = smoothstep(0.02, 1.0, saturate(_MoonPhase));
                    float3 localMoon = worldToLocalHorizon(normalize(moonDir));
                    float3 moonLight = normalize(float3(localMoon.x, abs(localMoon.y) + 0.25, localMoon.z));
                    float moonDiffuse = saturate(dot(cloudNormal, moonLight));
                    float moonEdge = pow(saturate(1.0 - abs(density - threshold) / max(edge * 3.0, 0.001)), 1.4);
                    float moonShaping = lerp(0.65 + moonDiffuse * 0.45, 0.50 + moonDiffuse * 0.70 + moonEdge * 0.45, thickness);
                    float moonHighlight = moonProximity
                                        * moonAbove
                                        * moonPhaseLight
                                        * nightLight
                                        * moonShaping
                                        * max(_CloudMoonHighlightStrength, 0.0);
                    cloudColor += _CloudMoonHighlightColor.rgb * moonHighlight;
                }

                return cloudColor * cloudAlpha;
            }

            float3 computeCatalogStars(float3 rayDir, float3 sunDir, float3 moonDir)
            {
                float3 dir = normalize(rayDir);
                float3 localDir = worldToLocalHorizon(dir);
                dir = _CatalogUseLocation > 0.5
                    ? localHorizonToEquatorial(dir)
                    : rotateAroundY(float3(-localDir.x, localDir.y, localDir.z), _CatalogStarRotation);

                float u = atan2(dir.z, dir.x) / UNITY_TWO_PI;
                u = frac(u + 1.0);
                float v = asin(clamp(dir.y, -1.0, 1.0)) / UNITY_PI + 0.5;
                float cellX = floor(u * _CatalogCellLonCount);
                float cellY = floor(saturate(v) * _CatalogCellLatCount);

                float3 stars = float3(0, 0, 0);

                if (_CatalogCellExpanded > 0.5)
                {
                    float4 cell = tex2Dlod(_CatalogStarCellMap, float4(catalogCellUV(cellX, cellY), 0.0, 0.0));
                    float start = floor(cell.r + 0.5);
                    float count = min(floor(cell.g + 0.5), 256.0);

                    [loop]
                    for (int n = 0; n < 256; n++)
                    {
                        if (n >= count)
                            break;
                        stars += catalogStarSample(dir, start + n);
                    }
                }
                else
                {
                    [unroll]
                    for (int oy = -1; oy <= 1; oy++)
                    {
                        [unroll]
                        for (int ox = -1; ox <= 1; ox++)
                        {
                            float4 cell = tex2Dlod(_CatalogStarCellMap, float4(catalogCellUV(cellX + ox, cellY + oy), 0.0, 0.0));
                            float start = floor(cell.r + 0.5);
                            float count = min(floor(cell.g + 0.5), 64.0);

                            [loop]
                            for (int n = 0; n < 64; n++)
                            {
                                if (n >= count)
                                    break;
                                stars += catalogStarSample(dir, start + n);
                            }
                        }
                    }
                }

                float horizFade = smoothstep(0.0, _StarFadeStrength * 2.0,
                                             -rayDir.y + (_StarFadeHeight * 3.0 - 1.0));
                stars = lerp(stars, float3(0,0,0), horizFade);

                float hd = horizonDot(rayDir, sunDir);
                float dawnFade = pow(saturate(hd * 0.5 + 0.5), 2.0)
                               * saturate(1.0 - abs(sunDir.y) * 5.0);
                stars = lerp(stars, float3(0,0,0), dawnFade);

                return stars * _CatalogStarBrightness;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 rayDir  = normalize(i.rayDir);
                float startupFade = saturate(_AetherStartupFade);
                float t = getEffectiveTimeOfDay();
                float3 sunDir  = _UseSeasonalSun > 0.5
                    ? getSeasonalSunDir(t, _SkyRotation)
                    : getSunDir(t, _SkyRotation);
                float moonTime = frac(t + _MoonPhase * 0.5);
                float3 moonDir = _UseSeasonalSun > 0.5
                    ? getSeasonalSunDir(moonTime, _SkyRotation)
                    : getSunDir(moonTime, _SkyRotation);
                if (_UseRealMoonPosition > 0.5)
                    moonDir = getRealMoonDir();

                float solarEclipse = solarEclipseCoverage(sunDir, moonDir);
                float3 sky = computeSky(rayDir, sunDir, t, solarEclipse);
                sky += float3(0.001, 0.002, 0.008);
                float urbanMask = cityLightMask(rayDir, sunDir);
                float3 urbanGlow = computeCityLight(rayDir, sunDir, urbanMask);
                sky += urbanGlow;

                float starVis = 1.0 - smoothstep(-0.12, 0.06, sunDir.y);
                float3 stars = float3(0, 0, 0);
                if (starVis > 0.0)
                {
                    if (_UseCatalogStars > 0.5)
                        stars = computeCatalogStars(rayDir, sunDir, moonDir);
                    else
                        stars = computeStars(rayDir, sunDir, moonDir);
                    float3 overlays = computeMilkyWay(rayDir) + computeOrionLines(rayDir);
                    float overlayHorizFade = smoothstep(0.0, _StarFadeStrength * 2.0,
                                                        -rayDir.y + (_StarFadeHeight * 3.0 - 1.0));
                    float overlayHd = horizonDot(rayDir, sunDir);
                    float overlayDawnFade = pow(saturate(overlayHd * 0.5 + 0.5), 2.0)
                                          * saturate(1.0 - abs(sunDir.y) * 5.0);
                    overlays = lerp(overlays, float3(0,0,0), max(overlayHorizFade, overlayDawnFade));
                    stars += overlays;
                }
                float urbanStarBlock = saturate(urbanMask * _CityLightStarOcclusion * saturate(_CityLightStrength));
                stars *= 1.0 - urbanStarBlock;
                float3 meteors = computeMeteors(rayDir, sunDir);
                float cloudAlpha = 0.0;
                float cloudOcclusion = 0.0;
                float3 clouds = computeProceduralClouds(rayDir, sunDir, moonDir, cloudAlpha, cloudOcclusion);
                clouds += _CityLightColor.rgb * urbanMask * cloudAlpha * _CityLightStrength * _CityLightCloudReflection;
                float cloudBlock = saturate(cloudOcclusion * max(_CloudStarOcclusion, 0.0));
                stars *= 1.0 - cloudBlock;
                meteors *= 1.0 - cloudBlock;
                meteors *= 1.0 - urbanStarBlock * 0.65;
                float moonBlock = max(moonOcclusion(rayDir, moonDir), moonStarSuppression(rayDir, moonDir)) * startupFade;
                stars *= 1.0 - moonBlock;
                meteors *= 1.0 - saturate(moonBlock * 0.65);
                float3 moon = computeMoon(rayDir, sunDir, moonDir);
                float moonCloudBlock = saturate(cloudOcclusion * 0.8);
                moon *= 1.0 - moonCloudBlock;
                moon *= startupFade;
                float3 sunDisk = computeSunDisk(rayDir, sunDir, moonDir, t, solarEclipse) * startupFade;
                float catalogEmission = _UseCatalogStars > 0.5 ? _CatalogStarEmission : 1.0;
                float3 color = saturate((sky + moon + clouds) * _Exposure)
                             + sunDisk * _Exposure
                             + stars * startupFade * starVis * _Exposure * catalogEmission
                             + meteors * startupFade * _Exposure;

                return half4(color, 1.0);
            }
            ENDCG
        }
    }

    Fallback Off
    CustomEditor "AetherGUI"
}
