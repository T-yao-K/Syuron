using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum AetherTimeZonePreset
{
    UTC,
    PST_Los_Angeles,
    MST_Denver,
    CST_Chicago,
    EST_New_York,
    BRT_Sao_Paulo,
    GMT_London,
    CET_Paris_Berlin,
    EET_Helsinki,
    MSK_Moscow,
    IST_India,
    CST_Shanghai,
    JST_Tokyo,
    KST_Seoul,
    AEST_Sydney,
    NZST_Auckland,
}

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class AetherTime : UdonSharpBehaviour
{
    private bool _smoothClockInitialized = false;
    private int _smoothClockSecondKey = -1;
    private float _smoothClockBaseSeconds = 0f;
    private float _smoothClockBaseRealtime = 0f;
    private const float SmoothClockCorrectionRate = 0.35f;
    private bool _timeOfDaySmoothInitialized = false;
    private float _smoothTimeOfDay = 0f;
    private float _timeOfDayBaseRealtime = 0f;
    private const float TimeOfDayCorrectionRate = 0.1f;
    private bool _siderealClockInitialized = false;
    private float _smoothSiderealRotation = 0f;
    private float _siderealClockBaseRealtime = 0f;
    private const float SiderealSecondsPerDay = 86164.0905f;
    private const float SiderealCorrectionRate = 0.0002f;
    private const float SiderealLargeCorrectionRate = 0.06f;
    private bool _moonPhaseSmoothInitialized = false;
    private float _smoothMoonPhase = 0f;
    private float _moonPhaseBaseRealtime = 0f;
    private const float MoonPhaseCorrectionRate = 0.18f;
    private bool _moonPositionSmoothInitialized = false;
    private float _smoothMoonRightAscension = 0f;
    private float _smoothMoonDeclination = 0f;
    private float _moonPositionBaseRealtime = 0f;
    private const float MoonRightAscensionCorrectionRate = 0.04f;
    private const float MoonDeclinationCorrectionRate = 12f;
    private bool _solarDeclinationSmoothInitialized = false;
    private float _smoothSolarDeclination = 0f;
    private float _solarDeclinationBaseRealtime = 0f;
    private const float SolarDeclinationCorrectionRate = 4f;
    private bool _sunLightSmoothInitialized = false;
    private bool _moonLightSmoothInitialized = false;
    private Vector3 _smoothSunLightDirection = Vector3.up;
    private Vector3 _smoothMoonLightDirection = Vector3.up;
    private float _smoothSunLightIntensity = 0f;
    private float _smoothMoonLightIntensity = 0f;
    private float _smoothSunShadowStrength = 0f;
    private float _smoothMoonShadowStrength = 0f;
    private Color _smoothSunLightColor = Color.white;
    private Color _smoothMoonLightColor = Color.white;
    private float _sunLightBaseRealtime = 0f;
    private float _moonLightBaseRealtime = 0f;
    private const float LightDirectionBlendRate = 8f;
    private const float LightIntensityCorrectionRate = 8f;
    private const float LightShadowCorrectionRate = 4f;
    private const float LightColorBlendRate = 6f;
    private bool _startupFadeActive = false;
    private float _startupFadeStartRealtime = 0f;
    private const float StartupFadeDuration = 0.15f;
    private bool _autoClockInitialized = false;
    private float _autoClockStartRealtime = 0f;
    private float _autoClockBaseRealtime = 0f;
    private float _autoClockBaseLocalDaySeconds = 0f;
    private float _autoClockBaseTotalSeconds = 0f;
    private float _autoClockBaseSiderealRotation = 0f;
    private float _autoClockSpeed = 1f;
    private bool _runtimeBlocked = false;
    private bool _configurationErrorLogged = false;

    [Tooltip("Aetherの空マテリアルを1つだけ登録します。空以外は制御しません。")]
    [SerializeField] Material[] _materials;
    [SerializeField, HideInInspector] int _controllerId = 0;
    [SerializeField, HideInInspector] bool _lightDefaultsInitialized = false;
    [Tooltip("実時刻同期に使うタイムゾーンです。")]
    [SerializeField] AetherTimeZonePreset _timeZone = AetherTimeZonePreset.JST_Tokyo;
    [Tooltip("同期時刻を秒単位で前後にずらします。")]
    [SerializeField] float _timeOffsetSeconds = 0f;
    [Tooltip("実時刻ではなく、指定速度で空の時刻を自動進行させます。")]
    [SerializeField] bool  _autoRotate        = false;
    [Tooltip("Auto Rotate有効時の進行速度です。1440で現実の1分が空の1日になります。")]
    [SerializeField] float _autoSpeed = 1f;
    [Tooltip("Aetherの太陽・月にDirectional Lightを連動します。")]
    [SerializeField] bool _syncDirectionalLights = false;
    [Tooltip("太陽に連動させるDirectional Lightです。")]
    [SerializeField] Light _sunDirectionalLight;
    [Tooltip("月に連動させるDirectional Lightです。")]
    [SerializeField] Light _moonDirectionalLight;
    [Tooltip("太陽ライトを連動します。")]
    [SerializeField] bool _syncSunLight = true;
    [Tooltip("月ライトを連動します。")]
    [SerializeField] bool _syncMoonLight = true;
    [Tooltip("太陽ライトの最大強度です。")]
    [Range(0f, 8f)]
    [SerializeField] float _sunLightIntensity = 1.0f;
    [Tooltip("月ライトの最大強度です。")]
    [Range(0f, 2f)]
    [SerializeField] float _moonLightIntensity = 0.05f;
    [Tooltip("Aetherの太陽色・月色をDirectional Lightへ反映します。")]
    [SerializeField] bool _syncLightColor = true;
    [Tooltip("太陽ライト色に掛ける補正色です。白でAetherの太陽色そのままです。")]
    [SerializeField] Color _sunLightTint = Color.white;
    [Tooltip("月ライト色に掛ける補正色です。白でAetherの月色そのままです。")]
    [SerializeField] Color _moonLightTint = Color.white;
    [Tooltip("Realtime Shadowsの強度制御を有効にします。")]
    [SerializeField] bool _enableRealtimeShadows = false;
    [Tooltip("太陽ライトの影の強さです。")]
    [Range(0f, 1f)]
    [SerializeField] float _sunShadowStrength = 0.75f;
    [Tooltip("月ライトの影の強さです。")]
    [Range(0f, 1f)]
    [SerializeField] float _moonShadowStrength = 0.12f;
    [Tooltip("ライトがフェードを始める高度です。0が水平線です。")]
    [Range(-0.1f, 0.2f)]
    [SerializeField] float _lightFadeStartAltitude = 0.02f;
    [Tooltip("ライトを完全に無効化する高度です。水平線より少し下にします。")]
    [Range(-0.3f, 0.05f)]
    [SerializeField] float _lightDisableAltitude = -0.06f;
    [Tooltip("ONにすると同期時刻を手動スライダーで固定します。空の時刻・月位置・恒星時のテストに使います。")]
    [SerializeField] bool  _overrideTime  = false;
    [Tooltip("手動同期時刻です。0.5が太陽の南中になる太陽時として扱います。0=真夜中、0.25=日の出側、0.75=日没側です。")]
    [Range(0f, 1f)]
    [SerializeField] float _overrideValue = 0.5f;
    [Tooltip("ONにすると天文同期用の日付時刻を手動指定します。月齢・月位置・恒星時・季節同期の検証に使います。")]
    [SerializeField] bool  _overrideDateTime = false;
    [Tooltip("天文同期テスト用の年です。")]
    [SerializeField] int   _overrideYear = 2026;
    [Tooltip("天文同期テスト用の月です。")]
    [SerializeField] int   _overrideMonth = 5;
    [Tooltip("天文同期テスト用の日です。")]
    [SerializeField] int   _overrideDay = 5;
    [Tooltip("天文同期テスト用の時です。")]
    [SerializeField] int   _overrideHour = 0;
    [Tooltip("天文同期テスト用の分です。")]
    [SerializeField] int   _overrideMinute = 0;

    void Start()
    {
        ApplySkyTime();
    }

    void OnEnable()
    {
        ResetRuntimeSession();
        BeginStartupFade();
        ApplySkyTime();
    }

    void Update()
    {
        ApplySkyTime();
    }

    void ApplySkyTime()
    {
        EnsureLightDefaults();

        if (_runtimeBlocked)
            return;

        Material skyMaterial = GetSingleAetherMaterial();
        if (skyMaterial == null)
        {
            DisableManagedLights();
            return;
        }

        if (!AcquireControllerOwnership(skyMaterial))
        {
            _runtimeBlocked = true;
            DisableManagedLights();
            return;
        }

        float timeOfDay;
        System.DateTime dt = Networking.GetNetworkDateTime();
        float zoneOffset = GetZoneOffset(_timeZone);
        bool dtUsesLocalZone = _overrideDateTime;
        if (_overrideDateTime)
        {
            dt = GetOverrideDateTime();
        }
        if (_overrideTime)
        {
            if (!dtUsesLocalZone)
                dt = dt.AddSeconds(zoneOffset);
            dt = ApplyOverrideSolarClock(dt, GetSolarTimeOffsetSeconds(skyMaterial, dt));
            dtUsesLocalZone = true;
        }
        float astronomyUtcOffset = dtUsesLocalZone
            ? zoneOffset - _timeOffsetSeconds
            : -_timeOffsetSeconds;
        bool smoothClock = !_overrideDateTime && !_overrideTime;
        bool smoothSyncedSky = smoothClock && !_autoRotate;
        float rawDaySeconds = GetRawDaySeconds(dt);
        float smoothDaySeconds = GetSmoothDaySeconds(dt, smoothClock);
        float smoothSecondOffset = GetWrappedSecondDelta(smoothDaySeconds, rawDaySeconds);
        float astronomyExtraSeconds = smoothSecondOffset;
        float rawSiderealRotation = GetGreenwichSiderealRotation(dt, astronomyUtcOffset, astronomyExtraSeconds);
        float smoothSiderealRotation = rawSiderealRotation;
        float autoSiderealRotation = 0f;
        System.DateTime localSolarDateTime = dtUsesLocalZone
            ? dt.AddSeconds(_timeOffsetSeconds)
            : dt.AddSeconds(zoneOffset + _timeOffsetSeconds);

        if (_overrideTime)
        {
            _autoClockInitialized = false;
            timeOfDay = _overrideValue;
        }
        else
        {
            if (_autoRotate)
            {
                float localOffset = dtUsesLocalZone ? 0f : zoneOffset;
                float currentLocalDaySeconds = Mathf.Repeat(smoothDaySeconds + localOffset + _timeOffsetSeconds, 86400f);
                UpdateAutoClock(currentLocalDaySeconds, rawSiderealRotation, _autoSpeed);
                float autoTotalSeconds = GetAutoTotalSeconds();
                float autoAstronomyDeltaSeconds = autoTotalSeconds - GetAutoRealElapsedSeconds();
                float autoDeltaSeconds = autoTotalSeconds - _autoClockBaseTotalSeconds;
                float daySeconds = Mathf.Repeat(_autoClockBaseLocalDaySeconds + autoDeltaSeconds, 86400f);
                autoSiderealRotation = Mathf.Repeat(_autoClockBaseSiderealRotation + autoDeltaSeconds * 1.0027379f / 86400f, 1f);
                localSolarDateTime = localSolarDateTime.AddSeconds(autoAstronomyDeltaSeconds);
                timeOfDay = Mathf.Repeat(daySeconds + GetSolarTimeOffsetSeconds(skyMaterial, localSolarDateTime), 86400f) / 86400f;
                astronomyExtraSeconds = smoothSecondOffset + autoAstronomyDeltaSeconds;
            }
            else
            {
                _autoClockInitialized = false;
                float localOffset = dtUsesLocalZone ? 0f : zoneOffset;
                float localDaySeconds = Mathf.Repeat(smoothDaySeconds + localOffset + _timeOffsetSeconds, 86400f);
                timeOfDay = Mathf.Repeat(localDaySeconds + GetSolarTimeOffsetSeconds(skyMaterial, localSolarDateTime), 86400f) / 86400f;
            }
        }

        ApplyStartupFade(skyMaterial, smoothSyncedSky);
        if (smoothSyncedSky)
        {
            timeOfDay = GetSmoothMaterialTimeOfDay(timeOfDay);
            smoothSiderealRotation = GetSmoothSiderealRotation(rawSiderealRotation);
        }
        else
        {
            _timeOfDaySmoothInitialized = false;
            _siderealClockInitialized = false;
            smoothSiderealRotation = rawSiderealRotation;
        }
        SetFloatIfChanged(skyMaterial, "_AetherTimeControllerActive", 1f, 0.0001f);
        SetFloatIfChanged(skyMaterial, "_AutoRotate", 0f, 0.0001f);
        SetFloatIfChanged(skyMaterial, "_AetherAutoTimeActive", _autoRotate ? 1f : 0f, 0.0001f);
        if (_autoRotate)
        {
            SetFloatIfChanged(skyMaterial, "_AetherAutoBaseTimeOfDay", timeOfDay, 0.0000001f);
            SetFloatIfChanged(skyMaterial, "_AetherAutoBaseSiderealRotation", autoSiderealRotation, 0.0000001f);
            SetFloatIfChanged(skyMaterial, "_AetherAutoStartTime", 0f, 0.0001f);
            SetFloatIfChanged(skyMaterial, "_AetherAutoTimeSpeed", 0f, 0.0001f);
        }
        if (_autoRotate)
        {
            SetFloatIfChanged(skyMaterial, "_TimeOfDay", timeOfDay, 0.0000001f);
            SyncCatalogSiderealTime(skyMaterial, autoSiderealRotation, timeOfDay);
        }
        else
        {
            SetFloatIfChanged(skyMaterial, "_TimeOfDay", timeOfDay, 0.0000001f);
            SyncCatalogSiderealTime(skyMaterial, smoothSiderealRotation, timeOfDay);
        }
        SyncSeasonalSun(skyMaterial, localSolarDateTime, smoothSyncedSky);
        SyncMoon(skyMaterial, dt, astronomyUtcOffset, astronomyExtraSeconds, true, smoothSyncedSky);

        float siderealRotation = _autoRotate
            ? autoSiderealRotation
            : smoothSiderealRotation;
        SyncDirectionalLights(skyMaterial, timeOfDay, siderealRotation, smoothSyncedSky);
    }

    void OnDisable()
    {
        ReleaseControllerOwnership();
        SetAutoTimeActive(false);
        SetTimeControllerActive(false);
        SetStartupFade(1f);
        DisableManagedLights();
    }

    void ResetRuntimeSession()
    {
        _runtimeBlocked = false;
        _configurationErrorLogged = false;
        _smoothClockInitialized = false;
        _timeOfDaySmoothInitialized = false;
        _siderealClockInitialized = false;
        _moonPhaseSmoothInitialized = false;
        _moonPositionSmoothInitialized = false;
        _solarDeclinationSmoothInitialized = false;
        _sunLightSmoothInitialized = false;
        _moonLightSmoothInitialized = false;
        _autoClockInitialized = false;
    }

    void BeginStartupFade()
    {
        _startupFadeActive = true;
        _startupFadeStartRealtime = Time.time;
    }

    void ApplyStartupFade(Material material, bool canFade)
    {
        if (!canFade)
        {
            SetFloatIfChanged(material, "_AetherStartupFade", 1f, 0.001f);
            _startupFadeActive = false;
            return;
        }

        if (!_startupFadeActive)
        {
            SetFloatIfChanged(material, "_AetherStartupFade", 1f, 0.001f);
            return;
        }

        float t = Mathf.Clamp01((Time.time - _startupFadeStartRealtime) / Mathf.Max(StartupFadeDuration, 0.001f));
        float fade = Mathf.SmoothStep(0f, 1f, t);
        SetFloatIfChanged(material, "_AetherStartupFade", fade, 0.001f);
        if (t >= 1f)
            _startupFadeActive = false;
    }

    void SetStartupFade(float value)
    {
        if (_materials == null)
            return;

        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i] != null)
                SetFloatIfChanged(_materials[i], "_AetherStartupFade", value, 0.001f);
        }
    }

    void SetAutoTimeActive(bool active)
    {
        if (_materials == null)
            return;

        float value = active ? 1f : 0f;
        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i] != null)
                SetFloatIfChanged(_materials[i], "_AetherAutoTimeActive", value, 0.0001f);
        }
    }

    void SetTimeControllerActive(bool active)
    {
        if (_materials == null)
            return;

        float value = active ? 1f : 0f;
        for (int i = 0; i < _materials.Length; i++)
        {
            if (_materials[i] != null)
                SetFloatIfChanged(_materials[i], "_AetherTimeControllerActive", value, 0.0001f);
        }
    }

    Material GetSingleAetherMaterial()
    {
        if (_materials == null)
        {
            LogConfigurationError("AetherTime requires exactly one Aether material.");
            return null;
        }

        Material found = null;
        int count = 0;
        for (int i = 0; i < _materials.Length; i++)
        {
            if (IsAetherMaterial(_materials[i]))
            {
                found = _materials[i];
                count++;
            }
        }

        if (count == 1)
            return found;

        if (count == 0)
            LogConfigurationError("AetherTime requires one Aether sky material in Materials.");
        else
            LogConfigurationError("AetherTime does not allow multiple Aether sky materials. Keep only one Aether material in Materials.");

        return null;
    }

    bool IsAetherMaterial(Material material)
    {
        return material != null
            && material.HasProperty("_AetherTimeControllerActive")
            && material.HasProperty("_CatalogSiderealRotation")
            && material.HasProperty("_MoonPhase");
    }

    bool AcquireControllerOwnership(Material material)
    {
        if (material == null || !material.HasProperty("_AetherTimeControllerOwner"))
            return true;

        float ownId = GetControllerId();
        float owner = material.GetFloat("_AetherTimeControllerOwner");
        if (owner > 0.5f && Mathf.Abs(owner - ownId) > 0.5f)
        {
            LogConfigurationError("Another AetherTime is already controlling this Aether material. Only one AetherTime is allowed.");
            return false;
        }

        SetFloatIfChanged(material, "_AetherTimeControllerOwner", ownId, 0.5f);
        return true;
    }

    void ReleaseControllerOwnership()
    {
        if (_materials == null)
            return;

        float ownId = GetControllerId();
        for (int i = 0; i < _materials.Length; i++)
        {
            Material material = _materials[i];
            if (material == null || !material.HasProperty("_AetherTimeControllerOwner"))
                continue;

            float owner = material.GetFloat("_AetherTimeControllerOwner");
            if (Mathf.Abs(owner - ownId) <= 0.5f)
                SetFloatIfChanged(material, "_AetherTimeControllerOwner", 0f, 0.5f);
        }
    }

    float GetControllerId()
    {
        if (_controllerId > 0)
            return _controllerId;

        int instanceId = GetInstanceID();
        if (instanceId < 0)
            instanceId = -instanceId;
        if (instanceId <= 0)
            instanceId = 1;
        return instanceId;
    }

    void LogConfigurationError(string message)
    {
        if (_configurationErrorLogged)
            return;

        _configurationErrorLogged = true;
        Debug.LogError("[AetherTime] " + message);
    }

    void SyncDirectionalLights(Material skyMaterial, float timeOfDay, float siderealRotation, bool canSmooth)
    {
        if (!_syncDirectionalLights)
        {
            DisableManagedLights();
            return;
        }

        Vector3 sunDir = GetSunDirection(skyMaterial, timeOfDay);
        Vector3 moonDir = GetMoonDirection(skyMaterial, timeOfDay, siderealRotation);
        if (_syncSunLight && _sunDirectionalLight != null)
        {
            float sunVisibility = GetAltitudeVisibility(sunDir.y) * GetSolarEclipseLightFactor(skyMaterial, sunDir, moonDir);
            Color sunColor = GetSunLightColor(skyMaterial, sunDir, timeOfDay);
            ApplyDirectionalLight(_sunDirectionalLight, sunDir, _sunLightIntensity, sunVisibility, _sunShadowStrength, sunColor, true, canSmooth);
        }
        else if (_sunDirectionalLight != null)
        {
            DisableLight(_sunDirectionalLight);
            _sunLightSmoothInitialized = false;
        }

        if (_syncMoonLight && _moonDirectionalLight != null)
        {
            float moonPhase = GetMaterialFloat(skyMaterial, "_MoonPhase", 0f);
            float moonPhaseLight = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(moonPhase));
            float nightLight = Mathf.Clamp01(-sunDir.y * 5f);
            float moonVisibility = GetAltitudeVisibility(moonDir.y) * moonPhaseLight * nightLight;
            Color moonColor = GetMoonLightColor(skyMaterial);
            ApplyDirectionalLight(_moonDirectionalLight, moonDir, _moonLightIntensity, moonVisibility, _moonShadowStrength, moonColor, false, canSmooth);
        }
        else if (_moonDirectionalLight != null)
        {
            DisableLight(_moonDirectionalLight);
            _moonLightSmoothInitialized = false;
        }
    }

    void ApplyDirectionalLight(Light targetLight, Vector3 sourceDirection, float maxIntensity, float visibility, float shadowStrength, Color lightColor, bool isSun, bool canSmooth)
    {
        Vector3 targetDirection = sourceDirection.normalized;
        float targetIntensity = Mathf.Max(0f, maxIntensity) * Mathf.Clamp01(visibility);
        float targetShadow = _enableRealtimeShadows ? Mathf.Clamp01(shadowStrength) * Mathf.Clamp01(visibility) : 0f;

        Vector3 smoothDirection;
        float smoothIntensity;
        float smoothShadow;
        Color smoothColor;

        if (isSun)
        {
            if (!canSmooth || !_sunLightSmoothInitialized)
            {
                _sunLightSmoothInitialized = canSmooth;
                if (canSmooth && targetLight.enabled)
                {
                    _smoothSunLightDirection = (-targetLight.transform.forward).normalized;
                    _smoothSunLightIntensity = targetLight.intensity;
                    _smoothSunShadowStrength = targetLight.shadowStrength;
                    _smoothSunLightColor = targetLight.color;
                }
                else
                {
                    _smoothSunLightDirection = targetDirection;
                    _smoothSunLightIntensity = canSmooth ? 0f : targetIntensity;
                    _smoothSunShadowStrength = canSmooth ? 0f : targetShadow;
                    _smoothSunLightColor = lightColor;
                }
                _sunLightBaseRealtime = Time.time;
            }
            else
            {
                float elapsed = Mathf.Max(0f, Time.time - _sunLightBaseRealtime);
                float dirT = Mathf.Clamp01(elapsed * LightDirectionBlendRate);
                float colorT = Mathf.Clamp01(elapsed * LightColorBlendRate);
                _smoothSunLightDirection = Vector3.Lerp(_smoothSunLightDirection, targetDirection, dirT).normalized;
                _smoothSunLightIntensity = MoveTowardsValue(_smoothSunLightIntensity, targetIntensity, elapsed * LightIntensityCorrectionRate);
                _smoothSunShadowStrength = MoveTowardsValue(_smoothSunShadowStrength, targetShadow, elapsed * LightShadowCorrectionRate);
                _smoothSunLightColor = Color.Lerp(_smoothSunLightColor, lightColor, colorT);
                _sunLightBaseRealtime = Time.time;
            }

            smoothDirection = _smoothSunLightDirection;
            smoothIntensity = _smoothSunLightIntensity;
            smoothShadow = _smoothSunShadowStrength;
            smoothColor = _smoothSunLightColor;
        }
        else
        {
            if (!canSmooth || !_moonLightSmoothInitialized)
            {
                _moonLightSmoothInitialized = canSmooth;
                if (canSmooth && targetLight.enabled)
                {
                    _smoothMoonLightDirection = (-targetLight.transform.forward).normalized;
                    _smoothMoonLightIntensity = targetLight.intensity;
                    _smoothMoonShadowStrength = targetLight.shadowStrength;
                    _smoothMoonLightColor = targetLight.color;
                }
                else
                {
                    _smoothMoonLightDirection = targetDirection;
                    _smoothMoonLightIntensity = canSmooth ? 0f : targetIntensity;
                    _smoothMoonShadowStrength = canSmooth ? 0f : targetShadow;
                    _smoothMoonLightColor = lightColor;
                }
                _moonLightBaseRealtime = Time.time;
            }
            else
            {
                float elapsed = Mathf.Max(0f, Time.time - _moonLightBaseRealtime);
                float dirT = Mathf.Clamp01(elapsed * LightDirectionBlendRate);
                float colorT = Mathf.Clamp01(elapsed * LightColorBlendRate);
                _smoothMoonLightDirection = Vector3.Lerp(_smoothMoonLightDirection, targetDirection, dirT).normalized;
                _smoothMoonLightIntensity = MoveTowardsValue(_smoothMoonLightIntensity, targetIntensity, elapsed * LightIntensityCorrectionRate);
                _smoothMoonShadowStrength = MoveTowardsValue(_smoothMoonShadowStrength, targetShadow, elapsed * LightShadowCorrectionRate);
                _smoothMoonLightColor = Color.Lerp(_smoothMoonLightColor, lightColor, colorT);
                _moonLightBaseRealtime = Time.time;
            }

            smoothDirection = _smoothMoonLightDirection;
            smoothIntensity = _smoothMoonLightIntensity;
            smoothShadow = _smoothMoonShadowStrength;
            smoothColor = _smoothMoonLightColor;
        }

        if (smoothIntensity <= 0.0001f)
        {
            DisableLight(targetLight);
            return;
        }

        targetLight.enabled = true;
        targetLight.transform.rotation = Quaternion.LookRotation(-smoothDirection.normalized, Vector3.up);
        if (_syncLightColor)
            targetLight.color = smoothColor;
        targetLight.intensity = smoothIntensity;
        targetLight.shadowStrength = smoothShadow;
    }

    void DisableManagedLights()
    {
        if (_sunDirectionalLight != null)
            DisableLight(_sunDirectionalLight);
        if (_moonDirectionalLight != null)
            DisableLight(_moonDirectionalLight);
    }

    void DisableLight(Light targetLight)
    {
        targetLight.intensity = 0f;
        targetLight.shadowStrength = 0f;
        targetLight.enabled = false;
    }

    float GetAltitudeVisibility(float altitude)
    {
        float disableAltitude = Mathf.Min(_lightDisableAltitude, _lightFadeStartAltitude - 0.001f);
        float fadeStart = Mathf.Max(_lightFadeStartAltitude, disableAltitude + 0.001f);
        float t = Mathf.InverseLerp(disableAltitude, fadeStart, altitude);
        return Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
    }

    float GetSolarEclipseLightFactor(Material skyMaterial, Vector3 sunDir, Vector3 moonDir)
    {
        float sunRadius = Mathf.Max(GetMaterialFloat(skyMaterial, "_SunSize", 0.008f), 0.0001f);
        float moonRadius = Mathf.Max(GetMaterialFloat(skyMaterial, "_MoonSize", 0.02f), 0.0001f);
        float separation = Mathf.Acos(Mathf.Clamp(Vector3.Dot(sunDir.normalized, moonDir.normalized), -1f, 1f));
        float overlap = GetCircleOverlapArea(sunRadius, moonRadius, separation);
        float sunArea = Mathf.PI * sunRadius * sunRadius;
        float coverage = Mathf.Clamp01(overlap / Mathf.Max(sunArea, 0.0000001f));
        return 1f - coverage;
    }

    float GetCircleOverlapArea(float a, float b, float d)
    {
        if (d >= a + b)
            return 0f;

        if (d <= Mathf.Abs(a - b))
        {
            float inner = Mathf.Min(a, b);
            return Mathf.PI * inner * inner;
        }

        float d2 = d * d;
        float a2 = a * a;
        float b2 = b * b;
        float angleA = Mathf.Acos(Mathf.Clamp((d2 + a2 - b2) / (2f * d * a), -1f, 1f));
        float angleB = Mathf.Acos(Mathf.Clamp((d2 + b2 - a2) / (2f * d * b), -1f, 1f));
        float root = Mathf.Sqrt(Mathf.Max(0f, (-d + a + b) * (d + a - b) * (d - a + b) * (d + a + b)));
        return a2 * angleA + b2 * angleB - 0.5f * root;
    }

    Color GetSunLightColor(Material skyMaterial, Vector3 sunDir, float timeOfDay)
    {
        Color sunrise = GetMaterialColor(skyMaterial, "_SunriseColor", new Color(1.0f, 0.62f, 0.40f, 1f));
        Color sunset = GetMaterialColor(skyMaterial, "_SunsetColor", new Color(1.0f, 0.38f, 0.20f, 1f));
        Color day = Color.white;

        float lowSun = (1f - Mathf.SmoothStep(0.08f, 0.42f, sunDir.y))
                     * Mathf.SmoothStep(-0.12f, 0.06f, sunDir.y);
        float morning = 1f - Mathf.SmoothStep(0.22f, 0.50f, timeOfDay);
        float evening = Mathf.SmoothStep(0.50f, 0.78f, timeOfDay);
        Color lowSunColor = Color.Lerp(sunrise, sunset, Mathf.Clamp01(evening / Mathf.Max(morning + evening, 0.001f)));
        float strength = lowSun * Mathf.Clamp01(GetMaterialFloat(skyMaterial, "_LowSunColorStrength", 0.85f) * 1.15f);
        return MultiplyColor(Color.Lerp(day, lowSunColor, strength), _sunLightTint);
    }

    Color GetMoonLightColor(Material skyMaterial)
    {
        Color moonColor = GetMaterialColor(skyMaterial, "_MoonColor", new Color(0.95f, 0.97f, 1.0f, 1f));
        return MultiplyColor(moonColor, _moonLightTint);
    }

    Color MultiplyColor(Color a, Color b)
    {
        return new Color(a.r * b.r, a.g * b.g, a.b * b.b, 1f);
    }

    void EnsureLightDefaults()
    {
        if (_lightDefaultsInitialized)
            return;

        _lightDefaultsInitialized = true;
        _syncLightColor = true;
        if (IsDefaultBlack(_sunLightTint))
            _sunLightTint = Color.white;
        if (IsDefaultBlack(_moonLightTint))
            _moonLightTint = Color.white;
    }

    bool IsDefaultBlack(Color color)
    {
        return color.r <= 0.0001f
            && color.g <= 0.0001f
            && color.b <= 0.0001f
            && color.a <= 0.0001f;
    }

    Vector3 GetSunDirection(Material skyMaterial, float timeOfDay)
    {
        float skyRotation = GetMaterialFloat(skyMaterial, "_SkyRotation", 0f);
        float useSeasonalSun = GetMaterialFloat(skyMaterial, "_UseSeasonalSun", 1f);
        if (useSeasonalSun >= 0.5f)
            return GetSeasonalSunDirection(skyMaterial, timeOfDay, skyRotation);

        return GetBasicSunDirection(skyMaterial, timeOfDay, skyRotation);
    }

    Vector3 GetMoonDirection(Material skyMaterial, float timeOfDay, float siderealRotation)
    {
        float useRealMoonPosition = GetMaterialFloat(skyMaterial, "_UseRealMoonPosition", 1f);
        if (useRealMoonPosition >= 0.5f)
        {
            float rightAscension = GetMaterialFloat(skyMaterial, "_MoonRightAscension", 0f);
            float declination = GetMaterialFloat(skyMaterial, "_MoonDeclination", 0f);
            Vector3 localDir = GetEquatorialLocalDirection(skyMaterial, rightAscension, declination, siderealRotation);
            return LocalToWorldHorizon(skyMaterial, localDir);
        }

        float moonPhase = GetMaterialFloat(skyMaterial, "_MoonPhase", 0f);
        float moonTime = Mathf.Repeat(timeOfDay + moonPhase * 0.5f, 1f);
        return GetSunDirection(skyMaterial, moonTime);
    }

    Vector3 GetBasicSunDirection(Material skyMaterial, float timeOfDay, float skyRotation)
    {
        float angle = (timeOfDay - 0.25f) * Mathf.PI * 2f;
        float rotAngle = skyRotation * Mathf.PI * 2f;
        float sinR = Mathf.Sin(rotAngle);
        float cosR = Mathf.Cos(rotAngle);
        float az = -Mathf.Cos(angle);
        Vector3 localDir = new Vector3(
            az * cosR - 0.3f * sinR,
            Mathf.Sin(angle),
            az * sinR + 0.3f * cosR
        ).normalized;
        return LocalToWorldHorizon(skyMaterial, localDir);
    }

    Vector3 GetSeasonalSunDirection(Material skyMaterial, float timeOfDay, float skyRotation)
    {
        float lat = GetMaterialFloat(skyMaterial, "_CatalogLatitude", 35.6895f) * Mathf.Deg2Rad;
        float dec = GetMaterialFloat(skyMaterial, "_SolarDeclination", 0f) * Mathf.Deg2Rad;
        float hourAngle = (timeOfDay - 0.5f) * Mathf.PI * 2f;

        float sinLat = Mathf.Sin(lat);
        float cosLat = Mathf.Cos(lat);
        float sinDec = Mathf.Sin(dec);
        float cosDec = Mathf.Cos(dec);
        float sinH = Mathf.Sin(hourAngle);
        float cosH = Mathf.Cos(hourAngle);

        float east = cosDec * sinH;
        float up = sinLat * sinDec + cosLat * cosDec * cosH;
        float north = cosLat * sinDec - sinLat * cosDec * cosH;

        float rotAngle = skyRotation * Mathf.PI * 2f;
        float sinR = Mathf.Sin(rotAngle);
        float cosR = Mathf.Cos(rotAngle);
        Vector3 localDir = new Vector3(
            east * cosR - north * sinR,
            up,
            east * sinR + north * cosR
        ).normalized;
        return LocalToWorldHorizon(skyMaterial, localDir);
    }

    Vector3 GetEquatorialLocalDirection(Material skyMaterial, float rightAscension01, float declinationDeg, float siderealRotation)
    {
        float lat = GetMaterialFloat(skyMaterial, "_CatalogLatitude", 35.6895f) * Mathf.Deg2Rad;
        float dec = declinationDeg * Mathf.Deg2Rad;
        float longitude = GetMaterialFloat(skyMaterial, "_CatalogLongitude", 139.6917f);
        float lst = Mathf.Repeat(siderealRotation + longitude / 360f, 1f) * Mathf.PI * 2f;
        float hourAngle = lst - rightAscension01 * Mathf.PI * 2f;

        float sinLat = Mathf.Sin(lat);
        float cosLat = Mathf.Cos(lat);
        float sinDec = Mathf.Sin(dec);
        float cosDec = Mathf.Cos(dec);
        float sinH = Mathf.Sin(hourAngle);
        float cosH = Mathf.Cos(hourAngle);

        return new Vector3(
            cosDec * sinH,
            sinLat * sinDec + cosLat * cosDec * cosH,
            cosLat * sinDec - sinLat * cosDec * cosH
        ).normalized;
    }

    Vector3 LocalToWorldHorizon(Material skyMaterial, Vector3 localDir)
    {
        Vector2 north = GetNorthAxis(skyMaterial);
        Vector2 east = new Vector2(-north.y, north.x);
        Vector2 h = north * localDir.z + east * (-localDir.x);
        return new Vector3(h.x, localDir.y, h.y).normalized;
    }

    Vector2 GetNorthAxis(Material skyMaterial)
    {
        float d = Mathf.Floor(GetMaterialFloat(skyMaterial, "_NorthDirection", 0f) + 0.5f);
        if (d < 0.5f)
            return new Vector2(0f, 1f);
        if (d < 1.5f)
            return new Vector2(0f, -1f);
        if (d < 2.5f)
            return new Vector2(1f, 0f);
        return new Vector2(-1f, 0f);
    }

    float GetMaterialFloat(Material material, string propertyName, float fallback)
    {
        if (material == null || !material.HasProperty(propertyName))
            return fallback;

        return material.GetFloat(propertyName);
    }

    Color GetMaterialColor(Material material, string propertyName, Color fallback)
    {
        if (material == null || !material.HasProperty(propertyName))
            return fallback;

        return material.GetColor(propertyName);
    }

    void UpdateAutoClock(float currentLocalDaySeconds, float currentSiderealRotation, float speed)
    {
        if (!_autoClockInitialized)
        {
            _autoClockInitialized = true;
            _autoClockStartRealtime = Time.time;
            _autoClockBaseRealtime = Time.time;
            _autoClockBaseLocalDaySeconds = currentLocalDaySeconds;
            _autoClockBaseTotalSeconds = 0f;
            _autoClockBaseSiderealRotation = currentSiderealRotation;
            _autoClockSpeed = speed;
            return;
        }

        if (Mathf.Abs(speed - _autoClockSpeed) <= 0.0001f)
            return;

        float currentTotalSeconds = GetAutoTotalSeconds();
        float deltaSeconds = currentTotalSeconds - _autoClockBaseTotalSeconds;
        _autoClockBaseLocalDaySeconds = Mathf.Repeat(_autoClockBaseLocalDaySeconds + deltaSeconds, 86400f);
        _autoClockBaseSiderealRotation = Mathf.Repeat(_autoClockBaseSiderealRotation + deltaSeconds * 1.0027379f / 86400f, 1f);
        _autoClockBaseTotalSeconds = currentTotalSeconds;
        _autoClockBaseRealtime = Time.time;
        _autoClockSpeed = speed;
    }

    float GetAutoTotalSeconds()
    {
        return _autoClockBaseTotalSeconds + (Time.time - _autoClockBaseRealtime) * _autoClockSpeed;
    }

    float GetAutoRealElapsedSeconds()
    {
        if (!_autoClockInitialized)
            return 0f;

        return Mathf.Max(0f, Time.time - _autoClockStartRealtime);
    }

    float GetRawDaySeconds(System.DateTime dt)
    {
        return dt.Hour * 3600f + dt.Minute * 60f + dt.Second;
    }

    float GetSmoothDaySeconds(System.DateTime dt, bool canSmooth)
    {
        float rawSeconds = GetRawDaySeconds(dt);
        if (!canSmooth)
        {
            _smoothClockInitialized = false;
            return rawSeconds;
        }

        int dayKey = dt.Year * 400 + dt.DayOfYear;
        if (!_smoothClockInitialized || Mathf.Abs(dayKey - _smoothClockSecondKey) > 1)
        {
            _smoothClockInitialized = true;
            _smoothClockSecondKey = dayKey;
            _smoothClockBaseSeconds = rawSeconds;
            _smoothClockBaseRealtime = Time.time;
            return rawSeconds;
        }

        _smoothClockSecondKey = dayKey;

        float elapsed = Mathf.Max(0f, Time.time - _smoothClockBaseRealtime);
        float smoothSeconds = Mathf.Repeat(_smoothClockBaseSeconds + elapsed, 86400f);
        float correction = GetWrappedSecondDelta(rawSeconds, smoothSeconds);
        float maxCorrection = Mathf.Max(elapsed * SmoothClockCorrectionRate, 0.0005f);
        smoothSeconds = Mathf.Repeat(smoothSeconds + Mathf.Clamp(correction, -maxCorrection, maxCorrection), 86400f);

        _smoothClockBaseSeconds = smoothSeconds;
        _smoothClockBaseRealtime = Time.time;
        return smoothSeconds;
    }

    float GetWrappedSecondDelta(float smoothSeconds, float rawSeconds)
    {
        float delta = smoothSeconds - rawSeconds;
        if (delta > 43200f)
            delta -= 86400f;
        if (delta < -43200f)
            delta += 86400f;
        return delta;
    }

    float GetSmoothMaterialTimeOfDay(float targetTimeOfDay)
    {
        targetTimeOfDay = Mathf.Repeat(targetTimeOfDay, 1f);
        if (!_timeOfDaySmoothInitialized)
        {
            _timeOfDaySmoothInitialized = true;
            _smoothTimeOfDay = targetTimeOfDay;
            _timeOfDayBaseRealtime = Time.time;
            return _smoothTimeOfDay;
        }

        float elapsed = Mathf.Max(0f, Time.time - _timeOfDayBaseRealtime);
        float predicted = Mathf.Repeat(_smoothTimeOfDay + elapsed / 86400f, 1f);
        float correction = GetWrappedCycleDelta(targetTimeOfDay, predicted);
        float maxCorrection = Mathf.Max(elapsed * TimeOfDayCorrectionRate, 0.0000001f);
        float result = Mathf.Repeat(predicted + Mathf.Clamp(correction, -maxCorrection, maxCorrection), 1f);

        _smoothTimeOfDay = result;
        _timeOfDayBaseRealtime = Time.time;
        return result;
    }

    float GetSmoothSiderealRotation(float targetRotation)
    {
        targetRotation = Mathf.Repeat(targetRotation, 1f);
        if (!_siderealClockInitialized)
        {
            _siderealClockInitialized = true;
            _smoothSiderealRotation = targetRotation;
            _siderealClockBaseRealtime = Time.time;
            return _smoothSiderealRotation;
        }

        float elapsed = Mathf.Max(0f, Time.time - _siderealClockBaseRealtime);
        float predicted = Mathf.Repeat(_smoothSiderealRotation + elapsed / SiderealSecondsPerDay, 1f);
        float correction = GetWrappedCycleDelta(targetRotation, predicted);
        float correctionRate = Mathf.Abs(correction) > 0.01f ? SiderealLargeCorrectionRate : SiderealCorrectionRate;
        float maxCorrection = Mathf.Max(elapsed * correctionRate, 0.0000001f);
        float result = Mathf.Repeat(predicted + Mathf.Clamp(correction, -maxCorrection, maxCorrection), 1f);

        _smoothSiderealRotation = result;
        _siderealClockBaseRealtime = Time.time;
        return result;
    }

    float GetWrappedCycleDelta(float target, float current)
    {
        float delta = target - current;
        if (delta > 0.5f)
            delta -= 1f;
        if (delta < -0.5f)
            delta += 1f;
        return delta;
    }

    float MoveTowardsValue(float current, float target, float maxDelta)
    {
        maxDelta = Mathf.Max(0f, maxDelta);
        float delta = target - current;
        float absDelta = Mathf.Abs(delta);
        if (absDelta <= maxDelta)
            return target;

        if (absDelta <= 0.000001f)
            return target;

        return current + delta / absDelta * maxDelta;
    }

    float MoveTowardsCycle(float current, float target, float maxDelta)
    {
        maxDelta = Mathf.Max(0f, maxDelta);
        float delta = GetWrappedCycleDelta(target, current);
        float absDelta = Mathf.Abs(delta);
        if (absDelta <= maxDelta)
            return Mathf.Repeat(target, 1f);

        if (absDelta <= 0.000001f)
            return Mathf.Repeat(target, 1f);

        return Mathf.Repeat(current + delta / absDelta * maxDelta, 1f);
    }

    System.DateTime GetOverrideDateTime()
    {
        int year = Mathf.Clamp(_overrideYear, 1900, 2100);
        int month = Mathf.Clamp(_overrideMonth, 1, 12);
        int day = Mathf.Clamp(_overrideDay, 1, GetDaysInMonth(year, month));
        int hour = Mathf.Clamp(_overrideHour, 0, 23);
        int minute = Mathf.Clamp(_overrideMinute, 0, 59);
        return new System.DateTime(year, month, day, hour, minute, 0);
    }

    System.DateTime ApplyOverrideSolarClock(System.DateTime dt, float solarOffsetSeconds)
    {
        double civilSeconds = Mathf.Repeat(_overrideValue, 1f) * 86400.0 - solarOffsetSeconds;
        int dayOffset = 0;
        while (civilSeconds < 0.0)
        {
            civilSeconds += 86400.0;
            dayOffset--;
        }
        while (civilSeconds >= 86400.0)
        {
            civilSeconds -= 86400.0;
            dayOffset++;
        }

        System.DateTime midnight = new System.DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        return midnight.AddSeconds(dayOffset * 86400.0 + civilSeconds);
    }

    float GetSolarTimeOffsetSeconds(Material material, System.DateTime localDateTime)
    {
        if (GetMaterialFloat(material, "_UseSeasonalSun", 1f) < 0.5f)
            return 0f;

        float longitude = GetMaterialFloat(material, "_CatalogLongitude", 139.6917f);
        float standardMeridian = GetZoneOffset(_timeZone) / 240f;
        float equationOfTimeMinutes = GetEquationOfTimeMinutes(localDateTime);
        float longitudeMinutes = 4f * (longitude - standardMeridian);
        return (equationOfTimeMinutes + longitudeMinutes) * 60f;
    }

    float GetEquationOfTimeMinutes(System.DateTime localDateTime)
    {
        float b = Mathf.Deg2Rad * (360f / 365f) * (localDateTime.DayOfYear - 81);
        return 9.87f * Mathf.Sin(2f * b) - 7.53f * Mathf.Cos(b) - 1.5f * Mathf.Sin(b);
    }

    int GetDaysInMonth(int year, int month)
    {
        if (month == 2)
            return IsLeapYear(year) ? 29 : 28;

        if (month == 4 || month == 6 || month == 9 || month == 11)
            return 30;

        return 31;
    }

    bool IsLeapYear(int year)
    {
        if (year % 400 == 0)
            return true;

        if (year % 100 == 0)
            return false;

        return year % 4 == 0;
    }

    float GetZoneOffset(AetherTimeZonePreset zone)
    {
        if (zone == AetherTimeZonePreset.PST_Los_Angeles)  return -28800f;
        if (zone == AetherTimeZonePreset.MST_Denver)       return -25200f;
        if (zone == AetherTimeZonePreset.CST_Chicago)      return -21600f;
        if (zone == AetherTimeZonePreset.EST_New_York)     return -18000f;
        if (zone == AetherTimeZonePreset.BRT_Sao_Paulo)    return -10800f;
        if (zone == AetherTimeZonePreset.GMT_London)       return      0f;
        if (zone == AetherTimeZonePreset.CET_Paris_Berlin) return   3600f;
        if (zone == AetherTimeZonePreset.EET_Helsinki)     return   7200f;
        if (zone == AetherTimeZonePreset.MSK_Moscow)       return  10800f;
        if (zone == AetherTimeZonePreset.IST_India)        return  19800f;
        if (zone == AetherTimeZonePreset.CST_Shanghai)     return  28800f;
        if (zone == AetherTimeZonePreset.JST_Tokyo)        return  32400f;
        if (zone == AetherTimeZonePreset.KST_Seoul)        return  32400f;
        if (zone == AetherTimeZonePreset.AEST_Sydney)      return  36000f;
        if (zone == AetherTimeZonePreset.NZST_Auckland)    return  43200f;
        return 0f;
    }

    void SyncCatalogSiderealTime(Material material, float siderealRotation, float timeOfDay)
    {
        if (!material.HasProperty("_CatalogUseLocation"))
            return;

        bool useCatalogLocation = material.GetFloat("_CatalogUseLocation") >= 0.5f;
        bool useMoonPosition = material.HasProperty("_UseRealMoonPosition")
                            && material.GetFloat("_UseRealMoonPosition") >= 0.5f;
        if (!useCatalogLocation && !useMoonPosition)
            return;

        SetFloatIfChanged(material, "_CatalogSiderealRotation", siderealRotation, 0.0000001f);
        SetFloatIfChanged(material, "_CatalogSiderealBaseTimeOfDay", Mathf.Repeat(timeOfDay, 1f), 0.0000001f);

        if (useCatalogLocation && material.HasProperty("_CatalogLatitude"))
        {
            float latitude = material.GetFloat("_CatalogLatitude");
            float latRad = latitude * Mathf.Deg2Rad;
            SetFloatIfChanged(material, "_StarAxisX", 0f, 0.0001f);
            SetFloatIfChanged(material, "_StarAxisY", Mathf.Sin(latRad), 0.0001f);
            SetFloatIfChanged(material, "_StarAxisZ", Mathf.Cos(latRad), 0.0001f);
        }
    }

    void SyncSeasonalSun(Material material, System.DateTime dt, bool canSmooth)
    {
        if (!material.HasProperty("_UseSeasonalSun"))
            return;

        if (material.GetFloat("_UseSeasonalSun") < 0.5f)
        {
            _solarDeclinationSmoothInitialized = false;
            return;
        }

        float targetDeclination = GetSolarDeclination(dt);
        float declination = targetDeclination;
        if (canSmooth)
        {
            if (!_solarDeclinationSmoothInitialized)
            {
                _solarDeclinationSmoothInitialized = true;
                _smoothSolarDeclination = targetDeclination;
                _solarDeclinationBaseRealtime = Time.time;
            }
            else
            {
                float elapsed = Mathf.Max(0f, Time.time - _solarDeclinationBaseRealtime);
                _smoothSolarDeclination = MoveTowardsValue(_smoothSolarDeclination, targetDeclination, elapsed * SolarDeclinationCorrectionRate);
                _solarDeclinationBaseRealtime = Time.time;
            }
            declination = _smoothSolarDeclination;
        }
        else
        {
            _solarDeclinationSmoothInitialized = false;
        }

        SetFloatIfChanged(material, "_SolarDeclination", declination, 0.0001f);
    }

    float GetSolarDeclination(System.DateTime dt)
    {
        int dayOfYear = dt.DayOfYear;
        float angle = Mathf.Deg2Rad * (360f / 365f) * (dayOfYear - 81);
        return 23.44f * Mathf.Sin(angle);
    }

    void SyncMoon(Material material, System.DateTime dt, float utcOffsetSeconds, float extraSeconds, bool updatePosition, bool canSmooth)
    {
        if (material.HasProperty("_UseMoonPhaseSync") && material.GetFloat("_UseMoonPhaseSync") >= 0.5f)
        {
            float targetPhase = GetMoonPhase(dt, utcOffsetSeconds, extraSeconds);
            float moonPhase = targetPhase;
            if (canSmooth)
            {
                if (!_moonPhaseSmoothInitialized)
                {
                    _moonPhaseSmoothInitialized = true;
                    _smoothMoonPhase = targetPhase;
                    _moonPhaseBaseRealtime = Time.time;
                }
                else
                {
                    float elapsed = Mathf.Max(0f, Time.time - _moonPhaseBaseRealtime);
                    _smoothMoonPhase = MoveTowardsValue(_smoothMoonPhase, targetPhase, elapsed * MoonPhaseCorrectionRate);
                    _moonPhaseBaseRealtime = Time.time;
                }
                moonPhase = _smoothMoonPhase;
            }
            else
            {
                _moonPhaseSmoothInitialized = false;
            }

            SetFloatIfChanged(material, "_MoonPhase", moonPhase, 0.0001f);
        }
        else
        {
            _moonPhaseSmoothInitialized = false;
        }

        if (!updatePosition)
            return;

        if (!material.HasProperty("_UseRealMoonPosition"))
            return;

        if (material.GetFloat("_UseRealMoonPosition") < 0.5f)
        {
            _moonPositionSmoothInitialized = false;
            return;
        }

        double jd = GetJulianDate(dt, utcOffsetSeconds, extraSeconds);
        double d = jd - 2451545.0;
        float l = Mathf.Repeat((float)(218.316 + 13.176396 * d), 360f);
        float m = Mathf.Repeat((float)(134.963 + 13.064993 * d), 360f);
        float f = Mathf.Repeat((float)(93.272 + 13.229350 * d), 360f);
        float lon = l + 6.289f * Mathf.Sin(m * Mathf.Deg2Rad);
        float lat = 5.128f * Mathf.Sin(f * Mathf.Deg2Rad);
        float obliq = 23.439f - 0.0000004f * (float)d;

        float lonRad = lon * Mathf.Deg2Rad;
        float latRad = lat * Mathf.Deg2Rad;
        float obRad = obliq * Mathf.Deg2Rad;
        float sinLon = Mathf.Sin(lonRad);
        float cosLon = Mathf.Cos(lonRad);
        float sinLat = Mathf.Sin(latRad);
        float cosLat = Mathf.Cos(latRad);
        float sinOb = Mathf.Sin(obRad);
        float cosOb = Mathf.Cos(obRad);

        float tanLat = sinLat / Mathf.Max(cosLat, 0.0001f);
        float ra = Mathf.Atan2(sinLon * cosOb - tanLat * sinOb, cosLon);
        float dec = Mathf.Asin(sinLat * cosOb + cosLat * sinOb * sinLon);
        float targetRightAscension = Mathf.Repeat(ra / (Mathf.PI * 2f), 1f);
        float targetDeclination = dec * Mathf.Rad2Deg;
        float rightAscension = targetRightAscension;
        float declination = targetDeclination;

        if (canSmooth)
        {
            if (!_moonPositionSmoothInitialized)
            {
                _moonPositionSmoothInitialized = true;
                _smoothMoonRightAscension = targetRightAscension;
                _smoothMoonDeclination = targetDeclination;
                _moonPositionBaseRealtime = Time.time;
            }
            else
            {
                float elapsed = Mathf.Max(0f, Time.time - _moonPositionBaseRealtime);
                _smoothMoonRightAscension = MoveTowardsCycle(_smoothMoonRightAscension, targetRightAscension, elapsed * MoonRightAscensionCorrectionRate);
                _smoothMoonDeclination = MoveTowardsValue(_smoothMoonDeclination, targetDeclination, elapsed * MoonDeclinationCorrectionRate);
                _moonPositionBaseRealtime = Time.time;
            }

            rightAscension = _smoothMoonRightAscension;
            declination = _smoothMoonDeclination;
        }
        else
        {
            _moonPositionSmoothInitialized = false;
        }

        SetFloatIfChanged(material, "_MoonRightAscension", Mathf.Repeat(rightAscension, 1f), 0.0001f);
        SetFloatIfChanged(material, "_MoonDeclination", declination, 0.0001f);
    }

    void SetFloatIfChanged(Material material, string propertyName, float value, float epsilon)
    {
        if (!material.HasProperty(propertyName))
            return;

        if (Mathf.Abs(material.GetFloat(propertyName) - value) <= epsilon)
            return;

        material.SetFloat(propertyName, value);
    }

    float GetMoonPhase(System.DateTime dt, float utcOffsetSeconds, float extraSeconds)
    {
        double jd = GetJulianDate(dt, utcOffsetSeconds, extraSeconds);
        double age = jd - 2451550.1;
        age -= System.Math.Floor(age / 29.530588853) * 29.530588853;
        if (age < 0.0)
            age += 29.530588853;
        float cycle = (float)(age / 29.530588853);
        return 1f - Mathf.Abs(cycle * 2f - 1f);
    }

    float GetGreenwichSiderealRotation(System.DateTime dt, float utcOffsetSeconds, float extraSeconds)
    {
        double jd = GetJulianDate(dt, utcOffsetSeconds, extraSeconds);
        double d = jd - 2451545.0;
        double gmst = 280.46061837 + 360.98564736629 * d;
        return Mathf.Repeat((float)(gmst / 360.0), 1f);
    }

    double GetJulianDate(System.DateTime dt, float utcOffsetSeconds, float extraSeconds)
    {
        int year = dt.Year;
        int month = dt.Month;
        double seconds = dt.Hour * 3600.0 + dt.Minute * 60.0 + dt.Second + extraSeconds - utcOffsetSeconds;
        double day = dt.Day + seconds / 86400.0;

        if (month <= 2)
        {
            year -= 1;
            month += 12;
        }

        int a = year / 100;
        int b = 2 - a + (a / 4);
        return System.Math.Floor(365.25 * (year + 4716))
             + System.Math.Floor(30.6001 * (month + 1))
             + day + b - 1524.5;
    }

    /// <summary>
    /// 研究用：ネットワーク時刻ではなく固定の空の時刻を使う。
    /// normalizedTime … 0=真夜中, 0.5=南中, 0.75=日没側（Override Value と同義）
    /// </summary>
    public void SetManualTimeOfDay(float normalizedTime)
    {
        _overrideTime = true;
        _overrideDateTime = false;
        _autoRotate = false;
        _overrideValue = Mathf.Clamp01(normalizedTime);
    }
}
