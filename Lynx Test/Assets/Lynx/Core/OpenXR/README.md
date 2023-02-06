# Lynx OpenXR

## Description

Provide OpenXR support and helpers for configuration.


## Setup project (from scratch)

1. Import SDK Lynx-Core unitypackage

2. Select Lynx > Apply Android settings > ARM64
		It will target Android platform form ARM64 and automatically configure your project.

3. Select Lynx > OpenXR > Install OpenXR package
		It will install _XR Plung-in Manager_, _OpenXR_ and enable XR2 support.

4. Under _XR Plung-in Manager_, enable _OpenXR_ (be sure the selected tab is Android)

5. Under _OpenXR_
	* Add _Khronos Simple Controller Profile_ in _Interaction Profiles_
	* Enable _Lynx-R1_ (be sure the selected tab is Android)
	
## Setup project (from existing)

1. Import SDK Lynx-Core unitypackage

2. Select Lynx > Apply Android settings > ARM64
		It will target Android platform form ARM64 and automatically configure your project.

3. Select Lynx > OpenXR > Enable XR2 support
		It will install _XR Plung-in Manager_, _OpenXR_ and enable XR2 support.
		
4. Be sure OpenXR plug-in Managemer and OpenXR are installed.

5. Under _XR Plung-in Manager_, enable _OpenXR_ (be sure the selected tab is Android)

6. Under _OpenXR_
	* Add _Khronos Simple Controller Profile_ in _Interaction Profiles_
	* Enable _Lynx-R1_ (be sure the selected tab is Android)

		
## Scene setup

1. Use XR camera
	*  XR > Convert Main Camera To XR Rig