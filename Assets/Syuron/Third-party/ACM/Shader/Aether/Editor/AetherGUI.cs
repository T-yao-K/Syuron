using UnityEngine;
using UnityEditor;

public class AetherGUI : ShaderGUI
{
    private bool _showTime  = true;
    private bool _showSunMoon = true;
    private bool _showAtmo  = true;
    private bool _showStars = true;
    private bool _showMilkyWay = true;
    private bool _jp = true;
    private readonly string[] _northDirectionNames = { "+Z", "-Z", "+X", "-X" };
    private readonly string[] _locationPresetNames =
    {
        "Custom",
        "Tokyo",
        "Sapporo",
        "Naha",
        "London",
        "Paris",
        "New York",
        "Los Angeles",
        "Sydney"
    };

    private string T(string ja, string en) => _jp ? ja : en;
    private GUIContent C(string ja, string en, string tipJa, string tipEn)
    {
        return new GUIContent(T(ja, en), T(tipJa, tipEn));
    }

    private MaterialProperty P(string name, MaterialProperty[] props) => FindProperty(name, props, false);

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        var timeOfDay     = P("_TimeOfDay",        props);
        var autoRotate    = P("_AutoRotate",       props);
        var autoSpeed     = P("_AutoSpeed",        props);
        var timeControllerActive = P("_AetherTimeControllerActive", props);
        var skyRotation   = P("_SkyRotation",      props);
        var northDirection = P("_NorthDirection",  props);
        var sunIntensity  = P("_SunIntensity",     props);
        var rayleigh      = P("_RayleighStrength", props);
        var mie           = P("_MieStrength",      props);
        var sunSize       = P("_SunSize",          props);
        var sunDiskIntensity = P("_SunDiskIntensity", props);
        var sunDiskSoftness = P("_SunDiskSoftness", props);
        var sunriseColor  = P("_SunriseColor",     props);
        var sunsetColor   = P("_SunsetColor",      props);
        var lowSunColorStrength = P("_LowSunColorStrength", props);
        var exposure      = P("_Exposure",         props);
        var useSeasonalSun = P("_UseSeasonalSun", props);
        var solarDeclination = P("_SolarDeclination", props);
        var starDensity   = P("_StarDensity",      props);
        var starScale     = P("_StarScale",        props);
        var starSeed      = P("_StarSeed",         props);
        var starBright    = P("_StarBrightness",   props);
        var starSpeed     = P("_StarRotateSpeed",  props);
        var starAxisX     = P("_StarAxisX",        props);
        var starAxisY     = P("_StarAxisY",        props);
        var starAxisZ     = P("_StarAxisZ",        props);
        var star1Color    = P("_Star1Color",       props);
        var star2Color    = P("_Star2Color",       props);
        var star2Amount   = P("_Star2Amount",      props);
        var star3Color    = P("_Star3Color",       props);
        var star3Amount   = P("_Star3Amount",      props);
        var starFadeStr   = P("_StarFadeStrength", props);
        var starFadeH     = P("_StarFadeHeight",   props);
        var starGlowStr   = P("_StarGlowStrength", props);
        var starGlowSize  = P("_StarGlowSize",     props);
        var starCrossTh   = P("_StarCrossThreshold", props);
        var starCrossRot  = P("_StarCrossRotation",  props);
        var starCrossSize = P("_StarCrossSize",      props);
        var starCrossLen  = P("_StarCrossLength",    props);
        var starCrossOpac = P("_StarCrossOpacity",   props);
        var starTwinkleRange = P("_StarTwinkleRange", props);
        var starTwinkleInvert = P("_StarTwinkleInvert", props);
        var starTwinkleSpeed = P("_StarTwinkleSpeed", props);
        var starTwinkleStrength = P("_StarTwinkleStrength", props);
        var useCatalogStars = P("_UseCatalogStars",       props);
        var catalogDataMap  = P("_CatalogStarDataMap",    props);
        var catalogColorMap = P("_CatalogStarColorMap",   props);
        var catalogCellMap  = P("_CatalogStarCellMap",    props);
        var catalogStarCount = P("_CatalogStarCount",     props);
        var catalogDataWidth = P("_CatalogDataMapWidth",  props);
        var catalogCellLon  = P("_CatalogCellLonCount",   props);
        var catalogCellLat  = P("_CatalogCellLatCount",   props);
        var catalogStarSize = P("_CatalogStarSize",       props);
        var catalogStarSharpness = P("_CatalogStarSharpness", props);
        var catalogStarGlow = P("_CatalogStarGlow",       props);
        var catalogStarEmission = P("_CatalogStarEmission", props);
        var catalogStarBright = P("_CatalogStarBrightness", props);
        var catalogStarRot = P("_CatalogStarRotation",    props);
        var catalogUseLocation = P("_CatalogUseLocation", props);
        var catalogLocationPreset = P("_CatalogLocationPreset", props);
        var catalogLatitude = P("_CatalogLatitude", props);
        var catalogLongitude = P("_CatalogLongitude", props);
        var catalogSiderealRot = P("_CatalogSiderealRotation", props);
        var useOrionLines = P("_UseOrionLines", props);
        var orionLineColor = P("_OrionLineColor", props);
        var orionLineStrength = P("_OrionLineStrength", props);
        var orionLineWidth = P("_OrionLineWidth", props);
        var useMeteors = P("_UseMeteors", props);
        var meteorFrequency = P("_MeteorFrequency", props);
        var meteorColor = P("_MeteorColor", props);
        var meteorIntensity = P("_MeteorIntensity", props);
        var meteorSpeed = P("_MeteorSpeed", props);
        var meteorLength = P("_MeteorLength", props);
        var meteorWidth = P("_MeteorWidth", props);
        var meteorDuration = P("_MeteorDuration", props);
        var meteorSeed = P("_MeteorSeed", props);
        var moonPhase     = P("_MoonPhase",        props);
        var moonSize      = P("_MoonSize",         props);
        var moonColor     = P("_MoonColor",        props);
        var moonHaloColor = P("_MoonHaloColor",    props);
        var moonHaloStr   = P("_MoonHaloStrength", props);
        var moonStarSupp  = P("_MoonStarSuppress", props);
        var moonStarRange = P("_MoonStarRange",    props);
        var useMoonPhaseSync = P("_UseMoonPhaseSync", props);
        var useRealMoonPosition = P("_UseRealMoonPosition", props);
        var moonRa = P("_MoonRightAscension", props);
        var moonDec = P("_MoonDeclination", props);
        var useMilkyWay = P("_UseMilkyWay", props);
        var useMilkyWayDensityMap = P("_UseMilkyWayDensityMap", props);
        var milkyWayDensityMap = P("_MilkyWayDensityMap", props);
        var useMilkyWayDustMap = P("_UseMilkyWayDustMap", props);
        var milkyWayDustMap = P("_MilkyWayDustMap", props);
        var milkyWayDustStrength = P("_MilkyWayDustStrength", props);
        var milkyWayStrength = P("_MilkyWayStrength", props);
        var milkyWayWidth = P("_MilkyWayWidth", props);
        var milkyWayDetail = P("_MilkyWayDetail", props);
        var milkyWaySaturation = P("_MilkyWaySaturation", props);
        var milkyWayCyanBoost = P("_MilkyWayCyanBoost", props);
        var milkyWayTintAmount = P("_MilkyWayTintAmount", props);
        var milkyWayCoreWarmth = P("_MilkyWayCoreWarmth", props);
        var milkyWayColor = P("_MilkyWayColor", props);
        var useProceduralClouds = P("_UseProceduralClouds", props);
        var cloudAmount = P("_CloudAmount", props);
        var cloudScale = P("_CloudScale", props);
        var cloudSoftness = P("_CloudSoftness", props);
        var cloudOpacity = P("_CloudOpacity", props);
        var cloudHeight = P("_CloudHeight", props);
        var cloudSpeed = P("_CloudSpeed", props);
        var cloudThickness = P("_CloudThickness", props);
        var cloudLayerDepth = P("_CloudLayerDepth", props);
        var cloudHorizonFlatten = P("_CloudHorizonFlatten", props);
        var cloudCurvatureDrop = P("_CloudCurvatureDrop", props);
        var cloudStarOcclusion = P("_CloudStarOcclusion", props);
        var cloudColor = P("_CloudColor", props);
        var cloudSunsetTint = P("_CloudSunsetTint", props);
        var cloudSunsetStrength = P("_CloudSunsetStrength", props);
        var useCloudMoonHighlight = P("_UseCloudMoonHighlight", props);
        var cloudMoonHighlightStrength = P("_CloudMoonHighlightStrength", props);
        var cloudMoonHighlightRange = P("_CloudMoonHighlightRange", props);
        var cloudMoonHighlightColor = P("_CloudMoonHighlightColor", props);
        var cloudMoonHighlightAltitude = P("_CloudMoonHighlightAltitude", props);
        var useCityLight = P("_UseCityLight", props);
        var cityLightColor = P("_CityLightColor", props);
        var cityLightStrength = P("_CityLightStrength", props);
        var cityLightUseDirection = P("_CityLightUseDirection", props);
        var cityLightDirection = P("_CityLightDirection", props);
        var cityLightSpread = P("_CityLightSpread", props);
        var cityLightStarOcclusion = P("_CityLightStarOcclusion", props);
        var cityLightCloudReflection = P("_CityLightCloudReflection", props);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        _jp = GUILayout.Toolbar(_jp ? 0 : 1, new[] { "JP", "EN" }, GUILayout.Width(74)) == 0;
        EditorGUILayout.EndHorizontal();

        _showTime = EditorGUILayout.BeginFoldoutHeaderGroup(_showTime, T("時刻・方向", "Time & Direction"));
        if (_showTime)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(T("時刻", "Time"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            float tod  = timeOfDay.floatValue;
            int   hour = Mathf.FloorToInt(tod * 24f);
            int   min  = Mathf.FloorToInt((tod * 24f - hour) * 60f);
            string timeLabel = _jp
                ? $"{hour:00}:{min:00}"
                : $"{(hour % 12 == 0 ? 12 : hour % 12)}:{min:00} {(hour < 12 ? "AM" : "PM")}";
            bool scriptManagedTime = Application.isPlaying && timeControllerActive != null && timeControllerActive.floatValue > 0.5f;
            if (scriptManagedTime)
                EditorGUILayout.HelpBox(T("AetherTimeが時刻を管理中です。時刻はAetherTime側で変更します。", "AetherTime is controlling sky time. Change time on the AetherTime component."), MessageType.Info);
            EditorGUI.BeginDisabledGroup(scriptManagedTime);
            materialEditor.ShaderProperty(timeOfDay, new GUIContent($"{T("時刻", "Time of Day")}  [{timeLabel}]", T("太陽時として扱う空の時刻です。0.5で太陽が南中します。実時間同期時は経度・タイムゾーン・均時差から自動補正されます。", "Sky time interpreted as solar time. 0.5 places the sun at local solar noon. Real-time sync corrects it from longitude, time zone, and the equation of time.")));
            if (autoRotate != null)
            {
                autoRotate.floatValue = EditorGUILayout.Toggle(
                    C("自動回転", "Auto Rotate", "マテリアル単体で時刻を自動進行させます。実時間同期を使う場合はOFF推奨です。", "Animates sky time inside the material. Turn off when real-time sync is used."),
                    autoRotate.floatValue > 0.5f) ? 1.0f : 0.0f;
                if (autoRotate.floatValue > 0.5f && autoSpeed != null)
                    materialEditor.ShaderProperty(autoSpeed, C("速度", "Speed", "自動回転の速度です。1で現実時間相当、1440で現実の1分が空の1日です。", "Auto rotation speed. 1 is real-time scale; 1440 makes one real minute equal one sky day."));
            }
            EditorGUI.EndDisabledGroup();
            materialEditor.ShaderProperty(skyRotation, C("水平回転", "Sky Rotation", "空全体を水平に回転します。方角合わせの微調整に使います。", "Rotates the whole sky around the horizon for direction alignment."));
            if (northDirection != null)
            {
                int north = Mathf.Clamp(Mathf.RoundToInt(northDirection.floatValue), 0, _northDirectionNames.Length - 1);
                north = EditorGUILayout.Popup(C("北方向", "North Direction", "ワールド上で北として扱う水平軸です。太陽・星・月の方角基準になります。", "World horizontal axis treated as north for sun, stars, and moon."), north, _northDirectionNames);
                northDirection.floatValue = north;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        _showSunMoon = EditorGUILayout.BeginFoldoutHeaderGroup(_showSunMoon, T("太陽・月", "Sun & Moon"));
        if (_showSunMoon)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(T("太陽", "Sun"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(sunIntensity, C("強度", "Intensity", "太陽光と昼空の明るさを調整します。", "Controls sunlight and daytime sky brightness."));
            materialEditor.ShaderProperty(sunSize,      C("サイズ", "Size", "太陽円盤の見た目の大きさです。", "Visual size of the sun disk."));
            if (sunDiskIntensity != null)
                materialEditor.ShaderProperty(sunDiskIntensity, C("まぶしさ", "Disk Intensity", "空全体ではなく、太陽円盤だけのHDR発光の強さです。Bloom/PPSにも乗りやすくなります。", "HDR brightness of the sun disk only, without brightening the whole sky. This is easier for Bloom/PPS to catch."));
            if (sunDiskSoftness != null)
                materialEditor.ShaderProperty(sunDiskSoftness, C("輪郭ぼかし", "Disk Softness", "太陽円盤の縁の柔らかさです。上げるほど輪郭がはっきりしにくくなります。", "Softness of the sun disk edge. Higher values make the outline less sharp."));
            if (sunriseColor != null)
                materialEditor.ShaderProperty(sunriseColor, C("朝焼け色", "Sunrise Color", "太陽が低い朝に使う太陽光の色です。", "Sunlight color used when the morning sun is low."));
            if (sunsetColor != null)
                materialEditor.ShaderProperty(sunsetColor, C("夕焼け色", "Sunset Color", "太陽が低い夕方に使う太陽光の色です。", "Sunlight color used when the evening sun is low."));
            if (lowSunColorStrength != null)
                materialEditor.ShaderProperty(lowSunColorStrength, C("低高度の色", "Low Sun Color", "太陽が地平線付近にある時、朝焼け/夕焼け色へ寄せる強さです。", "How strongly low sun shifts toward sunrise/sunset colors near the horizon."));
            if (useSeasonalSun != null)
            {
                useSeasonalSun.floatValue = EditorGUILayout.Toggle(
                    C("季節同期", "Seasonal Sun", "太陽赤緯を使い、季節による太陽高度を反映します。日付同期中は自動で更新されます。", "Uses solar declination so sun height follows the season. Date sync updates it automatically."),
                    useSeasonalSun.floatValue > 0.5f) ? 1.0f : 0.0f;
                if (useSeasonalSun.floatValue > 0.5f && solarDeclination != null)
                    materialEditor.ShaderProperty(solarDeclination, C("太陽赤緯", "Solar Declination", "太陽の季節角度です。同期中は自動で更新されます。", "Seasonal solar angle. Updated automatically while synced."));
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
            GUILayout.Label(T("月", "Moon"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(T("朔望", "Phase"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (useMoonPhaseSync != null)
            {
                useMoonPhaseSync.floatValue = EditorGUILayout.Toggle(
                    C("月齢同期", "Sync Phase", "設定した日付から月齢を自動計算します。", "Automatically computes moon phase from the selected date."),
                    useMoonPhaseSync.floatValue > 0.5f) ? 1.0f : 0.0f;
            }
            materialEditor.ShaderProperty(moonPhase, C("月齢", "Phase", "0が新月、1が満月です。月齢同期中は自動で更新されます。", "0 is new moon, 1 is full moon. Updated automatically while phase sync is active."));
            materialEditor.ShaderProperty(moonSize,  C("サイズ", "Size", "月の見た目の大きさです。", "Visual size of the moon."));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(T("位置", "Position"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (useRealMoonPosition != null)
            {
                useRealMoonPosition.floatValue = EditorGUILayout.Toggle(
                    C("実時刻同期", "Sync Position", "日付から月の位置を近似計算し、太陽とは別の動きで表示します。", "Approximates moon position from the date so it moves separately from the sun."),
                    useRealMoonPosition.floatValue > 0.5f) ? 1.0f : 0.0f;
                bool syncMoon = useRealMoonPosition.floatValue > 0.5f;
                EditorGUI.BeginDisabledGroup(syncMoon);
                if (moonRa != null)
                    materialEditor.ShaderProperty(moonRa, C("赤経", "Right Ascension", "月の赤経です。位置同期中は自動で更新されます。", "Moon right ascension. Updated automatically while position sync is enabled."));
                if (moonDec != null)
                    materialEditor.ShaderProperty(moonDec, C("赤緯", "Declination", "月の赤緯です。位置同期中は自動で更新されます。", "Moon declination. Updated automatically while position sync is enabled."));
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(T("色", "Color"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(moonColor,    C("月", "Moon", "月の明るい部分の色です。", "Color of the lit part of the moon."));
            materialEditor.ShaderProperty(moonHaloColor, C("暈", "Halo", "月の周囲の淡い光の色です。", "Color of the soft halo around the moon."));
            materialEditor.ShaderProperty(moonHaloStr,  C("暈の強度", "Halo Strength", "月の周囲に出る霞/暈の強さです。", "Strength of the moon halo/haze."));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(T("周辺の星", "Stars Around Moon"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(moonStarSupp,  C("強度", "Strength", "月の円盤と周囲で星をどれだけ消すかです。", "How strongly stars are suppressed around the moon disk."));
            materialEditor.ShaderProperty(moonStarRange, C("範囲", "Range", "月の周囲で星を減衰させる範囲です。", "Radius around the moon where stars fade."));
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        _showAtmo = EditorGUILayout.BeginFoldoutHeaderGroup(_showAtmo, T("大気", "Atmosphere"));
        if (_showAtmo)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(T("散乱", "Scattering"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(rayleigh, C("Rayleigh", "Rayleigh", "青空や夕焼けの散乱感を調整します。", "Controls Rayleigh scattering, affecting blue sky and sunset tint."));
            materialEditor.ShaderProperty(mie,      C("Mie", "Mie", "太陽周辺の霞や白っぽい散乱を調整します。", "Controls hazy scattering around the sun."));
            materialEditor.ShaderProperty(exposure, C("露出", "Exposure", "空全体の明るさです。星のHDR発光にも影響します。", "Overall sky exposure. Also affects HDR star emission."));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
            GUILayout.Label(T("雲", "Clouds"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (useProceduralClouds != null)
            {
                useProceduralClouds.floatValue = EditorGUILayout.Toggle(
                    C("表示", "Enable", "空に薄い雲を表示します。量・透明度・流速で見え方を調整できます。", "Shows thin clouds in the sky. Amount, opacity, and flow speed adjust the look."),
                    useProceduralClouds.floatValue > 0.5f) ? 1.0f : 0.0f;
                if (useProceduralClouds.floatValue > 0.5f)
                {
                    if (cloudAmount != null)
                        materialEditor.ShaderProperty(cloudAmount, C("量", "Amount", "雲の出る量です。0でほぼ無し、1で広く出ます。", "Cloud coverage. 0 is almost none; 1 covers much of the sky."));
                    if (cloudOpacity != null)
                        materialEditor.ShaderProperty(cloudOpacity, C("透明度", "Opacity", "雲の濃さです。星や天の川の隠れ方にも影響します。", "Cloud opacity. Also affects how stars and the Milky Way are hidden."));
                    if (cloudScale != null)
                        materialEditor.ShaderProperty(cloudScale, C("スケール", "Scale", "雲模様の大きさです。小さいほど大きな雲になります。", "Cloud pattern scale. Lower values make larger clouds."));
                    if (cloudSoftness != null)
                        materialEditor.ShaderProperty(cloudSoftness, C("ぼかし", "Softness", "雲の境界の柔らかさです。", "Softness of cloud edges."));
                    if (cloudHeight != null)
                        materialEditor.ShaderProperty(cloudHeight, C("高さ", "Height", "雲を出し始める空の高さです。", "Sky height where clouds begin to appear."));
                    if (cloudSpeed != null)
                        materialEditor.ShaderProperty(cloudSpeed, C("流速", "Flow Speed", "雲が流れる速度です。負数で逆方向に流れます。", "Cloud flow speed. Negative values move in the opposite direction."));
                    if (cloudThickness != null)
                        materialEditor.ShaderProperty(cloudThickness, C("厚み", "Thickness", "雲の陰影を強めて厚みを出します。0で薄雲寄りです。", "Adds stronger cloud shading for a thicker look. 0 keeps a thin-cloud look."));
                    if (cloudLayerDepth != null)
                        materialEditor.ShaderProperty(cloudLayerDepth, C("層の奥行き", "Layer Depth", "複数の雲層を重ねて奥行きを足します。上げるほど負荷も少し増えます。", "Adds depth by layering multiple cloud bands. Higher values add a little more shader cost."));
                    if (cloudHorizonFlatten != null)
                        materialEditor.ShaderProperty(cloudHorizonFlatten, C("遠景なじみ", "Horizon Flatten", "地平線方向の細かい模様を減らし、遠くの雲層っぽく見せます。", "Reduces fine detail near the horizon so distant clouds read as broader layers."));
                    if (cloudCurvatureDrop != null)
                        materialEditor.ShaderProperty(cloudCurvatureDrop, C("遠景沈み", "Curvature Drop", "遠くの雲が地平線へ少し沈んで見えるようにします。", "Makes distant clouds appear to sink slightly toward the horizon."));
                    if (cloudStarOcclusion != null)
                        materialEditor.ShaderProperty(cloudStarOcclusion, C("星の減衰", "Star Occlusion", "雲の向こうの星や天の川を消す強さです。1以上でより強く消せます。", "How strongly clouds hide stars and the Milky Way. Values above 1 hide them more aggressively."));
                    if (cloudColor != null)
                        materialEditor.ShaderProperty(cloudColor, C("色", "Color", "雲の基本色です。昼夜で自動的に明るさが変わります。", "Base cloud color. Brightness changes automatically between day and night."));
                    if (cloudSunsetTint != null)
                        materialEditor.ShaderProperty(cloudSunsetTint, C("夕焼け色", "Sunset Tint", "太陽が低い時に雲の縁や下面へ乗せる夕焼け色です。", "Warm tint added to cloud edges and undersides when the sun is low."));
                    if (cloudSunsetStrength != null)
                        materialEditor.ShaderProperty(cloudSunsetStrength, C("夕焼け反映", "Sunset Amount", "夕方の雲に夕焼け色を反映する強さです。0で無効、上げるほど赤みが強くなります。", "How strongly sunset tint affects clouds. 0 disables it; higher values make warmer evening clouds."));
                    if (useCloudMoonHighlight != null)
                    {
                        EditorGUILayout.Space(4);
                        EditorGUILayout.LabelField(T("月雲ハイライト", "Moonlit Clouds"), EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        useCloudMoonHighlight.floatValue = EditorGUILayout.Toggle(
                            C("表示", "Enable", "月の方角にある雲を夜だけ淡く明るくします。", "Lightens clouds near the moon direction at night."),
                            useCloudMoonHighlight.floatValue > 0.5f) ? 1.0f : 0.0f;
                        if (useCloudMoonHighlight.floatValue > 0.5f)
                        {
                            if (cloudMoonHighlightStrength != null)
                                materialEditor.ShaderProperty(cloudMoonHighlightStrength, C("強度", "Strength", "月明かりで雲が明るくなる強さです。", "How strongly moonlight brightens the clouds."));
                            if (cloudMoonHighlightRange != null)
                                materialEditor.ShaderProperty(cloudMoonHighlightRange, C("範囲", "Range", "月の周囲どれくらい広く雲を明るくするかです。", "How far around the moon the cloud brightening spreads."));
                            if (cloudMoonHighlightColor != null)
                                materialEditor.ShaderProperty(cloudMoonHighlightColor, C("色", "Color", "月明かりが雲に乗る色です。青白くすると夜空になじみやすいです。", "Tint of moonlight on clouds. Pale blue-white blends naturally into night skies."));
                            if (cloudMoonHighlightAltitude != null)
                                materialEditor.ShaderProperty(cloudMoonHighlightAltitude, C("月高度反映", "Moon Altitude", "月が低い時や地平線下にある時に弱くする度合いです。", "How much the effect fades when the moon is low or below the horizon."));
                        }
                        EditorGUI.indentLevel--;
                    }
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
            GUILayout.Label(T("都市光（夜間）", "City Light (Night)"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            if (useCityLight != null)
            {
                useCityLight.floatValue = EditorGUILayout.Toggle(
                    C("表示", "Enable", "地平線付近に遠くの街明かりのような光を追加します。夜景や光害の表現に使います。", "Adds distant city-like glow near the horizon for night scenery or light pollution."),
                    useCityLight.floatValue > 0.5f) ? 1.0f : 0.0f;
                if (useCityLight.floatValue > 0.5f)
                {
                    if (cityLightColor != null)
                        materialEditor.ShaderProperty(cityLightColor, C("色", "Color", "都市光の色です。暖色にすると街明かり、白寄りにすると強い光害っぽくなります。", "Color of the city glow. Warm colors feel like city lights; whiter colors feel like stronger light pollution."));
                    if (cityLightStrength != null)
                        materialEditor.ShaderProperty(cityLightStrength, C("強度", "Strength", "都市光そのものの明るさです。星の減衰や雲反射にも影響します。", "Brightness of the city glow. Also affects star attenuation and cloud reflection."));
                    bool useDirection = cityLightUseDirection == null || cityLightUseDirection.floatValue > 0.5f;
                    if (cityLightUseDirection != null)
                    {
                        useDirection = EditorGUILayout.Toggle(
                            C("方角指定", "Use Direction", "ONでは指定方角に都市光が寄ります。OFFでは地平線全体に均一に出します。", "When on, city glow favors the chosen direction. When off, it appears evenly around the horizon."),
                            useDirection);
                        cityLightUseDirection.floatValue = useDirection ? 1.0f : 0.0f;
                    }
                    if (useDirection && cityLightDirection != null)
                        materialEditor.ShaderProperty(cityLightDirection, C("方角", "Direction", "都市光が寄る方角です。0=北、0.25=東、0.5=南、0.75=西。北方向設定に追従します。", "Direction the city glow favors. 0=N, 0.25=E, 0.5=S, 0.75=W. Follows North Direction."));
                    if (cityLightSpread != null)
                        materialEditor.ShaderProperty(cityLightSpread, C("広がり", "Spread", "地平線から上方向へどれだけぼかして広げるかです。", "How far the glow blurs upward from the horizon."));
                    if (cityLightStarOcclusion != null)
                        materialEditor.ShaderProperty(cityLightStarOcclusion, C("星の減衰", "Star Occlusion", "都市光周辺で星や天の川を見えにくくする強さです。", "How strongly city glow hides stars and the Milky Way around it."));
                    if (cityLightCloudReflection != null)
                        materialEditor.ShaderProperty(cityLightCloudReflection, C("雲への反射", "Cloud Reflection", "都市光が雲へ反射して明るくなる強さです。", "How strongly city glow reflects onto clouds."));
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        _showStars = EditorGUILayout.BeginFoldoutHeaderGroup(_showStars, T("星", "Stars"));
        if (_showStars)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            bool catalogStarsEnabled = useCatalogStars != null && useCatalogStars.floatValue > 0.5f;
            if (useCatalogStars != null)
            {
                GUILayout.Label(T("実星表", "Catalog"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                catalogStarsEnabled = EditorGUILayout.Toggle(
                    C("ヒッパルコス星表を使う", "Use Hipparcos Catalog", "星表にもとづく実座標の星を描画します。OFFでは自動生成の星になります。", "Draws real-position stars based on the Hipparcos catalog. Off uses generated stars."),
                    catalogStarsEnabled);
                useCatalogStars.floatValue = catalogStarsEnabled ? 1.0f : 0.0f;
                if (catalogStarsEnabled)
                {
                    if (catalogDataMap != null)
                        materialEditor.TexturePropertySingleLine(C("座標マップ", "Data Map", "星の方向と明るさを保存したマップです。", "Map storing star direction and brightness."), catalogDataMap);
                    if (catalogColorMap != null)
                        materialEditor.TexturePropertySingleLine(C("色指数マップ", "Color Index Map", "星表のB-V色指数から推定した星色データです。", "Star color data estimated from B-V color index."), catalogColorMap);
                    if (catalogCellMap != null)
                        materialEditor.TexturePropertySingleLine(C("セルマップ", "Cell Map", "星表の表示を軽くするための補助マップです。", "Helper map used to keep catalog star rendering light."), catalogCellMap);
                    if (catalogStarSize != null)
                        materialEditor.ShaderProperty(catalogStarSize, C("サイズ", "Size", "星粒の見た目の大きさです。", "Visual size of catalog stars."));
                    if (catalogStarSharpness != null)
                        materialEditor.ShaderProperty(catalogStarSharpness, C("シャープ", "Sharpness", "星の芯の締まり具合です。高いほど点に近くなります。", "Sharpness of star cores. Higher values make tighter points."));
                    if (catalogStarGlow != null)
                        materialEditor.ShaderProperty(catalogStarGlow, C("グロー", "Glow", "星の周囲に出る淡いにじみです。内部では最大0.3として扱います。", "Soft glow around stars. Internally capped to 0.3."));
                    if (catalogStarEmission != null)
                        materialEditor.ShaderProperty(catalogStarEmission, C("エミッション", "Emission", "HDR加算の強さです。Bloom/PPSの発光に効きます。", "HDR additive strength. Affects bloom/post-process emission."));
                    if (catalogStarBright != null)
                        materialEditor.ShaderProperty(catalogStarBright, C("輝度", "Brightness", "星表星全体の明るさです。", "Overall brightness of catalog stars."));
                    if (catalogUseLocation != null)
                    {
                        catalogUseLocation.floatValue = EditorGUILayout.Toggle(
                            C("地域同期", "Use Location", "緯度経度と恒星時を使い、その地域で見える空へ変換します。", "Uses latitude, longitude, and sidereal time to show the sky for that location."),
                            catalogUseLocation.floatValue > 0.5f) ? 1.0f : 0.0f;
                        if (catalogUseLocation.floatValue > 0.5f)
                        {
                            if (catalogLocationPreset != null)
                            {
                                int preset = Mathf.Clamp(Mathf.RoundToInt(catalogLocationPreset.floatValue), 0, _locationPresetNames.Length - 1);
                                EditorGUI.BeginChangeCheck();
                                preset = EditorGUILayout.Popup(C("地域プリセット", "Location Preset", "代表的な地域の緯度経度を設定します。Customでは手動編集できます。", "Sets latitude/longitude for common locations. Custom allows manual editing."), preset, _locationPresetNames);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    catalogLocationPreset.floatValue = preset;
                                    ApplyLocationPreset(preset, catalogLatitude, catalogLongitude);
                                }
                            }
                            bool customLocation = catalogLocationPreset == null || Mathf.RoundToInt(catalogLocationPreset.floatValue) == 0;
                            EditorGUI.BeginDisabledGroup(!customLocation);
                            if (catalogLatitude != null)
                                materialEditor.ShaderProperty(catalogLatitude, C("緯度", "Latitude", "観測地点の緯度です。星と太陽/月の高度に影響します。", "Observer latitude. Affects altitude of stars, sun, and moon."));
                            if (catalogLongitude != null)
                                materialEditor.ShaderProperty(catalogLongitude, C("経度", "Longitude", "観測地点の経度です。恒星時と星の向きに影響します。", "Observer longitude. Affects sidereal time and star orientation."));
                            EditorGUI.EndDisabledGroup();
                            if (catalogSiderealRot != null)
                                materialEditor.ShaderProperty(catalogSiderealRot, C("恒星時回転", "Sidereal Rotation", "地球の自転に対応する星空回転値です。時刻同期中は自動で更新されます。", "Starfield rotation from Earth's rotation. Updated automatically while time sync is active."));
                        }
                        else if (catalogStarRot != null)
                        {
                            materialEditor.ShaderProperty(catalogStarRot, T("水平回転", "Rotation"));
                        }
                    }
                    else if (catalogStarRot != null)
                    {
                        materialEditor.ShaderProperty(catalogStarRot, T("水平回転", "Rotation"));
                    }
                    EditorGUI.BeginDisabledGroup(true);
                    if (catalogStarCount != null)
                        materialEditor.ShaderProperty(catalogStarCount, T("星数", "Star Count"));
                    if (catalogDataWidth != null)
                        materialEditor.ShaderProperty(catalogDataWidth, T("データ幅", "Data Width"));
                    if (catalogCellLon != null)
                        materialEditor.ShaderProperty(catalogCellLon, T("セル数 横", "Cell Longitude Count"));
                    if (catalogCellLat != null)
                        materialEditor.ShaderProperty(catalogCellLat, T("セル数 縦", "Cell Latitude Count"));
                    EditorGUI.EndDisabledGroup();
                    if (useOrionLines != null)
                    {
                        EditorGUILayout.Space(6);
                        EditorGUILayout.LabelField(T("星座補助", "Constellation Helper"), EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        useOrionLines.floatValue = EditorGUILayout.Toggle(
                            C("オリオン座線", "Orion Lines", "オリオン座の主要7星だけを補助線でつなぎます。星表の座標・地域同期に追従します。", "Connects only Orion's seven major stars. Follows catalog coordinates and location sync."),
                            useOrionLines.floatValue > 0.5f) ? 1.0f : 0.0f;
                        if (useOrionLines.floatValue > 0.5f)
                        {
                            if (orionLineColor != null)
                                materialEditor.ShaderProperty(orionLineColor, C("色", "Color", "オリオン座線の色です。", "Color of Orion helper lines."));
                            if (orionLineStrength != null)
                                materialEditor.ShaderProperty(orionLineStrength, C("強度", "Strength", "オリオン座線の明るさです。", "Brightness of Orion helper lines."));
                            if (orionLineWidth != null)
                                materialEditor.ShaderProperty(orionLineWidth, C("太さ", "Width", "オリオン座線の太さです。", "Width of Orion helper lines."));
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(4);
            }
            if (!catalogStarsEnabled)
            {
                GUILayout.Label(T("基本", "Basic"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(starDensity, T("密度", "Density"));
                materialEditor.ShaderProperty(starScale,   T("スケール", "Scale"));
                materialEditor.ShaderProperty(starSeed,    T("シード", "Seed"));
                materialEditor.ShaderProperty(starBright,  T("輝度", "Brightness"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(4);
                GUILayout.Label(T("回転", "Rotation"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(starSpeed, T("速度", "Speed"));
                GUILayout.Label(T("回転軸", "Axis"), EditorStyles.miniLabel);
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(starAxisX, "X");
                materialEditor.ShaderProperty(starAxisY, "Y");
                materialEditor.ShaderProperty(starAxisZ, "Z");
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(4);
            }
            GUILayout.Label(T("フェード", "Fade"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            materialEditor.ShaderProperty(starFadeH,   T("高さ", "Height"));
            materialEditor.ShaderProperty(starFadeStr, T("強度", "Strength"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
            if (!catalogStarsEnabled && starGlowStr != null && starGlowSize != null)
            {
                GUILayout.Label(T("発光", "Glow"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(starGlowStr,  T("強度", "Strength"));
                materialEditor.ShaderProperty(starGlowSize, T("サイズ", "Size"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(4);
            }
            if (starCrossTh != null && starCrossRot != null && starCrossSize != null && starCrossLen != null && starCrossOpac != null)
            {
                GUILayout.Label(T("十字光", "Cross Flare"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(starCrossTh,   T("量", "Amount"));
                materialEditor.ShaderProperty(starCrossRot,  T("回転", "Rotation"));
                materialEditor.ShaderProperty(starCrossSize, T("太さ", "Thickness"));
                materialEditor.ShaderProperty(starCrossLen,  T("長さ", "Length"));
                materialEditor.ShaderProperty(starCrossOpac, T("透明度", "Opacity"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(4);
            }
            if (starTwinkleRange != null && starTwinkleInvert != null && starTwinkleSpeed != null && starTwinkleStrength != null)
            {
                GUILayout.Label(T("またたき", "Twinkle"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(starTwinkleRange, C("明るさの範囲", "Brightness Range", "またたきをかける星の範囲です。0で明るい星だけ、1でほぼ全部です。", "Selects which stars twinkle. 0 targets only bright stars; 1 targets almost all stars."));
                starTwinkleInvert.floatValue = EditorGUILayout.Toggle(
                    C("反転", "Invert", "ONにすると暗い星側からまたたき対象にします。0で暗い星だけ、範囲を上げると全部へ広がります。", "Targets dim stars first. At low range only dim stars twinkle; higher range expands toward all stars."),
                    starTwinkleInvert.floatValue > 0.5f) ? 1.0f : 0.0f;
                materialEditor.ShaderProperty(starTwinkleSpeed, C("点滅速度", "Blink Speed", "星のまたたき速度です。0で停止します。", "Twinkle speed. 0 disables twinkling."));
                materialEditor.ShaderProperty(starTwinkleStrength, C("点滅強さ", "Blink Strength", "またたきの明暗差です。0で揺れなし、1で強く点滅します。", "Brightness variation amount. 0 disables variation; 1 makes twinkling strong."));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(4);
            }
            if (useMeteors != null)
            {
                GUILayout.Label(T("流れ星", "Meteors"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                useMeteors.floatValue = EditorGUILayout.Toggle(
                    C("表示", "Enable", "夜空に流れ星を表示します。頻度や速度で出方を調整できます。", "Shows shooting stars in the night sky. Frequency and speed adjust how they appear."),
                    useMeteors.floatValue > 0.5f) ? 1.0f : 0.0f;
                if (useMeteors.floatValue > 0.5f)
                {
                    if (meteorFrequency != null)
                        materialEditor.ShaderProperty(meteorFrequency, C("頻度", "Frequency", "1分あたりの出現数です。0で出ません。", "Appearances per minute. 0 disables spawning."));
                    if (meteorColor != null)
                        materialEditor.ShaderProperty(meteorColor, C("色", "Color", "流れ星の色です。淡い青白にすると夜空になじみやすいです。", "Meteor color. Pale blue-white blends naturally into night skies."));
                    if (meteorIntensity != null)
                        materialEditor.ShaderProperty(meteorIntensity, C("強度", "Intensity", "流れ星のHDR加算の強さです。Bloom/PPSにも効きます。", "HDR additive brightness. Also affects bloom/post-process emission."));
                    if (meteorSpeed != null)
                        materialEditor.ShaderProperty(meteorSpeed, C("速度", "Speed", "流れ星が空を横切る速さです。持続時間とは別に移動距離を調整します。", "How fast the meteor crosses the sky. Adjusts travel distance separately from duration."));
                    if (meteorLength != null)
                        materialEditor.ShaderProperty(meteorLength, C("長さ", "Length", "尾を引く長さです。", "Trail length."));
                    if (meteorWidth != null)
                        materialEditor.ShaderProperty(meteorWidth, C("太さ", "Width", "流れ星の線幅です。", "Meteor streak width."));
                    if (meteorDuration != null)
                        materialEditor.ShaderProperty(meteorDuration, C("持続時間", "Duration", "1つの流れ星が見えている秒数です。頻度が高い場合は自動で短く制限されます。", "Visible seconds for each meteor. It is capped automatically when frequency is high."));
                    if (meteorSeed != null)
                        materialEditor.ShaderProperty(meteorSeed, C("シード", "Seed", "出現位置と方向のパターンを変えます。", "Changes spawn positions and directions."));
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(4);
            }
            if (!catalogStarsEnabled)
            {
                GUILayout.Label(T("色", "Color"), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(star1Color, T("レイヤー1", "Layer 1"));
                EditorGUILayout.PrefixLabel(T("レイヤー2", "Layer 2"));
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(star2Color,  T("色", "Color"));
                materialEditor.ShaderProperty(star2Amount, T("量", "Amount"));
                EditorGUI.indentLevel--;
                EditorGUILayout.PrefixLabel(T("レイヤー3", "Layer 3"));
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(star3Color,  T("色", "Color"));
                materialEditor.ShaderProperty(star3Amount, T("量", "Amount"));
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        _showMilkyWay = EditorGUILayout.BeginFoldoutHeaderGroup(_showMilkyWay, T("天の川", "Milky Way"));
        if (_showMilkyWay)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            if (useMilkyWay != null)
            {
                useMilkyWay.floatValue = EditorGUILayout.Toggle(
                    C("表示", "Enable", "天の川の淡い帯を表示します。", "Shows the Milky Way band."),
                    useMilkyWay.floatValue > 0.5f) ? 1.0f : 0.0f;
                if (useMilkyWay.floatValue > 0.5f)
                {
                    if (useMilkyWayDensityMap != null)
                    {
                        useMilkyWayDensityMap.floatValue = EditorGUILayout.Toggle(
                            C("星表密度マップ", "Catalog Density Map", "7〜12等星の分布から生成した密度マップを使います。", "Uses a density map generated from 7th to 12th magnitude catalog stars."),
                            useMilkyWayDensityMap.floatValue > 0.5f) ? 1.0f : 0.0f;
                        if (useMilkyWayDensityMap.floatValue > 0.5f && milkyWayDensityMap != null)
                            materialEditor.TexturePropertySingleLine(C("密度マップ", "Density Map", "天の川の濃さと色を決めるマップです。", "Map controlling Milky Way density and color."), milkyWayDensityMap);
                        if (useMilkyWayDensityMap.floatValue > 0.5f && useMilkyWayDustMap != null)
                        {
                            useMilkyWayDustMap.floatValue = EditorGUILayout.Toggle(
                                C("ダスト暗黒帯", "Dust Lanes", "CSFDダストマップで暗黒帯を作ります。", "Uses the CSFD dust map to create dark lanes."),
                                useMilkyWayDustMap.floatValue > 0.5f) ? 1.0f : 0.0f;
                            if (useMilkyWayDustMap.floatValue > 0.5f)
                            {
                                if (milkyWayDustMap != null)
                                    materialEditor.TexturePropertySingleLine(C("ダストマップ", "Dust Map", "星間塵による暗黒帯の形を決めるマップです。", "Map controlling the shape of dark dust lanes."), milkyWayDustMap);
                                if (milkyWayDustStrength != null)
                                    materialEditor.ShaderProperty(milkyWayDustStrength, C("暗さ", "Darkness", "ダスト暗黒帯で天の川を暗くする強さです。", "How strongly dust lanes darken the Milky Way."));
                            }
                        }
                    }
                    if (milkyWayStrength != null)
                        materialEditor.ShaderProperty(milkyWayStrength, C("強度", "Strength", "天の川全体の明るさです。", "Overall Milky Way brightness."));
                    if (milkyWayWidth != null && (useMilkyWayDensityMap == null || useMilkyWayDensityMap.floatValue < 0.5f))
                        materialEditor.ShaderProperty(milkyWayWidth, C("幅", "Width", "密度マップを使わない時の天の川帯の幅です。", "Width of the Milky Way band when the density map is not used."));
                    if (milkyWayDetail != null)
                        materialEditor.ShaderProperty(milkyWayDetail, C("濃淡", "Detail", "天の川の密度差をどれだけ強調するかです。", "Emphasizes density variation in the Milky Way."));
                    if (milkyWaySaturation != null)
                        materialEditor.ShaderProperty(milkyWaySaturation, C("彩度", "Saturation", "天の川色の彩度です。0でグレー寄り、上げるほど色が強くなります。", "Milky Way color saturation. 0 is grayish; higher values increase color."));
                    if (milkyWayCyanBoost != null)
                        materialEditor.ShaderProperty(milkyWayCyanBoost, C("青/シアン", "Blue/Cyan", "黄色味を抑え、青〜シアン成分を少し持ち上げます。", "Reduces yellow cast and boosts blue/cyan components."));
                    if (milkyWayTintAmount != null)
                        materialEditor.ShaderProperty(milkyWayTintAmount, C("色寄せ", "Tint Amount", "星表由来色を下の色へ寄せる量です。", "Amount to tint catalog-derived colors toward the color below."));
                    if (milkyWayCoreWarmth != null)
                        materialEditor.ShaderProperty(milkyWayCoreWarmth, C("中心暖色", "Core Warmth", "濃い部分だけピンク〜暖色へ寄せます。写真風の銀河中心表現に使います。", "Warms dense regions toward pink for a photographic galactic-core look."));
                    if (milkyWayColor != null)
                        materialEditor.ShaderProperty(milkyWayColor, C("色", "Color", "色寄せの目標色です。", "Target tint color."));
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(8);
        materialEditor.RenderQueueField();
    }

    private void ApplyLocationPreset(int preset, MaterialProperty latitude, MaterialProperty longitude)
    {
        if (latitude == null || longitude == null || preset == 0)
            return;

        if (preset == 1) { latitude.floatValue = 35.6895f; longitude.floatValue = 139.6917f; }
        else if (preset == 2) { latitude.floatValue = 43.0618f; longitude.floatValue = 141.3545f; }
        else if (preset == 3) { latitude.floatValue = 26.2124f; longitude.floatValue = 127.6792f; }
        else if (preset == 4) { latitude.floatValue = 51.5072f; longitude.floatValue = -0.1276f; }
        else if (preset == 5) { latitude.floatValue = 48.8566f; longitude.floatValue = 2.3522f; }
        else if (preset == 6) { latitude.floatValue = 40.7128f; longitude.floatValue = -74.0060f; }
        else if (preset == 7) { latitude.floatValue = 34.0522f; longitude.floatValue = -118.2437f; }
        else if (preset == 8) { latitude.floatValue = -33.8688f; longitude.floatValue = 151.2093f; }
    }
}
