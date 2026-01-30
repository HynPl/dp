using Grasshopper;
using Grasshopper.Kernel;
using NodeFasadeG1.Jobs;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace MeshNodesFasade{
    public sealed class Electric : GH_Component {
        public Electric() : base(
            "Electric",
            "Electric",
            "Creates linked nodes from points with different z",

            "F","Basic") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
             pManager.AddGenericParameter("nodes", "nodes", "nodes", GH_ParamAccess.list);
             pManager.AddCurveParameter("bottom_curves", "bottom_curves", "bottom_curves", GH_ParamAccess.list);
             pManager.AddNumberParameter("seed", "seed", "seed", GH_ParamAccess.item);
             pManager.AddNumberParameter("side", "side", "side", GH_ParamAccess.item);
             pManager.AddNumberParameter("fulls", "fulls", "fulls", GH_ParamAccess.item);
             pManager.AddNumberParameter("tol", "tol", "tol", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddPointParameter("Polylines", "Polylines", "Polylines", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="access">The IDataAccess object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            List<Node> nodes=new List<Node>();
            DA.GetDataList(0, nodes);

            List<Curve> bottoms=new List<Curve>();
            DA.GetDataList(1, bottoms);

            double seed=0;
            DA.GetData(2, ref seed);

            double side=0;
            DA.GetData(3, ref side);

            double fulls=0;
            DA.GetData(4, ref fulls);

            double tol=0;
            DA.GetData(5, ref tol);

            NodeFasadeG1.Jobs.ElectricGeometry.RunScript(nodes, bottoms, (int)seed, out DataTree<Point3d> geometry, fulls, side);

            DA.SetDataTree(0, geometry);
        }

        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c09");
    }
}