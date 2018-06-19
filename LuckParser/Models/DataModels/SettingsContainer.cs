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
        public readonly bool DPSGraphTotals;//0
        public readonly bool PlayerGraphTotals;//1
        public readonly bool PlayerGraphBoss;//2
        public readonly bool PlayerBoonsUniversal;//3
        public readonly bool PlayerBoonsImpProf;//4
        public readonly bool PlayerBoonsAllProf;//5
        public readonly bool PlayerRot;//6
        public readonly bool PlayerRotIcons;//7
        public readonly bool EventList;//8
        public readonly bool BossSummary;//9
        public readonly bool SimpleRotation;//10
        public readonly bool ShowAutos;//11
        public readonly bool LargeRotIcons;//12
        public readonly bool ShowEstimates;//13
        public readonly bool ParsePhases;//14
        public readonly bool Show30s;//14
        public readonly bool Show10s;//14

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
        }
    }
}
