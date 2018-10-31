using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;
using System.Linq;

namespace EvenMoreDeepWoods
{
    class WinterPresent : FallDecoration
    {
        private static readonly string[] LOVED_GIFTS =
        {
            "Golden Pumpkin",
            "Pearl",
            "Prismatic Shard",
            "Rabbit's Foot",
            "Complete Breakfast",
            "Salmon Dinner",
            "Crab Cakes",
            "Duck Feather",
            "Lobster",
            "Pomegranate",
            "Tom Kha Soup",
            "Coffee",
            "Pickles",
            "Super Meal",
            "Truffle Oil",
            "Wine",
            "Cactus Fruit",
            "Maple Bar",
            "Pizza",
            "Tigerseye",
            "Frozen Tear",
            "Obsidian",
            "Pumpkin Soup",
            "Sashimi",
            "Void Egg",
            "Beer",
            "Hot Pepper",
            "Pepper Poppers",
            "Pizza",
            "Amethyst",
            "Blackberry Cobbler",
            "Chocolate Cake",
            "Pufferfish",
            "Pumpkin",
            "Spicy Eel",
            "Amethyst",
            "Aquamarine",
            "Cloth",
            "Emerald",
            "Jade",
            "Ruby",
            "Survival Burger",
            "Topaz",
            "Wool",
            "Coconut",
            "Fruit Salad",
            "Pink Cake",
            "Sunflower",
            "Goat Cheese",
            "Poppyseed Muffin",
            "Salad",
            "Stir Fry",
            "Truffle",
            "Vegetable Medley",
            "Wine",
            "Battery Pack",
            "Cauliflower",
            "Cheese Cauliflower",
            "Diamond",
            "Gold Bar",
            "Iridium Bar",
            "Miner's Treat",
            "Pepper Poppers",
            "Rhubarb Pie",
            "Strawberry",
            "Diamond",
            "Emerald",
            "Melon",
            "Poppy",
            "Poppyseed Muffin",
            "Red Plate",
            "Roots Platter",
            "Sandfish",
            "Tom Kha Soup",
            "Fish Taco",
            "Summer Spangle",
            "Amethyst",
            "Aquamarine",
            "Artichoke Dip",
            "Emerald",
            "Fiddlehead Risotto",
            "Gold Bar",
            "Iridium Bar",
            "Jade",
            "Omni Geode",
            "Ruby",
            "Topaz",
            "Bean Hotpot",
            "Ice Cream",
            "Rice Pudding",
            "Strawberry",
            "Amethyst",
            "Aquamarine",
            "Emerald",
            "Jade",
            "Omni Geode",
            "Ruby",
            "Topaz",
            "Beet",
            "Chocolate Cake",
            "Diamond",
            "Fairy Rose",
            "Stuffing",
            "Tulip",
            "Fried Mushroom",
            "Leek",
            "Diamond",
            "Escargot",
            "Fish Taco",
            "Orange",
            "Fairy Rose",
            "Pink Cake",
            "Plum Pudding",
            "Chocolate Cake",
            "Crispy Bass",
            "Diamond",
            "Eggplant Parmesan",
            "Fried Eel",
            "Pancakes",
            "Rhubarb Pie",
            "Vegetable Medley",
            "Fiddlehead Risotto",
            "Roasted Hazelnuts",
            "Diamond",
            "Iridium Bar",
            "Pumpkin",
            "Void Egg",
            "Void Mayonnaise",
            "Wild Horseradish",
            "Autumn's Bounty",
            "Glazed Yams",
            "Hot Pepper",
            "Vegetable Medley",
            "Blueberry Tart",
            "Cactus Fruit",
            "Coconut",
            "Dish o' The Sea",
            "Yam",
            "Diamond",
            "Farmer's Lunch",
            "Pink Cake",
            "Pumpkin Pie",
            "Beer",
            "Cactus Fruit",
            "Glazed Yams",
            "Mead",
            "Pale Ale",
            "Parsnip",
            "Parsnip Soup",
            "Fried Calamari",
            "Goat Cheese",
            "Peach",
            "Spaghetti",
            "Crocus",
            "Daffodil",
            "Sweet Pea",
            "Cranberry Candy",
            "Grape",
            "Pink Cake",
            "Catfish",
            "Diamond",
            "Iridium Bar",
            "Mead",
            "Octopus",
            "Pumpkin",
            "Sea Cucumber",
            "Sturgeon",
            "Purple Mushroom",
            "Solar Essence",
            "Super Cucumber",
            "Void Essence"
        };

        private static readonly int[] INDICES =
        {
            728, 729, 730
        };

        public readonly NetBool wasPickedUp = new NetBool(false);
        public readonly NetInt seed = new NetInt(0);

        public WinterPresent()
          : base()
        {
            InitNetFields();
        }

        public WinterPresent(int seed)
          : this()
        {
            Random random = new Random(seed);
            this.index.Value = INDICES[random.Next() % INDICES.Length];
            this.flipped.Value = random.Next() % 2 == 0;
            this.seed.Value = random.Next();
        }

        private void InitNetFields()
        {
            this.NetFields.AddFields(this.wasPickedUp);
        }

        public override bool isPassable(Character c = null)
        {
            return this.wasPickedUp;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            if (this.wasPickedUp)
                return false;

            if (Game1.player.addItemToInventoryBool(CreatePresent(), false))
            {
                this.wasPickedUp.Value = true;
                Game1.player.currentLocation.playSound("coin");
                Game1.player.animateOnce(StardewValley.FarmerSprite.harvestItemUp + Game1.player.FacingDirection);
                Game1.player.canMove = false;
                return true;
            }
            else
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                return false;
            }
        }

        private StardewValley.Object CreatePresent()
        {
            return new StardewValley.Object(GetRandomPresentId(), 1, false, -1, 0);
        }

        private int GetRandomPresentId()
        {
            // This is not the most performant way to get these objects, but for this demo it shall suffice
            string lovedGift = LOVED_GIFTS[new Random(seed.Value ^ Game1.random.Next()).Next() % LOVED_GIFTS.Length];
            return Game1.objectInformation.FirstOrDefault(x => x.Value.StartsWith(lovedGift+"/")).Key;
        }

        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location = null)
        {
            return false;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            if (this.wasPickedUp)
                return;

            base.draw(spriteBatch, tileLocation);
        }
    }
}
