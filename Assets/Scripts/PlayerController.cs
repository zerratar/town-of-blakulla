using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region secret fields

    private Animator animator;
    private Vector3 oldPosition;
    private float currentWalkTime = 0f;
    private float walkTime = 5f;

    private int currentWaypointIndex = 0;
    private float walkAnimationSpeed = 0.6f;
    private SkinnedMeshRenderer modelRenderer;

    private Camera gameCamera;
    private Vector3 nameTagNearOffset;
    private Vector3 nameTagFarOffset;
    private WaypointCamera wpCamera;

    private DateTime deathAnimationStart;
    private Transform labelTransform;

    private float labelOffsetY = 32f;

    #endregion

    public HouseController House;

    public Player PlayerData;

    public bool Lynched = false;

    public Color Color = UnityEngine.Color.white;

    private bool moveToNextWaypoint = false;

    public TextMeshProUGUI NameTag;

    public NavMeshAgent agent;

    public bool IsAssigned => !string.IsNullOrEmpty(PlayerData.AssignedUsername);

    public bool WaypointsEnabled = false;

    public Transform DeathAnimationPosition;

    public string PlayerName => this.PlayerData?.UsernameOrName;

    public Role Role => this.PlayerData?.AssignedRole;

    public bool IsDeathAnimationOver =>
        DateTime.UtcNow - this.deathAnimationStart > TimeSpan.FromSeconds(5);


    public void MoveToNextWaypoint()
    {
        this.WaypointsEnabled = true;
        this.oldPosition = this.transform.position;

        this.moveToNextWaypoint = true;
        this.currentWaypointIndex = GetNextWaypointIndex();
        //var oldRotation = this.transform.eulerAngles;

        var wp = this.House.GetWaypoint(this.currentWaypointIndex);
        this.transform.LookAt(wp);
        var rotation = this.transform.eulerAngles;
        this.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);

        if (this.animator) this.animator.SetFloat("Forward", walkAnimationSpeed);
    }

    public void Leave()
    {
        PlayerData.Reset();
        NameTag.color = Color.white;
        NameTag.text = "!town join";
    }

    public PlayerController Assign(string username, Color color, Role role)
    {
        PlayerData.AssignedUsername = username;
        PlayerData.AssignedRole = role;
        PlayerData.Color = color;
        NameTag.color = color;
        NameTag.text = username;
        return this;
    }

    public void NavigateTo(Vector3 destination)
    {
        this.agent.enabled = true;
        this.agent.isStopped = false;
        this.agent.destination = destination;
        if (this.animator) this.animator.SetFloat("Forward", walkAnimationSpeed);
    }

    public void DisableNavigation()
    {
        this.agent.enabled = false;
        if (this.animator) this.animator.SetFloat("Forward", 0);
    }

    public bool HasReached(Vector3 position)
    {
        var pos = this.transform.position;
        var dist = Vector3.Distance(pos, position);
        return dist < 0.21f;
    }

    public void ResetPosition()
    {
        this.transform.position = this.House.GetWaypoint(0);
    }

    public void PlayDeathAnimation()
    {
        Debug.Log("Play Death Animation");

        this.DisableNavigation();

        if (DeathAnimationPosition)
        {
            this.transform.position = DeathAnimationPosition.position;
            this.transform.rotation = DeathAnimationPosition.rotation;
        }

        this.animator.SetBool("Death_Hanging", true);
        this.deathAnimationStart = DateTime.UtcNow;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerData = Player.Random();
        this.gameCamera = GameObject.FindObjectOfType<Camera>();
        this.wpCamera = this.gameCamera.GetComponent<WaypointCamera>();
        if (!this.agent) this.agent = this.gameObject.GetComponent<NavMeshAgent>();

        if (this.agent)
        {
            this.DisableNavigation();
        }

        if (NameTag)
        {
            //var screenPosition = gameCamera.WorldToScreenPoint(GetLabelPosition());
            this.nameTagFarOffset = Vector3.zero; // NameTag.rectTransform.position - screenPosition;
            this.nameTagNearOffset = Vector3.zero;// NameTag.rectTransform.position - screenPosition;
        }

        this.transform.position = new Vector3(0, -999, 0);
        this.animator = this.GetComponent<Animator>();
        if (this.animator)
        {
            this.animator.SetFloat("Offset", UnityEngine.Random.value);
        }

        for (var i = 0; i < this.transform.childCount; ++i)
        {
            var c = this.transform.GetChild(i);
            if (!c.gameObject.activeInHierarchy)
            {
                continue;
            }

            this.modelRenderer = c.GetComponent<SkinnedMeshRenderer>();
            break;
        }

    }

    private Vector3 GetLabelPosition()
    {
        if (!labelTransform)
        {
            labelTransform = this.transform
                .Find("Root")
                .Find("Hips")
                .Find("Spine_01")
                .Find("Spine_02")
                .Find("Spine_03")
                .Find("Neck")
                .Find("Head")
                .Find("Eyebrows");
        }

        return (labelTransform ? labelTransform.position : this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {

        if (!this.House) return;

        if (this.NameTag && this.gameCamera)
        {
            var screenPosition = gameCamera.WorldToScreenPoint(GetLabelPosition());

            var isCameraNear = wpCamera.CurrentWaypointIndex == 0;

            var offset = isCameraNear
                ? Vector3.Lerp(nameTagFarOffset, nameTagNearOffset, wpCamera.WaypointProgress)
                : Vector3.Lerp(nameTagNearOffset, nameTagFarOffset, wpCamera.WaypointProgress);

            this.NameTag.rectTransform.position = screenPosition + offset + new Vector3(0, labelOffsetY, 0);
        }

        if (this.modelRenderer)
        {
            this.modelRenderer.material.SetColor("_AlbedoColor", this.Color);
        }

        if (Lynched)
        {
            return;
        }

        if (agent && agent.enabled)
        {
            if (!agent.isStopped && agent.hasPath && agent.destination != Vector3.zero)
            {
                if (Vector3.Distance(this.transform.position, agent.destination) <= 0.05f)
                {
                    DisableNavigation();
                    return;
                }
                return;
            }
        }

        if (!WaypointsEnabled || this.currentWaypointIndex == -1)
        {
            return;
        }

        if (this.moveToNextWaypoint)
        {
            currentWalkTime += Time.deltaTime;
            var procent = currentWalkTime / walkTime;
            if (currentWalkTime >= this.walkTime) procent = 1f;

            var target = this.House.GetWaypoint(this.currentWaypointIndex);
            this.transform.position = Vector3.Lerp(
                oldPosition,
                target,
                procent
            );

            if (OnWaypoint(this.currentWaypointIndex))
            {
                this.ArrivedAtWaypoint();
            }
        }
    }

    private void ArrivedAtWaypoint()
    {
        // yay!         
        this.currentWalkTime = 0f;
        this.transform.position = this.House.GetWaypoint(this.currentWaypointIndex);
        this.moveToNextWaypoint = false;
        if (this.animator) this.animator.SetFloat("Forward", 0);
    }

    private int GetNextWaypointIndex()
    {
        if (!this.House) return -1;
        return (this.currentWaypointIndex + 1) % this.House.WaypointCount;
    }

    private bool OnWaypoint(int number)
    {
        if (!this.House) return false;
        var target = this.House.GetWaypoint(number);
        return this.HasReached(target);
    }



}