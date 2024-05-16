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
      public bool showCaveMarkers { get; set; } = true;
      public bool showOutpostMarkers { get; set; } = true;
      public bool showMinerals { get; set; } = true;
      public bool showSumbarines { get; set; } = true;
      public bool showAiTargets { get; set; } = true;
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

  }
}