using UnityEngine;
using UnityEngine.EventSystems;

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
		#if UNITY_IOS
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
			{
				if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
					return;
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				if (Physics.Raycast(ray, out hit))
				{
					if (hit.transform.gameObject == this.gameObject)
					{
						performClick();
					}
				}
			}
		#endif
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			ps.Emit(20);
			gameObject.SetActive(false);
		}
	}

	void OnMouseDown() {
		#if !UNITY_EDITOR
			return;
		#endif

		if (checkObject())
			return;
		performClick();	
	}

	private void performClick()
	{
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
