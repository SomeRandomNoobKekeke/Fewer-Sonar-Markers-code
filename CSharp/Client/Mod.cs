using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

using System.IO;
using Barotrauma.Items.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public static string ModVersion = "1.0.0";
    public static string ModName = "Fewer Sonar Markers";
    public static string ModDir = "";

    public static string ModStage = "debug";

    public static Settings settings;

    public Harmony harmony;

    public void Initialize()
    {
      harmony = new Harmony("no.markers");
      figureOutModVersionAndDirPath();

      settings = new Settings();
      settings.load();

      addCommands();

      patchAll();



      if (ModStage == "debug") log("compiled!");
    }

    public void figureOutModVersionAndDirPath()
    {
      bool found = false;
      foreach (ContentPackage p in ContentPackageManager.EnabledPackages.All)
      {
        if (p.Name.Contains(ModName))
        {
          found = true;
          ModVersion = p.ModVersion;
          ModDir = Path.GetFullPath(p.Dir);
          break;
        }
      }

      if (!found) log($"Couldn't figure out {ModName} mod folder", Color.Orange);
    }

    public void patchAll()
    {
      harmony.Patch(
        original: typeof(Sonar).GetMethod("DrawSonar", AccessTools.all),
        prefix: new HarmonyMethod(typeof(Mod).GetMethod("Sonar_DrawSonar_Prefix"))
      );

      harmony.Patch(
        original: typeof(LuaGame).GetMethod("IsCustomCommandPermitted"),
        postfix: new HarmonyMethod(typeof(Mod).GetMethod("permitCommands"))
      );
    }


    public static void log(object msg, Color? cl = null, [CallerLineNumber] int lineNumber = 0)
    {
      if (cl == null) cl = Color.Cyan;
      DebugConsole.NewMessage($"{lineNumber} {msg ?? "null"}", cl);
    }

    public static string json(Object o, bool indent = false)
    {
      try { return JsonSerializer.Serialize(o, new JsonSerializerOptions { WriteIndented = indent }); }
      catch (Exception e) { if (ModStage == "debug") log(e); return ""; }
    }

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }

    public void Dispose()
    {
      harmony.UnpatchAll(harmony.Id);
      harmony = null;
      removeCommands();
    }
  }


}