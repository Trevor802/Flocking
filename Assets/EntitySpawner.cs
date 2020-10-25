using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class EntitySpawner : MonoBehaviour
{
	[SerializeField] private GameObject m_prefab;
	[SerializeField] private uint m_count = 10;
	public uint Seed;
	private Entity m_entityPrefab;
    public float SPN_RADIUS = 10f;
	private EntityManager m_manager;
	public GameObject Volume;
	private Random m_rand;
	void Start()
	{
		m_rand = new Random();
		m_rand.InitState(Seed);
		m_manager = World.DefaultGameObjectInjectionWorld.EntityManager;
		var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
		m_entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_prefab, settings);
		CreateRandomAgents(m_count);
		Volume.transform.localScale = Vector3.one * SPN_RADIUS;
	}

	private void CreateRandomAgents(uint count){
		while(count-- > 0){
			float3 position = m_rand.NextFloat3(-SPN_RADIUS, SPN_RADIUS);
			CreateEntity(position);
		}
	}

	private void CreateEntity(float3 pos){
		var entity = m_manager.Instantiate(m_entityPrefab);
		m_manager.SetComponentData(entity, new Translation{
			Value = pos
		});
		float3 dir = m_rand.NextFloat3Direction();
		m_manager.SetComponentData(entity, new MoveDirection{
			Value = dir,
		});
	}

}
