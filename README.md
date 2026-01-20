# 🎆 Fireworks Simulator (C# / MonoGame)

**Developed by Artem Brandt**

A feature-rich **C# MonoGame fireworks simulation** that models realistic firework launches, explosion patterns, particle physics, sound effects, and interactive controls.  
This project emphasizes **object-oriented design**, **modular architecture**, **factory patterns**, **input handling**, and **performance benchmarking**.

---

## Overview

Fireworks Simulator is a real-time graphical simulation built with **MonoGame**.  
Users can launch fireworks manually, trigger predefined explosion patterns, queue launches with the mouse, toggle sound and logging, and run a fully scripted fireworks show.

The project is structured as a **multi-project C# solution**, separating rendering, physics logic, shapes, simulation, and testing into reusable libraries.

---

## Key Features

### Firework Simulation
- Realistic firework launch trajectories with gravity
- Particle-based explosions with lifespans and cleanup
- Multiple explosion patterns:
  - Default random
  - Circle
  - Rectangle
  - Star

### Interaction & Controls
- Keyboard-triggered fireworks and patterns
- Mouse-based firework queuing and launching
- Toggleable sound effects
- Toggleable key-press logging
- On-screen UI overlay with live controls & status
- Scripted multi-wave fireworks display mode

### Audio
- Looping launcher sound while firework ascends
- Explosion sound synced to particle activity
- Per-firework sound instance management
- Global sound enable/disable

### Rendering & Visuals
- Virtual resolution scaling (800×600)
- Smooth motion trails using fade overlay
- Shape-based rendering (no sprites for particles)
- Dynamic UI text rendering

### Performance & Reliability
- Automatic cleanup of expired fireworks and particles
- Firework count limiting to prevent overload
- Dedicated benchmarking project for performance analysis

---

## Architecture & Project Structure

High-level solution layout:

```
FireworksSolution/
├── DrawingLibrary/        # Rendering, input abstraction, screen scaling
├── ShapeLibrary/          # Geometry primitives (Circle, Rectangle, Vector, etc.)
├── Fireworks/             # Core simulation logic (fireworks, particles, patterns)
├── FireworksSimulator/    # MonoGame executable (main simulation)
├── FireworksSimulationBenchmarking/
│   └── Performance tests and results
├── FireworksTest/         # MSTest unit tests (fireworks logic)
├── ShapeLibraryTests/     # MSTest unit tests (geometry & math)
```

---

## Controls

### Keyboard
| Key | Action |
|---|---|
| **SPACE** | Launch random firework |
| **R** | Rectangle explosion |
| **C** | Circle explosion |
| **H** | Star explosion |
| **L** | Run scripted fireworks show |
| **S** | Toggle sound |
| **T** | Toggle key logging |
| **ESC** | Exit |

### Mouse
| Action | Behavior |
|---|---|
| **Right Click** | Queue firework launch position (up to 10) |
| **Left Click** | Launch queued fireworks or single firework |

---

## Testing

- **MSTest** unit tests for:
  - Fireworks logic
  - Explosion patterns
  - Particle behavior
  - Shape geometry & math
- Dedicated benchmarking project for performance evaluation

---

## Running the Project

### Requirements
- Visual Studio 2022+
- .NET SDK compatible with MonoGame
- MonoGame Framework (DesktopGL)

### Steps
1. Clone the repository
2. Open the solution in Visual Studio
3. Set **FireworksSimulator** as the startup project
4. Build & Run

---

## Media

---

## Academic Context

Developed as part of a **C# / Object-Oriented Programming** course project, focusing on:
- Clean architecture
- Real-time simulation
- Performance considerations
- Advanced input & rendering handling
