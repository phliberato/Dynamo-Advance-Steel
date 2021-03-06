﻿using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.ConstructionTypes;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modeler;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Util
{
  /// <summary>
  /// Geometric functions to work with Beams - Straight Beams, Bend Beams....
  /// </summary>
  public class Beam
  {

    internal Beam()
    {
    }

    /// <summary>
    /// Set Advance Steel Beam Insert Reference Axis
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="refAxis"> Input Beam reference axis UpperLeft = 0, UpperSys = 1, UpperRight = 2, MidLeft = 3, SysSys = 4, MidRight = 5, LowerLeft = 6, LowerSys = 7, LowerRight = 8, ContourCenter = 9</param>
    public static void SetBeamReferenceAxis(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                            int refAxis)
    {
      if (Enum.IsDefined(typeof(Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis), refAxis) == false)
        throw new System.Exception("Invalid Reference axis");

      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              selectedObj.RefAxis = (Autodesk.AdvanceSteel.Modelling.Beam.eRefAxis)refAxis;
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object is null");
      }
    }

    /// <summary>
    /// Set Beam Cross Section to be Mirrored - i.e. Channels with toe pointing in the opposite direction
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="crossSectionMirrored"> Input True or False Value</param>
    public static void SetBeamCrossSectionMirrored(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                        [DefaultArgument("true")] bool crossSectionMirrored)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              selectedObj.SetCrossSectionMirrored(crossSectionMirrored);
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object is null");
      }
    }

    /// <summary>
    /// Set Beam System Line Start Point
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="point"> Input Dynamo Point</param>
    public static void SetSystemLineStartPoint(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                    Autodesk.DesignScript.Geometry.Point point)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              selectedObj.SetSysStart(Utils.ToAstPoint(point, true));
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object is null");
      }
    }

    /// <summary>
    /// Set Beam System Line End Point
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="point"> Input Dynamo Point</param>
    public static void SetSystemLineEndPoint(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                    Autodesk.DesignScript.Geometry.Point point)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              selectedObj.SetSysEnd(Utils.ToAstPoint(point, true));
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object is null");
      }
    }

    /// <summary>
    /// Get closest point on the system line relative to a point
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="pointOnSystemLine"> Dynamo point</param>
    /// <param name="unBounded">TRUE = Ignore ends of system line, FALSE = (Default) Use physical length of System line as limitation</param>
    /// <returns name="point"> closest point of the system line either restricted or not the physical system line</returns>
    public static Autodesk.DesignScript.Geometry.Point GetClosestPointToSystemline(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                                                  Autodesk.DesignScript.Geometry.Point pointOnSystemLine,
                                                                                  [DefaultArgument("False")] bool unBounded)
    {
      Autodesk.DesignScript.Geometry.Point ret = Autodesk.DesignScript.Geometry.Point.ByCoordinates(0, 0, 0);
      Point3d point = Utils.ToAstPoint(pointOnSystemLine, true);
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null || point != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              Point3d foundPoint = selectedObj.GetClosestPointToSystemline(point, unBounded);
              if (foundPoint != null)
              {
                ret = Utils.ToDynPoint(foundPoint, true);
              }
              else
                throw new System.Exception("No Point was returned from Function");
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object is null");
      }
      return ret;
    }

    /// <summary>
    /// Return Dynamo Line object from the Beam System Line. Supports Straight Beam, Tapered Beam, Unfolded Beam or Compound Beam
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <returns name="line"> beam system line as line object</returns>
    public static Autodesk.DesignScript.Geometry.Line GetBeamLine(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      Autodesk.DesignScript.Geometry.Line ret = null;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kStraightBeam) ||
                filerObj.IsKindOf(FilerObject.eObjectType.kBeamTapered) ||
                filerObj.IsKindOf(FilerObject.eObjectType.kUnfoldedStraightBeam) ||
                filerObj.IsKindOf(FilerObject.eObjectType.kCompoundStraightBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              Point3d foundStartPoint = selectedObj.GetPointAtStart();
              Point3d foundEndPoint = selectedObj.GetPointAtEnd();
              if (foundStartPoint != null || foundEndPoint != null)
              {
                ret = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(Utils.ToDynPoint(foundStartPoint, true), Utils.ToDynPoint(foundEndPoint, true));
              }
              else
                throw new System.Exception("No Points wer returned from the Object");
            }
            else
              throw new System.Exception("Not a Straight Beam, Tapered Beam, Unfolded Beam or Compound Beam Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object is null");
      }
      return ret;
    }

    /// <summary>
    /// Get Point at a distance from the END of the Beam
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="distance"> Distance from end point</param>
    /// <returns name="point"> from beam object end point return a point by the distance value</returns>
    public static Autodesk.DesignScript.Geometry.Point GetPointFromEnd(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                                       [DefaultArgument("0")] double distance)
    {
      Autodesk.DesignScript.Geometry.Point ret = Autodesk.DesignScript.Geometry.Point.ByCoordinates(0, 0, 0);
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              Point3d foundPoint = selectedObj.GetPointAtEnd(distance);
              if (foundPoint != null)
              {
                ret = Utils.ToDynPoint(foundPoint, true);
              }
              else
                throw new System.Exception("No Point was returned from Function");
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object is null");
      }
      return ret;
    }

    /// <summary>
    /// Get Point at a distance from the START of the Beam
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <param name="distance"> Distance from start point</param>
    /// <returns name="point"> from beam object start point return a point by the distance value</returns>
    public static Autodesk.DesignScript.Geometry.Point GetPointFromStart(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                                        [DefaultArgument("0")] double distance)
    {
      Autodesk.DesignScript.Geometry.Point ret = Autodesk.DesignScript.Geometry.Point.ByCoordinates(0, 0, 0);
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              Point3d foundPoint = selectedObj.GetPointAtStart(distance);
              if (foundPoint != null)
              {
                ret = Utils.ToDynPoint(foundPoint, true);
              }
              else
                throw new System.Exception("No Point was returned from Function");
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object is null");
      }
      return ret;
    }

    /// <summary>
    /// Get Coordinate System at point on System line
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="pointOnSystemLine"> Dynamo Point on System line</param>
    /// <returns name="coordinateSystem"> return coordinate system on beam at a specifc point</returns>
    public static Autodesk.DesignScript.Geometry.CoordinateSystem GetCoordinateSystemAtPoint(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                                                              Autodesk.DesignScript.Geometry.Point pointOnSystemLine)
    {
      Autodesk.DesignScript.Geometry.CoordinateSystem ret = Autodesk.DesignScript.Geometry.CoordinateSystem.ByOrigin(0, 0, 0);
      Point3d point = Utils.ToAstPoint(pointOnSystemLine, true);
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null || point != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              Matrix3d cs = selectedObj.GetCSAtPoint(point);
              if (cs != null)
              {
                ret = Utils.ToDynCoordinateSys(cs, true);
              }
              else
                throw new System.Exception("Not Cordinate System was returned from Point");
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return ret;
    }

    /// <summary>
    /// Get Saw Cut information from Beam Objects
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <returns name="beamSawCutInformation"> The Saw Length, Flange Angle At Start, Web Angle At Start, Flange Angle At End and Web Angle At End </returns>
    [MultiReturn(new[] { "SawLength", "FlangeAngleAtStart", "WebAngleAtStart", "FlangeAngleAtEnd", "WebAngleAtEnd" })]
    public static Dictionary<string, double> GetBeamSawInformation(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      Dictionary<string, double> ret = new Dictionary<string, double>();

      double sawLength = 0;
      double flangeAngleAtStart = 0;
      double webAngleAtStart = 0;
      double flangeAngleAtEnd = 0;
      double webAngleAtEnd = 0;
      ret.Add("SawLength", Utils.FromInternalDistanceUnits(sawLength, true));
      ret.Add("FlangeAngleAtStart", flangeAngleAtStart);
      ret.Add("WebAngleAtStart", webAngleAtStart);
      ret.Add("FlangeAngleAtEnd", flangeAngleAtEnd);
      ret.Add("WebAngleAtEnd", webAngleAtEnd);

      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              int executed = selectedObj.GetSawInformation(out sawLength,
                                                          out flangeAngleAtStart,
                                                          out webAngleAtStart,
                                                          out flangeAngleAtEnd,
                                                          out webAngleAtEnd);
              if (executed > 0)
              {
                ret["SawLength"] = Utils.FromInternalDistanceUnits(sawLength, true);
                ret["FlangeAngleAtStart"] = Utils.FromInternalAngleUnits(flangeAngleAtStart, true);
                ret["WebAngleAtStart"] = Utils.FromInternalAngleUnits(webAngleAtStart, true);
                ret["FlangeAngleAtEnd"] = Utils.FromInternalAngleUnits(flangeAngleAtEnd, true);
                ret["WebAngleAtEnd"] = Utils.FromInternalAngleUnits(webAngleAtEnd, true);
              }
              else
                throw new System.Exception("No Values were found for Steel Beam from Function");
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object is null");
      }
      return ret;
    }

    /// <summary>
    /// Get BEAM data
    /// </summary>
    /// <param name="steelObject"> Advance Steel element</param>
    /// <param name="bodyResolutionForLength"> Set Steel body display resolution</param>
    /// <returns name="beamData"> The beam Length, PaintArea, Exact Weight, Weight Per Unit, Profile Type and Profile Type Code</returns>
    [MultiReturn(new[] { "Length", "PaintArea", "ExactWeight", "WeightPerUnit", "ProfileType", "ProfileTypeCode" })]
    public static Dictionary<string, object> GetBeamData(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                                         [DefaultArgument("0")] int bodyResolutionForLength)
    {
      Dictionary<string, object> ret = new Dictionary<string, object>();

      double length = 0;
      double paintArea = 0;
      double weight = 0;
      double weightPerUnit = 0;
      int profileType = 0;
      string profileTypeCode = "No Code";
      ret.Add("Length", length);
      ret.Add("PaintArea", paintArea);
      ret.Add("ExactWeight", weight);
      ret.Add("WeightPerUnit", weightPerUnit);
      ret.Add("ProfileType", profileType);
      ret.Add("ProfileTypeCode", profileTypeCode);

      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              length = selectedObj.GetLength((BodyContext.eBodyContext)bodyResolutionForLength);
              paintArea = selectedObj.GetPaintArea();
              weight = selectedObj.GetWeight(2);
              weightPerUnit = selectedObj.GetWeightPerMeter();
              profileTypeCode = selectedObj.GetProfType().GetDSTVValues().GetProfileTypeString();
              profileType = (int)selectedObj.GetProfType().GetDSTVValues().DSTVType;
              ret["Length"] = Utils.FromInternalDistanceUnits(length, true);
              ret["PaintArea"] = Utils.FromInternalAreaUnits(paintArea, true);
              ret["ExactWeight"] = Utils.FromInternalWeightUnits(weight, true);
              ret["WeightPerUnit"] = Utils.FromInternalWeightPerDistanceUnits(weightPerUnit, true);
              ret["ProfileType"] = profileType;
              ret["ProfileTypeCode"] = profileTypeCode;
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("No AS Steel Object is null");
        }
        else
          throw new System.Exception("No Steel Object or Point is null");
      }
      return ret;
    }

    /// <summary>
    /// Get Beam Insert Reference Axis
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns name="beamReferenceAxis"> Integer value for beam reference axis</returns>
    public static int GetBeamReferenceAxis(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      int ret = -1;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              ret = (int)selectedObj.RefAxis;
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return ret;
    }

    /// <summary>
    /// Get Beam Length
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns name="beamLenth"> The beam length value</returns>
    public static double GetLength(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      double ret = 0;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              ret = (double)selectedObj.GetLength();
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return Utils.FromInternalDistanceUnits(ret, true);
    }

    /// <summary>
    /// Get Beam Length relative to object display
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <param name="bodyResolutionForLength"> Set Steel body display resolution</param>
    /// <returns name="beamLength"> The beam length value based on a particular beam display mode / resolution</returns>
    public static double GetLength(AdvanceSteel.Nodes.SteelDbObject steelObject,
                                   [DefaultArgument("0")] int bodyResolutionForLength)
    {
      double ret = 0;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              ret = (double)selectedObj.GetLength((BodyContext.eBodyContext)bodyResolutionForLength);
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return Utils.FromInternalDistanceUnits(ret, true);
    }

    /// <summary>
    /// Get Beam Weight Per Meter
    /// </summary>
    /// <param name="steelObject">Advance Steel element</param>
    /// <returns name="beamWeightPerMeter"> The beam weight per meter</returns>
    public static double GetWeightPerMeter(AdvanceSteel.Nodes.SteelDbObject steelObject)
    {
      double ret = 0;
      using (var ctx = new SteelServices.DocContext())
      {
        if (steelObject != null)
        {
          FilerObject filerObj = Utils.GetObject(steelObject.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kBeam))
            {
              Autodesk.AdvanceSteel.Modelling.Beam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.Beam;
              ret = (double)selectedObj.GetWeightPerMeter();
            }
            else
              throw new System.Exception("Not a BEAM Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return Utils.FromInternalWeightPerDistanceUnits(ret, true);
    }

    /// <summary>
    /// This node can set the Section for Advance Steel beams from Dynamo.
    /// For the Section use the following format: "HEA  DIN18800-1#@§@#HEA100"
    /// </summary>
    /// <param name="beamElement">Advance Steel beam</param>
    /// <param name="sectionName">Section</param>
    public static void SetSection(AdvanceSteel.Nodes.SteelDbObject beamElement, string sectionName)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = beamElement.Handle;

        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kBeam))
        {

          string sectionType = Utils.SplitSectionName(sectionName)[0];
          string sectionSize = Utils.SplitSectionName(sectionName)[1];

          Autodesk.AdvanceSteel.Modelling.Beam beam = obj as Autodesk.AdvanceSteel.Modelling.Beam;
          if (obj.IsKindOf(FilerObject.eObjectType.kCompoundBeam) && !Utils.CompareCompoundSectionTypes(beam.ProfSectionType, sectionType))
          {
            throw new System.Exception("Failed to change section as compound section type is different");
          }
          beam.ChangeProfile(sectionType, sectionSize);
        }
        else
          throw new System.Exception("Failed to change section");
      }
    }
    /// <summary>
    /// Returns a concatenated string containing the SectionType, a fixed string separator "#@§@#" and the SectionSize.
    /// </summary>
    /// <param name="sectionType">SectionType for a beam section</param>
    /// <param name="sectionSize">SectionSize for a beam section</param>
    /// <returns name="sectionName">Beam section name</returns>
    public static string CreateSectionString(string sectionType, string sectionSize)
    {
      return sectionType + Utils.Separator + sectionSize;
    }

  }
}