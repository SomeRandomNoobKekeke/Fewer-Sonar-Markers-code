using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public Harmony harmony;

    public void Initialize()
    {
      harmony = new Harmony("no.markers");

      patchAll();
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
      DebugConsole.NewMessage($"{lineNumber}| {msg ?? "null"}", cl);
    }

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }

    public void Dispose()
    {
      harmony.UnpatchAll(harmony.Id);
      harmony = null;
    }
  }


}