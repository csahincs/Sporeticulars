using System;
using Cyclone.Math;

namespace Cyclone
{
    /// <summary>
    /// A rigid body is the basic simulation object in the physics core.
    /// </summary>
    public class RigidBody
    {

        #region Characteristic Data and State

        /// <summary>
        /// Gets or sets the inverse mass of the rigid body.
        /// </summary>
        /// <remarks>
        /// It is more useful to hold the inverse mass because
        /// integration is simpler, and because in real-time simulation
        /// it is more useful to have bodies with infinite mass than
        /// zero mass.
        /// 
        /// This invalidates internal data for the rigid body.
        /// Either an integration function, or the calculateInternals
        /// function should be called before trying to get any settings
        /// from the rigid body.
        /// </remarks>
        protected double InverseMass { get; set; }

        /// <summary>
        /// Gets or sets the mass of the rigid body.
        /// </summary>
        /// <remarks>
        /// This invalidates internal data for the rigid body.
        /// Either an integration function, or the calculateInternals
        /// function should be called before trying to get any settings
        /// from the rigid body. 
        /// </remarks>
        public double Mass
        {
            get
            {
                if (Core.Equals(InverseMass, 0.0))
                {
                    return Double.MaxValue;
                }

                return 1.0 / InverseMass;
            }
            set
            {
                if (Core.Equals(value, 0.0))
                {
                    throw new Exception("mass cannot equal zero");
                }

                InverseMass = 1.0 / value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of damping applied to linear
        /// motion. Damping is required to remove energy added through
        /// numerical instability in the integrator.
        /// </summary>
        public double LinearDamping { get; set; }

        /// <summary>
        /// Gets or sets the linear position of the rigid boy in world space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the angular orientation of the rigid body in world space.
        /// </summary>
        public Quaternion Orientation { get; set; }

        /// <summary>
        /// Gets or sets the linear velocity of the rigid body in world space.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Gets or sets the angular velocity, or rotation of the rigid body in world space.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Holds the inverse of the body's inertia tensor. The
        /// inertia tensor provided must not be degenerate
        /// (that would mean the body had zero inertia for
        /// spinning along one axis). As long as the tensor is
        /// finite, it will be invertible. The inverse tensor
        /// is used for similar reasons to the use of inverse
        /// mass.
        ///
        /// The inertia tensor, unlike the other variables that
        /// define a rigid body, is given in body space.
        /// </summary>
        public Matrix3 InverseInertiaTensor { get; set; }

        /// <summary>
        /// Holds the amount of damping applied to angular
        /// motion. Damping is required to remove energy added
        /// through numerical instability in the integrator.
        /// </summary>
        public double AngularDamping { get; set; }

        #endregion
        

        #region Derived Data

        /// <summary>
        /// Holds the inverse inertia tensor of the body in world
        /// space. The inverse inertia tensor member is specified in
        /// the body's local space.
        /// </summary>
        private Matrix3 InverseInertiaTensorWorld { get; set; }

        /// <summary>
        /// Holds the amount of motion of the body. This is a recency
        /// weighted mean that can be used to put a body to sleap.
        /// </summary>
        double motion;

        /// <summary>
        /// A body can be put to sleep to avoid it being updated
        /// by the integration functions or affected by collisions
        /// with the world.
        /// </summary>
        bool isAwake;

        /// <summary>
        /// Some bodies may never be allowed to fall asleep.
        /// User controlled bodies, for example, should be
        /// always awake.
        /// </summary>
        bool canSleep;

        /// <summary>
        /// Gets or sets the transform matrix for converting body space into
        /// world space and vice versa. This can be acheived by calling the
        /// getPointIn*Space methods.
        /// </summary>
        /// <remarks>
        /// This matrix should be derived from the orientation and position
        /// once per frame, to make sure it is correct. This just acts as a
        /// cache to void repeated calculations.
        /// </remarks>
        protected Matrix4 TransformMatrix { get; set; }

        #endregion
        

        #region Force and Torque Accumulators

        /// <summary>
        /// Holds the accumulated force to be applied at the next
        /// integration step.
        /// </summary> 
        private Vector3 ForceAccum { get; set; }

        /// <summary>
        /// Holds the accumulated torque to be applied at the next
        /// integration step.
        /// </summary>
        private Vector3 TorqueAccum { get; set; }

        /// <summary>
        /// Holds the acceleration of the rigid body.  This value
        /// can be used to set acceleration due to gravity (its primary
        /// use), or any other constant acceleration.
        /// </summary>
        private Vector3 Acceleration { get; set; }

        /// <summary>
        /// Holds the linear acceleration of the rigid body, for the
        /// previous frame.
        /// </summary>
        private Vector3 LastFrameAcceleration { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="RigidBody"/> class.
        /// </summary>
        public RigidBody()
        {
            Position = new Vector3();
            Orientation = new Quaternion();
            Velocity = new Vector3();
            Rotation = new Vector3();
            InverseInertiaTensor = new Matrix3();
            InverseInertiaTensorWorld = new Matrix3();
            TransformMatrix = new Matrix4();
            ForceAccum = new Vector3();
            TorqueAccum = new Vector3();
            Acceleration = new Vector3();
            LastFrameAcceleration = new Vector3();
        }

        #endregion
        
        #region Integration and Simulation Functions

        /// <summary>
        /// Calculates internal data from state data. This should be called
        /// after the body's state is altered directly (is is called automatically
        /// during integration). If you change the body's state and then intend
        /// to integrate before querying any data (such as the transform matrix),
        /// then you can omit this step.
        /// </summary>
        public void CalculateDerivedData()
        {
            Orientation.Normalize();

            // Calculate the transform matrix for the body.
            CalculateTransformMatrix(TransformMatrix, Position, Orientation);

            // Calculate the inertiaTensor in world space.
            TransformInertiaTensor(InverseInertiaTensorWorld, Orientation, InverseInertiaTensor, TransformMatrix);
        }

        /// <summary>
        /// Create the transform matrix from position and orientation.
        /// </summary>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <param name="position">The position.</param>
        /// <param name="orientation">The orientation.</param>
        protected static void CalculateTransformMatrix(Matrix4 transformMatrix, Vector3 position, Quaternion orientation)
        {
            transformMatrix.Data[0] = 1 - 2 * orientation.j * orientation.j - 2 * orientation.k * orientation.k;
            transformMatrix.Data[1] = 2 * orientation.i * orientation.j - 2 * orientation.r * orientation.k;
            transformMatrix.Data[2] = 2 * orientation.i * orientation.k + 2 * orientation.r * orientation.j;
            transformMatrix.Data[3] = position.x;

            transformMatrix.Data[4] = 2 * orientation.i * orientation.j + 2 * orientation.r * orientation.k;
            transformMatrix.Data[5] = 1 - 2 * orientation.i * orientation.i - 2 * orientation.k * orientation.k;
            transformMatrix.Data[6] = 2 * orientation.j * orientation.k - 2 * orientation.r * orientation.i;
            transformMatrix.Data[7] = position.y;

            transformMatrix.Data[8] = 2 * orientation.i * orientation.k - 2 * orientation.r * orientation.j;
            transformMatrix.Data[9] = 2 * orientation.j * orientation.k + 2 * orientation.r * orientation.i;
            transformMatrix.Data[10] = 1 - 2 * orientation.i * orientation.i - 2 * orientation.j * orientation.j;
            transformMatrix.Data[11] = position.z;
        }

        /// <summary>
        /// Internal function to do an intertia tensor transform by a quaternion.
        /// Note that the implementation of this function was created by an
        /// automated code-generator and optimizer.
        /// </summary>
        /// <param name="iitWorld"></param>
        /// <param name="q"></param>
        /// <param name="iitBody"></param>
        /// <param name="rotmat"></param>
        protected static void TransformInertiaTensor(Matrix3 iitWorld, Quaternion q, Matrix3 iitBody, Matrix4 rotmat)
        {
            double t4 = rotmat.Data[0] * iitBody.Data[0] + rotmat.Data[1] * iitBody.Data[3] + rotmat.Data[2] * iitBody.Data[6];
            double t9 = rotmat.Data[0] * iitBody.Data[1] + rotmat.Data[1] * iitBody.Data[4] + rotmat.Data[2] * iitBody.Data[7];
            double t14 = rotmat.Data[0] * iitBody.Data[2] + rotmat.Data[1] * iitBody.Data[5] + rotmat.Data[2] * iitBody.Data[8];
            double t28 = rotmat.Data[4] * iitBody.Data[0] + rotmat.Data[5] * iitBody.Data[3] + rotmat.Data[6] * iitBody.Data[6];
            double t33 = rotmat.Data[4] * iitBody.Data[1] + rotmat.Data[5] * iitBody.Data[4] + rotmat.Data[6] * iitBody.Data[7];
            double t38 = rotmat.Data[4] * iitBody.Data[2] + rotmat.Data[5] * iitBody.Data[5] + rotmat.Data[6] * iitBody.Data[8];
            double t52 = rotmat.Data[8] * iitBody.Data[0] + rotmat.Data[9] * iitBody.Data[3] + rotmat.Data[10] * iitBody.Data[6];
            double t57 = rotmat.Data[8] * iitBody.Data[1] + rotmat.Data[9] * iitBody.Data[4] + rotmat.Data[10] * iitBody.Data[7];
            double t62 = rotmat.Data[8] * iitBody.Data[2] + rotmat.Data[9] * iitBody.Data[5] + rotmat.Data[10] * iitBody.Data[8];

            iitWorld.Data[0] = t4 * rotmat.Data[0] + t9 * rotmat.Data[1] + t14 * rotmat.Data[2];
            iitWorld.Data[1] = t4 * rotmat.Data[4] + t9 * rotmat.Data[5] + t14 * rotmat.Data[6];
            iitWorld.Data[2] = t4 * rotmat.Data[8] + t9 * rotmat.Data[9] + t14 * rotmat.Data[10];
            iitWorld.Data[3] = t28 * rotmat.Data[0] + t33 * rotmat.Data[1] + t38 * rotmat.Data[2];
            iitWorld.Data[4] = t28 * rotmat.Data[4] + t33 * rotmat.Data[5] + t38 * rotmat.Data[6];
            iitWorld.Data[5] = t28 * rotmat.Data[8] + t33 * rotmat.Data[9] + t38 * rotmat.Data[10];
            iitWorld.Data[6] = t52 * rotmat.Data[0] + t57 * rotmat.Data[1] + t62 * rotmat.Data[2];
            iitWorld.Data[7] = t52 * rotmat.Data[4] + t57 * rotmat.Data[5] + t62 * rotmat.Data[6];
            iitWorld.Data[8] = t52 * rotmat.Data[8] + t57 * rotmat.Data[9] + t62 * rotmat.Data[10];
        }

        /// <summary>
        /// Integrates the rigid body forward in time by the given amount.
        /// This function uses a Newton-Euler integration method, which is a
        /// linear approximation to the correct integral. For this reason it
        /// may be inaccurate in some cases.
        /// </summary>
        /// <param name="duration">
        /// Time interval over which to update the position and velocity.
        /// This is currently the time between frames.
        /// </param>
        public void Integrate(double duration)
        {
            if (!isAwake)
            {
                return;
            }

            // Calculate linear acceleration from force inputs.
            LastFrameAcceleration = GetAcceleration();
            LastFrameAcceleration.AddScaledVector(ForceAccum, InverseMass);

            // Calculate angular acceleration from torque inputs.
            Vector3 angularAcceleration = InverseInertiaTensorWorld.Transform(TorqueAccum);

            // Update linear velocity from both acceleration and impulse.
            Velocity.AddScaledVector(LastFrameAcceleration, duration);

            // Update angular velocity from both acceleration and impulse.
            Rotation.AddScaledVector(angularAcceleration, duration);

            // Impose drag.
            Velocity *= System.Math.Pow(LinearDamping, duration);
            Rotation *= System.Math.Pow(AngularDamping, duration);

            // Update linear position.
            Position.AddScaledVector(Velocity, duration);

            // Update angular position.
            Orientation.AddScaledVector(Rotation, duration);

            // Normalise the orientation, and update the matrices with the new position and orientation.
            CalculateDerivedData();

            // Clear accumulators.
            ClearAccumulators();

            // Update the kinetic energy store, and possibly put the body to sleep.
            if (canSleep)
            {
                double currentMotion = Velocity.ScalarProduct(Velocity) + Rotation.ScalarProduct(Rotation);

                double bias = System.Math.Pow(0.5, duration);
                motion = bias * motion + (1 - bias) * currentMotion;

                if (motion < Core.SleepEpsilon)
                {
                    SetAwake(false);
                }
                else if (motion > 10 * Core.SleepEpsilon)
                {
                    motion = 10 * Core.SleepEpsilon;
                }
            }
        }

        #endregion
        
        #region Accessor Functions for the Rigid Body's State
        
        public bool HasFiniteMass()
        {
            return InverseMass >= 0.0f;
        }
        
        public void SetInertiaTensor(Matrix3 inertiaTensor)
        {
            InverseInertiaTensor.SetInverse(inertiaTensor);
        }
        
        public void GetInertiaTensor(Matrix3 inertiaTensor)
        {
            inertiaTensor.SetInverse(InverseInertiaTensor);
        }
        
        public Matrix3 GetInertiaTensor()
        {
            Matrix3 it = new Matrix3();
            GetInertiaTensor(it);
            return it;
        }
        
        public void GetInertiaTensorWorld(Matrix3 inertiaTensor)
        {
            inertiaTensor.SetInverse(InverseInertiaTensorWorld);
        }
        public Matrix3 GetInertiaTensorWorld()
        {
            Matrix3 it = new Matrix3();
            GetInertiaTensorWorld(it);
            return it;
        }
        
        void SetInverseInertiaTensor(Matrix3 inverseInertiaTensor)
        {
            inverseInertiaTensor = new Matrix3(InverseInertiaTensor);
        }
        void GetInverseInertiaTensor(Matrix3 inverseInertiaTensor)
        {
            inverseInertiaTensor = new Matrix3(InverseInertiaTensor);
        }
        
        Matrix3 GetInverseInertiaTensor()
        {
            return new Matrix3(InverseInertiaTensor);
        }
        
        void GetInverseInertiaTensorWorld(Matrix3 inverseInertiaTensor)
        {
            inverseInertiaTensor = new Matrix3(InverseInertiaTensorWorld);
        }
        
        Matrix3 GetInverseInertiaTensorWorld()
        {
            return new Matrix3(InverseInertiaTensorWorld);
        }

        public void SetDamping(double linearDamping, double angularDamping)
        {
            LinearDamping = linearDamping;
            AngularDamping = angularDamping;
        }
        
        public void SetPosition(Vector3 position)
        {
            Position.x = position.x;
            Position.y = position.y;
            Position.z = position.z;
        }

        public void SetPosition(double x, double y, double z)
        {
            Position.x = x;
            Position.y = y;
            Position.z = z;
        }
        
        public void GetPosition(Vector3 position)
        {
            position.x = Position.x;
            position.y = Position.y;
            position.z = Position.z;
        }
        
        public Vector3 GetPosition()
        {
            return new Vector3(Position);
        }
        
        public void SetOrientation(Quaternion orientation)
        {
            Orientation.r = orientation.r;
            Orientation.i = orientation.i;
            Orientation.j = orientation.j;
            Orientation.k = orientation.k;
            Orientation.Normalize();
        }
        
        public void SetOrientation(double r, double i, double j, double k)
        {
            Orientation.r = r;
            Orientation.i = i;
            Orientation.j = j;
            Orientation.k = k;
            Orientation.Normalize();
        }
        
        public void GetOrientation(Quaternion orientation)
        {
            orientation.r = Orientation.r;
            orientation.i = Orientation.i;
            orientation.j = Orientation.j;
            orientation.k = Orientation.k;
        }
        
        public Quaternion GetOrientation()
        {
            return new Quaternion(Orientation);
        }
        
        public void GetOrientation(Matrix3 matrix)
        {
            GetOrientation(matrix.Data);
        }
        
        public void GetOrientation(double[] matrix)
        {
            matrix = new double[9];
            matrix[0] = TransformMatrix.Data[0];
            matrix[1] = TransformMatrix.Data[1];
            matrix[2] = TransformMatrix.Data[2];

            matrix[3] = TransformMatrix.Data[4];
            matrix[4] = TransformMatrix.Data[5];
            matrix[5] = TransformMatrix.Data[6];

            matrix[6] = TransformMatrix.Data[8];
            matrix[7] = TransformMatrix.Data[9];
            matrix[8] = TransformMatrix.Data[10];
        }
        
        public void GetTransform(Matrix4 transform)
        {
            TransformMatrix.Data.CopyTo(transform.Data, 0);
        }
        
        public void GetTransform( /*real matrix[16]*/ double[] matrix)
        {
            matrix = new double[16];
            TransformMatrix.Data.CopyTo(matrix, 0);
            matrix[12] = matrix[13] = matrix[14] = 0;
            matrix[15] = 1;
        }
        
        void GetGLTransform( /*float matrix[16]*/ float[] matrix)
        {
            matrix[0] = (float)TransformMatrix.Data[0];
            matrix[1] = (float)TransformMatrix.Data[4];
            matrix[2] = (float)TransformMatrix.Data[8];
            matrix[3] = 0;

            matrix[4] = (float)TransformMatrix.Data[1];
            matrix[5] = (float)TransformMatrix.Data[5];
            matrix[6] = (float)TransformMatrix.Data[9];
            matrix[7] = 0;

            matrix[8] = (float)TransformMatrix.Data[2];
            matrix[9] = (float)TransformMatrix.Data[6];
            matrix[10] = (float)TransformMatrix.Data[10];
            matrix[11] = 0;

            matrix[12] = (float)TransformMatrix.Data[3];
            matrix[13] = (float)TransformMatrix.Data[7];
            matrix[14] = (float)TransformMatrix.Data[11];
            matrix[15] = 1;
        }
        
        public Matrix4 GetTransform()
        {
            return new Matrix4(TransformMatrix);
        }
        
        public Vector3 GetPointInLocalSpace(Vector3 point)
        {
            return TransformMatrix.TransformInverse(point);
        }
        
        public Vector3 GetPointInWorldSpace(Vector3 point)
        {
            return TransformMatrix.Transform(point);
        }
        
        public Vector3 GetDirectionInLocalSpace(Vector3 direction)
        {
            return TransformMatrix.TransformInverseDirection(direction);
        }
        
        public Vector3 GetDirectionInWorldSpace(Vector3 direction)
        {
            return TransformMatrix.TransformDirection(direction);
        }
        
        public void SetVelocity(Vector3 velocity)
        {
            Velocity.x = velocity.x;
            Velocity.y = velocity.y;
            Velocity.z = velocity.z;
        }

        public void SetVelocity(double x, double y, double z)
        {
            Velocity.x = x;
            Velocity.y = y;
            Velocity.z = z;
        }
        
        public void GetVelocity(Vector3 velocity)
        {
            velocity.x = Velocity.x;
            velocity.y = Velocity.y;
            velocity.z = Velocity.z;
        }
        
        public Vector3 GetVelocity()
        {
            return new Vector3(Velocity);
        }

        public void AddVelocity(Vector3 deltaVelocity)
        {
            Velocity += deltaVelocity;
        }

        public void SetRotation(Vector3 rotation)
        {
            Rotation.x = rotation.x;
            Rotation.y = rotation.y;
            Rotation.z = rotation.z;
        }
        
        public void SetRotation(double x, double y, double z)
        {
            Rotation.x = x;
            Rotation.y = y;
            Rotation.z = z;
        }
        
        public void GetRotation(Vector3 rotation)
        {
            rotation.x = Rotation.x;
            rotation.y = Rotation.y;
            rotation.z = Rotation.z;
        }
        
        public Vector3 GetRotation()
        {
            return new Vector3(Rotation);
        }
        
        void AddRotation(Vector3 deltaRotation)
        {
            Rotation += deltaRotation;
        }
        
        public bool GetAwake()
        {
            return isAwake;
        }
        
        public void SetAwake(bool awake = true)
        {
            if (awake)
            {
                isAwake = true;
                
                motion = Core.SleepEpsilon * 2.0f;
            }
            else
            {
                isAwake = false;
                Velocity.Clear();
                Rotation.Clear();
            }
        }

        public bool GetCanSleep()
        {
            return canSleep;
        }

        public void SetCanSleep(bool canSleep = true)
        {
            this.canSleep = canSleep;

            if (!canSleep && !isAwake)
            {
                SetAwake();
            }
        }

        #endregion
        
        #region Retrieval Functions for Dynamic Quantities

        
        void GetLastFrameAcceleration(Vector3 linearAcceleration)
        {
            linearAcceleration.x = LastFrameAcceleration.x;
            linearAcceleration.y = LastFrameAcceleration.y;
            linearAcceleration.z = LastFrameAcceleration.z;
        }
        
        Vector3 GetLastFrameAcceleration()
        {
            return new Vector3(LastFrameAcceleration);
        }

        #endregion
        
        #region Force, Torque and Acceleration Set-up Functions
            
        public void ClearAccumulators()
        {
            ForceAccum.Clear();
            TorqueAccum.Clear();
        }
        
        public void AddForce(Vector3 force)
        {
            ForceAccum += force;
            isAwake = true;
        }
        
        public void AddForceAtPoint(Vector3 force, Vector3 point)
        {
            Vector3 pt = point;
            pt -= Position;

            ForceAccum += force;
            TorqueAccum += pt.CrossProduct(force);

            isAwake = true;
        }
        
        public void AddForceAtBodyPoint(Vector3 force, Vector3 point)
        {
            Vector3 pt = GetPointInWorldSpace(point);
            AddForceAtPoint(force, pt);
        }
        
        void AddTorque(Vector3 torque)
        {
            TorqueAccum += torque;
            isAwake = true;
        }
        
        public void SetAcceleration(Vector3 acceleration)
        {
            this.Acceleration.x = acceleration.x;
            this.Acceleration.y = acceleration.y;
            this.Acceleration.z = acceleration.z;
        }
        
        public void SetAcceleration(double x, double y, double z)
        {
            Acceleration.x = x;
            Acceleration.y = y;
            Acceleration.z = z;
        }
        
        public void GetAcceleration(Vector3 acceleration)
        {
            acceleration.x = this.Acceleration.x;
            acceleration.y = this.Acceleration.y;
            acceleration.z = this.Acceleration.z;
        }
        
        public Vector3 GetAcceleration()
        {
            return new Vector3(Acceleration);
        }

        #endregion
    }
}
