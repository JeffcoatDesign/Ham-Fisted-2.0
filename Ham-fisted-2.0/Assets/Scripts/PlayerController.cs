using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public string nickname;
    public PlayerInput playerInput;
    public float speed;
    public float hitCreditTime = 10.0f;
    public bool inGameScene;
    public bool dead = false;
    public Rigidbody rig;
    public CinemachineVirtualCamera vCam;
    public bool isLocalPlayer = false;
    public int id;
    public Material mat;
    public Material boxingGloveMat;
    public MeshRenderer sphereBottom;
    public MeshRenderer boxingGlove;
    public MeshRenderer sphereMinimap;
    public BoxingGloveController boxingGloveController;
    public Color[] colors;

    private Vector2 movementInput;
    private float lastHitTime;
    private int lastHitBy;
    private List<PlayerController> spectators = new();
    private PlayerController spectateTarget;

    [HideInInspector] public PlayerConfig playerConfig;
    [HideInInspector] public int kills = 0;

    private bool isGrounded;
    private int livesLeft;

    public void Initialize(PlayerConfig player)
    {
        dead = false;
        playerConfig = player;
        id = playerConfig.playerIndex;
        boxingGloveController.bGL.id = id;
        vCam.gameObject.GetComponent<CinemachineInputProvider>().PlayerIndex = id;
        vCam.gameObject.layer = 6 + id;
        vCam.m_Follow = transform;
        vCam.m_LookAt = transform;
        spectators = new();
        spectateTarget = null;
        if (inGameScene)
        {
            livesLeft = GameManager.instance.playerLives;
        }
        SetColor(id);
        nickname = "Player " + (id + 1);
        lastHitBy = id;

        /*if (!photonView.IsMine)
        {
            rig.isKinematic = true;
        }
        else
        {
            nameTag.Hide();
            GameUI.instance.SpawnPlayerIcon(GetPlayerIndex(id));
            isLocalPlayer = true;
        }*/
    }

    void FixedUpdate()
    {
        if (!inGameScene || dead)
            return;
        CheckGround();
        if (isGrounded && Time.time - lastHitTime > hitCreditTime)
            lastHitBy = id;
        //Vector3 camForward = cameraTransform.right;
        //Vector3 camHorizontal = cameraTransform.forward;

        Vector3 controllerInput = new Vector3 (movementInput.x * speed, 0 , movementInput.y * speed);

        Vector3 moveDirection = FlattenCameraInput(controllerInput);

        if (isGrounded && controllerInput.magnitude != 0)
            rig.AddForce(moveDirection, ForceMode.Force);
        //    rig.velocity += (camForward * x * Time.deltaTime) + (camHorizontal * z * Time.deltaTime);
        //rig.velocity = Vector3.ClampMagnitude(rig.velocity, 20f);

        if (transform.position.y < GameManager.instance.fallY)
            Respawn();
    }

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    void CheckGround ()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, 1.1f))
            isGrounded = true;
        else
            isGrounded = false;
    }

    Vector3 FlattenCameraInput (Vector3 input)
    {
        Quaternion flatten = Quaternion.LookRotation(-Vector3.up, vCam.transform.forward) * Quaternion.Euler(-90f, 0, 0);
        return flatten * input;
    }

    public void GetHit(Vector3 attackerPos, float force, int attackerID)
    {
        lastHitTime = Time.time;
        lastHitBy = attackerID;
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
            if (lastHitBy != id)
                GameManager.instance.GetPlayer(lastHitBy).AddKill(id);
            lastHitBy = id;
        }
        GameUI.instance.RemoveLife(id);
        if (livesLeft <= 0)
            Die();
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

    public void AddKill (int index)
    {
        kills++;
        StatTracker.instance.AddKill(index);
    }

    public void Die ()
    {
        dead = true;
        GameManager.instance.alivePlayers -= 1;
        GameUI.instance.RemoveIcon(id);

        ChangeFocusedPlayer();
        if (spectators.Count != 0)
            MoveSpectators();

        GameManager.instance.CheckWinCondition();
    }

    void SetColor (int index)
    {
        Color color = colors[index];
        mat = Instantiate(mat);
        boxingGloveMat = Instantiate(boxingGloveMat);
        boxingGloveMat.color = color;
        boxingGlove.material = boxingGloveMat;

        color.a = mat.color.a;
        mat.color = color;
        sphereBottom.material = mat;
        sphereMinimap.material = mat;
    }

    public void ChangeFocusedPlayer()
    {
        if (GameManager.instance.alivePlayers <= 0)
            return;
        
        if (spectateTarget != null)
            spectateTarget.RemoveSpectator(id);

        spectateTarget = GameManager.instance.players.First(x => !x.dead);
        vCam.m_Follow = spectateTarget.transform;
        vCam.m_LookAt = spectateTarget.transform;
        spectateTarget.AddSpectator(playerConfig.playerIndex);
    }

    public void AddSpectator(int actorNumber)
    {
        spectators.Add(GameManager.instance.players.First(p => p.playerConfig.playerIndex == actorNumber));
    }

    public void RemoveSpectator (int actorNumber)
    {
        spectators.Remove(spectators.First(p => p.playerConfig.playerIndex == actorNumber));
    }

    void MoveSpectators()
    {
        foreach (PlayerController spectator in spectators)
        {
            spectator.ChangeFocusedPlayer();
        }
    }
}
