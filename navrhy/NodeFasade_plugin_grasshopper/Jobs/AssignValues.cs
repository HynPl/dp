using Rhino.Geometry;
using System.Collections.Generic;

namespace NodeFasadeG1.Jobs {
    public static class AssignValues {
        static double tol=1;//1mm
        public static List<Node> AssignValuesRun(List<Node> nodes, List<double> values, List<Surface> surfaces) {
            if (surfaces.Count!=values.Count) throw new System.Exception("count does not match");//probably not
            var _nodes=Node.CloneNodes(nodes);

            List<Node> nodesUsed=new List<Node>();

            for (int i=0; i<surfaces.Count; i++) {
                Surface s=surfaces[i];
                if (s==null) throw new System.Exception("surface is null");//probably not

                Point3d ptSurf=s.PointAt(0,0);
                Node nodeClosest=null;
                double min=1e15;

                foreach (Node node in _nodes){ 
                    if (node==null) continue;//probably not
                    double dis=ptSurf.DistanceTo(node.Pt);
                    if (min>dis){ 
                        min=dis;
                        nodeClosest=node;
                        if (min<tol) { 
                            break; 
                        }
                    }
                }
                if (nodeClosest==null) throw new System.Exception("node is null");//probably not
                nodeClosest.usedElectric=true;
                nodeClosest.Value=values[i];
                nodesUsed.Add(nodeClosest);
            }

            // isolate
            for (int i=0; i<nodesUsed.Count; i++){ 
                Node node=nodesUsed[i];                
                if (node==null) continue;//probably not
               // if (node==null)throw new System.Exception("cleenup node is null");

                if (node.Up!=null) {
                    if (!node.Up.usedElectric) {
                        node.Up=null;
                    }
                }
                if (node.Down!=null) {
                    if (!node.Down.usedElectric) {
                        node.Down=null;
                    }
                }
                if (node.Left!=null) {
                    if (!node.Left.usedElectric) {
                        node.Left=null;
                    }
                }
                if (node.Right!=null) {
                    if (!node.Right.usedElectric) {
                        node.Right=null;
                    }
                }
            }

            return nodesUsed;
        }
    }
}
