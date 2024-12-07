/**
* System wich disable water consumption for all buildings
*/
namespace NoWaterElectricity
{
	using Game;
	using Game.Common;
	using Unity.Entities;
    using Game.Buildings;
    using Unity.Collections;
    using Colossal.Logging;
    using Game.Prefabs;
    using System.Xml;

    internal sealed partial class DisableWaterConsumptionSystem : GameSystemBase
    {
        private EntityQuery m_Query; //To get all buildings with water consumer component
        public static ILog log = LogManager.GetLogger($"{nameof(NoWaterElectricity)}").SetShowsErrorsInUI(false);

        protected override void OnCreate()
        {
            base.OnCreate();
            m_Query = GetEntityQuery(
                ComponentType.ReadOnly<Building>(),
                ComponentType.ReadWrite<WaterConsumer>(), 
                ComponentType.Exclude<Deleted>());
            RequireForUpdate(m_Query);
        }

        protected override void OnUpdate()
        {
            var m_BuildingNeedWater = Mod.getBuildingNeedWater();
            NativeArray<WaterConsumer> m_WaterConsumerArray = m_Query.ToComponentDataArray<WaterConsumer>(Allocator.Persistent);
            NativeArray<Entity> m_EntityList = m_Query.ToEntityArray(Allocator.Persistent);

            for (int i = 0; i < m_WaterConsumerArray.Length; i++)
            {
                var  m_WaterConsumer = m_WaterConsumerArray[i];
                var m_Entity = m_EntityList[i];
                if (!m_BuildingNeedWater)
                {
                    // If water building  has not already been disabled
                    if (m_WaterConsumer.m_WantedConsumption != 0)
                    {
                        m_WaterConsumer.m_WantedConsumption = 0;
                    }
                }
                else
                {
                    var m_prefabEntity = EntityManager.GetComponentData<PrefabRef>(m_Entity);
                    var component = EntityManager.GetComponentData<ConsumptionData>(m_prefabEntity);
                    var m_DefaulWaterConsumption = component.m_WaterConsumption;
                    if (m_DefaulWaterConsumption < 1 && m_DefaulWaterConsumption > 0)
                    {
                        m_DefaulWaterConsumption = 1;
                    }
                    m_WaterConsumer.m_WantedConsumption = (int)m_DefaulWaterConsumption;
                }

                m_WaterConsumerArray[i] = m_WaterConsumer;
            }
            m_Query.CopyFromComponentDataArray(m_WaterConsumerArray);
            m_WaterConsumerArray.Dispose();
            m_EntityList.Dispose();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            NativeArray<WaterConsumer> m_WaterConsumerArray = m_Query.ToComponentDataArray<WaterConsumer>(Allocator.Persistent);
            NativeArray<Entity> m_EntityList = m_Query.ToEntityArray(Allocator.Persistent);
            for (int i = 0; i < m_WaterConsumerArray.Length; i++)
            {
                var m_WaterConsumer = m_WaterConsumerArray[i];
                var m_prefabEntity = EntityManager.GetComponentData<PrefabRef>(m_EntityList[i]);
                var component = EntityManager.GetComponentData<ConsumptionData>(m_prefabEntity);
                var m_DefaulWaterConsumption = component.m_WaterConsumption;

                if (m_DefaulWaterConsumption < 1 && m_DefaulWaterConsumption > 0)
                {
                    m_DefaulWaterConsumption = 1;
                }
                m_WaterConsumer.m_WantedConsumption = (int)m_DefaulWaterConsumption;
                m_WaterConsumerArray[i] = m_WaterConsumer;
            }
            m_Query.CopyFromComponentDataArray(m_WaterConsumerArray);
            m_WaterConsumerArray.Dispose();
            m_EntityList.Dispose();
            log.Info("Reset default water consumer before quiting");
        }
    }

}