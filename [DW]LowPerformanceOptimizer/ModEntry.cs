using DeepWoodsMod.API;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace LowPerformanceOptimizer
{
    public class ModEntry : Mod
    {
        private static int SALT { get; } = 35917392;

        public class Settings
        {
            public double GrassReduction { get; set; } = 0.5;
        }

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTicked;
        }

        private void GameEvents_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Helper.ModRegistry.IsLoaded("maxvollmer.deepwoodsmod"))
            {
                IDeepWoodsAPI deepWoodsAPI = Helper.ModRegistry.GetApi<IDeepWoodsAPI>("maxvollmer.deepwoodsmod");
                if (deepWoodsAPI != null)
                {
                    CustomizeDeepWoods(deepWoodsAPI);
                }
                else
                {
                    Monitor.Log("DeepWoodsAPI could not be loaded.", LogLevel.Warn);
                }
            }
            else
            {
                Monitor.Log("DeepWoodsMod is not loaded.", LogLevel.Warn);
            }
        }

        private void CustomizeDeepWoods(IDeepWoodsAPI deepWoodsAPI)
        {
            var settings = Helper.ReadConfig<Settings>() ?? new Settings();

            // Disable baubles and weatherdebris (we could also reduce the amount in AfterDebrisCreation)
            deepWoodsAPI.OverrideDebrisCreation += (IDeepWoodsLocation) =>
            {
                return true;
            };

            // Reduce amount of grass
            deepWoodsAPI.AfterFill += (IDeepWoodsLocation deepWoodsLocation) =>
            {
                if (deepWoodsLocation.IsCustomMap) // we don't touch custom levels
                    return;

                var random = new Random(deepWoodsLocation.Seed ^ SALT);

                var terrainFeatures = (deepWoodsLocation as GameLocation).terrainFeatures;
                var terrainFeatureLocations = new List<Vector2>(terrainFeatures.Keys);
                foreach (var location in terrainFeatureLocations)
                {
                    if (terrainFeatures[location] is Grass
                        && (random.NextDouble() < settings.GrassReduction))
                    {
                        terrainFeatures.Remove(location);
                    }
                }
            };
        }
    }
}
