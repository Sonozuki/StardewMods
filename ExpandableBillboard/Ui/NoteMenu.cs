using ExpandableBillboard.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ExpandableBillboard.Ui
{
    public class NoteMenu : IClickableMenu
    {
        private ClickableComponent AcceptQuestButton;
        private Texture2D AcceptQuestButtonTexture;
        private Texture2D NoteBackgroundTexture;
        private BillBoardQuest Quest;

        private Vector2 QuestTitleTextDimensions;

        public NoteMenu(BillBoardQuest quest)
        {
            Quest = quest;

            // load assets
            AcceptQuestButtonTexture = ModEntry.ModHelper.Content.Load<Texture2D>("Assets/AcceptButton.png", ContentSource.ModFolder);
            NoteBackgroundTexture = ModEntry.ModHelper.Content.Load<Texture2D>("Assets/NoteBackground.png", ContentSource.ModFolder);
            var closeButtonTexture = ModEntry.ModHelper.Content.Load<Texture2D>("Assets/CloseButton.png", ContentSource.ModFolder);

            // get the top left position for the background asset (pass it *4 as that's the scale)
            Vector2 backgroundTopLeftPosition = Utility.getTopLeftPositionForCenteringOnScreen(NoteBackgroundTexture.Width * 4, NoteBackgroundTexture.Height * 4);
            this.xPositionOnScreen = (int)backgroundTopLeftPosition.X;
            this.yPositionOnScreen = (int)backgroundTopLeftPosition.Y;

            this.upperRightCloseButton = new ClickableTextureComponent(
                bounds: new Rectangle(this.xPositionOnScreen + NoteBackgroundTexture.Width * 4 - 32, this.yPositionOnScreen - 24, 48, 48), 
                texture: closeButtonTexture, 
                sourceRect: new Rectangle(0, 0, 12, 12),
                scale: 4f
            );

            Vector2 acceptQuestButtonDimensions = Game1.dialogueFont.MeasureString("Accept Quest") + new Vector2(24);
            AcceptQuestButton = new ClickableComponent(
                bounds: new Rectangle(
                    x: this.xPositionOnScreen + (NoteBackgroundTexture.Width / 2 * 4) - ((int)acceptQuestButtonDimensions.X / 2),
                    y: this.yPositionOnScreen + (NoteBackgroundTexture.Height * 4) - (int)((int)acceptQuestButtonDimensions.Y / 1.5),
                    width: (int)acceptQuestButtonDimensions.X,
                    height: (int)acceptQuestButtonDimensions.Y
                ),
                name: ""
            );

            QuestTitleTextDimensions = Game1.dialogueFont.MeasureString(Quest.Title);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            // zoom in on the accept quest and turn pink if hovered
            float oldScale = AcceptQuestButton.scale;
            AcceptQuestButton.scale = AcceptQuestButton.bounds.Contains(x, y) ? 1.5f : 1f;
            if (AcceptQuestButton.scale > oldScale)
            {
                Game1.playSound("Cowboy_gunshot");
            }
        }

        public override void draw(SpriteBatch b)
        {
            // dark background
            b.Draw(
                texture: Game1.fadeToBlackRect,
                destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds,
                color: Color.Black * 0.75f
            );

            // note background
            b.Draw(
                texture: NoteBackgroundTexture,
                position: new Vector2(this.xPositionOnScreen, this.yPositionOnScreen),
                sourceRectangle: new Rectangle(0, 0, NoteBackgroundTexture.Width, NoteBackgroundTexture.Height),
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: 4,
                effects: SpriteEffects.None,
                layerDepth: 1
            );

            // quest title
            string questTitle = Game1.parseText(
                text: Quest.Title, 
                whichFont: Game1.dialogueFont, 
                width: 640
            );
            Utility.drawTextWithColoredShadow(
                b: b,
                text: questTitle,
                font: Game1.dialogueFont,
                position: new Vector2(this.xPositionOnScreen + NoteBackgroundTexture.Width / 2 * 4 - QuestTitleTextDimensions.X / 2 * 1.5f, this.yPositionOnScreen + 64),
                color: Game1.textColor,
                scale: 1.5f,
                shadowColor: new Color(151, 151, 151)
            );

            // quest description
            string questDescription = Game1.parseText(
                text: Quest.Description,
                whichFont: Game1.dialogueFont,
                width: 500
            );
            Utility.drawTextWithColoredShadow(
                b: b,
                text: questDescription,
                font: Game1.dialogueFont,
                position: new Vector2(this.xPositionOnScreen + 32, this.yPositionOnScreen + 128),
                color: Game1.textColor,
                shadowColor: new Color(151, 151, 151)
            );

            // requester sign off
            string questRequester = Game1.parseText(
                text: $"   - {Quest.Requester}",
                whichFont: Game1.dialogueFont,
                width: 640
            );
            Utility.drawTextWithColoredShadow(
                b: b,
                text: questRequester,
                font: Game1.dialogueFont,
                position: new Vector2(this.xPositionOnScreen + 64, this.yPositionOnScreen + NoteBackgroundTexture.Height * 4 - 248),
                color: Game1.textColor,
                shadowColor: new Color(151, 151, 151)
            );

            // friendship reward
            if (Quest.FriendshipReward > 0)
            {
                string questFriendshipReward = Game1.parseText(
                    text: $"- {Quest.Requester} will be thankful",
                    whichFont: Game1.dialogueFont,
                    width: 640
                );
                Utility.drawTextWithColoredShadow(
                    b: b,
                    text: questFriendshipReward,
                    font: Game1.dialogueFont,
                    position: new Vector2(this.xPositionOnScreen + 64, this.yPositionOnScreen + NoteBackgroundTexture.Height * 4 - 128),
                    color: Game1.textColor,
                    shadowColor: new Color(151, 151, 151)
                );
            }

            // gold reward 
            if (Quest.MoneyReward > 0)
            {
                string questMoneyReward = Game1.parseText(
                    text: $"- {Quest.MoneyReward}g on delivery",
                    whichFont: Game1.dialogueFont,
                    width: 640
                );
                Utility.drawTextWithColoredShadow(
                    b: b,
                    text: questMoneyReward,
                    font: Game1.dialogueFont,
                    position: new Vector2(this.xPositionOnScreen + 64, this.yPositionOnScreen + NoteBackgroundTexture.Height * 4 - (Quest.FriendshipReward > 0 ? 176 : 128)),
                    color: Game1.textColor,
                    shadowColor: new Color(151, 151, 151)
                );
            }

            // accept button button
            IClickableMenu.drawTextureBox(
                b: b,
                texture: AcceptQuestButtonTexture,
                sourceRect: new Rectangle(0, 0, 9, 9),
                x: AcceptQuestButton.bounds.X,
                y: AcceptQuestButton.bounds.Y,
                width: AcceptQuestButton.bounds.Width,
                height: AcceptQuestButton.bounds.Height,
                color: AcceptQuestButton.scale > 1 ? Color.LightPink : Color.White, // when hovering over the acceptQuest button, it should be pink
                scale: 4f * AcceptQuestButton.scale,
                drawShadow: true
            );

            // accept quest text
            Utility.drawTextWithColoredShadow(
                b: b,
                text: "Accept Quest",
                font: Game1.dialogueFont,
                position: new Vector2((float)(AcceptQuestButton.bounds.X + 12), (float)(AcceptQuestButton.bounds.Y + 16)),
                color: Game1.textColor,
                shadowColor: AcceptQuestButton.scale > 1 ? new Color(171,93, 104) : new Color(151, 151, 151)
            );

            // close button
            base.draw(b);

            // cursor
            this.drawMouse(b);
        }
    }
}
