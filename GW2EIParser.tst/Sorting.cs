﻿using GW2EIEvtcParser.ParsedData;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GW2EIEvtcParser.Internals.tst;

class Sorting
{
    public class TestEvent(long t, int order) : AbstractTimeCombatEvent(t)
    {
        public int Order = order;

        public override string ToString() => $"({Time} {Order})";


        public class Comparer : IComparer
        {
            public static readonly Comparer Instance = new();
            public int Compare(object x, object y) => (int)(((TestEvent)x).Time.CompareTo(((TestEvent)y).Time));
        }
    }

    [Test]
    public void StableSortByTime_AbstractTimeCombatEvent()
    {
        //NOTE(Rennorb): Robustly testing this is actually quite hard, as the default implementation will still work for a bunch of cases.
        // But this should at least catch obvious issues.
        var list = new List<TestEvent>() {
            new(1, 3),
            new(0, 1),
            new(1, 4),
            new(0, 2),
        };

        list.SortByTime();

        for(int i = 0; i < list.Count - 1; i++)
        {
            var a = list[i];
            var b = list[i + 1];
            Assert.That(a.Time <= b.Time, "SortByTime must sort by time ascending");
            Assert.Less(a.Order, b.Order, "SortByTime must be a stable sort");
        }
    }


    class TestCombatItem(long t, int v) : CombatItem(t, default, default, v, default, default, default, default, default, default, default, 
            default, default, default, default, default, default, default, default, default, default, default, default, default)
    {
        public override string ToString()
        {
            return $"({Time} {Value})";
        }
    }

    [Test]
    public void StableSortByTime_CombatItem()
    {
        //NOTE(Rennorb): Robustly testing this is actually quite hard, as the default implementation will still work for a bunch of cases.
        // But this should at least catch obvious issues.
        var list = new List<TestCombatItem>() {
            new(1, 3),
            new(0, 1),
            new(1, 4),
            new(0, 2),
        };

        list.SortByTime();

        for(int i = 0; i < list.Count - 1; i++)
        {
            var a = list[i];
            var b = list[i + 1];
            Assert.That(a.Time <= b.Time, "SortByTime must sort by time ascending");
            Assert.Less(a.Value, b.Value, "SortByTime must be a stable sort");
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    class TestCastEvent(long t, bool swap, int order) : AbstractCastEvent(t, new(swap), null!)
    {
        public int Order = order;

        public override string ToString()
        {
            return $"({Time} {Skill.IsSwap} {Order})";
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete

    [Test]
    public void StableSortByTimeThenSwap()
    {
        //NOTE(Rennorb): Robustly testing this is actually quite hard, as the default implementation will still work for a bunch of cases.
        // But this should at least catch obvious issues.
        var list = new List<TestCastEvent>() {
            new(0, false, 0),
            new(1, false, 3),
            new(0, true , 1),
            new(1, true , 5),
            new(0, true , 2),
            new(1, false, 4),
        };

        list.SortByTimeThenSwap();

        for(int i = 0; i < list.Count - 1; i++)
        {
            var a = list[i];
            var b = list[i + 1];
            Assert.That(a.Time < b.Time || (a.Time == b.Time && Convert.ToInt32(a.Skill.IsSwap) <= Convert.ToInt32(b.Skill.IsSwap)), "SortByTimeThenSwap must sort by time ascending, then by isSwap");
            Assert.Less(a.Order, b.Order, "SortByTimeThenSwap must be a stable sort");
        }
    }

    [Test]
    public void StableSortByTimeThenNegatedSwap()
    {
        //NOTE(Rennorb): Robustly testing this is actually quite hard, as the default implementation will still work for a bunch of cases.
        // But this should at least catch obvious issues.
        var list = new List<TestCastEvent>() {
            new(0, true , 0),
            new(1, true , 3),
            new(0, true , 1),
            new(0, false, 2),
            new(1, false, 4),
            new(1, false, 5),
        };

        list.SortByTimeThenNegatedSwap();

        for(int i = 0; i < list.Count - 1; i++)
        {
            var a = list[i];
            var b = list[i + 1];
            Assert.That(a.Time < b.Time || (a.Time == b.Time && Convert.ToInt32(a.Skill.IsSwap) >= Convert.ToInt32(b.Skill.IsSwap)), "SortByTimeThenNegatedSwap must sort by time ascending, then by not isSwap");
            Assert.Less(a.Order, b.Order, $"SortByTimeThenNegatedSwap must be a stable sort. index {i} (+1)");
        }
    }

    [Test]
    public void Branch3()
    {
        //TODO(Rennorb) make more minimal
        var list = new List<AbstractBuffEvent>() {
            #pragma warning disable CS0618 // Type or member is obsolete
            #region unhinged test
            new BuffApplyEvent(1),
            new BuffApplyEvent(1),
            new BuffApplyEvent(1),
            new BuffApplyEvent(1),
            new BuffApplyEvent(1),
            new BuffRemoveSingleEvent(8702),
            new BuffRemoveSingleEvent(8702),
            new BuffRemoveSingleEvent(8702),
            new BuffRemoveSingleEvent(8702),
            new BuffRemoveSingleEvent(8702),
            new BuffApplyEvent(8752),
            new BuffApplyEvent(9104),
            new BuffApplyEvent(9391),
            new BuffApplyEvent(9823),
            new BuffApplyEvent(9991),
            new BuffRemoveSingleEvent(11673),
            new BuffRemoveSingleEvent(11673),
            new BuffRemoveSingleEvent(11673),
            new BuffRemoveSingleEvent(11673),
            new BuffRemoveSingleEvent(11673),
            new BuffApplyEvent(12275),
            new BuffApplyEvent(12391),
            new BuffApplyEvent(12476),
            new BuffApplyEvent(13390),
            new BuffRemoveSingleEvent(13989),
            new BuffRemoveSingleEvent(13989),
            new BuffRemoveSingleEvent(13989),
            new BuffRemoveSingleEvent(13989),
            new BuffApplyEvent(13989),
            new BuffApplyEvent(14556),
            new BuffApplyEvent(14556),
            new BuffApplyEvent(14955),
            new BuffRemoveSingleEvent(15507),
            new BuffRemoveSingleEvent(15507),
            new BuffRemoveSingleEvent(15507),
            new BuffRemoveSingleEvent(15507),
            new BuffApplyEvent(15672),
            new BuffApplyEvent(15989),
            new BuffApplyEvent(16155),
            new BuffApplyEvent(16423),
            new BuffApplyEvent(16823),
            new BuffRemoveSingleEvent(17440),
            new BuffRemoveSingleEvent(17440),
            new BuffRemoveSingleEvent(17440),
            new BuffRemoveSingleEvent(17440),
            new BuffRemoveSingleEvent(17440),
            new BuffApplyEvent(17874),
            new BuffApplyEvent(18073),
            new BuffApplyEvent(18223),
            new BuffApplyEvent(18627),
            new BuffRemoveSingleEvent(19356),
            new BuffRemoveSingleEvent(19356),
            new BuffRemoveSingleEvent(19356),
            new BuffRemoveSingleEvent(19356),
            new BuffApplyEvent(19556),
            new BuffApplyEvent(19706),
            new BuffApplyEvent(19874),
            new BuffApplyEvent(20390),
            new BuffApplyEvent(20506),
            new BuffRemoveSingleEvent(21543),
            new BuffRemoveSingleEvent(21543),
            new BuffRemoveSingleEvent(21543),
            new BuffRemoveSingleEvent(21543),
            new BuffRemoveSingleEvent(21543),
            new BuffApplyEvent(21989),
            new BuffApplyEvent(22513),
            new BuffApplyEvent(23200),
            new BuffApplyEvent(23547),
            new BuffApplyEvent(23863),
            new BuffRemoveSingleEvent(25195),
            new BuffRemoveSingleEvent(25195),
            new BuffRemoveSingleEvent(25195),
            new BuffRemoveSingleEvent(25195),
            new BuffRemoveSingleEvent(25195),
            new BuffApplyEvent(25546),
            new BuffApplyEvent(25630),
            new BuffApplyEvent(25830),
            new BuffApplyEvent(26112),
            new BuffApplyEvent(26746),
            new BuffRemoveSingleEvent(28430),
            new BuffRemoveSingleEvent(28430),
            new BuffRemoveSingleEvent(28430),
            new BuffRemoveSingleEvent(28430),
            new BuffRemoveSingleEvent(28430),
            new BuffApplyEvent(28792),
            new BuffApplyEvent(29142),
            new BuffApplyEvent(29275),
            new BuffApplyEvent(29275),
            new BuffApplyEvent(30742),
            new BuffRemoveSingleEvent(31676),
            new BuffRemoveSingleEvent(31676),
            new BuffRemoveSingleEvent(31676),
            new BuffRemoveSingleEvent(31676),
            new BuffRemoveSingleEvent(31676),
            new BuffApplyEvent(32395),
            new BuffApplyEvent(33075),
            new BuffApplyEvent(33867),
            new BuffApplyEvent(33867),
            new BuffApplyEvent(34066),
            new BuffRemoveSingleEvent(34983),
            new BuffRemoveSingleEvent(34983),
            new BuffRemoveSingleEvent(34983),
            new BuffRemoveSingleEvent(34983),
            new BuffRemoveSingleEvent(34983),
            new BuffApplyEvent(35033),
            new BuffApplyEvent(35433),
            new BuffApplyEvent(35832),
            new BuffApplyEvent(36035),
            new BuffApplyEvent(36151),
            new BuffRemoveSingleEvent(37032),
            new BuffRemoveSingleEvent(37032),
            new BuffRemoveSingleEvent(37032),
            new BuffRemoveSingleEvent(37032),
            new BuffRemoveSingleEvent(37032),
            new BuffApplyEvent(37232),
            new BuffApplyEvent(37665),
            new BuffApplyEvent(38516),
            new BuffApplyEvent(38782),
            new BuffApplyEvent(38782),
            new BuffRemoveSingleEvent(42114),
            new BuffRemoveSingleEvent(42114),
            new BuffRemoveSingleEvent(42114),
            new BuffRemoveSingleEvent(42114),
            new BuffRemoveSingleEvent(42115),
            new BuffApplyEvent(42465),
            new BuffApplyEvent(42783),
            new BuffApplyEvent(43116),
            new BuffApplyEvent(43382),
            new BuffApplyEvent(43551),
            new BuffRemoveSingleEvent(45788),
            new BuffRemoveSingleEvent(45788),
            new BuffRemoveSingleEvent(45788),
            new BuffRemoveSingleEvent(45788),
            new BuffRemoveSingleEvent(45788),
            new BuffApplyEvent(46108),
            new BuffApplyEvent(46475),
            new BuffApplyEvent(46508),
            new BuffApplyEvent(46674),
            new BuffApplyEvent(48105),
            new BuffRemoveSingleEvent(56302),
            new BuffRemoveSingleEvent(56302),
            new BuffRemoveSingleEvent(56302),
            new BuffRemoveSingleEvent(56302),
            new BuffRemoveSingleEvent(56302),
            new BuffApplyEvent(57824),
            new BuffApplyEvent(58711),
            new BuffApplyEvent(59636),
            new BuffApplyEvent(60862),
            new BuffApplyEvent(61959),
            new BuffRemoveSingleEvent(64343),
            new BuffRemoveSingleEvent(64343),
            new BuffRemoveSingleEvent(64343),
            new BuffRemoveSingleEvent(64343),
            new BuffRemoveSingleEvent(64343),
            new BuffApplyEvent(65077),
            new BuffApplyEvent(65310),
            new BuffApplyEvent(65826),
            new BuffApplyEvent(66144),
            new BuffApplyEvent(66145),
            new BuffRemoveSingleEvent(67911),
            new BuffRemoveSingleEvent(67911),
            new BuffRemoveSingleEvent(67911),
            new BuffRemoveSingleEvent(67911),
            new BuffRemoveSingleEvent(67911),
            new BuffApplyEvent(68431),
            new BuffApplyEvent(68631),
            new BuffApplyEvent(69148),
            new BuffApplyEvent(69597),
            new BuffRemoveSingleEvent(70147),
            new BuffRemoveSingleEvent(70147),
            new BuffRemoveSingleEvent(70147),
            new BuffRemoveSingleEvent(70147),
            new BuffApplyEvent(70397),
            new BuffApplyEvent(70632),
            new BuffApplyEvent(70747),
            new BuffApplyEvent(70864),
            new BuffRemoveSingleEvent(71548),
            new BuffRemoveSingleEvent(71548),
            new BuffRemoveSingleEvent(71548),
            new BuffRemoveSingleEvent(71548),
            new BuffApplyEvent(71631),
            new BuffApplyEvent(72432),
            new BuffApplyEvent(72914),
            new BuffApplyEvent(73397),
            new BuffApplyEvent(73664),
            new BuffRemoveSingleEvent(74263),
            new BuffRemoveSingleEvent(74263),
            new BuffRemoveSingleEvent(74263),
            new BuffRemoveSingleEvent(74264),
            new BuffRemoveSingleEvent(74264),
            new BuffApplyEvent(74548),
            new BuffApplyEvent(74630),
            new BuffApplyEvent(74997),
            new BuffApplyEvent(75464),
            new BuffApplyEvent(75514),
            new BuffRemoveSingleEvent(76197),
            new BuffRemoveSingleEvent(76197),
            new BuffRemoveSingleEvent(76197),
            new BuffRemoveSingleEvent(76197),
            new BuffRemoveSingleEvent(76197),
            new BuffApplyEvent(76396),
            new BuffApplyEvent(76713),
            new BuffApplyEvent(77713),
            new BuffApplyEvent(78947),
            new BuffApplyEvent(79064),
            new BuffRemoveSingleEvent(79464),
            new BuffRemoveSingleEvent(79464),
            new BuffRemoveSingleEvent(79464),
            new BuffRemoveSingleEvent(79464),
            new BuffRemoveSingleEvent(79464),
            new BuffApplyEvent(79864),
            new BuffApplyEvent(80148),
            new BuffApplyEvent(80148),
            new BuffApplyEvent(80464),
            new BuffApplyEvent(80831),
            new BuffRemoveSingleEvent(81547),
            new BuffRemoveSingleEvent(81547),
            new BuffRemoveSingleEvent(81547),
            new BuffRemoveSingleEvent(81547),
            new BuffRemoveSingleEvent(81547),
            new BuffApplyEvent(81946),
            new BuffApplyEvent(82064),
            new BuffApplyEvent(82346),
            new BuffApplyEvent(83116),
            new BuffApplyEvent(83466),
            new BuffRemoveSingleEvent(84232),
            new BuffRemoveSingleEvent(84232),
            new BuffRemoveSingleEvent(84232),
            new BuffRemoveSingleEvent(84232),
            new BuffRemoveSingleEvent(84232),
            new BuffApplyEvent(84868),
            new BuffApplyEvent(84869),
            new BuffApplyEvent(85469),
            new BuffApplyEvent(86749),
            new BuffApplyEvent(87066),
            new BuffRemoveSingleEvent(89266),
            new BuffRemoveSingleEvent(89266),
            new BuffRemoveSingleEvent(89266),
            new BuffRemoveSingleEvent(89266),
            new BuffRemoveSingleEvent(89266),
            new BuffApplyEvent(89635),
            new BuffApplyEvent(89669),
            new BuffApplyEvent(89984),
            new BuffApplyEvent(90202),
            new BuffApplyEvent(90783),
            new BuffRemoveSingleEvent(91351),
            new BuffRemoveSingleEvent(91351),
            new BuffRemoveSingleEvent(91351),
            new BuffRemoveSingleEvent(91351),
            new BuffRemoveSingleEvent(91351),
            new BuffApplyEvent(91503),
            new BuffApplyEvent(91843),
            new BuffApplyEvent(91990),
            new BuffApplyEvent(92235),
            new BuffApplyEvent(92236),
            new BuffRemoveSingleEvent(93386),
            new BuffRemoveSingleEvent(93386),
            new BuffRemoveSingleEvent(93386),
            new BuffRemoveSingleEvent(93386),
            new BuffRemoveSingleEvent(93386),
            new BuffApplyEvent(93952),
            new BuffApplyEvent(94436),
            new BuffApplyEvent(94753),
            new BuffApplyEvent(95152),
            new BuffApplyEvent(96069),
            new BuffRemoveSingleEvent(96119),
            new BuffRemoveSingleEvent(96119),
            new BuffRemoveSingleEvent(96119),
            new BuffRemoveSingleEvent(96119),
            new BuffRemoveSingleEvent(96119),
            new BuffApplyEvent(96956),
            new BuffApplyEvent(97588),
            new BuffApplyEvent(97754),
            new BuffApplyEvent(102705),
            new BuffApplyEvent(103871),
            new BuffRemoveSingleEvent(120472),
            new BuffRemoveSingleEvent(120472),
            new BuffRemoveSingleEvent(120472),
            new BuffRemoveSingleEvent(120472),
            new BuffRemoveSingleEvent(120472),
            new BuffApplyEvent(121073),
            new BuffApplyEvent(121307),
            new BuffApplyEvent(121790),
            new BuffApplyEvent(122024),
            new BuffApplyEvent(122024),
            new BuffRemoveSingleEvent(123593),
            new BuffRemoveSingleEvent(123593),
            new BuffRemoveSingleEvent(123593),
            new BuffRemoveSingleEvent(123593),
            new BuffRemoveSingleEvent(123593),
            new BuffApplyEvent(124070),
            new BuffApplyEvent(124311),
            new BuffApplyEvent(124426),
            new BuffApplyEvent(125144),
            new BuffApplyEvent(125474),
            new BuffRemoveSingleEvent(126025),
            new BuffRemoveSingleEvent(126025),
            new BuffRemoveSingleEvent(126025),
            new BuffRemoveSingleEvent(126025),
            new BuffRemoveSingleEvent(126025),
            new BuffApplyEvent(126143),
            new BuffApplyEvent(126227),
            new BuffApplyEvent(126476),
            new BuffApplyEvent(126593),
            new BuffApplyEvent(126628),
            new BuffRemoveSingleEvent(127109),
            new BuffRemoveSingleEvent(127109),
            new BuffRemoveSingleEvent(127109),
            new BuffRemoveSingleEvent(127109),
            new BuffRemoveSingleEvent(127109),
            new BuffApplyEvent(127593),
            new BuffApplyEvent(127943),
            new BuffApplyEvent(128876),
            new BuffApplyEvent(129110),
            new BuffRemoveSingleEvent(130025),
            new BuffRemoveSingleEvent(130025),
            new BuffRemoveSingleEvent(130025),
            new BuffRemoveSingleEvent(130025),
            new BuffApplyEvent(130110),
            new BuffApplyEvent(130193),
            new BuffApplyEvent(130675),
            new BuffApplyEvent(130827),
            new BuffRemoveSingleEvent(131476),
            new BuffRemoveSingleEvent(131476),
            new BuffRemoveSingleEvent(131476),
            new BuffRemoveSingleEvent(131476),
            new BuffApplyEvent(131825),
            new BuffApplyEvent(131825),
            new BuffApplyEvent(132109),
            new BuffApplyEvent(132142),
            new BuffRemoveSingleEvent(133474),
            new BuffRemoveSingleEvent(133474),
            new BuffRemoveSingleEvent(133474),
            new BuffRemoveSingleEvent(133474),
            new BuffApplyEvent(134113),
            new BuffApplyEvent(134475),
            new BuffApplyEvent(134709),
            new BuffApplyEvent(135109),
            new BuffApplyEvent(135508),
            new BuffRemoveSingleEvent(135942),
            new BuffRemoveSingleEvent(135942),
            new BuffRemoveSingleEvent(135942),
            new BuffRemoveSingleEvent(135942),
            new BuffRemoveSingleEvent(135942),
            new BuffApplyEvent(136145),
            new BuffApplyEvent(136309),
            new BuffApplyEvent(136309),
            new BuffApplyEvent(136625),
            new BuffApplyEvent(137109),
            new BuffRemoveSingleEvent(138675),
            new BuffRemoveSingleEvent(138675),
            new BuffRemoveSingleEvent(138675),
            new BuffRemoveSingleEvent(138675),
            new BuffRemoveSingleEvent(138675),
            new BuffApplyEvent(138710),
            new BuffApplyEvent(139542),
            new BuffApplyEvent(140625),
            new BuffApplyEvent(140877),
            new BuffApplyEvent(141426),
            new BuffRemoveSingleEvent(142076),
            new BuffRemoveSingleEvent(142076),
            new BuffRemoveSingleEvent(142076),
            new BuffRemoveSingleEvent(142076),
            new BuffRemoveSingleEvent(142076),
            new BuffApplyEvent(142744),
            new BuffApplyEvent(143275),
            new BuffApplyEvent(145190),
            new BuffApplyEvent(145473),
            new BuffApplyEvent(145674),
            new BuffRemoveSingleEvent(146022),
            new BuffRemoveSingleEvent(146022),
            new BuffRemoveSingleEvent(146022),
            new BuffRemoveSingleEvent(146022),
            new BuffRemoveSingleEvent(146022),
            new BuffApplyEvent(146506),
            new BuffApplyEvent(146673),
            new BuffApplyEvent(146823),
            new BuffApplyEvent(147758),
            new BuffApplyEvent(152224),
            new BuffRemoveSingleEvent(156345),
            new BuffRemoveSingleEvent(156345),
            new BuffRemoveSingleEvent(156345),
            new BuffRemoveSingleEvent(156345),
            new BuffRemoveSingleEvent(156345),
            new BuffApplyEvent(157074),
            new BuffApplyEvent(157425),
            new BuffApplyEvent(157509),
            new BuffApplyEvent(158189),
            new BuffApplyEvent(158588),
            new BuffRemoveSingleEvent(160105),
            new BuffRemoveSingleEvent(160105),
            new BuffRemoveSingleEvent(160105),
            new BuffRemoveSingleEvent(160105),
            new BuffRemoveSingleEvent(160105),
            new BuffApplyEvent(160438),
            new BuffApplyEvent(160989),
            new BuffApplyEvent(161105),
            new BuffApplyEvent(161222),
            new BuffApplyEvent(161392),
            new BuffRemoveSingleEvent(162471),
            new BuffRemoveSingleEvent(162471),
            new BuffRemoveSingleEvent(162471),
            new BuffRemoveSingleEvent(162471),
            new BuffRemoveSingleEvent(162471),
            new BuffApplyEvent(162505),
            new BuffApplyEvent(163187),
            new BuffApplyEvent(163388),
            new BuffApplyEvent(163505),
            new BuffApplyEvent(163670),
            new BuffRemoveSingleEvent(164275),
            new BuffRemoveSingleEvent(164275),
            new BuffRemoveSingleEvent(164275),
            new BuffRemoveSingleEvent(164275),
            new BuffRemoveSingleEvent(164275),
            new BuffApplyEvent(164709),
            new BuffApplyEvent(165591),
            new BuffApplyEvent(166423),
            new BuffApplyEvent(167023),
            new BuffApplyEvent(167357),
            new BuffRemoveSingleEvent(167674),
            new BuffRemoveSingleEvent(167674),
            new BuffRemoveSingleEvent(167674),
            new BuffRemoveSingleEvent(167674),
            new BuffRemoveSingleEvent(167674),
            new BuffApplyEvent(1),
            new BuffApplyEvent(1),
            new BuffApplyEvent(1),
            new BuffApplyEvent(1),
            new BuffApplyEvent(1),
            new BuffRemoveSingleEvent(8036),
            new BuffRemoveSingleEvent(8036),
            new BuffRemoveSingleEvent(8036),
            new BuffRemoveSingleEvent(8036),
            new BuffRemoveSingleEvent(8036),
            new BuffApplyEvent(8670),
            new BuffApplyEvent(8869),
            new BuffApplyEvent(8902),
            new BuffApplyEvent(9423),
            new BuffApplyEvent(9706),
            new BuffRemoveSingleEvent(10392),
            new BuffRemoveSingleEvent(10392),
            new BuffRemoveSingleEvent(10392),
            new BuffRemoveSingleEvent(10392),
            new BuffRemoveSingleEvent(10392),
            new BuffApplyEvent(10706),
            new BuffApplyEvent(10907),
            new BuffApplyEvent(11274),
            new BuffApplyEvent(11790),
            new BuffApplyEvent(12111),
            new BuffRemoveSingleEvent(13755),
            new BuffRemoveSingleEvent(13755),
            new BuffRemoveSingleEvent(13755),
            new BuffRemoveSingleEvent(13755),
            new BuffRemoveSingleEvent(13755),
            new BuffApplyEvent(13789),
            new BuffApplyEvent(14273),
            new BuffApplyEvent(14306),
            new BuffApplyEvent(14306),
            new BuffApplyEvent(14590),
            new BuffRemoveSingleEvent(15507),
            new BuffRemoveSingleEvent(15507),
            new BuffRemoveSingleEvent(15507),
            new BuffRemoveSingleEvent(15507),
            new BuffRemoveSingleEvent(15507),
            new BuffApplyEvent(15705),
            new BuffApplyEvent(16073),
            new BuffApplyEvent(16556),
            new BuffApplyEvent(17106),
            new BuffRemoveSingleEvent(17758),
            new BuffRemoveSingleEvent(17758),
            new BuffRemoveSingleEvent(17758),
            new BuffRemoveSingleEvent(17758),
            new BuffApplyEvent(18272),
            new BuffApplyEvent(18306),
            new BuffApplyEvent(18477),
            new BuffApplyEvent(18955),
            new BuffApplyEvent(18955),
            new BuffRemoveSingleEvent(19390),
            new BuffRemoveSingleEvent(19390),
            new BuffRemoveSingleEvent(19390),
            new BuffRemoveSingleEvent(19390),
            new BuffRemoveSingleEvent(19390),
            new BuffApplyEvent(19946),
            new BuffApplyEvent(20472),
            new BuffApplyEvent(20506),
            new BuffApplyEvent(21390),
            new BuffApplyEvent(22462),
            new BuffRemoveSingleEvent(22995),
            new BuffRemoveSingleEvent(22995),
            new BuffRemoveSingleEvent(22995),
            new BuffRemoveSingleEvent(22995),
            new BuffRemoveSingleEvent(22995),
            new BuffApplyEvent(23479),
            new BuffApplyEvent(23596),
            new BuffApplyEvent(23596),
            new BuffApplyEvent(24062),
            new BuffApplyEvent(24479),
            new BuffRemoveSingleEvent(25195),
            new BuffRemoveSingleEvent(25195),
            new BuffRemoveSingleEvent(25195),
            new BuffRemoveSingleEvent(25195),
            new BuffRemoveSingleEvent(25195),
            new BuffApplyEvent(25796),
            new BuffApplyEvent(26112),
            new BuffApplyEvent(26112),
            new BuffApplyEvent(26796),
            new BuffApplyEvent(27918),
            new BuffRemoveSingleEvent(31945),
            new BuffRemoveSingleEvent(31945),
            new BuffRemoveSingleEvent(31945),
            new BuffRemoveSingleEvent(31945),
            new BuffRemoveSingleEvent(31945),
            new BuffApplyEvent(32756),
            new BuffApplyEvent(33424),
            new BuffApplyEvent(33424),
            new BuffApplyEvent(34633),
            new BuffApplyEvent(34950),
            new BuffRemoveSingleEvent(35952),
            new BuffRemoveSingleEvent(35952),
            new BuffRemoveSingleEvent(35952),
            new BuffRemoveSingleEvent(35952),
            new BuffRemoveSingleEvent(35952),
            new BuffApplyEvent(36515),
            new BuffApplyEvent(36715),
            new BuffApplyEvent(36865),
            new BuffApplyEvent(37265),
            new BuffApplyEvent(38632),
            new BuffRemoveSingleEvent(40515),
            new BuffRemoveSingleEvent(40515),
            new BuffRemoveSingleEvent(40515),
            new BuffRemoveSingleEvent(40515),
            new BuffRemoveSingleEvent(40515),
            new BuffApplyEvent(40717),
            new BuffApplyEvent(40982),
            new BuffApplyEvent(41183),
            new BuffApplyEvent(41665),
            new BuffApplyEvent(41715),
            new BuffRemoveSingleEvent(42315),
            new BuffRemoveSingleEvent(42315),
            new BuffRemoveSingleEvent(42315),
            new BuffRemoveSingleEvent(42315),
            new BuffRemoveSingleEvent(42315),
            new BuffApplyEvent(42715),
            new BuffApplyEvent(42865),
            new BuffApplyEvent(43182),
            new BuffApplyEvent(43183),
            new BuffApplyEvent(43748),
            new BuffRemoveSingleEvent(56269),
            new BuffRemoveSingleEvent(56269),
            new BuffRemoveSingleEvent(56269),
            new BuffRemoveSingleEvent(56269),
            new BuffRemoveSingleEvent(56269),
            new BuffApplyEvent(65194),
            new BuffApplyEvent(65194),
            new BuffApplyEvent(66744),
            new BuffApplyEvent(67276),
            new BuffRemoveSingleEvent(68262),
            new BuffRemoveSingleEvent(68262),
            new BuffRemoveSingleEvent(68262),
            new BuffRemoveSingleEvent(68262),
            new BuffApplyEvent(68716),
            new BuffApplyEvent(68864),
            new BuffApplyEvent(68982),
            new BuffApplyEvent(69350),
            new BuffApplyEvent(69750),
            new BuffRemoveSingleEvent(70998),
            new BuffRemoveSingleEvent(70998),
            new BuffRemoveSingleEvent(70998),
            new BuffRemoveSingleEvent(70998),
            new BuffRemoveSingleEvent(70998),
            new BuffApplyEvent(71265),
            new BuffApplyEvent(71265),
            new BuffApplyEvent(71631),
            new BuffApplyEvent(72032),
            new BuffApplyEvent(72032),
            new BuffRemoveSingleEvent(72514),
            new BuffRemoveSingleEvent(72514),
            new BuffRemoveSingleEvent(72514),
            new BuffRemoveSingleEvent(72514),
            new BuffRemoveSingleEvent(72514),
            new BuffApplyEvent(72747),
            new BuffApplyEvent(73148),
            new BuffApplyEvent(73464),
            new BuffApplyEvent(74113),
            new BuffApplyEvent(74347),
            new BuffRemoveSingleEvent(75197),
            new BuffRemoveSingleEvent(75197),
            new BuffRemoveSingleEvent(75197),
            new BuffRemoveSingleEvent(75197),
            new BuffRemoveSingleEvent(75197),
            new BuffApplyEvent(75514),
            new BuffApplyEvent(75865),
            new BuffApplyEvent(76397),
            new BuffApplyEvent(76430),
            new BuffApplyEvent(76662),
            new BuffRemoveSingleEvent(80514),
            new BuffRemoveSingleEvent(80514),
            new BuffRemoveSingleEvent(80514),
            new BuffRemoveSingleEvent(80514),
            new BuffRemoveSingleEvent(80514),
            new BuffApplyEvent(80664),
            new BuffApplyEvent(81030),
            new BuffApplyEvent(81147),
            new BuffApplyEvent(81464),
            new BuffApplyEvent(82063),
            new BuffRemoveSingleEvent(82632),
            new BuffRemoveSingleEvent(82632),
            new BuffRemoveSingleEvent(82632),
            new BuffRemoveSingleEvent(82632),
            new BuffRemoveSingleEvent(82632),
            new BuffApplyEvent(83066),
            new BuffApplyEvent(83232),
            new BuffApplyEvent(83399),
            new BuffApplyEvent(83865),
            new BuffApplyEvent(84399),
            new BuffRemoveSingleEvent(88632),
            new BuffRemoveSingleEvent(88632),
            new BuffRemoveSingleEvent(88632),
            new BuffRemoveSingleEvent(88632),
            new BuffRemoveSingleEvent(88632),
            new BuffApplyEvent(88783),
            new BuffApplyEvent(88869),
            new BuffApplyEvent(89118),
            new BuffApplyEvent(89467),
            new BuffApplyEvent(89919),
            new BuffRemoveSingleEvent(90953),
            new BuffRemoveSingleEvent(90953),
            new BuffRemoveSingleEvent(90953),
            new BuffRemoveSingleEvent(90953),
            new BuffRemoveSingleEvent(90953),
            new BuffApplyEvent(91270),
            new BuffApplyEvent(91270),
            new BuffApplyEvent(91874),
            new BuffApplyEvent(92386),
            new BuffApplyEvent(93187),
            new BuffRemoveSingleEvent(93302),
            new BuffRemoveSingleEvent(93302),
            new BuffRemoveSingleEvent(93302),
            new BuffRemoveSingleEvent(93302),
            new BuffRemoveSingleEvent(93302),
            new BuffApplyEvent(93519),
            new BuffApplyEvent(93868),
            new BuffApplyEvent(93986),
            new BuffApplyEvent(94468),
            new BuffApplyEvent(94918),
            new BuffRemoveSingleEvent(118754),
            new BuffRemoveSingleEvent(118754),
            new BuffRemoveSingleEvent(118754),
            new BuffRemoveSingleEvent(118754),
            new BuffRemoveSingleEvent(118754),
            new BuffApplyEvent(118873),
            new BuffApplyEvent(120064),
            new BuffApplyEvent(120064),
            new BuffApplyEvent(120907),
            new BuffApplyEvent(122993),
            new BuffRemoveSingleEvent(125674),
            new BuffRemoveSingleEvent(125674),
            new BuffRemoveSingleEvent(125674),
            new BuffRemoveSingleEvent(125674),
            new BuffRemoveSingleEvent(125674),
            new BuffApplyEvent(126025),
            new BuffApplyEvent(126025),
            new BuffApplyEvent(126393),
            new BuffApplyEvent(126628),
            new BuffApplyEvent(127145),
            new BuffRemoveSingleEvent(128115),
            new BuffRemoveSingleEvent(128115),
            new BuffRemoveSingleEvent(128115),
            new BuffRemoveSingleEvent(128115),
            new BuffRemoveSingleEvent(128115),
            new BuffApplyEvent(128742),
            new BuffApplyEvent(128876),
            new BuffApplyEvent(129110),
            new BuffRemoveSingleEvent(129543),
            new BuffRemoveSingleEvent(129543),
            new BuffRemoveSingleEvent(129543),
            new BuffApplyEvent(129792),
            new BuffApplyEvent(129909),
            new BuffApplyEvent(130076),
            new BuffApplyEvent(130424),
            new BuffApplyEvent(130558),
            new BuffRemoveSingleEvent(131708),
            new BuffRemoveSingleEvent(131708),
            new BuffRemoveSingleEvent(131708),
            new BuffRemoveSingleEvent(131708),
            new BuffRemoveSingleEvent(131708),
            new BuffApplyEvent(132142),
            new BuffApplyEvent(132475),
            new BuffApplyEvent(132742),
            new BuffApplyEvent(133707),
            new BuffApplyEvent(134909),
            new BuffRemoveSingleEvent(139075),
            new BuffRemoveSingleEvent(139075),
            new BuffRemoveSingleEvent(139075),
            new BuffRemoveSingleEvent(139075),
            new BuffRemoveSingleEvent(139075),
            new BuffApplyEvent(139597),
            new BuffApplyEvent(140026),
            new BuffApplyEvent(140026),
            new BuffApplyEvent(140429),
            new BuffApplyEvent(140709),
            new BuffRemoveSingleEvent(141509),
            new BuffRemoveSingleEvent(141509),
            new BuffRemoveSingleEvent(141509),
            new BuffRemoveSingleEvent(141509),
            new BuffRemoveSingleEvent(141509),
            new BuffApplyEvent(141993),
            new BuffApplyEvent(142141),
            new BuffApplyEvent(142193),
            new BuffApplyEvent(142310),
            new BuffApplyEvent(142875),
            new BuffRemoveSingleEvent(146906),
            new BuffRemoveSingleEvent(146906),
            new BuffRemoveSingleEvent(146906),
            new BuffRemoveSingleEvent(146906),
            new BuffRemoveSingleEvent(146906),
            new BuffApplyEvent(147155),
            new BuffApplyEvent(148873),
            new BuffRemoveSingleEvent(164825),
            new BuffRemoveSingleEvent(164825),
            #endregion
            #pragma warning restore CS0618 // Type or member is obsolete
        };

        list.SortByTime();

    }

    //TODO(Rennorb) @cleanup: this might need to run multiple times because the branch depends a bit on the data
    [Test]
    public void Branch2()
    {
        var r = new Random();
        //  260031, 116634, 261071
        int len = 3000;
        var list = new List<TestEvent>(len);
        for(int i = 0; i < len; i++)
        {
            list.Add(new(r.Next(), i));
        }
        list.SortByTime();

        CollectionAssert.IsOrdered(list, TestEvent.Comparer.Instance);
    }

    [Test]
    public void Branch4()
    {
        var r = new Random();
        int len = 492052;
        #region cursed
        #endregion
        StableSort<long>.fluxsort(list.AsSpan(), (a, b) => a.CompareTo(b));

        CollectionAssert.IsOrdered(list);
    }

     [Test]
    public void Branch5()
    {
        var r = new Random();
        int len = 1288;
        #region cursed
        var list = new List<long>(len) { -48,432,832,1274,1757,2606,3041,4128,5088,5638,6076,6432,7356,7808,8160,8716,9154,9526,10717,11163,11530,12077,12286,13233,13686,14043,14115,14594,14752,16802,17207,18489,18925,19367,19845,20327,20636,21391,21886,22370,22852,23311,23812,24403,25165,25558,26005,26159,26648,27487,27530,27977,28405,29094,30045,30606,31035,31035,31392,31846,32473,32928,32928,33297,33838,34294,34294,34294,34649,35240,35978,36396,36396,36396,36753,37314,37404,38360,38435,38919,39768,40927,41315,42083,42514,42996,43485,43961,44437,44894,46167,46646,47118,47153,47643,48242,49012,49520,49962,50325,50814,51921,52369,52761,53528,54482,55032,55494,55494,55494,55837,56207,56879,57325,57325,57325,57681,58240,58683,58683,59034,59569,60241,60675,60675,61038,61606,62035,63010,63495,64337,65612,66017,66765,67209,67693,68642,69481,69724,70232,71714,72488,72961,73439,73601,74010,74436,74608,75089,75164,76012,76445,77034,77644,78609,79316,79886,80318,86726,93880,94360,94841,95326,95686,96444,96799,97202,97686,98164,98643,99120,100262,101617,102093,102970,103536,104378,104793,105173,105807,106772,107334,107769,108247,108496,109176,109606,109970,110518,110957,111324,111919,112598,113045,113392,113961,114324,115280,115280,115764,116236,116236,117088,118155,118155,118559,118559,118760,119517,119517,120481,120481,120961,120961,121440,121440,121917,121917,122402,123165,123165,123643,124128,124593,124758,124758,125233,125848,126607,126715,126715,127112,127112,127570,127684,127684,128161,128207,128207,129042,129042,129477,129927,130569,131529,132072,132525,132887,133285,133971,134412,134769,135324,135768,136115,136675,137367,137798,138166,138727,138999,139960,139960,140446,140528,140528,141361,142551,142551,142965,143730,143730,144674,145175,145645,146114,146552,147314,147807,148283,148768,148877,148877,149365,149967,150724,150724,151112,151112,151564,151564,152049,152295,152295,153120,153239,153239,154154,154768,155724,156274,156715,157084,157476,158128,158570,158924,159493,159930,160286,160881,161491,161926,162299,162299,162842,163120,164086,164086,164563,164563,165395,168164,168164,168552,169317,169317,170289,170769,171243,171710,172206,172964,173440,173916,174435,174435,174929,175516,175763,176529,176529,176962,177449,177497,177497,177879,178361,178361,179197,179197,179925,180780,181569,182532,182532,182532,183077,183520,183520,183885,183885,184288,185084,185532,185532,185886,185886,186436,187760,188564,189357,189799,189799,190164,190164,190719,190766,190766,190766,191718,191815,192298,192321,193159,194368,194757,195517,198528,200403,202842,203608,204196,204644,205635,206524,207446,208154,208154,208154,209118,209118,209118,209684,210116,210116,210488,210883,211610,212046,212399,212960,213401,213756,214272,214969,215405,215756,216330,216410,216410,216410,217364,217408,217880,218037,218877,219958,221135,224527,226040,227970,228396,228922,229119,229683,230649,231090,231564,232361,232402,232402,232402,233364,233364,233927,234374,234719,235041,235839,236286,236646,237204,237635,238000,238573,239324,239759,240120,240688,241088,241088,242035,243116,243596,244076,244313,244797,245289,245770,246245,246723,247200,247682,248168,248644,249114,249613,249675,249885,250000,250113,251036,251437,251883,251918,252410,253770,253919,254362,254955,255689,255689,256652,256652,257201,257637,257999,258410,259078,259520,259883,260445,260883,261236,261800,262233,262605,265352,266399,267037,267475,267840,268395,268759,269725,270153,270515,270677,271515,272118,272600,273176,273679,274845,275292,275758,276244,276725,277198,278805,279283,279808,280286,280874,282368,283314,283794,283881,284722,285155,285603,286293,287241,287800,288236,288616,289672,291044,293088,294452,295441,297931,300035,300520,301194,301680,302165,304127,305003,306133,306896,313197,315009,319358,321771,324875,326196,326690,327157,327638,328122,329875,330486,332409,338131,339875,341235,343320,344694,352121,355554,357171,357768,359247,361915,363735,365105,366433,370926,371359,371796,373212,373794,376012,377096,378368,379277,379770,380237,380714,381195,383164,383655,383714,386326,386808,389051,391612,393410,259078,259520,259883,260445,260883,261236,261800,262233,262605,265352,266399,267037,267475,267840,268395,268759,269725,270153,270515,270677,271515,272118,272600,273176,273679,274845,275292,275758,276244,276725,277198,278805,279283,279808,280286,280874,282368,283314,283794,283881,284722,285155,285603,286293,287241,287800,288236,288616,289672,291044,293088,294452,295441,297931,300035,300520,301194,301680,302165,304127,305003,306133,306896,313197,315009,319358,321771,324875,326196,326690,327157,327638,328122,329875,330486,332409,338131,339875,341235,343320,344694,352121,355554,357171,357768,359247,361915,363735,365105,366433,370926,371359,371796,373212,373794,376012,377096,378368,379277,379770,380237,380714,381195,383164,383655,383714,386326,386808,389051,391612,393410,395087,396770,398803,417565,419169,420060,421398,423054,423526,424015,424485,424959,425248,426001,426482,426965,427453,427925,428399,428734,429493,429960,430445,430928,434015,311009,311689,312648,314396,315443,317885,318675,319798,320166,320327,321277,321845,322673,324478,325766,329996,330650,331919,332553,332968,334047,334894,335920,336597,337569,338554,338919,339274,340335,340680,341683,342035,342560,343762,344150,344847,345804,346284,347126,348156,349320,352881,354008,354486,356337,356729,357291,357959,358806,359770,360364,361369,362371,362733,363097,364166,364515,365520,365879,366544,372234,373329,373842,374687,375444,376693,377846,378834,383992,385448,385912,386363,387285,387769,388606,389474,390089,391038,392049,392410,392765,394001,394368,396084,397200,397568,397844,399313,400333,409393,414062,416610,416934,418000,418360,418676,419217,420407,420842,422172,422659,423102,431459,431927,432805,433448,434440,434815,438680,439725,440086,440519,441917,442005,442929,443395,444092,445758,446364,447354,447440,447882,448235,449252,450317,452282,453283,454517,455877,457236,457357,458573,460440,461954,462569,463930,464559,465653,467093,468296,469963,472003,472915,475285,475926,478007,478395,479353,480210,481170,481795,482760,483777,484557,485610,487136,487916,488834,490048,490810,491887,492366,493211,494449,495597,496038,499371,501041,502917,503402,504281,507686,508766,509690,510569,511357,512317,513330,514095,515336,516698,517282,518916,2317,2317,2317,2317,3085,13393,13393,13394,13685,14170,14312,16325,16326,18203,18394,25804,26205,27130,27131,27131,27132,27895,37634,37634,37634,38040,38400,38525,40444,40446,41846,42083,49810,50414,51603,51604,51605,51605,52369,62219,62220,62221,62477,62954,63010,65114,65115,66643,66837,74123,74812,75567,75568,75569,75570,76330,88642,88642,88642,88955,88955,89403,94888,95032,95326,96796,103970,103971,103971,103971,104794,114560,114561,114562,114885,115313,115916,117798,117799,120047,120366,127294,127805,128642,128643,128644,128644,129411,139208,139208,139209,139520,139960,140161,142168,142171,143612,144916,151157,151880,152847,152848,152849,152849,153605,163333,163333,163334,163680,164086,164086,167246,168882,169127,176893,177851,179577,179578,179579,179579,180319,190994,190994,190995,191289,191766,192057,193884,193886,195581,195804,203353,204074,205325,206236,206237,206237,206238,207009,216635,216635,216638,217001,217408,217614,219566,219568,221133,221249,228643,229167,230041,230042,230042,230042,230801,241280,241280,241280,241796,241797,242035,243035,243196,243436,243725,253645,253646,253647,253647,254410,260365,269395,269395,269396,269809,270153,270315,273236,273237,274640,274887,282842,283531,284288,284289,284290,284290,285053,294755,294755,294756,295167,295516,295608,298009,298010,299633,299956,306774,307724,308534,308534,308535,308535,309292,320564,320564,320565,320893,321327,321559,323400,323403,325563,325806,331432,333656,334538,334539,334539,334539,335285,345073,345073,345074,345321,345851,345888,347766,347767,349320,349479,356797,357522,358323,358323,358324,358324,359093,372364,372607,372609,372930,373373,373534,376300,376301,379317,379610,386612,387285,388092,388093,388093,388093,388835,398476,398476,398477,398853,399246,416243,417692,418094,418335,418886,420059,420060,420817,421118,421121,430612,430612,430613,430614,430614,430928,431370,431480,433969,434125,442393,442994,447123,447124,447882,463208,463208,463209,463563,463971,464011,466721,466723,468651,468897,473973,474807,479009,479930,479930,479931,479932,480676,491053,491054,491056,491397,491811,491934,493959,493960,495800,496016,507325,508366,509284,509285,509286,509286,510034,3205,16325,28114,40444,52440,65113,76681,88955,104967,117798,129641,142168,153797,167246,180497,193884,207117,219566,231278,241796,254634,273236,285274,298009,310816,323400,335554,347766,359446,376300,389170,421118,432478,466721,480805,493958,510166 };
        #endregion
        StableSort<long>.fluxsort(list.AsSpan(), (a, b) => a.CompareTo(b));

        CollectionAssert.IsOrdered(list);
    }
}


[TestFixtureSource(typeof(Sort_Generated), nameof(Sort_Generated.GenerateTests))]
class Sort_Generated
{
    public static IEnumerable GenerateTests
    {
        get
        {
            var r = new Random();
            var list = new List<Sorting.TestEvent>(1000);
            for(int i = 0; i < 1000; i++)
            {
                list.Add(new(r.Next(), i));
                yield return new TestFixtureData(new List<Sorting.TestEvent>(list)); // clone so every list has to be sorted
            }
        }
    }


    List<Sorting.TestEvent> data;
    public Sort_Generated(List<Sorting.TestEvent> data) => this.data = data;

    [Test]
    public void IsSorted()
    {
        data.SortByTime();

        CollectionAssert.IsOrdered(data, Sorting.TestEvent.Comparer.Instance, $"{data.Count} failed");
    }
}