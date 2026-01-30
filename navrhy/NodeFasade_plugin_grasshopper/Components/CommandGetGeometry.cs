using Grasshopper.Kernel;
using NodeFasadeG1.Jobs;
using System;
using System.Collections.Generic;

namespace MeshNodesFasade{
    public sealed class CommandetGeometry : GH_Component {
        public CommandetGeometry() : base(
            "GetGeometryAndValues",
            "GetGeometryAndValues",
            "get assigned geometries and values of elements",

            "F", "Basic") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddGenericParameter("nodes", "nodes", "nodes",GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGenericParameter("surfaces","surfaces","surfaces", GH_ParamAccess.list);
            pManager.AddNumberParameter("values","values","values", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="access">The IDataAccess object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            List<Node> nodes=new List<Node>();
            DA.GetDataList(0, nodes);

            double[] values=new double[nodes.Count];
            object[] surfaces=new object[nodes.Count]; 
            for (int i=0; i<nodes.Count; i++){ 
                values[i]= nodes[i].Value;
                surfaces[i]=nodes[i].Geometry;              
            }

            DA.SetDataList(0, surfaces);
            DA.SetDataList(1, values);
        }
        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c13");
    }
}




