// // Fairfield Tek L.L.C.
// // Copyright (c) 2016, Fairfield Tek L.L.C.
// // 
// // 
// // THIS SOFTWARE IS PROVIDED BY WINTERLEAF ENTERTAINMENT LLC ''AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
// //  INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// // PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL WINTERLEAF ENTERTAINMENT LLC BE LIABLE FOR ANY DIRECT, INDIRECT, 
// // INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// // SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// // ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
// // OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
// // DAMAGE. 

using System;
using System.Globalization;

namespace FairfieldTekLLC.Tiny.Udp.Example.Server.Math
{
    public class Point3F
    {
        public Point3F()
        {
            X = 0f;
            Y = 0f;
            Z = 0f;
        }

        public Point3F(string location)
        {
            if (location == null)
                return;

            string[] xyz = location.Split(' ');
            if (xyz.Length < 3)
                throw new Exception("Invalid Location Format, (x y z)");

            X = double.Parse(xyz[0], CultureInfo.CurrentCulture);
            Y = double.Parse(xyz[1], CultureInfo.CurrentCulture);
            Z = double.Parse(xyz[2], CultureInfo.CurrentCulture);
        }

        public Point3F(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double DistanceTo(Point3F location)
        {
            double xSqr = (X - location.X)*(X - location.X);
            double ySqr = (Y - location.Y)*(Y - location.Y);
            double zSqr = (Z - location.Z)*(Z - location.Z);
            double mySqr = xSqr + ySqr + zSqr;
            return System.Math.Sqrt(mySqr);
        }

        public override string ToString()
        {
            return
                $"{X.ToString("0.000", CultureInfo.InvariantCulture)} {Y.ToString("0.000", CultureInfo.InvariantCulture)} {Z.ToString("0.000", CultureInfo.InvariantCulture)}";
        }
    }
}