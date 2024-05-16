using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;
using Barotrauma.Networking;

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    // this is relative to barotrauma folder
    public static string ModSettingsFolder = "ModSettings\\";
    public static string SettingsFolder = "ModSettings\\Fewer Sonar Markers\\";
    public static string SettingsFileName = "Settings.json";
    public static string PresetsFolder = "Presets";

    public static void createFolders()
    {
      if (!Directory.Exists(ModSettingsFolder)) Directory.CreateDirectory(ModSettingsFolder);
      if (!Directory.Exists(SettingsFolder)) Directory.CreateDirectory(SettingsFolder);
    }

    public partial class Settings
    {


    }
  }
}