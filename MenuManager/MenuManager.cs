using System.Collections.Concurrent;
using Newtonsoft.Json;
using System;
using AltV.Net.Elements.Entities;
using System.Threading.Tasks;
using AltV.Net.Async;
using AltV.Net;

namespace MenuManagement
{
    class MenuManager
    {
        #region Private static properties
        private static ConcurrentDictionary<IPlayer, Menu> _clientMenus = new ConcurrentDictionary<IPlayer, Menu>();
        #endregion

        #region Constructor
        public MenuManager()
        {
        }
        #endregion

        #region Private API triggers
        public static void OnPlayerDisconnect(IPlayer origin, string reason)
        {
            Menu menu;
            _clientMenus.TryRemove(origin, out menu);
        }

        public static void OnPlayerEvent(IPlayer sender, string eventName, object[] args)
        {          
            switch (eventName)
            {
                case "MENUMANAGER_ClosedMenu":
                    {
                        try
                        {
                            Menu menu = null;
                            _clientMenus.TryGetValue(sender, out menu);

                            //if (menu != null && !menu.BackCloseMenu)
                            //    menu.Callback(sender, menu, null, -1, false, null);
                            //else if (menu != null)
                                _clientMenus.TryRemove(sender, out menu);
                        }
                        catch (Exception ex)
                        {
                            string id = args == null ? "null" : args.Length >= 1 ? (string)args[0] : "null";
                            Alt.Log(ex.ToString() + "\nERZEUGT DURCH: " + sender.Name + " AN STELLE " + sender.Position + " MenuID: " + id );
                        }
                        break;
                    }
                case "MENUMANAGER_ExecuteCallback":
                    {
                        try
                        {
                            Menu menu = null;
                            _clientMenus.TryGetValue(sender, out menu);

                            if (menu != null && menu.Callback != null)
                            {
                                if (args.Length < 5) return;

                                string menuId = (string)args[0];
                                string itemId = (string)args[1];
                                int itemIndex = Convert.ToInt32(args[2]);
                                bool forced = (bool)args[3];

                                dynamic data = JsonConvert.DeserializeObject((string)args[4]);

                                foreach (MenuItem menuItem in menu.Items)
                                {
                                    if (menuItem.Type == MenuItemType.CheckboxItem && menuItem is CheckboxItem)
                                        ((CheckboxItem)menuItem).Checked = data[menuItem.Id];
                                    else if (menuItem.Type == MenuItemType.ListItem && menuItem is ListItem)
                                        ((ListItem)menuItem).SelectedItem = data[menuItem.Id]["Index"];
                                    else if (menuItem.InputMaxLength > 0)
                                        menuItem.InputValue = data[menuItem.Id];
                                }

                                menu.Callback(sender, menu, menu.Items[itemIndex], itemIndex, forced, data);
                            }
                        }
                        catch (Exception ex)
                        {
                            Alt.Log(ex.ToString() + "\nERZEUGT DURCH: " + sender.Name + " AN STELLE " + sender.Position + " MenuID: " + (string)args[0]);
                        }
                        break;
                    }
            }
        }
        #endregion

        #region Public static methods
        public static bool CloseMenu(IPlayer client)
        {
            Menu menu;

            if (_clientMenus.TryRemove(client, out menu))
            {
                menu.Finalizer?.Invoke(client, menu);
                client.Emit("MENUMANAGER_CloseMenu");
                return true;
            }

            return false;
        }

        public static void ForceCallback(IPlayer client)
        {
            Menu menu = null;
            _clientMenus.TryGetValue(client, out menu);

            if (menu == null || menu.Callback == null)
                return;

            client.Emit("MENUMANAGER_ForceCallback");
        }

        public static Menu GetMenu(IPlayer client)
        {
            Menu menu = null;
            _clientMenus.TryGetValue(client, out menu);

            return menu;
        }

        public static bool HasOpenMenu(IPlayer client)
        {
            return _clientMenus.ContainsKey(client);
        }

        public static void OpenMenu(IPlayer client, Menu menu)
        {
            Menu oldMenu = null;
            _clientMenus.TryRemove(client, out oldMenu);

            if (oldMenu != null)
            {
                oldMenu.Finalizer?.Invoke(client, menu);
                client.Emit("MENUMANAGER_CloseMenu");
            }

            _clientMenus.TryAdd(client, menu);
            string json = JsonConvert.SerializeObject(menu, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            client.Emit("MENUMANAGER_OpenMenu", json);
        }
        #endregion
    }
}
