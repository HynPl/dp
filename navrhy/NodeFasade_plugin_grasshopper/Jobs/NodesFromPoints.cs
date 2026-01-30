using Grasshopper.GUI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NodeFasadeG1.Jobs {
    public static class NodesFromPoints {
        const double tol=10; //=10mm

        public static void RunScript(List<Point3d> points, double len, out List<Node> _nodes) {
            List<Point3d> pivots=new List<Point3d>();
            List<Node> nodes=new List<Node>();

            // sort nodes by same z level
            List<List<Node>> nodes_z=new List<List<Node>>();

            // same level pts
            Node Last=null;
            foreach (Point3d pt in points) {
                bool existsZ=false;
                // try to find if exist point level with same z
                foreach (List<Node> sameZ in nodes_z) {
                    if (Math.Abs(pt.Z-sameZ[0].Pt.Z)<tol) {
                        Node node=new Node(pt);
                        existsZ=true;

                        if (Math.Abs(node.Pt.Z-Last.Pt.Z)<tol){// same level
                            if (node.Pt.DistanceTo(Last.Pt)<len){
                                node.Right=Last;
                                if (Last!=null) Last.Left=node;
                            }
                        }
                        sameZ.Add(node);
                        nodes.Add(node);
                        Last=node;
                        break;
                    }
                }
                // new level
                if (!existsZ) {
                    Node node=new Node(pt);
                    if (Last!=null){
                        if (Math.Abs(node.Pt.Z-Last.Pt.Z)<tol){// same level
                            if (node.Pt.DistanceTo(Last.Pt)<len){
                                node.Right=Last;
                                if (Last!=null && Last.Left==null) Last.Left=node;
                            }
                        }
                    }

                    nodes.Add(node);
                    nodes_z.Add(new List<Node>(){node});
                    Last=node;
                }
            }

            List<Node>[] sorted =nodes_z.OrderBy(i=>i[0].Pt.Z).ToArray();

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

            // remove far candidates
            foreach (Node node in nodes) { 
                double zlevel=node.Pt.Z;

              /*   // check candidates
               foreach (Node can in node.candidates) { //todo: convert to for loop
                    if (Math.Abs(node.Pt.Z-can.Pt.Z)<tol){
                        if (node.Pt.DistanceTo(can.Pt)<len){

                        }else node.candidates.Remove(can);// too far
                    }else node.candidates.Remove(can); //not same z level
                }*/

                // test
               // if (node.candidates.Count==2) { 
                  //  node.Left=node.candidates[0];
                   // node.Right=node.candidates[1];

                // find more candidates
                if (node.Left!=null && node.Right==null) { 
                    foreach (Node node2 in nodes) { 
                        if (node2.Left==null){
                            if (Math.Abs(node.Pt.Z-node2.Pt.Z)<tol) {
                                double dis=node.Pt.DistanceTo(node2.Pt);
                                if (dis<len) {
                                    node.Right=node2;
                                    node2.Left=node;
                                    break;
                                }
                            }
                        }
                    }
                }else if (node.Right!=null && node.Left==null) { 
                    foreach (Node node2 in nodes) { 
                        if (node2.Right==null) {
                            if (Math.Abs(node.Pt.Z-node2.Pt.Z)<tol) {
                                double dis=node.Pt.DistanceTo(node2.Pt);
                                if (dis<len) {
                                    node.Left=node2;
                                    node2.Right=node;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            _nodes=nodes;
        }
    }    
}
