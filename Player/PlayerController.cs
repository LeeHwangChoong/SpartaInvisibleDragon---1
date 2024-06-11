using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;
    public Animator animator;
    private GameObject equippedItem;
    private EquipTool equippedTool;

    [Header("Movement")]
    private float moveSpeed = 5f;
    private float baseMoveSpeed;
    private float jumpPower = 135;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;
    
    [Header("Look")]
    public Transform cameraContainer;
    public CinemachineVirtualCamera backViewCam;
    private float minXLook = -85;
    private float maxXLook = 85;
    private float camCurXRot;
    private float lookSensitivity = 0.1f;
    private Vector2 mouseDelta;
    public bool canLook = true;

    [Header("QuilkSlot")]


    public Action inventory;
    public Action crafting;
    public Action building;

    private bool isInventoryOpen = false; //Ã¢ ÄÑÁ®ÀÖÀ»¶© °ø°Ý¸øÇÏ°Ô    
    private bool isBoosted = false;
    private bool Punching;
    private float PunchRate = 0.8f;

    private Status playerStatus;
    private PlayerCondition playerCondition;

    private Coroutine footstepCoroutine; // ¹ßÀÚ±¹ ¼Ò¸®

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerStatus = GetComponent<Status>();
        playerCondition = GetComponent<PlayerCondition>();
        baseMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    private void Move()
    {
        float currentSpeed = isBoosted ? baseMoveSpeed * 1.5f : baseMoveSpeed;

        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rigidbody.velocity.y;

        rigidbody.velocity = dir;

        animator.SetFloat("MoveX", curMovementInput.x);
        animator.SetFloat("MoveY", curMovementInput.y);

        if (curMovementInput != Vector2.zero && footstepCoroutine == null)
        {
            // ÇÃ·¹ÀÌ¾î°¡ ¿òÁ÷ÀÌ±â ½ÃÀÛÇÏ¸é ¹ßÀÚ±¹ ¼Ò¸® ÄÚ·çÆ¾ ½ÃÀÛ
            footstepCoroutine = StartCoroutine(PlayFootstepSound());
        }
        else if (curMovementInput == Vector2.zero && footstepCoroutine != null)
        {
            // ÇÃ·¹ÀÌ¾î°¡ ¸ØÃß¸é ¹ßÀÚ±¹ ¼Ò¸® ÄÚ·çÆ¾ ÁßÁö
            StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }
    }

    private IEnumerator PlayFootstepSound()
    {
        while (true)
        {
            if (IsGrounded() && curMovementInput != Vector2.zero)
            {
                SoundManager.Instance.PlayWalkSound();
                float footstepInterval = playerCondition.isBoosted ? 0.42f : 0.64f; //
                yield return new WaitForSeconds(footstepInterval); 
            }
            else
            {
                yield return null; // ÇÃ·¹ÀÌ¾î°¡ ¿òÁ÷ÀÌÁö ¾ÊÀ¸¸é ´ÙÀ½ ÇÁ·¹ÀÓ±îÁö ´ë±â
            }
        }
    }

    private void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0); // ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½È£ï¿½ï¿½ ï¿½Ý´ï¿½ï¿?ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ - ï¿½ï¿½È£

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!playerCondition.isAlive) return;

        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!playerCondition.isAlive) return;

        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!playerCondition.isAlive) return;

        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            animator.SetTrigger("Jump");
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        bool grounded = Physics.BoxCast(transform.position + Vector3.up * 0.1f, new Vector3(0.1f, 0.01f, 0.1f), Vector3.down, Quaternion.identity, 0.1f);
        animator.SetBool("IsGrounded", grounded);
        return grounded;
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            //ToggleCursor();
            
        }
    }

    public void OnCrafting(InputAction.CallbackContext context)
    {
        if (!playerCondition.isAlive) return;

        if (context.phase == InputActionPhase.Started)
        {
            crafting?.Invoke();            
        }
    }

    public void OnBuilding(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            building?.Invoke();
        }
    }

    public void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
        isInventoryOpen = toggle;
        //isInventoryOpen = !toggle;
    }

    public void Boost(float duration)
    {
        if (!playerCondition.isAlive) return;

        StartCoroutine(BuffActiveCoroutine(EBuffType.BOOST, duration));
    }   

    public void Invincibility(float duration)
    {
        if (!playerCondition.isAlive) return;

        StartCoroutine(BuffActiveCoroutine(EBuffType.INVINCIBILITY, duration));
    }

    public void AntiCold(float duration)
    {
        if (!playerCondition.isAlive) return;

        StartCoroutine(BuffActiveCoroutine(EBuffType.ANTICOLD, duration));
    }

    public void ApplyBuff(EBuffType buffType, float duration)
    {
        StartCoroutine(BuffActiveCoroutine(buffType, duration));
    }

    private IEnumerator BuffActiveCoroutine(EBuffType buffType, float duration)
    {
        CharacterManager.Instance.Player.condition.ApplyBuff(buffType, duration);

        switch (buffType)
        {
            case EBuffType.BOOST:    
                animator.SetBool("IsBoosted", true);
                break;
            case EBuffType.INVINCIBILITY:
                CharacterManager.Instance.Player.condition.CanInvincibility(true);
                break;
            case EBuffType.ANTICOLD:
                break;
        }

        yield return new WaitForSecondsRealtime(duration);
        
        switch (buffType)
        {
            case EBuffType.BOOST:   
                animator.SetBool("IsBoosted", false);
                break;
            case EBuffType.INVINCIBILITY:
                CharacterManager.Instance.Player.condition.CanInvincibility(false);
                break;
            case EBuffType.ANTICOLD:
                break;
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        float delta = context.ReadValue<Vector2>().y;
        if (delta > 0)
        {
            backViewCam.Priority = 9;
        }
        else if(delta < 0)
        {
            backViewCam.Priority = 11;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!playerCondition.isAlive ) return;

        if (context.phase == InputActionPhase.Started && !isInventoryOpen) // ÀÎº¥Åä¸® Ã¢ÀÌ ¿­·Á ÀÖÁö ¾ÊÀ» ¶§¸¸ °ø°Ý
        {            
            if (equippedTool == null)
            {
                animator.SetTrigger("Punch");
                Punch();                
            }
            else
            {                
                // ¹«±â Å¸ÀÔ¿¡ µû¶ó ¾Ö´Ï¸ÞÀÌ¼Ç Æ®¸®°Å ¼³Á¤
                if (equippedTool.weaponType == EWeaponType.STAB)
                {
                    animator.SetTrigger("Stab");
                }
                else
                {
                    animator.SetTrigger("Swing");
                }
                equippedTool.OnAttackInput();                
            }
        }
    }
        
    private void Punch()
    {
        if (!Punching) 
        {
            Punching = true;
            SoundManager.Instance.PlayAttackSound();            
            Invoke("OnCanPunch", PunchRate);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f)) // °ø°Ý ¹üÀ§¸¦ 1.5¹ÌÅÍ·Î ¼³Á¤
            {
                IDamagable target = hit.collider.GetComponent<IDamagable>();
                if (target != null)
                {
                    int attackDamage = Mathf.RoundToInt(playerStatus.attackPower.Value);
                    target.TakePhysicalDamage(attackDamage); // ÇÃ·¹ÀÌ¾îÀÇ °ø°Ý·Â Àü´Þ
                }
            }
        }                
    }

    private void OnCanPunch()
    {
        Punching = false;
    }

    public void SetEquippedItem(GameObject item)
    {
        if (equippedItem != null)
        {
            Destroy(equippedItem);
        }
        equippedItem = item;
        equippedTool = item.GetComponent<EquipTool>();
    }

    public void UnEquipCurrentItem()
    {
        if (equippedItem != null)
        {
            EquipTool equipTool = equippedItem.GetComponent<EquipTool>();
            if (equipTool != null)
            {
                CharacterManager.Instance.Player.status.UnequipItem(equipTool);
            }

            Destroy(equippedItem);
            equippedItem = null;
            equippedTool = null;
        }
    }

}
