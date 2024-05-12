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

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public static string BarotraumaFolder = "";
    public static string ModSettingsFolder = "ModSettings\\";
    public static string SettingsFolder = "ModSettings\\Fewer Sonar Markers\\";
    public static string SettingsFileName = "Settings.json";
    public static string PresetsFolder = "Presets";
    public class Settings
    {
      public bool ModEnabled { get; set; } = true;
      public SonarSettings StaticSonar { get; set; } = new SonarSettings();
      public SonarSettings HandheldSonar { get; set; } = new SonarSettings();

      public bool showMissionsWithPositionCaveIfTheyAreNotRegisteredInTheirCaveForSomeReason { get; set; } = true;
      public string Version { get; set; } = "0.0.0";

      public Settings()
      {
        this.Version = ModVersion;
      }

      public static void forceChangeSomething()
      {
        // nothing yet
      }

      public static void createStuffIfItDoesntExist()
      {
        // this is relative to barotrauma folder
        if (!Directory.Exists(ModSettingsFolder)) Directory.CreateDirectory(ModSettingsFolder);
        if (!Directory.Exists(SettingsFolder)) Directory.CreateDirectory(SettingsFolder);

        if (!File.Exists(Path.Combine(SettingsFolder, SettingsFileName))) settings.save();
      }

      public void load(string path = "")
      {
        createStuffIfItDoesntExist();

        if (path == "") path = Path.Combine(SettingsFolder, SettingsFileName);
        else log($"loading preset {path}");

        try
        {
          settings = JsonSerializer.Deserialize<Settings>(
            File.ReadAllText(path)
          );
        }
        catch (Exception e) { log(e.Message, Color.Orange); }

        if (String.Compare(settings.Version, ModVersion) < 0)
        {
          settings.Version = ModVersion;
          forceChangeSomething();
        }

        settings.save();
      }

      public void save()
      {
        try
        {
          File.WriteAllText(
            Path.Combine(SettingsFolder, SettingsFileName),
            JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true })
          );
        }
        catch (Exception e) { log(e.Message, Color.Orange); }
      }
    }
  }
}