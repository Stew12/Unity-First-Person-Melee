using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum QuickSelectType
{
    ITEMSELECT,
    SPELLSELECT
}

public class QuickSelect : MonoBehaviour
{
    [SerializeField] private QuickSelectType quickSelectType;
    private PlayerController player;

    [SerializeField] private bool isSpellQS = false;
    public bool itemInQS = false;
    private bool useCircIncreasing = false;

    [SerializeField] private float useCircleIncreaseAmount = 2f;

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
                    if (quickSelectType == QuickSelectType.ITEMSELECT)
                    {
                        // Use selected item
                        player.GetComponent<PlayerController>().playerInventory.GetComponent<PlayerInventory>().UseItem();
                    }
                    else
                    {
                        player.GetComponent<PlayerController>().CastDragonSpell();
                        useCircIncreasing = false;  
                    }

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

    public void StartFillUseCircle(PlayerController player)
    {
        this.player = player;

        // If a spell quick select, and not enough dragon points, circle will not be able to be filled at all.
        if (player.GetComponent<PlayerValues>().currentDragonPoints >= player.GetComponent<PlayerSpell>().castDPCost || !isSpellQS)
        {
            useCircIncreasing = true;
        }
        else
        {
            //TODO: Notify player they cannot cast the spell e.g. a sound effect
        }
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
