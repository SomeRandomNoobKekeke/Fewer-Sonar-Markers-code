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
    private void DrawSonar(SpriteBatch spriteBatch, Rectangle rect)
    {
      displayBorderSize = 0.2f;
      center = rect.Center.ToVector2();
      DisplayRadius = (rect.Width / 2.0f) * (1.0f - displayBorderSize);
      DisplayScale = DisplayRadius / range * zoom;

      screenBackground?.Draw(spriteBatch, center, 0.0f, rect.Width / screenBackground.size.X);

      if (useDirectionalPing)
      {
        directionalPingBackground?.Draw(spriteBatch, center, 0.0f, rect.Width / directionalPingBackground.size.X);
        if (directionalPingButton != null)
        {
          int buttonSprIndex = 0;
          if (pingDragDirection != null)
          {
            buttonSprIndex = 2;
          }
          else if (MouseInDirectionalPingRing(rect, true))
          {
            buttonSprIndex = 1;
          }
          directionalPingButton[buttonSprIndex]?.Draw(spriteBatch, center, MathUtils.VectorToAngle(pingDirection), rect.Width / directionalPingBackground.size.X);
        }
      }

      if (currentPingIndex != -1)
      {
        var activePing = activePings[currentPingIndex];
        if (activePing.IsDirectional && directionalPingCircle != null)
        {
          directionalPingCircle.Draw(spriteBatch, center, Color.White * (1.0f - activePing.State),
              rotate: MathUtils.VectorToAngle(activePing.Direction),
              scale: DisplayRadius / directionalPingCircle.size.X * activePing.State);
        }
        else
        {
          pingCircle.Draw(spriteBatch, center, Color.White * (1.0f - activePing.State), 0.0f, (DisplayRadius * 2 / pingCircle.size.X) * activePing.State);
        }
      }

      float signalStrength = 1.0f;
      if (UseTransducers)
      {
        signalStrength = 0.0f;
        foreach (ConnectedTransducer connectedTransducer in connectedTransducers)
        {
          signalStrength = Math.Max(signalStrength, connectedTransducer.SignalStrength);
        }
      }

      Vector2 transducerCenter = GetTransducerPos();// + DisplayOffset;

      if (sonarBlips.Count > 0)
      {
        float blipScale = 0.08f * (float)Math.Sqrt(zoom) * (rect.Width / 700.0f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

        foreach (SonarBlip sonarBlip in sonarBlips)
        {
          DrawBlip(spriteBatch, sonarBlip, transducerCenter + DisplayOffset, center, sonarBlip.FadeTimer / 2.0f * signalStrength, blipScale);
        }

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
      }

      if (item.Submarine != null && !DetectSubmarineWalls)
      {
        transducerCenter += DisplayOffset;
        DrawDockingPorts(spriteBatch, transducerCenter, signalStrength);
        DrawOwnSubmarineBorders(spriteBatch, transducerCenter, signalStrength);
      }
      else
      {
        DisplayOffset = Vector2.Zero;
      }

      float directionalPingVisibility = useDirectionalPing && currentMode == Mode.Active ? 1.0f : showDirectionalIndicatorTimer;
      if (directionalPingVisibility > 0.0f)
      {
        Vector2 sector1 = MathUtils.RotatePointAroundTarget(pingDirection * DisplayRadius, Vector2.Zero, MathHelper.ToRadians(DirectionalPingSector * 0.5f));
        Vector2 sector2 = MathUtils.RotatePointAroundTarget(pingDirection * DisplayRadius, Vector2.Zero, MathHelper.ToRadians(-DirectionalPingSector * 0.5f));
        DrawLine(spriteBatch, Vector2.Zero, sector1, Color.LightCyan * 0.2f * directionalPingVisibility, width: 3);
        DrawLine(spriteBatch, Vector2.Zero, sector2, Color.LightCyan * 0.2f * directionalPingVisibility, width: 3);
      }

      if (GameMain.DebugDraw)
      {
        GUI.DrawString(spriteBatch, rect.Location.ToVector2(), sonarBlips.Count.ToString(), Color.White);
      }

      screenOverlay?.Draw(spriteBatch, center, 0.0f, rect.Width / screenOverlay.size.X);

      if (signalStrength <= 0.5f)
      {
        signalWarningText.Text = TextManager.Get(signalStrength <= 0.0f ? "SonarNoSignal" : "SonarSignalWeak");
        signalWarningText.Color = signalStrength <= 0.0f ? negativeColor : warningColor;
        signalWarningText.Visible = true;
        return;
      }
      else
      {
        signalWarningText.Visible = false;
      }

      foreach (AITarget aiTarget in AITarget.List)
      {
        if (aiTarget.InDetectable) { continue; }
        if (aiTarget.SonarLabel.IsNullOrEmpty() || aiTarget.SoundRange <= 0.0f) { continue; }

        if (Vector2.DistanceSquared(aiTarget.WorldPosition, transducerCenter) < aiTarget.SoundRange * aiTarget.SoundRange)
        {
          DrawMarker(spriteBatch,
              aiTarget.SonarLabel.Value,
              aiTarget.SonarIconIdentifier,
              aiTarget,
              aiTarget.WorldPosition, transducerCenter,
              DisplayScale, center, DisplayRadius * 0.975f);
        }
      }

      if (GameMain.GameSession == null) { return; }

      if (Level.Loaded != null)
      {
        if (Level.Loaded.StartLocation?.Type is { ShowSonarMarker: true })
        {
          DrawMarker(spriteBatch,
              Level.Loaded.StartLocation.DisplayName.Value,
              (Level.Loaded.StartOutpost != null ? "outpost" : "location").ToIdentifier(),
              "startlocation",
              Level.Loaded.StartExitPosition, transducerCenter,
              DisplayScale, center, DisplayRadius);
        }

        if (Level.Loaded is { EndLocation.Type.ShowSonarMarker: true, Type: LevelData.LevelType.LocationConnection })
        {
          DrawMarker(spriteBatch,
              Level.Loaded.EndLocation.DisplayName.Value,
              (Level.Loaded.EndOutpost != null ? "outpost" : "location").ToIdentifier(),
              "endlocation",
              Level.Loaded.EndExitPosition, transducerCenter,
              DisplayScale, center, DisplayRadius);
        }

        for (int i = 0; i < Level.Loaded.Caves.Count; i++)
        {
          var cave = Level.Loaded.Caves[i];
          if (cave.MissionsToDisplayOnSonar.None()) { continue; }
          DrawMarker(spriteBatch,
              caveLabel.Value,
              "cave".ToIdentifier(),
              "cave" + i,
              cave.StartPos.ToVector2(), transducerCenter,
              DisplayScale, center, DisplayRadius);
        }
      }

      int missionIndex = 0;
      foreach (Mission mission in GameMain.GameSession.Missions)
      {
        int i = 0;
        foreach ((LocalizedString label, Vector2 position) in mission.SonarLabels)
        {
          if (!string.IsNullOrEmpty(label.Value))
          {
            DrawMarker(spriteBatch,
                label.Value,
                mission.SonarIconIdentifier,
                "mission" + missionIndex + ":" + i,
                position, transducerCenter,
                DisplayScale, center, DisplayRadius * 0.95f);
          }
          i++;
        }
        missionIndex++;
      }

      if (HasMineralScanner && UseMineralScanner && CurrentMode == Mode.Active && MineralClusters != null &&
          (item.CurrentHull == null || !DetectSubmarineWalls))
      {
        foreach (var c in MineralClusters)
        {
          var unobtainedMinerals = c.resources.Where(i => i != null && i.GetComponent<Holdable>() is { Attached: true });
          if (unobtainedMinerals.None()) { continue; }
          if (!CheckResourceMarkerVisibility(c.center, transducerCenter)) { continue; }
          var i = unobtainedMinerals.FirstOrDefault();
          if (i == null) { continue; }

          bool disrupted = false;
          foreach ((Vector2 disruptPos, float disruptStrength) in disruptedDirections)
          {
            float dot = Vector2.Dot(Vector2.Normalize(c.center - transducerCenter), disruptPos);
            if (dot > 1.0f - disruptStrength)
            {
              disrupted = true;
              break;
            }
          }
          if (disrupted) { continue; }

          DrawMarker(spriteBatch,
              i.Name, "mineral".ToIdentifier(), "mineralcluster" + i,
              c.center, transducerCenter,
              DisplayScale, center, DisplayRadius * 0.95f,
              onlyShowTextOnMouseOver: true);
        }
      }

      foreach (Submarine sub in Submarine.Loaded)
      {
        if (!sub.ShowSonarMarker) { continue; }
        if (connectedSubs.Contains(sub)) { continue; }
        if (Level.Loaded != null && sub.WorldPosition.Y > Level.Loaded.Size.Y) { continue; }

        if (item.Submarine != null || Character.Controlled != null)
        {
          //hide enemy team
          if (sub.TeamID == CharacterTeamType.Team1 && (item.Submarine?.TeamID == CharacterTeamType.Team2 || Character.Controlled?.TeamID == CharacterTeamType.Team2))
          {
            continue;
          }
          else if (sub.TeamID == CharacterTeamType.Team2 && (item.Submarine?.TeamID == CharacterTeamType.Team1 || Character.Controlled?.TeamID == CharacterTeamType.Team1))
          {
            continue;
          }
        }

        DrawMarker(spriteBatch,
            sub.Info.DisplayName.Value,
            (sub.Info.HasTag(SubmarineTag.Shuttle) ? "shuttle" : "submarine").ToIdentifier(),
            sub,
            sub.WorldPosition, transducerCenter,
            DisplayScale, center, DisplayRadius * 0.95f);
      }

      if (GameMain.DebugDraw)
      {
        var steering = item.GetComponent<Steering>();
        steering?.DebugDrawHUD(spriteBatch, transducerCenter, DisplayScale, DisplayRadius, center);
      }
    }


  }
}