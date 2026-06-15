using NUnit.Framework;
using TMPro;
using UnityEngine;

// NOTE: This will be the dedicated machine for calculating the player's score at the end of each level, taking into account which extensions are in use, how fast the player delivered the packages,
// and any other variables we may introduce later down the line. This does not necessarily need to exist as an object in-game, or be independent really (it can just exist as an instance, should probably
// be a singleton!), but whenever a score is needed, it will be found by utilizing this bad boy

public class ScoreCalculator : MonoBehaviour
{
    private TruckExtensionSelection truckExtensionSelect;
    private TruckExtensionsCoordinator truckExtensionsCoord;

    private void Start()
    {
        // NOTE: Invoke is used here as, because the TruckExtensionsCoordinator is linked to the Game Wizard at the same time the ScoreCalculator tries to call the Game Wizard to link itself to the
        // TruckExtensionsCoordinator, there needs to be a delay so that by the time the ScoreCalculator makes its call the TruckExtensionsCoordinator has already been assigned in the Game Wizard.
        // As for why the linking-up function is called indirectly through askGameWizardToLinkUp, this is because the ScoreCalculator needs to insert itself as an argument for linkUpScoreCalculator,
        // and Invoke, to my knowledge, cannot run functions with parameters. I'm sure there's a more efficient way to do this, but this'll work for now
        Invoke("askGameWizardToLinkUp", 0.05f);
    }

    // Sole purpose of running the linkUpScoreCalculator function. This is contained within this other function so that it can be used by Invoke
    private void askGameWizardToLinkUp() { GameWizard.instanceFetch.linkUpScoreCalculator(this); }

    public void setTruckExtensionSelection(TruckExtensionSelection setTruckExtensionSelect) { truckExtensionSelect = setTruckExtensionSelect; }
    public void setTruckExtensionsCoordinator(TruckExtensionsCoordinator setTruckExtensionsCoord) { truckExtensionsCoord = setTruckExtensionsCoord; }

    public float CalculateScore(float timeRemaining)
    {
        float score; // Guess what this is
        float baseScoreFromRemainingTime = timeRemaining; // The base score, as set by the remaining time
        float scoreDeductionFromExtensions = 0.0f; // The score deduction from adding up all extensions' point values

        // Calculation for score deduction
        foreach (TruckExtensionsCoordinator.Extension selectedExtension in truckExtensionSelect.ReturnSelectedExtensions())
        {
            scoreDeductionFromExtensions += truckExtensionsCoord.retrieveExtensionComponent(selectedExtension).pointCost;
        }
        scoreDeductionFromExtensions *= 1.25f;

        // Calculation of base score, from time remaining
        baseScoreFromRemainingTime *= 6.5f;

        // Final calculation of score
        score = baseScoreFromRemainingTime - scoreDeductionFromExtensions;

        return score;
    }
}
