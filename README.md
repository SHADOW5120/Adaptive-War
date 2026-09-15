Adaptive War
2D Strategy Game Integrated with Reinforcement Learning

📖 Introduction

Adaptive War is a 2D isometric strategy game developed with the Unity Engine, with a primary focus on applying Reinforcement Learning (RL) to game AI.

Unlike conventional NPCs that rely entirely on predefined scripts and behaviors, Adaptive War integrates a machine learning model trained with Proximal Policy Optimization (PPO). The model enables NPCs to make adaptive combat decisions based on the current game state.

The project combines traditional game AI techniques with modern Reinforcement Learning, creating a layered AI architecture that balances strategic control, tactical decision-making, and computational efficiency.

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/b3389f25-8f09-4b78-9b49-8b065a453a89" />
Main Menu

✨ Key Features
🧠 Adaptive AI

Integrates a machine learning model directly into Unity using the ONNX format.

NPCs can select actions based on the current game state instead of relying exclusively on fixed behavior scripts.

The PPO model is trained offline and deployed during gameplay for inference.

🏗️ Layered AI Architecture

The AI system is divided into three layers:

Layer	Technologies	Responsibility
Macro Layer	Behavior Tree, Rule-Based System	Controls overall strategy, game flow, and high-level behavior
Micro Layer	FSM, PPO Neural Network	Controls individual unit combat decisions
Support Layer	A* Pathfinding	Calculates navigation paths around obstacles

This architecture allows Reinforcement Learning to work together with deterministic game logic instead of replacing the entire AI system.

🎮 2D Isometric Gameplay

2D isometric environment and characters.

8-directional unit movement.

Smooth character animation using Unity Blend Trees.

Obstacles and navigable environments supported by A* pathfinding.

Strategic combat and unit deployment.

📱 Cross-Platform Architecture

The project is designed with both PC and mobile (Android) deployment in mind.

🧠 Artificial Intelligence Architecture

Adaptive War uses a Layered AI Architecture that combines multiple AI techniques according to their strengths.

<img width="1024" height="559" alt="image" src="https://github.com/user-attachments/assets/7a52438c-ea26-44ee-aefb-8111353099e1" />
Layered AI Architecture

Macro Layer

The Macro Layer manages high-level NPC behavior and game strategy.

It uses:

Behavior Tree

Rule-Based System

The purpose of this layer is to determine the overall behavior and control flow of NPCs while ensuring that AI decisions remain consistent with the game's rules.

Micro Layer

The Micro Layer is responsible for tactical decisions made by individual units.

It combines:

Finite State Machine (FSM)

PPO Neural Network

The PPO model provides an action decision, while the FSM and existing game logic handle the execution of that decision inside Unity.

Support Layer

The Support Layer handles navigation and movement.

It uses the A* Pathfinding Algorithm to calculate paths through the isometric environment while avoiding obstacles.

🔄 NPC Decision-Making Flow

During gameplay, an NPC continuously observes the environment and uses the current state to determine its next action.

The simplified process is:

Game Environment
       │
       ▼
Extract Environment Features
       │
       ▼
Actor-Critic Neural Network
       │
       ▼
Select Action
       │
       ▼
Behavior Tree / FSM
       │
       ▼
Game Logic
       │
       ▼
Animation / Movement / Combat

1. Environment Observation

The AI extracts information describing the current state of the NPC and its opponent.

Example features include:

NPC health

Opponent health

X-axis distance

Y-axis distance

Total distance

Attack-range status

Other game-state information required by the trained model

2. Neural Network Inference

The extracted features are passed into the trained Actor-Critic neural network.

The policy selects one of 10 possible actions:

Attack

Idle

Move Up

Move Down

Move Left

Move Right

Move Up-Left

Move Up-Right

Move Down-Left

Move Down-Right

3. Action Execution

The selected action is passed to the game's existing AI architecture.

The Behavior Tree and FSM coordinate the execution of the decision, allowing the neural network to influence NPC behavior without bypassing the game's core logic.

<img width="4008" height="3348" alt="image" src="https://github.com/user-attachments/assets/774d2cef-b1d0-4f87-81f6-2a1722594588" />
Behavior Tree

🤖 Reinforcement Learning
PPO — Proximal Policy Optimization

The Adaptive War agent is trained using Proximal Policy Optimization (PPO), a policy-gradient Reinforcement Learning algorithm.

The model uses an Actor-Critic architecture:

Actor — learns the policy and determines which action should be selected.

Critic — estimates the value of the current state and helps evaluate the quality of the Actor's decisions.

During training, the agent interacts with the environment, receives rewards, and gradually improves its policy.

Generalized Advantage Estimation

The training process uses Generalized Advantage Estimation (GAE) to estimate the advantage of actions and improve training stability.

The overall training pipeline can be summarized as:

Game Environment
       │
       ▼
State Observation
       │
       ▼
Actor-Critic Network
       │
       ▼
Action
       │
       ▼
Reward
       │
       ▼
GAE / PPO Update
       │
       └──────────► Repeat

📊 AI Training Evaluation

The RL model was evaluated using several training metrics, including:

Eval Return — measures the total reward obtained by the agent during evaluation.

Policy Entropy — indicates the degree of randomness/exploration in the policy.

Actor Loss — measures the optimization behavior of the policy network.

Critic Loss — measures the performance of the value estimation network.

KL Divergence — helps monitor the change between successive policies.

The training curves indicate that the agent gradually converges toward a more stable policy.

After training, the agent learned behaviors such as:

Approaching targets proactively.

Maintaining an appropriate distance from opponents.

Recognizing when an opponent is within attack range.

Choosing when to attack.

Moving in different directions according to the current combat situation.

<img width="2850" height="1200" alt="image" src="https://github.com/user-attachments/assets/8724c6cb-19f3-4bbe-bf69-4da07993dd96" />
Training Curves

🛠️ Technologies & Platforms
Game Development

Unity

C#

2D Isometric Game Architecture

Unity Animator / Blend Trees

Artificial Intelligence

Reinforcement Learning

PPO — Proximal Policy Optimization

Actor-Critic

GAE — Generalized Advantage Estimation

Behavior Tree

Finite State Machine (FSM)

Rule-Based AI

A* Pathfinding

Machine Learning & Deployment

Python

Neural Network training

ONNX

Offline model training

Runtime model inference inside Unity

Target Platforms

🖥️ PC

📱 Android

📁 Project Structure

A simplified project structure is shown below:

AdaptiveWar/
├── Assets/
│   ├── Scenes/
│   │   └── MainMenu
│   ├── Resources/
│   │   └── *.onnx
│   ├── Scripts/
│   ├── Prefabs/
│   ├── Animations/
│   └── ...
│
├── Packages/
├── ProjectSettings/
└── README.md


The trained AI model is included in the Unity project as an .onnx file under the Resources directory.

🚀 Installation & Setup
Prerequisites

Before running the project, make sure you have:

Unity Editor compatible with the project version.

Required Unity packages.

A PC capable of running the Unity Editor.

Installation

Clone this repository:

git clone <repository-url>


Open the project using Unity Hub or the Unity Editor.

Make sure the required packages are installed, including:

A* Pathfinding Project

TextMesh Pro

Other packages specified by the project

Navigate to:

Assets/Scenes/


Open the MainMenu scene.

Press Play in the Unity Editor.

Start the game and experience the adaptive NPC AI.

Note: The trained AI model has already been integrated into the project as an .onnx file in the Resources folder, so no additional model training is required to run the game.

🎯 Project Objectives

The main objectives of Adaptive War are:

Explore the application of Reinforcement Learning in game development.

Implement PPO-based adaptive NPC behavior.

Combine Machine Learning with traditional game AI techniques.

Develop an AI architecture suitable for real-time gameplay.

Evaluate whether an RL agent can learn meaningful combat behaviors.

Deploy a trained AI model directly inside a Unity game.

🎓 Academic Context

The project focuses on the intersection of:

Game Development × Artificial Intelligence × Reinforcement Learning

It demonstrates how a trained RL model can be integrated into a conventional game AI architecture to create NPCs capable of making adaptive decisions while maintaining predictable and controllable game logic.

📸 Screenshots

<img width="3840" height="2160" alt="image" src="https://github.com/user-attachments/assets/cd591d72-ca75-4809-861d-3c272fd8b984" />

<img width="722" height="473" alt="image" src="https://github.com/user-attachments/assets/a3aaa768-8f81-4974-9249-1c2705bff286" />

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/0ed317e0-47f0-4085-9f65-a8822fbee3b6" />

⭐ If you find this project interesting, feel free to explore the implementation and the Reinforcement Learning approach used to build the adaptive NPC system.
