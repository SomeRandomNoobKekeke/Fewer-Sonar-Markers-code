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

namespace FewerSonarMarkers
{
  public partial class Mod : IAssemblyPlugin
  {
    public static string[] allPositionTypes = new string[]{
      "None","MainPath","SidePath","Cave","Ruin","Wreck","BeaconStation","Abyss","AbyssCave","Outpost",
    };

    public class DrawMarkersInSettings // omg
    {
      public bool ClearRuinMission { get; set; } = true;
      public bool ClearThalamusMission { get; set; } = true;
      public bool BeaconMission { get; set; } = true;
      public bool AbandonedOutpostMission { get; set; } = true;
      public bool EndMission { get; set; } = true;
      public bool MineralMission { get; set; } = true;
      public bool MonsterMission { get; set; } = true;
      public bool NestMission { get; set; } = true;
      public bool PirateMission { get; set; } = true;
      public bool SalvageMission { get; set; } = true;
      public bool ScanMission { get; set; } = true;
    }

    public class SonarSettings
    {
      public bool showMarkersOnlyOnMouseHover { get; set; } = true;
      public bool showCaveMarkers { get; set; } = true;
      public bool showOutpostMarkers { get; set; } = true;
      public bool showMinerals { get; set; } = true;
      public bool showSumbarines { get; set; } = true;
      public bool showAiTargets { get; set; } = true;
      public DrawMarkersInSettings drawMarkersIn { get; set; } = new DrawMarkersInSettings();
      public Dictionary<string, Dictionary<string, bool>> allowedPositionsIn { get; set; } = new Dictionary<string, Dictionary<string, bool>>(){
        {"MineralMission", new Dictionary<string, bool>() {
          {"Cave",false},
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
          {"Cave",false},
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