<div align="center">

<h1>🔫 Project: Echoes</h1>
<h3>Linear Narrative-Driven FPS & Engineering Showcase</h3>

<div align="center"> 
<a href="https://omerburakozgur.itch.io/project-echoes" target="_blank"> <img src="https://img.shields.io/badge/Play_on-Itch.io-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white" alt="Play on Itch.io"> </a>
<a href="https://www.artstation.com/omerburakozgur"><img src="https://img.shields.io/badge/ArtStation-13AFF0?style=for-the-badge&logo=artstation&logoColor=white" alt="Assets"></a> 
</div>

<h3>🎥 Gameplay Showcase (Click to Watch)</h3> 
<a href="https://youtu.be/Cln-pehkH1k" target="_blank" title="Click to watch full gameplay video"> 
<img src="https://github.com/user-attachments/assets/8212fa8b-1802-4f8e-b0bc-d515c7d89f96" alt="Project Echoes Gameplay GIF" width="100%" style="border-radius: 8px; border: 2px solid #00C6FF;"> 
</a>
<br>
<i>(Click on the image to watch the full gameplay video)</i>
<br><br>

| **Engine** | **Language** | **Architecture** | **Input & Pipeline** |
| :---: | :---: | :---: | :---: |
| <img src="https://img.shields.io/badge/Unity_6.4_LTS-000000?style=flat-square&logo=unity&logoColor=white" /> | <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white" /> | <img src="https://img.shields.io/badge/Event--Driven_Arch-2B5797?style=flat-square" /> | <img src="https://img.shields.io/badge/New_Input_System-005571?style=flat-square" /> |

<br>
<table width="100%" cellpadding="10">
  <tr>
    <td width="50%" align="center">
      <img src="https://github.com/user-attachments/assets/ff4c70fa-e880-4df8-82f0-0c5ac9409320" width="100%" style="border-radius: 8px;">
      <br><b>Tactical Gunplay & Weapons</b>
    </td>
    <td width="50%" align="center">
      <img src="https://github.com/user-attachments/assets/d1e5be6e-4cfb-468f-bef7-5c1d6e546bcb" width="100%" style="border-radius: 8px;">
      <br><b>Various Enemy Archetypes</b>
    </td>
  </tr>
  <tr>
    <td width="50%" align="center">
      <img src="https://github.com/user-attachments/assets/74ba1978-63e3-45ab-832b-5220ebd53b03" width="100%" style="border-radius: 8px;">
      <br><b>Enemy Abilities & Pressure</b>
    </td>
    <td width="50%" align="center">
      <img src="https://github.com/user-attachments/assets/a9953759-9736-47fd-9c9d-f4fb8b9f9b25" width="100%" style="border-radius: 8px;">
      <br><b>Collectibles & Object Carry System</b>
    </td>
  </tr>
</table>

</div>

---

## 📖 Project Overview

**Project: Echoes** is a linear narrative-driven first-person shooter (FPS) set in the isolated SYNAPSE Research Facility, where experimental neuro-technologies designed to erase traumatic memories have critically malfunctioned. Suppressed human emotions have physically manifested into aggressive, deformed entities, causing complete containment failure.

Playing as an amnesiac scientist, the player must survive the chaotic facility while uncovering the truth behind the experiments. Heavily inspired by the **Half-Life** series, this project focuses on delivering highly responsive gunplay, smooth physics-based movement, and modular software architecture.

---

## ⚙️ Core Architecture & Engineering

The codebase is built upon strict software engineering principles to prevent tight coupling and ensure maximum performance during fast-paced combat loops.

### 🔗 1. Zero-Coupling Event System
To maintain clean architecture, systems (UI, Combat, Vitals, AI) never reference each other directly. 
* Integrated Unity's **New Input System** alongside highly decoupled **ScriptableObject Event Channels** (`VoidEventChannelSO`, `FloatEventChannelSO`, `StringEventChannelSO`).
* **Flow Example:** When a player picks up an object, an event broadcast updates the state logic without the movement or weapon scripts needing manual references.

### 🚀 2. Deterministic Object Pooling
Strictly optimized to ensure zero Garbage Collection (GC) allocation spikes during runtime execution.
* Utilizing Unity's native `UnityEngine.Pool` API, all bullet decals, impact VFX, blood splatters, and sound emitters are pre-warmed in memory at initialization. Working with runtime `Instantiate` or `Destroy` is completely restricted within combat controllers.

### 📊 3. Flyweight Pattern & Data-Driven Logic
Weapon mechanics and AI configuration parameters are centrally managed via **ScriptableObjects** to separate immutable data from mutable runtime controller states.
* Immutable base statistics (damage curves, capacities, sound profiles) are securely stored in `WeaponDataSO` and `AudioDataSO` files, allowing seamless iteration without code duplication.

---

## 🛠️ Gameplay Systems & Implementation

### 🔫 Advanced Gunplay & Regional Hitboxes
* **Raycast Mechanics:** Gunplay utilizes highly responsive hitscan calculations optimized with procedural recoil vectors driven by customized `AnimationCurves`. Features include incremental recoil distribution, multi-shot shotgun spreading, and dynamic Aim-Down-Sights (ADS) logic.
* **Precise Hitbox Framework:** Implemented individual skeletal collision boxes (Head vs. Torso colliders) mapped to parent controllers to deliver accurate regional damage scaling.

### 🧠 Vitals & Sanity (Akıl Sağlığı) Loop
* **HEV Suit Damage Mitigation:** Utilizing an `IDamageable` interface, incoming attacks are mitigated by an active Armor system (80% absorbed by Armor, 20% affecting direct Health).
* **Active Sanity Pressure Loop:** Implemented a continuous survival stress mechanism where players lose sanity over time. Reaching zero sanity induces continuous health drain alongside heartbeat-synced visual vignette effects, directly incentivizing an aggressive forward-push combat style to execute enemies and restore mental stability.

### 🧲 Physics-Driven Movement & Carry Mechanics
* **Dynamic Player Controller:** Engineered a pure physics-based controller handling inputs via `Rigidbody` velocity manipulation in `FixedUpdate`. Supports slope calculations, crouching, custom acceleration curves, and Source-engine style mechanics (b-hop & air control dynamics) capped safely to prevent infinite velocity exploits.
* **Environmental Carry:** Integrated raycast interaction validation allowing players to carry physics-enabled objects securely via joint/velocity tracking with automatic angle correction to prevent wall clipping.

---

## 🤖 Modular Enemy AI (State Machines)

Enemies utilize scalable **Finite State Machines (FSM)** structured with shared interfaces (`IState`) to handle complex behavioral nodes without standard switch-case structures. Detection loops apply an **Early Exit** principle (evaluating distance -> dot product FOV -> line-of-sight raycasts) to save frame processing resources.

### 🪖 AI Archetypes
* **WRATH (The Rusher):** Highly aggressive melee unit utilizing super-armor dashes to breach safe boundaries, forcing players into tactical backpedaling.
* **DEPRESSION (The Tank):** High-health juggernaut executing heavy area-of-effect (AOE) ground slams that trigger localized shockwaves and global camera shakes.
* **CERU Soldiers:** Intelligent ranged tactical units operating across varying operational ranges. They actively query level geometry for dynamic NavMesh cover nodes, reposition instantly upon line-of-sight exposure, and maintain optimal kiting distances.

---

<div align="center">
  <a href="mailto:omerburakozgur1@gmail.com">
    <img src="https://img.shields.io/badge/Contact_Me-D14836?style=for-the-badge&logo=gmail&logoColor=white" alt="Email" />
  </a>
  <a href="https://www.linkedin.com/in/omerburakozgur/">
    <img src="https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white" alt="LinkedIn" />
  </a>
</div>
