<div align="center">

<h1>🏰 Dungeon Janitors: Estate of Chaos</h1>
<h3>First-Person Co-op Simulation & Architecture Prototype</h3>

<p>
  <a href="#" target="_blank">
    <img src="https://img.shields.io/badge/Prototype_Build-Itch.io-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white" alt="Itch.io" />
  </a>
  <a href="https://docs.google.com/document/d/1wt112ZX0hZ7NIivY41Cdhh5RROT_jld6gSN8U51ylDo/edit?usp=sharing"><img src="https://img.shields.io/badge/GDD_Doc-2B5797?style=for-the-badge&logo=googledocs&logoColor=white" alt="GDD"></a>
  <a href="https://docs.google.com/document/d/1wqi1_IfI3CmmCAdL-SX_Pjgux8QhREAc288wea1te8Q/edit?usp=sharing"><img src="https://img.shields.io/badge/TDD_Doc-2B5797?style=for-the-badge&logo=googledocs&logoColor=white" alt="GDD"></a>
  <a href="#" target="_blank">
    <img src="https://img.shields.io/badge/Watch-Tech_Demo-FF0000?style=for-the-badge&logo=youtube&logoColor=white" alt="Youtube" />
  </a>
</p>



| **Engine** | **Language** | **Architecture** | **Networking** |
| :---: | :---: | :---: | :---: |
| <img src="https://img.shields.io/badge/Unity_2022+-000000?style=flat-square&logo=unity&logoColor=white" /> | <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white" /> | <img src="https://img.shields.io/badge/Host--Authoritative-red?style=flat-square" /> | <img src="https://img.shields.io/badge/Net--Ready_(Fusion)-005571?style=flat-square&logo=photon&logoColor=white" /> |

</div>

---

## 📖 Project Overview

**Dungeon Janitors: Estate of Chaos** is a first-person simulation game about cleaning up dangerous dungeons after "heroes" have raided them. It combines the methodical cleaning mechanics of *Viscera Cleanup Detail* with the layered exploration of *House Flipper*, all set within an active, hostile environment.

**Core Pillars:**
* **Layered Cleaning:** Trash Collection -> Physics Carry -> Deep Cleaning.
* **Active Danger:** The dungeon fights back with both Nuisance (Slimes) and Combat (Goblin) enemies.
* **Risk vs. Reward:** A run-based extraction loop where staying longer increases danger but yields more loot.

> **Technical Goal:** The primary objective of this project is to implement a **'Net-Ready' Host-Authoritative Architecture** that simulates multiplayer authority even in Single Player, ensuring a seamless migration to Photon Fusion.

---

## ⚙️ Technical Architecture (The Core)

This project strictly follows a **Host-Authoritative** model defined in the Technical Design Document (TDD).

### 🌐 1. Authority & Manager Singletons
Instead of decentralized logic, the game state is managed by central **Singleton Managers** that act as the "Server/Host" authority.
* **Rule:** Client scripts (e.g., `PlayerInteraction`) never modify the game state directly. They send **Requests**.
* **Flow:** `Input` -> `RequestClean()` -> `CleaningManager (Authority Validation)` -> `Execution` -> `UpdateVisuals`.

### 🔗 2. Event-Driven Decoupling
To prevent spaghetti code, **ScriptableObject Event Channels** are used to decouple logic from "Listeners" like UI and Audio.
* **Implementation:** When `TrashManager` collects an item, it raises a `VoidEventChannelSO`. The UI listens to this event to update the counter, without `TrashManager` knowing about the UI exists.

### 📡 3. Photon Fusion Readiness
The codebase is designed with specific migration steps for Multiplayer:
* **Networked Objects:** Players, Heavy Items, and Enemies are designed to become `NetworkObject`s.
* **Optimized Sync:** High-frequency objects like "Dirt Decals" are **NOT** networked objects. Instead, their state is managed via a networked `Dictionary<int, float>` in the `CleaningManager`, drastically reducing network traffic.

---

## 🛠️ Key Systems & Implementation

### 🧹 Layer 1 & 2: Trash & Physics
* **Inventory Management:** The `TrashManager` validates collection requests against capacity limits defined in `PlayerTrashController`.
* **Physics Interactions:** Heavy objects use rigidbodies. In the MP design, ownership of the Rigidbody is transferred from Host to Client upon interaction request to allow smooth carrying and rotation.

### 🧼 Layer 3: Deep Cleaning System
* **Optimized Visuals:** The `CleaningManager` holds the authoritative dirtiness value (0.0 to 1.0).
* **Logic:** When a player cleans, the Host validates the tool (e.g., Mop) and solution amount. If valid, it updates the data and commands the visual decal to update its alpha channel locally.

### ⚔️ Combat & AI (State Machines)
* **AI Architecture:** `EnemyAIManager` handles the FSM (Finite State Machine) for all active enemies in a central update loop (`TickAI`), simulating server-side AI processing.
* **Combat Logic:** Hit detection is performed via server-side (Host) **BoxCast** validation to prevent client-side cheating.

---

## 🔁 Gameplay Loop (Run-Based)

1.  **Preparation (Hub):** Purchase upgrades (e.g., faster Mop) and select a contract.
2.  **Deploy:** Enter the active dungeon.
3.  **The Job (Core Loop):**
    * **Collect:** Pick up loose trash.
    * **Carry:** Move heavy obstacles to reveal hidden dirt.
    * **Clean:** Scrub blood and slime using specific tools.
    * **Defend:** Fight off Goblins or push back Slimes.
4.  **Extract:** Decide when to leave based on the rising **Danger Meter**.

---

## 📂 Project Structure (Data-Driven)

The game relies heavily on **ScriptableObjects** for configuration, making it easy for designers to tweak values without touching code.
* `ToolData.cs`: Defines tool stats (clean speed, damage).
* `LootTable.cs`: Controls drop rates for enemies.
* `EnemyStats.cs`: Defines AI behavior parameters.

---

<div align="center">
  <a href="mailto:omerburakozgur1@gmail.com">
    <img src="https://img.shields.io/badge/Contact_Me-D14836?style=for-the-badge&logo=gmail&logoColor=white" alt="Email" />
  </a>
  <a href="https://www.linkedin.com/in/omerburakozgur/">
    <img src="https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white" alt="LinkedIn" />
  </a>
</div>
