using Rhino.Geometry;

namespace NodeFasadeG1.Jobs {
    public class Command {
        public string Name;

    }

    public class FillArea: Command {
        public int x, z;
        public double value;
        public Point3d offset;
    }

    public class Fill:Command {
        public double value;
    }

    public class Text: Command {
        public Point3d offset;
        public string text;
        public bool inversed;
    }
}
