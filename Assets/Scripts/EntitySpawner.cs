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
	public static EntitySpawner Instance { private set; get;}
	public uint Seed;
	private Entity m_entityPrefab;
	private EntityManager m_manager;
	public GameObject Volume;
	public Random Rand => m_rand;
	private Random m_rand;
	public PointOctree<(float3, float3)> Tree {set; get;}
	private void Awake() {
		Instance = this;
	}
	void Start()
	{
		m_rand = new Random();
		m_rand.InitState(Seed);
		m_manager = World.DefaultGameObjectInjectionWorld.EntityManager;
		GameObjectConversionSettings settings = null;
		using (var store = new BlobAssetStore()){
			settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
		}
		m_entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_prefab, settings);
		CreateRandomAgents(m_count);
		Volume.transform.localScale = Vector3.one * FlockingManager.Instance.STY_RADIUS * 2; // radius
	}

	private void CreateRandomAgents(uint count){
		while(count-- > 0){
			float3 position = m_rand.NextFloat3(-FlockingManager.Instance.STY_RADIUS, FlockingManager.Instance.STY_RADIUS);
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
		m_manager.SetComponentData(entity, new Rotation{
			Value = quaternion.LookRotation(dir, MathExtension.GetUp(dir))
		});
	}

	private void OnDrawGizmos() {
		if (Tree != null && FlockingManager.Instance.DEBUG){
			Tree.DrawAllBounds();
		}
	}
}
