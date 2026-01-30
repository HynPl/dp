using System;
using System.Collections.Generic;
using System.Linq;
using static Rhino.Geometry.SubDDisplayParameters;

namespace NodeFasadeG1.Jobs {
    public static class FloydSteinbergDither {
        // nodes.Value: 0-1

        public static List<Node> FloydSteinbergDitherJob(List<Node> nodes, double pivot) {
            List<Node> _nodes =Node.CloneNodes(nodes);

            // seřadit podle úrovní Z
            var grouped=_nodes.GroupBy(i=>i.Pt.Z).OrderBy(g => g.Key).ToList();
                
            // projít uzly
            foreach (IGrouping<double, Node> zLevel in grouped) {
                foreach (Node node in zLevel) {
                    double oldPixel =node.Value;
                    double newPixel = oldPixel < pivot ? 0d : 1d;
                    double error = oldPixel - newPixel;

                    // hodnota "pixelu"
                    node.Value = newPixel;

                    node.Right?.Value += error * 7.0/16.0;                
                    node.Left?.Down?.Value+=error * 3.0/16.0;                  
                    node.Down?.Value+=error * 5.0/16.0;                  
                    node.Right?.Down?.Value += error * 1.0/16.0;                
                }
            }

            // výsledná hodnota
            foreach (Node node in _nodes) {
                if (node.Value<=pivot/*0.5*/) node.Value=0d; else node.Value=1d;
            }

            return _nodes;
        } 
        
        public static List<Node> Matrix8Job(List<Node> nodes, double pivot, double density, int iterations) {
            List<Node> _nodes =Node.CloneNodes(nodes);
           
          //  InitializeNodes(_nodes, density);

            for (int i = 0; i < iterations; i++) {

                Node densest = null;
                Node emptiest = null;

                double maxD = double.MinValue;
                double minD = double.MaxValue;

                foreach (Node n in _nodes) {

                    double d = Density(n);

                    if (n.Value == 1.0 && d > maxD) {
                        maxD = d;
                        densest = n;
                    }

                    if (n.Value == 0.0 && d < minD) {
                        minD = d;
                        emptiest = n;
                    }
                }

                if (densest == null || emptiest == null)
                    break;

                densest.Value = 0.0;
                emptiest.Value = 1.0;
            }

            foreach (Node node in _nodes) {
                if (node.Value<=pivot) node.Value=0d; else node.Value=1d;
            }

            return _nodes;
        }

        static double Density(Node n) {
            double sum = n.Value * 1.0;

            if (n.Left != null)  sum += n.Left.Value * 0.5;
            if (n.Right != null) sum += n.Right.Value * 0.5;
            if (n.Up != null)    sum += n.Up.Value * 0.5;
            if (n.Down != null)  sum += n.Down.Value * 0.5;

            // diagonals (optional)
            if (n.Left?.Up != null)    sum += n.Left.Up.Value * 0.25;
            if (n.Right?.Up != null)   sum += n.Right.Up.Value * 0.25;
            if (n.Left?.Down != null)  sum += n.Left.Down.Value * 0.25;
            if (n.Right?.Down != null) sum += n.Right.Down.Value * 0.25;

            return sum;
        }

        static void InitializeNodes(List<Node> nodes, double density) {
            Random rng = new Random(1);

            foreach (Node n in nodes)
                n.Value = 0.0;

            int target = (int)(nodes.Count * density);
            int count = 0;

            while (count < target) {
                Node n = nodes[rng.Next(nodes.Count)];
                if (n.Value == 0.0) {
                    n.Value = 1.0;
                    count++;
                }
            }
        }

     

    }
}
