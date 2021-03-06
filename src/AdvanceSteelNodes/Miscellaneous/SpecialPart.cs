﻿using Autodesk.AdvanceSteel.CADAccess;
using Autodesk.AdvanceSteel.Geometry;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Linq;
using SteelServices = Dynamo.Applications.AdvanceSteel.Services;

namespace AdvanceSteel.Nodes.Miscellaneous
{
  /// <summary>
  /// Advance Steel Special Part
  /// </summary>
  [DynamoServices.RegisterForTrace]
  public class SpecialPart : GraphicObject
  {
    internal SpecialPart()
    {
    }

    internal SpecialPart(Matrix3d insertMatrix, string blockName, List<Property> cameraProperties)
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {

          List<Property> defaultData = cameraProperties.Where(x => x.Level == ".").ToList<Property>();
          List<Property> postWriteDBData = cameraProperties.Where(x => x.Level == "Z_PostWriteDB").ToList<Property>();

          double scale = (double)defaultData.FirstOrDefault<Property>(x => x.Name == "Scale").InternalValue;

          string handle = SteelServices.ElementBinder.GetHandleFromTrace();

          Autodesk.AdvanceSteel.Modelling.SpecialPart specPart = null;
          if (string.IsNullOrEmpty(handle) || Utils.GetObject(handle) == null)
          {
            specPart = new Autodesk.AdvanceSteel.Modelling.SpecialPart(insertMatrix);
            specPart.SetBlock(blockName, scale);
            if (defaultData != null)
            {
              Utils.SetParameters(specPart, defaultData);
            }
            specPart.WriteToDb();

            if (postWriteDBData != null)
            {
              Utils.SetParameters(specPart, postWriteDBData);
            }

          }
          else
          {
            specPart = Utils.GetObject(handle) as Autodesk.AdvanceSteel.Modelling.SpecialPart;

            if (specPart != null && specPart.IsKindOf(FilerObject.eObjectType.kSpecialPart))
            {
              specPart.SetBlock(blockName, scale);
              if (defaultData != null)
              {
                Utils.SetParameters(specPart, defaultData);
              }

              if (postWriteDBData != null)
              {
                Utils.SetParameters(specPart, postWriteDBData);
              }
            }
            else
              throw new System.Exception("Not a Special Part");
          }

          Handle = specPart.Handle;
          SteelServices.ElementBinder.CleanupAndSetElementForTrace(specPart);
        }
      }
    }

    /// <summary>
    /// Create an Advance Steel Special Part
    /// </summary>
    /// <param name="coordinateSystem">Input Dynamo Coordinate System</param>
    /// <param name="blockName"> Input Blockname to be used by SpecialPart</param>
    /// <param name="scale"> Input Special Part Scale</param>
    /// <param name="additionalSpecialPartsParameters"> Optional Input Special Part Build Properties </param>
    /// <returns name="specialPart"> specialPart</returns>
    public static SpecialPart ByCSAndBlockName(Autodesk.DesignScript.Geometry.CoordinateSystem coordinateSystem,
                              string blockName,
                              [DefaultArgument("1")] double scale,
                              [DefaultArgument("null")] List<Property> additionalSpecialPartsParameters)
    {
      Matrix3d spMatrix = Utils.ToAstMatrix3d(coordinateSystem, true);
      additionalSpecialPartsParameters = PreSetDefaults(additionalSpecialPartsParameters, scale);

      return new SpecialPart(spMatrix, blockName, additionalSpecialPartsParameters);
    }

    /// <summary>
    /// Set BlockName and Scale of Special Part
    /// </summary>
    /// <param name="steelObject">Input Dynamo Coordinate System</param>
    /// <param name="scale"> Input Special Part Scale</param>
    /// <param name="blockName"> Input Blockname to be used by SpecialPart</param>
    public static void SetBlock(SteelDbObject steelObject,
                                string blockName,
                                [DefaultArgument("1")] double scale)
    {
      using (var ctx = new SteelServices.DocContext())
      {
        string handle = steelObject.Handle;
        FilerObject obj = Utils.GetObject(handle);

        if (obj != null && obj.IsKindOf(FilerObject.eObjectType.kSpecialPart))
        {
          Autodesk.AdvanceSteel.Modelling.SpecialPart specialPart = obj as Autodesk.AdvanceSteel.Modelling.SpecialPart;
          specialPart.SetBlock(blockName, scale);
        }
        else
          throw new System.Exception("Failed to Get Special Part Object");
      }
    }

    private static List<Property> PreSetDefaults(List<Property> listSpecialPartData, double scale)
    {
      if (listSpecialPartData == null)
      {
        listSpecialPartData = new List<Property>() { };
        Utils.CheckListUpdateOrAddValue(listSpecialPartData, "Scale", scale, ".");
      }
      return listSpecialPartData;
    }


    [IsVisibleInDynamoLibrary(false)]
    public override Autodesk.DesignScript.Geometry.Curve GetDynCurve()
    {
      lock (access_obj)
      {
        using (var ctx = new SteelServices.DocContext())
        {
          var camera = Utils.GetObject(Handle) as Autodesk.AdvanceSteel.Modelling.SpecialPart;

          Matrix3d cameraCS = camera.CS;
          Vector3d xVect = null;
          Vector3d yVect = null;
          Vector3d ZVect = null;
          Point3d origin = null;
          cameraCS.GetCoordSystem(out origin, out xVect, out yVect, out ZVect);

          using (var dynPoint = Utils.ToDynPoint(origin, true))
          {
            return Autodesk.DesignScript.Geometry.Circle.ByCenterPointRadius(dynPoint, 0.01);
          }
        }
      }
    }
  }
}