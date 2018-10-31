using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvenMoreDeepWoods
{
    class FallDecoration : Grass
    {
        private static readonly int[][] INDICES =
        {
            new int[] { 527 },                      // cobweb
            new int[] { 558, 559 },                 // jack o'lantern
            new int[] { 592 },                      // spider
            new int[] { 593, 594 },                 // human bones
            new int[] { 531, 532, 533, 534, 535 }   // creepy hand
        };

        public readonly NetInt index = new NetInt(0);
        public readonly NetBool flipped = new NetBool(false);

        public FallDecoration()
          : base(Grass.springGrass, 1)
        {
            InitNetFields();
        }

        public FallDecoration(int seed)
          : this()
        {
            Random random = new Random(seed);
            int i1 = random.Next() % INDICES.Length;
            int i2 = random.Next() % INDICES[i1].Length;
            this.index.Value = INDICES[i1][i2];
            this.flipped.Value = random.Next() % 2 == 0;
        }

        private void InitNetFields()
        {
            this.NetFields.AddFields(this.index, this.flipped);
        }

        public override bool seasonUpdate(bool onLoad)
        {
            return false;
        }

        protected override string textureName()
        {
            return "Maps\\Festivals";
        }

        public override void loadSprite()
        {

        }

        public override bool isPassable(Character c = null)
        {
            return index.Value == 527;
        }

        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location = null)
        {
            return false;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64, tileLocation.Y * 64));

            Rectangle destinationRectangle = new Rectangle((int)local.X, (int)local.Y, 64, 64);
            Rectangle sourceRectangle = Game1.getSourceRectForStandardTileSheet(this.texture.Value, this.index, 16, 16);

            spriteBatch.Draw(this.texture.Value, destinationRectangle, sourceRectangle, Color.White, 0, Vector2.Zero, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}
