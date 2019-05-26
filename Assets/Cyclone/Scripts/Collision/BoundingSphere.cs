using System.Text;

namespace Cyclone
{
    public class BoundingSphere
    {
        public Vector3 Center { get; private set; }
        
        public double Radius { get; private set; }

        public double Size
        {
            get
            {
                return 1.333333 * 3.141593 * Radius * Radius * Radius;
            }
        }
        
        public BoundingSphere(Vector3 center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public BoundingSphere(BoundingSphere one, BoundingSphere two)
        {
            Vector3 centreOffset = two.Center - one.Center;
            double distance = centreOffset.SquareMagnitude;
            double radiusDiff = two.Radius - one.Radius;
            
            if (radiusDiff * radiusDiff >= distance)
            {
                if (one.Radius > two.Radius)
                {
                    Center = one.Center;
                    Radius = one.Radius;
                }
                else
                {
                    Center = two.Center;
                    Radius = two.Radius;
                }
            }
            
            else
            {
                distance = System.Math.Sqrt(distance);
                Radius = (distance + one.Radius + two.Radius) * 0.5;
                
                Center = one.Center;
                if (distance > 0)
                {
                    Center += centreOffset * ((Radius - one.Radius) / distance);
                }
            }
        }

        public bool Overlaps(BoundingSphere other)
        {
            double distanceSquared = (Center - other.Center).SquareMagnitude;
            return distanceSquared < (Radius + other.Radius) * (Radius + other.Radius);
        }
        
        public double GetGrowth(BoundingSphere other)
        {
            BoundingSphere newSphere = new BoundingSphere(this, other);
            
            return newSphere.Radius * newSphere.Radius - Radius * Radius;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("C.x: {0}, C.y: {1}, C.z: {2}, R: {3}", Center.x, Center.y, Center.z, Radius);
            return sb.ToString();
        }

        public void SetCenter(Vector3 newCenter)
        {
            Center = newCenter;
        }
    };
}
