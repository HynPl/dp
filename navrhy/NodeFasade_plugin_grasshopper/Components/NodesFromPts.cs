using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace MeshNodesFasade{
    public sealed class NodesFromPoints : GH_Component {
        public NodesFromPoints() : base(
            "NodesFromPoints",
            "NodesFromPoints",
            "Creates linked nodes from points with different z",

            "F","Basic") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
             pManager.AddPointParameter("points", "points", "points", GH_ParamAccess.list);
             pManager.AddNumberParameter("len", "len", "len", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGenericParameter("nodes", "nodes", "nodes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="access">The IDataAccess object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            List<Point3d> points=new List<Point3d>();
            DA.GetDataList(0, points);

            double len=0;
            DA.GetData(1, ref len);

            NodeFasadeG1.Jobs.NodesFromPoints.RunScript(points, len, out List<NodeFasadeG1.Jobs.Node> nodes);

            DA.SetDataList(0, nodes);
        }

        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c07");
    }
}