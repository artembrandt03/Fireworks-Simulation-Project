# Fireworks Simulator - Artem Brandt

A C# MonoGame project that simulates launching and exploding fireworks.  
The system models physical particle motion (gravity, velocity, lifespan) and uses a layered architecture with reusable libraries for shapes, rendering, and physics logic.

---

## Project Overview

This solution consists of several interdependent projects:

| Project | Description |
|----------|--------------|
| **ShapeLibrary** | Provides geometric primitives (`Vector`, `Circle`, etc.) and reusable math utilities. |
| **Fireworks** | Core logic for individual fireworks, particles, explosion patterns, and environment management. |
| **FireworksSimulator** | The MonoGame-based executable that renders the simulation and handles user input. |
| **FireworksTests** | MSTest project that validates logic in the Fireworks and ShapeLibrary components. |
| **ShapesTests** | MSTest project that validates logic for Shapes. |

---

## Features

- **Launch Simulation** – Fireworks launch upward with randomized velocity and explode after a variable lifespan.  
- **Explosion Patterns** – Each firework spawns multiple colored particles moving in randomized circular trajectories.  
- **Gravity Physics** – Gravity continuously affects particle velocity to create a realistic falling effect.  
- **Fade Effect** – A translucent black overlay gradually fades the previous frame for smooth motion trails.  
- **Environment Management** – Automatically removes expired fireworks and prevents memory overload beyond 50 fireworks.  
- **Factories** – Uses factory methods (`FireworkFactory`, `ParticleFactory`, `ExplosionPatternFactory`).

---

## Controls

| Key | Action |
|-----|--------|
| **Spacebar** | Launch a new firework with a random color and pattern |
| **Escape** | Exit the simulation |