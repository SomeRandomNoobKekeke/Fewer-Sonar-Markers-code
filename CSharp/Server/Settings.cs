using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;
using Barotrauma.Networking;

namespace FewerSonarMarkers
{
  public partial class Mod : IAssemblyPlugin
  {
    public partial class Settings
    {
      public static void net_recieve_init(object[] args)
      {
        info("net_recieve_init server");
        IReadMessage netMessage = args[0] as IReadMessage;
        Client client = args[1] as Client;

        IWriteMessage message = GameMain.LuaCs.Networking.Start("fsm_init");
        Settings.encode(settings, message);

        GameMain.LuaCs.Networking.Send(message);
      }

      public static void net_recieve_sync(object[] args)
      {
        info("net_recieve_sync server");

        IReadMessage inMsg = args[0] as IReadMessage;
        Client client = args[1] as Client;

        if (client.Connection != GameMain.Server.OwnerConnection && !client.HasPermission(ClientPermissions.All))
        {
          //log($"{client.Name} tried to change fsm settings, but he doesn't have permission");
          return;
        }

        Settings.decode(settings, inMsg);

        IWriteMessage outMsg = GameMain.LuaCs.Networking.Start("fsm_sync");
        Settings.encode(settings, outMsg);

        GameMain.LuaCs.Networking.Send(outMsg);
      }
    }
  }
}