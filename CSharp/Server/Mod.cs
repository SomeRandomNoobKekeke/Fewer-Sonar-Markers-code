using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

using Barotrauma;
using Barotrauma.Networking;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;

[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public static Settings settings;
    public void Initialize()
    {
      figureOutModVersionAndDirPath();
      createFolders();
      settings = Settings.load();

      GameMain.LuaCs.Networking.Receive("fsm_init", Settings.net_recieve_init);
      GameMain.LuaCs.Networking.Receive("fsm_sync", Settings.net_recieve_sync);
    }

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }

    public void Dispose() { }
  }
}