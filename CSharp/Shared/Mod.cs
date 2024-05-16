using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Barotrauma;
using Microsoft.Xna.Framework;

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
    public static string ModStage = "acceptable";

    public static void log(object msg, Color? cl = null, [CallerLineNumber] int lineNumber = 0)
    {
      if (cl == null) cl = Color.Cyan;
      string line = "";
      if (ModStage == "debug") line = $"{lineNumber} ";
      LuaCsLogger.LogMessage($"{line}{msg ?? "null"}", cl, cl);
    }

    public static string json(Object o, bool indent = false)
    {
      try { return JsonSerializer.Serialize(o, new JsonSerializerOptions { WriteIndented = indent }); }
      catch (Exception e) { if (ModStage == "debug") log(e); return ""; }
    }

    public static string Base64Encode(string plainText)
    {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
      var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
      return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static byte ConvertBoolArrayToByte(bool[] source)
    {
      byte result = 0;
      int index = 8 - source.Length;

      foreach (bool b in source)
      {
        if (b)
          result |= (byte)(1 << (7 - index));

        index++;
      }

      return result;
    }

    public static bool[] ConvertByteToBoolArray(byte b)
    {
      bool[] result = new bool[8];

      for (int i = 0; i < 8; i++)
        result[i] = (b & (1 << i)) != 0;

      Array.Reverse(result);

      return result;
    }


  }
}