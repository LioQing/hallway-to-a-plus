# Hallway to A+

## Overview

### Introduction

This is a single-player horror game that take place in Haking Wong Building of the University of Hong Kong. In the story, the player attempts to change their grades by sneaking into professor's office at midnight, but found out that there is something strange about the hallways when they are returning. The player has to avoid returning from the hallway with anomalies to avoid being trapped in the hallway.

The game is published on [itch.io](https://lio-qing.itch.io/hallway-to-a-plus) and is available for free. The game is designed to be played on Windows desktop.

### Features

- Realistic 3D scene reconstruction via Gaussian splatting of Haking Wong Building
- Paranormal environment and uncomfortable environment created by modifying the reconstructed scene
- Immersive first-person experience with a variety of mode of exploration
- Available on Windows desktop

### Inspiration

This video game is inspired by the indie horror game, [The Exit 8](https://store.steampowered.com/app/2653790/The_Exit_8/), where the player have to navigate through a haunted hallway that may have anomalies. When the player notice any anomaly in the hallway, they need to return around and walk back to where they started. If they successfully avoided the haunted hallways or walked through normal hallways for 8 consecutive times, they would successfully escape the hallway.

![The Exit 8](media/the_exit_8.png)

## Story

The story starts with a cutscene where the player was telling their friend they have to sneak into professors' offices in Haking Wong Building at midnight to change their grades because they messed up their exams. Their friend then said to them that there are rumors about the hallways being haunted at midnights, they need to avoid anomalies or they would be trapped there. But the player insisted to go.

![Intro Cutscene](media/intro_cutscene.png)

Upon arriving at the Haking Wong Building, the player did not encounter any anomalies at first. However, after they changed their grades and were returning, they started to notice some strange things in the hallways. The player then have to avoid returning from the hallway with anomalies to avoid being trapped in the hallway.

![Anomaly](media/anomaly.png)

After 5 consecutive successful nights, the player will be able to escape the hallways with all A+ on their grades. The game ends with a cinematic cutscene showing that while the player successfully escaped, the anomalies continue to linger in the hallways.

## Gameplay Mechanics

### Controls

The game is designed to have a standard first-person control scheme. THe player can use the following controls to navigate through the environment:

- `W` to move forward
- `S` to move backward
- `A` to move left
- `D` to move right
- `Space` to jump
- `Shift` to sprint
- Move the mouse to look around
- Hold `Left Mouse Button` to use the torch or LiDAR (if available)
- `E` to interact

### Game Rules

When entering the scene, the game first let the player choose to walk through the left or right hallway. At this stage, there is no anomaly in the hallway, it is designed to let the player get familiar with the controls and the environment. The player will walk through the hallway and reach the end of the hallway to enter the professors' office.

![Enter Choice](media/enter_choice.png)

Upon returning, one of the hallways will have an anomaly. The player will have to identify the anomaly and instead return through the other hallway. If the player successfully avoided the haunted hallways, they will be able to escape and proceed to the next night. If they failed to avoid the haunted hallway, when taking the lift to leave the building, they will found themselves to by mysteriously put back to the first night when they were returning from the professors' office.

### Night Progression

Additionally, after each night, the player will find themselves in a different environment and have to navigate through the hallways with different lighting environment. Each nights' environments are as follows:

| Night | Environment   | Screenshot                   |
| ----- | ------------- | ---------------------------- |
| 1     | Normal        | ![Night 1](media/night1.png) |
| 2     | Distance Fade | ![Night 2](media/night2.png) |
| 3     | Torch         | ![Night 3](media/night3.png) |
| 4     | LiDAR         | ![Night 4](media/night4.png) |
| 5     | Normal        | ![Night 1](media/night1.png) |

### Game Flow

Each night's iteration is designed to make the player feel more familiar with the original environment as the environment gets harder to navigate through. The flowchart below shows the flow of the game:

```mermaid
flowchart TD
    Start([Enter Scene])
    Office([Arrive at Professors' Office])
    Return([Return Scene with Anomaly Added])
    Success{Player Avoids Anomaly?}
    NextNight([Increase Night Number])
    LastNight{Night Number = 5?}
    End([End after 5 Nights])
    Fail([Return to First Night])

    Start --> Office
    Office --> Return
    Return --> Success
    Success -- Yes --> NextNight
    NextNight --> LastNight
    LastNight -- No --> Start
    LastNight -- Yes --> End
    Success -- No --> Fail
    Fail --> Return
```

## Aesthetic

### Environment

The graphics design of Hallway To A+ focuses on the environment for providing an immersive and realistic experience. While the technology of Gaussian splatting is not perfect at reconstructing the scene in a photorealistic way, it is able to provide a mixture of realistic and surrealistic environment.

### Horror

The game is designed to be a horror game without any violent scene or jump scares. The atmosphere is created by the use of lighting and uncomfortable environment. Targeted at students and staffs of the University of Hong Kong, the game sets palce in the Haking Wong Building, a building that is relatively familiar to the players. However, with some modifications, the environment is designed to be uncomfortable and surrealistic. The mixture of familiar and unfamiliar environment is designed to create a sense of horror and discomfort. The followings are some of the design choices made:

| Description                                         | Original Environment                          | Modified Environment                          |
| --------------------------------------------------- | --------------------------------------------- | --------------------------------------------- |
| Color of the poster is modified                     | ![Original Poster](media/original_poster.png) | ![Modified Poster](media/modified_poster.png) |
| Color of the door is modified                       | ![Original Door](media/original_door.png)     | ![Modified Door](media/modified_door.png)     |
| Door is removed revealing outside the playable area | ![Original Escape](media/original_escape.png) | ![Modified Escape](media/modified_escape.png) |

There are also a number of anomalies presented without modifying the scene, instead they are driven by scripts, 3D models, and animations. The followings are some of these anomalies presented in the game:

| Description                                                                             | Screenshot                        |
| --------------------------------------------------------------------------------------- | --------------------------------- |
| A mannequin is placed in the hallway, it hides from the player when they approaches.    | ![Mannequin](media/mannequin.png) |
| The environment shrinks and disappears as the player enters the area.                   | ![Shrinks](media/shrinks.png)     |
| Dr. T.W. Chim, the course's lecturer chase the player when the player get close to him. | ![TWChim](media/twchim.png)       |

### Cutscene

The cutscenes are recorded in real life using mobile phone in the Haking Wong Building. Without any conversation, the cutscenes are designed to be simple but effective in conveying anxiety and horror.

With the exception of the ending cutscene, which is recorded with in game rendering. The ending cutscene is designed to be cinematic, requiring smooth camera movement and lighting, bringing the player a sense of accomplishment and relief after escaping the hallways.

### Acknowledgements on Assets

In order to reduce the time of game development, various copyright-free assets are used in the game. The team is grateful for plaggy on OpenGameArt for providing the [mannequin model](https://opengameart.org/content/mannequin-male) and freesound_community on pixabay for providing the [footstep sound effect](https://pixabay.com/sound-effects/footsteps-in-a-hallway-47842/).

## Technology

### Unity Game Engine

The game is developed using Unity. The game engine is chosen because of its flexibility and ease of use. The game is designed to be a first-person horror game, and Unity provides a good framework for developing such games.

### Gaussian Splatting

[Gaussian splatting](https://repo-sam.inria.fr/fungraph/3d-gaussian-splatting/) is used for reconstructing the scene from RGB images taken by the team in the Haking Wong Building. A total of ~1000 images are used for reconstructing the scene. The images are taken in a variety of angles to provide a good coverage of the scene. The 4 areas of the sccene are reconstructed separately for occlusion culling, see the [Unity Gaussian Splatting Plugin](#unity-gaussian-splatting-plugin) for more details. The 4 areas are as follows:

| Area      | Screenshot                                  |
| --------- | ------------------------------------------- |
| Left      | ![Left Area](media/left_area.png)           |
| Right     | ![Right Area](media/right_area.png)         |
| Left Turn | ![Left Turn Area](media/left_turn_area.png) |
| Connect   | ![Connect Area](media/connect_area.png)     |

Additionally, the team also reconstructed Dr. T.W. Chim, the course's professor as one of the anomalies in the game. The images are extracted from 2 videos taken after a lecture with a total length of ~21 seconds. The images are taken in a variety of angles to provide a good coverage of the professor. However, due to a limited number of images and a variety of movements, the reconstruction is not perfect. The reconstructed model is shown below:

| Angle | Screenshot                              |
| ----- | --------------------------------------- |
| Front | ![TWChim Front](media/twchim_front.png) |
| Left  | ![TWChim Left](media/twchim_left.png)   |
| Right | ![TWChim Right](media/twchim_right.png) |
| Back  | ![TWChim Back](media/twchim_back.png)   |

### Unity Gaussian Splatting Plugin

The game is developed using the [Unity Gaussian Splatting](https://github.com/aras-p/UnityGaussianSplatting) plugin. The plugin provides flexible and easy-to-use Gaussian splatting components for rendering the scene.

Gaussian splatting is performance heavy on large scenes, so the team optimized the plugin by using the following techniques:

| Technique                             | Description                                                                                               | Performance Gain (on RTX 2060 Max-Q on Laptop) |
| ------------------------------------- | --------------------------------------------------------------------------------------------------------- | ---------------------------------------------- |
| Cropping                              | The scene is cropped to only render the area that is well lit and visible within the scene.               | ~10 FPS -> ~20 FPS                             |
| Area Segmentation & Occlusion Culling | The scene is segmented into different areas, and only the area that is visible to the camera is rendered. | ~20 FPS -> ~45 FPS                             |

Furthermore, the team also implemented custom components and custom shaders in the plugin to achieve the different navigation environments.

| Environment    | Modification                                                                                                                         |
| ------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| Normal        | No modification                                                                                                                             |
| Distance Fade | Modified the shader to render the object opacity based on the distance to the camera.                                                       |
| Torch         | Modified the shader to render the object opacity based on the distance to the camera and the light source.                                  |
| LiDAR         | Modified the plugin to hide the Gaussian splats and only render them to a second camera, where LiDAR point cloud is constructed on-the-fly. |

### 3D Gaussian Splatting Viewer App

The team developed a custom [3D Gaussian Splatting Viewer App](https://github.com/lioqing/wgpu-3dgs-viewer-app), which also allows editing the scene in real time. Many of the anomalies in the game are designed using this viewer. The viewer provides the ability to edit the color, and remove the Gaussian splats at specific locations. The viewer also provides other features but they are not used in the game.

![3D Gaussian Splatting Viewer App](media/3dgs_viewer_app.png)

### Suno & Audacity

Suno AI is used for generating the background musics while editings are done using Audacity. The background musics are designed to be uncomfortable and horror-like, with a mixture of ambient and noise sounds. The background musics are designed to be played in a loop, with a fade-in and fade-out effect.

## Conclusion

The game is one of the first games developed using Gaussian splatting. The game is designed to be a horror game without any violent scene or jump scares while providing an immersive and surrealistic experience. The game can be played on Windows desktop and is available for free on [itch.io](https://lio-qing.itch.io/hallway-to-a-plus). With more time and effort, games can be hugely benefited from the use of Gaussian splatting. The team is looking forward to seeing more games developed using this technology.