using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NodeFasadeG1.Jobs {
    public class Node{
        public double Value, Value2;
        public Point3d Pt;
        public Node Up, Down, Left, Right;
        public bool usedElectric;
        public List<Node> candidates=new List<Node>();
        public object Geometry;
        public static Dictionary<VertexKey, int> keysMeshVerticles = new Dictionary<VertexKey, int>();

        public Node(Point3d pt) {
            Pt=pt;
        }

        public MeshFace? GetMeshFaceSimple(Dictionary<Node, int> map) {
            if (Left == null || Down == null || Left.Down == null) return null;

            MeshFace face=new MeshFace(
                map[this],
                map[Left],
                map[Left.Down],
                map[Down]
            );
        
            return face;
        }
        
        public void GetMeshFace(Mesh mesh, int divide, double offsetFromBorder=0.1, double innerSpacings=0.05) {
            if (Left == null || Down == null || Left.Down == null) return;

            // compute color
            int val=Math.Min((int)(Value*255), 255);
            Color col=Color.FromArgb(val,val,val);
 
            // positions of subdividing
          //  double offsetFromBorder=0.1, innerSpacings=0.05;

            // grid pts
            Point3d a0 = Pt, b0 = Left.Pt, c0 = Left.Down.Pt, d0 = Down.Pt;

            // make borders
            Vector3d leftBorder=(b0-a0)*offsetFromBorder,
                downBorder=(d0-a0)*offsetFromBorder;

            Point3d a=a0+leftBorder+downBorder, 
                    b=b0-leftBorder+downBorder, 
                    c=c0-leftBorder-downBorder,
                    d=d0+leftBorder-downBorder;

         //   double len=a.DistanceTo(d); //len betwwen nodes

            // todo effect with points if Value is between 0 or 1         
            if (Value!=0 && Value!=1) { 
                // effect horizontal rotare 
                double angle=Math.PI*Value;
                double sin=Math.Sin(angle), cos=Math.Cos(angle);// premultipled for multiple to speed up
                Vector3d axis = a - b;
                axis.Unitize();

                Vector3d next=d-b;

                double t11=(double)(0)/divide;
                Point3d cor_a1=a+(c-a)*(t11+innerSpacings); //+innerSpacings from bottom (agains doubbled space)

                double t21=(double)(0-innerSpacings)/divide;
                Point3d cor_d1=a+(c-a)*(t21+innerSpacings);//+innerSpacings from bottom (agains doubbled space)

                // line of rotation
                Point3d ad_mid1=(cor_a1+cor_d1)/2;

                Vector3d v = cor_a1 - ad_mid1;

                // rotare mooving vector
                Vector3d vRot = v * cos +
                    Vector3d.CrossProduct(axis, v) * sin +
                    axis * Vector3d.Multiply(axis, v) * (1 - cos);


                for (int i=0; i<divide; i++) { 
                    double t1=(double)(i)/divide;
                    Point3d cor_a=a+(c-a)*(t1+innerSpacings); //+innerSpacings from bottom (agains doubbled space)
                    Point3d cor_b=cor_a+next; //=b+(d-b)*t

                    double t2=(double)(i-innerSpacings)/divide;
                    Point3d cor_d=a+(c-a)*(t2+innerSpacings);//+innerSpacings from bottom (agains doubbled space)
                    Point3d cor_c=cor_d+next; 

                    // line of rotation
                    Point3d ad_mid=(cor_a+cor_c)/2;
                    Point3d bd_mid=(cor_b+cor_d)/2;

                    // rotare
                     /*Vector3d v = cor_a - ad_mid; //mooving vector (same for multiple, should be premultipled)
                   Vector3d vRot = v * cos +
                                    Vector3d.CrossProduct(axis, v) * sin +
                                    axis * Vector3d.Multiply(axis, v) * (1 - cos);*/ //mooving vector (same for multiple, should be premultipled)
                    Point3d m_a=ad_mid + vRot, m_b=bd_mid + vRot;
                    Point3d m_d=ad_mid - vRot, m_c=bd_mid - vRot;
                    
                    int vA = mesh.Vertices.Add(m_a);
                    mesh.VertexColors.Add(col);
                    int vB = mesh.Vertices.Add(m_b);
                    mesh.VertexColors.Add(col);
                    int vC = mesh.Vertices.Add(m_c);
                    mesh.VertexColors.Add(col);
                    int vD = mesh.Vertices.Add(m_d);
                    mesh.VertexColors.Add(col);

                    mesh.Faces.AddFace(vA, vB, vC, vD);
                }
            } else { // standart, no rotare, cca 95%
                // premultipled
                Vector3d next=b-a, down=d-a;
                Vector3d downD=(1d/divide-innerSpacings)*down;
                double more1=1+innerSpacings;

                for (int i=0; i<divide; i++) { 
                    double t=(double)i/divide*more1;
                    Point3d cor_a=a+down*t,
                        cor_b=cor_a+next;

                    Point3d cor_d=cor_a+downD,
                        cor_c=cor_d+next; 

                    int vA = mesh.Vertices.Add(cor_a);
                    mesh.VertexColors.Add(col);
                    int vB = mesh.Vertices.Add(cor_b);
                    mesh.VertexColors.Add(col);
                    int vC = mesh.Vertices.Add(cor_c);
                    mesh.VertexColors.Add(col);
                    int vD = mesh.Vertices.Add(cor_d);
                    mesh.VertexColors.Add(col);

                    mesh.Faces.AddFace(vA, vB, vC, vD);
                }
            }    
        }

        #region mesh
       /* // add mesh face
        static void AddFaceRaw(Mesh mesh, VertexKey vk1, VertexKey vk2, VertexKey vk3, VertexKey vk4) {
            int v1=AddVertex(mesh, vk1);
            int v2=AddVertex(mesh, vk2);
            int v3=AddVertex(mesh, vk3);
            int v4=AddVertex(mesh, vk4);

            mesh.Faces.AddFace(new MeshFace(v1, v2, v3, v4));
        }

        private static int AddVertex(Mesh mesh, VertexKey k) {
            // search existing verticles
            if (keysMeshVerticles.TryGetValue(k, out int index)) return index;

            //add to keysMeshVerticles?
            index = mesh.Vertices.Add(k.Pt);
            mesh.VertexColors.Add(k.Color);

            keysMeshVerticles[k] = index;
            return index;
        }*/
        #endregion

        public static List<Node> CloneNodes(List<Node> original) {
            Dictionary<Node, Node> map = new Dictionary<Node, Node>();

            foreach (Node n in original) {
                if (n!=null){
                    map[n] = new Node(n.Pt) {
                        Value = n.Value,
                        usedElectric=n.usedElectric,
                        Geometry=n.Geometry,                        
                    };
                }
            }

            foreach (Node n in original) {
                Node c = map[n];
                if (n == null) continue;
               /* c.Up = n.Up != null ? map[n.Up] : null;
                c.Down = n.Down != null ? map[n.Down] : null;
                c.Left = n.Left != null ? map[n.Left] : null;
                c.Right = n.Right != null ? map[n.Right] : null;
                */
                c.Up = (n.Up != null && map.ContainsKey(n.Up)) ? map[n.Up] : null;
                c.Down = (n.Down != null && map.ContainsKey(n.Down)) ? map[n.Down] : null;
                c.Left = (n.Left != null && map.ContainsKey(n.Left)) ? map[n.Left] : null;
                c.Right = (n.Right != null && map.ContainsKey(n.Right)) ? map[n.Right] : null;
            }

            return map.Values.ToList();
        }
    }    
    
    public class VertexKey {
        public Point3d Pt;
        public Color Color;

        public VertexKey(Point3d pt, Color color) {
            Pt = pt;
            Color = color;
        }

        public override int GetHashCode() {
            unchecked {
                int h = 17;
                h = h * 23 + Pt.GetHashCode();
                h = h * 23 + Color.R; // only monochrome in project
                return h;
            }
        }

        public override bool Equals(object obj) {
        /*   if (!(obj is VertexKey)) return false;
            VertexKey k = (VertexKey)obj;
            return Pt.Equals(k.Pt) && Color.Equals(k.Color);*/

            // primary
            if (!Pt.Equals(((VertexKey)obj).Pt)) return false;

            // not much
            if (!Color.Equals(((VertexKey)obj).Color)) return false;
            return true;
        }
    }
}
