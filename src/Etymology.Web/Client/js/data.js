import Cookies from "js-cookie";

const global = window;
const $ = global.$;
const Math = global.Math;

const commonChars = "寶貝兵車徹鬯厂長曹倉采步不卜亳帛伯并秉丙楚賓畢妣比次匕保邦般敗百白臣辰晨曾成承乘遲齒赤沖舂對丑出初畜傳春祠此朿丹北逢單旦得八安帝典奠父鼎冬東動斗豆斷隊多而兒耳二伐弓反匚方分封大酆缶夫弗服稻登福斧簠黼复丁婦復干剛高羔告戈格厚乎鬲工公攻畫妃古谷鼓豐官觀盥光歸甫癸國果亥蒿好禾合何河后呼壺庚降虎化淮黃今惠會或鑊基箕及九吉龜即亟鬼耤己季行考既祭家甲寇斝監侯姜彊角教巾斤進盡京兢獲井競六酒舊沮句絕疾君畯可克客口牢老見醴立利麗解良林敏陵令柳龍盧魯陸鹿爵洛內旅年率馬買麥來卯枚眉每媚門蒙盟皿名明鳴命莫母前遣羌聾妾侵秦卿丘牡區曲取去犬人壬日戎木入若卅三散目牧乃南逆廿鳥涉宁寧牛農奴諾女旁轡彭辟品僕圃七戚祈齊杞啟气千且如山男麓上少紹射申沈生省師十石食時史矢豕使始士氏事室視首受獸唯未位畏衛文聞問我吳五午武舞勿戊夕兮昔析奚襲徙喜系下先咸獻相叔蜀嗇戍束商水絲死巳四祀綏歲尸孫它唐天田聽同土退屯豚鼉外亡王韋黍司宋祥饗向象小效辛新星興兄休羞宿戌須徐宣旋學旬亞言炎衍甗央羊昜陽揚夭爻頁一伊衣依匜夷宜疑彝乙亦育聿玉圉雨羽漁虞魚雩盂余于幼又酉有友昃增游猶幽攸用永尹寅禋因義肄異易邑昱御元員曰月戉樂龠允宰載再在朱葬乍宅旃折貞朕郢征正鄭姪祖執旨黹至爰豸彘中終仲帚逐祝追召茲子自宗足卒之族俎尊止舟州周貯作";
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
