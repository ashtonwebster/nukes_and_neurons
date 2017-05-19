# Nukes and Neurons

## Main Features

Our game is a simple first person shooter. Our twist is that the game uses a variety of grenade/bomb-like projectiles instead of conventional weapons in a fully voxelized, dynamic, destructable environment. It supports additional controllers (built for Logitech Gamepad) for local multiplayer, and includes an AI enemy to fight against that learns from the human. Unlike traditionally programmed AI, our AI solution contains no domain knowledge about movement, gameplay, or game mechanics; it learns these things by watching the humans play via an imitation learning neural network.

[Watch the Demo video](https://www.youtube.com/watch?v=b-7VWuQGQMw)

### Usage

Gameplay starts with a 3D voxel menu for the user to choose between "PLAY" and "TRAIN" (we'll cover TRAIN in the imitation learning section). PLAY creates two more menus to choose between fighting agains the AI that has been trained, or fighting against a local multiplayer. When gameplay begins, a random voxel world is chosen among a forest world, city world, and a star wars world, and the two playerss are spawned randomly in it. Hitting "Q" at any time takes the user back to the main menu.

### Destructable Surroundings

All of the voxel worlds are fully interactable and destructable. As explosives hit the environment, the voxels are dislodged, blasted around, and eventually disappear.  Over time, large portions of the environment may be destroyed and holes may form. Be careful - if a user falls through a hole (of off the map), they die. This adds an interesting dynamic to the game where players can try to trap their opponents on islands by fully destroying the land around them. The City World and Forest world are standard voxel assets that come bundled with Cubiquity; however, the Star Wars world was put together by us by combining 6 prebuilt models from Sketchfab and converting it into a format Cubiquity could understand. We also adatped existing Cubiquity source code to allow for projectiles to destroy the voxel environment.

### Multiple Bomb Types

There are three different and fun types of bombs available:

* *Standard Bombs*: These bombs explode on impact (usually another player or voxel surface).  
* *Grenades*: These explode after a set amount of time (3 seconds). They're easier to evade, since you have time to escape and see them coming, but as a result, they are much stronger than the standard bombs and explosively destroy large portions of the map.
* *Trip Mines*: These explode when the user or another grenade enter their radius. As they're the easiest to avoid, they also do the most damage - they often kill the opponent instantly.

The mouse/keyboard controller can switch between them with keys [1, 2, 3] to select each bomb type, while the controller can cycle through them with the "B" button. There are nolimitations on ammo to limit gameplay, so users can throw as many as needed! Other objects (on death for players and on explosion for bombs) explode into voxels to add to the effect.

### Controller Support

We purchased a Logitech controller (Gamepad F370) and use it for local split screen gameplay. The inputs are fully abstracted away, so players in the game can be controlled via the mouse/keyboard, the controller, or by the AI neural network. This also means it's fully adaptable so that different controls or other inputs can be used in the future.

### Imitation Learning AI

The AI is trained using "imitation learning", where it receives pairs of input output pairs of the form (game state, expert action).  Then, a supervised learning model is constructed from a simple one layer feedforward network.  This model learns how the human behaves in a variety of game states and tries to mimic the human behavior.  The benefit of this method over conventional methods (e.g. the if-else methods of AI) is that multiple behaviors can be learned and complicated, often manually determined rules for behavior can be learned instead.

The training pairs are collected by recording a human as they play against another human or a previous version of the AI. This training can be performed in any map, but for simplicity we use a simplified "flatland" map where there are no obstacles. This can be accessed using the TRAIN menu option at the beginning of the game.  Training data is then written to a file which can be later read back for training the model. Here is an example of a line from the file, with headers:

```
XZangleToObj,YZangleToObj,DistToObj|horizontalPan,forwardPan,xRotInput,yRotInput,sprintButtonDown,fireButtonDown,jumpButtonDown
10.08294,-2.662269,17.4743|0.1380242,0.9209551,-0.25,-0.7,1,0,0
```

This line has two parts, divided by a `|` character.  The left half is the Game State Summary, which records the angle to the target object in the XZ and YZ plane and the distance to the object. The right half is all of the actions being performed by the user for this frame, including panning, rotation, sprinting, firing, and jumping. It is important to note that the AI uses exactly the same interface as the human for performing actions; the device used is just delegated to the model instead of the controller or the mouse. All training data and models are in the `training_data` folder.

Training data is then used to build a single hidden layer neural network. This supervised learning model was selected because it has been successful with other game based learning tasks and allows for multiple outputs (otherwise we would have had to train numerous independent models). We used the implementation from [AForge.Net](http://www.aforgenet.com/framework/features/neural_networks.html). The model is trained for 1000 epochs of the full available data, ignoring lines where no action is taken. Generally this takes about 1 minute on a commodity laptop. The model is then saved to a `.bin` file which is a serialized version of the model. This serialized version loads almost instantaneously, and if a trained model is passed it is used instead of re-training from the data itself.

The resulting model is accessed by the AIController, which basically just wraps the inputs to the first person controller by querying the model at each time step. Rotation is handled by the `AIMouseLook.cs` file, which overwrites the usual `MouseLook.cs` behavior. This AIController can be used in any environment, but because it has no environmental awareness, it performs best in the flatland map. Additional features could make the neural network more robust to these environmental features.

Training configuration options are controlled by a configuration JSON file in `config/CONFIG.json`. These parameters include the number of hidden units in the neural network (default 50), output file for training data, output file for training model, and other features. Thus, new models can be trained without any code modifications by simply modifying the CONFIG file. More details about the config file is described in the `config/CONFIG.README.md` file.

## Acknowledgements

We used some libraries and starter code as a basis for this game.

* Cubiquity (example city and forest world, and voxel helper methods)
* First Person Shooter (Standard Assets)
* AForge.NET (Neural network libraries)
* Farland Skies (for skybox)
