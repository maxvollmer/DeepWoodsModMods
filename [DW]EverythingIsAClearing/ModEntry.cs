using DeepWoodsMod.API;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace EverythingIsAClearing
{
    public class ModEntry : Mod
    {
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
            deepWoodsAPI.OnCreate += (IDeepWoodsLocation deepWoodsLocation) =>
            {
                if (!deepWoodsLocation.IsCustomMap) // we don't touch custom levels
                {
                    deepWoodsLocation.IsClearing = true;
                }
            };
        }
    }
}
