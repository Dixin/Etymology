---
layout: post
title: Simplification of Chinese Characters
---

# Simplification of Chinese Characters

## The Problem in 1950

1. For 2000 years there had been no real control over the proliferation of Chinese characters.
1. The standard Kangxi dictionary had 47,000 characters of which only about 7000 were useful or common.
1. There were many variant characters that meant the same thing, or nearly the same thing
1. The characters were very complicated and hard to write
1. Most people could not read

## The Fix

1. Analyze common variant characters and mandate which ones will be standard and which ones will be banned.
1. Analyze which characters are useful and ban all the rest
1. Simplify and standardize the written form of complicated characters

## The Questions to ask

1. Who made these rules
1. What were the rules
1. What was the logic behind these rules
1. What changes were made to these rules over history.

## Traditional to Simplified

1. First apply any variant rule that may apply (discussed below)
1. Second apply any simplification rule that may apply (discussed below)
1. Third apply any font rule that may apply

## Simplified to Traditional

To go from simplified to traditional you may have to figure out which of several traditional characters is it supposed to be.

You may have to consult an associated rule:
1. Variant Rule
1. Simplification Rule
1. Applied Rule
1. Font Rule
1. Total Character List

(discussed below)

# Variant Rule V001 to V810

Standard table of Variants 第一批异体字整理表

## Definitions:

**Variants**: Two or more characters which have

 1. exact same pronunciation
 1. exact same meaning
 1. equivalent logic.

**Equivalent Logic**: 碴 and 𦉆 are both pronounced “cha” and both mean “chip”

1. In one the logic means it may be a chip from a rock 石 shí

1. In the other the logic means it may be a chip from a piece of pottery

缶 fǒu These two characters have “equivalent logic”

**Same Character**: 报報 and 𡙈 are derived from the same ancient character. 㚔 is derived from the original pictograph. 幸 is derived from 㚔 and 扌is derived from 幸. They are the same character.

Substituted Character:

## History

In 1956 the government published a list of 810 acceptable variant characters and 1055 associated variant characters which were to be banned.

## Problems:

1. A variant should be the exact same meaning and pronunciation. But some of the characters only had the same pronunciation but had different meanings. They were in fact substitutions.
1. There were some special rules associated with this list, where you could use the banned version in some situations depending on its meaning or pronunciation.
1. This resulted in modern generation of Chinese not being able to recognize many common traditional variant characters.
1. Over the years, some modifications were made and some banned characters were resurrected. The most recent table has

796 accepted characters.

1024 banned characters.

# Simplification Rule A001-A350

Stand Alone Simplification Only 个不作简化偏旁用的简化字

## The History

In 1955, a list of 350 characters was published which were given simplified forms.

These characters were to be simplified in their stand alone form, but not if they are part of another character.

Actually here are cases where they are simplified as components.

## The Problems

1. In several cases multiple characters are converged to one character.
1. In several cases the character convergences have been resurrected.
1. Sometimes the character is substituted for a rare traditional character. 
    - This complicates the definition of what is a traditional character and what is a traditional character.
1. Sometimes new characters are formed because of the simplification logic.
    - These new characters never existed in any dictionary previously and so I call them “new” characters, even though they may have existed in hand written Chinese for a long time.
1. In some characters, even the radical component is simplified
1. In some cases, the character is not simplified because of some special use.

# Simplification Rule B001-B123

Simplified as Stand Alone or as Radical 个可作简化偏旁用的简化字和简化偏旁

## The History

In 1955, a list of 132 characters was published which are simplified in their standalone form and are also simplified when used as components of another character.

There are 1 to n convergences due to table 5

# Simplification Rule R01-R14

Simplified radicals 简化偏旁14个

## The History

In 1955 a set of 14 components that if they have stand alone forms, the standalone forms  are not simplified. Some of these characters never have stand alone forms. They are just components of characters. There are also places where characters are components, but not simplified. 

# Applied Rule:

Rule based simplification 1753个应用第二表所列简化字和简化偏旁得出来的简化字

## The History

There were 1753 Characters that were simplified according to table 1, 2, 3 and 4

There are characters that are simplified using other rules that are not included in Table 1,2 and 3

Some of these characters are quite uncommon characters.

## The Problems

1. In some cases the rules are not strictly applied and the simplified character is an anomaly.
1. Sometimes the rules are applied to banned alternates and you come up with strange situations.
1. The number 1753 was applied to the original list of printed characters, so that list is not complete now.
1. This list is out of date. There are characters from this list that are not included in the 2013 standard list of characters and there are simplifications in the 2013 list of characters that are beyond this list.

# Complete Simplified Character List and the New Character Font

## Purpose of a Complete Character list

1. Limit the number of useless characters
1. Establish Standard Printed Forms using the above “variant rules” and “simplification rules”
1. Establish exact printed form for characters called the “new character forms”
1. Establish Literacy Standard

## The History

There have been four publications that can be considered complete lists of simplified characters
1.  印刷通用汉字字形表 1965 list of 6196 simplified characters for publishers
1.  1980 list of standard simplified computer characters GB2312-80
1.  現代漢語常用字表 1988 list of 7000 simplified characters
1.  通用規範漢字表 2013 list of 8105 simplified characters

## The Problems

1. There are always characters not in the list that you think you need that you don’t have
1. The “New Character Forms”
1. The lists were published with no associated traditional characters.

# 新字形 New Character Forms and the 標楷體 Standard Kai Form

## Font Rule

Besides following all the rules for simplification of characters, the exact character form was specified in what is called the “new character form”.  These forms were probably barely noticed by most people but when computers came along, it caused a number of characters to have different code points because of the difference in how governments thought characters were supposed to be written.

## Example

黄 and 黃 are two different code points and ways to write the character for “yellow”

黄 is the new character form and

黃 is the standard Kai form or Old Character forms

# The Unicode Standard and Han Unification

## The Problem

Over the years there had evolved some differences in how Korea, Japan, China, Taiwan, Hongkong and Vietnam wrote the same Chinese character.  

## The Solution

1. Korea, Japan, China, Taiwan, Hongkong and Vietnam decided to get together and decide on code points and character structure for all the useful Han characters. If they could agree that the characters were the same, they were all put in one code point, if not, there were different code points for instance 黄 and 黃 are two different code points, even though they are the same character.
1. Today almost all Han characters on computers are encoded in Unicode.
