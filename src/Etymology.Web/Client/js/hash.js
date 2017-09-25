const global = window;
const location = global.location;
const $document = $(global.document);

let navOffset = 0;
let positions = null;
let oldTop = 0;
let scrollTimeoutId = null;

const updatePositions = $elements => positions = $.map($elements, element => {
    const $element = $(element);
    const position = $element.position();
    return {
        top: position.top - navOffset,
        bottom: position.top - navOffset + $element.height(),
        id: element.id
    };
});

const set = hash => {
    if (hash.indexOf("#") === 0) { // IE does not support starsWith.
        hash = hash.substring(1);
    }
    const $hash = $(`#${hash}`);
    if ($hash.length > 0) {
        $hash.removeAttr("id"); // Prevent jumping to the target element.
        location.hash = hash;
        $hash.prop("id", hash);
    } else {
        location.hash = hash;
    }
};

const get = () => global.decodeURIComponent(location.hash.substring(1));

export default {
    init: (value, $elements) => {
        navOffset = value;
        updatePositions($elements);
        oldTop = $document.scrollTop();
    },

    get: get,

    set: set,

    scrollTo: hash => {
        if (hash.indexOf("#") === 0) { // IE does not support starsWith.
            hash = hash.substring(1);
        }
        const $hash = $(`#${hash}`);
        if ($hash.length > 0) {
            $("html, body").animate({
                scrollTop: $hash.offset().top - navOffset
            }, 800);
            return true;
        }
        return false;
    },

    updatePositions: updatePositions,

    setAfterScroll: callback => {
        if (scrollTimeoutId) {
            global.clearTimeout(scrollTimeoutId);
        }
        scrollTimeoutId = global.setTimeout(() => {
            const newTop = $document.scrollTop();
            if (newTop !== oldTop) {
                oldTop = newTop;
                $.each(positions, (index, position) => {
                    if (position.top >= newTop || position.bottom <= newTop) {
                        return true; // Not current.
                    }
                    if (get() === position.id) {
                        return false;
                    }
                    set(position.id);
                    return false;
                });
            }
            if (callback) {
                callback();
            }
        }, 200);
    }
};
