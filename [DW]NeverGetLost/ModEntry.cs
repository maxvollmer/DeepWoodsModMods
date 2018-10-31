using DeepWoodsMod.API;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace NeverGetLost
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            GameEvents.FirstUpdateTick += this.GameEvents_FirstUpdateTick;
        }

        private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
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
                deepWoodsLocation.CanGetLost = false;
            };
        }
    }
}
