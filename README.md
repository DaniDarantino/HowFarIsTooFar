# PutMeThere
This project contains the source code for the user study of the paper: "How Far is Too Far? The Trade-Off between Selection Distance and Accuracy during Teleportation in Immersive Virtual Reality" (doi: https://doi.org/10.1109/TVCG.2025.3632345).

# Run the Application without Unity
There are two ways to run the final version of our Application without building the software yourself via Unity.
## 1. With VR Headset
Go to the Build folder and unzip the "AndroidRelease.zip" file. Install the .apk file on a Meta Quest 3 VR Headset. Start the application and follow the instructions presented on screen.
Controls are as follows: To select the handedness, select the appropriate hand icon using the controller's trigger button. Click through the user interface using any of the face buttons (A/B or Y/X) depending on the chosen handedness.
In the river scene, you can teleport by holding the trigger button. After aiming, releasing the trigger button will result in a teleport or an error sound being played, depending on the chosen target location.

## 2. Without VR Headset
If you still want to explore the application of our user study without a VR Headset, go to the Build folder, unzip the "WindowsRelease.zip" file, and double-click the "PutMeThere.exe" file, which should launch the application with the Unity XR Device Simulator enabled.
Controls are as follows: to open the controls overview, click the + icon in the bottom-left corner of the screen (it might already be expanded by default). With the Tab key, you can switch between controlling the HMD, the left, or the right controller. To advance to the river scene, e.g., press tab twice to control the right controller, then press N to click through the initial user interfaces. In the river scene, you can teleport by holding the left mouse button, move the mouse to aim, and then let go of the left mouse button to teleport (make sure the controller is selected, the XR Device Simulator control panel should visualize this accordingly, by pressing tab you can cycle through the different devices).

# Build the application with Unity

The application targets only **Windows 10/11** (MacOS or Linux versions can potentially be build via the Unity Editor) and can be deployed to any VR system compatible with the **OpenXR** API,
although minimum modifications may be necessary to support non-tested hand controllers

- The builds have been tested with **Meta Quest 3** (via Link/AirLink)** w/ Touch controllers.
- Download Unity Hub from [Unity Hub](https://unity.com/unity-hub)
  - You will need a Unity account to log in.
- Once logged in, navigate to the **Projects** tab and click **Add**.
- Select the root folder of this git repository.
- Unity will prompt you to install the correct version for this project (Unity 2022.3.29f1).
- Install the specified Unity version.
  - **Note:** Installing Visual Studio is optional and only required if you plan to edit code.
- After the installation is complete, open the project through Unity Hub.
- Within the project, navigate to the **Scenes** folder and double-click on **SettingsScene** to load the starting scene.
- Press play in the toolbar to launch the application starting at the **SettingsScene**

Per default the XR Device Simulator is activated, which allows you to test the application without a VR Headset. To disable it go to Edit->Project Settings Select XR Interaction Toolkit under XR Plug-in Management in the left panel and disable `Use XR Device Simulator in scenes`
Note that all assets were removed from the unity project files to avoid copyright issues.
