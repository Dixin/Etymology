import "spin.js";
import Ladda from "ladda";

const global = window;
const $ = global.$;

const ui = {
    init: () => {
        ui.$nav = $("#etymologyNav");
        ui.$navMenu = $("#etymologyNavMenu");
        ui.$positions = $("div.etymology-nav");
        ui.$links = $("a.etymology-link");
        ui.buttons = $("#etymologySearchButton, #etymologyNavSearchButton, #etymologyRandomButton").map((index, element) => Ladda.create(element));
        ui.$error = $("#etymologyError");
        ui.$errorMessage = ui.$error.find("#etymologyErrorMessage");
        ui.$result = $("#etymologyResult");
        ui.$search = $("#etymologySearch, #etymologyNavSearch");
        ui.$random = $("#etymologyRandomButton");
        ui.$videos = $("#videos .media-left a");
        ui.$chinese = $("#etymologySearchChar, #etymologyNavSearchChar");
    },

    $nav: null,
    $navMenu: null,
    $positions: null,
    $links: null,
    buttons: null,
    $error: null,
    $errorMessage: null,
    $result: null,
    $search: null,
    $random: null,
    $videos: null,
    $chinese: null,

    startLoading: () => $.each(ui.buttons, (index, button) => button.start()),

    stopLoading: () => $.each(ui.buttons, (index, button) => button.stop()),

    showResult: data => {
        ui.$error.hide();
        ui.$result.html(data).show();
    },

    showError: (status, responseText, textStatus, error) => {
        ui.$result.hide();
        const message = !status ? "Network connection failed." : `[${status} ${textStatus}] ${error}. ${responseText}`;
        ui.$errorMessage.text(message);
        ui.$error.show();
    },

    loadVideoThumbnails: () => {
        ui.$videos.each((index, element) => {
            const $link = $(element);
            $link.css({ "background-image": `url(${$link.data("background")})` });
        });
    },

    getChinese: search => search["chinese"].value,

    getNavOffset: () => ui.$nav.height() - 1,

    collapseMenu: () => {
        if (ui.$navMenu.is(":visible")) {
            ui.$navMenu.collapse('hide');
        }
    }
};

export default ui;
