using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] GameObject modelLevel1, modelLevel2, modelLevel3;
    [SerializeField] int level = 1;
    HeroMovement heroMovement; // tham chiếu tới script di chuyển

    private void Start()
    {
        heroMovement = GetComponent<HeroMovement>();
        modelLevel1.SetActive(true);
    }

    public void UpgradeHero(int newLevel)
    {
        if (newLevel < 2 || newLevel > 3 || newLevel == level) return;

        StartCoroutine(PlayUpgradeAnimation(level, newLevel));
        level = newLevel;
    }

    private IEnumerator PlayUpgradeAnimation(int oldLevel, int newLevel)
    {
        // Khi bắt đầu nâng cấp -> đứng yên
        if (heroMovement != null)
            heroMovement.SetSpeed(0);

        GameObject oldModel = GetModelByLevel(oldLevel);
        GameObject newModel = GetModelByLevel(newLevel);

        if (oldModel != null)
        {
            Animator anim = oldModel.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.SetBool("Upgrade", true);
                if (CameraShake.Instance != null)
                    StartCoroutine(CameraShake.Instance.Shake(1f, 0.1f));
                yield return new WaitForSeconds(1.5f);
            }

            oldModel.SetActive(false);
        }

        if (newModel != null)
        {
            newModel.SetActive(true);
        }

        // Sau khi nâng cấp xong -> set tốc độ theo level mới
        if (heroMovement != null)
            heroMovement.SetSpeed(newLevel); // level 1 = speed 1, level 2 = speed 2, ...
    }

    private GameObject GetModelByLevel(int level)
    {
        switch (level)
        {
            case 1: return modelLevel1;
            case 2: return modelLevel2;
            case 3: return modelLevel3;
            default: return null;
        }
    }
}
