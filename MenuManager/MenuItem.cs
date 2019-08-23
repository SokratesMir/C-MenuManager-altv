using System.Collections.Generic;

namespace MenuManagement
{
    class MenuItem
    {
        #region Private fields
        private Dictionary<string, object> _data;
        #endregion

        #region Public properties
        public MenuItemType Type { get; protected set; }
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string LeftBadge { get; set; }
        public string RightBadge { get; set; }
        public string RightLabel { get; set; }
        public bool ExecuteCallback { get; set; }
        public bool InputSetRightLabel { get; set; }
        public string InputValue { get; set; }
        public byte? InputMaxLength { get; set; }
        public InputType? InputType { get; set; }
        #endregion

        #region Constructor
        public MenuItem(string text, string description = null, string id = null)
        {
            Type = MenuItemType.MenuItem;

            if (text != null && text.Trim().Length == 0)
                Text = null;
            else
                Text = text;

            if (description != null && description.Trim().Length == 0)
                Description = null;
            else
                Description = description;

            if (id != null && id.Trim().Length == 0)
                Id = null;
            else
                Id = id;

            LeftBadge = null;
            RightBadge = null;
            RightLabel = null;
            ExecuteCallback = false;
            InputSetRightLabel = false;
            InputValue = null;
            InputMaxLength = null;
            InputType = null;
            _data = new Dictionary<string, object>();
        }
        #endregion

        #region Public virtual methods
        public virtual bool IsInput()
        {
            return InputMaxLength > 0;
        }
        #endregion

        #region Public methods
        public dynamic getData(string key)
        {
            if (!_data.ContainsKey(key))
                return null;

            return _data[key];
        }

        public bool hasData(string key)
        {
            return _data.ContainsKey(key);
        }

        public void resetData(string key)
        {
            _data.Remove(key);
        }

        public void setData(string key, object value)
        {
            _data[key] = value;
        }

        public MenuItem SetInput(string defaultText, byte maxLength, InputType inputType)
        {
            InputValue = defaultText;
            InputMaxLength = maxLength;
            InputType = inputType;

            return this;
        }
        #endregion
    }
}
