using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarlari")]
    public float moveSpeed = 3.9f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool canMove = true;

    [Header("Savas Ayarlari (Sol Tik)")]
    public float fireRate = 0.25f;
    public GameObject bulletPrefab;
    public float mermiHasari = 8f;
    public float mermiHizi = 14f;
    private float nextFireTime;

    [Header("Alan Hasarli Yakin Vurus (Sag Tik)")]
    public float meleeDamage = 9f;
    public float meleeCooldown = 0.6f;
    public Vector2 meleeBoxSize = new Vector2(3.2f, 3.2f);
    public float meleeOffset = 1.4f;
    public float damageDelay = 0.15f;
    private float nextMeleeTime;

    [Header("Can Sistemi")]
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthBarFill;

    [Header("XP ve UI")]
    public Slider xpSlider;
    public TMP_Text levelText;
    public int toplamPuan = 0;
    public int level = 1;
    public int sonrakiSeviyeXP = 200;

    [Header("Ekonomi (Altın)")]
    public int currentGold = 0;
    public TMP_Text goldText;

    [Header("Upgrade Sistemi Bağlantısı")]
    public UpgradeManager upgradeManager;

    [Header("Game Over Paneli")]
    public GameObject gameOverPanel;

    private Animator anim;
    private SpriteRenderer sprite;
    private Collider2D playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        currentHealth = maxHealth;

        Time.timeScale = 1f;

        if (levelText != null) levelText.text = "LEVEL: " + level;
        if (xpSlider != null) { xpSlider.maxValue = sonrakiSeviyeXP; xpSlider.value = 0; }
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        UpdateGoldUI();
    }

    void Update()
    {
        if (!canMove || Time.timeScale == 0)
        {
            if (anim != null) anim.SetBool("isRunning", false);
            return;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < transform.position.x) sprite.flipX = true;
        else sprite.flipX = false;

        anim.SetBool("isRunning", moveInput.magnitude > 0);

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime) ShootAtMouse();

        if (Input.GetMouseButtonDown(1) && Time.time >= nextMeleeTime)
            StartCoroutine(MeleeAttackRoutine());
    }

    void FixedUpdate()
    {
        if (canMove && Time.timeScale != 0)
            rb.linearVelocity = moveInput * moveSpeed;
    }

    void ShootAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 direction = ((Vector2)mousePos - (Vector2)transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null) bulletRb.linearVelocity = direction * mermiHizi;

        Bullet bScript = bullet.GetComponent<Bullet>();
        if (bScript != null) bScript.damage = mermiHasari;

        nextFireTime = Time.time + fireRate;
    }

    IEnumerator MeleeAttackRoutine()
    {
        if (!canMove) yield break;

        nextMeleeTime = Time.time + meleeCooldown;
        anim.SetTrigger("MeleeAttack");

        yield return new WaitForSeconds(damageDelay);

        Vector2 attackPoint = transform.position;
        if (sprite.flipX) attackPoint += Vector2.left * meleeOffset;
        else attackPoint += Vector2.right * meleeOffset;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint, meleeBoxSize, 0f);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                // Tüm düşman türlerine tek noktadan hasar ve geri itme uygular
                IDamageable damageable = enemyCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector2 knockbackDir = (enemyCollider.transform.position - transform.position).normalized;
                    damageable.TakeDamage(meleeDamage, knockbackDir);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!canMove) return;

        currentHealth -= damage;
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;

        if (currentHealth <= 0) Die();
        else if (damage > 0) anim.SetTrigger("TakeDamage");
    }

    void Die()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("isDead");

        if (playerCollider != null) playerCollider.enabled = false;

        StartCoroutine(ShowGameOverRoutine());
    }

    IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSeconds(1.2f);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void YenidenBasla()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AnaMenuyeDon()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void AddXP(int amount)
    {
        toplamPuan += amount;
        if (xpSlider != null) xpSlider.value = toplamPuan;
        if (toplamPuan >= sonrakiSeviyeXP) LevelUp();
    }

    public void PuanEkle(int miktar) { AddXP(miktar); }

    // --- ALTIN SİSTEMİ FONKSİYONLARI ---
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            return true;
        }
        return false;
    }

    void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = "ALTIN: " + currentGold;
    }

    void LevelUp()
    {
        level++;
        toplamPuan = 0;
        sonrakiSeviyeXP = Mathf.RoundToInt(sonrakiSeviyeXP * 1.3f);

        if (levelText != null) levelText.text = "LEVEL: " + level;
        if (xpSlider != null) { xpSlider.maxValue = sonrakiSeviyeXP; xpSlider.value = 0; }

        // Rastgele kartları seçip menüyü açar
        if (upgradeManager != null)
        {
            upgradeManager.MenuyuAcVeKartlariSec();
        }
    }
}