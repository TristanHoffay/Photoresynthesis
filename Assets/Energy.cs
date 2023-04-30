using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Energy : MonoBehaviour
{
    [SerializeField]
    private int energy = 1;

    public static Energy Instance;

    private TextMeshProUGUI textMP;
    

    // Start is called before the first frame update
    void Start()
    {
        textMP = GetComponentInChildren<TextMeshProUGUI>();
        textMP.text = "Energy: " + energy;
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void UpdateEnergy(int newEnergy)
    {
        energy = newEnergy;
        textMP.text = "Energy: " + energy;
        Player.Instance.CheckOutline();
    }
    public void AddEnergy(int amount)
    {
        UpdateEnergy(energy + amount);
    }
    public bool UseEnergy(int amount, Vector3 location)
    {
        if (amount > energy)
            return false;
        else
        {
            energy -= amount;
            textMP.text = "Energy: " + energy;
            GameManager.Instance.CreateIndicator(location, "Energy -" + amount, -amount, 60, 0.3f);
            return true;
        }
    }
    // check if the given amount of energy can be used
    public bool CheckEnergyUse(int amount)
    {
        if (amount > energy)
            return false;
        else
        {
            return true;
        }
    }
}
