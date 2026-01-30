using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodeFasadeG1.Jobs {
    public static class CurveSplitter {
        const double tol=1e1; //=10mm

        public static void RunScript(List<Curve> curves, Point3d start, double len, out List<Node> _nodes) {
            List<Point3d> pivots=new List<Point3d>();
            List<Node> nodes=new List<Node>();

            pivots.Add(start);

            double steps=10; // repeat limit
            for (int s=0; s<steps; s++) { // again and again
                for (int i=0; i<curves.Count; i++) {
                    Curve c = curves[i];
                    Point3d pivot=Point3d.Unset;
   
                    // fast divide from start (probably only step=0)
                   /*if (FindPivotAtStart(c, pivots, ref pivot)) {
                        // divide curve from pivot
                        c.DivideByLength(len, true, out Point3d[] points);

                        setPts(points.ToArray());
                    
                        //add divided end (not end of curve, but it's close) to pivots (if not exists)
                        foreach (Point3d p in points){
                            bool exists=false;
                            foreach (Point3d pi in pivots){
                                if (Math.Abs(pi.X-p.X)<tol && Math.Abs(pi.Y-p.Y)<tol) exists=true;
                            }
                            if (!exists) pivots.Add(p);
                        }
                    
                        //solved
                        curves.RemoveAt(i); 
                        i--;
                        continue;
                    }*/
                
                    // slover divide from somewhere
                    if (FindPivotAtMiddle(c, pivots, ref pivot)) {
                        List<Point3d> points=new List<Point3d>();
                        Curve_DivideByLengthFromPivotBothDir(c, len, pivot, ref points);
                      
                        setPts(points.ToArray());
                    
                        //add divided end (not end of curve, but it's close) to pivots (if not exists)
                        foreach (Point3d p in points){
                            bool exists=false;
                            foreach (Point3d pi in pivots) {
                                if (Math.Abs(start.X-p.X)<tol && Math.Abs(start.Y-p.Y)<tol) exists=true;
                            }
                            if (!exists) pivots.Add(p);
                        }

                        //solved
                        curves.RemoveAt(i); 
                        i--;             
                    }

                    if (curves.Count==0) break;
                }
            }

            // prepare nodes
            void setPts(Point3d[] points){
                Node[] nodes_converted=points.Select(i=>new Node(i)).ToArray();

                for (int j=0; j<nodes_converted.Length; j++) {
                    Node n=nodes_converted[j];
                    if (j<points.Length-1) n.Left=nodes_converted[j+1];
                    if (j>0) n.Right=nodes_converted[j-1];
                    nodes.Add(n);
                }
            }

            // -- links between points -- //

            // sort nodes by same z level
            List<List<Node>> nodes_z=new List<List<Node>>();

            // same level pts
            foreach (Node n in nodes) {
                bool existsZ=false;
                // try to find if exist point level with same z
                foreach (List<Node> sameZ in nodes_z) {
                    if (Math.Abs(n.Pt.Z-sameZ[0].Pt.Z)<tol) {
                        sameZ.Add(n);
                        existsZ=true;
                        break;
                    }
                }
                // new level
                if (!existsZ) {
                    nodes_z.Add(new List<Node>(){n});
                }
            }

            var sorted=nodes_z.OrderBy(i=>i[0].Pt.Z).ToArray();

            // assign up/down
            for (int z=0; z<sorted.Length; z++) {
                List<Node> nodes_currect=sorted[z];
                List<Node> nodes_down=null, nodes_up=null;
                if (z>0)nodes_down=sorted[z-1];
                if (z<sorted.Length-1)nodes_up=sorted[z+1];
            
                foreach (Node n in nodes_currect) {
                    double x=n.Pt.X, y=n.Pt.Y;

                    if (z>0) {
                        foreach (Node d in nodes_down) {
                            if (Math.Abs(d.Pt.X-x)<tol && Math.Abs(d.Pt.Y-y)<tol) {
                                d.Up=n;
                                n.Down=d;
                                break;
                            } 
                        }
                    }
                    if (z<sorted.Length-1) {
                        foreach (Node u in nodes_up) {
                            if (Math.Abs(u.Pt.X-x)<tol && Math.Abs(u.Pt.Y-y)<tol) {
                                u.Down=n;
                                n.Up=u;
                                break;
                            } 
                        }
                    }
                }
            }

            // left, right probably correct if curves have correct dirs (unreliable, but works)
            //nodes.ForEach(i=>i.TryBasicDirSolve());

            //export
            // basic
            //Polyline[] polylines=nodes.Select(i=>i.GetPolyline()).Where(i => i!=null).ToArray(); // ok
            //a = polylines;

            // create mesh
            /*Mesh mesh=new Mesh();

            Dictionary<Node, int> indexMap = new Dictionary<Node, int>();

            foreach (Node n in nodes) {
                indexMap[n] = mesh.Vertices.Add(n.Pt);
            }
        
            foreach (Node n in nodes) {
                MeshFace? f = n.GetMeshFace(indexMap);
                if (f!=null) mesh.Faces.AddFace((MeshFace)f);
            }

            mesh.Normals.ComputeNormals();
            mesh.Compact();

            _mesh=mesh;*/
            _nodes=nodes;
        }

        static void Curve_DivideByLengthFromPivotBothDir(Curve curve, double len, Point3d pivot, ref List<Point3d> points) {
            if (curve == null || !curve.IsValid || len <= RhinoMath.ZeroTolerance)
                return;

            // find pivot parameter
            if (!curve.ClosestPoint(pivot, out double tPivot))
                return;
            
            // always include pivot
          //  points.Add(curve.PointAt(tPivot));//pivot is already added, no dup

         //   points = new List<Point3d>();

            double totalLen = curve.GetLength();
            double lenToPivot = curve.GetLength(new Interval(curve.Domain.T0, tPivot));

            // always include pivot
           points.Add(curve.PointAt(tPivot));

            // ---- forward direction ----
            double accLen = lenToPivot + len;
            while (accLen < totalLen + tol) {
                if (curve.LengthParameter(accLen, out double t)) {
                    points.Add(curve.PointAt(t));
                } else {
                    break;
                }
                accLen += len;
            }

            // ---- backward direction ----
            accLen = lenToPivot - len;
            List<Point3d> backPts = new List<Point3d>();

            while (accLen > -tol) {
                if (curve.LengthParameter(accLen, out double t)) {
                    backPts.Add(curve.PointAt(t)); 
                  //points.Add(curve.PointAt(t)); // faster up
                } else {
                    break;
                }
                accLen -= len;
            }


            // prepend backward points in correct order
            backPts.Reverse();
            points.InsertRange(0, backPts);
        }

     /*  bool FindPivotAtStart(Curve curve, List<Point3d> pivots, ref Point3d pivot) {
            foreach (Point3d p in pivots){
                Point3d start=curve.PointAtStart;
                if (Math.Abs(start.X-p.X)<tol && Math.Abs(start.Y-p.Y)<tol) { // same floor plan pt
                    pivot=p;
                    return true;
                }
            }

            return false;
        }*/

        static bool FindPivotAtMiddle(Curve curve, List<Point3d> pivots, ref Point3d pivot) {
            foreach (Point3d p in pivots){ 
                Point3d start=curve.PointAtStart;
                Point3d estPos=new Point3d(p.X,p.Y, start.Z); // estimated position

                if (curve.ClosestPoint(estPos, out double par)) {
                    Point3d onCurve=curve.PointAt(par);
                    if (estPos.DistanceTo(onCurve)<tol){
                        pivot=onCurve;
                        return true;
                    }
                }
            }

            return false;
        }
    }

    //[DllImport("Node_5interactive.dll")]
    //public extern class Node;

    /*
    class Node{
        public Point3d Pt;
        public Node Up, Down, Left, Right;
        public Node[] Candidates;//left or right

        public Node(Point3d pt) {
            Pt=pt;
            Candidates=new Node[2];
        }

        public void TryBasicDirSolve(){
            Left=Candidates[0];
            Right=Candidates[1];
        }

        public Polyline GetPolyline() {
            if (Left == null || Down == null || Left.Down == null) return null;

            Point3d[] list=new Point3d[5]{Pt, Left.Pt, Left.Down.Pt, Down.Pt, Pt};
            Polyline p=new Polyline(list);
            return p;
        }

        public MeshFace? GetMeshFace(Dictionary<Node, int> map) {
            if (Left == null || Down == null || Left.Down == null) return null;

            MeshFace face=new MeshFace(
                map[this],
                map[Left],
                map[Left.Down],
                map[Down]
            );
        
            return face;
        }
    }*/
}
