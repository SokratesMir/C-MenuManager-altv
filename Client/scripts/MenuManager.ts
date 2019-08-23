/// <reference path="../typings/altv-client.d.ts"/>
/// <reference path="../typings/natives.d.ts"/>

import * as alt from 'alt';
import * as NativeUI from 'NativeUIMenu/NativeUI.js';
import * as game from 'natives';

var menu = null;
var menuItems = null;
var menuData = null;
var backPressed = false;
var exitEscapeMenu = false;

var inputView = null;
var inputItem = null;
var inputIndex = -1;

export default async () => {
    alt.onServer("MENUMANAGER_OpenMenu", (data) => {
        menuItems = new Array();
        menuData = JSON.parse(data);

        if (menuData.Id === undefined) {
            menuData.Id = "";
        }

        if (menuData.Title === undefined || menuData.Title === "") {
            menuData.Title = " ";
        }

        if (menuData.SubTitle === undefined || menuData.SubTitle === "") {
            menuData.SubTitle = " ";
        }

        if (menuData.BannerSprite !== undefined) {
            menu = new NativeUI.Menu(menuData.Title, menuData.SubTitle, new NativeUI.Point(menuData.PosX, menuData.PosY), menuData.BannerSprite.Dict, menuData.BannerSprite.Name);
        } else {
            menu = new NativeUI.Menu(menuData.Title, menuData.SubTitle, new NativeUI.Point(menuData.PosX, menuData.PosY));
        }

        for (let i = 0; i < menuData.Items.length; i++) {
            let item = menuData.Items[i];
            let menuItem;

            if (item.Id === undefined) {
                item.Id = "";
            }

            if (item.Text === undefined) {
                item.Text = "";
            }

            if (item.Description === undefined) {
                item.Description = "";
            }

            if (item.Type === 0 || item.Type === 2) {

                if (item.Type === 2) {
                    let background = hexToRgb(item.BackgroundColor);
                    let hightlight = hexToRgb(item.HighlightColor);

                    menuItem = new NativeUI.UIMenuItem(item.Text, item.Description, new NativeUI.Color(background.r, background.g, background.b), new NativeUI.Color(hightlight.r, hightlight.g, hightlight.b));
                } else {
                    menuItem = new NativeUI.UIMenuItem(item.Text, item.Description);
                }

                if (item.RightBadge !== undefined) {
                    menuItem.SetRightBadge(eval(item.RightBadge));
                }

                if (item.RightLabel !== undefined) {
                    menuItem.SetRightLabel(item.RightLabel);
                }
            }
            else if (item.Type === 1) {
                menuItem = new NativeUI.UIMenuCheckboxItem(item.Text, item.Checked, item.Description)
            }
            else if (item.Type === 3) {
                let listItems = [];

                for (let j = 0; j < item.Items.length; j++) {
                    listItems.push(item.Items[j]);
                }

                menuItem = new NativeUI.UIMenuListItem(item.Text, item.Description, new NativeUI.ItemsCollection(listItems), item.SelectedItem);
            }

            if (item.LeftBadge !== undefined) {
                menuItem.SetLeftBadge(eval(item.LeftBadge));
            }

            menu.AddItem(menuItem);
            menuItems[i] = menuItem;
        }

        if (menuData.NoExit) {
            menu.DeactivateBack();
        }

        menu.IndexChange.on((index) => {
            if (menuData.OnIndexChange !== undefined) {
                eval(menuData.OnIndexChange);
            }
        });

        menu.CheckboxChange.on((item, checked) => {
            let index = getIndexOfMenuItem(item);
            let menuItem = menuData.Items[index];
            menuItem.Checked = checked;

            if (menuData.OnCheckboxChange !== undefined) {
                eval(menuData.OnCheckboxChange);
            }

            if (menuItem.ExecuteCallback) {
                let data = saveData();
                alt.emitServer("MENUMANAGER_ExecuteCallback", menuData.Id, menuItem.Id, index, false, JSON.stringify(data));
            }
        });

        menu.ListChange.on((item, index) => {
            menuData.Items[getIndexOfMenuItem(item)].SelectedItem = index;

            if (menuData.OnListChange !== undefined) {
                eval(menuData.OnListChange);
            }
        });

        menu.ItemSelect.on((item, index) => {
            if (inputView !== null)
                return;

            let menuItem = menuData.Items[index];

            if (menuItem.InputMaxLength > 0) {
                if (menuItem.InputValue === undefined) {
                    menuItem.InputValue = "";
                }

                inputIndex = index;
                inputItem = item;

                inputView = new alt.WebView("http://resources/client/webview/input.html");
                inputView.focus();
                alt.showCursor(true);
                alt.toggleGameControls(false);

                inputView.emit('INPUT_Data', menuItem.InputMaxLength, menuItem.InputValue);

                inputView.on('INPUT_Submit', (text) => {
                    saveInput(text);
                });
            }

            if (inputView == null && menuData.OnItemSelect !== undefined) {
                eval(menuData.OnItemSelect);
            }

            if (inputView == null && menuItem.ExecuteCallback) {
                let data = saveData();
                alt.emitServer("MENUMANAGER_ExecuteCallback", menuData.Id, menuItem.Id, index, false, JSON.stringify(data));
            }
        });

        menu.MenuClose.on(() => {
            if (backPressed === false) {
                return;
            }

            if (menuData.OnMenuClose !== undefined) {
                eval(menuData.OnMenuClose);
            }

            backPressed = false;
            alt.emitServer("MENUMANAGER_ClosedMenu");
        });

        if (menuData.SelectedIndex === -1)
            menuData.SelectedIndex = 0;

        menu.CurrentSelection = menuData.SelectedIndex;

        if (menuData.OnMenuOpen) {
            eval(menuData.OnMenuOpen);
        }

        menu.Open();
    });

    alt.onServer("MENUMANAGER_ForceCallback", () => {
        let data = saveData();
        alt.emitServer("MENUMANAGER_ExecuteCallback", menuData.Id, "", menu.CurrentSelection, true, JSON.stringify(data));
    });

    alt.onServer("MENUMANAGER_CloseMenu", () => {
        if (menu != null) {
            menu.Close();
            menu = null;
        }
    });
};

function saveInput(inputText) {
    if (inputItem !== null) {
        let menuItem = menuData.Items[inputIndex];
        let valid = true;

        if (menuItem.InputType === 1) {
            inputText = inputText.trim();

            if (inputText.length !== 0) {
                let con_inputText = parseInt(inputText);

                if (isNaN(con_inputText)) {
                    valid = false;
                } else {
                    inputText = inputText.toString();
                }
            }
        } else if (menuItem.InputType === 2) {
            inputText = inputText.trim();

            if (inputText.length !== 0) {
                let con_inputText = parseInt(inputText);

                if (isNaN(con_inputText) || con_inputText < 0) {
                    valid = false;
                } else {
                    inputText = inputText.toString();
                }
            }
        } else if (menuItem.InputType === 3) {
            inputText = inputText.trim();

            if (inputText.length !== 0) {
                let con_inputText = parseFloat(inputText);

                if (isNaN(con_inputText)) {
                    valid = false;
                } else {
                    inputText = inputText.toString();
                }
            }
        }

        if (valid) {
            menuItem.InputValue = inputText;

            if (menuItem.InputSetRightLabel) {
                inputItem.SetRightLabel(inputText);
            }
        }

        if (menuData.OnItemSelect !== undefined) {
            eval(menuData.OnItemSelect);
        }

        if (menuItem.ExecuteCallback) {
            let data = saveData();
            alt.emitServer("MENUMANAGER_ExecuteCallback", menuData.Id, menuItem.Id, inputIndex, false, JSON.stringify(data));
        }

        inputItem = null;
        inputIndex = -1;
        alt.showCursor(false);
        alt.toggleGameControls(true);

        inputView.destroy();
        alt.setTimeout(resetInputView, 1000);
    }
}

function resetInputView() {
    inputView = null;
}

function getIndexOfMenuItem(menuItem) {
    for (let i = 0; i < menuItems.length; i++) {
        if (menuItems[i] === menuItem) {
            return i;
        }
    }

    return -1;
}

function saveData() {
    var data = new Object();

    for (let i = 0; i < menuData.Items.length; i++) {
        var menuItem = menuData.Items[i];

        if (menuItem.Type === 1) {
            data[menuItem.Id] = menuItem.Checked;
        } else if (menuItem.Type === 3) {
            data[menuItem.Id] = new Object();
            data[menuItem.Id]["Index"] = menuItem.SelectedItem;
            data[menuItem.Id]["Value"] = menuItem.Items[menuItem.SelectedItem];
        } else if (menuItem.InputMaxLength > 0 && menuItem.InputValue !== undefined && menuItem.InputValue.length > 0) {
            data[menuItem.Id] = menuItem.InputValue;
        }
    }

    return data;
}

function getCharFromKey(key) {
    return String.fromCharCode((96 <= key && key <= 105) ? key - 48 : key);
}

function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

alt.on('keydown', (e) => {
    if (menu !== null && e == 8) {
        backPressed = true;
    }
    else if (menu !== null && e == 27) {
        exitEscapeMenu = true;
    }
});

alt.on('keyup', (e) => {
    if (menu !== null && exitEscapeMenu === true && e == 27) {
        exitEscapeMenu = false;
        menu.Visible = true;
        alt.log("Hallo");
    }
});

alt.setInterval(() => {
    if (menu == null || !menu.Visible)
        return;

    if (menu._justOpened) {
        if (menu._logo !== null && !menu._logo.IsTextureDictionaryLoaded)
            menu._logo.LoadTextureDictionary();
        if (!menu._background.IsTextureDictionaryLoaded)
            menu._background.LoadTextureDictionary();
        if (!menu._descriptionRectangle.IsTextureDictionaryLoaded)
            menu._descriptionRectangle.LoadTextureDictionary();
        if (!menu._upAndDownSprite.IsTextureDictionaryLoaded)
            menu._upAndDownSprite.LoadTextureDictionary();
    }
    menu._mainMenu.Draw();
    menu.ProcessMouse();
    menu.ProcessControl();
    menu._background.size =
        menu.MenuItems.length > menu.MaxItemsOnScreen + 1
            ? new NativeUI.Size(431 + menu.WidthOffset, 38 * (menu.MaxItemsOnScreen + 1))
            : new NativeUI.Size(431 + menu.WidthOffset, 38 * menu.MenuItems.length);
    menu._background.Draw();
    if (menu.MenuItems.length > 0) {
        menu.MenuItems[menu._activeItem % menu.MenuItems.length].Selected = true;
        if (menu.MenuItems[menu._activeItem % menu.MenuItems.length].Description.trim() !== "") {
            menu.RecalculateDescriptionPosition();
            let descCaption = menu.MenuItems[menu._activeItem % menu.MenuItems.length].Description;
            menu._descriptionText.caption = descCaption;
            const numLines = menu._descriptionText.caption.split("\n").length;
            menu._descriptionRectangle.size = new NativeUI.Size(431 + menu.WidthOffset, numLines * 25 + 15);
            menu._descriptionBar.Draw();
            menu._descriptionRectangle.Draw();
            menu._descriptionText.Draw();
        }
    }
    if (menu.MenuItems.length <= menu.MaxItemsOnScreen + 1) {
        let count = 0;
        for (const item of menu.MenuItems) {
            item.SetVerticalPosition(count * 38 - 37 + menu.extraOffset);
            item.Draw();
            count++;
        }
        if (menu._counterText && menu.counterOverride) {
            menu._counterText.caption = menu.counterPretext + menu.counterOverride;
            menu._counterText.Draw();
        }
    }
    else {
        let count = 0;
        for (let index = menu._minItem; index <= menu._maxItem; index++) {
            var item = menu.MenuItems[index];
            item.SetVerticalPosition(count * 38 - 37 + menu.extraOffset);
            item.Draw();
            count++;
        }
        menu._extraRectangleUp.size = new NativeUI.Size(431 + menu.WidthOffset, 18);
        menu._extraRectangleDown.size = new NativeUI.Size(431 + menu.WidthOffset, 18);
        menu._upAndDownSprite.pos = new NativeUI.Point(190 + menu.offset.X + menu.WidthOffset / 2, 147 +
            37 * (menu.MaxItemsOnScreen + 1) +
            menu.offset.Y -
            37 +
            menu.extraOffset);
        menu._extraRectangleUp.Draw();
        menu._extraRectangleDown.Draw();
        menu._upAndDownSprite.Draw();
        if (menu._counterText) {
            if (!menu.counterOverride) {
                const cap = menu.CurrentSelection + 1 + " / " + menu.MenuItems.length;
                menu._counterText.caption = menu.counterPretext + cap;
            }
            else {
                menu._counterText.caption =
                    menu.counterPretext + menu.counterOverride;
            }
            menu._counterText.Draw();
        }
    }
    menu._logo.Draw();    
}, 1);