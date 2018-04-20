import Cookies from "js-cookie";

const global = window;
const $ = global.$;
const Math = global.Math;

const commonChars = "安八白百败般邦保宝北贝比毕辟宾兵丙秉伯帛卜不步采仓曹长厂车彻臣辰晨成乘承迟齿赤冲丑初出楚传春此次大丹单旦稻得登邓帝典奠丁鼎东冬动斗豆断队对多而儿耳二伐反方分封逢夫服福弗甫斧复父妇干刚高羔告戈格庚工攻公弓鼓古谷官观光归龟鬼癸国果亥好禾何河侯厚后呼乎壶虎化淮黄惠会获或基箕吉及疾即己季祭既家甲监见姜降角教解巾斤今进兢京井竞九酒旧沮句爵君考可克客口寇来牢老乐丽利立良林陵令柳六聋卢鲁麓鹿陆旅率洛马买麦卯枚眉每媚门蒙盟皿敏明鸣名命莫牡母木目牧乃南男内逆年鸟宁牛农奴女诺旁彭品仆圃戚七齐祈启气千前遣羌强且侵秦卿丘区取去犬壬人日戎如入若三散山上少绍射涉申生省师尸十石时食史矢使始士事氏室视首受叔蜀黍束戍水司丝死四巳宋宿绥岁孙它唐天田同徒土退屯外王亡韦唯未畏位卫文闻问我吴武五午舞戊勿昔析夕袭喜下先相祥向象小效辛新兴行兄休羞戌徐畜宣旋学旬亚言炎衍央扬羊阳页一依伊衣夷疑宜彝乙易邑肄义因寅尹永用幽酉有友又幼于盂虞余鱼渔雨羽玉御育元员曰月允宰载再在葬增曾乍宅召折贞征正郑之执止旨至中终仲舟周州帚朱逐贮祝追兹子自宗足卒族祖尊作";
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
        return code >= 0x3400 && code <= 0x2FA1D;
    }
};
