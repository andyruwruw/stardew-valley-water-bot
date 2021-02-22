<p align="center">
  <img width="300px" src="./documentation/logo.svg" />
</p>

# WaterBot

**WaterBot** is a [Stardew Valley](https://www.stardewvalley.net/) mod that helps you water your crops while staying as vanilla as possible.

Upon watering a plant, the mod will control your player to water the rest of your plants, fetching water when required. Any buttons pressed while active will turn the bot off.

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

To begin the mod listens for whenever the player left clicks with a watering can on their farm. If the targeted block is Hoed Dirt, it begins the bot.

The bot first reads the farm map data, marking each tile as walkable, refillable, and in need of watering.

The tiles that need watering are then grouped based on adjacent tiles that also need watering.

The greedy approach to the travelling salesman problem is then applied to find a path through the groups.

Each group is watered using the boundary fill algorithm.

When the watering can is low, the bot will go to the nearest source of water to refill.
