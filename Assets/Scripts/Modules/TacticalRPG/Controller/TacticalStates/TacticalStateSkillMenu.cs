using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Input;

public class TacticalStateSkillMenu : TacticalStateBase
{
    private SkillData selectedSkill;

    public TacticalStateSkillMenu(TacticalStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(TacticalStateBase previousState)
    {
        // Logic for entering the skill menu state
        Debug.Log("Entering Skill Menu State");

        selectedSkill = null;

        controller.tacticalMenu.ShowSkillMenu();

        UpdateRendering();
    }

    public override void CancelKey()
    {
        // Handle back event logic here
        Debug.Log("Back event triggered.");

        stateMachine.EnterState(stateMachine.unitActionState);
    }

    public override void OnClickButton(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < controller.selectedUnit.skills.Count)
        {
            selectedSkill = controller.selectedUnit.GetSkillByIndex(buttonIndex);
            Debug.Log("Selected Skill: " + selectedSkill.skillName);
        }
        else
        {
            Debug.LogWarning("Invalid skill button index: " + buttonIndex);
            selectedSkill = null;
        }

        UpdateRendering();
    }

    public override void Exit()
    {
        // Logic for exiting the skill menu state
        Debug.Log("Exiting Skill Menu State");
        controller.tacticalMenu.Hide();
    }

    public override void UpdateRendering()
    {
        // Reset tile illumination if no skill is selected
        foreach (Tile tile in controller.grid)
        {
            if (tile != null)
            {
                tile.ResetIllumination();
            }
        }

        if (selectedSkill != null)
        {
            // Highlight tiles based on the selected skill's range
            List<Tile> affectedTiles = selectedSkill.areaOfEffect.GetAffectedTiles(controller.selectedUnit.currentTile, controller);
            foreach (Tile tile in affectedTiles)
            {
                if (tile != null)
                {
                    tile.Illuminate(Color.blue); // Example color for skill range
                }
            }
        }
    }
}