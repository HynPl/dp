using Grasshopper.Kernel;
using NodeFasadeG1.Jobs;
using Rhino.Geometry;
using System;

namespace MeshNodesFasade{
    public sealed class CommandText : GH_Component {
        public CommandText() : base(
            "TextCommand",
            "TextCommand",
            "Writes text on nodes",

            "F", "Commands") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddGenericParameter("text", "text", "text", GH_ParamAccess.item);
            pManager.AddBooleanParameter("inversed", "inversed", "inversed", GH_ParamAccess.item);
            pManager.AddPointParameter("offset", "offset", "offset", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGenericParameter("CommandText","CommandText","CommandText",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="access">The IDataAccess object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {     
            string text="";
            DA.GetData(0, ref text);

            bool inversed=false;
            DA.GetData(1, ref inversed);

            Point3d offset=Point3d.Unset;
            DA.GetData(2, ref offset);

            DA.SetData(0, new Text(){text=text, inversed=inversed, offset=offset});
        }
        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c03");
    }
}