using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public GameObject powerBar;
    public GameObject powBarBonus;
    private GameObject powerBarInst = null;
    private GameObject bonusBar = null;

    private Image filledImage; // The Image with Image.Type.Filled
    private RectTransform targetImage; // The other UI element to check against

    private float powerBarStartPos = -42.6f;

    [HideInInspector] public float powBarSpeed;
    [HideInInspector] public bool bonusZoneHit = false;
    [HideInInspector] public bool powBarPaused = false;

    float startX;
    float endX;

    void Start()
    {
        endX = (startX + GetComponent<RectTransform>().rect.width) / 2 - 8f; 
    }

    public void SpawnPowerBar()
    {
        powerBarInst = Instantiate(powerBar);
        powerBarInst.transform.SetParent(gameObject.transform);
        
        powerBarInst.transform.localPosition = new Vector3(powerBarStartPos, 0, 0);
        powerBarInst.transform.localScale = new Vector3(powerBarInst.transform.localScale.x, 1, 1);

        filledImage = powerBarInst.GetComponent<Image>();
        startX = powerBarInst.GetComponent<RectTransform>().anchoredPosition.x;
    }

    public void DestroyPowerBar()
    {
        if (powerBarInst != null && !powBarPaused)
        {
            Destroy(powerBarInst);
        }
    }

    public void SetBonusBar(float startPos, float endPos)
    {
        if (bonusBar != null)
        {
            Destroy(bonusBar);
        }

        bonusBar = Instantiate(powBarBonus);
        bonusBar.transform.SetParent(gameObject.transform);
        
        bonusBar.transform.localPosition = new Vector3(startPos, 0, 0);
        bonusBar.transform.localScale = new Vector3(endPos, 1, 1);

        targetImage = bonusBar.GetComponent<RectTransform>();
    }

    // OVERLAPPING
    void Update()
    {
        if (powerBarInst != null)
        {
            //float progress;

            //float xPosition;

            float progress = powBarSpeed;

            float xPosition = Mathf.Lerp(startX, endX, progress);

            powerBarInst.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosition, powerBarInst.GetComponent<RectTransform>().anchoredPosition.y);
            

            if (IsFilledImageOverlapping(filledImage, targetImage))
            {
                Debug.Log("The filled portion overlaps the target!");
                bonusZoneHit = true;
            }
            else
            {
                bonusZoneHit = false;
            }
        }
    }

    bool IsFilledImageOverlapping(Image fillImg, RectTransform target)
    {
        if (fillImg.type != Image.Type.Filled) return false;

        RectTransform fillRect = fillImg.rectTransform;

        // Step 1: Check standard rectangular overlap first
        if (!RectOverlaps(fillRect, target)) return false;

        // Step 2: Shrink the bounding box of the filled image based on fillAmount and direction
        Rect effectiveRect = GetEffectiveFilledRect(fillImg);

        // Step 3: Convert world/screen coordinates or do local bounds check
        return EffectiveRectOverlaps(effectiveRect, target, fillRect);
    }

    bool RectOverlaps(RectTransform rect1, RectTransform rect2)
    {
        // Get world corners for both RectTransforms
        Vector3[] corners1 = new Vector3[4];
        Vector3[] corners2 = new Vector3[4];
        rect1.GetWorldCorners(corners1);
        rect2.GetWorldCorners(corners2);

        Rect r1 = new Rect(corners1[0], corners1[2] - corners1[0]);
        Rect r2 = new Rect(corners2[0], corners2[2] - corners2[0]);

        return r1.Overlaps(r2);
    }

    Rect GetEffectiveFilledRect(Image img)
    {
        Rect rect = img.rectTransform.rect;
        float fill = img.fillAmount;

        // Example adjustment for horizontal left-to-right fill
        if (img.fillMethod == Image.FillMethod.Horizontal && img.fillOrigin == 0)
        {
            float newWidth = rect.width * fill;
            return new Rect(rect.x, rect.y, newWidth, rect.height);
        }
        // Add additional checks for Vertical / Radial if needed
        
        return rect;
    }

    bool EffectiveRectOverlaps(Rect localFillRect, RectTransform target, RectTransform fillRectTransform)
    {
        // Map local effective filled rect corners back to world space
        // Or simplify by checking if the target's world bounds fall within the active fill range
        return true; // Expand based on specific fill direction math
    }


}
