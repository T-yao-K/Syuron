using UnityEditor;
using UnityEngine;
using UdonSharpEditor;

[CustomEditor(typeof(AetherTime))]
public class AetherTimeEditor : Editor
{
    private SerializedProperty _materials;
    private SerializedProperty _controllerId;
    private SerializedProperty _lightDefaultsInitialized;
    private SerializedProperty _timeZone;
    private SerializedProperty _timeOffsetSeconds;
    private SerializedProperty _autoRotate;
    private SerializedProperty _autoSpeed;
    private SerializedProperty _syncDirectionalLights;
    private SerializedProperty _sunDirectionalLight;
    private SerializedProperty _moonDirectionalLight;
    private SerializedProperty _syncSunLight;
    private SerializedProperty _syncMoonLight;
    private SerializedProperty _sunLightIntensity;
    private SerializedProperty _moonLightIntensity;
    private SerializedProperty _syncLightColor;
    private SerializedProperty _sunLightTint;
    private SerializedProperty _moonLightTint;
    private SerializedProperty _enableRealtimeShadows;
    private SerializedProperty _sunShadowStrength;
    private SerializedProperty _moonShadowStrength;
    private SerializedProperty _lightFadeStartAltitude;
    private SerializedProperty _lightDisableAltitude;
    private SerializedProperty _overrideTime;
    private SerializedProperty _overrideValue;
    private SerializedProperty _overrideDateTime;
    private SerializedProperty _overrideYear;
    private SerializedProperty _overrideMonth;
    private SerializedProperty _overrideDay;
    private SerializedProperty _overrideHour;
    private SerializedProperty _overrideMinute;

    private bool _showOverride = true;
    private bool _showLights = true;
    private int _datePreset = 0;
    private bool _repaintQueued = false;

    private static readonly string[] DatePresetNames =
    {
        "None",
        "Now",
        "Spring Equinox 21:00",
        "Summer Solstice 21:00",
        "Autumn Equinox 21:00",
        "Winter Solstice 21:00",
        "Orion Season 22:00",
        "Milky Way Season 23:00",
        "Midnight Test",
        "Sunrise Test",
        "Sunset Test",
    };

    private void OnEnable()
    {
        CacheProperties();
        QueueRepaint();
    }

    private void OnDisable()
    {
        EditorApplication.delayCall -= DelayedRepaint;
    }

    private void DelayedRepaint()
    {
        _repaintQueued = false;
        if (this != null)
            Repaint();
    }

    private void QueueRepaint()
    {
        if (_repaintQueued)
            return;

        _repaintQueued = true;
        EditorApplication.delayCall += DelayedRepaint;
    }

    private void CacheProperties()
    {
        _materials = serializedObject.FindProperty("_materials");
        _controllerId = serializedObject.FindProperty("_controllerId");
        _lightDefaultsInitialized = serializedObject.FindProperty("_lightDefaultsInitialized");
        _timeZone = serializedObject.FindProperty("_timeZone");
        _timeOffsetSeconds = serializedObject.FindProperty("_timeOffsetSeconds");
        _autoRotate = serializedObject.FindProperty("_autoRotate");
        _autoSpeed = serializedObject.FindProperty("_autoSpeed");
        _syncDirectionalLights = serializedObject.FindProperty("_syncDirectionalLights");
        _sunDirectionalLight = serializedObject.FindProperty("_sunDirectionalLight");
        _moonDirectionalLight = serializedObject.FindProperty("_moonDirectionalLight");
        _syncSunLight = serializedObject.FindProperty("_syncSunLight");
        _syncMoonLight = serializedObject.FindProperty("_syncMoonLight");
        _sunLightIntensity = serializedObject.FindProperty("_sunLightIntensity");
        _moonLightIntensity = serializedObject.FindProperty("_moonLightIntensity");
        _syncLightColor = serializedObject.FindProperty("_syncLightColor");
        _sunLightTint = serializedObject.FindProperty("_sunLightTint");
        _moonLightTint = serializedObject.FindProperty("_moonLightTint");
        _enableRealtimeShadows = serializedObject.FindProperty("_enableRealtimeShadows");
        _sunShadowStrength = serializedObject.FindProperty("_sunShadowStrength");
        _moonShadowStrength = serializedObject.FindProperty("_moonShadowStrength");
        _lightFadeStartAltitude = serializedObject.FindProperty("_lightFadeStartAltitude");
        _lightDisableAltitude = serializedObject.FindProperty("_lightDisableAltitude");
        _overrideTime = serializedObject.FindProperty("_overrideTime");
        _overrideValue = serializedObject.FindProperty("_overrideValue");
        _overrideDateTime = serializedObject.FindProperty("_overrideDateTime");
        _overrideYear = serializedObject.FindProperty("_overrideYear");
        _overrideMonth = serializedObject.FindProperty("_overrideMonth");
        _overrideDay = serializedObject.FindProperty("_overrideDay");
        _overrideHour = serializedObject.FindProperty("_overrideHour");
        _overrideMinute = serializedObject.FindProperty("_overrideMinute");
    }

    public override void OnInspectorGUI()
    {
        AetherTime proxy = target as AetherTime;
        if (proxy == null)
            return;

        if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(proxy))
            return;

        if (!EnsureProperties())
        {
            EditorGUILayout.HelpBox("AetherTime inspector is refreshing. If this stays empty, reselect the object.", MessageType.Info);
            DrawDefaultInspector();
            QueueRepaint();
            return;
        }

        serializedObject.UpdateIfRequiredOrScript();
        EnsureControllerId();
        EnsureLightDefaults();
        DrawSceneGuards();

        EditorGUILayout.PropertyField(_materials, new GUIContent(
            "Materials",
            "Aetherの空マテリアルを1つだけ登録します。空以外のマテリアルはAetherTimeでは制御しません。"));
        DrawMaterialValidation();

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Time", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(_timeZone, new GUIContent(
            "Time Zone",
            "実時刻同期に使うタイムゾーンです。JST_Tokyoなら日本時間基準になります。"));
        EditorGUILayout.PropertyField(_timeOffsetSeconds, new GUIContent(
            "Time Offset Seconds",
            "同期時刻を秒単位で前後にずらします。3600で1時間進みます。"));
        EditorGUILayout.PropertyField(_autoRotate, new GUIContent(
            "Auto Rotate",
            "実時刻ではなく、指定速度で空の時刻を自動進行させます。"));
        if (_autoRotate.boolValue)
        {
            EditorGUILayout.PropertyField(_autoSpeed, new GUIContent(
                "Auto Speed",
                "Auto Rotate有効時の進行速度です。1440で現実の1分が空の1日になります。"));
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Space(6);
        _showLights = EditorGUILayout.BeginFoldoutHeaderGroup(_showLights, "Directional Lights");
        if (_showLights)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_syncDirectionalLights, new GUIContent(
                "Enable",
                "Aetherの太陽・月にDirectional Lightを連動します。空に登録したAetherマテリアルだけを基準にします。"));
            if (_syncDirectionalLights.boolValue)
            {
                EditorGUILayout.PropertyField(_syncSunLight, new GUIContent(
                    "Sun Light",
                    "太陽ライトを連動します。"));
                EditorGUI.BeginDisabledGroup(!_syncSunLight.boolValue);
                EditorGUILayout.PropertyField(_sunDirectionalLight, new GUIContent(
                    "Directional Light",
                    "太陽に連動させるDirectional Lightです。"));
                EditorGUILayout.Slider(_sunLightIntensity, 0f, 8f, new GUIContent(
                    "Intensity",
                    "太陽ライトの最大強度です。水平線下ではフェードアウトします。"));
                EditorGUILayout.PropertyField(_sunLightTint, new GUIContent(
                    "Color Tint",
                    "Aetherの太陽色に掛ける補正色です。白でそのまま反映します。"));
                EditorGUILayout.Slider(_sunShadowStrength, 0f, 1f, new GUIContent(
                    "Shadow Strength",
                    "太陽ライトの影の強さです。Realtime ShadowsがOFFなら0になります。"));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(_syncMoonLight, new GUIContent(
                    "Moon Light",
                    "月ライトを連動します。夜かつ月が見える時だけ有効になります。"));
                EditorGUI.BeginDisabledGroup(!_syncMoonLight.boolValue);
                EditorGUILayout.PropertyField(_moonDirectionalLight, new GUIContent(
                    "Directional Light",
                    "月に連動させるDirectional Lightです。"));
                EditorGUILayout.Slider(_moonLightIntensity, 0f, 2f, new GUIContent(
                    "Intensity",
                    "月ライトの最大強度です。月齢と夜の暗さでも弱くなります。"));
                EditorGUILayout.PropertyField(_moonLightTint, new GUIContent(
                    "Color Tint",
                    "Aetherの月色に掛ける補正色です。白でそのまま反映します。"));
                EditorGUILayout.Slider(_moonShadowStrength, 0f, 1f, new GUIContent(
                    "Shadow Strength",
                    "月ライトの影の強さです。Realtime ShadowsがOFFなら0になります。"));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(_syncLightColor, new GUIContent(
                    "Color Sync",
                    "ONではAetherの太陽色・月色をDirectional Lightへ反映します。"));
                EditorGUILayout.PropertyField(_enableRealtimeShadows, new GUIContent(
                    "Realtime Shadows",
                    "ONでは割り当てたLightのRealtime Shadowsを使います。重い場合はOFFにしてください。"));
                EditorGUILayout.Slider(_lightFadeStartAltitude, -0.1f, 0.2f, new GUIContent(
                    "Fade Start",
                    "ライトがフェードを始める高度です。0が水平線です。"));
                EditorGUILayout.Slider(_lightDisableAltitude, -0.3f, 0.05f, new GUIContent(
                    "Disable Below",
                    "ライトを完全に無効化する高度です。水平線より少し下にします。"));
                DrawLightValidation();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(6);
        _showOverride = EditorGUILayout.BeginFoldoutHeaderGroup(_showOverride, "Override");
        if (_showOverride)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_overrideTime, new GUIContent(
                "Manual Time",
                "ONにすると同期時刻を手動スライダーで固定します。空の時刻・月位置・恒星時の確認に使います。"));
            EditorGUI.BeginDisabledGroup(!_overrideTime.boolValue);
            float time = _overrideValue.floatValue;
            string timeLabel = FormatTime(time);
            _overrideValue.floatValue = EditorGUILayout.Slider(new GUIContent(
                $"Test Time [{timeLabel}]",
                "手動同期時刻です。0.5が太陽の南中になる太陽時として扱います。月位置同期には、観測地点の経度・タイムゾーン・均時差から逆算した時計時刻が反映されます。"), time, 0f, 1f);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(_overrideDateTime, new GUIContent(
                "Override Date Time",
                "ONにすると天文同期用の日付時刻を手動指定します。月齢・月位置・恒星時・季節同期の検証に使います。"));
            EditorGUILayout.BeginHorizontal();
            _datePreset = EditorGUILayout.Popup(new GUIContent(
                "Date Preset",
                "選択中のTime Zoneにおける現地の観測日時として入力します。星の見える地域はマテリアル側の緯度/経度を使います。"), _datePreset, DatePresetNames);
            if (GUILayout.Button(new GUIContent("Apply", "選択した観測日時をOverride Date Timeへ反映します。"), GUILayout.Width(64)))
            {
                ApplyDatePreset(_datePreset);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(!_overrideDateTime.boolValue);
            EditorGUILayout.PropertyField(_overrideYear, new GUIContent("Year", "天文同期テスト用の年です。"));
            EditorGUILayout.IntSlider(_overrideMonth, 1, 12, new GUIContent("Month", "天文同期テスト用の月です。"));
            EditorGUILayout.IntSlider(_overrideDay, 1, 31, new GUIContent("Day", "天文同期テスト用の日です。存在しない日は実行時に月末へ丸められます。"));
            EditorGUILayout.IntSlider(_overrideHour, 0, 23, new GUIContent("Hour", "天文同期テスト用の時です。"));
            EditorGUILayout.IntSlider(_overrideMinute, 0, 59, new GUIContent("Minute", "天文同期テスト用の分です。"));
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (serializedObject.ApplyModifiedProperties())
        {
            ApplyEditorShadowMode();
            if (proxy != null)
                UdonSharpEditorUtility.CopyProxyToUdon(proxy, ProxySerializationPolicy.All);
        }
    }

    private bool EnsureProperties()
    {
        if (_materials == null
            || _controllerId == null
            || _lightDefaultsInitialized == null
            || _timeZone == null
            || _timeOffsetSeconds == null
            || _autoRotate == null
            || _autoSpeed == null
            || _syncDirectionalLights == null
            || _sunDirectionalLight == null
            || _moonDirectionalLight == null
            || _syncSunLight == null
            || _syncMoonLight == null
            || _sunLightIntensity == null
            || _moonLightIntensity == null
            || _syncLightColor == null
            || _sunLightTint == null
            || _moonLightTint == null
            || _enableRealtimeShadows == null
            || _sunShadowStrength == null
            || _moonShadowStrength == null
            || _lightFadeStartAltitude == null
            || _lightDisableAltitude == null
            || _overrideTime == null
            || _overrideValue == null
            || _overrideDateTime == null
            || _overrideYear == null
            || _overrideMonth == null
            || _overrideDay == null
            || _overrideHour == null
            || _overrideMinute == null)
        {
            CacheProperties();
        }

        return _materials != null
            && _controllerId != null
            && _lightDefaultsInitialized != null
            && _timeZone != null
            && _timeOffsetSeconds != null
            && _autoRotate != null
            && _autoSpeed != null
            && _syncDirectionalLights != null
            && _sunDirectionalLight != null
            && _moonDirectionalLight != null
            && _syncSunLight != null
            && _syncMoonLight != null
            && _sunLightIntensity != null
            && _moonLightIntensity != null
            && _syncLightColor != null
            && _sunLightTint != null
            && _moonLightTint != null
            && _enableRealtimeShadows != null
            && _sunShadowStrength != null
            && _moonShadowStrength != null
            && _lightFadeStartAltitude != null
            && _lightDisableAltitude != null
            && _overrideTime != null
            && _overrideValue != null
            && _overrideDateTime != null
            && _overrideYear != null
            && _overrideMonth != null
            && _overrideDay != null
            && _overrideHour != null
            && _overrideMinute != null;
    }

    private void EnsureControllerId()
    {
        if (_controllerId == null || _controllerId.intValue > 0)
            return;

        _controllerId.intValue = Random.Range(100000, 999999999);
    }

    private void EnsureLightDefaults()
    {
        if (_lightDefaultsInitialized == null || _lightDefaultsInitialized.boolValue)
            return;

        _lightDefaultsInitialized.boolValue = true;
        if (_syncLightColor != null)
            _syncLightColor.boolValue = true;
        if (_sunLightTint != null && IsDefaultBlack(_sunLightTint.colorValue))
            _sunLightTint.colorValue = Color.white;
        if (_moonLightTint != null && IsDefaultBlack(_moonLightTint.colorValue))
            _moonLightTint.colorValue = Color.white;
    }

    private static bool IsDefaultBlack(Color color)
    {
        return color.r <= 0.0001f
            && color.g <= 0.0001f
            && color.b <= 0.0001f
            && color.a <= 0.0001f;
    }

    private void DrawSceneGuards()
    {
        AetherTime[] controllers = Resources.FindObjectsOfTypeAll<AetherTime>();
        int sceneCount = 0;
        if (controllers != null)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] != null && !EditorUtility.IsPersistent(controllers[i]))
                    sceneCount++;
            }
        }

        if (sceneCount > 1)
        {
            EditorGUILayout.HelpBox(
                "AetherTimeは1シーンに1つだけ配置してください。複数あると時刻・月位置・ライト制御が競合します。",
                MessageType.Error);
        }
    }

    private void DrawMaterialValidation()
    {
        int aetherCount = CountAetherMaterials();
        if (aetherCount == 0)
        {
            EditorGUILayout.HelpBox(
                "MaterialsにはAetherの空マテリアルを1つ登録してください。",
                MessageType.Warning);
        }
        else if (aetherCount > 1)
        {
            EditorGUILayout.HelpBox(
                "AetherTimeは複数のAether空マテリアルを許容しません。Materials内のAetherマテリアルは1つだけにしてください。",
                MessageType.Error);
        }

        if (CountNonAetherMaterials() > 0)
        {
            EditorGUILayout.HelpBox(
                "AetherTimeは空のAetherマテリアルだけを制御します。窓など空以外の連動にはWindowTimeを使ってください。",
                MessageType.Info);
        }
    }

    private int CountAetherMaterials()
    {
        if (_materials == null)
            return 0;

        int count = 0;
        for (int i = 0; i < _materials.arraySize; i++)
        {
            Material material = _materials.GetArrayElementAtIndex(i).objectReferenceValue as Material;
            if (IsAetherMaterial(material))
                count++;
        }
        return count;
    }

    private int CountNonAetherMaterials()
    {
        if (_materials == null)
            return 0;

        int count = 0;
        for (int i = 0; i < _materials.arraySize; i++)
        {
            Material material = _materials.GetArrayElementAtIndex(i).objectReferenceValue as Material;
            if (material != null && !IsAetherMaterial(material))
                count++;
        }
        return count;
    }

    private static bool IsAetherMaterial(Material material)
    {
        return material != null
            && material.shader != null
            && material.shader.name == "ACM/Aether";
    }

    private void DrawLightValidation()
    {
        Light sun = _sunDirectionalLight.objectReferenceValue as Light;
        Light moon = _moonDirectionalLight.objectReferenceValue as Light;

        if (_syncSunLight.boolValue && sun == null)
            EditorGUILayout.HelpBox("太陽ライトを連動する場合はDirectional Lightを割り当ててください。", MessageType.Warning);
        if (_syncMoonLight.boolValue && moon == null)
            EditorGUILayout.HelpBox("月ライトを連動する場合はDirectional Lightを割り当ててください。", MessageType.Warning);

        if (sun != null && sun.type != LightType.Directional)
            EditorGUILayout.HelpBox("太陽ライトにはDirectional Lightを割り当ててください。", MessageType.Error);
        if (moon != null && moon.type != LightType.Directional)
            EditorGUILayout.HelpBox("月ライトにはDirectional Lightを割り当ててください。", MessageType.Error);

        if (_lightDisableAltitude.floatValue >= _lightFadeStartAltitude.floatValue)
            EditorGUILayout.HelpBox("Disable BelowはFade Startより低い値にしてください。", MessageType.Warning);
    }

    private void ApplyEditorShadowMode()
    {
        if (_syncDirectionalLights == null || _enableRealtimeShadows == null || !_syncDirectionalLights.boolValue)
            return;

        ApplyEditorShadowMode(_sunDirectionalLight != null ? _sunDirectionalLight.objectReferenceValue as Light : null);
        ApplyEditorShadowMode(_moonDirectionalLight != null ? _moonDirectionalLight.objectReferenceValue as Light : null);
    }

    private void ApplyEditorShadowMode(Light light)
    {
        if (light == null)
            return;

        Undo.RecordObject(light, "Aether Light Shadow Mode");
        light.shadows = _enableRealtimeShadows.boolValue ? LightShadows.Soft : LightShadows.None;
        EditorUtility.SetDirty(light);
    }

    private static string FormatTime(float value)
    {
        float day = Mathf.Repeat(value, 1f) * 24f;
        int hour = Mathf.FloorToInt(day);
        int minute = Mathf.FloorToInt((day - hour) * 60f);
        return $"{hour:00}:{minute:00}";
    }

    private void ApplyDatePreset(int preset)
    {
        if (preset <= 0)
            return;

        int year = Mathf.Clamp(_overrideYear.intValue, 1900, 2100);
        if (preset == 1)
        {
            SetOverrideDateTime(GetCurrentTimeInSelectedZone());
            return;
        }

        _overrideDateTime.boolValue = true;
        _overrideTime.boolValue = false;

        if (preset == 2) SetOverrideDateTime(year, 3, 20, 21, 0);
        else if (preset == 3) SetOverrideDateTime(year, 6, 21, 21, 0);
        else if (preset == 4) SetOverrideDateTime(year, 9, 23, 21, 0);
        else if (preset == 5) SetOverrideDateTime(year, 12, 22, 21, 0);
        else if (preset == 6) SetOverrideDateTime(year, 1, 15, 22, 0);
        else if (preset == 7) SetOverrideDateTime(year, 7, 15, 23, 0);
        else if (preset == 8) SetOverrideDateTime(year, _overrideMonth.intValue, _overrideDay.intValue, 0, 0);
        else if (preset == 9) SetOverrideDateTime(year, _overrideMonth.intValue, _overrideDay.intValue, 5, 0);
        else if (preset == 10) SetOverrideDateTime(year, _overrideMonth.intValue, _overrideDay.intValue, 18, 0);
    }

    private System.DateTime GetCurrentTimeInSelectedZone()
    {
        return System.DateTime.UtcNow.AddSeconds(GetSelectedZoneOffsetSeconds());
    }

    private float GetSelectedZoneOffsetSeconds()
    {
        AetherTimeZonePreset zone = (AetherTimeZonePreset)_timeZone.enumValueIndex;
        if (zone == AetherTimeZonePreset.PST_Los_Angeles)  return -28800f;
        if (zone == AetherTimeZonePreset.MST_Denver)       return -25200f;
        if (zone == AetherTimeZonePreset.CST_Chicago)      return -21600f;
        if (zone == AetherTimeZonePreset.EST_New_York)     return -18000f;
        if (zone == AetherTimeZonePreset.BRT_Sao_Paulo)    return -10800f;
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

    private void SetOverrideDateTime(System.DateTime dateTime)
    {
        SetOverrideDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute);
    }

    private void SetOverrideDateTime(int year, int month, int day, int hour, int minute)
    {
        _overrideDateTime.boolValue = true;
        _overrideYear.intValue = Mathf.Clamp(year, 1900, 2100);
        _overrideMonth.intValue = Mathf.Clamp(month, 1, 12);
        _overrideDay.intValue = Mathf.Clamp(day, 1, 31);
        _overrideHour.intValue = Mathf.Clamp(hour, 0, 23);
        _overrideMinute.intValue = Mathf.Clamp(minute, 0, 59);
    }
}
