# DroneSimulation

This project serves as a supplemental material for the following paper: J.H. Plümer, M. Tatzgern. "Towards a Framework for Validation XR Prototyping for Performance Evaluations of Simulated User Experiences", 2023 IEEE International Symposium on Mixed and Augmented Reality (ISMAR), Sydney, Australia, 2023, pp. X-X, doi: XXX.

The project represents the source code utilized for performing the user study described in the paper. Please cite the paper above when utilizing this project.

# Requirements

- Varjo-XR 3 and a PC capable of driving it (see [Hardware-Requirements](
  https://varjo.com/use-center/get-started/varjo-headsets/system-requirements/xr-3-vr-3/))
  - Varjo Base Software Version used: 3.10.0.6
  - Varjo-XR 3 Firmware Version used: 0.17.0.469
  - Varjo Unity XR Plugin Version used: 3.5.0
  - PC must be able to connect to the wireless network the drone is providing for the REAL and MED conditions.
- Unity 2022.2.16f installed
- Access to the [_Yue Ultimate FPV Drone Physics_](
  https://assetstore.unity.com/packages/tools/physics/yue-ultimate-fpv-drone-physics-231651)-Asset (Version 1.0)
- Steam-VR Room Setup ready and configured with one paired VIVE Tracker
  - Steam-VR Software Version used: 1.25.6
  - Steam Base-Station Generation used: 2.0
  - VIVE Tracker Generation used: 2018
- XBox-Controller, connected to the PC, for steering the drone
- [Ryze Tello](https://www.ryzerobotics.com/de/tello) drone for the REAL and MED conditions

## Room Model

For the MED condition, the real room was overlaid with a virtual representation of itself, utilizing a VIVE Tracker. To do this, the real room was first measured and then remodelled in Unity. Hence, the room model will not match other settings and needs to be adapted for any use in other scenarios. 

# Preparing the project

This project is dependent on the drone asset mentioned [above](#requirements). After including it, small changes are required in _YueDronePhysics.cs_:

---
Delete:
````csharp
[RequireComponent(typeof(YueInputModule))]
````
and change
````csharp
public class YueDronePhysics : MonoBehaviour
````
to: 
````csharp
internal abstract class YueDronePhysics : MonoBehaviour
````
---
Change
````csharp
private YueInputModule inputModule;
````
to:
````csharp
protected InputModule inputModule;
````
and fix the resulting error in `Start()` by changing
````csharp
inputModule = GetComponent<YueInputModule>();
````
to:
````csharp
inputModule = GetComponent<InputModule>();
````
---
Change
````csharp
private void Start()
````
to:
````csharp
protected void Start()
````
and provide a new start rotation by changing
````csharp
targetQuad.SetPositionAndRotation(transform.position, transform.rotation * Quaternion.Euler(0, 90, 0));
````
to: 
````csharp
targetQuad.SetPositionAndRotation(transform.position, transform.rotation * Quaternion.Euler(0, 0, 0));
````
---
Change:
````csharp
private Rigidbody rb;
````
to:
````csharp
protected Rigidbody rb;
````

# How to use the program

The entry point of the program is the _Menu Scene_. 
After launch, `SceneType` and `TaskType` can be configured and a participant ID (numeric) must be provided. 
This is also the default screen after a participant completed a condition and the corresponding data was saved.
From here, the program can be closed and the collected data can be exported to a `.csv`-file.

_Main Scene_ enables the participants to fly the drones and to complete the conditions.
There are some optional buttons for special cases, e. g., to log crashes or to reconnect to the drone.
On the right side, the experimenter can type in the accuracies per landing for the REAL and MED conditions.
**ATTENTION**: Do not do this, while the participant is still flying, as selecting the input fields can interfere
with the participants ability to control the drone.

# How to fly the drone

> This only works wile the _Main Scene_ is active.
  In case of REAL and MED conditions, the PC must also be connected to the drone's wireless network.

For each condition, the _Menu_-Button needs to be pressed once, initially. 
Afterwards, the drone can be started with pressing _A_ and landed with _B_. 
Use the left stick to control the drones horizontal position (forward, backwards, left, right).
Use the right stick to control the drones vertical position (up, down).

# How to collect the data

After pressing the _Export to CSV_-Button within the _Menu Scene_, all collected data will be exported
to an _data.csv_-file, which can be found at the following path:
> C:\Users\<your_username>\AppData\LocalLow\Fachhochschule Salzburg GmbH\TFVXRP\Savegames\

This directory also contains the raw data in json-Format, partly separated per participant ID.

# License
This project is licensed under [The MIT License](LICENSE.md).

# Attributions

Knowledge, orientation and ideas of how to connect and control the _Tello_-drone were gained from:
- [TelloLib](https://github.com/Kragrathea/TelloLib)
- [TelloForUnity](https://github.com/comoc/TelloForUnity)
- [UnityControllerForTello](https://github.com/carter-james89/UnityControllerForTello)

The final implementation is based on and utilizing the [Tello SDK User Guide](
https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf).

