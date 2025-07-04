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

namespace FewerSonarMarkers
{
  public partial class Mod : IAssemblyPlugin
  {
    public static Settings settings;

    public Harmony harmony;

    public void Initialize()
    {
      harmony = new Harmony("fewer.sonar.markers");
      try
      {
        figureOutModVersionAndDirPath();
        createFolders();

        settings = Settings.load();
        Settings.save(settings);

        addCommands();

        patchAll();

        if (GameMain.IsMultiplayer)
        {
          GameMain.LuaCs.Networking.Receive("fsm_init", Settings.net_recieve_init);
          GameMain.LuaCs.Networking.Receive("fsm_sync", Settings.net_recieve_sync);

          Settings.askServerForSettings();
        }

        info($"{ModName} {ModVersion} Compiled");
      }
      catch (Exception e)
      {
        log("can't load Fewer Sonar Markers", Color.Orange);
        err(e);
      }
    }

    public void patchAll()
    {
      harmony.Patch(
        original: typeof(Sonar).GetMethod("DrawSonar", AccessTools.all),
        prefix: new HarmonyMethod(typeof(Mod).GetMethod("Sonar_DrawSonar_Replace"))
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
      harmony.UnpatchSelf();
      harmony = null;
      removeCommands();
      settings = null;
    }
  }


}