using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PlayerController : MonoBehaviourPun
{
    public float speed;
    public float hitCreditTime = 10.0f;
    public bool dead = false;
    public Rigidbody rig;
    public Transform cameraTransform;
    public bool isLocalPlayer = false;
    public int id;
    public Material mat;
    public Material boxingGloveMat;
    public MeshRenderer sphereBottom;
    public MeshRenderer boxingGlove;
    public MeshRenderer sphereMinimap;
    public BoxingGloveController boxingGloveController;
    public FloatingNameTag nameTag;

    private float lastHitTime;
    private int lastHitBy;
    private List<PlayerController> spectators = new();
    private PlayerController spectateTarget;

    [HideInInspector] public Player photonPlayer;
    [HideInInspector] public int kills = 0;

    private bool isGrounded;
    private int livesLeft;

    private void Awake()
    {
        cameraTransform = Camera.main.transform.parent;
        livesLeft = GameManager.instance.playerLives;
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = photonPlayer.ActorNumber;
        boxingGloveController.bGL.id = id;
        GameManager.instance.players[GetPlayerIndex(id)] = this;
        SetColor(GetPlayerIndex(id));
        nameTag.SetName(photonPlayer.NickName);
        lastHitBy = GetPlayerIndex(id);

        if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }
        else
        {
            nameTag.Hide();
            GameUI.instance.photonView.RPC("SpawnPlayerIcon", RpcTarget.AllBuffered, GetPlayerIndex(id));
            isLocalPlayer = true;
            CameraController.instance.SetRigParent(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine || dead)
            return;
        CheckGround();
        if (isGrounded && Time.time - lastHitTime > hitCreditTime)
            lastHitBy = GetPlayerIndex(id);
        //Vector3 camForward = cameraTransform.right;
        //Vector3 camHorizontal = cameraTransform.forward;

        Vector3 controllerInput = new Vector3 (Input.GetAxis("Horizontal") * speed, 0 , Input.GetAxis("Vertical") * speed);

        Vector3 moveDirection = FlattenCameraInput(controllerInput);

        if (isGrounded && controllerInput.magnitude != 0)
            rig.AddForce(moveDirection, ForceMode.Force);
        //    rig.velocity += (camForward * x * Time.deltaTime) + (camHorizontal * z * Time.deltaTime);
        //rig.velocity = Vector3.ClampMagnitude(rig.velocity, 20f);

        if (transform.position.y < GameManager.instance.fallY)
            Respawn();
    }

    void CheckGround ()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, 1.1f))
            isGrounded = true;
        else
            isGrounded = false;
    }
    
    int GetPlayerIndex (int actorNum)
    {
        int i = System.Array.IndexOf(PhotonNetwork.PlayerList, PhotonNetwork.PlayerList.First(x => x.ActorNumber == actorNum));
        return i;
    }

    int GetPlayerActorNum (int index)
    {
        int actorNum = PhotonNetwork.PlayerList[index].ActorNumber;
        return actorNum;
    }

    Vector3 FlattenCameraInput (Vector3 input)
    {
        Quaternion flatten = Quaternion.LookRotation(-Vector3.up, cameraTransform.forward) * Quaternion.Euler(-90f, 0, 0);
        return flatten * input;
    }

    [PunRPC]
    public void GetHit(Vector3 attackerPos, float force, int attackerID)
    {
        lastHitTime = Time.time;
        lastHitBy = GetPlayerIndex(attackerID);
        //Debug.Log("Getting Hit by: " + attackerID);
        Vector3 launchDir = transform.position - attackerPos;
        rig.AddForce(launchDir * force, ForceMode.Impulse);
    }

    void Respawn ()
    {
        livesLeft -= 1;
        //Debug.Log("Killed by: " + lastHitBy);
        if (lastHitBy > -1 && StatTracker.instance != null)
        {
            StatTracker.instance.AddDeath(lastHitBy);
            if (lastHitBy != GetPlayerIndex(id))
                photonView.RPC("AddKill", GameManager.instance.GetPlayer(GetPlayerActorNum(lastHitBy)).photonPlayer, GetPlayerIndex(id));
            lastHitBy = GetPlayerIndex(id);
        }
        GameUI.instance.photonView.RPC("RemoveLife", RpcTarget.All, GetPlayerIndex(id));
        if (livesLeft <= 0)
            photonView.RPC("Die", RpcTarget.All);
        if (dead)
            return;
        rig.velocity = Vector3.zero;
        GetRespawnPoint();
    }

    void GetRespawnPoint ()
    {
        GameManager.instance.ShuffleSpawnPoints();
        Transform target = GameManager.instance.spawnPoints.First(x => !x.isCollidingWithPlayer).transform;
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    [PunRPC]
    public void AddKill (int index)
    {
        kills++;
        StatTracker.instance.AddKill(index);
    }

    [PunRPC]
    public void Die ()
    {
        dead = true;
        GameManager.instance.alivePlayers -= 1;
        GameUI.instance.RemoveIcon(GetPlayerIndex(id));

        if (photonView.IsMine)
        {
            ChangeFocusedPlayer();
            if (spectators.Count != 0)
                MoveSpectators();
        }

        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();
    }

    void SetColor (int index)
    {
        Color color = GameManager.instance.colors[index];
        mat = Instantiate(mat);
        boxingGloveMat = Instantiate(boxingGloveMat);
        boxingGloveMat.color = color;
        boxingGlove.material = boxingGloveMat;

        color.a = mat.color.a;
        mat.color = color;
        sphereBottom.material = mat;
        sphereMinimap.material = mat;
    }

    [PunRPC]
    public void ChangeFocusedPlayer()
    {
        if (GameManager.instance.alivePlayers <= 0)
            return;
        
        if (spectateTarget != null)
            spectateTarget.photonView.RPC("RemoveSpectator", spectateTarget.photonPlayer);

        spectateTarget = GameManager.instance.players.First(x => !x.dead);
        CameraController.instance.SetRigParent(spectateTarget.gameObject);
        spectateTarget.photonView.RPC("AddSpectator", spectateTarget.photonPlayer, photonPlayer.ActorNumber);
    }

    [PunRPC]
    public void AddSpectator(int actorNumber)
    {
        spectators.Add(GameManager.instance.players.First(p => p.photonPlayer.ActorNumber == actorNumber));
    }

    [PunRPC]
    public void RemoveSpectator (int actorNumber)
    {
        spectators.Remove(spectators.First(p => p.photonPlayer.ActorNumber == actorNumber));
    }

    void MoveSpectators()
    {
        foreach (PlayerController spectator in spectators)
        {
            spectator.photonView.RPC("ChangeFocusedPlayer", spectator.photonPlayer);
        }
    }
}
