# Spectra Cities
## General

Demo project to allow viewing the city in VR.<br>
Please note: Only basic optimizations have been done. A high end graphics card is required to run the project at the moment(for example NVidia RTX 3070 or higher).
<br><br>
Unity editor version: 2021.3.6f1<br>
<br>
Please use the discussions tab for questions.

## Scenes
### Physical City
The main scene can be found here: "Assets/_Spectra_Cities/_Scenes/SC_Masterplan_Demo.unity".
### Virtual City
The virtual city (night) scene can be found here: "Assets/_Spectra_Cities/_Scenes/SC_Masterplan_Demo_Night_Complete.unity".
### Apartments
The apartments are in this scene: "Assets/_Spectra_Cities/_Scenes/Apartment demo.unity".
### Concert
The concert scene is located here: "Assets/_Spectra_Cities/_Scenes/SC_Masterplan_Demo_Night.unity".

## Branches
Feature branches should follow this naming convention: YY-MM_FeatureName<br>
Example: 22-08_BuildingUpdates<br>
The "Development" branch contains the latest changes.<br>
The "main" branch contains builds.<br>

## Structure
### Modules
Modules should have a parent prefab, that can be added to scenes. There should be a "[Main Modules]" object in every scene containing the project module prefabs.<br>
Ideally all the dependencies of a module are included in the module folder <br>
(for example: "Assets/_VIRTUAL_CITY/Modules/Boundary").<br>
Assets and dependencies that are shared across different modules should be in a folder for shared assets.
### Assets & Folders
The asset & folder structure will change with future optimizations. Please do not add files to the _Spectra_Cities and _VIRTUAL_CITY folders.
