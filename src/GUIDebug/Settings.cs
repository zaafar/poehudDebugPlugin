using System.Windows.Forms;
using SharpDX;
using PoeHUD.Plugins;
using PoeHUD.Hud.Settings;

namespace GUIDebug
{
    //All properties and public fields of this class will be saved to file
    public class Settings : SettingsBase
    {
        public Settings()
        {
            Enable = true;
            continousUpdating = false;

            Clear = Keys.Back;
            EnterSelect = Keys.Right;
            Up = Keys.Up;
            Down = Keys.Down;
            Back = Keys.Left;
            Save = Keys.J;
        }

        [Menu("Clear")]
        public HotkeyNode Clear { get; set; }
        [Menu("EnterSelect")]
        public HotkeyNode EnterSelect { get; set; }
        [Menu("Up")]
        public HotkeyNode Up { get; set; }
        [Menu("Down")]
        public HotkeyNode Down { get; set; }
        [Menu("Back")]
        public HotkeyNode Back { get; set; }

        [Menu("SaveSelectedAddrToFile")]
        public HotkeyNode Save { get; set; }
        [Menu("Continous Updating")]
        public ToggleNode continousUpdating { get; set; }
    }
}
