import Data from "./data";
import UI from "./ui";
import Hash from "./hash";

const global = window;
const $ = global.$;
const $global = $(global);
const $document = $(global.document);

const trySearchFromHash = () => {
    const chinese = Hash.get();
    if (Data.isChinese(chinese) && $(`#${chinese}`).length === 0) {
        UI.$chinese.val(chinese);
        UI.startLoading();

        Data.search(chinese).done(data => {
            UI.showResult(data);

            Hash.updatePositions(UI.$positions);
            Hash.scrollTo(chinese);
        }).fail((jqXHR, textStatus, error) => {
            UI.showError(jqXHR.status, jqXHR.responseText, textStatus, error);

            Hash.updatePositions(UI.$positions);
            Hash.scrollTo(UI.$error.prop("id"));
        }).always(() => UI.stopLoading());
    }
};

$document.on("ready", () => {
    UI.init();

    Hash.init(UI.getNavOffset(), UI.$positions);

    UI.$links.on("click", event => {
        event.preventDefault();
        Hash.scrollTo(event.currentTarget.hash);
        UI.collapseMenu();
    });

    UI.$search.on("submit", event => {
        event.preventDefault();
        const chinese = UI.getChinese(event.currentTarget);
        if (Data.isChinese(chinese)) {
            UI.collapseMenu();
            if (!Hash.scrollTo(chinese)) {
                Hash.set(chinese);
            }
        } else {
            UI.showInvalidInput();
        }
    });

    UI.$random.on("click", () => Hash.set(Data.randomChinese()));

    UI.$main.on("click", () => UI.collapseMenu());

    trySearchFromHash();
});

$global.on("hashchange", () => trySearchFromHash());

$global.on("scroll", () => Hash.setAfterScroll(UI.collapseMenu));

$global.on("load", () => UI.loadVideoThumbnails());

import "ie10-viewport-bug-workaround.js";

import "./baidu";
import "./google";
