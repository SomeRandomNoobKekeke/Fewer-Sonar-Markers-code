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
      {"hideall",(string[] args)=>{
        settings.load(Path.Combine(ModDir, PresetsFolder, "Hide all.json"));
        settings.save();
      }},
      {"revealall",(string[] args)=>{
        settings.load(Path.Combine(ModDir, PresetsFolder, "Reveal all.json"));
        settings.save();
      }},
      {"hide",(string[] args)=>{
        if(args.Length == 0) { log("hide what?"); return;}

        if(args.Length == 1) {
          if(settings.HandheldSonar.drawMarkersIn.ContainsKey(args[0])){
            settings.HandheldSonar.drawMarkersIn[args[0]] = false;
            log($"{args[0]} markers on handheld sonar are hidden");
          } else log("no such mission");

          if(settings.StaticSonar.drawMarkersIn.ContainsKey(args[0])){
            settings.StaticSonar.drawMarkersIn[args[0]] = false;
            log($"{args[0]} markers on static sonar are hidden");
          } else log("no such mission");
        }

        if(args.Length == 2) {
          switch(args[1]){
            case "onhandheldsonar":
              if(settings.HandheldSonar.drawMarkersIn.ContainsKey(args[0])){
                settings.HandheldSonar.drawMarkersIn[args[0]] = false;
                log($"{args[0]} markers on handheld sonar are hidden");
              } else log("no such mission");
              break;
            case "onstaticsonar":
              if(settings.StaticSonar.drawMarkersIn.ContainsKey(args[0])){
                settings.StaticSonar.drawMarkersIn[args[0]] = false;
                log($"{args[0]} markers on static sonar are hidden");
              } else log("no such mission");
              break;
            default:
              log("on which sonar?");
              break;
          }
        }

        if(args.Length > 2) {
          switch(args[1]){
            case "onhandheldsonar":
              if(settings.HandheldSonar.allowedPositionsIn.ContainsKey(args[0])){
                settings.HandheldSonar.allowedPositionsIn[args[0]][args[2]] = false;
                log($"{args[0]} markers {args[1]} in position {args[2]} are hidden");
              } else log("this mission don't have spawn positions");
              break;
            case "onstaticsonar":
              if(settings.StaticSonar.allowedPositionsIn.ContainsKey(args[0])){
                settings.StaticSonar.allowedPositionsIn[args[0]][args[2]] = false;
                log($"{args[0]} markers {args[1]} in position {args[2]} are hidden");
              } else log("this mission don't have spawn positions");
              break;
            default:
              log("on which sonar?");
              break;
          }
        }
      }},
      {"reveal",(string[] args)=>{
        if(args.Length == 0) { log("reveal what?"); return;}

        if(args.Length == 1) {
          if(settings.HandheldSonar.drawMarkersIn.ContainsKey(args[0])){
            settings.HandheldSonar.drawMarkersIn[args[0]] = true;
            log($"{args[0]} markers on handheld sonar are revealed");
          } else log("no such mission");

          if(settings.StaticSonar.drawMarkersIn.ContainsKey(args[0])){
            settings.StaticSonar.drawMarkersIn[args[0]] = true;
            log($"{args[0]} markers on static sonar are revealed");
          } else log("no such mission");
        }

        if(args.Length == 2) {
          switch(args[1]){
            case "onhandheldsonar":
              if(settings.HandheldSonar.drawMarkersIn.ContainsKey(args[0])){
                settings.HandheldSonar.drawMarkersIn[args[0]] = true;
                log($"{args[0]} markers on handheld sonar are revealed");
              } else log("no such mission");
              break;
            case "onstaticsonar":
              if(settings.StaticSonar.drawMarkersIn.ContainsKey(args[0])){
                settings.StaticSonar.drawMarkersIn[args[0]] = true;
                log($"{args[0]} markers  on static sonar are revealed");
              } else log("no such mission");
              break;
            default:
              log("on which sonar?");
              break;
          }
        }

        if(args.Length > 2) {
          switch(args[1]){
            case "onhandheldsonar":
              if(settings.HandheldSonar.drawMarkersIn.ContainsKey(args[0])){
                settings.HandheldSonar.drawMarkersIn[args[0]] = true;
              } else log("no such mission");
              if(settings.HandheldSonar.allowedPositionsIn.ContainsKey(args[0])){
                settings.HandheldSonar.allowedPositionsIn[args[0]][args[2]] = true;
                log($"{args[0]} markers {args[1]} in position {args[2]} are revealed");
              } else log("this mission don't have spawn positions");
              break;
            case "onstaticsonar":
              if(settings.StaticSonar.drawMarkersIn.ContainsKey(args[0])){
                settings.StaticSonar.drawMarkersIn[args[0]] = true;
              } else log("no such mission");
              if(settings.StaticSonar.allowedPositionsIn.ContainsKey(args[0])){
                settings.StaticSonar.allowedPositionsIn[args[0]][args[2]] = true;
                log($"{args[0]} markers {args[1]} in position {args[2]} are revealed");
              } else log("this mission don't have spawn positions");
              break;
            default:
              log("on which sonar?");
              break;
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

      string[][] help = new string[][] {
        commands.Keys.ToArray(),
        settings.HandheldSonar.drawMarkersIn.Keys.ToArray(),
        new string[]{"onhandheldsonar","onstaticsonar"},
        allPositionTypes,
      };

      DebugConsole.Commands.Add(new DebugConsole.Command("sm", json(help), (string[] args) =>
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
      }, () => help));
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