using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class EntitySpawner : MonoBehaviour
{
	[SerializeField] private GameObject m_prefab;
	[SerializeField] private int m_count = 10;
	[SerializeField] private float m_speed = 5f;
	private Entity m_entityPrefab;
	private EntityManager m_manager;
	private Random m_rand;
	void Start()
	{
		m_rand = new Random();
		m_rand.InitState();
		m_manager = World.DefaultGameObjectInjectionWorld.EntityManager;
		var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
		m_entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_prefab, settings);
		CreateRandomAgents(m_count);
	}

	private void CreateRandomAgents(int count){
		while(count-- > 0){
			float3 position = m_rand.NextFloat3(-5f, 5f);
			CreateEntity(position);
		}
	}

	private void CreateEntity(float3 pos){
		var entity = m_manager.Instantiate(m_entityPrefab);
		m_manager.SetComponentData(entity, new Translation{
			Value = pos
		});
		quaternion q = m_rand.NextQuaternionRotation();
		m_manager.SetComponentData(entity, new Rotation{
			Value = q
		});
		float3 dir = math.mul(q, new float3(0, 1, 0));
		m_manager.SetComponentData(entity, new MoveDirection{
			Direction = dir,
			Speed = m_speed
		});
	}

}
