using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using Barotrauma.Extensions;
using Barotrauma.Networking;
using FarseerPhysics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Barotrauma.Items.Components;

using PositionType = Barotrauma.Level.PositionType;

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public static string[] allPositionTypes = new string[]{
      "None","MainPath","SidePath","Cave","Ruin","Wreck","BeaconStation","Abyss","AbyssCave","Outpost",
    };

    public class SonarSettings
    {
      public bool showMarkersOnlyOnMouseHover { get; set; } = true;
      //public bool hideCaveMarkers { get; set; } = false;
      public Dictionary<string, bool> drawMarkersIn { get; set; } = new Dictionary<string, bool>(){
        {"ClearRuinMission",true},
        {"BeaconMission",true},
        {"AbandonedOutpostMission",true},
        {"EndMission",true},
        {"MineralMission",true},
        {"MonsterMission",true},
        {"NestMission",true},
        {"PirateMission",true},
        {"SalvageMission",true},
        {"ScanMission",true},
      };
      public Dictionary<string, Dictionary<string, bool>> allowedPositionsIn { get; set; } = new Dictionary<string, Dictionary<string, bool>>(){
        {"MineralMission", new Dictionary<string, bool>() {
          {"Cave",true},
          {"SidePath",true},
          {"MainPath",true},
          {"AbyssCave",true},
        }},
        {"MonsterMission", new Dictionary<string, bool>() {
          {"MainPath",true},
          {"SidePath",true},
          {"Abyss",true},
        }},
        {"NestMission", new Dictionary<string, bool>() {
          {"Cave",true},
          {"Ruin",true},
        }},
        {"SalvageMission", new Dictionary<string, bool>() {
          {"Ruin",true},
          {"Cave",true},
          {"AbyssCave",true},
          {"Wreck",true},
          {"Outpost",true},
          {"None",true},
        }},
      };
    }

    public static bool once = false;

    public static bool isMissionAllowed(Mission mission, Sonar _)
    {
      SonarSettings mySettings = _.DetectSubmarineWalls ? settings.HandheldSonar : settings.StaticSonar;

      try
      {
        if (mission is AbandonedOutpostMission)
        {
          if (!mySettings.drawMarkersIn["AbandonedOutpostMission"]) return false;
          return true;
        };

        if (mission is AlienRuinMission)
        {
          if (!mySettings.drawMarkersIn["ClearRuinMission"]) return false;
          return true;
        };

        if (mission is BeaconMission)
        {
          if (!mySettings.drawMarkersIn["BeaconMission"]) return false;
          return true;
        };

        if (mission is EndMission)
        {
          if (!mySettings.drawMarkersIn["EndMission"]) return false;
          return true;
        };

        if (mission is MineralMission mm)
        {
          if (!mySettings.drawMarkersIn["MineralMission"]) return false;

          if (!mySettings.allowedPositionsIn["MineralMission"][$"{mm.positionType}"])
          {
            if (settings.showMissionsWithPositionCaveIfTheyAreNotRegisteredInTheirCaveForSomeReason && mm.positionType == PositionType.Cave && mm.caves.Count == 0) return true;

            return false;
          }

          return true;
        }

        if (mission is MonsterMission monm)
        {
          if (!mySettings.drawMarkersIn["MonsterMission"]) return false;

          string[] pos = $"{monm.spawnPosType}".Split(", ");
          if (!pos.Any((p) => mySettings.allowedPositionsIn["MonsterMission"][p])) return false;
          return true;
        };

        if (mission is NestMission nm)
        {
          if (!mySettings.drawMarkersIn["NestMission"]) return false;

          string[] pos = $"{nm.spawnPositionType}".Split(", ");

          if (!pos.Any((p) => mySettings.allowedPositionsIn["NestMission"][p]))
          {
            if (settings.showMissionsWithPositionCaveIfTheyAreNotRegisteredInTheirCaveForSomeReason && pos.Any((p) => p == "Cave") && nm.selectedCave == null) return true;

            return false;
          }

          return true;
        };

        if (mission is PirateMission)
        {
          if (!mySettings.drawMarkersIn["PirateMission"]) return false;
          return true;
        };

        if (mission is SalvageMission sm)
        {
          if (!mySettings.drawMarkersIn["SalvageMission"]) return false;

          string[] pos = $"{sm.targets.First().SpawnPositionType}".Split(", ");

          if (!pos.Any((p) => mySettings.allowedPositionsIn["SalvageMission"][p])) return false;

          return true;
        };

        if (mission is ScanMission)
        {
          if (!mySettings.drawMarkersIn["ScanMission"]) return false;
          return true;
        };

        return true;
      }
      catch (Exception e)
      {
        if (ModStage == "debug" && !once)
        {
          once = true;
          log(e);
          log(mission);
          log(json(settings, true));
        }
        return true;
      }
    }
  }
}