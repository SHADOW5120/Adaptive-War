# ⚔️ Adaptive War

> **2D Isometric Strategy Game with Reinforcement Learning**

Adaptive War is a 2D isometric strategy game developed with Unity, featuring an adaptive NPC combat system powered by **Reinforcement Learning (RL)**.

Instead of relying entirely on predefined behaviors, NPCs use a trained **Proximal Policy Optimization (PPO)** model to make tactical decisions based on the current game state.

The project combines Reinforcement Learning with traditional Game AI techniques — **Behavior Trees**, **Finite State Machines**, **Rule-Based Systems**, and **A\* Pathfinding** — in a layered architecture that keeps AI adaptive while maintaining predictable and controllable gameplay.

---

![Main Menu](https://github.com/user-attachments/assets/b3389f25-8f09-4b78-9b49-8b065a453a89)

---

## 📖 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [AI Architecture](#-ai-architecture)
- [NPC Decision-Making Pipeline](#-npc-decision-making-pipeline)
- [Reinforcement Learning](#-reinforcement-learning)
- [Training & Evaluation](#-ai-training--evaluation)
- [Technologies](#️-technologies)
- [Project Structure](#-project-structure)
- [Installation & Setup](#-installation--setup)
- [Project Objectives](#-project-objectives)
- [Screenshots](#-screenshots)

---

## 📖 Overview

The main idea behind Adaptive War is to integrate a trained Machine Learning model into a conventional game AI architecture.

The RL model does **not** control the NPC directly. Instead, it acts as a **tactical decision-making component** inside the existing AI system, providing a balance between:

| Aspect | Technology |
|---|---|
| 🧠 Adaptive decision-making | Reinforcement Learning (PPO) |
| 🌳 High-level strategic control | Behavior Tree |
| 🔄 Deterministic action execution | FSM + Game Logic |
| 🗺️ Navigation | A\* Pathfinding |
| ⚡ Real-time inference | ONNX inside Unity |

---

## ✨ Key Features

### 🧠 Adaptive NPC AI

- Integrates a trained neural network directly into Unity via **ONNX**
- Uses **PPO** to learn combat decision-making from scratch
- NPCs react to the environment and combat state dynamically
- RL works **alongside** deterministic game logic — not replacing it

### 🏗️ Layered AI Architecture

| Layer | Technologies | Responsibility |
|---|---|---|
| **Macro Layer** | Behavior Tree, Rule-Based System | High-level strategy and game flow |
| **Micro Layer** | FSM, PPO Neural Network | Tactical decisions for individual units |
| **Support Layer** | A\* Pathfinding | Navigation and obstacle avoidance |

### 🎮 2D Isometric Gameplay

- 2D isometric environment and characters
- 8-directional movement
- Unity Animator & Blend Trees
- Navigable environments with obstacles
- Strategic combat and unit deployment

### 📱 Cross-Platform Design

Designed for deployment on:
- 🖥️ PC
- 📱 Android

---

## 🧠 AI Architecture

![Layered AI Architecture](https://github.com/user-attachments/assets/7a52438c-ea26-44ee-aefb-8111353099e1)

### Macro Layer

Responsible for **high-level NPC behavior and strategic control**.

- Determines overall NPC behavior and decision flow
- Reacts to game-state changes
- Enforces rule-based constraints and behavior transitions

### Micro Layer

Handles **tactical decisions** made by individual units.

- The **PPO neural network** selects the tactical action
- The **FSM** executes the selected action through existing game logic
- This separation lets the neural network influence behavior without full control

### Support Layer

Handles **movement and navigation**.

- A\* Pathfinding calculates paths through the isometric environment
- The RL model focuses on *what* to do; pathfinding handles *how* to get there

---

## 🔄 NPC Decision-Making Pipeline

```
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
```

### 1. Environment Observation

The AI extracts features describing the current combat state:

- NPC health & Opponent health
- X-axis / Y-axis distance
- Total distance to opponent
- Attack-range status
- Other relevant game-state information

### 2. Neural Network Inference

The extracted features are passed into the trained **Actor-Critic** network, which selects one of **10 possible actions**:

| # | Action |
|---|---|
| 1 | Attack |
| 2 | Idle |
| 3 | Move Up |
| 4 | Move Down |
| 5 | Move Left |
| 6 | Move Right |
| 7 | Move Up-Left |
| 8 | Move Up-Right |
| 9 | Move Down-Left |
| 10 | Move Down-Right |

### 3. Action Execution

The selected action is passed back into the AI architecture. The **Behavior Tree** and **FSM** coordinate execution while underlying game systems handle movement, animation, and combat.

![Behavior Tree](https://github.com/user-attachments/assets/774d2cef-b1d0-4f87-81f6-2a1722594588)

---

## 🤖 Reinforcement Learning

### PPO — Proximal Policy Optimization

The agent is trained using **PPO** with an **Actor-Critic** architecture:

**Actor** — Learns the policy; determines which action to take given the current state.

**Critic** — Estimates state value; helps evaluate how effective the Actor's decisions are.

The agent repeatedly interacts with the environment, receives rewards, and updates its policy to improve future decisions.

### Generalized Advantage Estimation (GAE)

Training uses **GAE** to estimate action advantages, providing more stable advantage estimates and a smoother learning process during PPO optimization.

### Training Pipeline

```
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
```

---

## 📊 AI Training & Evaluation

To see more details about trainning, go to this detached project [https://github.com/SHADOW5120/Apdative-War-PPO-Model](https://github.com/SHADOW5120/Apdative-War-PPO-Model.git)

| Metric | Description |
|---|---|
| **Eval Return** | Total reward obtained during evaluation |
| **Policy Entropy** | Measures randomness / exploration of the policy |
| **Actor Loss** | Optimization behavior of the policy network |
| **Critic Loss** | Performance of value estimation |
| **KL Divergence** | Monitors changes between successive policies |

![Training Curves](https://github.com/user-attachments/assets/8724c6cb-19f3-4bbe-bf69-4da07993dd96)

### 🎯 Learned Combat Behaviors

After training, the agent learned several meaningful behaviors:

- ✅ Approaching targets proactively
- ✅ Maintaining appropriate combat distance
- ✅ Recognizing when an opponent enters attack range
- ✅ Choosing when to attack vs. reposition
- ✅ Moving in different directions based on combat state
- ✅ Adapting actions to environment changes

---

## 🛠️ Technologies

### 🎮 Game Development

- **Unity** / **C#**
- 2D Isometric Game Architecture
- Unity Animator & Blend Trees

### 🤖 Artificial Intelligence

- Reinforcement Learning — **PPO** (Proximal Policy Optimization)
- Actor-Critic Architecture
- **GAE** — Generalized Advantage Estimation
- Behavior Tree
- Finite State Machine (FSM)
- Rule-Based AI
- A\* Pathfinding

### 🧠 Machine Learning & Deployment

- **Python** — Neural network training
- **ONNX** — Model export and runtime inference in Unity
- Offline training + real-time inference pipeline

---

## 📁 Project Structure

```
AdaptiveWar/
├── Assets/
│   ├── Scenes/
│   │   └── MainMenu
│   ├── Resources/
│   │   └── *.onnx          ← Trained AI model
│   ├── Scripts/
│   ├── Prefabs/
│   ├── Animations/
│   └── ...
│
├── Packages/
├── ProjectSettings/
└── README.md
```

> The trained AI model is included as an `.onnx` file under `Assets/Resources/`.
> **No additional training is required to run the game.**

---

## 🚀 Installation & Setup

### Prerequisites

- Unity Editor (compatible with project version)
- Required Unity packages
- A PC capable of running the Unity Editor

### Steps

**1. Clone the repository**

```bash
git clone <repository-url>
```

**2. Open the project**

Open the project using **Unity Hub** or the **Unity Editor**.

**3. Install required packages**

Ensure the following packages are installed:

- A\* Pathfinding Project
- TextMesh Pro
- Other packages listed in the project manifest

**4. Open the Main Menu scene**

```
Assets/Scenes/MainMenu
```

**5. Press Play**

The trained PPO model is already integrated as an `.onnx` file — no additional training needed.

---

## 🎯 Project Objectives

- Explore the application of Reinforcement Learning in game development
- Implement PPO-based adaptive NPC behavior
- Combine Machine Learning with traditional Game AI techniques
- Develop an AI architecture suitable for real-time gameplay
- Evaluate whether an RL agent can learn meaningful combat behaviors
- Deploy a trained RL model directly inside a Unity game
- Investigate the advantages of combining deterministic AI with learned behavior

---

## 📸 Screenshots

![Gameplay](https://github.com/user-attachments/assets/cd591d72-ca75-4809-861d-3c272fd8b984)

![AI Combat](https://github.com/user-attachments/assets/a3aaa768-8f81-4974-9249-1c2705bff286)

![Game Environment](https://github.com/user-attachments/assets/0ed317e0-47f0-4085-9f65-a8822fbee3b6)

---

## 🎓 Academic Context

Adaptive War explores the intersection of:

**Game Development × Artificial Intelligence × Reinforcement Learning**

The project demonstrates how a trained RL model can be integrated into a conventional game AI architecture — not replacing traditional AI systems, but serving as a **tactical decision-making component** while deterministic systems remain responsible for high-level control, action execution, navigation, and gameplay rules.

This approach combines the **adaptability of Machine Learning** with the **predictability and controllability** required in real-time games.

---

## ⭐ Conclusion

Adaptive War demonstrates a practical approach to integrating Reinforcement Learning into a real-time game. By combining:

```
Behavior Tree + FSM + Rule-Based AI + A* Pathfinding + PPO
```

the project creates a layered AI architecture where each technique handles the type of problem it solves best. The result is an NPC system capable of learning adaptive combat behavior while remaining fully compatible with the deterministic systems required by a real-time strategy game.

---

> ⭐ If you find this project interesting, feel free to explore the implementation and the Reinforcement Learning approach used to build the adaptive NPC system.
