using System.Collections.Generic;
using System.Drawing;
using AltV.Net.Elements.Entities;
using Newtonsoft.Json;

namespace MenuManagement
{
    class Menu
    {
        #region Public enums
        //public enum MenuAnchor
        //{
        //    TopLeft,
        //    TopMiddle,
        //    TopRight,
        //    MiddleLeft,
        //    Middle,
        //    TopLeftFull,
        //    MiddleRight,
        //    BottomLeft,
        //    BottomMiddle,
        //    BottomRight
        //}
        #endregion

        #region Public delegates
        public delegate void MenuCallback(IPlayer client, Menu menu, MenuItem menuItem, int itemIndex, bool forced, dynamic data);
        public delegate void MenuFinalizer(IPlayer client, Menu menu);
        #endregion

        #region Private fields
        private Dictionary<string, object> _data;
        private List<MenuItem> _items;
        private int _selectedIndex;
        private MenuItem _selectedItem;
        #endregion

        #region Public properties
        public string Id { get; set; }
        public Banner BannerSprite { get; set; }
        public Color? BannerColor { get; set; }
        public string BannerTexture { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        // public MenuAnchor Anchor { get; set; }
        public bool NoExit { get; set; }
        public bool EnableBanner { get; set; }
        public List<MenuItem> Items
        {
            get { return _items; }
        }
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value >= _items.Count)
                    return;

                _selectedIndex = value;
                _selectedItem = _items[value];
            }
        }
        public string OnCheckboxChange { get; set; }
        public string OnIndexChange { get; set; }
        public string OnItemSelect { get; set; }
        public string OnListChange { get; set; }
        public string OnMenuClose { get; set; }
        public string OnMenuOpen { get; set; }
        #endregion

        #region Public Json ignored properties
        [JsonIgnore]
        public MenuItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (!_items.Contains(value))
                    return;

                _selectedItem = value;
                _selectedIndex = _items.IndexOf(value);
            }
        }
        [JsonIgnore]
        public MenuCallback Callback { get; set; }
        [JsonIgnore]
        public MenuFinalizer Finalizer { get; set; }
        [JsonIgnore]
        public Menu LastMenu { get; set; }
        #endregion

        #region Constructor
        public Menu(string id, string title, string subTitle, int posX, int posY, bool noExit = false, bool enableBanner = true)
        {
            _selectedIndex = -1;
            _selectedItem = null;

            if (id == null && id.Trim().Length == 0)
                Id = null;
            else
                Id = id;

            BannerSprite = null;
            BannerColor = null;
            BannerTexture = null;

            if (title != null && title.Trim().Length == 0)
                Title = null;
            else
                Title = title;

            if (subTitle != null && subTitle.Trim().Length > 0)
                SubTitle = subTitle;

            PosX = posX;
            PosY = posY;
            // Anchor = anchor;
            NoExit = noExit;
            EnableBanner = enableBanner;
            _data = new Dictionary<string, object>();
            _items = new List<MenuItem>();
            OnCheckboxChange = null;
            OnIndexChange = null;
            OnItemSelect = null;
            OnListChange = null;
            OnMenuClose = null;
            OnMenuOpen = null;
            Callback = null;
            Finalizer = null;
            LastMenu = null;
        }
        #endregion

        #region Public operators
        public MenuItem this[string id]
        {
            get
            {
                foreach (MenuItem menuItem in _items)
                {
                    if (menuItem.Id == id)
                        return menuItem;
                }

                return null;
            }
        }
        #endregion

        #region Public methods
        public void Add(MenuItem menuItem)
        {
            _items.Add(menuItem);
        }

        public void ClearItems()
        {
            _items.Clear();
        }

        public bool Contains(string id)
        {
            foreach (MenuItem menuItem in _items)
            {
                if (menuItem.Id == id)
                    return true;
            }

            return false;
        }

        public bool Contains(MenuItem menuItem)
        {
            return _items.Contains(menuItem);
        }

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

        public void Insert(int index, MenuItem menuItem)
        {
            _items.Insert(index, menuItem);

            if (index <= _selectedIndex)
                _selectedIndex++;
        }

        public bool Remove(string id)
        {
            foreach (MenuItem menuItem in _items)
            {
                if (menuItem.Id == id)
                    return Remove(menuItem);
            }

            return false;
        }

        public bool Remove(MenuItem menuItem)
        {
            int pos = _items.IndexOf(menuItem);
            bool result = _items.Remove(menuItem);

            if (pos == -1 || !result)
                return false;

            if (_items.Count == 0)
            {
                _selectedIndex = -1;
                _selectedItem = null;
            }
            else if (menuItem == _selectedItem)
            {
                if (_selectedIndex == _items.Count)
                    _selectedIndex--;

                _selectedItem = _items[_selectedIndex];
            }
            else if (pos < _selectedIndex)
                _selectedIndex--;

            return true;
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);

            if (_items.Count == 0)
            {
                _selectedIndex = -1;
                _selectedItem = null;
            }
            else if (index == _selectedIndex)
            {
                if (_selectedIndex == _items.Count)
                    _selectedIndex--;

                _selectedItem = _items[_selectedIndex];
            }
            else if (index < _selectedIndex)
                _selectedIndex--;
        }

        public void Reset()
        {
            _items.Clear();
            _data.Clear();
        }

        public void resetData(string key)
        {
            _data.Remove(key);
        }

        public void setData(string key, object value)
        {
            _data[key] = value;
        }
        #endregion
    }
}
