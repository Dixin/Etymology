import "ie10-viewport-bug-workaround.js";
import Spin from "spin.js";
import Ladda from "ladda";
import Cookies from "js-cookie";

const global = window;
const $global = $(global);
const location = global.location;
const $document = $(global.document);

let navOffset = 0;

const animateToHash = hash => {
    const $hash = $(hash.indexOf("#") === 0 ? hash : `#${hash}`); // IE does not support starsWith.
    if ($hash.length > 0) {
        $('html, body').animate({
            scrollTop: $hash.offset().top - navOffset
        }, 800);
    }
};

const setHash = hash => {
    if (hash.indexOf("#") === 0) { // IE does not support starsWith.
        hash = hash.substring(1);
    }
    const $hash = $(`#${hash}`);
    if ($hash.length > 0) {
        $hash.removeAttr("id");
        location.hash = hash;
        $hash.prop("id", hash);
    } else {
        location.hash = hash;
    }
};

const commonChars = "安八白百败般邦保宝北贝比毕辟宾兵丙秉伯帛卜不步采仓曹长厂车彻臣辰晨成乘承迟齿赤冲丑初出楚传春此次大丹单旦稻得登邓帝典奠丁鼎东冬动斗豆断队对多而儿耳二伐反方分封逢夫服福弗甫斧复父妇干刚高羔告戈格庚工攻公弓鼓古谷官观光归龟鬼癸国果亥好禾何河侯厚后呼乎壶虎化淮黄惠会获或基箕吉及疾即己季祭既家甲监见姜降角教解巾斤今进兢京井竞九酒旧沮句爵君考可克客口寇来牢老乐丽利立良林陵令柳六聋卢鲁麓鹿陆旅率洛马买麦卯枚眉每媚门蒙盟皿敏明鸣名命莫牡母木目牧乃南男内逆年鸟宁牛农奴女诺旁彭品仆圃戚七齐祈启气千前遣羌强且侵秦卿丘区取去犬壬人日戎如入若三散山上少绍射涉申生省师尸十石时食史矢使始士事氏室视首受叔蜀黍束戍水司丝死四巳宋宿绥岁孙它唐天田同徒土退屯外王亡韦唯未畏位卫文闻问我吴武五午舞戊勿昔析夕袭喜下先相祥向象小效辛新兴行兄休羞戌徐畜宣旋学旬亚言炎衍央扬羊阳页一依伊衣夷疑宜彝乙易邑肄义因寅尹永用幽酉有友又幼于盂虞余鱼渔雨羽玉御育元员曰月允宰载再在葬增曾乍宅召折贞征正郑之执止旨至中终仲舟周州帚朱逐贮祝追兹子自宗足卒族祖尊作";
const commonCharsLength = commonChars.length;

$document.on("ready", () => {
    navOffset = $("nav").height() - 1;

    $("a.etymology-link").on('click', event => {
        animateToHash(event.currentTarget.hash);
    });

    const buttons = $("#etymologySearchButton, #etymologyNavSearchButton, #etymologyRandomButton").map((index, element) => Ladda.create(element));
    const $error = $("#etymologyError");
    const $result = $("#etymologyResult");
    const search = chinese => {
        $("#etymologySearchChar, #etymologyNavSearchChar").val(chinese);

        $.each(buttons, (index, button) => {
            button.start();
        });
        const token = Cookies.get("Bronze");
        $.ajax({
            type: "POST",
            data: { chinese, Bronze: token },
            dataType: "html",
            url: "etymology",
            headers: {
                Seal: token
            }
        }).done(data => {
            $error.hide();
            $result.html(data).show();

            savePositions();
            animateToHash(chinese, true);
        }).fail((jqXHR, textStatus, error) => {
            $result.hide();
            const message = !jqXHR.status || !jqXHR.responseText ? "Network connection failed." : `[${jqXHR.status} ${textStatus}] ${error}. ${jqXHR.responseText}`;
            $error.find("#etymologyErrorMessage").text(message).end().show();

            savePositions();
            animateToHash("#etymologyError");
        }).always(() => {
            $.each(buttons, (index, button) => {
                button.stop();
            });
        });
    };

    $("#etymologySearch, #etymologyNavSearch").on("submit", event => {
        event.preventDefault();
        let chinese = "";
        if (event.currentTarget.id === "etymologySearch") {
            chinese = $("#etymologySearchChar").val();
        }
        if (event.currentTarget.id === "etymologyNavSearch") {
            chinese = $("#etymologyNavSearchChar").val();
        }
        if ($(`#${chinese}`).length > 0) {
            animateToHash(chinese);
        } else {
            location.hash = chinese;
        }
    });

    $("#etymologyRandomButton").on("click", () => {
        const index = Math.floor(Math.random() * commonCharsLength + 1);
        location.hash = commonChars[index];
    });

    const isChinese = value => /[\u3400-\u9FBF]/.test(value);
    const searchFromHash = () => {
        const chineseFromHash = location.hash.substring(1);
        if (isChinese(chineseFromHash) && $(`#${chineseFromHash}`).length === 0) {
            search(chineseFromHash);
        } else {
            const decoded = global.decodeURIComponent(chineseFromHash);
            if (isChinese(decoded) && $(`#${decoded}`).length === 0) {
                search(decoded);
            }
        }
    };

    let oldHash = location.hash.substring(1);
    $global.on('hashchange', () => {
        const newHash = location.hash.substring(1);
        if (oldHash !== newHash) {
            searchFromHash();
            oldHash = newHash;
        }
    });

    savePositions();
    searchFromHash();
});

$global.on("load", () => {
    $("#videos .media-left a").each((index, element) => {
        const $link = $(element);
        $link.css({ "background-image": `url(${$link.data("background")})` });
    });
});

let positions = null;
var savePositions = () => {
    positions = $.map($(".etymology-nav"), element => {
        const $element = $(element);
        const position = $element.position();
        return {
            top: position.top - 100,
            bottom: position.top - 100 + $element.height(),
            hash: element.id
        };
    });
};

let top = null;
let currentHash = null;
let scrollTimeoutId = null;
$global.on("scroll", () => {
    if (scrollTimeoutId) {
        global.clearTimeout(scrollTimeoutId);
    }
    scrollTimeoutId = global.setTimeout(() => {
        const newTop = $document.scrollTop();
        if (newTop !== top) {
            top = newTop;
            $.each(positions, (index, position) => {
                if (position.top >= top || position.bottom <= top) {
                    return true;
                }
                if (currentHash === position.hash) {
                    return false;
                }
                currentHash = position.hash;
                setHash(currentHash);
            });
        }
        return true;
    }, 300);
});

import "./baidu";
require("./google");
