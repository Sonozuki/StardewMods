using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace CompletionistHelper
{
    public class CompletionistMenu : IClickableMenu
    {
        private readonly ClickableTextureComponent ScrollUpButton;
        private readonly ClickableTextureComponent ScrollDownButton;

        public CompletionistMenu()
        {
            // add scroll buttons
            ScrollUpButton = new ClickableTextureComponent(Rectangle.Empty, Game1.mouseCursors, new Rectangle(76, 72, 40, 44), 1);
            ScrollDownButton = new ClickableTextureComponent(Rectangle.Empty, Game1.mouseCursors, new Rectangle(12, 76, 40, 44), 1);

            // update layout
            UpdateLayout();
        }

        public override void draw(SpriteBatch spriteBatch)
        {
                // Calculate dimensions
                int x = this.xPositionOnScreen;
                int y = this.yPositionOnScreen;
                const int gutter = 15;
                float leftOffset = gutter;
                float topOffset = gutter;
                float contentWidth = this.width - gutter * 2;
                float contentHeight = this.height - gutter * 2;
                int tableBorderWidth = 1;

                // Get the game's font
                SpriteFont font = Game1.smallFont;
                float lineHeight = font.MeasureString("ABC").Y;
                float spaceWidth = font.MeasureString("A B").X - font.MeasureString("AB").X; ;

                // Draw the background
                using (SpriteBatch backgroundBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
                {
                    backgroundBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
                    backgroundBatch.Draw(Game1.content.Load<Texture2D>("LooseSprites\\letterBG"), new Vector2(x, y), new Rectangle(0, 0, 320, 180), Color.White, 0, Vector2.Zero, this.width / 320, SpriteEffects.None, 0);
                    backgroundBatch.End();
                }
            /*
                // Draw the foreground
                using (SpriteBatch contentBatch = new SpriteBatch(Game1.graphics.GraphicsDevice))
                {
                    GraphicsDevice device = Game1.graphics.GraphicsDevice;
                    Rectangle prevScissorRectangle = device.ScissorRectangle;
                    try
                    {
                        // begin draw
                        device.ScissorRectangle = new Rectangle(x + gutter, y + gutter, (int)contentWidth, (int)contentHeight);
                        contentBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, new RasterizerState { ScissorTestEnable = true });

                        // scroll view
                        this.CurrentScroll = Math.Max(0, this.CurrentScroll); // don't scroll past top
                        this.CurrentScroll = Math.Min(this.MaxScroll, this.CurrentScroll); // don't scroll past bottom
                        topOffset -= this.CurrentScroll; // scrolled down == move text up

                        // draw portrait
                        if (subject.DrawPortrait(contentBatch, new Vector2(x + leftOffset, y + topOffset), new Vector2(70, 70)))
                            leftOffset += 72;

                        // draw fields
                        float wrapWidth = this.width - leftOffset - gutter;
                        {
                            // draw name & item type
                            {
                                Vector2 nameSize = contentBatch.DrawTextBlock(font, $"{subject.Name}.", new Vector2(x + leftOffset, y + topOffset), wrapWidth, bold: Constant.AllowBold);
                                Vector2 typeSize = contentBatch.DrawTextBlock(font, $"{subject.Type}.", new Vector2(x + leftOffset + nameSize.X + spaceWidth, y + topOffset), wrapWidth);
                                topOffset += Math.Max(nameSize.Y, typeSize.Y);
                            }

                            // draw description
                            if (subject.Description != null)
                            {
                                Vector2 size = contentBatch.DrawTextBlock(font, subject.Description?.Replace(Environment.NewLine, " "), new Vector2(x + leftOffset, y + topOffset), wrapWidth);
                                topOffset += size.Y;
                            }

                            // draw spacer
                            topOffset += lineHeight;

                            // draw custom fields
                            if (this.Fields.Any())
                            {
                                ICustomField[] fields = this.Fields;
                                float cellPadding = 3;
                                float labelWidth = fields.Where(p => p.HasValue).Max(p => font.MeasureString(p.Label).X);
                                float valueWidth = wrapWidth - labelWidth - cellPadding * 4 - tableBorderWidth;
                                foreach (ICustomField field in fields)
                                {
                                    if (!field.HasValue)
                                        continue;

                                    // draw label & value
                                    Vector2 labelSize = contentBatch.DrawTextBlock(font, field.Label, new Vector2(x + leftOffset + cellPadding, y + topOffset + cellPadding), wrapWidth);
                                    Vector2 valuePosition = new Vector2(x + leftOffset + labelWidth + cellPadding * 3, y + topOffset + cellPadding);
                                    Vector2 valueSize =
                                        field.DrawValue(contentBatch, font, valuePosition, valueWidth)
                                        ?? contentBatch.DrawTextBlock(font, field.Value, valuePosition, valueWidth);
                                    Vector2 rowSize = new Vector2(labelWidth + valueWidth + cellPadding * 4, Math.Max(labelSize.Y, valueSize.Y));

                                    // draw table row
                                    Color lineColor = Color.Gray;
                                    contentBatch.DrawLine(x + leftOffset, y + topOffset, new Vector2(rowSize.X, tableBorderWidth), lineColor); // top
                                    contentBatch.DrawLine(x + leftOffset, y + topOffset + rowSize.Y, new Vector2(rowSize.X, tableBorderWidth), lineColor); // bottom
                                    contentBatch.DrawLine(x + leftOffset, y + topOffset, new Vector2(tableBorderWidth, rowSize.Y), lineColor); // left
                                    contentBatch.DrawLine(x + leftOffset + labelWidth + cellPadding * 2, y + topOffset, new Vector2(tableBorderWidth, rowSize.Y), lineColor); // middle
                                    contentBatch.DrawLine(x + leftOffset + rowSize.X, y + topOffset, new Vector2(tableBorderWidth, rowSize.Y), lineColor); // right

                                    // track link area
                                    if (field is ILinkField linkField)
                                        this.LinkFieldAreas[linkField] = new Rectangle((int)valuePosition.X, (int)valuePosition.Y, (int)valueSize.X, (int)valueSize.Y);

                                    // update offset
                                    topOffset += Math.Max(labelSize.Y, valueSize.Y);
                                }
                            }
                        }

                        // update max scroll
                        this.MaxScroll = Math.Max(0, (int)(topOffset - contentHeight + this.CurrentScroll));

                        // draw scroll icons
                        if (this.MaxScroll > 0 && this.CurrentScroll > 0)
                            this.ScrollUpButton.draw(contentBatch);
                        if (this.MaxScroll > 0 && this.CurrentScroll < this.MaxScroll)
                            this.ScrollDownButton.draw(spriteBatch);

                        // end draw
                        contentBatch.End();
                    }
                    finally
                    {
                        device.ScissorRectangle = prevScissorRectangle;
                    }
                }*/

                // draw cursor
                this.drawMouse(Game1.spriteBatch);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Update the layout dimensions based on the current game scale.</summary>
        private void UpdateLayout()
        {
            // update size
            width = Math.Min(Game1.tileSize * 14, Game1.viewport.Width);
            height = Math.Min((int)(180 / 320 * width), Game1.viewport.Height);

            // update position
            Vector2 origin = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
            xPositionOnScreen = (int)origin.X;
            yPositionOnScreen = (int)origin.Y;

            // update up/down buttons
            int x = xPositionOnScreen;
            int y = yPositionOnScreen;
            int gutter = 15;
            float contentHeight = height - gutter * 2;
            ScrollUpButton.bounds = new Rectangle(x + gutter, (int)(y + contentHeight - 44 - gutter - 44), 44, 40);
            ScrollDownButton.bounds = new Rectangle(x + gutter, (int)(y + contentHeight - 44), 44, 40);
        }
    }
}
