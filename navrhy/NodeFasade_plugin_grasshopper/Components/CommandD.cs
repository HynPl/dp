using Grasshopper.Kernel;
using NodeFasadeG1.Jobs;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace MeshNodesFasade{
    public sealed class FloydSteinbergDither : GH_Component {
        public FloydSteinbergDither() : base(
            "FloydSteinbergDither",
            "FloydSteinbergDither",
            "Floyd Steinberg Dither",

            "F", "Basic") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddGenericParameter("nodes", "nodes", "nodes",GH_ParamAccess.list);
            pManager.AddNumberParameter("pivot", "pivot", "pivot",GH_ParamAccess.item);
         //   pManager.AddNumberParameter("type", "type", "type",GH_ParamAccess.item);
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

            double pivot=0;
            DA.GetData(1, ref pivot);

          // double type=0;
          //  DA.GetData(2, ref type);
            
            List<Node> _nodes;
           // if (type==0){
                _nodes=NodeFasadeG1.Jobs.FloydSteinbergDither.FloydSteinbergDitherJob(nodes, pivot);
           // }else{ 
              //  _nodes=NodeFasadeG1.Jobs.FloydSteinbergDither.Matrix8Job(nodes, pivot, (int)type, 4);
          //  }
            DA.SetDataList(0, _nodes);
        }
        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c11");
    }
}