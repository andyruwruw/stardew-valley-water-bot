using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Pathfinding;

namespace WaterBot.Framework
{
    /// <summary>
    /// Defines the process of the Bot being active.
    /// </summary>
    class WaterBotController
    {
        private readonly IModHelper helper;
        public bool active;
        public Map map;
        public int currentGroup;
        public List<Group> path;
        public int currentTile;
        public List<ActionableTile> order;
        private ActionableTile? refillStation;
        public console? console;

        public WaterBotController(IModHelper helper)
        {
            this.helper = helper;
            this.active = false;
            this.map = new Map();
            this.path = new List<Group>();
            this.order = new List<ActionableTile>();
        }

        /// <summary>
        /// Starts the bot up.
        /// </summary>
        ///
        /// <param name="console">Function for printing to debug console.</param>
        public void start(console console, bool showMessage = true)
        {
            this.console = console;
            this.active = true;

            // Load map data
            this.map.loadMap();

            if (!this.active) return;

            // Group waterable tiles
            List<Group> groupings;
            if (WaterBot.config.UseSmallGrouping)
            {
                groupings = this.map.getMinimalCoverGroups();
            }
            else
            {
                groupings = this.map.findGroupings(this.console);
            }

            if (!this.active) return;

            this.currentGroup = 0;
            this.currentTile = 0;

            this.path = this.map.findGroupPath(this.console, groupings);

            if (path.Count == 0)
            {
                this.active = false;
                return;
            }

            if (showMessage)
            {
                this.displayMessage(this.helper.Translation.Get("process.start"), 2);
            }

            if (!this.active) return;

            this.order = this.map.findFillPath(this.path[this.currentGroup], this.console);

            if (!this.active) return;

            WateringCan? wateringCan = this.getCurrentWateringCanOrStop();
            if (wateringCan == null)
            {
                return;
            }

            if (wateringCan.WaterLeft <= 0)
            {
                this.refillWater();
                return;
            }

            Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, this.order[this.currentTile].getStand(), 2, this.startWatering);
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

            WateringCan? wateringCan = this.getCurrentWateringCanOrStop();
            if (wateringCan == null)
            {
                return;
            }

            if (wateringCan.WaterLeft <= 0)
            {
                this.refillWater();
                return;
            }

            Point point = this.order[this.currentTile].Pop();

            if (point.X != -1)
            {
                this.water(point);
                Task.Delay(800).ContinueWith(o => { this.startWatering(c, location); });
            }
            else
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
            WateringCan? wateringCan = this.getCurrentWateringCanOrStop();
            if (wateringCan == null)
            {
                return;
            }

            if (Game1.player.TilePoint.Y > tile.Y)
            {
                Game1.player.FacingDirection = 0;
            }
            else if (Game1.player.TilePoint.Y < tile.Y)
            {
                Game1.player.FacingDirection = 2;
            }
            else if (Game1.player.TilePoint.X > tile.X)
            {
                Game1.player.FacingDirection = 3;
            }
            else if (Game1.player.TilePoint.X < tile.X)
            {
                Game1.player.FacingDirection = 1;
            }

            if (Game1.player.isEmoteAnimating)
            {
                Game1.player.EndEmoteAnimation();
            }

            Game1.player.FarmerSprite.SetOwner(Game1.player);
            Game1.player.CanMove = false;
            Game1.player.UsingTool = true;
            Game1.player.canReleaseTool = true;

            Game1.player.Halt();
            Game1.player.CurrentTool.Update(Game1.player.FacingDirection, 0, Game1.player);

            Game1.player.stopJittering();
            Game1.player.canReleaseTool = false;

            int addedAnimationMultiplayer = ((!(Game1.player.Stamina <= 0f)) ? 1 : 2);
            if (Game1.isAnyGamePadButtonBeingPressed() || !Game1.player.IsLocalPlayer)
            {
                Game1.player.lastClick = Game1.player.GetToolLocation();
            }

            if (wateringCan.WaterLeft > 0 && Game1.player.ShouldHandleAnimationSound())
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
                this.currentGroup += 1;
                this.currentTile = 0;

                if (this.currentGroup >= this.path.Count)
                {
                    WateringCan? wateringCan = this.getCurrentWateringCanOrStop();
                    if (wateringCan == null)
                    {
                        return;
                    }

                    if (WaterBot.config.RefillOnFinish && WaterBot.config.RefillIfLower * 0.01 > (float)wateringCan.WaterLeft / (float)wateringCan.waterCanMax)
                    {
                        this.refillWater();
                        return;
                    }
                    this.end();
                    return;
                }

                if (console != null)
                {
                    this.order = this.map.findFillPath(this.path[this.currentGroup], console);
                }
                else
                {
                    this.console?.Invoke("Console is null. Ending process.");
                    this.active = false;
                    return;
                }
            }

            Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, this.order[this.currentTile].getStand(), 2, this.startWatering);
        }

        public void navigateNoUpdate()
        {
            Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, this.order[this.currentTile].getStand(), 2, this.startWatering);
        }

        public void refillWater()
        {
            if (!this.active) return;

            Tile playerLocation = this.map.map[Game1.player.TilePoint.Y][Game1.player.TilePoint.X];

            if (console != null)
            {
                this.refillStation = this.map.getClosestRefill(playerLocation, console);
            }
            else
            {
                this.console?.Invoke("Console is null. Ending process.");
                this.active = false;
                return;
            }

            if (!this.active) return;

            if (this.refillStation != null)
            {
                Game1.player.controller = new PathFindController(Game1.player, Game1.currentLocation, refillStation.getStand(), 2, this.startRefilling);
            }
            else
            {
                this.noWater();
            }
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

            Point point = this.refillStation?.Pop() ?? new Point(-1, -1);

            if (point.X != -1)
            {
                this.water(point);
                Task.Delay(800).ContinueWith(o => {
                    if (WaterBot.config.RedoPathOnRefill)
                    {
                        this.start(console, false);
                    }
                    else
                    {
                        this.navigateNoUpdate();
                    }
                });
            }
        }

        private WateringCan? getCurrentWateringCanOrStop()
        {
            if (Game1.player?.CurrentTool is WateringCan wateringCan)
            {
                return wateringCan;
            }

            if (this.active)
            {
                this.console?.Invoke("Bot interrupted because current tool changed. Ending process.");
                this.stop();
            }

            return null;
        }

        /// <summary>
        /// Cancels the bot's progress.
        /// </summary>
        public void stop()
        {
            this.active = false;
            Game1.player.controller = null;
            this.displayMessage(this.helper.Translation.Get("process.interrupt"), 1);
        }

        /// <summary>
        /// Cancels the bot's progress.
        /// </summary>
        public void exhausted()
        {
            this.console?.Invoke("Bot interrupted by lack of stamina. Ending process.");
            this.active = false;
            Game1.player.controller = null;
            this.displayMessage(this.helper.Translation.Get("process.exhausted"), 3);
        }

        /// <summary>
        /// Cancels the bot's progress.
        /// </summary>
        public void noWater()
        {
            this.console?.Invoke("Bot could not find suitable refill tile. Ending process.");
            this.active = false;
            Game1.player.controller = null;
            this.displayMessage(this.helper.Translation.Get("process.waterless"), 3);
        }

        /// <summary>
        /// Cancels the bot's progress.
        /// </summary>
        public void end()
        {
            this.console?.Invoke("Bot finished watering accessible crops. Ending process.");
            this.active = false;
            Game1.player.controller = null;
            this.displayMessage(this.helper.Translation.Get("process.end"), 1);
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
