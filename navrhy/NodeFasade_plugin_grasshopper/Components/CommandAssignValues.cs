using Grasshopper.Kernel;
using NodeFasadeG1.Jobs;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace MeshNodesFasade{
    public sealed class CommandAssignValues : GH_Component {
        public CommandAssignValues() : base(
            "AssignValues",
            "AssignValues",
            "Fills area of fasade",

            "F", "Basic") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddGenericParameter("nodes", "nodes", "nodes",GH_ParamAccess.list);
            pManager.AddNumberParameter("values", "values", "values",GH_ParamAccess.list);
            pManager.AddSurfaceParameter("surfaces", "surfaces", "surfaces",GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGenericParameter("nodes","nodes","nodes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="access">The IDataAccess object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            List<Node> nodes=new List<Node>();
            DA.GetDataList(0, nodes);

            List<double> values=new List<double>();
            DA.GetDataList(1, values);

            List<Surface> surfaces=new List<Surface>();
            DA.GetDataList(2, surfaces);

            List<Node> nodesE =AssignValues.AssignValuesRun(nodes, values, surfaces);
            DA.SetDataList(0, nodesE);
        }
        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c10");
    }
}