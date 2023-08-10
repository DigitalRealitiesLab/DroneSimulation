namespace TFVXRP.Input {
    /// <summary>
    ///     Vector4 with roll, pitch, yaw and elevation.
    /// </summary>
    internal readonly struct InputVector {
        internal readonly float Roll;
        internal readonly float Pitch;
        internal readonly float Yaw;
        internal readonly float Thrust;

        internal bool IsZero => Roll == 0 && Pitch == 0 && (Yaw == 0) & (Thrust == 0);

        internal InputVector(float roll, float pitch, float yaw, float thrust) {
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
            Thrust = thrust;
        }
    }
}