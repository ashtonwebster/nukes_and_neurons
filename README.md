# Project Name

What we used from other people:
* Cubiquity (example environment city, forest, and voxel helper methods)
* First Person Shooter (Standard Assets)
* AForge.NET (Neural network libraries)

Unless otherwise indicated, all other code is ours (i.e. everything under scripts).

## Main Features

Our game is a simple first person shooter. Our twist is that the game uses a variety of grenade/bomb-like projectiles instead of conventional weapons in a fully voxelized, dynamic, destructable environment. It supports additional controllers (built for Logitech Gamepad) for local multiplayer, and includes an AI enemy to fight against that learns from the human. 

### Usage

Gameplay starts with a 3D voxel menu for the user to choose between "PLAY" and "TRAIN" (we'll cover TRAIN in the imitation learning section). PLAY creates two more menus to choose between fighting agains the AI that has been trained, or fighting against a local multiplayer. A random voxel world is chosen among a forest world, city world, and a star wars world, and the two users are spawned in it. Hitting "Q" at any time takes the user back to the main menu. 

### Destructable Surroundings

All of the voxel worlds are fully destructable and playable. 

### Multiple Bomb Types

### Controller Support

### Imitation Learning AI

The AI is trained using "imitation learning", where it receives pairs of input output pairs of the form (game state, expert action).  Then, a supervised learning model is constructed from a simple one layer feedforward network.  This model learns how the human behaves in a variety of game states and tries to mimic the human behavior.  The benefit of this method over conventional methods (e.g. the if-else methods of AI) is that multiple behaviors can be learned and complicated, often manually determined rules for behavior can be learned instead.

The training pairs are collected by recording a human as they play against another human or a previous version of the AI.  This training can be performed in any map, but for simplicity we use a simplified "flatland" map where there are no obstacles.  Training data is then written to a file which can be later read back for training the model.  Here is an example of a line from the file, with headers:

```
XZangleToObj,YZangleToObj,DistToObj|horizontalPan,forwardPan,xRotInput,yRotInput,sprintButtonDown,fireButtonDown,jumpButtonDown
10.08294,-2.662269,17.4743|0.1380242,0.9209551,-0.25,-0.7,1,0,0
```

This line has two parts, divided by a `|` character.  The left half is the Game State Summary, which records the angle to the target object in the XZ and YZ plane and the distance to the object.  The right half is all of the actions being performed by the user for this frame, including panning, rotation, sprinting, firing, and jumping.  It is important to note that the AI uses exactly the same interface as the human for performing actions; the device used is just delegated to the model instead of the controller or the mouse.  All training data and models are in the `training_data` folder.

Training data is then used to build a single hidden layer neural network.  This supervised learning model was selected because it has been successful with other game based learning tasks and allows for multiple outputs (otherwise we would have had to train numerous independent models).  We used the implementation from [AForge.Net](http://www.aforgenet.com/framework/features/neural_networks.html).The model is trained for 1000 epochs of the full available data, ignoring lines where no action is taken. Generally this takes about 1 minute on a commodity laptop.  The model is then saved to a `.bin` file which is a serialized version of the model.  This serialized version loads almost instantaneously, and if a trained model is passed it is used instead of re-training from the data itself.

The resulting model is accessed by the AIController, which basically just wraps the inputs to the first person controller by querying the model at each time step.  Rotation is handled by the `AIMouseLook.cs` file, which overwrites the usual `MouseLook.cs` behavior.  This AIController can be used in any environment, but because it has no environmental awareness, it performs best in the flatland map.  Additional features could make the neural network more robust to these environmental features.

Training configuration options are controlled by a configuration JSON file in `config/CONFIG.json`.  These parameters include the number of hidden units in the neural network (default 50), output file for training data, output file for training model, and other features.  Thus, new models can be trained without any code modifications by simply modifying the CONFIG file.  More details about the config file is described in the `config/CONFIG.README.md` file.


