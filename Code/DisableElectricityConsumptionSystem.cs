/**
* System wich disable electrecity consumption for all buildings
*/
namespace NoWaterElectricity
{
    using Colossal.Logging;
    using Game;
    using Game.Common;
    using Unity.Entities;
    using Game.Buildings;
    using Unity.Collections;
    using Game.Prefabs;

    internal sealed partial class DisableElectricityConsumptionSystem : GameSystemBase
    {
        public static ILog log = LogManager.GetLogger($"{nameof(NoWaterElectricity)}").SetShowsErrorsInUI(false);
        private EntityQuery m_Query; //To get all buildings with electricty consumer component

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Query = GetEntityQuery(
                ComponentType.ReadOnly<Building>(),
                ComponentType.ReadWrite<ElectricityConsumer>(),
                ComponentType.Exclude<Deleted>());
            RequireForUpdate(m_Query);
        }
        
        protected override void OnUpdate()
        {
            var m_buildingNeedElectrecity = Mod.getBuildingNeedElectricity();
            NativeArray<ElectricityConsumer> m_ElectrecityConsumerArray = m_Query.ToComponentDataArray<ElectricityConsumer>(Allocator.Persistent);
            NativeArray<Entity> m_EntityList = m_Query.ToEntityArray(Allocator.Persistent);

            for (int i = 0; i < m_ElectrecityConsumerArray.Length; i++)
            {
                var m_ElectrecityConsumer = m_ElectrecityConsumerArray[i];
                var m_Entity = m_EntityList[i];
                if (!m_buildingNeedElectrecity)
                {
                    if (m_ElectrecityConsumer.m_WantedConsumption != 0)
                    {
                        m_ElectrecityConsumer.m_WantedConsumption = 0;
                    }
                }
                else
                {
                    var m_prefabEntity = EntityManager.GetComponentData<PrefabRef>(m_Entity);
                    var component = EntityManager.GetComponentData<ConsumptionData>(m_prefabEntity);
                    var m_DefaultElectricityConsumption = component.m_ElectricityConsumption;
                    if(m_DefaultElectricityConsumption < 1 && m_DefaultElectricityConsumption > 0)
                    {
                        m_DefaultElectricityConsumption = 1;
                    }
                    m_ElectrecityConsumer.m_WantedConsumption = (int)m_DefaultElectricityConsumption;
                }

                m_ElectrecityConsumerArray[i] = m_ElectrecityConsumer;
            }
            m_Query.CopyFromComponentDataArray(m_ElectrecityConsumerArray);
            m_ElectrecityConsumerArray.Dispose();
            m_EntityList.Dispose();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            NativeArray<ElectricityConsumer> m_ElectrecityConsumerArray = m_Query.ToComponentDataArray<ElectricityConsumer>(Allocator.Persistent);
            NativeArray<Entity> m_EntityList = m_Query.ToEntityArray(Allocator.Persistent);
            for (int i = 0; i < m_ElectrecityConsumerArray.Length; i++)
            {
                var m_ElectrecityConsumer = m_ElectrecityConsumerArray[i];
                var m_prefabEntity = EntityManager.GetComponentData<PrefabRef>(m_EntityList[i]);
                var component = EntityManager.GetComponentData<ConsumptionData>(m_prefabEntity);
                var m_DefaultElectricityConsumption = component.m_ElectricityConsumption;
                if (m_DefaultElectricityConsumption < 1 && m_DefaultElectricityConsumption > 0)
                {
                    m_DefaultElectricityConsumption = 1;
                }
                m_ElectrecityConsumer.m_WantedConsumption = (int)m_DefaultElectricityConsumption;
            }
            m_Query.CopyFromComponentDataArray(m_ElectrecityConsumerArray);
            m_ElectrecityConsumerArray.Dispose();
            m_EntityList.Dispose();
            log.Info("Reset default electricity consumer before quiting");
        }
    }

}