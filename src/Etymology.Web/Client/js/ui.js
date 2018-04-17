import "spin.js";
import Ladda from "ladda";

const global = window;
const $ = global.$;

const ui = {
    init: () => {
        ui.$main = $("#etymologyMain");
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
        ui.$characters = () => $("#etymologyCharacters button");
        ui.$modal = $("#etymologyModal");
    },

    startLoading: () => $.each(ui.buttons, (index, button) => button.start()),

    stopLoading: () => $.each(ui.buttons, (index, button) => button.stop()),

    showResult: data => {
        ui.$error.hide();
        ui.$result.html(data).show();
        ui.initializeCharacters();
    },

    initializeCharacters: () => {
        ui.$modal.on('show.bs.modal', event => {
            const id = $.trim($(event.relatedTarget).text()); // Button that triggered the modal
            ui.$modal.find(".modal-title").text(id);
            ui.$modal.find('.modal-content').prop("id", id);
        });
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
        if (ui.$navMenu.hasClass("in")) {
            ui.$navMenu.collapse('hide');
        }
    },

    showInvalidInput: () => global.alert("Please input single Chinese character 请输入单个汉字.")
};

export default ui;
