using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace NodeFasadeG1.Jobs {
    public static class AssignCommandsToNodes {
        public static void RunScript(List<Node> nodes, List<Command> commands, ref List<Node> _nodes) {  
            _nodes=Node.CloneNodes(nodes);

            foreach (Command command in commands) {
                if (command is Text text) {
                    Node n=FindNode(text.offset, _nodes);

                    double[,] fieldVal=Font.GetText(text.text);
                    int x=fieldVal.GetLength(1),y=fieldVal.GetLength(0);
                    Node[,] field=GetField(n, x, y);
                    if (field!=null){ 
                        SetNodesValues(field, fieldVal,text.inversed); 
                    }
                } else if (command is FillArea fillarea) {
                    Node n=FindNode(fillarea.offset, _nodes);  
                    if (n!=null){
                        int x=fillarea.x,z=fillarea.z;
                        Node[,] field=GetField(n, x, z);
                        if (field!=null){ 
                            SetNodesValues(field, fillarea.value); 
                        }
                    }
                }else if (command is Fill fill){ 
                    foreach (Node n in _nodes){ 
                        n.Value=fill.value;
                    }
                }
            }              
        }

        ///<summary>closest point</summary>
        public static Node FindNode(Point3d pt, List<Node> nodes) {
            double min=1e10;
            Node closest=null;
            foreach (Node n in nodes) {
                double dis=n.Pt.DistanceTo(pt);
                
                if (min>dis){ 
                    min=dis;
                    closest=n;
                }    
            }
            return closest;
        }

        public static Node[,] GetField(Node start, int sizeX, int sizeZ) {
            Node[,] field=new Node[sizeX, sizeZ];

            Node top=start;
            for (int x=0; x<sizeX; x++) { 
                if (top.Left!=null){ 
                    top=top.Left;
                    Node c=top;
                    for (int z=0; z<sizeZ; z++) { 
                        if (c.Down!=null) { 
                            field[x,z] = c;
                            c=c.Down;
                        }//else return null;
                    }                     
                }//else return null;
            }

            return field;
        }

        public static void SetNodesValues(Node[,] nodes, double[,] field, bool inversed){ 
            for (int x=0;x<nodes.GetLength(0);x++) { 
                for (int y=0;y<nodes.GetLength(1);y++) { 
                    if (nodes[x,y]!=null/*x<field.GetLength(1) && y<field.GetLength(0)*/){
                        double val=field[y,x];
                        if (val==1){
                            if (inversed)nodes[x,y].Value=1-val;
                            else nodes[x,y].Value=val;
                        }     
                    }
                }
            }
        } 
        
        public static void SetNodesValues(Node[,] nodes, double value){ 
            for (int x=0;x<nodes.GetLength(0);x++) { 
                for (int y=0;y<nodes.GetLength(1);y++) { 
                    nodes[x,y].Value=value;
                }
            }
        }
    }
}
