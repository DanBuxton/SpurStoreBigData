using System.Collections.Generic;

namespace SpurStoreBigData.CommandLine
{
    public abstract class AbstractMenu
    {
        protected Dictionary<int, string> MenuItems { get; set; } = new Dictionary<int, string>();

        protected internal void Show() => Menu();

        protected abstract void Menu();

        protected abstract void MenuSelection();
        protected abstract int MenuResponse();
    }
}