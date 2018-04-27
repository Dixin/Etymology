import Cookies from "js-cookie";

const global = window;
const $ = global.$;
const Math = global.Math;

const commonChars = "安八白百敗般邦保寶北貝匕比妣畢賓兵丙秉并伯帛亳卜不步采倉曹曾長厂鬯車徹臣辰晨成承乘遲齒赤沖舂丑出初楚畜傳春祠此朿次大丹單旦稻得登帝典奠丁鼎冬東動斗豆斷隊對多而兒耳二伐反匚方妃分封豐酆逢缶夫弗服福甫斧簠黼父复婦復干剛高羔告戈格鬲庚工弓公攻古谷鼓官觀盥光龜歸鬼癸國果亥行蒿好禾合何河侯后厚乎呼壺虎化畫淮黃惠會或獲鑊基箕及吉即亟疾耤己季既祭家甲斝監見姜彊降角教解巾斤今進盡京兢井競九酒舊沮句絕爵君畯考可克客口寇來牢老醴立利麗良林陵令柳六龍聾盧魯陸鹿麓洛旅率馬買麥卯枚眉每媚門蒙盟皿敏名明鳴命莫母牡木目牧乃男南內逆年廿鳥宁寧牛農奴諾女旁轡彭辟品僕圃七戚祈齊杞啟气千前遣羌且妾侵秦卿丘區曲取去犬人壬日戎如入若卅三散嗇山商上少紹射涉申沈生省尸師十石食時史矢豕使始士氏事室視首受獸叔黍蜀戍束水司絲死巳四祀宋綏歲孫它唐天田聽同土退屯豚鼉外亡王韋唯未位畏衛文聞問我吳五午武舞勿戊夕兮昔析奚襲徙喜系下先咸獻相祥饗向象小效辛新星興兄休羞宿戌須徐宣旋學旬亞言炎衍甗央羊昜陽揚夭爻頁一伊衣依匜夷宜疑彝乙亦邑易異肄義因禋寅尹郢永用攸幽猶游友有酉又幼于余盂雩魚虞漁羽雨圉玉聿育昱御元爰員曰月戉樂龠允宰載再在葬昃增乍宅旃召折貞朕征正鄭之姪執止旨黹至豸彘中終仲舟州周帚朱逐祝貯追茲子自宗足卒族俎祖尊作";
const commonCharsLength = commonChars.length;

/*! https://mths.be/codepointat v0.2.0 by @mathias */
if (!String.prototype.codePointAt) {
    (function () {
        'use strict'; // needed to support `apply`/`call` with `undefined`/`null`
        var defineProperty = (function () {
            // IE 8 only supports `Object.defineProperty` on DOM elements
            try {
                var object = {};
                var $defineProperty = Object.defineProperty;
                var result = $defineProperty(object, object, object) && $defineProperty;
            } catch (error) { }
            return result;
        }());
        var codePointAt = function (position) {
            if (this == null) {
                throw TypeError();
            }
            var string = String(this);
            var size = string.length;
            // `ToInteger`
            var index = position ? Number(position) : 0;
            if (index != index) { // better `isNaN`
                index = 0;
            }
            // Account for out-of-bounds indices:
            if (index < 0 || index >= size) {
                return undefined;
            }
            // Get the first code unit
            var first = string.charCodeAt(index);
            var second;
            if ( // check if it’s the start of a surrogate pair
                first >= 0xD800 && first <= 0xDBFF && // high surrogate
                size > index + 1 // there is a next code unit
            ) {
                second = string.charCodeAt(index + 1);
                if (second >= 0xDC00 && second <= 0xDFFF) { // low surrogate
                    // https://mathiasbynens.be/notes/javascript-encoding#surrogate-formulae
                    return (first - 0xD800) * 0x400 + second - 0xDC00 + 0x10000;
                }
            }
            return first;
        };
        if (defineProperty) {
            defineProperty(String.prototype, 'codePointAt', {
                'value': codePointAt,
                'configurable': true,
                'writable': true
            });
        } else {
            String.prototype.codePointAt = codePointAt;
        }
    }());
}

export default {
    search: chinese => {
        const token = Cookies.get("Bronze");
        return $.ajax({
            type: "POST",
            data: { chinese, Bronze: token },
            dataType: "html",
            url: "etymology",
            headers: {
                Seal: token
            }
        });
    },

    randomChinese: () => {
        const index = Math.floor(Math.random() * commonCharsLength + 1);
        return commonChars[index];
    },

    isChinese: value => {
        if (!value) {
            return false;
        }
        const length = value.length;
        if (length < 1 || length > 2) {
            return false;
        }
        const code = value.codePointAt(0);
        return code >= 0x2E80 && code <= 0x2FA1F;
    }
};
