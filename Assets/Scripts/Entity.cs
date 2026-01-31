using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Entity : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float jumpForce;
    [SerializeField] protected float impulseForce;
    [SerializeField] protected bool isGrounded;

    [Header("Character Settings")]
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    protected int characterDirection = -1;

    protected enum states
    {
        idle,
        walking,
        jumping,
        falling,
        attacking,
        hurt,
        alive,
        dead,
        mask1,
        mask2,
        mask3
    };

    [Header("Components")]
    [SerializeField]protected Rigidbody2D rb;
    
    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
