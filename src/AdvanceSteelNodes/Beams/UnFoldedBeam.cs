﻿using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.AdvanceSteel.Modelling;
using Autodesk.AdvanceSteel.Profiles;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;
using System.Linq;

namespace AdvanceSteel.Nodes.Beams
{
  /// <summary>
  /// Advance Steel Unfolded Straight Beams
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class UnFoldedBeam : GraphicObject
  {

    internal UnFoldedBeam()
    {
    }

    internal UnFoldedBeam(Polyline3d poly,
                          Autodesk.DesignScript.Geometry.Point ptStart,
                          Autodesk.DesignScript.Geometry.Point ptEnd,
                          Autodesk.DesignScript.Geometry.Vector vOrientation,
                          List<Property> beamProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {

          List<Property> defaultData = beamProperties.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = beamProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();
          Property foundThickness = beamProperties.FirstOrDefault<Property>(x => x.Name == "Thickness");
          double thickness = (double)foundThickness.InternalValue;

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Point3d beamStart = Utils.ToAstPoint(ptStart, true);
          Point3d beamEnd = Utils.ToAstPoint(ptEnd, true);
          Vector3d refVect = Utils.ToAstVector3d(vOrientation, true);

          Autodesk.AdvanceSteel.Modelling.UnfoldedStraightBeam beam = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            beam = new Autodesk.AdvanceSteel.Modelling.UnfoldedStraightBeam(poly, beamStart, beamEnd, refVect, thickness);

            if (defaultData != null)
            {
              Utils.SetParameters(beam, defaultData);
            }

            beam.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(beam, postWriteDBData);
            }
          }
          else
          {
            beam = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.UnfoldedStraightBeam;

            if (beam != null && beam.IsKindOf(FilerObject.eObjectType.kUnfoldedStraightBeam))
            {
              Utils.AdjustBeamEnd(beam, beamStart);
              beam.SetSysStart(beamStart);
              beam.SetSysEnd(beamEnd);

              if (defaultData != null)
              {
                Utils.SetParameters(beam, defaultData);
              }

              Utils.SetOrientation(beam, refVect);

              if (postWriteDBData != null)
              {
                Utils.SetParameters(beam, postWriteDBData);
              }
            }
            else
              throw new System.Exception("Not an UnFolded Straight Beam");
          }
          Handle = beam.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(beam);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Unfolded Beam between two points from three points to make an arc
    /// </summary>
    /// <param name="startPointCurve"> Input Start Point of Arc</param>
    /// <param name="pointOnCurve"> Input Point along Arc</param>
    /// <param name="endPointCurve"> Input End Point of Arc</param>
    /// <param name="orientation"> Input reference vector of arc</param>
    /// <param name="startPoint"> Input Start Point of Unfolded Beam</param>
    /// <param name="endPoint"> Input End Point of Unfolded Beam</param>
    /// <param name="thickness">  Input thickness of Unfolded Beam</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns name="unFoldedBeam"> beam</returns>
    public static UnFoldedBeam ByThreePointArc(Autodesk.DesignScript.Geometry.Point startPointCurve,
                                                    Autodesk.DesignScript.Geometry.Point pointOnCurve,
                                                    Autodesk.DesignScript.Geometry.Point endPointCurve,
                                                    Autodesk.DesignScript.Geometry.Vector orientation,
                                                    Autodesk.DesignScript.Geometry.Point startPoint,
                                                    Autodesk.DesignScript.Geometry.Point endPoint,
                                                    double thickness,
                                                    [DefaultArgument("null")] List<Property> additionalBeamParameters)
    {
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters, Utils.ToInternalDistanceUnits(thickness, true));
      CircArc3d cc = new CircArc3d(Utils.ToAstPoint(startPointCurve, true),
                                    Utils.ToAstPoint(pointOnCurve, true),
                                    Utils.ToAstPoint(endPointCurve, true));
      Polyline3d poly = new Polyline3d();
      poly = cc.GetPolyline3d();
      if (poly == null)
        throw new System.Exception("No Valid Poly");
      return new UnFoldedBeam(poly, startPoint, endPoint, orientation, additionalBeamParameters);
    }

    /// <summary>
    /// Create an Advance Steel Unfolded Beam between two points from an arc
    /// </summary>
    /// <param name="arc"></param>
    /// <param name="orientation"> Input reference vector of arc</param>
    /// <param name="startPoint"> Input Start Point of Unfolded Beam</param>
    /// <param name="endPoint"> Input End Point of Unfolded Beam</param>
    /// <param name="thickness">  Input thickness of Unfolded Beam</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns name="unFoldedBeam"> beam</returns>
    public static UnFoldedBeam ByArc(Autodesk.DesignScript.Geometry.Arc arc,
                                                Autodesk.DesignScript.Geometry.Vector orientation,
                                                Autodesk.DesignScript.Geometry.Point startPoint,
                                                Autodesk.DesignScript.Geometry.Point endPoint,
                                                double thickness,
                                                [DefaultArgument("null")] List<Property> additionalBeamParameters)
    {
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters, Utils.ToInternalDistanceUnits(thickness, true));
      CircArc3d cc = new CircArc3d(Utils.ToAstPoint(arc.StartPoint, true),
                                    Utils.ToAstPoint(arc.PointAtSegmentLength(arc.Length / 2), true),
                                    Utils.ToAstPoint(arc.EndPoint, true));
      Polyline3d poly = new Polyline3d();
      poly = cc.GetPolyline3d();
      if (poly == null)
        throw new System.Exception("No Valid Poly");
      return new UnFoldedBeam(poly, startPoint, endPoint, orientation, additionalBeamParameters);
    }

    /// <summary>
    /// Create an Advance Steel Unfolded Beam between two points from a polycurve
    /// </summary>
    /// <param name="polyCurve"></param>
    /// <param name="orientation"> Input reference vector of arc</param>
    /// <param name="startPoint"> Input Start Point of Unfolded Beam</param>
    /// <param name="endPoint"> Input End Point of Unfolded Beam</param>
    /// <param name="thickness">  Input thickness of Unfolded Beam</param>
    /// <param name="additionalBeamParameters"> Optional Input Beam Build Properties </param>
    /// <returns name="unFoldedBeam"> beam</returns>
    public static UnFoldedBeam ByPolyCurve(Autodesk.DesignScript.Geometry.PolyCurve polyCurve,
                                            Autodesk.DesignScript.Geometry.Vector orientation,
                                            Autodesk.DesignScript.Geometry.Point startPoint,
                                            Autodesk.DesignScript.Geometry.Point endPoint,
                                            double thickness,
                                            [DefaultArgument("null")] List<Property> additionalBeamParameters)
    {
      additionalBeamParameters = PreSetDefaults(additionalBeamParameters, Utils.ToInternalDistanceUnits(thickness, true));
      Polyline3d poly = Utils.ToAstPolyline3d(polyCurve, true);
      if (poly == null)
        throw new System.Exception("No Valid Poly");
      return new UnFoldedBeam(poly, startPoint, endPoint, orientation, additionalBeamParameters);
    }


    /// <summary>
    /// Return True or False depending if the UnfoldedBeam is Closed or not.
    /// </summary>
    /// <param name="unFoldedBeam">Input beam</param>
    /// <returns name="isClosed">True or False depending if the UnfoldedBeam is Closed or not</returns>
    public static bool IsClosed(UnFoldedBeam unFoldedBeam)
    {
      bool ret;
      using (var ctx = new SteelServices.DocContext())
      {
        if (unFoldedBeam != null)
        {
          FilerObject filerObj = Utils.GetObject(unFoldedBeam.Handle);
          if (filerObj != null)
          {
            if (filerObj.IsKindOf(FilerObject.eObjectType.kUnfoldedStraightBeam))
            {
              Autodesk.AdvanceSteel.Modelling.UnfoldedStraightBeam selectedObj = filerObj as Autodesk.AdvanceSteel.Modelling.UnfoldedStraightBeam;
              ret = (bool)selectedObj.IsClosed();
            }
            else
              throw new System.Exception("Not an Unfolded Beam Object");
          }
          else
            throw new System.Exception("AS Object is null");
        }
        else
          throw new System.Exception("Steel Object or Point is null");
      }
      return ret;
    }

    private static List<Property> PreSetDefaults(List<Property> listBeamData, double thickness)
    {
      if (listBeamData == null)
      {
        listBeamData = new List<Property>() { };
      }
      Utils.CheckListUpdateOrAddValue(listBeamData, "Thickness", thickness);
      return listBeamData;
    }

    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var beam = Utils.GetObject(Handle) as Beam;

          Point3d asPt1 = beam.GetPointAtStart(0);
          Point3d asPt2 = beam.GetPointAtEnd(0);

          using (var pt1 = Utils.ToDynPoint(beam.GetPointAtStart(0), true))
          using (var pt2 = Utils.ToDynPoint(beam.GetPointAtEnd(0), true))
          {
            var line = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(pt1, pt2);
            return line;
          }
        }
      }
    }
  }
}