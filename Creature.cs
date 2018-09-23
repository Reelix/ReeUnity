using UnityEngine;
using UnityEngine.AI;
using ModelOutline = cakeslice.Outline; // 3D https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/outline-effect-78608

public class Creature : MonoBehaviour
{
    public int ConnectionId = 0;

    [Header("Creature Stats")]
    public string CreatureName;

    public int Health;

    public int MaxHealth;

    public int Mana;

    public int MaxMana;

    public Vector3 SpawnLocation;

    public bool isAlive = true;

    public int damage;

    public float attackRange;

    public NavMeshAgent navAgent;

    [Header("Creature Animations")]
    public GameObject modelParent; // The parent GameObject containing the bones and the base model

    public GameObject model; // The base model itself
    private Animation animationController;
    public AnimationClip moveAnimation;
    public AnimationClip idleAnimation;
    public AnimationClip attackAnimation;
    public AnimationClip deathAnimation;

    private ModelOutline modelOutline;
    // private ReeNet networkManager;

    protected virtual void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        // networkManager = GameObject.Find("ReeNet").GetComponent<ReeNet>();
    }

    protected virtual void Start()
    {
        if (modelParent != null)
        {
            if (moveAnimation == null || idleAnimation == null || attackAnimation == null || deathAnimation == null)
            {
                Debug.LogWarning("Warning: " + CreatureName + " has insufficient animations set to assign an animation controller!");
            }
            else
            {
                moveAnimation.legacy = true;
                idleAnimation.legacy = true;
                attackAnimation.legacy = true;
                deathAnimation.legacy = true;
                animationController = modelParent.AddComponent<Animation>();
                animationController.cullingType = AnimationCullingType.BasedOnRenderers;
                animationController.AddClip(moveAnimation, moveAnimation.name);
                animationController.AddClip(idleAnimation, idleAnimation.name);
                animationController.AddClip(attackAnimation, attackAnimation.name);
                animationController.AddClip(deathAnimation, deathAnimation.name);
            }

            if (GetComponent<Animator>() != null)
            {
                // Whilst I know that Animator is technically newer, the Animation component allows more generic code
                Debug.LogError("This class doesn't support the Animator - Please drag the animation clips into the appropriate slots and delete the Animator");
            }
        }

        if (model == null)
        {
            Debug.LogWarning("Please assign the model to this creature script!");
        }
        else
        {
            modelOutline = model.AddComponent<ModelOutline>();
            modelOutline.color = 0;
            modelOutline.enabled = false;
        }

        // Server only reads
        if (!Application.isEditor)
        {
            InvokeRepeating("NetCode", 0f, 0.1f);
        }
    }

    private void NetCode()
    {
        if (ConnectionId == 0)
        {
            return;
        }
        if (isAlive && IsMoving())
        {
            // networkManager.UpdateMovement(ConnectionId, transform.position);
        }
    }

    protected virtual void Update()
    {
        if (!isAlive || animationController == null)
        {
            return;
        }

        if (!IsMoving() && !animationController.IsPlaying(idleAnimation.name) && !animationController.IsPlaying(attackAnimation.name))
        {
            PlayAnimation(idleAnimation);
        }
        else if (IsMoving() && !animationController.IsPlaying(moveAnimation.name))
        {
            PlayAnimation(moveAnimation);
        }
    }

    public void PlayAnimation(string animationName, float animationSpeed = 1f)
    {
        if (animationController[animationName] == null)
        {
            Debug.LogWarning("The animation " + animationName + " does not exist as part of the animation controller!");
            return;
        }
        animationController[animationName].speed = animationSpeed;
        animationController.CrossFade(animationName);
    }

    public void PlayAnimation(AnimationClip animationClip, float animationSpeed = 1f)
    {
        if (animationController[animationClip.name] == null)
        {
            Debug.LogWarning("The animation " + animationClip.name + " does not exist as part of the animation controller!");
            return;
        }
        animationController[animationClip.name].speed = animationSpeed;
        animationController.CrossFade(animationClip.name);
    }

    public void AlterLife(int lifeValue, Vector3 hitDirection = new Vector3())
    {
        Health += lifeValue;
        if (Health <= 0)
        {
            Die(hitDirection);
        }
    }

    protected virtual void Die(Vector3 hitDirection = new Vector3())
    {
        Debug.Log("I have died!");
        isAlive = false;
        animationController.CrossFade(deathAnimation.name);
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    public void LookAt(Vector3 toLook)
    {
        toLook.y = 0; // 0 -> Don't want to tilt up and down :p
        transform.LookAt(toLook);
    }

    public void StopMoving()
    {
        if (navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
        }
        else
        {
            Debug.LogError(name + " is not on a navMesh and will be deleted!");
            DestroyGameObject();
        }
    }

    public void SetDestination(Vector3 destination)
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(destination);
    }

    public bool IsMoving()
    {
        return (navAgent.velocity.magnitude > 0f);
    }

    public void ToggleOutline()
    {
        if (modelOutline != null)
        {
            modelOutline.enabled = !modelOutline.enabled;
        }
        else
        {
            Debug.LogError("You tried to outline a creature that has no outline!");
        }
    }

    public void Spawn()
    {
        navAgent.enabled = false;
        transform.position = SpawnLocation;
        navAgent.enabled = true;
    }

    /*

    Could never get this to work properly...

    public List<object> GetNearbyObjects(float distance, Type someType)
    {
        string typeString = someType.ToString();
        List<Collider> colliders = Physics.OverlapSphere(transform.position, distance).ToList();
        colliders = colliders.Where(x => x.gameObject.GetComponent(Type.GetType(typeString)) != null).ToList();
        return colliders.Select(x => (object)x.gameObject).ToList();
    }*/
}
