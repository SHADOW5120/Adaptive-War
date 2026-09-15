⚔️ Adaptive War
2D Isometric Strategy Game with Reinforcement Learning

Adaptive War is a 2D isometric strategy game developed with Unity, featuring an adaptive NPC combat system powered by Reinforcement Learning (RL).

Instead of relying entirely on predefined behaviors, NPCs use a trained Proximal Policy Optimization (PPO) model to make tactical decisions based on the current game state.

The project combines Reinforcement Learning with traditional Game AI techniques such as Behavior Trees, Finite State Machines, Rule-Based Systems, and A* Pathfinding. This layered architecture allows the AI to remain adaptive while maintaining predictable and controllable gameplay.

📖 Overview

The main idea behind Adaptive War is to integrate a trained Machine Learning model into a conventional game AI architecture.

The RL model does not control the entire NPC directly. Instead, it acts as a tactical decision-making component inside the existing AI system.

This approach provides a balance between:

🧠 Adaptive decision-making through Reinforcement Learning

🌳 High-level control through Behavior Trees

🔄 Deterministic action execution through FSM and game logic

🗺️ Navigation through A* Pathfinding

⚡ Real-time inference using an ONNX model inside Unity

Main Menu
<img width="1920" height="1080" alt="Main Menu" src="https://github.com/user-attachments/assets/b3389f25-8f09-4b78-9b49-8b065a453a89" />
✨ Key Features
🧠 Adaptive NPC AI

Integrates a trained neural network directly into Unity using ONNX.

Uses PPO to learn combat decision-making.

NPCs make decisions based on the current environment and combat state.

The trained model is used for offline training and runtime inference.

Reinforcement Learning works alongside deterministic game logic rather than replacing it.

🏗️ Layered AI Architecture

Adaptive War uses three main AI layers:

Layer	Technologies	Responsibility
Macro Layer	Behavior Tree, Rule-Based System	High-level strategy and game flow
Micro Layer	FSM, PPO Neural Network	Tactical decisions for individual units
Support Layer	A* Pathfinding	Navigation and obstacle avoidance

This architecture separates strategic control, tactical decision-making, and navigation, making the AI easier to control and maintain.

🎮 2D Isometric Gameplay

2D isometric environment and characters

8-directional movement

Unity Animator and Blend Trees

Navigable environments with obstacles

A* pathfinding

Strategic combat and unit deployment

Adaptive NPC combat behavior

📱 Cross-Platform Design

The project is designed with multiple deployment targets in mind:

🖥️ PC

📱 Android

🧠 AI Architecture

The core of Adaptive War is a Layered AI Architecture that combines different AI techniques according to their strengths.

<img width="1024" height="559" alt="Layered AI Architecture" src="https://github.com/user-attachments/assets/7a52438c-ea26-44ee-aefb-8111353099e1" />
Macro Layer

The Macro Layer is responsible for high-level NPC behavior and strategic control.

Technologies

Behavior Tree

Rule-Based System

Responsibilities

The Macro Layer determines:

Overall NPC behavior

High-level decision flow

Game-state reactions

Behavior transitions

Rule-based constraints

This layer ensures that AI behavior remains consistent with the rules and objectives of the game.

Micro Layer

The Micro Layer handles tactical decisions made by individual units.

Technologies

Finite State Machine (FSM)

PPO Neural Network

The PPO model determines the tactical action that should be taken, while the FSM and existing game logic are responsible for executing that action.

This separation allows the neural network to influence NPC behavior without directly controlling every aspect of the game.

Support Layer

The Support Layer handles movement and navigation.

Technology

A* Pathfinding

The A* algorithm calculates navigation paths through the isometric environment while avoiding obstacles.

This allows the RL model to focus on what the unit should do, while the pathfinding system handles how the unit gets there.

🔄 NPC Decision-Making Pipeline

During gameplay, an NPC continuously observes the environment and uses the current state to select an appropriate action.

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

Example observations include:

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

Action
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

The model therefore learns to select actions according to the current combat situation rather than following a completely fixed sequence of behaviors.

3. Action Execution

The selected action is passed back into the existing AI architecture.

The Behavior Tree and FSM coordinate the execution of the decision, while the underlying game systems handle movement, animation, combat, and other gameplay logic.

This design keeps the RL model focused on decision-making while preserving deterministic control over game execution.

Behavior Tree
<img width="4008" height="3348" alt="Behavior Tree" src="https://github.com/user-attachments/assets/774d2cef-b1d0-4f87-81f6-2a1722594588" />
🤖 Reinforcement Learning
PPO — Proximal Policy Optimization

The Adaptive War agent is trained using Proximal Policy Optimization (PPO), a policy-gradient Reinforcement Learning algorithm.

The model uses an Actor-Critic architecture consisting of two main components:

Actor

The Actor learns the policy and determines which action should be selected given the current state.

Critic

The Critic estimates the value of the current state and helps evaluate how effective the Actor's decisions are.

During training, the agent repeatedly interacts with the environment, receives rewards, and updates its policy to improve future decisions.

Generalized Advantage Estimation

The training process uses Generalized Advantage Estimation (GAE) to estimate the advantage of actions.

GAE helps provide more stable advantage estimates during PPO training and improves the learning process.

Training Pipeline
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
Environment Reward
       │
       ▼
GAE / PPO Update
       │
       └──────────────► Repeat

📊 AI Training & Evaluation

The RL model was evaluated using several training metrics:

Metric	Description
Eval Return	Total reward obtained by the agent during evaluation
Policy Entropy	Measures the randomness/exploration of the policy
Actor Loss	Indicates optimization behavior of the policy network
Critic Loss	Measures the performance of value estimation
KL Divergence	Monitors changes between successive policies

The training curves show the agent gradually moving toward a more stable policy.

Training Curves
<img width="2850" height="1200" alt="Training Curves" src="https://github.com/user-attachments/assets/8724c6cb-19f3-4bbe-bf69-4da07993dd96" />
🎯 Learned Combat Behaviors

After training, the agent learned several meaningful combat behaviors, including:

Approaching targets proactively

Maintaining an appropriate distance from opponents

Recognizing when an opponent enters attack range

Choosing when to attack

Moving in different directions according to the combat situation

Adapting actions based on changes in the environment

These behaviors demonstrate how Reinforcement Learning can be used as a tactical decision-making component inside a larger game AI system.

🛠️ Technologies
🎮 Game Development

Unity

C#

2D Isometric Game Architecture

Unity Animator

Unity Blend Trees

🤖 Artificial Intelligence

Reinforcement Learning

PPO — Proximal Policy Optimization

Actor-Critic

GAE — Generalized Advantage Estimation

Behavior Tree

Finite State Machine (FSM)

Rule-Based AI

A* Pathfinding

🧠 Machine Learning & Deployment

Python

Neural Network Training

ONNX

Offline Model Training

Runtime Neural Network Inference in Unity

🎯 Target Platforms

🖥️ PC

📱 Android

📁 Project Structure

A simplified project structure:

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


The trained AI model is included in the Unity project as an .onnx file under:

Assets/Resources/


No additional model training is required to run the game.

🚀 Installation & Setup
Prerequisites

Before running the project, make sure you have:

Unity Editor compatible with the project version

Required Unity packages

A PC capable of running the Unity Editor

Installation
1. Clone the repository
git clone <repository-url>

2. Open the project

Open the project using Unity Hub or the Unity Editor.

3. Install required packages

Make sure the required packages are installed, including:

A* Pathfinding Project

TextMesh Pro

Other packages specified by the project

4. Open the Main Menu scene

Navigate to:

Assets/Scenes/


Open the MainMenu scene.

5. Run the game

Press Play in the Unity Editor and start the game.

The trained PPO model is already integrated into the project as an .onnx file, so no additional training is required for gameplay.

🎯 Project Objectives

The main objectives of Adaptive War are:

Explore the application of Reinforcement Learning in game development.

Implement PPO-based adaptive NPC behavior.

Combine Machine Learning with traditional Game AI techniques.

Develop an AI architecture suitable for real-time gameplay.

Evaluate whether an RL agent can learn meaningful combat behaviors.

Deploy a trained RL model directly inside a Unity game.

Investigate the advantages of combining deterministic AI with learned behavior.

🎓 Academic Context

Adaptive War explores the intersection of:

Game Development × Artificial Intelligence × Reinforcement Learning

The project demonstrates how a trained Reinforcement Learning model can be integrated into a conventional game AI architecture.

Rather than replacing traditional AI systems entirely, the RL model is used as a tactical decision-making component, while deterministic systems remain responsible for high-level control, action execution, navigation, and gameplay rules.

This approach aims to combine the adaptability of Machine Learning with the predictability and controllability required in real-time games.

📸 Screenshots

<img width="3840" height="2160" alt="Gameplay" src="https://github.com/user-attachments/assets/cd591d72-ca75-4809-861d-3c272fd8b984" />

<img width="722" height="473" alt="AI Combat" src="https://github.com/user-attachments/assets/a3aaa768-8f81-4974-9249-1c2705bff286" />

<img width="1920" height="1080" alt="Game Environment" src="https://github.com/user-attachments/assets/0ed317e0-47f0-4085-9f65-a8822fbee3b6" />
⭐ Conclusion

Adaptive War demonstrates a practical approach to integrating Reinforcement Learning into a real-time game.

By combining:

Behavior Tree + FSM + Rule-Based AI + A Pathfinding + PPO*

the project creates a layered AI architecture where each technique is responsible for the type of problem it handles best.

The result is an NPC system capable of learning adaptive combat behavior while remaining compatible with the deterministic systems required by a real-time strategy game.

⭐ If you find this project interesting, feel free to explore the implementation and the Reinforcement Learning approach used to build the adaptive NPC system.
