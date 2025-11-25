# PutMeThere
This project contains the source code for the user study of the paper: "How Far is Too Far? The Trade-Off between Selection Distance and Accuracy during Teleportation in Immersive Virtual Reality" (doi: https://doi.org/10.1109/TVCG.2025.3632345).

# Replicate the User Study
There are two ways to replicate our user study.
## 1. Replication with VR Headset
Download the .apk file that we published on Zenodo: https://doi.org/10.5281/zenodo.15094087 and install it on a Meta Quest 3 VR Headset. Start the application and follow the instructions presented on Screen.
Controlls are as follows: To select the handedness, select the appropriate hand icon using the controllers trigger button. Click through the userr interface using any of the face buttons (A/B or Y/X) depending on the choosen handedness.
In the river scene you can teleport by holding the trigger button. After aiming, releasing the trigger button will result in a teleport or an error sound being played depending on the chosen target location.

## 2. Replication without VR Headset
If you still want to explore the application of our user study without acess to a VR Headset, go to the Build folder unzip the "WindowsRelease.zip" file and double click the "PutMeThere.exe" file which should launch the application with the Unity XR Device Simulator activated.
Controlls are as follows: to open the controlls overview click on the + icon in the bottom left of the screen (it might already be expanded by default). With the Tab key, you can switch between controlling the HMD, the left or the right controller. To advance to the river scene e.g. press tab twice to controll the right controller, then press N to to click through the initial user interfaces. On the river scene you can teleport by holding the left mouse button, move the mouse to aim and then let go of the left mouse button to teleport (make sure the controller is selected, the XR Device Simulator controll panel should visualize this accorrdingly, by pressing tab you can cycle through the different devices).

# Explore the sourc code.

To run the project in unity, simply clone it and open it within the unity hub. The project uses Unity version 2022.3.29 and was desgined to be used with a Meta Quest 3 headset, but should also run on any other VR headset supporting Open XR.
When you launch the project via the unity editor make sure to start it at the "SettingsScene" in order to properly initialize all parameters. Per default the XR Device Simulator is activated so it runs without a VR headset.

Note that all assets were removed to avoid copyright issues. You can downloald the .apk file (which can be installed on a standalone VR headset) that was used during the user study here: https://doi.org/10.5281/zenodo.15094087.
