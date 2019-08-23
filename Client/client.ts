/// <reference path="typings/altv-client.d.ts"/>
/// <reference path="typings/natives.d.ts"/>

import * as alt from 'alt';
import * as game from 'natives';

import menuManager from 'scripts/MenuManager';

const init = async () => {
    try {
        let count = 0;
		
        await menuManager();
        count++;

        alt.log(`All ${count} scripts loaded successfully.`);
    }
    catch (Exception)
    {
        alt.logError(`Failed to load scripts.\nMessage: ${Exception.Message}`);
        game.freezeEntityPosition(alt.Player.local.scriptID, true);
        game.doScreenFadeOut(0);
        alt.logError(`Blocking spawning! Try to reconnect, if the problem happens again please contact the serverowner.`);
    }
};
init();