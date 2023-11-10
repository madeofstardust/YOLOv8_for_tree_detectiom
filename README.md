# YOLOv8_for_tree_detectiom
A repo containing files I used for my research project at Utrecht University, in which I intended to use artificially generated imagery in order to detect trees on drone/satellite footage.


## Datasets

In the folder "data", datasets used for training YOLOv8 can be found. They are divided into images (.png or .jpg) and labels (.txt files - format is the standard YOLOv8 label format) and into validation and training set.

The folder "Real-world footage" contains real-world images, taken from a drone footage recorded above a forest in Ostfriesland, Germany.

## Files

### Unity files/CaptureAndSaveBounds.cs
In the folder Unity files, a .cs file used for creation of the Unity data can be found. This file is responsible for generating and deleting trees on the scene, changing the colour of the background, moving the camera and capturing screenshots. 

Attach this file to the Plane object. Remember to change the paths to prefabs and materials to your own resources.

### Utils/tsne.ipynb

This file contains code used for tsne analysis. The paths need to be modified to your own dataset.

