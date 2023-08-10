using TFVXRP.Input;

namespace TFVXRP.Drone {
    internal class TelloDroneManager : InputReceiver {
        private void Start() => TelloDrone.Initialize();

        private void OnDestroy() => TelloDrone.DeInitialize();

        internal override void OnStartup() {
            base.OnStartup();
            TelloDrone.Takeoff();
        }

        protected override void ProcessInput() => TelloDrone.Fly(input);

        internal override void OnShutdown() {
            base.OnShutdown();
            TelloDrone.Land();
        }

        internal static void EstablishDroneConnection() => TelloDrone.EstablishConnection();
    }
}