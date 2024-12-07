using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace NoWaterElectricity
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(NoWaterElectricity)}").SetShowsErrorsInUI(false);
        public static bool buildingNeedWater => m_Setting.buildingNeedWater;
        public static bool buildingNeedElectricity => m_Setting.buildingNeedElectricity;

        private static Setting m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info("Mod is loaded");
            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));
            AssetDatabase.global.LoadSettings(nameof(NoWaterElectricity), m_Setting, new Setting(this));
            updateSystem.UpdateBefore<DisableWaterConsumptionSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateBefore<DisableElectricityConsumptionSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }

        public static bool getBuildingNeedWater()
        {
            return m_Setting.buildingNeedWater;
        }

        public static bool getBuildingNeedElectricity()
        {
            return m_Setting.buildingNeedElectricity;
        }
    }

}
