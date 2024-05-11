using System;
using System.Reflection;
using System.Runtime.CompilerServices;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NoMarkersNamespace
{
  public partial class Mod : IAssemblyPlugin
  {
    public class SonarSettings
    {
      public bool showMarksOnlyOnMouseHover { get; set; } = true;
    }
  }
}