using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace NodeFasadeG1{
    public class NodeFasadeG1Info : GH_AssemblyInfo {
        public override string Name => "NodeFasade";

        //Return a 24x24 pixel bitmap to represent this GHA library.
       // public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("05682a78-01f0-4588-bb18-0418b70f88a9");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";

        //Return a string representing the version.  This returns the same version as the assembly.
        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
}