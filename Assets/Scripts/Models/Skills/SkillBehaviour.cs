using UnityEngine;
using UniRx;
public class SkillBehaviour : MonoBehaviour {

	public Skill Skill;

	private ParticleSystem ps;
	private GameObject player;
	public GameObject skillDialog;

	void Start () {
		player = GameObject.Find("ThirdPersonController");
		ps = player.GetComponent<ParticleSystem>();
	}

	private Vector3 getPlayerPosition() {
		return player.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			ps.Emit(20);
			gameObject.SetActive(false);
		}
	}

	void OnMouseDown() {
		if (checkObject())
			return;
			
		var mangaer = skillDialog.GetComponent<SkillDialogManager>();
		mangaer.Skill = Skill;
		mangaer.skillObject = gameObject;
		skillDialog.SetActive(true);
	}

	private bool checkObject() 
	{
		#if UNITY_EDITOR
			return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
		#else
			return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
		#endif
	}

	public void pickup() {
		GetComponent<Rigidbody>().AddForce((getPlayerPosition() - transform.position).normalized * 150, ForceMode.Impulse);
		RestClient.pickupSkill(PlayerPrefs.GetString("token", ""), Skill.skillId.ToString())
			.Subscribe(
				x => { afterPickup(); },
				e => Debug.Log(e)
			);
	}

	private void afterPickup()
	{
		ProfileRepository.Instance.LoadProfile().skills.Add(Skill);
		SkillCache.Instance.RemoveItem(Skill);
	}
}
