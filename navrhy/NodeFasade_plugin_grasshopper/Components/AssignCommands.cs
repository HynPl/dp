using Grasshopper.Kernel;
using NodeFasadeG1.Jobs;
using System;
using System.Collections.Generic;

namespace NodeFasadeG1{
    public class AssignCommands : GH_Component {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public AssignCommands(): base(
            "AssignCommands", 
            "AssignCommands",
            "Set command on the fasade",

            "F", "Basic") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddGenericParameter("nodes", "nodes", "pivot", GH_ParamAccess.list);
            pManager.AddGenericParameter("commands", "commands", "distance between curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
             pManager.AddGenericParameter("nodes", "nodes", "node list", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            List<Node> nodes=new List<Node>();
            DA.GetDataList(0, nodes);

            List<Command> commands=new List<Command>();
            DA.GetDataList(1, commands);

            List<Node> _nodes=new List<Node>();
            AssignCommandsToNodes.RunScript(nodes, commands, ref _nodes);

            DA.SetDataList(0, _nodes);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        //protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c01");
    }
}