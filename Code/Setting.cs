using System.Collections.Generic;
using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace NoWaterElectricity
{
    [FileLocation("ModsSettings/NoWaterElectricity/NoWaterElectricity.coc")]
    [SettingsUIGroupOrder(kToggleGroup)]
    [SettingsUIShowGroupName(kToggleGroup)]

    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kToggleGroup = "Configuration";


        [SettingsUISection(kSection, kToggleGroup)]
        public bool buildingNeedElectricity { get; set; } = true;

        [SettingsUISection(kSection, kToggleGroup)]
        public bool buildingNeedWater { get; set; } = true;

        [SettingsUISection(kSection, kToggleGroup)]
        public string version
        {
            get => "0.1.0";
        }

        public override void SetDefaults()
        {
        }

        public Setting(IMod mod) : base(mod)
        {
        }


    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;


        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                //Menu ui
                { m_Setting.GetSettingsLocaleID(), "No water electrecity" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kToggleGroup), "Configuration" },              
                
                //Building need electricity
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.buildingNeedElectricity)), "Building need electricity" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.buildingNeedElectricity)), $"Set false to disable building need electricity" },

                //Building need water
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.buildingNeedWater)), "Building need water" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.buildingNeedWater)), $"Set false to disable building need water" },

                //Version
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.version)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.version)), $"Current version of the mod" },

            };
        }

        public void Unload() { }

    }
}