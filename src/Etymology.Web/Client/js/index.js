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

    Hash.init(UI.$nav.height() - 1, UI.$positions);

    UI.$links.on("click", event => Hash.scrollTo(event.currentTarget.hash));

    UI.$search.on("submit", event => {
        event.preventDefault();
        const chinese = event.currentTarget["chinese"].value;
        if (Data.isChinese(chinese)) {
            if (!Hash.scrollTo(chinese)) {
                Hash.set(chinese);
            }
        } else {
            global.alert("Please input single Chinese character 请输入单个汉字.");
        }
    });

    UI.$random.on("click", () => Hash.set(Data.randomChinese()));

    trySearchFromHash();
});

$global.on("hashchange", () => trySearchFromHash());

$global.on("scroll", Hash.setAfterScroll);

$global.on("load", () => UI.loadVideoThumbnails());

import "ie10-viewport-bug-workaround.js";

import "./baidu";
import "./google";
