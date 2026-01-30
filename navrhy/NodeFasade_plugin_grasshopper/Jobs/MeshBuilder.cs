using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace NodeFasadeG1.Jobs {
    public static class MeshBuilder {
        public static void RunScript(List<Node> nodes, int type, out Mesh _mesh, int divideCnt=4, double border=0.1, double spacing=0.05) {
            Mesh mesh=new Mesh();

            if (type==0) { // very simple preview
                //est capacity
                int cap=nodes.Count*4;
                mesh.Vertices.Capacity = cap;
                mesh.Faces.Capacity = cap;
                mesh.VertexColors.Capacity = cap;

                Dictionary<Node, int> indexMap = new Dictionary<Node, int>();
                foreach (Node n in nodes) {
                    indexMap[n] = mesh.Vertices.Add(n.Pt);
                    mesh.VertexColors.Add(Math.Min((int)(n.Value*255),255), Math.Min((int)(n.Value*255),255), Math.Min((int)(n.Value*255),255));
                }
        
                foreach (Node n in nodes) {
                    MeshFace? f = n.GetMeshFaceSimple(indexMap);
                    if (f!=null) mesh.Faces.AddFace((MeshFace)f);
                }
            } else if (type==1) { // final
                //est capacity
                int cap=nodes.Count*4*divideCnt;
                mesh.Vertices.Capacity = cap;
                mesh.Faces.Capacity = cap;
                mesh.VertexColors.Capacity = cap;

                foreach (Node n in nodes) {
                   n.GetMeshFace(mesh, divideCnt, border, spacing);
                }
            } else throw new Exception("Unknown type (number between 0-1)!");

            mesh.Normals.ComputeNormals();
            mesh.Compact();

            _mesh=mesh;
            
            // clean up
            Node.keysMeshVerticles.Clear();
        }

        public static List<Surface> Surfaces(List<Node> nodes,double border=0.1) {
            List<Surface> surfaces=new List<Surface>(nodes.Count);

            foreach (Node n in nodes) {
                Node right=n.Right;
                if (right==null)continue;

                Node down=n.Down;
                if (down==null) continue;

                Node downright=n.Down.Right;
                if (downright==null) continue;


                Surface s=Surface.CreateExtrusion(new Line(n.Pt,right.Pt).ToNurbsCurve(), n.Pt-down.Pt);
                

                n.Geometry=s;
                surfaces.Add(s);
            }
        
            return surfaces;
        }
    }
}
