using Grasshopper.Kernel;
using NodeFasadeG1.Jobs;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MeshNodesFasade{
    public sealed class CommandAssignGeometry : GH_Component {
        public CommandAssignGeometry() : base(
            "AssignGeometry",
            "AssignGeometry",
            "Fills area of fasade",

            "F", "Basic") {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddGenericParameter("nodes", "nodes", "nodes",GH_ParamAccess.list);
            pManager.AddGenericParameter("surfaces", "surfaces", "surfaces",GH_ParamAccess.list);
            pManager.AddNumberParameter("values", "values", "values",GH_ParamAccess.list);
            pManager.AddNumberParameter("tol", "tol", "tol",GH_ParamAccess.item);
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

            List<object> surfaces=new List<object>();
            DA.GetDataList(1, surfaces);

            List<double> values=new List<double>();
            DA.GetDataList(2, values);

            double tol=1;
            DA.GetData(3, ref tol);

            List<Node> nonnull=new List<Node>();
            if (values.Count==surfaces.Count){
                List<Node> _nodes =Node.CloneNodes(nodes);

                if (tol<0){
                    // by closest
                    for (int i=0; i<_nodes.Count; i++){ 
                        Node node=_nodes[i];
                        if (node==null) continue;

                        double mindis=1e10;
                        int closestIndex = -1;
                        for (int s=0; s<surfaces.Count; s++){
                            object surf=surfaces[s]; 
                            if (surf==null) continue;

                            Point3d p0;
                            if (surf is Surface suface){ 
                                p0=suface.PointAt(0,0);
                            }else if (surf is Grasshopper.Kernel.Types.GH_Surface surface){ 
                                p0=surface.Face.PointAt(0,0);
                            }else throw new Exception("unknown type"+surf.GetType());

                            double dis=Distance2(node.Pt, p0);
                            if (dis<mindis){
                                closestIndex=s;
                            }
                        }
                
                        if (node.Geometry!=null) {
                            node.Value=values[closestIndex];
                            node.Geometry=surfaces[closestIndex];
                            nonnull.Add(node);
                        }
                    }
                }else{
                    double tol2=tol*tol;
                    for (int i=0; i<_nodes.Count; i++){ 
                        Node node=_nodes[i];
                        if (node==null) continue;

                        int use=-1;
                        for (int s=0; s<surfaces.Count; s++){
                            object surf=surfaces[s]; 
                            if (surf==null) continue;
                            Point3d p0;
                            if (surf is Surface suface){ 
                                p0=suface.PointAt(0,0);
                            }else if (surf is Grasshopper.Kernel.Types.GH_Surface surface){ 
                                p0=surface.Face.PointAt(0,0);
                            }else throw new Exception("unknown type"+surf.GetType());
                            
                            double dis2=Distance2(node.Pt, p0);
                            if (dis2<tol2){
                                use = s;
                                break;    
                            }
                        }
                
                        if (use>=0) {
                            node.Value=values[use];
                            node.Geometry=surfaces[use];
                            nonnull.Add(node);
                        }
                    }
                }
            }

            //  List<Node> nodesE =AssignValues.AssignValuesRun(nodes, values, surfaces);
            DA.SetDataList(0, nonnull);
        }
        public override Guid ComponentGuid => new Guid("4274982b-81a2-4d32-b4a6-5a7d9f070c12");

        public static double Distance2(Point3d p1, Point3d p2){ 
            double dX=p1.X-p2.X, dY=p1.Y-p2.Y, dZ=p1.Z-p2.Z; 
            return dX*dX + dY*dY + dZ*dZ;
        }
        public static double Distance(Point3d p1, Point3d p2){ 
            double dX=p1.X-p2.X, dY=p1.Y-p2.Y, dZ=p1.Z-p2.Z; 
            return Math.Sqrt(dX*dX + dY*dY + dZ*dZ);
        }
    }
}