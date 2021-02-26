using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WaterBot.Framework
{
    /// <summary>
    /// Defines the process of the Bot being active.
    /// </summary>
    class WaterBotControler
    {
        public bool active;

        public Map map;

        public int currentGroup;

        public List<Group> path;

        public int currentTile;

        public List<ActionableTile> order;

        private ActionableTile refillStation;

        public console console;

        public WaterBotControler()
        {
            this.active = false;
            this.map = new Map();
        }

        /// <summary>
        /// Starts the bot up.
        /// </summary>
        /// 
        /// <param name="console">Function for printing to debug console.</param>
        public void start(console console)
        {
            this.console = console;
            this.active = true;

            this.displayMessage("Time to start watering!", 2);

            // Load map data
            this.map.loadMap();

            if (!this.active) return;

            // Group waterable tiles
            this.map.findGroupings(this.console);

            if (!this.active) return;

            this.currentGroup = 0;
            this.currentTile = 0;

            this.path = this.map.findGroupPath(this.console);

            if (!this.active) return;

            this.order = this.map.findFillPath(this.path[this.currentGroup]);

            if (!this.active) return;

            Game1.player.controller = new PathFindController(Game1.player, Game1.getFarm(), this.order[this.currentTile].getStand(), 2, this.startWatering);
        }

        /// <summary>
        /// Begins the process of watering current actionable tile
        /// </summary>
        /// 
        /// <param name="c">Character object of player.</param>
        /// <param name="location">Location of character.</param>
        public void startWatering(Character c, GameLocation location)
        {
            Game1.player.controller = null;

            if (!this.active) return;

            if (Game1.player.Stamina <= 2f)
            {
                this.exhausted();
                return;
            }

            if (((WateringCan)Game1.player.CurrentTool).WaterLeft <= 0)
            {
                this.refillWater();

                return;
            }

            Point point = this.order[this.currentTile].Pop();

            if (point.X != -1)
            {
                this.water(point);

                Task.Delay(new TimeSpan(0, 0, 0, 0, 800)).ContinueWith(o => { this.startWatering(c, location); });
            } else
            {
                this.navigate();
            }
        }

        /// <summary>
        /// Waters a tile
        /// </summary>
        /// 
        /// <param name="tile">Tile to water.</param>
        public void water(Point tile)
        {
            if (Game1.player.getTileY() > tile.Y)
            {
                Game1.player.FacingDirection = 0;
            }
            else if (Game1.player.getTileY() < tile.Y)
            {
                Game1.player.FacingDirection = 2;
            } else if (Game1.player.getTileX() > tile.X)
            {
                Game1.player.FacingDirection = 3;
            } else if (Game1.player.getTileX() < tile.X)
            {
                Game1.player.FacingDirection = 1;
            } 

            if (Game1.player.isEmoteAnimating)
            {
                Game1.player.EndEmoteAnimation();
            }

            Game1.player.FarmerSprite.setOwner(Game1.player);
            Game1.player.CanMove = false;
            Game1.player.UsingTool = true;
            Game1.player.canReleaseTool = true;

            Game1.player.Halt();
            Game1.player.CurrentTool.Update(Game1.player.FacingDirection, 0, Game1.player);

            if ((int)Game1.player.CurrentTool.upgradeLevel <= 0)
            {
                Game1.player.stopJittering();
                Game1.player.canReleaseTool = false;

                int addedAnimationMultiplayer = ((!(Game1.player.Stamina <= 0f)) ? 1 : 2);
                if (Game1.isAnyGamePadButtonBeingPressed() || !Game1.player.IsLocalPlayer)
                {
                    Game1.player.lastClick = Game1.player.GetToolLocation();
                }

                if (((WateringCan)Game1.player.CurrentTool).WaterLeft > 0 && Game1.player.ShouldHandleAnimationSound())
                {
                    Game1.player.currentLocation.localSound("wateringCan");
                }

                Game1.player.lastClick = new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize);

                switch (Game1.player.FacingDirection)
                {
                    case 2:
                        ((FarmerSprite)Game1.player.Sprite).animateOnce(164, 125f * (float)addedAnimationMultiplayer, 3);
                        break;
                    case 1:
                        ((FarmerSprite)Game1.player.Sprite).animateOnce(172, 125f * (float)addedAnimationMultiplayer, 3);
                        break;
                    case 0:
                        ((FarmerSprite)Game1.player.Sprite).animateOnce(180, 125f * (float)addedAnimationMultiplayer, 3);
                        break;
                    case 3:
                        ((FarmerSprite)Game1.player.Sprite).animateOnce(188, 125f * (float)addedAnimationMultiplayer, 3);
                        break;
                }
            }
            else
            {
                Game1.player.jitterStrength = 0.25f;
                switch (Game1.player.FacingDirection)
                {
                    case 0:
                        Game1.player.FarmerSprite.setCurrentFrame(180);
                        Game1.player.CurrentTool.Update(0, 0, Game1.player);
                        break;
                    case 1:
                        Game1.player.FarmerSprite.setCurrentFrame(172);
                        Game1.player.CurrentTool.Update(1, 0, Game1.player);
                        break;
                    case 2:
                        Game1.player.FarmerSprite.setCurrentFrame(164);
                        Game1.player.CurrentTool.Update(2, 0, Game1.player);
                        break;
                    case 3:
                        Game1.player.FarmerSprite.setCurrentFrame(188);
                        Game1.player.CurrentTool.Update(3, 0, Game1.player);
                        break;
                }
            }

            this.map.map[tile.Y][tile.X].waterable = false;
        }

        /// <summary>
        /// Navigates to a point
        /// </summary>
        public void navigate()
        {
            if (!this.active) return;

            this.currentTile += 1;

            if (this.currentTile >= this.order.Count)
            {
                this.currentGroup = this.currentGroup += 1;
                this.currentTile = 0;

                if (this.currentGroup >= this.path.Count)
                {
                    this.end();
                    return;
                }

                this.order = this.map.findFillPath(this.path[this.currentGroup]);
            }

            Game1.player.controller = new PathFindController(Game1.player, Game1.getFarm(), this.order[this.currentTile].getStand(), 2, this.startWatering);
        }

        public void navigateNoUpdate()
        {
            Game1.player.controller = new PathFindController(Game1.player, Game1.getFarm(), this.order[this.currentTile].getStand(), 2, this.startWatering);
        }

        public void refillWater()
        {
            if (!this.active) return;

            Tile playerLocation = this.map.map[Game1.player.getTileY()][Game1.player.getTileX()];

            this.refillStation = this.map.getClosestRefill(playerLocation, this.console);

            if (!this.active) return;

            Game1.player.controller = new PathFindController(Game1.player, Game1.getFarm(), refillStation.getStand(), 2, this.startRefilling);
        }

        public void startRefilling(Character c, GameLocation location)
        {
            Game1.player.controller = null;

            if (!this.active) return;

            if (Game1.player.Stamina <= 2f)
            {
                this.exhausted();
                return;
            }

            Point point = this.refillStation.Pop();

            if (point.X != -1)
            {
                this.water(point);

                Task.Delay(new TimeSpan(0, 0, 0, 0, 800)).ContinueWith(o => { this.navigateNoUpdate(); });
            }
        }

        /// <summary>
        /// Cancels the bot's progress.
        /// </summary>
        public void stop()
        {
            this.active = false;
            Game1.player.controller = null;
        }

        /// <summary>
        /// Cancels the bot's progress.
        /// </summary>
        public void exhausted()
        {
            this.active = false;
            Game1.player.controller = null;
            this.displayMessage("Tired of watering", 3);
        }

        /// <summary>
        /// Cancels the bot's progress.
        /// </summary>
        public void end()
        {
            this.active = false;
            Game1.player.controller = null;
            this.displayMessage("Finished Watering", 1);
        }

        /// <summary>
        /// Displays banner message.
        /// </summary>
        /// 
        /// <param name="message">Banner text.</param>
        /// <param name="type">Banner type.</param>
        public void displayMessage(string message, int type)
        {
            Game1.addHUDMessage(new HUDMessage(message, type));
        }
    }
}
