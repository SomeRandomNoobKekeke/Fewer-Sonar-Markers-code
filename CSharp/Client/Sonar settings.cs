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

namespace FewerSonarMarkers
{
  public partial class Mod : IAssemblyPlugin
  {
    public static bool once = false;

    public static bool isMissionAllowed(Mission mission, Sonar _)
    {
      SonarSettings mySettings = _.DetectSubmarineWalls ? settings.HandheldSonar : settings.StaticSonar;

      try
      {
        if (mission is AbandonedOutpostMission)
        {
          if (!mySettings.drawMarkersIn.AbandonedOutpostMission) return false;
          return true;
        };

        if (mission is EliminateTargetsMission etm)
        {
          if (etm.TargetSubType == SubmarineType.Wreck && !mySettings.drawMarkersIn.ClearThalamusMission) return false;

          if (etm.TargetSubType == SubmarineType.Ruin && !mySettings.drawMarkersIn.ClearRuinMission) return false;

          return true;
        };

        if (mission is BeaconMission)
        {
          if (!mySettings.drawMarkersIn.BeaconMission) return false;
          return true;
        };

        if (mission is EndMission)
        {
          if (!mySettings.drawMarkersIn.EndMission) return false;
          return true;
        };

        if (mission is MineralMission mm)
        {
          if (!mySettings.drawMarkersIn.MineralMission) return false;

          if (!mySettings.allowedPositionsIn["MineralMission"][$"{mm.positionType}"])
          {
            if (settings.showMissionsWithPositionCaveIfTheyAreNotRegisteredInTheirCaveForSomeReason && mm.positionType == PositionType.Cave && mm.caves.Count == 0) return true;

            return false;
          }

          return true;
        }

        if (mission is MonsterMission monm)
        {
          if (!mySettings.drawMarkersIn.MonsterMission) return false;

          string[] pos = $"{monm.spawnPosType}".Split(", ");
          if (!pos.Any((p) => mySettings.allowedPositionsIn["MonsterMission"][p])) return false;
          return true;
        };

        if (mission is NestMission nm)
        {
          if (!mySettings.drawMarkersIn.NestMission) return false;

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
          if (!mySettings.drawMarkersIn.PirateMission) return false;
          return true;
        };

        if (mission is SalvageMission sm)
        {
          if (!mySettings.drawMarkersIn.SalvageMission) return false;

          string[] pos = $"{sm.targets.First().SpawnPositionType}".Split(", ");

          if (!pos.Any((p) => mySettings.allowedPositionsIn["SalvageMission"][p])) return false;

          return true;
        };

        if (mission is ScanMission)
        {
          if (!mySettings.drawMarkersIn.ScanMission) return false;
          return true;
        };

        return true;
      }
      catch (Exception e)
      {
        if (!once)
        {
          once = true;
          err(e);
          err(mission);
          err(json(settings, true));
        }
        return true;
      }
    }
  }
}