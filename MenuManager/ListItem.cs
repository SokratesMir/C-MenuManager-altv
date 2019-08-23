using System.Collections.Generic;

namespace MenuManagement
{
    class ListItem : MenuItem
    {
        #region Public properties
        public IReadOnlyCollection<string> Items { get; }
        public int SelectedItem { get; set; }
        #endregion

        #region Constructor
        public ListItem(string text, string description, string id, IReadOnlyCollection<string> items, int selectedItem) : base(text, description, id)
        {
            Type = MenuItemType.ListItem;
            Items = items;
            SelectedItem = selectedItem;
        }
        #endregion

        #region Public overrided methods
        public override bool IsInput()
        {
            return false;
        }
        #endregion
    }
}
