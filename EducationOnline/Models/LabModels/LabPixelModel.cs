using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace DistanceLearning.Models {
    public class LabPixelModel {

        public int x1 { get; set; }
        public int y1 { get; set; }
        public int x2 { get; set; }
        public int y2 { get; set; }

        public double L { get; set; }
        public double A { get; set; }
        public double B { get; set; }

        public double V { get; set; }
        public double C { get; set; }
        public double M { get; set; }
        public double Y { get; set; }

        public String Comment { get; set; }
        //public String Color { get; set; }

        public LabPixelModel() {
            Comment = "Неизвестная зона";
            //Color = "white";
            L = A = B = V = C = M = Y = 0;
        }
    }
}