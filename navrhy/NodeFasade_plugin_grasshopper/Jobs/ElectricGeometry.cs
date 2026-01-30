using Eto.Forms;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

namespace NodeFasadeG1.Jobs {
    static class ElectricGeometry {
       // static double tol=1;
        static int cnt_limit=1000;

        ///<summary>Electric aestetics</summary>
        public static void RunScript(List<Node> nodes, List<Curve> bottoms, int seed, out DataTree<Point3d> pts, double fulls=0, double sideChange=2) { 
            var _nodes=Node.CloneNodes(nodes);
            Random rnd=new Random(seed);

            List<ElectricRay> rays=new List<ElectricRay>();

            // Full rays
            // from bottom
            int nextFull=rnd.Next((int)(fulls/2));
            
            for (int step=0; step<40; step++) { 
                bool added=false;
                
                foreach (Node node in _nodes){                
                    if (node.Down==null && !node.usedElectric) { 
                        if (nextFull<=0) {
                            rays.Add(new ElectricRay(){ nodes=new List<Node>{node}, type=0}); 
                            node.usedElectric=true;
                            nextFull=rnd.Next((int)fulls);
                            added=true;
                        } else {                                                   
                            nextFull--;
                        }
                    }
                }
                foreach (Node node in nodes){ 
                    foreach (ElectricRay ray in rays){ 
                        if (ray.type==0){ 
                            Node cnode=ray.nodes[0];

                            for (int i=0; i<cnt_limit; i++) {
                                // change direction 
                                int side=0;
                                if (rnd.Next((int)sideChange)==0) { 
                                    side=rnd.Next(2)*2-1;//-1 or 1
                                }
                                if (!ray.FindNextUp(side, ref cnode)) { 
                                    break; // ray finished                               
                                }
                            }                        
                        }    
                    }
                }

                // from top
                nextFull=rnd.Next((int)(fulls/2));
                foreach (Node node in _nodes){                
                    if (node.Up==null && !node.usedElectric) { 
                        if (nextFull<=0) {
                            rays.Add(new ElectricRay(){ nodes=new List<Node>{node}, type=0}); 
                            node.usedElectric=true;
                            nextFull=rnd.Next((int)fulls);
                            added=true;
                        } else {                                                   
                            nextFull--;
                        }
                    }
                }
                foreach (Node node in nodes){ 
                    foreach (ElectricRay ray in rays){ 
                        if (ray.type==0){ 
                            Node cnode=ray.nodes[0];

                            for (int i=0; i<cnt_limit; i++) {
                                // change direction 
                                int side=0;
                                if (rnd.Next((int)sideChange)==0) { 
                                    side=rnd.Next(2)*2-1;//-1 or 1
                                }
                                if (!ray.FindNextDown(side, ref cnode)) { 
                                    break; // ray finished                               
                                }
                            }                        
                        }    
                    }
                }
                if (!added) break;
            }

            //todo: fill empty nodes
            for (int level=1; level<cnt_limit; level++){                
                foreach (Node node in _nodes) {                
                    if (node.Down==null) {  
                        Node lookup=node;
                        for (int l=0; l<level; l++){// from bottom
                            if (lookup.Up!=null){
                                lookup=lookup.Up;
                                if (!lookup.usedElectric){ 
                                    ElectricRay ray=new ElectricRay(){ nodes=new List<Node>{lookup}, type=0};
                                    rays.Add(ray); 
                                    lookup.usedElectric=true;
                                    for (int r=0; r<cnt_limit; r++){
                                        if (!ray.FindNextUpAlive(ref lookup, rnd)) break;
                                    }
                                    break;
                                }
                            }
                        }                       
                    }
                }                
            }             

            // return
            pts=new DataTree<Point3d>();

            int ii = 0;
            foreach (ElectricRay ray in rays){   
                GH_Path path = new GH_Path(ii);
                pts.AddRange(ray.GetArrOfPts(), path);
                ii++;
            }               
        }
    }

    class ElectricRay{ 
        public List<Node> nodes;
        public bool finished; //true=>do not add nodes
        public int type;//0=long, 1=near to end, 2 fill

        public Polyline ToPolyline(){ 
             return new Polyline(nodes.ConvertAll(i=>i.Pt));
        }

        public List<Point3d> GetArrOfPts(){ 
            List<Point3d> pts=new List<Point3d>();
            pts.Capacity=nodes.Count;
            for (int i=0; i<nodes.Count; i++){ 
                Point3d o=nodes[i].Pt;
                pts.Add(o/*new Point3d(o.X,o.Y,o.Z)*/);
            }
            return pts;
        }

        ///<summary></summary>
        ///<returns>true if ok, false on finished (cannot continue)</returns>
        public bool FindNextUp(int up, ref Node node) { 
            Node last=nodes[nodes.Count-1];
            if (up==-1) { 
                if (last.Up!=null && last.Up.Left!=null) {
                    node=last.Up.Left;                    
                    if (!node.usedElectric && !last.Up.usedElectric) {//not cross
                        node.usedElectric=true;
                        nodes.Add(node);                        
                        return true;   
                    }
                }
            } else if (up==1) { 
                if (last.Up!=null && last.Up.Right!=null) {
                    node=last.Up.Right;
                    if (!node.usedElectric && !last.Up.usedElectric) {//not cross
                        node.usedElectric=true;
                        nodes.Add(node);                    
                        return true;    
                    }
                }
            } 
            if (last.Up!=null) {
                node=last.Up;
                if (!node.usedElectric) {
                    node.usedElectric=true;
                    nodes.Add(node);
                    return true;   
                }
            }
            finished=true;
            return false;
        }

        ///<summary>Připojit se k dalšímu uzlu, který je níže</summary>
        ///<param name="down">0 nebo výměčně 1 nebo -1</param>
        ///<param name="node">napojující se uzel</param>
        ///<returns>false okdyž se nemůže napojit</returns>
        public bool FindNextDown(int down, ref Node node) { 
            Node last=nodes[nodes.Count-1];
            if (down==-1) { 
                if (last.Down!=null && last.Down.Left!=null) {
                    if (!last.Down.Left.usedElectric && !last.Down.usedElectric) {//nekříží se
                        node=last.Down.Left;                    
                        node.usedElectric=true;
                        nodes.Add(node);                        
                        return true;   
                    }
                }
            } else if (down==1) { 
                if (last.Down!=null && last.Down.Right!=null) {
                    if (!last.Down.Right.usedElectric && !last.Down.usedElectric) {//nekříží se
                        node=last.Down.Right;
                        node.usedElectric=true;
                        nodes.Add(node);                    
                        return true;    
                    }
                }
            }
            if (last.Down!=null) {
                if (!last.Down.usedElectric) {
                    node=last.Down;
                    node.usedElectric=true;
                    nodes.Add(node);
                    return true;   
                }
            }
            finished=true;
            return false;
        }

         ///<summary></summary>
        ///<returns>true if ok, false on finished (cannot continue)</returns>
        public bool FindNextUpAlive(ref Node node, Random rnd) { 
            Node last=nodes[nodes.Count-1];

            // try up
            if (last.Up!=null) {
                if (!last.Up.usedElectric) {
                    node=last.Up;
                    node.usedElectric=true;
                    nodes.Add(node);
                    return true;   
                }
            }

            // try side
            //try right first
            if (rnd.Next(2)==1){  
                bool used=Right(ref node);
                if (!used) used=Left(ref node);
                return used;
                
            //try left firt                     
            } else {                    
                bool used=Left(ref node);
                if (!used) used=Right(ref node);
                return used;
            }

            bool Left(ref Node node2) { 
                if (last.Left!=null) {
                    if (last.Left.Up!=null){
                        if (!last.Left.Up.usedElectric) {
                            if (last.Left.usedElectric && last.Up.usedElectric){
                                }else{
                                node2=last.Left.Up;                    
                                node2.usedElectric=true;
                                nodes.Add(node2);                        
                                return true;  
                            }
                        }
                    }
                }
                return false;
            }

            bool Right(ref Node node2) { 
                    if (last.Right!=null) {
                        if (last.Right.Up!=null){
                            if (!last.Right.Up.usedElectric) {
                                if (last.Right.usedElectric && last.Up.usedElectric){
                            }else{
                                    node2=last.Right.Up;                    
                                    node2.usedElectric=true;
                                    nodes.Add(node2);                        
                                    return true;  
                                }
                            }
                        }
                    }
                    return false;
                }         
        }

    }
}




