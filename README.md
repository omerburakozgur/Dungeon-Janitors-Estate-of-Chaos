<div align="center">

<h1>🏰 Dungeon Janitors: Estate of Chaos</h1>
<h3>Multiplayer Co-op Simulation Prototype</h3>

<p>
  <a href="https://omerburakozgur.itch.io/" target="_blank">
    <img src="https://img.shields.io/badge/Prototype_Build-Itch.io-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white" alt="Itch.io" />
  </a>
  <a href="#" target="_blank">
    <img src="https://img.shields.io/badge/Watch-Tech_Demo-FF0000?style=for-the-badge&logo=youtube&logoColor=white" alt="Youtube" />
  </a>
</p>

<img src="https://via.placeholder.com/800x400?text=Multiplayer+Sync+GIF+Placeholder" alt="Gameplay Demo" width="100%" />

<br>

| **Engine** | **Language** | **Networking** | **Architecture** |
| :---: | :---: | :---: | :---: |
| <img src="https://img.shields.io/badge/Unity_3D-000000?style=flat-square&logo=unity&logoColor=white" /> | <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white" /> | <img src="https://img.shields.io/badge/Photon_Fusion-005571?style=flat-square&logo=photon&logoColor=white" /> | <img src="https://img.shields.io/badge/Host--Authoritative-red?style=flat-square" /> |

</div>

---

## 📖 Project Overview

**Dungeon Janitors: Estate of Chaos** is a 2-4 player online co-op simulation game developed within the scope of the **"Game Architecture"** master's course.

The primary goal of this project was not just to make a game, but to build a robust **'Net-Ready' architecture** that handles latency, state synchronization, and physics interactions in a multiplayer environment.

---

## 🎮 Core Mechanics

* **Online Co-op:** Up to 4 players working together in real-time.
* **Physics-Based Interaction:** Players can pick up, throw, and manipulate objects which are synchronized across the network.
* **Cleaning Simulation:** Mechanics involving cleaning stains, repairing furniture, and organizing dungeon loot before the "Boss" returns.
* **Chaos Management:** Players must coordinate to solve timed challenges without colliding or blocking each other.

---

## ⚙️ Technical Architecture & Networking

This project serves as a technical showcase for **Multiplayer Game Engineering**.

### 🌐 Host-Authoritative Networking
Utilizing **Photon Fusion**, I implemented a **Host-Authoritative** topology.
* **State Authority:** The Host machine owns the world state (physics, object positions), preventing client-side cheating and desync.
* **Client Prediction:** Implemented input prediction so clients feel responsive (0 latency feel) even though the server confirms actions later.
* **Lag Compensation:** Networked hitboxes ensure that interactions remain fair even with variable ping.

### 🏗️ Design Patterns & Decoupling
To ensure the codebase remains scalable and testable:
* **Observer Pattern:** Used heavily for UI updates and Game State changes (e.g., updating score without checking variables every frame).
* **Scriptable Object Event Channels:** Decoupled systems by using SO-based events. The "Player Controller" doesn't know about the "Audio System"; it just raises an event.

### 📝 Agile Development & Documentation
Before writing code, I prioritized engineering discipline:
* **TDD (Technical Design Document):** Defined class diagrams, network topology, and data structures.
* **GDD (Game Design Document):** Outlined core loops and mechanics.
* **Sprint Planning:** Managed the development lifecycle with planned sprints and regular code reviews.

---

## 🎨 Asset Pipeline (Prototype Focus)

As this project focuses on **Architecture and Networking**, I utilized high-quality placeholder assets to accelerate development.

* **3D Assets:** Primarily used **Synty Studios - SIMPLE Dungeons** for characters and environment to maintain a cohesive low-poly aesthetic.
* **Prototyping:** The focus was on "Grey-boxing" the level design to test networking capabilities before polishing visuals.
* **Custom Integration:** While assets are premade, all **Interaction Logic**, **Animation State Machines**, and **Network Sync Components** were manually programmed and configured.

---

## 🚀 Future Roadmap

* [ ] Implementing "Dedicated Server" support.
* [ ] Adding voice chat integration.
* [ ] Polishing visual effects (VFX) for cleaning actions.

---

<div align="center">
  <a href="mailto:omerburakozgur1@gmail.com">
    <img src="https://img.shields.io/badge/Contact_Me-D14836?style=for-the-badge&logo=gmail&logoColor=white" alt="Email" />
  </a>
  <a href="https://www.linkedin.com/in/omerburakozgur/">
    <img src="https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white" alt="LinkedIn" />
  </a>
</div>
