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

using Barotrauma.Networking;

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public static Dictionary<string, Action<string[]>> subCommands = new Dictionary<string, Action<string[]>>()
    {
      {"hard",(string[] args)=>{
        settings = Settings.load(Path.Combine(ModDir, PresetsFolder, "Hard.json"));
        Settings.save(settings);
        log("loaded hard preset");
      }},
      {"easy",(string[] args)=>{
        settings = Settings.load(Path.Combine(ModDir, PresetsFolder, "Easy.json"));
        Settings.save(settings);
        log("loaded hard preset");
      }},
      {"hide",(string[] args)=>{
        string what = args.ElementAtOrDefault(0);
        string positionType = args.ElementAtOrDefault(1);
        string onWhichSonar = args.ElementAtOrDefault(2);


        if(what == null) { log("hide what?"); return;}

        if(what == "all"){
          settings = Settings.load(Path.Combine(ModDir, PresetsFolder, "Hide all.json"));
          Settings.save(settings);
          log("All sonar markers are hidden");
          return;
        }

        if(onWhichSonar == null || onWhichSonar == "onhandheldsonar"){
          modSonar("onhandheldsonar");
        }

        if(onWhichSonar == null || onWhichSonar == "onstaticsonar"){
          modSonar("onstaticsonar");
        }

        if(onWhichSonar != null && onWhichSonar != "onstaticsonar" && onWhichSonar != "onhandheldsonar"){
          log("onWhichSonar?");
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

          if(positionType == null || positionType == "any" || !allPositionTypes.Contains(positionType)){
            if(sonar.drawMarkersIn.ContainsKey(what)){
              sonar.drawMarkersIn[what] = false;
              log($"{what} markers {name} are hidden");
            } else  log("no such mission");
          } else {
            if(sonar.allowedPositionsIn.ContainsKey(what)){
              sonar.allowedPositionsIn[what][positionType] = false;
              log($"{what} markers {name} in {positionType} are hidden");
            } else log("no such mission or it doesn't support this position");
          }
        }
      }},

      {"reveal",(string[] args)=>{
        string what = args.ElementAtOrDefault(0);
        string positionType = args.ElementAtOrDefault(1);
        string onWhichSonar = args.ElementAtOrDefault(2);

        if(what == null) { log("reveal what?"); return;}

        if(what == "all"){
          settings = Settings.load(Path.Combine(ModDir, PresetsFolder, "Reveal all.json"));
          Settings.save(settings);
          log("All sonar markers are revealed");
          return;
        }

        if(onWhichSonar == null || onWhichSonar == "onhandheldsonar"){
          modSonar("onhandheldsonar");
        }

        if(onWhichSonar == null || onWhichSonar == "onstaticsonar"){
          modSonar("onstaticsonar");
        }

        if(onWhichSonar != null && onWhichSonar != "onstaticsonar" && onWhichSonar != "onhandheldsonar"){
          log("onWhichSonar?");
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

          if(positionType == null || positionType == "any" || !allPositionTypes.Contains(positionType)){
            if(sonar.drawMarkersIn.ContainsKey(what)){
              sonar.drawMarkersIn[what] = true;
              log($"{what} markers {name} are revealed");
            } else  log("no such mission");
          } else {
            if(sonar.allowedPositionsIn.ContainsKey(what)){
              sonar.allowedPositionsIn[what][positionType] = true;
              sonar.drawMarkersIn[what] = true;
              log($"{what} markers {name} in {positionType} are revealed");
            } else log("no such mission or it doesn't support this position");
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

      DebugConsole.Commands.Add(new DebugConsole.Command("sm_log", "print settings", (string[] args) =>
      {
        log(json(settings, true));
      }));

      DebugConsole.Commands.Add(new DebugConsole.Command("sm_save", "save settings to", (string[] args) =>
      {
        string path = Path.Combine(SettingsFolder, SettingsFileName);
        if (args.Length > 0) path = Path.Combine(SettingsFolder, $"{args[0]}.json");

        Settings.save(settings, path);

        log($"settings saved to {path}");
      }));

      DebugConsole.Commands.Add(new DebugConsole.Command("sm_load", "load settings from", (string[] args) =>
      {
        if (GameMain.IsMultiplayer && !GameMain.Client.IsServerOwner && !GameMain.Client.HasPermission(ClientPermissions.All))
        {
          log("you need to be host or have permission 'all' to use it");
          return;
        }

        string path = Path.Combine(SettingsFolder, SettingsFileName);
        if (args.Length > 0) path = Path.Combine(SettingsFolder, $"{args[0]}.json");

        if (!File.Exists(path))
        {
          log("no such file");
          return;
        }

        settings = Settings.load(path);

        log($"settings loaded from {path}");
        if (GameMain.IsMultiplayer) Settings.sync(settings);
      }));



      string[][] hints = new string[][] {
        subCommands.Keys.ToArray(),
        settings.HandheldSonar.drawMarkersIn.Keys.Concat(new string[]{"all","labels","caves","minerals","outposts","submarines","aitargets"}).ToArray(),
        allPositionTypes.Concat(new string[]{"any"}).ToArray(),
        new string[]{"onhandheldsonar","onstaticsonar"},
      };

      string help = "sm hide/reveal what inWhichPosition onWhichSonar\n" + json(hints, true);

      DebugConsole.Commands.Add(new DebugConsole.Command("sm", help, (string[] args) =>
      {
        if (GameMain.IsMultiplayer && !GameMain.Client.IsServerOwner && !GameMain.Client.HasPermission(ClientPermissions.All))
        {
          log("you need to be host or have permission 'all' to use it");
          return;
        }

        if (args.Length == 0)
        {
          settings.ModEnabled = !settings.ModEnabled;
          log(settings.ModEnabled ? "Sonar markers hidden" : "Sonar markers revealed");
        }

        if (args.Length > 0)
        {
          if (subCommands.TryGetValue(args[0], out var com)) com(args.Skip(1).ToArray());
        }

        Settings.save(settings);

        if (GameMain.IsMultiplayer) Settings.sync(settings);
      }, () => hints));
    }

    public static void removeCommands()
    {
      DebugConsole.Commands.RemoveAll(c => c.Names.Contains("debugmissions"));
      DebugConsole.Commands.RemoveAll(c => c.Names.Contains("sm"));
      DebugConsole.Commands.RemoveAll(c => c.Names.Contains("sm_save"));
      DebugConsole.Commands.RemoveAll(c => c.Names.Contains("sm_load"));
      DebugConsole.Commands.RemoveAll(c => c.Names.Contains("sm_log"));
    }

    public static void permitCommands(Identifier command, ref bool __result)
    {
      if (command.Value == "debugmissions") __result = true;
      if (command.Value == "sm") __result = true;
      if (command.Value == "sm_save") __result = true;
      if (command.Value == "sm_load") __result = true;
      if (command.Value == "sm_log") __result = true;
    }
  }
}