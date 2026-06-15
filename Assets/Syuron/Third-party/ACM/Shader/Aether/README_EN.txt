Aether - All-Time Sky Shader
Last updated: 2026-05-11


Overview

Aether is a skybox shader for VRChat that syncs with time and location.
It can render morning, daytime, evening, and night skies with a single material, and its starry sky is drawn procedurally from star catalog data rather than a background texture.

Supported environments:
- Unity Built-in Render Pipeline
- VRChat SDK Worlds 3.10.2 or later
- UdonSharp

Not supported:
- Quest
- URP / HDRP


Package Contents

- Shader
- Material
- Time synchronization controller
- Map data for the real star catalog, Milky Way, and dust display

If you use the real star catalog, Milky Way, or dust display features, do not move or delete the included data files.


Quick Start

1. Prepare an Aether material

Use the included ACM_Aether.mat, or create a new material and set its Shader to ACM/Aether.

2. Assign it as the skybox

Unity top menu:
Window > Rendering > Lighting > Environment
Set the Aether material as the Skybox Material.

You can also drag and drop it directly onto the sky.

3. Place the TimeController

Place TimeController.prefab in the scene.
Register the Aether material used for the sky in the TimeController's Materials list.

If you want to synchronize sunlight and moonlight with Directional Lights, create two Directional Lights in the hierarchy and assign them to the controller. Their intensity can be set to 0.

4. Check in Play mode

When you enter Play mode, AetherTime updates the sky based on the current time.
By default, it is set to Tokyo time and sky settings.




About Time Control

Aether can control time either directly on the material or centrally through AetherTime.

1. Using the material by itself

From Time / Direction in the material Inspector, you can directly adjust the sky time.
Use this when you want to quickly check the appearance without scripts.

2. Managing time with AetherTime

With TimeController / AetherTime, you can update time, season, moon phase, moon position, and sidereal time together.
For materials managed by AetherTime during Play mode, the material Inspector's time, auto rotation, and speed controls are shown as disabled.

Use the AetherTime settings when you want real-time synchronization or want to test a specific observation date and time.


AetherTime Settings

Materials
Register only one Aether sky material.
AetherTime does not control materials other than the sky material.

Time Zone
Select the time zone used for real-time synchronization.
JST_Tokyo uses Japan time as the reference.
The sun position is corrected to local apparent solar time from the material longitude, Time Zone, and equation of time.
Because of this, time 0.50 is treated as solar noon, not clock time 12:00.

Time Offset Seconds
Offsets the synchronized time forward or backward in seconds.
3600 advances the time by one hour.
This is also useful for checking the sun, moon phase, moon position, and sidereal time.

Auto Rotate
Advances the sky time automatically at a specified speed instead of using real time.
Use this to quickly check day-night cycles or moon movement.
When driven by AetherTime, the moon and starry sky update with the same flow of time as the sun.
A speed of 0 fixes the observation date and time.

Auto Speed
The progression speed when Auto Rotate is enabled.
1 equals real time, while 1440 makes one real minute equal one sky day.


Override

The Override section in the AetherTime Inspector contains time controls for checking the sky.

Manual Time
When enabled, fixes the time with the Test Time slider.
Use this to check the sky time, moon position, and sidereal time.

Test Time
This is treated as solar time.
0.00 = solar midnight
0.25 = sunrise side
0.50 = solar noon
0.75 = sunset side
1.00 = returns to 0.00
For moon position and sidereal time, Aether uses a clock time calculated back from the observation longitude, Time Zone, and equation of time.

Override Date Time
When enabled, lets you manually specify the date and time for astronomical synchronization.
You can check moon phase, moon position, sidereal time, and seasonal synchronization together.

Date Preset
Presets for the observation date and time.
Test presets include the spring equinox, summer solstice, autumn equinox, winter solstice, Orion check, Milky Way check, midnight, sunrise, and sunset.

Note:
Date Preset values are entered as local time in the selected Time Zone.
The visible starry-sky region uses the material's latitude / longitude settings.


Directional Lights

AetherTime can move the sun light and moon light to match their positions in the sky.

Enable
When enabled, aligns the assigned Directional Lights with the sun / moon positions of the registered Aether sky material.

Sun Light
The Directional Light linked to the sun.
When the sun sinks below the horizon, the light gradually becomes weaker, and once it is far enough below the horizon, the Light is disabled.
Its color is based on Aether's sun color and also follows sunrise / sunset colors.
When the sun and moon overlap, the sun light becomes darker according to the eclipsed area.
Solar eclipses can occur to some extent, but accurate annular eclipses and similar phenomena cannot be reproduced.

Moon Light
The Directional Light linked to the moon.
When the moon sinks below the horizon, the light gradually becomes weaker, and once it is far enough below the horizon, the Light is disabled.
The moon light also becomes weaker according to the moon phase and night darkness.
Its color is based on Aether's moon color.

Color Sync
When enabled, applies Aether's sun color / moon color to the Directional Lights.
You can adjust the result with the Sun Light / Moon Light Color Tint.
When using an existing TimeController, Color Sync is initialized to ON and Color Tint to white the first time it is displayed.

Realtime Shadows
When enabled, uses Realtime Shadows on the assigned Lights.
Turn this off if the performance cost is too high.

Notes:
AetherTime uses only the single Aether material used for the sky as its reference.
It does not link lights to materials other than the sky material.
Do not place multiple AetherTime objects in one scene.
Do not register multiple Aether materials in AetherTime's Materials list.


Material Inspector

The Aether material Inspector is divided into the following main categories.


[Time / Direction]

Time
The sky time treated as solar time.
At 0.50, the sun reaches solar noon.
When managed by AetherTime, this is updated by the script.

Auto Rotate
Automatically advances time on the material by itself.
When using AetherTime, use Auto Rotate on the AetherTime side.

Speed
The auto-rotation speed for the material by itself.

Horizontal Rotation
Rotates the entire sky horizontally.
Use this to fine-tune direction alignment.

North Direction
The horizontal axis treated as north in the world.
This becomes the direction reference for the sun, moon, and stars.


[Sun / Moon]

Sun:

Intensity
The brightness of the sunlight and daytime sky.

Size
The apparent size of the sun disk.

Glare
The HDR emission intensity of only the sun disk.
Adjust this when you want the sun to glow with Bloom / PPS.

Soft Edge
The softness of the edge of the sun disk.

Solar Eclipse
When the apparent positions of the sun and moon overlap, the moon disk cuts into the sun.
The sun glow, direct scattering in the daytime sky, and sun Directional Light also weaken according to the eclipsed amount.
This is not an eclipse prediction feature, but a visual effect based on the sun / moon positions and sizes inside Aether.

Sunrise Color / Sunset Color
Colors used when the sun is low.

Low Altitude Color
Controls how strongly the sky shifts toward the sunrise / sunset colors when the sun is near the horizon.

Season Sync
Uses solar declination to reflect the seasonal change in sun altitude.
When managed by AetherTime, this is updated automatically according to the date.

Moon:

Phase
Configures moon phase synchronization, moon phase, and moon size.
Moon phase 0 is new moon, and 1 is full moon.

Position
When real-time synchronization is enabled, the moon position is approximated from the date.
You can also set right ascension / declination manually.

Color
Configures the moon body, halo, and halo intensity.

Nearby Stars
Controls how much stars are suppressed around the moon disk and halo.


[Atmosphere]

Scattering:

Rayleigh
The scattering feel of the blue sky and sunset.

Mie
The whitish haze around the sun.

Exposure
The overall brightness of the sky.
This also affects the HDR feel of stars and the Milky Way.

Clouds:

Display
Displays clouds.

Amount
The amount of clouds.

Opacity
The density of the clouds.
This also affects how stars and the Milky Way are hidden.

Scale
The size of the cloud pattern.

Blur
The softness of the cloud boundaries.

Height
The sky height where clouds begin to appear.

Flow Speed
The speed at which clouds move.
Negative values move them in the opposite direction.

Thickness
Strengthens cloud shading.

Layer Depth
Adds depth by layering multiple cloud layers.

Distance Blend
Reduces fine detail toward the horizon, making clouds look more like distant cloud layers.

Distance Sink
Makes distant clouds appear to sink slightly toward the horizon.

Star Attenuation
The strength of hiding stars and the Milky Way behind clouds.

Sunset Color / Sunset Influence
The color applied to evening clouds and its strength.

Moon Cloud Highlight:

Display
Softly brightens clouds in the moon direction at night only.

Intensity
The strength of cloud brightening from moonlight.

Range
How widely clouds are brightened around the moon.

Color
The color of moonlight applied to clouds.

Moon Altitude Influence
How much the effect weakens when the moon is low or below the horizon.

City Lights (Night):

Display
Adds light near the horizon that resembles distant city lights.

Color
The color of the city lights.

Intensity
The brightness of the city lights themselves.

Directional
When ON, city lights gather toward a specified direction.
When OFF, they appear evenly around the whole horizon.

Direction
The direction where city lights gather.
0 = north, 0.25 = east, 0.5 = south, 0.75 = west.
This follows the North Direction setting.

Spread
How far upward from the horizon the lights are blurred and spread.

Star Attenuation
The strength of making stars and the Milky Way less visible around city lights.

Cloud Reflection
The strength of city lights reflecting onto and brightening clouds.


[Stars]

Real Star Catalog:

Use Hipparcos Star Catalog
When ON, draws stars at real coordinates based on the star catalog.
When OFF, stars are procedurally generated from Voronoi patterns. This is the same processing used by the currently sold starry-sky shader SS-515.

Coordinate Map
A map storing star directions and brightness.

Color Index Map
Star color data estimated from the B-V color index.

Cell Map
An auxiliary map used to reduce the cost of star catalog display.

Size
The apparent size of star points.

Sharpness
Controls how crisp the stars are.

Glow
A faint blur around stars.

Emission
The strength of HDR additive emission.
This affects Bloom glow.

Brightness
The brightness of all star catalog stars.

Location Sync
Uses latitude, longitude, and sidereal time to transform the sky into the view visible from that region.

Location Preset
Sets latitude / longitude for representative regions.
Custom allows manual editing.

Latitude / Longitude
The latitude / longitude of the observation point.

Sidereal Rotation
The starry-sky rotation value corresponding to Earth's rotation.
When managed by AetherTime, this is updated automatically.

Constellation Guide:

Orion Lines
Connects only the seven main stars of Orion with guide lines.
This follows the star catalog coordinates and location synchronization.
It can be used to check the orientation of the starry sky.

Basic:

These are procedural star settings shown only when the Hipparcos star catalog is OFF.
You can configure density, scale, seed, brightness, rotation, rotation axis, and color layers.

Fade:

Height / Intensity
The range and strength for fading out stars near the horizon or during twilight.

Emission:

Glow settings for procedural stars.

Cross Flare:

Cross-shaped flares on bright stars.
You can configure amount, rotation, thickness, length, and opacity.

Twinkle:

Brightness Range
The range of stars affected by twinkling.
0 affects only bright stars, while 1 affects almost all stars.

Invert
When ON, twinkling is applied starting from darker stars.

Blink Speed
The twinkling speed.

Blink Strength
The brightness difference caused by twinkling.

Shooting Stars:

Display
Displays shooting stars.

Frequency
The number of appearances per minute.

Color
The color of shooting stars.

Intensity
The strength of HDR additive emission.

Speed
How fast shooting stars cross the sky.

Length
The length of the tail.

Thickness
The line width.

Duration
The number of seconds one shooting star remains visible.

Seed
Changes the pattern of appearance positions and directions.


[Milky Way]

Display
Displays the faint band of the Milky Way.

Star Density Map
Uses a density map generated from the distribution of magnitude 7 to 12 stars.

Density Map
A map that determines the density and color of the Milky Way.

Dust Dark Lanes
Uses a dust map to reflect the dark lanes of the Milky Way.

Dust Map
The map used for dark lanes.

Darkness
The strength of darkening the Milky Way with dust dark lanes.

Intensity
The overall brightness of the Milky Way.

Width
The width of the Milky Way band when not using the density map.

Contrast
How strongly density differences in the Milky Way are emphasized.

Saturation
The saturation of the Milky Way color.

Blue/Cyan
Boosts colors in the blue to cyan direction.

Color Shift
The strength of shifting toward the specified color.

Warm Galactic Center
The warmth around the galactic center of the Milky Way.

Color
The base color applied to the Milky Way.


FAQ

Q. The sky does not change unless I enter Play mode.
A. AetherTime runs during Play mode. If you want to check the time in the editor, use Manual Time / Test Time in AetherTime.

Q. Changing the material time has no effect.
A. If AetherTime is managing that material during Play mode, the material-side time is disabled. Change it from AetherTime instead.

Q. Does the moon also move when I change time on the material by itself?
A. Even when real moon position is ON, the moon and real star catalog rotate according to the time difference. However, use AetherTime if you also want the moon phase and right ascension / declination to update according to the date.

Q. Is 0.50 clock time 12:00?
A. In Aether, 0.50 is treated as solar noon. When using AetherTime, it is corrected to local apparent solar time from longitude, Time Zone, and the equation of time, so it may not exactly match clock time 12:00.

Q. Do sunset and sunrise match real time?
A. With Season Sync ON, the sun altitude is approximated using latitude, date, longitude, Time Zone, and the equation of time. Real seasonal and regional differences are reflected, but altitude, atmospheric refraction, horizon unevenness, and daylight saving time are not handled automatically.

Q. During Auto Rotate, do the moon and stars move at the same speed?
A. With Auto Rotate on the AetherTime side, not only the sun but also moon phase, moon position, sidereal time, and seasonal synchronization update with the same flow of time. A speed of 0 can fix the observation date and time.

Q. What happens when the sun and moon overlap?
A. The sun disk is eclipsed by the moon, and the sun glow, direct scattering in the daytime sky, and sun Directional Light weaken according to the eclipsed amount.

Q. The moon is not visible.
A. Check moon phase, moon position, time, moon size, moon color, and halo intensity. During real-time synchronization, there may be times when the moon is not visible depending on the date and observation location.

Q. Stars are not visible.
A. Check sun altitude, star brightness, star attenuation, clouds, city lights, and star attenuation around the moon.

Q. The Milky Way is too white / its color is weak.
A. Adjust Milky Way saturation, Blue/Cyan, Color Shift, Warm Galactic Center, Density Map, and Dust Dark Lanes.

Q. Stars are visible through clouds.
A. Increase cloud Star Attenuation. The appearance also changes depending on cloud opacity and thickness.

Q. Can this be used on Quest?
A. No. It is for PC.

Q. Ambient light does not follow the sky.
A. VRChat ambient light generally depends on baked lighting and Lighting settings. It may not automatically follow the sky color.


Other Limitations

- Built for the Unity Built-in Render Pipeline only.
- Intended for VRChat PC worlds.
- Quest is not supported.
- Daylight saving time is not handled automatically. Adjust it with Time Offset Seconds if needed.
- Star colors in the Hipparcos star catalog are estimated from the B-V color index.
- Moon position, moon phase, and solar declination are approximations for world presentation. They are not high-precision calculations for astronomical observation.
- Clouds, city lights, shooting stars, and the Milky Way are drawn inside the shader. They do not simulate real weather or city lights.
