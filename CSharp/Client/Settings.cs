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

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {


    public partial class Settings
    {
      public static void sync(Settings s)
      {
        if (GameMain.IsSingleplayer) return;
        // if (GameMain.Client == null)
        // {
        //   log("GameMain.Client == null");
        //   return;
        // }

        // if (!GameMain.Client.IsServerOwner && !GameMain.Client.HasPermission(ClientPermissions.All))
        // {
        //   log("you need to be host or have permission 'all' to use it");
        //   return;
        // }

        IWriteMessage message = GameMain.LuaCs.Networking.Start("fsm_sync");
        Settings.encode(s, message);

        if (ModStage == "debug") log("sync start");

        GameMain.LuaCs.Networking.Send(message);
      }

      public static void askServerForSettings()
      {
        if (ModStage == "debug") log("init start");
        IWriteMessage message = GameMain.LuaCs.Networking.Start("fsm_init");
        GameMain.LuaCs.Networking.Send(message);
      }

      public static void net_recieve_init(object[] args)
      {
        if (ModStage == "debug") log("net_recieve_init client");

        IReadMessage netMessage = args[0] as IReadMessage;
        Client client = args[1] as Client;

        Settings serverSettings = new Settings();
        Settings.decode(serverSettings, netMessage);
        if (!Settings.Compare(serverSettings, settings))
        {
          settings = serverSettings;
          log("Sonar markers settings changed");
        }
      }

      public static void net_recieve_sync(object[] args)
      {
        if (ModStage == "debug") log("net_recieve_sync client");

        IReadMessage netMessage = args[0] as IReadMessage;
        Client client = args[1] as Client;

        Settings.decode(settings, netMessage);
        log("Sonar markers settings changed");
      }

    }
  }
}