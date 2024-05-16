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
    public void Initialize()
    {
      GameMain.LuaCs.Networking.Receive("fsm_sync", (object[] args) =>
      {
        if (ModStage == "debug") log("sync server");

        IReadMessage netMessage = args[0] as IReadMessage;
        Client client = args[1] as Client;

        if (client.Connection != GameMain.Server.OwnerConnection && !client.HasPermission(ClientPermissions.All))
        {
          log($"{client.Name} tried to change fsm settings, but he doesn't have permission");
          return;
        }

        IWriteMessage message = GameMain.LuaCs.Networking.Start("fsm_sync");
        Settings.cloneMsg(netMessage, message);

        GameMain.LuaCs.Networking.Send(message);
      });
    }


    public void OnLoadCompleted() { }
    public void PreInitPatching() { }

    public void Dispose() { }
  }
}