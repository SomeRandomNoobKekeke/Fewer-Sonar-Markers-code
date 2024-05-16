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
using Barotrauma.Networking;

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



    public static Settings settings;

    public Harmony harmony;

    public void Initialize()
    {
      harmony = new Harmony("no.markers");
      try
      {
        figureOutModVersionAndDirPath();

        createFolders();

        settings = Settings.load(Path.Combine(ModDir, PresetsFolder, "Easy.json"));
        Settings.save(settings);

        addCommands();

        patchAll();

        GameMain.LuaCs.Networking.Receive("fsm_sync", (object[] args) =>
        {
          if (ModStage == "debug") log("sync client");

          IReadMessage netMessage = args[0] as IReadMessage;
          Client client = args[1] as Client;

          Settings.decode(settings, netMessage);
          log("Sonar markers settings changed");
        });

        if (ModStage == "debug") log("compiled!");
      }
      catch (Exception e)
      {
        log("can't load Fewer Sonar Markers", Color.Orange);
        log(e, Color.Orange);
      }
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