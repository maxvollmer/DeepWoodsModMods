using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace EvenMoreDeepWoods
{
    class WinterTree : LargeTerrainFeature
    {
        private Texture2D texture;

        public WinterTree(Vector2 tileLocation)
            : base()
        {
            this.tilePosition.Value = tileLocation;
            this.texture = Game1.content.Load<Texture2D>("Maps\\winter_town");
        }

        public override bool isActionable()
        {
            return false;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override bool isPassable(Character c = null)
        {
            return false;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 192, 128);
        }

        public override void loadSprite()
        {
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64, tileLocation.Y * 64));

            Rectangle topSourceRectangle = Game1.getSourceRectForStandardTileSheet(this.texture, 887, 16, 16);
            topSourceRectangle.Width = 48;
            topSourceRectangle.Height = 48;

            Rectangle bottomSourceRectangle = Game1.getSourceRectForStandardTileSheet(this.texture, 983, 16, 16);
            bottomSourceRectangle.Width = 48;
            bottomSourceRectangle.Height = 32;

            Rectangle topDestinationRectangle = new Rectangle((int)local.X, (int)local.Y - 192, 192, 192);
            Rectangle bottomDestinationRectangle = new Rectangle((int)local.X, (int)local.Y, 192, 128);

            spriteBatch.Draw(this.texture, topDestinationRectangle, topSourceRectangle, Color.White, 0, Vector2.Zero, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
            spriteBatch.Draw(this.texture, bottomDestinationRectangle, bottomSourceRectangle, Color.White, 0, Vector2.Zero, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));

            /*
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64, tileLocation.Y * 64));
            Rectangle destinationRectangle = new Rectangle((int)local.X, (int)local.Y, 64, 64);

            Vector2 globalPosition = tileLocation * 64f;

            Rectangle topSourceRectangle = Game1.getSourceRectForStandardTileSheet(this.texture, 887, 48, 48);
            Vector2 globalTopPosition = new Vector2(globalPosition.X, globalPosition.Y - 192);

            Rectangle bottomSourceRectangle = Game1.getSourceRectForStandardTileSheet(this.texture, 983, 48, 32);
            Vector2 globalBottomPosition = new Vector2(globalPosition.X, globalPosition.Y);

            spriteBatch.Draw(this.texture, destinationRectangle, topSourceRectangle, Color.White, 0, Vector2.Zero, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
            // spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, globalTopPosition), topSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
            // spriteBatch.Draw(this.texture, Game1.GlobalToLocal(Game1.viewport, globalBottomPosition), bottomSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
            */
        }
    }
}
