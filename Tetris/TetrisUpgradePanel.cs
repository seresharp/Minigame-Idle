using Microsoft.Xna.Framework;

namespace MinigameIdle.Tetris
{
    public class TetrisUpgradePanel
    {
        public TetrisGame Game { get; init; }

        public Button SpeedButton { get; init; }

        public Button AutostartButton { get; init; }

        public Button[] GenericUpgradeButtons { get; init; }

        public TetrisUpgradePanel(TetrisGame game)
        {
            Game = game;

            SpeedButton = new(Game.MainGame, new(), Color.DimGray, $"Speed Upgrade (0/100)\n{GetSpeedCost(0)} Points");
            AutostartButton = new(Game.MainGame, new(), Color.DimGray, $"Unlock Autostart\n{GetAutostartCost(0)} Points");
            GenericUpgradeButtons = new Button[5];
            for (int i = 0; i < GenericUpgradeButtons.Length; i++)
            {
                GenericUpgradeButtons[i] = new(Game.MainGame, new(), Color.DimGray, "This is a bug!");
            }
        }

        public void Resize()
        {
            const int buttonX = 690;
            const int buttonY = 215;

            const int buttonWidth = 245;
            const int buttonHeight = 80;
            const int buttonPadding = 15;

            SpeedButton.Resize(new Rectangle(
                Game.MainGame.ScaleX(buttonX),
                Game.MainGame.ScaleY(buttonY),
                Game.MainGame.ScaleX(buttonWidth),
                Game.MainGame.ScaleY(buttonHeight)));

            AutostartButton.Resize(new Rectangle(
                Game.MainGame.ScaleX(buttonX),
                Game.MainGame.ScaleY(buttonY + buttonHeight + buttonPadding),
                Game.MainGame.ScaleX(buttonWidth),
                Game.MainGame.ScaleY(buttonHeight)));

            for (int i = 0; i < GenericUpgradeButtons.Length; i++)
            {
                GenericUpgradeButtons[i].Resize(new Rectangle(
                    Game.MainGame.ScaleX(buttonX),
                    Game.MainGame.ScaleY(buttonY + ((i + 2) * (buttonHeight + buttonPadding))),
                    Game.MainGame.ScaleX(buttonWidth),
                    Game.MainGame.ScaleY(buttonHeight)));
            }
        }

        public void Update()
        {
            // Decide which upgrades to place on generic buttons

            // Button input
            if (SpeedButton.WasClicked() && Game.BoughtUpgrades.TickUpgrades < 100
                && Game.MainGame.Points >= GetSpeedCost(Game.BoughtUpgrades.TickUpgrades))
            {
                Game.MainGame.Points -= GetSpeedCost(Game.BoughtUpgrades.TickUpgrades);
                Game.BoughtUpgrades.TickUpgrades++;
            }

            if (AutostartButton.WasClicked() && Game.BoughtUpgrades.AutoStartUpgrades < 11
                && Game.MainGame.Points >= GetAutostartCost(Game.BoughtUpgrades.AutoStartUpgrades))
            {
                Game.MainGame.Points -= GetAutostartCost(Game.BoughtUpgrades.AutoStartUpgrades);
                Game.BoughtUpgrades.AutoStartUpgrades++;
            }

            // Button text
            if (Game.BoughtUpgrades.TickUpgrades == 100)
            {
                SpeedButton.UpdateText("Speed Upgrade (100/100)");
            }
            else
            {
                int up = Game.BoughtUpgrades.TickUpgrades;
                SpeedButton.UpdateText($"Speed Upgrade ({up}/100){$"\n{GetSpeedCost(up):0.##} Points"}");
            }

            SpeedButton.UpdateText($"Speed Upgrade ({Game.BoughtUpgrades.TickUpgrades}/100){(Game.BoughtUpgrades.TickUpgrades >= 100 ? "" : $"\n{GetSpeedCost(Game.BoughtUpgrades.TickUpgrades):0.##} Points")}");

            if (Game.BoughtUpgrades.AutoStartUpgrades == 0)
            {
                AutostartButton.UpdateText($"Unlock Autostart\n{GetAutostartCost(0)} Points");
            }
            else if (Game.BoughtUpgrades.AutoStartUpgrades == 11)
            {
                AutostartButton.UpdateText("Autostart Upgrade (10/10)");
            }
            else
            {
                int up = Game.BoughtUpgrades.AutoStartUpgrades;
                AutostartButton.UpdateText($"Autostart Upgrade ({up - 1}/10){$"\n{GetAutostartCost(up):0.##} Points"}");
            }
        }

        public void Draw()
        {
            SpeedButton.Draw();
            AutostartButton.Draw();
            foreach (Button b in GenericUpgradeButtons)
            {
                b.Draw();
            }
        }

        public static double GetSpeedCost(int upgrades)
            => Math.Pow(upgrades + 1, 3) / 50;

        public static double GetAutostartCost(int upgrades)
            => Math.Pow(2.95, upgrades);
    }
}
