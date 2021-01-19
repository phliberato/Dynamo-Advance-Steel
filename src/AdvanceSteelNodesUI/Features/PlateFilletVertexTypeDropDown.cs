﻿using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace AdvanceSteel.Nodes
{
  [NodeName("Plate Fillet Vertex Type")]
  [NodeDescription("Lists the Advance Steel Plate Fillet Vertex type options")]
  [NodeCategory("AdvanceSteel.Nodes.Properties.Properties-Type")]
  [OutPortNames("vertexType")]
  [OutPortTypes("int")]
  [OutPortDescriptions("integer")]
  [IsDesignScriptCompatible]
  public class PlateFilletVertexType : AstDropDownBase
  {
    private const string outputName = "vertexType";

    public PlateFilletVertexType()
        : base(outputName)
    {
      InPorts.Clear();
      OutPorts.Clear();
      RegisterAllPorts();
    }

    [JsonConstructor]
    public PlateFilletVertexType(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
    : base(outputName, inPorts, outPorts)
    {
    }

    protected override SelectionState PopulateItemsCore(string currentSelection)
    {
      Items.Clear();

      var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Select Plate Corener Cut Type...", -1),
                new DynamoDropDownItem("Convex", (int)0),
                new DynamoDropDownItem("Concave", (int)1),
                new DynamoDropDownItem("Striaght", (int)2)
            };

      Items.AddRange(newItems);

      SelectedIndex = 0;
      return SelectionState.Restore;
    }

    public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
    {
      if (Items.Count == 0 ||
          Items[SelectedIndex].Name == "Select Plate Corener Cut Type..." ||
          SelectedIndex < 0)
      {
        return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
      }

      var intNode = AstFactory.BuildIntNode((int)Items[SelectedIndex].Item);
      var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);
      return new List<AssociativeNode> { assign };

    }
  }
}