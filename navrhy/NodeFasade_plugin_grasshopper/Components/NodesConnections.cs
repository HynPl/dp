using Grasshopper.Kernel;
using NodeFasadeG1.Jobs;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace MeshNodesFasade{
    public sealed class NodesConnections : GH_Component {
        public NodesConnections() : base(
            "NodesConnections",
            "NodesConnections",
            "Creates linked nodes from points with different z",

            "F","Basic") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
             pManager.AddGenericParameter("nodes", "nodes", "nodes", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddCurveParameter("lines", "lines", "lines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="access">The IDataAccess object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            List<Node> nodes=new List<Node>();
            DA.GetDataList(0, nodes);

            List<Line> lines=new List<Line>();
            foreach(Node node in nodes) {
                if (node.Up!=null) lines.Add(new Line(node.Pt,node.Up.Pt));
                if (node.Down!=null) lines.Add(new Line(node.Pt,node.Down.Pt));
                if (node.Left!=null) lines.Add(new Line(node.Pt,node.Left.Pt));
                if (node.Right!=null) lines.Add(new Line(node.Pt,node.Right.Pt));
            }


            DA.SetDataList(0, lines);
        }

        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c08");
    }
}