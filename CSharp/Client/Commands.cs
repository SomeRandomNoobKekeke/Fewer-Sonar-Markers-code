using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public static Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>()
    {
      {"load",(string[] args)=>{
        settings.load();
        log("Settings loaded");
      }},
      {"save",(string[] args)=>{
        settings.save();
        log("Settings saved");
      }},
      {"hard",(string[] args)=>{
        settings.load(Path.Combine(ModDir, PresetsFolder, "Hard.json"));
        settings.save();
      }},
      {"easy",(string[] args)=>{
        settings.load(Path.Combine(ModDir, PresetsFolder, "Easy.json"));
        settings.save();
      }},

      {"hide",(string[] args)=>{
        string what = args.ElementAtOrDefault(0);
        string positionType = args.ElementAtOrDefault(1);
        string onWhichSonar = args.ElementAtOrDefault(2);


        if(what == null) { log("hide what?"); return;}

        if(what == "all"){
          settings.load(Path.Combine(ModDir, PresetsFolder, "Hide all.json"));
          settings.save();
          return;
        }

        if(onWhichSonar == null || onWhichSonar == "onhandheldsonar"){
          modSonar("onhandheldsonar");
        }

        if(onWhichSonar == null || onWhichSonar == "onstaticsonar"){
          modSonar("onstaticsonar");
        }


        void modSonar(string name){
          SonarSettings sonar = name =="onstaticsonar" ?  settings.StaticSonar : settings.HandheldSonar ;

          if(what=="labels"){
            sonar.showMarkersOnlyOnMouseHover = true;
            log($"showMarkersOnlyOnMouseHover = {true} {name}");
            return;
          }

          if(what=="caves"){
            sonar.showCaveMarkers = false;
            log($"showCaveMarkers = false {name}");
            return;
          }

          if(what=="minerals"){
            sonar.showMinerals = false;
            log($"showMinerals = false {name}");
            return;
          }
          if(what=="aitargets"){
            sonar.showAiTargets = false;
            log($"showAiTargets = false {name}");
            return;
          }
          if(what=="outposts"){
            sonar.showOutpostMarkers = false;
            log($"showOutpostMarkers = false {name}");
            return;
          }

          if(what=="submarines"){
            sonar.showSumbarines = false;
            log($"showSumbarines = false {name}");
            return;
          }

          if(positionType == null){
            if(sonar.drawMarkersIn.ContainsKey(what)){
              sonar.drawMarkersIn[what] = false;
              log($"{what} markers {name} are hidden");
            } else  log("no such mission");
          } else {
            if(sonar.allowedPositionsIn.ContainsKey(what)){
              sonar.allowedPositionsIn[what][positionType] = false;
              log($"{what} markers {name} in {positionType} are hidden");
            } else log("no such mission or it doesn't have spawn positions");
          }
        }
      }},

      {"reveal",(string[] args)=>{
        string what = args.ElementAtOrDefault(0);
        string positionType = args.ElementAtOrDefault(1);
        string onWhichSonar = args.ElementAtOrDefault(2);

        if(what == null) { log("reveal what?"); return;}

        if(what == "all"){
          settings.load(Path.Combine(ModDir, PresetsFolder, "Reveal all.json"));
          settings.save();
          return;
        }

        if(onWhichSonar == null || onWhichSonar == "onhandheldsonar"){
          modSonar("onhandheldsonar");
        }

        if(onWhichSonar == null || onWhichSonar == "onstaticsonar"){
          modSonar("onstaticsonar");
        }


        void modSonar(string name){
          SonarSettings sonar = name =="onstaticsonar" ?  settings.StaticSonar : settings.HandheldSonar ;

          if(what=="labels"){
            sonar.showMarkersOnlyOnMouseHover = false;
            log($"showMarkersOnlyOnMouseHover = {false} {name}");
            return;
          }

          if(what=="caves"){
            sonar.showCaveMarkers = true;
            log($"showCaveMarkers = true {name}");
            return;
          }

          if(what=="minerals"){
            sonar.showMinerals = true;
            log($"showMinerals = true {name}");
            return;
          }
          if(what=="aitargets"){
            sonar.showAiTargets = true;
            log($"showAiTargets = true {name}");
            return;
          }
          if(what=="outposts"){
            sonar.showOutpostMarkers = true;
            log($"showOutpostMarkers = true {name}");
            return;
          }

          if(what=="submarines"){
            sonar.showSumbarines = true;
            log($"showSumbarines = true {name}");
            return;
          }

          if(positionType == null){
            if(sonar.drawMarkersIn.ContainsKey(what)){
              sonar.drawMarkersIn[what] = true;
              log($"{what} markers {name} are revealed");
            } else  log("no such mission");
          } else {
            if(sonar.allowedPositionsIn.ContainsKey(what)){
              sonar.allowedPositionsIn[what][positionType] = true;
              log($"{what} markers {name} in {positionType} are revealed");
            } else log("no such mission or it doesn't have spawn positions");
          }
        }
      }},
    };


    public static void addCommands()
    {
      DebugConsole.Commands.Add(new DebugConsole.Command("debugmissions", "print missions", (string[] args) =>
      {
        if (GameMain.GameSession.Missions.Count() == 0) { log("no missions :("); return; }

        foreach (Mission mission in GameMain.GameSession.Missions)
        {
          try
          {
            string pos = "";
            if (mission is MineralMission mm) pos = $"{mm.positionType}";  //  k
            if (mission is MonsterMission monm) pos = $"{monm.spawnPosType}";  //  e
            if (mission is NestMission nm) pos = $"{nm.spawnPositionType}";  //  k
            if (mission is SalvageMission sm) pos = $"{sm.targets.First().SpawnPositionType}";  //  w

            log($"{mission} | {mission.Name.Value} | {pos}");
          }
          catch (Exception e) { log(e, Color.Orange); }
        }
      }));

      string[][] hints = new string[][] {
        commands.Keys.ToArray(),
        settings.HandheldSonar.drawMarkersIn.Keys.Concat(new string[]{"all","labels","caves","minerals","outposts","submarines","aitargets"}).ToArray(),
        allPositionTypes,
        new string[]{"onhandheldsonar","onstaticsonar"},
      };

      string help = "sm hide/reveal what inWhichPosition onWhichSonar\n" + json(hints, true);

      DebugConsole.Commands.Add(new DebugConsole.Command("sm", help, (string[] args) =>
      {
        if (args.Length == 0)
        {
          settings.ModEnabled = !settings.ModEnabled;
          log(settings.ModEnabled ? "Sonar markers hidden" : "Sonar markers revealed");
        }

        if (args.Length > 0)
        {
          if (commands.TryGetValue(args[0], out var com)) com(args.Skip(1).ToArray());
        }

        settings.save();
      }, () => hints));
    }

    public static void removeCommands()
    {
      DebugConsole.Commands.RemoveAll(c => c.Names.Contains("debugmissions"));
      DebugConsole.Commands.RemoveAll(c => c.Names.Contains("sm"));
    }

    public static void permitCommands(Identifier command, ref bool __result)
    {
      if (command.Value == "debugmissions") __result = true;
      if (command.Value == "sm") __result = true;
    }
  }
}