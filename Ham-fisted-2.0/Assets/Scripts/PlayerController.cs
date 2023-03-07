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
    public MeshRenderer hamsterBodyMR;
    public MeshRenderer boxingGlove;
    public MeshRenderer sphereMinimap;
    public BoxingGloveController boxingGloveController;

    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private RadialIndicator radialIndicator;

    [Header("Camera Shake")]
    [SerializeField] private float lurchThreshhold = 85.5f;
    [SerializeField] private float lurchShakeTime = 0.5f;
    [SerializeField] private float lurchShakeIntensity = 1.0f;
    [SerializeField] private float hitShakeTime = 1.0f;
    [SerializeField] private float hitShakeIntensity = 2.0f;
    private bool isShaking = false;

    [Header("Falling")]
    [SerializeField] private float fallTime = 2.0f;
    [SerializeField] private float fallOffsetAmount = 0.05f;

    [Header("Smoke Trail")]
    [SerializeField] private Transform smokeTrailPivot;
    [SerializeField] private float smokeTrailTime;
    [SerializeField] private ParticleSystemController smokeSystem;

    [Header("Stun Stars")]
    [SerializeField] private ParticleSystemController starsSystem;
    private bool isStunned = false;
    public bool IsStunned { get { return isStunned; } }

    private CinemachineOrbitalTransposer vCamCOT;
    private Vector3 vCamOffset;
    private Vector2 movementInput;
    private float lastHitTime;
    private int lastHitBy;
    private List<PlayerController> spectators = new();
    private PlayerController spectateTarget;
    private float oldVelocity = 0;

    public PlayerConfig playerConfig;
    [HideInInspector] public int kills = 0;

    private bool isGrounded;
    private bool isFallingOut = false;
    [HideInInspector] public int livesLeft;

    private void Start()
    {
        vCamCOT = vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        vCamOffset = vCamCOT.m_FollowOffset;
        starsSystem.gameObject.SetActive(false);
    }

    public void LateInitialize(PlayerConfig player)
    {
        StartCoroutine(CallInitializeAfterSeconds(player));
    }

    public void Initialize(PlayerConfig player)
    {
        radialIndicator.ResetValues();
        starsSystem.gameObject.SetActive(false);
        isStunned = false;
        dead = false;
        playerConfig = player;
        id = playerConfig.playerIndex;
        boxingGloveController.bGL.id = id;
        boxingGloveController.ClearCharge();
        vCam.gameObject.GetComponent<CinemachineInputProvider>().PlayerIndex = id;
        vCam.gameObject.layer = 6 + id;
        vCam.m_Follow = transform;
        vCam.m_LookAt = transform;
        radialIndicator.transform.parent.gameObject.layer = 6 + id;
        smokeSystem.ToggleParticles(false);
        rig.angularVelocity = Vector3.zero;
        rig.velocity = Vector3.zero;
        spectators = new();
        spectateTarget = null;
        if (inGameScene)
        {
            livesLeft = GameManager.instance.playerLives;
        }
        SetColor(id);
        nickname = "Player " + (id + 1);
        lastHitBy = id;
    }

    void FixedUpdate()
    {
        smokeTrailPivot.position = transform.position;
        if (!inGameScene || dead)
            return;

        if (transform.position.y < GameManager.instance.fallY)
            StartCoroutine(Fall());

        if (isStunned)
            return;

        CheckGround();
        CheckVelocity();
        if (isGrounded && Time.time - lastHitTime > hitCreditTime)
            lastHitBy = id;

        Vector3 controllerInput = new Vector3(movementInput.x * speed, 0, movementInput.y * speed);

        Vector3 moveDirection = FlattenCameraInput(controllerInput);

        if (isGrounded && controllerInput.magnitude != 0)
            rig.AddForce(moveDirection, ForceMode.Force);
    }

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();

    void CheckVelocity()
    {
        if (oldVelocity - rig.velocity.sqrMagnitude > lurchThreshhold)
        {
            StartCoroutine(ShakeCamera(lurchShakeIntensity, lurchShakeTime));
        }

        oldVelocity = rig.velocity.sqrMagnitude;
    }

    void CheckGround()
    {
        Ray ray = new(transform.position, Vector3.down);
        if (Physics.Raycast(ray, 1.1f))
            isGrounded = true;
        else
            isGrounded = false;
    }

    Vector3 FlattenCameraInput(Vector3 input)
    {
        Quaternion flatten = Quaternion.LookRotation(-Vector3.up, vCam.transform.forward) * Quaternion.Euler(-90f, 0, 0);
        return flatten * input;
    }

    public void GetHit(Vector3 forward, float force, int attackerID)
    {
        if (attackerID == id)
            return;
        isStunned = false;
        lastHitTime = Time.time;
        lastHitBy = attackerID;
        StartCoroutine(ShakeCamera(hitShakeIntensity, hitShakeTime));
        StartCoroutine(StartSmokeTrail(forward));
        Vector3 launchDir = (forward).normalized;
        rig.AddForce(launchDir * force, ForceMode.Impulse);
    }

    void Respawn()
    {
        isStunned = false;
        vCamCOT.m_FollowOffset = vCamOffset;
        if (livesLeft <= 0)
            Die();
        if (dead)
            return;
        rig.velocity = Vector3.zero;
        oldVelocity = rig.velocity.sqrMagnitude;
        boxingGloveController.ClearCharge();
        GetRespawnPoint();
    }

    void GetRespawnPoint()
    {
        GameManager.instance.ShuffleSpawnPoints();
        SpawnPoint spawnPoint = GameManager.instance.spawnPoints.First(x => !x.isCollidingWithPlayer);
        SetLocation(spawnPoint.transform, false);
        spawnPoint.isCollidingWithPlayer = true;
    }

    public void LeaveGame() {
        if (GameManager.instance != null)
        {
            Die();
        }
        if (GameManager.instance == null || GameManager.instance.alivePlayers >= 1)
        {
            PlayerConfigManager.instance.HandlePlayerLeave(playerConfig);
            Destroy(transform.parent.gameObject);
        }
    }

    public void SetLocation (Transform targetTransform, bool flipped)
    {
        float cRot = targetTransform.rotation.eulerAngles.y;
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.position = targetTransform.position;
        boxingGloveController.transform.rotation = Quaternion.Euler(0, cRot, 0);
        playerModel.transform.rotation = Quaternion.Euler(0, cRot, 0);
        if (flipped)
            cRot += 180;
        vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = cRot;
    }

    public void AddKO(int index)
    {
        kills++;
        StatTracker.instance.AddKO(id, index);
    }

    public void Die()
    {
        dead = true;
        GameManager.instance.alivePlayers -= 1;
        CameraManager.instance.playerGameUIs[id].RemoveIcon(id);

        ChangeFocusedPlayer();
        if (spectators.Count != 0)
            MoveSpectators();

        GameManager.instance.CheckWinCondition();
    }

    public void SetColor(int index)
    {
        Color color = ColorManager.instance.colors[index];
        hamsterBodyMR.material = ColorManager.instance.playerSkins[index];
        if (CameraManager.instance != null)
        {
            CameraManager.instance.playerGameUIs[id].SetSliderColor(color);
        }
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

    public void RemoveSpectator(int actorNumber)
    {
        spectators.Remove(spectators.First(p => p.playerConfig.playerIndex == actorNumber));
    }

    public void StartStun()
    {
        StartCoroutine(Stun());
    }

    void MoveSpectators()
    {
        if (spectators.Count == 0)
            return;
        foreach (PlayerController spectator in spectators)
        {
            spectator.Invoke("ChangeFocusedPlayer", 0.1f);
        }
    }

    IEnumerator Fall ()
    {
        if(!isFallingOut)
        {
            livesLeft -= 1;
            if (StatTracker.instance != null)
            {
                if (lastHitBy > -1)
                {
                    if (lastHitBy != id)
                        GameManager.instance.GetPlayer(lastHitBy).AddKO(id);
                    else
                        StatTracker.instance.AddSD(id);
                    lastHitBy = id;
                }
            }
            CameraManager.instance.playerGameUIs[id].RemoveLife(id);

            float fallStartTime = Time.time;
            isFallingOut = true;
            while(Time.time - fallStartTime < fallTime)
            {
                vCamCOT.m_FollowOffset += new Vector3(0, fallOffsetAmount, 0);
                yield return null;
            }
            isFallingOut = false;
            Respawn();
        }

        yield return null;
    }

    IEnumerator CallInitializeAfterSeconds(PlayerConfig player)
    {
        yield return new WaitForEndOfFrame();
        Initialize(player);
    }

    IEnumerator ShakeCamera(float intensity, float shakeTime)
    {
        if (!isShaking)
        {
            CinemachineBasicMultiChannelPerlin vCamNoise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            float startShakeTime = Time.time;
            isShaking = true;
            vCamNoise.m_AmplitudeGain = intensity;
            yield return new WaitForSeconds(shakeTime);
            vCamNoise.m_AmplitudeGain = 0.0f;
            isShaking = false;
        }
        yield return null;
    }

    IEnumerator StartSmokeTrail (Vector3 dir)
    {
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        smokeTrailPivot.rotation = rot;
        yield return new WaitForSeconds(0.1f);
        smokeSystem.ToggleParticles(true);
        yield return new WaitForSeconds(smokeTrailTime);
        smokeSystem.ToggleParticles(false);
        yield return null;
    }

    IEnumerator Stun ()
    {
        if (!isStunned)
        {
            isStunned = true;
            starsSystem.gameObject.SetActive(true);
            starsSystem.ToggleParticles(true);
            yield return new WaitForSeconds(boxingGloveController.ShieldStunTime);
            isStunned = false;
            starsSystem.gameObject.SetActive(false);
            yield return null;
        }
    }

    /*IEnumerator Rumble ()
    {
        yield return null;
    }*/
}