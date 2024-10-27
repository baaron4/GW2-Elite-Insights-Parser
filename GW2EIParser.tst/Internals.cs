using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GW2EIEvtcParser.ParsedData.tst;

class Internals
{
    public class TestEvent(long t, int order) : AbstractTimeCombatEvent(t)
    {
        public int Order = order;

        public override string ToString() => $"({Time} {Order})";
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
    public void AAAA()
    {
        //TODO(Rennorb) make more minimal
        var list = new List<AbstractBuffEvent>() {
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

        };

        list.SortByTime();
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
            var list = new List<Internals.TestEvent>(1000);
            for(int i = 0; i < 1000; i++)
            {
                list.Add(new(r.Next(), i));
                yield return new TestFixtureData(new List<Internals.TestEvent>(list)); // clone so every list has to be sorted
            }
        }
    }

    public class Order : IComparer
    {
        public static readonly Order Instance = new();
        public int Compare(object x, object y) => (int)(((Internals.TestEvent)x).Time - ((Internals.TestEvent)y).Time);
    }


    List<Internals.TestEvent> data;
    public Sort_Generated(List<Internals.TestEvent> data) => this.data = data;

    [Test]
    public void IsSorted()
    {
        data.SortByTime();

        CollectionAssert.IsOrdered(data, Order.Instance, $"{data.Count} failed");
    }
}

[TestFixtureSource(typeof(Sort_Generated_Subtypes), nameof(Sort_Generated_Subtypes.GenerateTests))]
class Sort_Generated_Subtypes
{
    public static IEnumerable GenerateTests
    {
        get
        {
            var r = new Random();
            var list = new List<AbstractBuffEvent>(1000);
            for(int i = 0; i < 1000; i++)
            {
                list.Add(r.NextSingle() < 0.5 ? new BuffApplyEvent(r.Next()) : new BuffRemoveSingleEvent(r.Next()));
                yield return new TestFixtureData(new List<AbstractBuffEvent>(list)); // clone so every list has to be sorted
            }
        }
    }

    public class Order : IComparer
    {
        public static readonly Order Instance = new();
        public int Compare(object x, object y) => (int)(((AbstractBuffEvent)x).Time - ((AbstractBuffEvent)y).Time);
    }


    List<AbstractBuffEvent> data;
    public Sort_Generated_Subtypes(List<AbstractBuffEvent> data) => this.data = data;

    [Test]
    public void IsSorted()
    {
        data.SortByTime();

        CollectionAssert.IsOrdered(data, Order.Instance, $"{data.Count} failed");
    }
}
