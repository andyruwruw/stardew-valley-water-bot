<p align="center">
  <img width="300px" src="./documentation/logo.svg" />
</p>

# WaterBot

**WaterBot** is a [Stardew Valley](https://www.stardewvalley.net/) mod that helps you water your crops while staying as vanilla as possible.

When you `right-click` a plant with your `Watering Can`, the bot will take control of your character, **watering all your crops** and **refilling the water** can at the nearest water source whenever necessary.

Left-click at any point to stop the bot. The bot will automatically stop if you have low stamina.

# Contents

- [Install]()
- [Configure]()
- [Compatibility]()
- [Implementation]()

# Install

1. Install the latest version of [SMAPI](https://smapi.io/).
2. Download this mod and unzip the contents.
3. Place the mod in your Mods folder.
4. Run the game using SMAPI.

# Configure

# Compatibility

# Implementation

To begin the mod listens for whenever the player `right-clicks`.

If the target block is **Hoed Dirt** with a **crop** and the player is holding their **Watering Can**, the bot is started.

### Loading the Players Farm

The bot first reads the farm map data, marking each tile as `walkable`, `refillable`, and in `need of watering`.

The tiles that need watering are then grouped based on adjacent tiles that also need watering.

The greedy approach to the travelling salesman problem is then applied to find a path through the groups.

Each group is watered using the boundary fill algorithm.

When the watering can is low, the bot will go to the nearest source of water to refill.
