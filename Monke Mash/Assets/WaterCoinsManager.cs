using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WaterCoinsManager : MonoBehaviour
{
    [Header("Currency Settings")]
    public TMP_Text currencyText; // Text (TMP) (UI) for currency display
    public int startingCurrency; // Initial currency amount on first playthrough
    public float countUpDuration = 1f; // Duration for currency count-up animation

    [Header("Daily Timer Settings")]
    public TMP_Text timerText; // Text (TMP) (NOT UI) for daily timer display
    public int claimDaysCooldown = 1; // Days for cooldown before claiming again

    [Header("Collision Detection Settings")]
    public Collider detectingCollider; // Collider for currency collection
    public string detectingTag; // Tag that triggers currency collection

    [Header("Outcome Settings")]
    public List<Outcome> outcomeList; // List of possible outcomes

    // Private Variables
    private int currency;
    private System.DateTime nextClaimTime;
    private bool firstClaim = true; // Flag for the first claim

    [System.Serializable]
    public struct Outcome
    {
        public int currencyAmount;
        public float delayBeforeGranting;
        public List<GameObject> objectsToEnableImmediately;
        public List<GameObject> objectsToEnableAfterDelay;
    }

    private void Start()
    {
        InitializeCurrency();
        InitializeTimer();
    }

    private void Update()
    {
        UpdateTimerDisplay();
    }

    private void InitializeCurrency()
    {
        if (!PlayerPrefs.HasKey("Currency"))
        {
            PlayerPrefs.SetInt("Currency", startingCurrency);
        }
        currency = PlayerPrefs.GetInt("Currency");
        currencyText.text = currency.ToString();
    }

    private void InitializeTimer()
    {
        if (PlayerPrefs.HasKey("NextClaimTime"))
        {
            long storedClaimTime = long.Parse(PlayerPrefs.GetString("NextClaimTime"));
            nextClaimTime = System.DateTime.FromBinary(storedClaimTime);
            firstClaim = false;
        }
        else
        {
            timerText.text = "0d 0h 0m 0s";
        }
    }

    private void UpdateTimerDisplay()
    {
        if (firstClaim || System.DateTime.Now >= nextClaimTime)
        {
            timerText.text = "0d 0h 0m 0s";
        }
        else
        {
            var timeRemaining = nextClaimTime - System.DateTime.Now;
            timerText.text = $"{timeRemaining.Days}d {timeRemaining.Hours}h {timeRemaining.Minutes}m {timeRemaining.Seconds}s";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(detectingTag) && (firstClaim || System.DateTime.Now >= nextClaimTime))
        {
            Outcome selectedOutcome = outcomeList[UnityEngine.Random.Range(0, outcomeList.Count)];

            // Enable immediate objects
            foreach (GameObject obj in selectedOutcome.objectsToEnableImmediately)
            {
                obj.SetActive(true);
            }

            // Start delayed granting of outcome
            StartCoroutine(GrantOutcomeWithDelay(selectedOutcome));

            SetNextClaimTime();
            firstClaim = false;
        }
    }

    private IEnumerator GrantOutcomeWithDelay(Outcome outcome)
    {
        yield return new WaitForSeconds(outcome.delayBeforeGranting);

        // Enable delayed objects
        foreach (GameObject obj in outcome.objectsToEnableAfterDelay)
        {
            obj.SetActive(true);
        }

        // Start counting up the currency
        int newCurrency = currency + outcome.currencyAmount;
        StartCoroutine(CountUpCurrency(currency, newCurrency, countUpDuration));
        currency = newCurrency;

        // Save the new currency amount
        PlayerPrefs.SetInt("Currency", currency);
    }

    private IEnumerator CountUpCurrency(int startAmount, int endAmount, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentAmount = Mathf.RoundToInt(Mathf.Lerp(startAmount, endAmount, elapsed / duration));
            currencyText.text = currentAmount.ToString();
            yield return null;
        }
        currencyText.text = endAmount.ToString();
    }

    private void SetNextClaimTime()
    {
        nextClaimTime = System.DateTime.Now.AddDays(claimDaysCooldown);
        PlayerPrefs.SetString("NextClaimTime", nextClaimTime.ToBinary().ToString());
    }

    [ContextMenu("Reset Player Data")]
    private void ResetData()
    {
        PlayerPrefs.DeleteKey("Currency");
        PlayerPrefs.DeleteKey("NextClaimTime");
        InitializeCurrency();
        InitializeTimer();
    }
}
