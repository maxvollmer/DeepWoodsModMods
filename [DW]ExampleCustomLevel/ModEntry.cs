using DeepWoodsMod.API;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using xTile;
using xTile.Dimensions;

namespace ExampleCustomLevel
{
    public class ModEntry : Mod
    {
        private IDeepWoodsAPI deepWoodsAPI = null;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTicked;
        }

        private void GameEvents_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Helper.ModRegistry.IsLoaded("maxvollmer.deepwoodsmod"))
            {
                deepWoodsAPI = Helper.ModRegistry.GetApi<IDeepWoodsAPI>("maxvollmer.deepwoodsmod");
                if (deepWoodsAPI != null)
                {
                    CustomizeDeepWoods();
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

        private void CustomizeDeepWoods()
        {
            // Load custom map (every even level):
            deepWoodsAPI.OverrideMapGeneration += LoadCustomMap;

            // Disable default DeepWoods stuff in custom map (every even level):
            deepWoodsAPI.OverrideDebrisCreation += deepWoodsLocation => deepWoodsLocation.Level % 2 == 0;
            deepWoodsAPI.OverrideFill += deepWoodsLocation => deepWoodsLocation.Level % 2 == 0;
            deepWoodsAPI.OverrideMonsterGeneration += deepWoodsLocation => deepWoodsLocation.Level % 2 == 0;
        }

        private static Location CustomMapEnterLocation { get; } = new Location(5, 5);
        private static Location CustomMapExitLocation { get; } = new Location(9, 9);

        private bool LoadCustomMap(IDeepWoodsLocation deepWoodsLocation)
        {
            if (deepWoodsLocation.Level % 2 == 1)
                return false;

            deepWoodsLocation.IsClearing = false;
            (deepWoodsLocation as GameLocation).IsOutdoors = false;

            Map map = Helper.Content.Load<Map>("ExampleMap.tbin", ContentSource.ModFolder);
            deepWoodsLocation.MapSize = Tuple.Create(map.DisplayWidth / 64, map.DisplayHeight / 64);
            (deepWoodsLocation as GameLocation).Map = map;

            deepWoodsLocation.EnterLocation = new Location(CustomMapEnterLocation.X + 1, CustomMapEnterLocation.Y + 1);

            if (deepWoodsLocation.ParentDeepWoods is IDeepWoodsLocation parent)
            {
                deepWoodsAPI.AddExitLocation(
                    deepWoodsLocation,
                    null,
                    CustomMapEnterLocation);
            }

            if (deepWoodsLocation.Exits.FirstOrDefault() is IDeepWoodsExit firstExit)
            {
                deepWoodsAPI.AddExitLocation(
                    deepWoodsLocation,
                    firstExit,
                    CustomMapExitLocation);
            }

            foreach (var exit in deepWoodsLocation.Exits)
            {
                exit.Location = new Location(CustomMapExitLocation.X - 1, CustomMapExitLocation.Y - 1);
            }

            return true;
        }
    }
}
