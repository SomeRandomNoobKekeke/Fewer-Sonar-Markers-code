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

namespace FewerSonarMarkers
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
      public bool ModEnabled { get; set; } = true;
      public SonarSettings StaticSonar { get; set; } = new SonarSettings();
      public SonarSettings HandheldSonar { get; set; } = new SonarSettings();

      public bool showMissionsWithPositionCaveIfTheyAreNotRegisteredInTheirCaveForSomeReason { get; set; } = true;
      public string Version { get; set; } = "0.0.0";

      public Settings()
      {
        this.Version = ModVersion;
      }

      public static void forceChangeSomething(Settings newSettings)
      {

      }

      public static Settings load(string path = "")
      {
        if (path == "") path = Path.Combine(SettingsFolder, SettingsFileName);

        Settings newSettings = new Settings();

        if (File.Exists(path))
        {
          try
          {
            newSettings = JsonSerializer.Deserialize<Settings>(
              File.ReadAllText(path)
            );
          }
          catch (Exception e) { log(e, Color.Orange); }

          if (String.Compare(newSettings.Version, ModVersion) < 0)
          {
            newSettings.Version = ModVersion;
            forceChangeSomething(newSettings);
          }
        }

        return newSettings;
      }

      public static void save(Settings s, string path = "")
      {
        if (path == "") path = Path.Combine(SettingsFolder, SettingsFileName);

        try
        {
          File.WriteAllText(path, json(s, true));
        }
        catch (Exception e) { log(e, Color.Orange); }
      }

      public static bool Compare(Settings a, Settings b)
      {
        return json(a) == json(b);
      }

      public static void encode(Settings s, IWriteMessage msg)
      {
        msg.WriteBoolean(s.ModEnabled);
        msg.WriteBoolean(s.showMissionsWithPositionCaveIfTheyAreNotRegisteredInTheirCaveForSomeReason);

        msg.WriteBoolean(s.StaticSonar.showAiTargets);
        msg.WriteBoolean(s.StaticSonar.showCaveMarkers);
        msg.WriteBoolean(s.StaticSonar.showMarkersOnlyOnMouseHover);
        msg.WriteBoolean(s.StaticSonar.showMinerals);
        msg.WriteBoolean(s.StaticSonar.showOutpostMarkers);
        msg.WriteBoolean(s.StaticSonar.showSumbarines);

        // StaticSonar
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.ClearRuinMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.ClearThalamusMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.BeaconMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.AbandonedOutpostMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.EndMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.MineralMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.MonsterMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.NestMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.PirateMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.SalvageMission);
        msg.WriteBoolean(s.StaticSonar.drawMarkersIn.ScanMission);

        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["MineralMission"]["Cave"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["MineralMission"]["SidePath"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["MineralMission"]["MainPath"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["MineralMission"]["AbyssCave"]);

        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["MonsterMission"]["MainPath"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["MonsterMission"]["SidePath"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["MonsterMission"]["Abyss"]);

        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["NestMission"]["Cave"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["NestMission"]["Ruin"]);

        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["SalvageMission"]["Ruin"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["SalvageMission"]["Cave"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["SalvageMission"]["AbyssCave"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["SalvageMission"]["Wreck"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["SalvageMission"]["Outpost"]);
        msg.WriteBoolean(s.StaticSonar.allowedPositionsIn["SalvageMission"]["None"]);


        // HandheldSonar
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.ClearRuinMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.ClearThalamusMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.BeaconMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.AbandonedOutpostMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.EndMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.MineralMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.MonsterMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.NestMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.PirateMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.SalvageMission);
        msg.WriteBoolean(s.HandheldSonar.drawMarkersIn.ScanMission);

        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["MineralMission"]["Cave"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["MineralMission"]["SidePath"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["MineralMission"]["MainPath"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["MineralMission"]["AbyssCave"]);

        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["MonsterMission"]["MainPath"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["MonsterMission"]["SidePath"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["MonsterMission"]["Abyss"]);

        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["NestMission"]["Cave"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["NestMission"]["Ruin"]);

        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["SalvageMission"]["Ruin"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["SalvageMission"]["Cave"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["SalvageMission"]["AbyssCave"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["SalvageMission"]["Wreck"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["SalvageMission"]["Outpost"]);
        msg.WriteBoolean(s.HandheldSonar.allowedPositionsIn["SalvageMission"]["None"]);

        msg.WriteString(s.Version);
      }

      public static void decode(Settings s, IReadMessage msg)
      {
        s.ModEnabled = msg.ReadBoolean();
        s.showMissionsWithPositionCaveIfTheyAreNotRegisteredInTheirCaveForSomeReason = msg.ReadBoolean();

        s.StaticSonar.showAiTargets = msg.ReadBoolean();
        s.StaticSonar.showCaveMarkers = msg.ReadBoolean();
        s.StaticSonar.showMarkersOnlyOnMouseHover = msg.ReadBoolean();
        s.StaticSonar.showMinerals = msg.ReadBoolean();
        s.StaticSonar.showOutpostMarkers = msg.ReadBoolean();
        s.StaticSonar.showSumbarines = msg.ReadBoolean();

        // StaticSonar
        s.StaticSonar.drawMarkersIn.ClearRuinMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.ClearThalamusMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.BeaconMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.AbandonedOutpostMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.EndMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.MineralMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.MonsterMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.NestMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.PirateMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.SalvageMission = msg.ReadBoolean();
        s.StaticSonar.drawMarkersIn.ScanMission = msg.ReadBoolean();

        s.StaticSonar.allowedPositionsIn["MineralMission"]["Cave"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["MineralMission"]["SidePath"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["MineralMission"]["MainPath"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["MineralMission"]["AbyssCave"] = msg.ReadBoolean();

        s.StaticSonar.allowedPositionsIn["MonsterMission"]["MainPath"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["MonsterMission"]["SidePath"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["MonsterMission"]["Abyss"] = msg.ReadBoolean();

        s.StaticSonar.allowedPositionsIn["NestMission"]["Cave"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["NestMission"]["Ruin"] = msg.ReadBoolean();

        s.StaticSonar.allowedPositionsIn["SalvageMission"]["Ruin"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["SalvageMission"]["Cave"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["SalvageMission"]["AbyssCave"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["SalvageMission"]["Wreck"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["SalvageMission"]["Outpost"] = msg.ReadBoolean();
        s.StaticSonar.allowedPositionsIn["SalvageMission"]["None"] = msg.ReadBoolean();


        // HandheldSonar
        s.HandheldSonar.drawMarkersIn.ClearRuinMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.ClearThalamusMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.BeaconMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.AbandonedOutpostMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.EndMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.MineralMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.MonsterMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.NestMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.PirateMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.SalvageMission = msg.ReadBoolean();
        s.HandheldSonar.drawMarkersIn.ScanMission = msg.ReadBoolean();

        s.HandheldSonar.allowedPositionsIn["MineralMission"]["Cave"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["MineralMission"]["SidePath"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["MineralMission"]["MainPath"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["MineralMission"]["AbyssCave"] = msg.ReadBoolean();

        s.HandheldSonar.allowedPositionsIn["MonsterMission"]["MainPath"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["MonsterMission"]["SidePath"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["MonsterMission"]["Abyss"] = msg.ReadBoolean();

        s.HandheldSonar.allowedPositionsIn["NestMission"]["Cave"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["NestMission"]["Ruin"] = msg.ReadBoolean();

        s.HandheldSonar.allowedPositionsIn["SalvageMission"]["Ruin"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["SalvageMission"]["Cave"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["SalvageMission"]["AbyssCave"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["SalvageMission"]["Wreck"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["SalvageMission"]["Outpost"] = msg.ReadBoolean();
        s.HandheldSonar.allowedPositionsIn["SalvageMission"]["None"] = msg.ReadBoolean();

        s.Version = msg.ReadString();
      }

      // never used
      public static void cloneMsg(IReadMessage source, IWriteMessage target)
      {
        for (int i = 0; i < 58; i++)
        {
          target.WriteBoolean(source.ReadBoolean());
        }
        target.WriteString(source.ReadString());
      }

    }
  }
}