using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    public bool isEnemyHP;
    [SerializeField] private Image healthFill;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] private Vector2 screenOffset;
    private IHealth target;
    private Camera cam;

    public void Init(IHealth targetRef)
    {
        target = targetRef;
        cam = Camera.main;
        healthText.gameObject.SetActive(!isEnemyHP);
        UpdateHealth();
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (target == null || cam == null) return;

        Vector3 headPos = target.GetHeadPosition();
        Vector3 screenPos = cam.WorldToScreenPoint(headPos);

        if(isEnemyHP) screenPos += new Vector3(screenOffset.x, screenOffset.y, 0f);
        transform.position = screenPos;
    }

    public void UpdateHealth()
    {
        if (healthFill != null && target != null)
            healthFill.fillAmount = target.CurrentHealth / target.MaxHealth;

        healthText.text = target.CurrentHealth.ToString();
    }
}
public interface IHealth
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    Vector3 GetHeadPosition();
}
