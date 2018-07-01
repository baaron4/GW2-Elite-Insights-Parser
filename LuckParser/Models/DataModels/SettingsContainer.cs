using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Properties;

namespace LuckParser.Models.DataModels
{
    class SettingsContainer
    {
        public readonly bool DPSGraphTotals;
        public readonly bool PlayerGraphTotals;
        public readonly bool PlayerGraphBoss;
        public readonly bool PlayerBoonsUniversal;
        public readonly bool PlayerBoonsImpProf;
        public readonly bool PlayerBoonsAllProf;
        public readonly bool PlayerRot;
        public readonly bool PlayerRotIcons;
        public readonly bool EventList;
        public readonly bool BossSummary;
        public readonly bool SimpleRotation;
        public readonly bool ShowAutos;
        public readonly bool LargeRotIcons;
        public readonly bool ShowEstimates;
        public readonly bool ParsePhases;
        public readonly bool Show30s;
        public readonly bool Show10s;
        public readonly bool LightTheme;
        public readonly bool ParseCombatReplay;

        public SettingsContainer(Settings settings)
        {
            this.DPSGraphTotals = settings.DPSGraphTotals;
            this.PlayerGraphTotals = settings.PlayerGraphTotals;
            this.PlayerGraphBoss = settings.PlayerGraphBoss;
            this.PlayerBoonsUniversal = settings.PlayerBoonsUniversal;
            this.PlayerBoonsImpProf = settings.PlayerBoonsImpProf;
            this.PlayerBoonsAllProf = settings.PlayerBoonsAllProf;
            this.PlayerRot = settings.PlayerRot;
            this.PlayerRotIcons = settings.PlayerRotIcons;
            this.EventList = settings.EventList;
            this.BossSummary = settings.BossSummary;
            this.SimpleRotation = settings.SimpleRotation;
            this.ShowAutos = settings.ShowAutos;
            this.LargeRotIcons = settings.LargeRotIcons;
            this.ShowEstimates = settings.ShowEstimates;
            this.ParsePhases = settings.ParsePhases;
            this.Show10s = settings.Show10s;
            this.Show30s = settings.Show30s;
            this.LightTheme = settings.LightTheme;
            this.ParseCombatReplay = settings.ParseCombatReplay;
        }
    }
}
