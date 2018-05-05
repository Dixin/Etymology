import Cookies from "js-cookie";
import "String.prototype.codePointAt";

const global = window;
const $ = global.$;
const Math = global.Math;

const commonChars =
          "安八白百敗般邦保寶北貝匕比妣畢賓兵丙秉并伯帛亳卜不步采倉曹曾長厂鬯車徹臣辰晨成承乘遲齒赤沖舂丑出初楚畜傳春祠此朿次大丹單旦稻得登帝典奠丁鼎冬東動斗豆斷隊對多而兒耳二伐反匚方妃分封豐酆逢缶夫弗服福甫斧簠黼父复婦復干剛高羔告戈格鬲庚工弓公攻古谷鼓官觀盥光龜歸鬼癸國果亥行蒿好禾合何河侯后厚乎呼壺虎化畫淮黃惠會或獲鑊基箕及吉即亟疾耤己季既祭家甲斝監見姜彊降角教解巾斤今進盡京兢井競九酒舊沮句絕爵君畯考可克客口寇來牢老醴立利麗良林陵令柳六龍聾盧魯陸鹿麓洛旅率馬買麥卯枚眉每媚門蒙盟皿敏名明鳴命莫母牡木目牧乃男南內逆年廿鳥宁寧牛農奴諾女旁轡彭辟品僕圃七戚祈齊杞啟气千前遣羌且妾侵秦卿丘區曲取去犬人壬日戎如入若卅三散嗇山商上少紹射涉申沈生省尸師十石食時史矢豕使始士氏事室視首受獸叔黍蜀戍束水司絲死巳四祀宋綏歲孫它唐天田聽同土退屯豚鼉外亡王韋唯未位畏衛文聞問我吳五午武舞勿戊夕兮昔析奚襲徙喜系下先咸獻相祥饗向象小效辛新星興兄休羞宿戌須徐宣旋學旬亞言炎衍甗央羊昜陽揚夭爻頁一伊衣依匜夷宜疑彝乙亦邑易異肄義因禋寅尹郢永用攸幽猶游友有酉又幼于余盂雩魚虞漁羽雨圉玉聿育昱御元爰員曰月戉樂龠允宰載再在葬昃增乍宅旃召折貞朕征正鄭之姪執止旨黹至豸彘中終仲舟州周帚朱逐祝貯追茲子自宗足卒族俎祖尊作",

    commonCharsLength = commonChars.length,

    isSurrogatePair = text => text.match(/^[\uD800-\uDBFF][\uDC00-\uDFFF]$/),

    basicRanges = [
        0x4E00, 0x9FFF,
        0x3400, 0x4DBF,
        0x2E80, 0x2EFF,
        0x2F00, 0x2FDF,
        0x2FF0, 0x2FFF,
        0x3000, 0x303F,
        0x3040, 0x309F,
        0x30A0, 0x30FF,
        0x3100, 0x312F,
        0x31A0, 0x31BF,
        0x31C0, 0x31EF,
        0x31F0, 0x31FF,
        0xF900, 0xFAFF,
        0xFE30, 0xFE4F,
        0xFF00, 0xFFEF
    ],

    surrogateRanges = [
        0x20000, 0x2A6DF,
        0x2A700, 0x2B73F,
        0x2B740, 0x2B81F,
        0x2F800, 0x2FA1F,
    ];

export default {
    search: chinese => {
        const token = Cookies.get("Bronze");
        return $.ajax({
            type: "POST",
            data: { chinese, Bronze: token },
            dataType: "html",
            url: "etymology",
            headers: { chinese: chinese.codePointAt(0), Seal: token }
        });
    },

    randomChinese: () => {
        const index = Math.floor(Math.random() * commonCharsLength + 1);
        return commonChars[index];
    },

    isSingleChinese: text => {
        if (!text) {
            return false;
        }
        const length = text.length;
        if (length < 1 || length > 2) {
            return false;
        }
        const isSurrogate = isSurrogatePair(text);
        if (isSurrogate) {
            if (length === 1) {
                return false;
            }
        } else {
            if (length === 2) {
                return false;
            }
        }
        
        const codePoint = text.codePointAt(0);
        const range = isSurrogate ? surrogateRanges : basicRanges;
        for (let index = 0; index < range.length; index += 2) {
            if (range[index] <= codePoint && range[index + 1] >= codePoint) {
                return true;
            }
        }
        return false;
    }
};
