using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemQuickSelect : MonoBehaviour
{
    public bool itemInQS = false;
    private bool useCircIncreasing = false;

    [SerializeField] private float useCircleIncreaseAmount = 0.1f;

    //White circle goes around green circle gradually. When full, use item.
    [SerializeField] private Image useCircle;
    [SerializeField] private Sprite blankImg;

    // Start is called before the first frame update
    void Start()
    {
        useCircle.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (itemInQS)
        {
            if (useCircIncreasing)
            {
                if (useCircle.fillAmount >= 1)
                {
                    // Use selected item
                    Debug.Log("Use item!");
                    useCircle.fillAmount = 0;
                }
                else
                {
                    useCircle.fillAmount += useCircleIncreaseAmount * Time.deltaTime;
                }
            }
            else
            {
                if (useCircle.fillAmount > 0)
                {
                    useCircle.fillAmount -= useCircleIncreaseAmount * Time.deltaTime;
                }
            }
        }
    }

    public void StartFillUseCircle()
    {
        useCircIncreasing = true;
    }

    public void StopFillUseCircle()
    {
        useCircIncreasing = false;
    }

    public void MakeImageBlank()
    {
        GetComponent<Image>().sprite = blankImg;
    }
}
