using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    protected Animator animator;

    Vector3 cameraStartPosition;
    Vector3 cameraPostion;
    Vector3 cameraFocus;
    Transform attacker;
    Transform target;

    int attackType;

    float camMoveSpeed = 20;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        cameraStartPosition = transform.position;
        IntroCamera();
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        //BattleCamSelect();
    }

    public void IntroCamera()
    {
        int panNumber = Random.Range(0, 3);
        animator.SetInteger("BattleIntroID", (panNumber + 1));
    }

    public void BattleCamInput(Transform _attacker, Transform _target, int _attackType)
    {
        attacker = _attacker;
        target = _target;
        attackType = _attackType;
    }

    public void BattleCamReset()
    {
        attackType = 0;
    }

    void BattleCamSelect()
    {
        // Attack types: 0 = No attack, 1 = Melee, 2 = Magic, 3 = Archer
        if (attackType == 0)
        {
            NormalCam();
        }
        else if (attackType == 1)
        {
            int meleeCamSelect = Random.Range(0, 6);

            switch (meleeCamSelect)
            {
                case 0:
                case 1:
                case 2:
                    // Todo: Add counter to make sure there aren't too many non cinema cam attacks
                    break;
                case 3:
                    TrackZoomCam();
                    break;
                case 4:
                case 5:

                    break;
            }
        }
    }

    // Normal camera position
    void NormalCam()
    {

    }

    // Melee Cameras
    public void FollowCam()
    {

    }

    public void TrackZoomCam()
    {
        Vector3 midPoint = (attacker.position + target.position) / 2;

        Quaternion targetRotation = Quaternion.LookRotation(transform.position, midPoint);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
        //transform.LookAt(midPoint);

    }

    public void BehindEnemyCam()
    {

    }


    // Magic Cameras

    // Archer Cameras

    // Camera Movement
    private bool MoveCamTowardTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, camMoveSpeed * Time.deltaTime));
    }

    private bool MoveCamTowardStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, camMoveSpeed * Time.deltaTime));
    }
}
