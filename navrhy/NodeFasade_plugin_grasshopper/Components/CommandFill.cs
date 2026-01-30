using Grasshopper.Kernel;
using NodeFasadeG1.Jobs;
using System;

namespace MeshNodesFasade{
    public sealed class CommandFill : GH_Component {
        public CommandFill() : base(
            "CommandFill",
            "CommandFill",
            "Fills all nodes",

            "F", "Commands") {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddNumberParameter("value", "velue", "0 to 1",GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddGenericParameter("fill","fill","fill command",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="access">The IDataAccess object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            double value=0;
            DA.GetData(0, ref value);

            DA.SetData(0, new Fill(){value=value});
        }
        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c04");
    }
}