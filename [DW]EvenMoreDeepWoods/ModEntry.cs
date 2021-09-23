using DeepWoodsMod.API;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace EvenMoreDeepWoods
{
    public class ModEntry : Mod
    {
        private static int SALT { get; } = 35714856;

        private int[] flowerTypes = new int[]{
            427, // 591, // Tulip, spring (30g)
            429, // 597, // BlueJazz, spring (50g)
            455, // 593, // SummerSpangle, summer (90g)
            453, // 376, // Poppy, summer (140g)
            425, // 595, // FairyRose, fall (290g)
        };

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.GameEvents_UpdateTicked;
        }

        private void GameEvents_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (e.Ticks == 1)
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
        }

        private void CustomizeDeepWoods(IDeepWoodsAPI deepWoodsAPI)
        {
            deepWoodsAPI.RegisterObject(ShouldAddSeasonableObjectHere, CreateSeasonableObject);
            deepWoodsAPI.OverrideFill += FillClearingWithEvenMoreThings;
        }

        private Random currentRandom = null;
        private int currentSeed = 0;
        private double currentLuck = 0.0;

        private bool ShouldAddSeasonableObjectHere(IDeepWoodsLocation deepWoodsLocation, Vector2 location)
        {
            if (deepWoodsLocation.IsClearing || deepWoodsLocation.IsCustomMap)
                return false;

            currentLuck = deepWoodsLocation.LuckLevel;
            if (currentSeed != deepWoodsLocation.Seed || currentRandom == null)
            {
                currentRandom = new Random(deepWoodsLocation.Seed ^ SALT);
                currentSeed = deepWoodsLocation.Seed;
            }

            // 20% chance (no luck modifier)
            return currentRandom.NextDouble() < 0.2;
        }

        private StardewValley.Object CreateSeasonableObject()
        {
            int parentSheetIndex;
            if (Game1.currentSeason == "summer" && Game1.random.NextDouble() < 0.1)
                parentSheetIndex = 259; // Fiddlehead Fern
            else
                parentSheetIndex = GetForagableForCurrentSeason(currentRandom.Next());

            return new StardewValley.Object(parentSheetIndex, 1, false, -1, GetRandomObjectQuality(currentRandom)) { IsSpawnedObject = true };
        }

        private int GetForagableForCurrentSeason(int random)
        {
            if (Game1.currentSeason == "spring")
            {
                return new int[] {
                    16,
                    18,
                    20,
                    22
                }[random % 4];
            }
            else if (Game1.currentSeason == "summer")
            {
                return new int[] {
                    396,
                    398,
                    400,
                    402
                }[random % 4];
            }
            else if (Game1.currentSeason == "fall")
            {
                return new int[] {
                    406,
                    408,
                    410
                }[random % 3];
            }
            else if (Game1.currentSeason == "winter")
            {
                return new int[] {
                    412,
                    414,
                    416,
                    418
                }[random % 4];
            }
            throw new Exception("Invalid season: " + Game1.currentSeason);
        }

        private int GetRandomObjectQuality(Random random)
        {
            double luck = Game1.player.team.sharedDailyLuck.Value + currentLuck;

            if (luck < 0)
                return StardewValley.Object.lowQuality;

            double d = random.NextDouble();
            if (d < (luck * 0.1))
                return StardewValley.Object.bestQuality;
            else if (d < (luck * 0.25))
                return StardewValley.Object.highQuality;
            else if (d < (luck * 0.5))
                return StardewValley.Object.medQuality;
            else
                return StardewValley.Object.lowQuality;
        }

        private bool FillClearingWithEvenMoreThings(IDeepWoodsLocation deepWoodsLocation)
        {
            if (!deepWoodsLocation.IsClearing || deepWoodsLocation.IsCustomMap)
                return false;

            var random = new Random(deepWoodsLocation.Seed ^ SALT);

            // 10% chance +- 20% from luck
            if ((random.NextDouble() * 10.0) < (1.0 + Game1.player.team.sharedDailyLuck.Value + deepWoodsLocation.LuckLevel))
            {
                if (Game1.currentSeason == "winter")
                    AddWinterStuffToClearing(deepWoodsLocation);
                else if (Game1.currentSeason == "fall")
                    AddFallStuffToClearing(deepWoodsLocation);
                else if (Game1.currentSeason == "summer")
                    AddSummerStuffToClearing(deepWoodsLocation);
                else if (Game1.currentSeason == "spring")
                    AddSpringStuffToClearing(deepWoodsLocation);

                return true;
            }

            return false;
        }

        private StardewValley.Object CreateBeeHouse(Vector2 location)
        {
            var beeHouse = new StardewValley.Object(location, 10, false);
            beeHouse.heldObject.Value = new StardewValley.Object(Vector2.Zero, 340, null, false, true, false, false);
            beeHouse.readyForHarvest.Value = true;
            beeHouse.MinutesUntilReady = 0;
            beeHouse.showNextIndex.Value = true;
            return beeHouse;
        }

        private TerrainFeature CreateFlower(int flowerType, Vector2 location)
        {
            return (TerrainFeature)ReflectionHelper.GetDeepWoodsType("Flower")
                .GetConstructor(new Type[] { typeof(int), typeof(Vector2) })
                .Invoke(new object[] { flowerType, location });
        }

        private int GetRandomFlowerType(Random random)
        {
            return flowerTypes[random.Next(0, flowerTypes.Length)];
        }

        private bool IsAreaFree(GameLocation gameLocation, Vector2 location, int width, int height)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (!gameLocation.isTileLocationTotallyClearAndPlaceable(new Vector2(location.X + x, location.Y + y)))
                        return false;
            return true;
        }

        private void AddSpringStuffToClearing(IDeepWoodsLocation deepWoodsLocation)
        {
            // Flower field with bee houses and honey

            Random random = new Random(deepWoodsLocation.Seed ^ SALT ^ Game1.random.Next());

            int flowerBorderDist = Math.Min(8, Math.Min(deepWoodsLocation.MapSize.Item1, deepWoodsLocation.MapSize.Item2) / 2 - 1);
            int beeHouseBorderDist = Math.Min(10, Math.Min(deepWoodsLocation.MapSize.Item1, deepWoodsLocation.MapSize.Item2) / 2 - 1);

            int numBeeHouses = random.Next(4, 9);
            int numFlowers = random.Next(16, 36);
            int numGrass = random.Next(30, 60);

            var objects = (deepWoodsLocation as GameLocation).objects;
            var terrainFeatures = (deepWoodsLocation as GameLocation).terrainFeatures;

            for (int i = 0; i < numBeeHouses; i++)
            {
                Vector2 location = new Vector2(
                    random.Next(beeHouseBorderDist, deepWoodsLocation.MapSize.Item1 - beeHouseBorderDist),
                    random.Next(beeHouseBorderDist, deepWoodsLocation.MapSize.Item2 - beeHouseBorderDist)
                    );
                if (objects.ContainsKey(location) || terrainFeatures.ContainsKey(location))
                    continue;
                objects[location] = CreateBeeHouse(location);
            }

            for (int i = 0; i < numFlowers; i++)
            {
                Vector2 location = new Vector2(
                    random.Next(flowerBorderDist, deepWoodsLocation.MapSize.Item1 - flowerBorderDist),
                    random.Next(flowerBorderDist, deepWoodsLocation.MapSize.Item2 - flowerBorderDist)
                    );
                if (objects.ContainsKey(location) || terrainFeatures.ContainsKey(location))
                    continue;
                terrainFeatures[location] = CreateFlower(GetRandomFlowerType(random), location);
            }

            for (int i = 0; i < numGrass; i++)
            {
                Vector2 location = new Vector2(
                    random.Next(flowerBorderDist, deepWoodsLocation.MapSize.Item1 - flowerBorderDist),
                    random.Next(flowerBorderDist, deepWoodsLocation.MapSize.Item2 - flowerBorderDist)
                    );
                if (objects.ContainsKey(location) || terrainFeatures.ContainsKey(location))
                    continue;
                terrainFeatures[location] = new Grass(Grass.springGrass, random.Next(1, 3));
            }
        }

        private void AddSummerStuffToClearing(IDeepWoodsLocation deepWoodsLocation)
        {
            // We just do the spring stuff here
            AddSpringStuffToClearing(deepWoodsLocation);
        }

        private void AddFallStuffToClearing(IDeepWoodsLocation deepWoodsLocation)
        {
            // Huge pumpkins with scary deco

            Random random = new Random(deepWoodsLocation.Seed ^ SALT ^ Game1.random.Next());

            int borderDist = Math.Min(8, Math.Min(deepWoodsLocation.MapSize.Item1, deepWoodsLocation.MapSize.Item2) / 2 - 1);

            int numBigPumpkins = random.Next(1, 4);
            int numSmallPumpkins = random.Next(10, 20);
            int numDecorations = random.Next(10, 20);

            var objects = (deepWoodsLocation as GameLocation).objects;
            var resourceClumps = deepWoodsLocation.ResourceClumps;
            var terrainFeatures = (deepWoodsLocation as GameLocation).terrainFeatures;

            for (int i = 0; i < numBigPumpkins; i++)
            {
                Vector2 location = new Vector2(
                    random.Next(borderDist, deepWoodsLocation.MapSize.Item1 - borderDist),
                    random.Next(borderDist, deepWoodsLocation.MapSize.Item2 - borderDist)
                    );
                if (!IsAreaFree(deepWoodsLocation as GameLocation, location, 3, 3))
                    continue;
                resourceClumps.Add(new GiantCrop(276, location));
            }

            for (int i = 0; i < numSmallPumpkins; i++)
            {
                Vector2 location = new Vector2(
                    random.Next(borderDist, deepWoodsLocation.MapSize.Item1 - borderDist),
                    random.Next(borderDist, deepWoodsLocation.MapSize.Item2 - borderDist)
                    );
                if (!IsAreaFree(deepWoodsLocation as GameLocation, location, 1, 1))
                    continue;
                objects[location] = new StardewValley.Object(276, 1, false, -1, GetRandomObjectQuality(random)) { IsSpawnedObject = true };
            }

            for (int i = 0; i < numDecorations; i++)
            {
                Vector2 location = new Vector2(
                    random.Next(borderDist, deepWoodsLocation.MapSize.Item1 - borderDist),
                    random.Next(borderDist, deepWoodsLocation.MapSize.Item2 - borderDist)
                    );
                if (!IsAreaFree(deepWoodsLocation as GameLocation, location, 1, 1))
                    continue;
                terrainFeatures[location] = new FallDecoration(random.Next());
            }
        }

        private void AddWinterStuffToClearing(IDeepWoodsLocation deepWoodsLocation)
        {
            // Winter tree with presents

            Random random = new Random(deepWoodsLocation.Seed ^ SALT ^ Game1.random.Next());

            int borderDist = Math.Min(12, Math.Min(deepWoodsLocation.MapSize.Item1, deepWoodsLocation.MapSize.Item2) / 2 - 1);

            int numPresents = random.Next(6, 16);

            var largeTerrainFeatures = (deepWoodsLocation as GameLocation).largeTerrainFeatures;
            var terrainFeatures = (deepWoodsLocation as GameLocation).terrainFeatures;

            largeTerrainFeatures.Add(new WinterTree(new Vector2(deepWoodsLocation.MapSize.Item1 / 2 + random.Next(-3, 0), deepWoodsLocation.MapSize.Item2 / 2 + random.Next(-3, 0))));

            for (int i = 0; i < numPresents; i++)
            {
                Vector2 location = new Vector2(
                    random.Next(borderDist, deepWoodsLocation.MapSize.Item1 - borderDist),
                    random.Next(borderDist, deepWoodsLocation.MapSize.Item2 - borderDist)
                    );
                if (!IsAreaFree(deepWoodsLocation as GameLocation, location, 1, 1))
                    continue;
                terrainFeatures[location] = new WinterPresent(random.Next());
            }
        }
    }
}
