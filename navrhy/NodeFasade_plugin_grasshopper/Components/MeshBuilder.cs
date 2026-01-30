using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace MeshNodesFasade{
    public sealed class MeshBuilder : GH_Component {
        public MeshBuilder() : base(
            "MeshBuilder",
            "MeshBuilder",
            "Creates mesh from nodes",

            "F", "Basic") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddGenericParameter("nodes","nodes","nodes",GH_ParamAccess.list);
            pManager.AddNumberParameter("type","type","1 or 2",GH_ParamAccess.item);
            pManager.AddNumberParameter("len","len","len",GH_ParamAccess.item);
            pManager.AddNumberParameter("divide","divide","divide",GH_ParamAccess.item);
            pManager.AddNumberParameter("border","border","border",GH_ParamAccess.item);
            pManager.AddNumberParameter("spacing","spacing","spacing",GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddMeshParameter("mesh","mesh","mesh",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="access">The IDataAccess object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            
            List<NodeFasadeG1.Jobs.Node> nodes=new List<NodeFasadeG1.Jobs.Node>();
            DA.GetDataList(0, nodes);

            double type=1;
            DA.GetData(1, ref type);

            double len=0;
            DA.GetData(2, ref len);

            double divide=1;
            DA.GetData(3, ref divide);

            double border=0.1;
            DA.GetData(4, ref border);

            double spacing=0.1;
            DA.GetData(5, ref spacing);

            NodeFasadeG1.Jobs.MeshBuilder.RunScript(nodes, (int)type, out Mesh mesh, (int)divide, border, spacing);

            DA.SetData(0, mesh);
        }
        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c06");
    }
}