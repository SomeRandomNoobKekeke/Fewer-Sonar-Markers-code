using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public static string BarotraumaFolder = "";
    public static string ModSettingsFolder = "ModSettings\\";
    public static string SettingsFolder = "ModSettings\\Fewer Sonar Markers\\";
    public static string SettingsFileName = "Settings.json";
    public class Settings
    {
      public bool ModEnabled { get; set; } = true;
      public SonarSettings StaticSonar { get; set; } = new SonarSettings();
      public SonarSettings HandheldSonar { get; set; } = new SonarSettings();
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

        if (!File.Exists(Path.Combine(SettingsFolder, SettingsFileName))) saveSettings();
      }

      public static void load()
      {
        settings = new Settings();

        createStuffIfItDoesntExist();

        try
        {
          settings = JsonSerializer.Deserialize<Settings>(
            File.ReadAllText(Path.Combine(SettingsFolder, SettingsFileName))
          );
        }
        catch (Exception e) { log(e.Message, Color.Orange); }

        if (String.Compare(settings.Version, ModVersion) < 0)
        {
          settings.Version = ModVersion;
          forceChangeSomething();
        }

        saveSettings();
      }

      public static void saveSettings()
      {
        File.WriteAllText(
          Path.Combine(SettingsFolder, SettingsFileName),
          JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true })
        );
      }
    }
  }
}