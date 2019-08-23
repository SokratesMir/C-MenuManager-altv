﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using MenuManagement;

namespace MenuManagement
{
    internal class Main
    {
        private MenuManager MenuManager = new MenuManager();
        private Menu savedMenu = null;

        private void TestMenuBuilder(Client client)
        {
						Menu menu = savedMenu;

                        if (menu == null)
                        {
                            // Create the menu
                            menu = new Menu("TestMenu", "Test Menu", "This is a subtitle", 0, 0);

                            // Set menu callback function that will be executed when a menu item with ExecuteCallback property is selected
                            menu.Callback = TestMenuManager;

                            // Add a simple MenuItem
                            MenuItem menuItem = new MenuItem("Simple MenuItem");
                            menu.Add(menuItem);

                            // Add a MenuItem with commentary
                            menuItem = new MenuItem("MenuItem with commentary", "This is the commentary");
                            menu.Add(menuItem);

                            // Add a MenuItem with left badge
                            menuItem = new MenuItem("MenuItem with Left badge");
                            menuItem.LeftBadge = BadgeStyle.Star;
                            menu.Add(menuItem);

                            // Add an selectable MenuItem which opens a sub-menu and display a right badge
                            menuItem = new MenuItem("Open submenu");
                            menuItem.ExecuteCallback = true;
                            menuItem.RightBadge = BadgeStyle.Trevor;
                            menu.Add(menuItem);

                            // Add an selectable MenuItem which opens a sub-menu
                            menuItem = new MenuItem("Shop menu", "Open a shop menu with advanced features");
                            menuItem.ExecuteCallback = true;
                            menu.Add(menuItem);

                            // Add a MenuItem with a right label
                            menuItem = new MenuItem("MenuItem with right label");
                            menuItem.RightLabel = "BlaBlaBla";
                            menu.Add(menuItem);

                            // Add a ListItem with 3 items and a commentary
                            List<string> values = new List<string>() { "Item 1", "Item 2", "Item 3" };
                            menuItem = new ListItem("A ListItem control", "Select the item you want", "List", values, 0);
                            menu.Add(menuItem);

                            // Add a CheckboxItem with checkbox unselected by default
                            menuItem = new CheckboxItem("This is a CheckboxItem", "", "Checkbox", false);
                            menu.Add(menuItem);

                            // Add a MenuItem which will ask user to input text when selected. Default text is set to "My default text"
                            menuItem = new MenuItem("MenuItem with input", "Text input", "Input");
                            menuItem.SetInput("My Default text", 30, InputType.Text);
                            menu.Add(menuItem);

                            // Add a coloredItem with right label which will act as a "Submit" button
                            // When submitted all input field will transmited to server
                            ColoredItem coloredItem = new ColoredItem("Submit data", "Submit all data of editable MenuItems", "Submit", "#FF0000", "#0000FF");
                            coloredItem.RightLabel = "Also with right label";
                            coloredItem.ExecuteCallback = true;
                            menu.Add(coloredItem);
                        }

                        // Open the menu client side
                        MenuManager.OpenMenu(player, menu);
        }

        private void TestMenuManager(Client client, Menu menu, MenuItem menuItem, int itemIndex, bool forced, dynamic data)
        {
			Alt.log(menuItem.Text);
        }

    }
}
