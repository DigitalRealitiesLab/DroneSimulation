using System;
using TFVXRP.DataCollection;
using UnityEngine;

namespace TFVXRP.Input {
    internal abstract class InputReceiver : MonoBehaviour {
        protected InputVector input;
        internal Action<Vector3> SaveSnapshot { get; set; }

        internal Action<Vector3, Quaternion> Respawn { get; set; }

        protected virtual void Update() => ProcessInput();

        internal void OnReceive(InputVector inputVector) => input = inputVector;

        internal virtual void OnStartup() => DataCollectionManager.Log(DataCollectionManager.LogType.StartingVehicle);

        protected abstract void ProcessInput();

        internal virtual void OnShutdown() => DataCollectionManager.Log(DataCollectionManager.LogType.StoppingVehicle);
    }
}