using PoeHUD.Plugins;
using PoeHUD.Poe;
using SharpDX;
using System.Collections.Generic;
using System.IO;


namespace GUIDebug
{
    public class Core : BaseSettingsPlugin<Settings>
    {
        public Core() { PluginName = "GUIDebug"; }

        public override void Initialise()
        {
            //Called on start
        }

        private Element CurElement;
        private Element CurSelectedChildElement;

        private int SelectedChildIndex = -1;
        private List<Element> Children = new List<Element>();
        private Stack<int> currentChildIndexStack = new Stack<int>();

        private void enterUiElement(Element uIHover)
        {
            if (CurElement == null)
                CurElement = uIHover;
            else if (CurSelectedChildElement != null)
            {
                CurElement = CurSelectedChildElement;
                currentChildIndexStack.Push(SelectedChildIndex);
            }

            if (CurElement != null)
            {
                Children = CurElement.Children;
                if (Children.Count > 0)
                {
                    SelectedChildIndex = 0;
                    CurSelectedChildElement = Children[0];
                }
                else
                {
                    CurSelectedChildElement = null;
                    SelectedChildIndex = -1;
                }
            }
        }
        private void exitUiElement()
        {
            if (CurElement != null && CurElement.Parent != null)
            {
                CurElement = CurElement.Parent;

                Children = CurElement.Children;
                if (Children.Count > 0)
                {
                    if (currentChildIndexStack.Count > 0)
                    {
                        SelectedChildIndex = currentChildIndexStack.Pop();
                        CurSelectedChildElement = Children[SelectedChildIndex];
                    }
                    else
                    {
                        SelectedChildIndex = 0;
                        CurSelectedChildElement = Children[0];
                    }
                }
                else
                {
                    CurSelectedChildElement = null;
                    SelectedChildIndex = -1;
                }
            }
        }
        private void clearEverything()
        {
            CurElement = null;
            Children = new List<Element>();
            SelectedChildIndex = -1;
            CurSelectedChildElement = null;
            currentChildIndexStack.Clear();
        }

        public override void Render()
        {
            var uIHover = GameController.Game.IngameState.UIHover;

            if (!Settings.Enable.Value)
                return;

            if (uIHover != null && CurElement == null) 
                Graphics.DrawFrame(uIHover.GetClientRect(), 2, Color.Yellow);

            if (Settings.Clear.PressedOnce())
                clearEverything();

            if (Settings.EnterSelect.PressedOnce())
                enterUiElement(uIHover);

            if (Settings.Back.PressedOnce())
                exitUiElement();

            #region Up/DownLogicAndContinousUpdate
            if (CurElement != null)
            {
                if(Settings.continousUpdating.Value && Children.Count != CurElement.Children.Count)
                    Children = CurElement.Children;

                if (Children.Count > 0)
                {
                    bool up = Settings.Up.PressedOnce();
                    if (up || Settings.Down.PressedOnce())
                    {
                        if (up)
                            SelectedChildIndex--;
                        else
                            SelectedChildIndex++;

                        if (SelectedChildIndex < 0)
                            SelectedChildIndex = Children.Count - 1;
                        else if (SelectedChildIndex >= Children.Count)
                            SelectedChildIndex = 0;

                        CurSelectedChildElement = Children[SelectedChildIndex];
                    } else if (Settings.continousUpdating.Value && SelectedChildIndex >= Children.Count)
                    {
                        SelectedChildIndex = 0;
                        CurSelectedChildElement = Children[SelectedChildIndex];
                    } else
                    {
                        CurSelectedChildElement = Children[SelectedChildIndex];
                    }

                    foreach (var child in Children)
                    {
                        string label = child.Address.ToString("X");
                        if (CurSelectedChildElement == child)
                            label = "-> " + label;

                        Graphics.DrawFrame(child.GetClientRect(), 2, Color.Gray);

                        LogMessage(label, 0);
                    }
                }
                else
                {
                    LogMessage("No children", 0);
                }
            }
            else
            {
                LogMessage("Select gui element", 0);
            }
            #endregion
            #region SaveAddressLogic
            if (CurSelectedChildElement != null)
            {
                Graphics.DrawFrame(CurSelectedChildElement.GetClientRect(), 2, Color.Green);
                if (Settings.Save.PressedOnce())
                    File.WriteAllText(Path.Combine(PluginDirectory, "ChildAddr.txt"), CurSelectedChildElement.Address.ToString("x"));
            }
            #endregion
        }
    }
}