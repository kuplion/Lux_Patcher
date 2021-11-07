# Enhanced Lighting for ENB Synthesis patcher

## Description

Carries over all changes from [Enhanced Lighting for ENB](https://www.nexusmods.com/skyrimspecialedition/mods/1377). Also patches some modded locations to use ELE's lighting templates & image spaces, and adjusts some modded imagespaces & lights. 

<details>
  <summary>Detailed list of included changes</summary>

  - Image Spaces: HDR, cinematic, tint
  - Lights: record flags, flags, object bounds, radius, color, near clip, fade value
  - Worldspaces: interior lighting
  - Cells: lighting, lighting template, water height, water noise texture, sky and weather from region, image space
  - Placed objects: record flags, primitive, light data, bound half extents, unknown, lighting template, image space, location reference, placement
</details>

<details>
  <summary>Supported mods</summary>
  Patcher was made for version in parantheses, but should mostly work okay for any version.

  - Based on ELE's official patches, with updates here & there as said patches are 2 years old.
    - [Beyond Skyrim - Bruma SE](https://www.nexusmods.com/skyrimspecialedition/mods/10917) (1.4.2)
    - [Cutting Room Floor - SSE](https://www.nexusmods.com/skyrimspecialedition/mods/276) (3.1.9)
    - [Darkend](https://www.nexusmods.com/skyrimspecialedition/mods/10423) (1.4)
    - [Falskaar](https://www.nexusmods.com/skyrimspecialedition/mods/2057) (2.2)
    - [Helgen Reborn](https://www.nexusmods.com/skyrimspecialedition/mods/5673) (V106.SSE)
      - Added light bulb colors
    - [Lanterns of Skyrim](https://www.nexusmods.com/skyrimspecialedition/mods/2429) (any version)
    - [Legacy of the Dragonborn SSE](https://www.nexusmods.com/skyrimspecialedition/mods/11802) (5.5.2, 4.1.1 support included)
      - Added light bulb colors
      - v5 version uses brighter lighting templates for the museum interior, since the lighting almost purely depends on those now
    - [Ravengate](https://www.nexusmods.com/skyrimspecialedition/mods/12617) (0.06BTASSE)
      - Added light bulb colors
    - [Medieval Lanterns of Skyrim](https://www.nexusmods.com/skyrimspecialedition/mods/27622) (any version)
</details>

## Installation

### Synthesis

If you have Synthesis, there are 3 options:
- In Synthesis, click on Git repository, and choose ELE Patcher from the list of patchers.
  - Not available at the time of writing due to an issue on Synthesis' side.
- In Synthesis, click on Git repository, click on Input, and paste in `https://github.com/Benna96-Synthesis/ELE_patcher`. Then choose ELE_Patcher from the projects.
  - This will cause the name of the patcher to get stuck on Synthesis-Misc-Patchers, at the time of writing there's no way to change the name.
- [Grab the exe](https://github.com/Benna96-Synthesis/ELE_patcher/releases/latest/download/ELE_Patcher.exe), then in Synthesis, click on External Program, and browse for the exe.
  - Not recommended.

### Standalone

The patcher does run without Synthesis as well. Just [grab the exe](https://github.com/Benna96-Synthesis/ELE_patcher/releases/latest/download/ELE_Patcher.exe) and run it. The generated plugin is called `Synthesis ELE patch.esp`.

If you're an MO2 user, as with all things, remember to run through MO2!