using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class EnemiesManager : MonoBehaviour
{
	public TextAsset XMLFile;
	public GameObject enemyPrefab;

	private EnemiesRoot enemiesXML;

	public class EnemiesRoot
	{
		public List<EnemyXML> Enemies = new List<EnemyXML>();

		public EnemyXML GetEnemyByName(string enemyName)
		{
			foreach(EnemyXML enemy in Enemies)
			{
				if (enemy.name == enemyName)
				{
					return enemy;
				}
			}
			return null;
		}
	}

	public class EnemyXML
	{
		[XmlAttribute]
		public string name;

		public Vector2 Position;
		public float Speed;
		public bool FacingRight;
		public string Sentence;
		public ChickenType MyChickenType;

		public GameObject InstantiateObject(GameObject enemyPrefab)
		{
			GameObject obj = Instantiate(enemyPrefab, new Vector3(Position.x, Position.y, 0), Quaternion.identity);
			obj.name = name;

			Enemy enemy = obj.GetComponent<Enemy>();
			enemy.speed = Speed;
			enemy.facingRight = FacingRight;
			enemy.sentence = Sentence;
			enemy.chickenType = MyChickenType;

			return obj;
		}
	}

	private void Start()
	{
		if(XMLFile != null)
			LoadXml();
	}

	void LoadXml()
	{
		XmlSerializer deserializer = new XmlSerializer(typeof(EnemiesRoot));
		StringReader reader = new StringReader(XMLFile.text);
		enemiesXML = (EnemiesRoot)deserializer.Deserialize(reader);

		foreach(EnemyXML enemy in enemiesXML.Enemies)
		{
			enemy.InstantiateObject(enemyPrefab);
		}
	}
}
