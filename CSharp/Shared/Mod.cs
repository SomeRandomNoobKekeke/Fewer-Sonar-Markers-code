using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Barotrauma;
using Microsoft.Xna.Framework;

using System.Text.Json;
using System.Text.Json.Serialization;

using System.IO;
using Barotrauma.Networking;

[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace FewerSonarMarkers
{
  public partial class Mod : IAssemblyPlugin
  {

    public static string ModVersion = "1.0.0";
    public static string ModName = "Fewer Sonar Markers";
    public static string ModDir = "";
    public static bool debug = true;

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

    public static void log(object msg, Color? cl = null, string line = "")
    {
      if (cl == null) cl = Color.Cyan;
#if SERVER
      cl *= 0.8f;
#endif
      LuaCsLogger.LogMessage($"{line}{msg ?? "null"}", cl, cl);
    }
    public static void info(object msg, [CallerLineNumber] int lineNumber = 0) { if (debug) log(msg, Color.Cyan, $"{lineNumber}| "); }
    public static void err(object msg, [CallerLineNumber] int lineNumber = 0) { if (debug) log(msg, Color.Orange, $"{lineNumber}| "); }

    public static string json(Object o, bool indent = false)
    {
      try { return JsonSerializer.Serialize(o, new JsonSerializerOptions { WriteIndented = indent }); }
      catch (Exception e) { err(e); return ""; }
    }
  }
}